using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRAsm
{
    /// <summary>
    /// 8080 Assembler
    /// Assemble 8080 opcodes using the standard Intel format. 
    /// Keywords:
    ///     ; comment. The ; and everything after is discarded.
    ///     ORG sets the address of the next opcode or data: START ORG 1000h 
    ///     DB: define bytes. Arguments can be numbers, characters, or strings.
    ///         Data is stored in BYTE ORDER.
    ///         space separated hex: 00 01 0F F0 
    ///         comma separated values: 12, 34h, 45o
    ///         characters: 'abc'
    ///         null-terminated string: "def"
    ///     DW: expects 2-byte words. Data is stored Little Endian order
    ///         DW 1234h assembles to 43 12 in the output.
    ///     DS: Reserves n bytes of memory for data
    ///         DS 20h ; reserve 32 bytes of memory for program data
    ///     =  : replace token with a literal value
    ///         PORT = 0H
    ///     EQU: replace token with a literal value
    ///         HOUSE EQU 1234h
    ///     SET: same as EQU
    ///         HOUSE SET 1234h
    ///     +-/*: basic arithmetic produces literal values of the result
    ///         1+2 will output 3
    ///         HOUSE+1 will output 1235h
    ///     IF/ENDIF - conditional assembly
    ///         TRUE EQU FFFFh ; 
    ///         FALSE EQU 0h   ;
    ///         TTY EQU TRUE   ;
    ///         IF TTY         ; any non-zero value will register as true
    ///         xxx
    ///         ELSEIF NOT TTY ; 16-bit bitwise NOT. Non-zero = true. 
    ///         xxx
    ///         ENDIF
    ///     
    /// Numbers: numeric values are interpreted as decimal by default
    ///     F0h, F0h, $F0, 360o and 240 all translate to the same binary value
    ///     of 11110000.
    /// 
    /// case is ignored outside of string literals. 
    ///     3f=3F=63
    ///     rrc = RRC
    /// 
    /// Labels 
    ///     BEGIN MVI A,12h
    ///     DATA: DS 100h
    ///     If a colon is present, the word is always a label. 
    ///     Otherwise, assembler assumes first word is a label
    ///     if the word is not a reserved word. Reserved words are
    ///     all opcodes and directives listed above.
    ///     
    /// Process: 
    ///     A well structured assembly line may look like:
    ///     line# label operation operand ;comment
    ///     Numbers at the start of the line are discarded.
    ///     Comments are discarded 
    ///     line is trimmed and whitespace removed.
    ///     
    ///     An operand may be x or x,y or x, y or x , y. 
    ///     Spaces are discarded to always make x or x,y
    ///     
    ///     Parts are dropped into slots for each part:
    ///     if first part has a :, it's a label
    ///     if first part is not a reserved word, it's a label
    ///     if next part is a reserved word, it's an operation
    ///     if next part exists, it's an operator
    ///     
    ///     Finally, opcodes are translated and populated in the output data
    /// </summary>
    public class Assembler8080
    {
        enum TokenHints
        {
            None,
            LineNumber,
            Label,
            Operator,
            Operand,
            String,
            Comment
        };

        private string CurrentLine;
        private int LineNumber;

        const string DIGITS = "0123456789ABCDEF";

        /// <summary>
        /// assembler source. 
        /// </summary>
        public string[] SourceLines = new string[]
            {
                "START:  ORG 000h      ;forward reference test",
                "VAR     EQU 02h       ;literal value",
                "        JMP LATER     ;forward reference test",
                "Label1: MVI A,F0h     ;comment text",
                "        LXI H, REF2   ; unresolved forward reference",
                "Label2  MOV B,A",
                "\t      MVI A, VAR    ;use that EQU we set earlier",
                "        ADD B",
                "        LDA 100H",
                "        ADD B",
                "        STA 101H",
                "        SHLD 1234h",
                "later:  HLT          ;forward reference resolution",
                "REF2:   DS 20h",
                "DATA    ORG 40h      ;start on an even 16 byte boundary",
                "HEXTST: DB 03h, 00h, 30h, 00h, 02h, 33h, 7Ah ; checksum should be 1Eh"
            };

        public string[] ReservedWords = new string[]
        {
            "NOP", "MOV", "ADD", "SUB", "ANA", "ORA", "RNZ", "RNC", "RPO", "RP",
            "LXI", "POP", "STAX", "SHLD", "STA", "JNZ", "JNC", "JPO", "JP",
            "INX", "JMP", "OUT", "XTHL", "DI", "INR", "CNZ", "CNC", "CPO", "CP",
            "DCR", "PUSH", "MVI", "HLT", "ORA", "ADI", "SUI", "ANI", "ORI",
            "RLC", "RAL", "DAA", "STC", "RST", "RZ", "RC", "RPE", "RM",
            "DAD", "RET", "PHCL", "SPHL", "LDAX", "LHLD", "LDA", "ADC", "SBB",
            "XRA", "CMP", "JZ", "JC", "JPE", "JM", "DCX", "IN", "XCHG", "EI",
            "CZ", "CC", "CPE", "CM", "CALL", "ACI", "SBI", "XRI", "CPI",
            "RRC", "RAR", "CMA", "CMC", "RST",
            "A", "B", "C", "D", "E", "H", "L", "SP", "PSW", "PC",
            "EQU", "SET", "ORG", "DB", "DW", "DS", "IF", "ELSE", "ELSEIF", "ENDIF",
            "="
        };

        /// <summary>
        /// Program output.
        /// </summary>
        public byte[] Data = new byte[0xFFFF];

        /// <summary>
        /// True if the starting address has been set or if any code has
        /// been assembled.
        /// </summary>
        private bool StartAddressValid = false;
        /// <summary>
        /// The starting address of the program output. The Hex output
        /// will start at this location when exporting the program
        /// output. 
        /// </summary>
        public int StartAddress;
        /// <summary>
        /// Length of the program output.
        /// </summary>
        private int _length;
        /// <summary>
        /// Current position in the assembled code. This is usually at the end,
        /// but an ORG statement can change the location being assembled to.
        /// To find the end of the assembled code, use Length.
        /// </summary>
        private UInt16 _codePosition;
        /// <summary>
        /// valid blocks of code in Ouptut. Values are
        /// StartByte,EndByte
        /// </summary>
        public Dictionary<int, int> Segments = new Dictionary<int, int>();

        /// <summary>
        /// Labels saved for forward references. 
        /// </summary>
        public Dictionary<string, Label> Labels = new Dictionary<string, Label>();

        private List<Instruction> Instructions = new List<Instruction>();
        public List<string> Listing = new List<string>();
        public List<string> Errors = new List<string>();
        public int ErrorCount = 0;

        public int EndAddress
        {
            get => _length;
        }

        public UInt16 CodePosition
        {
            get => _codePosition;
            set
            {
                _codePosition = value;
                if (value > _length)
                    _length = value;
            }
        }

        public string ListingText
        {
            get
            {
                StringBuilder s = new StringBuilder();
                foreach (string line in Listing)
                {
                    s.AppendLine(line);
                }
                return s.ToString();
            }
        }

        public void Assemble()
        {
            StartAddressValid = false;

            for (int i = 0; i < SourceLines.Length; i++)
            {
                CurrentLine = SourceLines[i];
                LineNumber = i;

                AssembleLine(SourceLines[i]);
            }

            Listing.Add(";");
            Listing.Add(";Labels");
            Listing.Add(";");
            foreach (Label label in Labels.Values)
            {
                Listing.Add(label.ToString());
            }

            Listing.Add(";");
            Listing.Add(";Pass 2");
            Listing.Add(";");

            // pass 2
            ResolveLabels();

            for (int i = 0; i < Instructions.Count; i++)
            {
                Listing.Add(Instructions[i].ToString());
            }

            // done. Now print the results
            PrintResults();
        }

        private void ResolveLabels()
        {
            foreach (Label l in Labels.Values)
            {
                if (!l.Resolved)
                {
                    LineNumber = l.LineNumber;
                    AddError("Unresolved Label: " + l.ToString());
                    continue;
                }

                //foreach (int r in l.References)
                //{
                //    this.Data[r] = (byte)(l.Value & 0xff);
                //    this.Data[r + 1] = (byte)((l.Value >> 8) & 0xff);
                //}
                for (int i = 0; i < l.References.Count; i++)
                {
                    Instruction inst = l.References[i];
                    GenerateOpcode(inst);
                    AppendOpcode(inst);
                }

            }
        }

        private void PrintResults()
        {
            int i = 0;
            Listing.Insert(i++, ErrorCount + " Errors");
            Listing.Insert(i++, "");
            if (ErrorCount > 0)
            {
                Listing.Insert(i++, ";");
                Listing.Insert(i++, "Errors");
                Listing.Insert(i++, ";");
                foreach (string s in Errors)
                {
                    Listing.Insert(i++, s);
                }
            }
        }

        private void AssembleLine(string line)
        {
            Instruction instr = new Instruction();
            Instructions.Add(instr);
            instr.CodePosition = this.CodePosition;
            ParseLine(line, instr);
            TranslateLabels(instr);
            GenerateOpcode(instr);
            AppendOpcode(instr);

            if (instr.Length > 0)
            {
                this.CodePosition += (UInt16)instr.Length;
                StartAddressValid = true;
            }
        }

        private void TranslateLabels(Instruction instr)
        {
            if (instr.Label == "")
                return;

            Label l;
            if (Labels.ContainsKey(instr.Label))
            {
                l = Labels[instr.Label];
                if (!l.Resolved)
                {
                    l.Value = CodePosition;
                    l.Resolved = true;
                }
            }
            else
            {
                l = new Label()
                {
                    Name = instr.Label.ToUpper(),
                    Value = CodePosition,
                    Resolved = true
                };
                Labels.Add(instr.Label, l);
            }

            // "EQU", "SET"   Set a literal value
            // "ORG"          Change the program address. Also sets label value to the address.
            // "DB", "DW"     Store values as bytes or words
            // "DS"           Create n bytes of blank memory (forward the program address by n bytes)
            // "IF", "ELSE", "ELSEIF", "ENDIF" Conditional assembly
            switch (instr.Command.ToUpper())
            {
                case "=":
                case "EQU":
                case "SET":
                    l.Text = instr.Operand;
                    l.Value = GetInt16(l.Text, instr);
                    break;
                case "ORG":
                    l.Text = instr.Operand;
                    UInt16 addr = GetInt16(l.Text, instr);
                    l.Value = addr;
                    break;
                default:
                    break;
            }
        }

        private void AppendOpcode(Instruction Instr)
        {
            int pos = Instr.CodePosition;
            for (int i = 0; i < Instr.Data.Count; i++)
            {
                this.Data[pos] = Instr.Data[i];
                pos += 1;
            }
        }

        private string RemoveComment(string line)
        {
            int lpos = line.IndexOf(';');
            if (lpos == 0)
                return "";
            return line.Substring(0, lpos - 1);
        }

        private void ParseLine(string Line, Instruction Instr)
        {
            bool inQuotes = false;
            char quoteChar = '\0';
            StringBuilder token = new StringBuilder();

            for (int i = 0; i < Line.Length; i++)
            {
                char c = Line[i];
                if (!inQuotes && c == ';') // start of comment
                {
                    HandleToken(Line.Substring(i), Instr, TokenHints.Comment);
                    break;
                }

                // start of a string literal.
                if (!inQuotes && (c == '\'' || c == '\"'))
                {
                    inQuotes = true;
                    quoteChar = c;
                    token.Append(c);
                }
                // end of a string literal. This is always an operand
                else if (inQuotes && c == quoteChar)
                {
                    inQuotes = false;
                    token.Append(c);
                    HandleToken(token.ToString(), Instr, TokenHints.Operand);
                    token.Clear();
                }
                else if (!inQuotes && (c == '='))
                {
                    if (token.Length > 0)
                        HandleToken(token.ToString(), Instr, TokenHints.Label);
                    token.Clear();
                    HandleToken("EQU", Instr, TokenHints.Operator);
                }
                else if (!inQuotes && (c <= ' ' || c == ',' || c == ':'))
                {
                    if (token.Length > 0)
                    {
                        if (c == ':') // word ending with : is always a label 
                            HandleToken(token.ToString(), Instr, TokenHints.Label);
                        else if (token[0] == '.') // psuedo-opcode, like .DB or .DW
                            HandleToken(token.ToString(), Instr, TokenHints.Operator);
                        else
                            HandleToken(token.ToString(), Instr, TokenHints.None);
                        token.Clear();
                    }
                }
                else
                    token.Append(c);
            }
            if (token.Length > 0)
                HandleToken(token.ToString(), Instr, TokenHints.None);

            Instr.Linenumber = (this.LineNumber + 1);
            //Instr.CodePosition = CodePosition;
        }

        private void HandleToken(string Token, Instruction Instr, TokenHints Hint)
        {
            // a word followed by a : is a label
            switch (Hint)
            {
                case TokenHints.LineNumber:
                    Instr.Linenumber = int.Parse(Token);
                    return;
                case TokenHints.Label:
                    Instr.Label = Token.ToUpper();
                    return;
                case TokenHints.Operator:
                    if (Token[0] == '.')
                        Instr.Command = Token.Substring(1);
                    else
                        Instr.Command = Token;
                    return;
                case TokenHints.Operand:
                    Instr.Operands.Add(Token);
                    return;
                case TokenHints.Comment:
                    Instr.Comment = Token;
                    return;
                case TokenHints.String:
                    Instr.Operand = Token;
                    return;
            }

            // a number at the beginning of the line is a line number. 
            if (Instr.Command == ""
                && Instr.Label == ""
                && Instr.Linenumber == 0
                && Token[0] >= '0' && Token[0] <= '9')
            {
                Instr.Linenumber = int.Parse(Token);
                return;
            }

            // reserved words are automatically commands
            if (Instr.Command == ""
                && ReservedWords.Contains(Token.ToUpper()))
            {
                Instr.Command = Token;
                return;
            }

            // if it's not a reserved word, it must be a label
            if (Instr.Command == "" && Instr.Label == "")
            {
                Instr.Label = Token.ToUpper();
                return;
            }

            // everything after the command is an operand 
            if (Instr.Command != "")
            {
                Instr.Operands.Add(Token);
                return;
            }

            // if we can't figure out where it goes, this is an error.
            AddError("Could not identify \"" + Token + "\"");
        }

        private void AddError(string Message)
        {
            StringBuilder s = new StringBuilder();
            s.Append((LineNumber + 1).ToString("D5") + ": ");
            if (LineNumber < SourceLines.Length)
                s.AppendLine(SourceLines[LineNumber]);
            s.AppendLine(Message);
            Errors.Add(s.ToString());
            ErrorCount += 1;
        }

        private void GenerateOpcode(Instruction Instr)
        {
            try
            {
                switch (Instr.Command.ToUpper())
                {
                    case "NOP":
                        Instr.Opcode = 0x00;
                        break;
                    case "LXI":
                        Instr.Opcode = GetRegBitsBDHSP(0x01, Instr.Operand);
                        Instr.DataWord = GetInt16(Instr.Operand2, Instr);
                        break;
                    case "STAX":
                        Instr.Opcode = GetRegOpcodes(Instr.Operand,
                            new string[] { "B", "D" },
                            new byte[] { 0x02, 0x12 });
                        break;
                    case "SHLD":
                        Instr.Opcode = 0X22;
                        Instr.DataWord = GetInt16(Instr.Operand, Instr);
                        break;
                    case "STA":
                        Instr.Opcode = 0X32;
                        Instr.DataWord = GetInt16(Instr.Operand, Instr);
                        break;

                    case "INX":
                        Instr.Opcode = GetRegBitsBDHSP(0x03, Instr.Operand);
                        break;
                    case "INR":
                        Instr.Opcode = GetRegBitsBDHMCELA(0x04, Instr.Operand);
                        break;
                    case "DCR":
                        Instr.Opcode = GetRegBitsBDHMCELA(0x05, Instr.Operand);
                        break;
                    case "MVI":
                        Instr.Opcode = GetRegBitsBDHMCELA(0x06, Instr.Operand);
                        Instr.DataByte = GetInt8(Instr.Operand2, Instr);
                        break;

                    case "RLC":
                        Instr.Opcode = 0x07;
                        break;
                    case "RAL":
                        Instr.Opcode = 0x17;
                        break;
                    case "DAA":
                        Instr.Opcode = 0x27;
                        break;
                    case "STC":
                        Instr.Opcode = 0x37;
                        break;

                    case "DAD":
                        Instr.Opcode = GetRegBitsBDHSP(0X09, Instr.Operand);
                        break;
                    case "LDAX":
                        Instr.Opcode = GetRegOpcodes(Instr.Operand, new string[] { "B", "D" }, new byte[] { 0x0A, 0x1A });
                        break;
                    case "LHLD":
                        Instr.Opcode = 0X2A;
                        Instr.DataWord = GetInt16(Instr.Operand, Instr);
                        break;
                    case "LDA":
                        Instr.Opcode = 0X3A;
                        Instr.DataWord = GetInt16(Instr.Operand, Instr);
                        break;
                    case "DCX":
                        Instr.Opcode = GetRegBitsBDHSP(0x0B, Instr.Operand);
                        break;

                    case "RRC":
                        Instr.Opcode = 0x0F;
                        break;
                    case "RAR":
                        Instr.Opcode = 0x1F;
                        break;
                    case "CMA":
                        Instr.Opcode = 0x2F;
                        break;
                    case "CMC":
                        Instr.Opcode = 0x3F;
                        break;

                    case "MOV":
                        // get the operation + dest register bits of the opcode
                        Instr.Opcode = GetRegBitsBDHMCELA(0x40, Instr.Operand);
                        // get the source register bits of the opcode
                        Instr.Opcode = GetRegBitsBCDEHLMA(Instr.Opcode, Instr.Operand2);
                        break;
                    case "HLT":
                        Instr.Opcode = 0x76;
                        break;
                    case "ADD":
                        Instr.Opcode = GetRegBitsBCDEHLMA(0x80, Instr.Operand);
                        break;
                    case "ADC":
                        Instr.Opcode = GetRegBitsBCDEHLMA(0x88, Instr.Operand);
                        break;
                    case "SUB":
                        Instr.Opcode = GetRegBitsBCDEHLMA(0x90, Instr.Operand);
                        break;
                    case "SBB":
                        Instr.Opcode = GetRegBitsBCDEHLMA(0x98, Instr.Operand);
                        break;
                    case "ANA":
                        Instr.Opcode = GetRegBitsBCDEHLMA(0xA0, Instr.Operand);
                        break;
                    case "XRA":
                        Instr.Opcode = GetRegBitsBCDEHLMA(0xA8, Instr.Operand);
                        break;
                    case "ORA":
                        Instr.Opcode = GetRegBitsBCDEHLMA(0xB0, Instr.Operand);
                        break;
                    case "CMP":
                        Instr.Opcode = GetRegBitsBCDEHLMA(0xB8, Instr.Operand);
                        break;

                    case "RNZ":
                        Instr.Opcode = 0xC0;
                        break;
                    case "rnc":
                        Instr.Opcode = 0xD0;
                        break;
                    case "RPO":
                        Instr.Opcode = 0xE0;
                        break;
                    case "RP":
                        Instr.Opcode = 0xF0;
                        break;

                    case "POP":
                        Instr.Opcode = GetRegBitsBDHPSW(0xC1, Instr.Operand);
                        break;
                    case "JNZ":
                        Instr.Opcode = 0xC2;
                        Instr.DataWord = GetInt16(Instr.Operand, Instr);
                        break;
                    case "JNC":
                        Instr.Opcode = 0xD2;
                        Instr.DataWord = GetInt16(Instr.Operand, Instr);
                        break;
                    case "JPO":
                        Instr.Opcode = 0xE2;
                        Instr.DataWord = GetInt16(Instr.Operand, Instr);
                        break;
                    case "JP":
                        Instr.Opcode = 0xF2;
                        Instr.DataWord = GetInt16(Instr.Operand, Instr);
                        break;

                    case "JMP":
                        Instr.Opcode = 0xC3;
                        Instr.DataWord = GetInt16(Instr.Operand, Instr);
                        break;
                    case "OUT":
                        Instr.Opcode = 0xD3;
                        Instr.DataByte = GetInt8(Instr.Operand, Instr);
                        break;
                    case "XTHL":
                        Instr.Opcode = 0xE3;
                        break;
                    case "DI":
                        Instr.Opcode = 0xF3;
                        break;

                    case "CNZ":
                        Instr.Opcode = 0xC4;
                        Instr.DataWord = GetInt16(Instr.Operand, Instr);
                        break;
                    case "CNC":
                        Instr.Opcode = 0xD4;
                        Instr.DataWord = GetInt16(Instr.Operand, Instr);
                        break;
                    case "CPO":
                        Instr.Opcode = 0xE4;
                        Instr.DataWord = GetInt16(Instr.Operand, Instr);
                        break;
                    case "CP":
                        Instr.Opcode = 0xF4;
                        Instr.DataWord = GetInt16(Instr.Operand, Instr);
                        break;

                    case "PUSH":
                        Instr.Opcode = GetRegBitsBDHPSW(0xC5, Instr.Operand);
                        break;

                    case "ADI":
                        Instr.Opcode = 0xC6;
                        Instr.DataByte = GetInt8(Instr.Operand, Instr);
                        break;
                    case "SUI":
                        Instr.Opcode = 0xD6;
                        Instr.DataByte = GetInt8(Instr.Operand, Instr);
                        break;
                    case "ANI":
                        Instr.Opcode = 0xE6;
                        Instr.DataByte = GetInt8(Instr.Operand, Instr);
                        break;
                    case "ORI":
                        Instr.Opcode = 0xF6;
                        Instr.DataByte = GetInt8(Instr.Operand, Instr);
                        break;

                    case "RST":
                        Instr.Opcode = GetRegOpcodes(Instr.Operand,
                            new string[] { "0", "2", "4", "6", "1", "3", "5", "7" },
                            new byte[] { 0xC7, 0xD7, 0xE7, 0xF7, 0xCF, 0xDF, 0xEF, 0xFF });
                        break;

                    case "RZ":
                        Instr.Opcode = 0xC8;
                        break;
                    case "RC":
                        Instr.Opcode = 0xD8;
                        break;
                    case "RPE":
                        Instr.Opcode = 0xE8;
                        break;
                    case "RM":
                        Instr.Opcode = 0xF8;
                        break;

                    case "RET":
                        Instr.Opcode = 0xC9;
                        break;
                    case "PCHL":
                        Instr.Opcode = 0xE9;
                        break;
                    case "SPHL":
                        Instr.Opcode = 0xF9;
                        break;

                    case "JZ":
                        Instr.Opcode = 0xCA;
                        Instr.DataWord = GetInt16(Instr.Operand, Instr);
                        break;
                    case "JC":
                        Instr.Opcode = 0xDA;
                        Instr.DataWord = GetInt16(Instr.Operand, Instr);
                        break;
                    case "JPE":
                        Instr.Opcode = 0xEA;
                        Instr.DataWord = GetInt16(Instr.Operand, Instr);
                        break;
                    case "JM":
                        Instr.Opcode = 0xFA;
                        Instr.DataWord = GetInt16(Instr.Operand, Instr);
                        break;

                    case "IN":
                        Instr.Opcode = 0xDB;
                        Instr.DataByte = GetInt8(Instr.Operand, Instr);
                        break;
                    case "XCHG":
                        Instr.Opcode = 0xEB;
                        break;
                    case "EI":
                        Instr.Opcode = 0xFB;
                        break;

                    case "CALL":
                        Instr.Opcode = 0xCD;
                        Instr.DataWord = GetInt16(Instr.Operand, Instr);
                        break;

                    case "ACI":
                        Instr.Opcode = 0xCE;
                        Instr.DataByte = GetInt8(Instr.Operand, Instr);
                        break;
                    case "SBI":
                        Instr.Opcode = 0xDE;
                        Instr.DataByte = GetInt8(Instr.Operand, Instr);
                        break;
                    case "XRI":
                        Instr.Opcode = 0xEE;
                        Instr.DataByte = GetInt8(Instr.Operand, Instr);
                        break;
                    case "CPI":
                        Instr.Opcode = 0xFE;
                        Instr.DataByte = GetInt8(Instr.Operand, Instr);
                        break;

                    // psuedo opcodes
                    case "=":
                    case "EQU":
                    case "SET":
                        break;
                    case "ORG":
                        Instr.CodePosition = GetInt16(Instr.Operand, Instr);
                        CodePosition = Instr.CodePosition;
                        if (!StartAddressValid)
                            StartAddress = CodePosition;
                        StartAddressValid = true;
                        break;
                    // todo: allow any length sequence of bytes. 
                    case "DB":
                        ConvertBytes(Instr, 8);
                        break;
                    case "DW":
                        ConvertBytes(Instr, 16);
                        break;
                    case "DS":
                        Instr.Length = GetInt16(Instr.Operand, Instr);
                        break;
                    case "":
                        break;
                    default:
                        AddError("Invalid command " + Instr.Command);
                        break;
                }
            }
            catch (Exception ex)
            {
                AddError("Could not process command " + Instr.Command
                    + "\r\n" + ex.Message
                    + "\r\n" + Instr.ToString());
            }
        }

        /// <summary>
        /// Gets bytes from character string or decodes hex string to byte data. Labels are 
        /// either decoded from their stored value or forward references are created. 
        /// </summary>
        /// <param name="operands">string to decode</param>
        /// <param name="Instr">instruction to add associate with created labels</param>
        /// <param name="WordSize">8 or 16 bits. 16-bit words are stored little-endian</param>
        /// <returns></returns>
        private void ConvertBytes(Instruction Instr, int WordSize)
        {
            Instr.Data.Clear();
            for (int i = 0; i < Instr.Operands.Count; i++)
            {
                string s = Instr.Operands[i];
                // " is null-terminated string, ' is character array
                if (s.StartsWith("\"") || s.StartsWith("\'"))
                {
                    byte[] chardata = ASCIIEncoding.ASCII.GetBytes(s.Substring(1, s.Length - 2));
                    Instr.Data.AddRange(chardata);
                    if (s.StartsWith("\""))
                        Instr.Data.Add(0x00);
                }
                else if (WordSize == 8)
                    Instr.Data.Add(GetInt8(s, Instr));
                else if (WordSize == 16)
                {
                    UInt16 value = GetInt16(s, Instr);
                    Instr.Data.Add((byte)(value & 0xFF));
                    Instr.Data.Add((byte)((value >> 8) & 0xFF));
                }
                else
                    throw new Exception("WordSize must be 8 or 16.");
            }
        }

        /// <summary>
        /// Converts string to a 16-bit number. If the string is a label name, this
        /// will attempt to lookup the label's value and return that. If the label
        /// has not yet been defined, this will create a forward reference and store
        /// Location in the forward reference to be fixed in the next pass. 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="Location"></param>
        /// <returns></returns>
        private UInt16 GetInt16(string v, Instruction Instr)
        {
            Label l;
            if (Labels.ContainsKey(v.ToUpper()))
            {
                l = Labels[v.ToUpper()];
                if (!l.Resolved)
                    l.References.Add(Instr);
                return l.Value;
            }

            try
            {
                string dec = v;

                if (v.StartsWith("'") && v.EndsWith("'"))
                    dec = ((int)v[1]).ToString();
                else if (v.StartsWith("$"))
                    dec = HexToDecimal(v.Substring(1));
                else if (v.ToLower().StartsWith("0x"))
                    dec = HexToDecimal(v.Substring(2));
                else if (v.ToLower().EndsWith("h"))
                    dec = HexToDecimal(v.Substring(0, v.Length - 1));
                else if (v.ToLower().EndsWith("o") || v.ToLower().EndsWith("q"))
                    dec = OctalToDecimal(v.Substring(0, v.Length - 1));

                if (dec.Length > 0 && dec[0] >= '0' && dec[0] <= '9')
                    return UInt16.Parse(dec);
            }
            catch (Exception ex)
            {
                AddError("Can't convert " + v + " to integer.\r\n" + ex.Message);
                return 0;
            }

            // if it's not a number, it's a forward reference. Create a label
            // and set it to not resolved. 
            l = new Label
            {
                Name = v.ToUpper(),
                Resolved = false,
                LineNumber = this.LineNumber,
                Text = ""
            };
            Labels.Add(l.Name, l);
            l.References.Add(Instr);
            l.Value = 0x0000;

            return l.Value;
        }

        private byte GetInt8(string v, Instruction Instr)
        {
            return (byte)(GetInt16(v, Instr) & 0xff);
        }

        private string HexToDecimal(string HexNum)
        {
            string s = HexNum.ToUpper();
            int ret = 0;
            for (int i = 0; i < s.Length; i++)
            {
                ret = ret << 4;
                char c = s[i];
                int bits = DIGITS.IndexOf(c);
                if (bits < 0 || bits > 15)
                    return "";
                ret = ret + bits;
            }
            return ret.ToString();
        }

        private string OctalToDecimal(string OctalNum)
        {
            int ret = 0;
            for (int i = 0; i < OctalNum.Length; i++)
            {
                ret = ret << 3;
                char c = OctalNum[i];
                // gets the value of the digit '0' = 0, '1'=1, etc.
                int bits = DIGITS.IndexOf(c);
                ret = ret + bits;
            }
            return ret.ToString();
        }

        /// <summary>
        /// Gets the bit value for the left operand of a MOV instruction. 
        /// You will need to use GetRegBitsBCDEHLMA to get the last part
        /// of the opcode for the right operand. 
        /// </summary>
        /// <param name="BaseBDHM">Base value (MOV starts at 0x40)</param>
        /// <param name="Register">Register name</param>
        /// <returns>Opcode for MOV plus left operand. </returns>
        private byte GetRegBitsBDHMCELA(byte BaseBDHM, string Register)
        {
            byte bits = 0;
            switch (Register.ToUpper())
            {
                case "B":
                    bits = (byte)(BaseBDHM + 0);
                    break;
                case "D":
                    bits = (byte)(BaseBDHM + 0x10);
                    break;
                case "H":
                    bits = (byte)(BaseBDHM + 0x20);
                    break;
                case "M":
                    bits = (byte)(BaseBDHM + 0x30);
                    break;
                case "C":
                    bits = (byte)(BaseBDHM + 0x8);
                    break;
                case "E":
                    bits = (byte)(BaseBDHM + 0x18);
                    break;
                case "L":
                    bits = (byte)(BaseBDHM + 0x28);
                    break;
                case "A":
                    bits = (byte)(BaseBDHM + 0x38);
                    break;
                default:
                    AddError("Invalid operand: " + Register);
                    break;
            }
            return bits;
        }

        /// <summary>
        /// Gets the last three bits of the opcodes that use any register
        /// as a source. 
        /// </summary>
        /// <param name="BaseVal">Base value of opcode. MOV starts at 0x40, ADD starts at 0x80, etc.</param>
        /// <param name="Register">Register on right side of operand (the source register)</param>
        /// <returns></returns>
        private byte GetRegBitsBCDEHLMA(byte BaseVal, string Register)
        {
            byte bits = 0;
            switch (Register.ToUpper())
            {
                case "B":
                    bits = 0x0;
                    break;
                case "C":
                    bits = 0x1;
                    break;
                case "D":
                    bits = 0x2;
                    break;
                case "E":
                    bits = 0x3;
                    break;
                case "H":
                    bits = 0x4;
                    break;
                case "L":
                    bits = 0x5;
                    break;
                case "M":
                    bits = 0x6;
                    break;
                case "A":
                    bits = 0x7;
                    break;
                default:
                    AddError("Invalid operand: " + Register);
                    break;
            }
            return (byte)(BaseVal + bits);
        }

        /// <summary>
        /// Gets register bits for operations that only use the B, D, H, and SP registers
        /// </summary>
        /// <param name="BaseVal">Base value of opcode. INX B is 0x03, INX D is 0x13, etc. 
        /// <param name="Register">Register on right side of operand (the source register)</param>
        /// <returns></returns>
        private byte GetRegBitsBDHSP(byte BaseVal, string Register)
        {
            byte bits = 0;
            switch (Register.ToUpper())
            {
                case "B":
                    bits = 0x00;
                    break;
                case "D":
                    bits = 0x10;
                    break;
                case "H":
                    bits = 0x20;
                    break;
                case "SP":
                    bits = 0x30;
                    break;
                default:
                    AddError("Invalid operand: " + Register);
                    break;
            }
            return (byte)(BaseVal + bits);
        }

        /// <summary>
        /// Gets register bits for operations that only use the B, D, H, and SP registers
        /// </summary>
        /// <param name="BaseVal">Base value of opcode. INR B is 0x04, INX D is 0x14, etc. 
        /// <param name="Register">Register on right side of operand (the source register)</param>
        /// <returns></returns>
        private byte GetRegBitsBDHM(byte BaseVal, string Register)
        {
            byte bits = 0;
            switch (Register.ToUpper())
            {
                case "B":
                    bits = 0x00;
                    break;
                case "D":
                    bits = 0x10;
                    break;
                case "H":
                    bits = 0x20;
                    break;
                case "M":
                    bits = 0x30;
                    break;
                default:
                    AddError("Invalid operand: " + Register);
                    break;
            }
            return (byte)(BaseVal + bits);
        }

        private byte GetRegBitsCELA(byte BaseVal, string Register)
        {
            byte bits = 0;
            switch (Register.ToUpper())
            {
                case "C":
                    bits = 0x00;
                    break;
                case "E":
                    bits = 0x10;
                    break;
                case "L":
                    bits = 0x20;
                    break;
                case "A":
                    bits = 0x30;
                    break;
                default:
                    AddError("Invalid operand: " + Register);
                    break;
            }
            return (byte)(BaseVal + bits);
        }

        private byte GetRegBitsBDHPSW(byte BaseVal, string Register)
        {
            byte bits = 0;
            switch (Register.ToUpper())
            {
                case "B":
                    bits = 0x00;
                    break;
                case "D":
                    bits = 0x10;
                    break;
                case "H":
                    bits = 0x20;
                    break;
                case "PSW":
                    bits = 0x30;
                    break;
                default:
                    AddError("Invalid operand: " + Register);
                    break;
            }
            return (byte)(BaseVal + bits);
        }

        /// <summary>
        /// Matches register names and a bit pattern. 
        /// </summary>
        /// <param name="BaseVal">Base value of opcode</param>
        /// <param name="Register">Register used in statement</param>
        /// <param name="Registers">List of valid registers for this operator</param>
        /// <param name="Opcodes">Final values for each operation</param>
        /// <returns></returns>
        private byte GetRegOpcodes(string Register, string[] Registers, byte[] Opcodes)
        {
            for (int i = 0; i < Registers.Length; i++)
            {
                if (Registers[i] == Register.ToUpper())
                {
                    return Opcodes[i];
                }
            }

            AddError("Could not match Operand " + Register);
            return 0;
        }


    }
}
