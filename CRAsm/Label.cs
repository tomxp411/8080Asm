using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRAsm
{
    public class Label
    {
        /// <summary>
        /// Name of the label (without the :)
        /// </summary>
        public string Name;
        /// <summary>
        /// Value of the label (address when defined in code or DB,DW,DS. Value when defined with EQU or SET)
        /// </summary>
        public UInt16 Value = 0x0;
        /// <summary>
        /// The source text on the right side of EQU statements. This could be anohter label or 
        /// a numeric value. 
        /// </summary>
        /// <example>
        /// HI: EQU 16h  ; Text is "16h"
        /// LO: EQU HI   ; Text is "HI"
        /// </example>
        private string _text = "";
        /// <summary>
        /// This label has been resolved to an actual value. 
        /// </summary>
        public bool Resolved;
        /// <summary>
        /// All the locations in the assembled code where the label is referenced. These will be filled with FFFFh
        /// in the first pass and resolved to their correct values in the second pass.
        /// </summary>
        //public List<int> References = new List<int>();
        public List<Instruction> References = new List<Instruction>();
        /// <summary>
        /// Line number where this label is first encountered. 
        /// </summary>
        public int LineNumber;

        public string Text
        {
            get => _text;
            set => _text = value;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();

            s.AppendFormat("{0,-10}  Value={1:X4}  Resolved={2}", this.Name, this.Value, this.Resolved);
            if (!string.IsNullOrEmpty(Text))
            {
                s.Append("  Text=");
                s.Append(this.Text);
            }
            if (References.Count > 0)
            {
                s.Append("  Refs=");
                //+ ", Value= " + this.Value.ToString("X") + ", Text=" + Text + ", Resolve=" + Resolved.ToString() + ", Locations= ");
                foreach (Instruction l in References)
                {
                    s.Append(l.CodePosition.ToString("X2") + ", ");
                }
            }
            return s.ToString();
        }
    }
}
