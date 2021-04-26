using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRAsm
{
    public class Instruction
    {
        public int Linenumber = 0;
        public string Label = "";
        public string Command = "";
        public List<string> Operands = new List<string>();

        public List<byte> Data = new List<byte>();
        public string Comment;
        public UInt16 CodePosition;

        // output listing in Hex, Dec, or Octal. Using Octal makes it easier
        // to verify the data on the Altair front panel
        //public static int NumberBase = 10;
        public static int NumberBase = 16;
        //public static int NumberBase = 8;

        /// <summary>
        /// Sets the length of this instruction (in bytes.) Setting this larger
        /// than the data already assigned will append 0x00 to the end. Setting
        /// this smaller will truncate any data already in the instruction. 
        /// </summary>
        public int Length
        {
            get
            {
                return Data.Count;
            }
            set
            {
                if (value < 0)
                    return;
                while (Data.Count < value)
                    Data.Add(0);
                while (Data.Count > value)
                    Data.RemoveAt(Data.Count - 1);
            }
        }

        public byte Opcode
        {
            get
            {
                if (Data.Count < 1)
                    return 0;
                return Data[0];
            }
            set
            {
                if (Data.Count < 1)
                    Data.Add((byte)value);
                Data[0] = (byte)value;
            }
        }

        public string Operand
        {
            get
            {
                if (Operands.Count < 1)
                    return "";
                return Operands[0];
            }
            set
            {
                if (Operands.Count < 1)
                    Operands.Add(value);
                Operands[0] = value;
            }
        }

        public string Operand2
        {
            get
            {
                if (Operands.Count < 2)
                    return "";
                return Operands[1];
            }
            set
            {
                while(Operands.Count < 2)
                    Operands.Add("");
                Operands[1] = value;
            }
        }

        public byte DataByte
        {
            get
            {
                if (Data.Count < 2)
                    return 0;
                return Data[1];
            }
            set
            {
                while (Data.Count < 2)
                    Data.Add(0);
                Data[1] = (byte)value;
            }
        }

        /// <summary>
        /// Sets or gets a Little Endian 16 bit value 
        /// </summary>
        public UInt16 DataWord
        {
            get
            {
                UInt16 val;
                if (Data.Count < 3)
                    return 0;
                val = (UInt16)(Data[2] << 8);
                val += Data[1];
                return val;
            }
            set
            {
                while (Data.Count < 3)
                    Data.Add(0);
                Data[1] = (byte)(value & 0xFF);
                Data[2] = (byte)((value >> 8) & 0xFF);
            }
        }

        public void AddByte(byte value)
        {
            Data.Add(value);
        }

        public void AddWord(UInt16 value)
        {
            Data.Add((byte)(value & 0xFF));
            Data.Add((byte)((value >> 8) & 0xFF));
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(Linenumber.ToString());
            while (s.Length < 6)
                s.Append(" ");
            s.Append(PrintDigits(CodePosition));
            while (s.Length < 14)
                s.Append(" ");

            if (!string.IsNullOrEmpty(Label))
                s.Append(Label + ":");
            while (s.Length < 22)
                s.Append(" ");

            s.Append(Command + " ");
            if (Operands.Count > 0)
            {
                //s.Append("{");
                for (int i = 0; i < Operands.Count; i++)
                {
                    if (i > 0)
                        s.Append(", ");
                    s.Append(Operands[i]);
                }
                //s.Append("}");
            }

            while (s.Length < 52)
                s.Append(" ");

            s.Append(";");
            for (int i = 0; i < Data.Count; i++)
            {
                if (i>0 && i%16==0)
                {
                    s.AppendLine();
                    s.Append(new string(' ', 52)+";");
                }
                s.Append(PrintDigits(Data[i]));
                s.Append(" ");
            }

            while (s.Length < 62)
                s.Append(" ");
            s.Append(Comment);
            return s.ToString();
        }

        const string digits = "0123456789ABCDEF";
        private string PrintDigits(ushort value)
        {
            string s = "";
            int comparitor = NumberBase - 1;
            int bits = 1;

            while(bits < ushort.MaxValue)
            {
                int a = value % NumberBase;
                s = digits[a].ToString() + s;
                value = (ushort)(value / NumberBase);
                bits = bits * NumberBase;
            }

            return s;
        }

        private string PrintDigits(byte value)
        {
            string s = "";
            int comparitor = NumberBase - 1;
            int bits = 1;

            while (bits < byte.MaxValue)
            {
                int a = value % NumberBase;
                s = digits[a].ToString() + s;
                value = (byte)(value / NumberBase);
                bits = bits * NumberBase;
            }

            return s;
        }

    }
}
