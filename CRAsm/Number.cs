using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRAsm
{
    /// defines a number in any numbering system (binary through hex)
    public struct Number
    {
        string sValue;
        UInt32 _value;
        UInt32 NumberBase;

        public Number(string v) : this()
        {
            sValue = "";
            _value = 0;
            NumberBase = 0;
            SetValue(v);
        }

        public UInt32 Value
        {
            get { return _value; }
            set
            {
                this._value = value;
                sValue = value.ToString();
                NumberBase = 10;
            }
        }

        public void SetValue(string v)
        {
            this.sValue = v;
            ComputeBase();
            ComputeValue();
        }

        public static UInt32 GetDigit(char c)
        {
            if (c >= '0' && c <= '9')
                return (UInt32)(c - '0');
            if (c >= 'A' && c <= 'F')
                return (UInt32)(c - 'A' + 10);
            if (c >= 'a' && c <= 'f')
                return (UInt32)(c - 'a' + 10);
            return UInt32.MaxValue;
        }

        public bool IsNumeric()
        {
            return this.NumberBase > 0;
        }

        public static bool IsNumeric(string v)
        {
            return IsNumeric(v, 16);
        }

        public static bool IsNumeric(string v, UInt32 NumberBase)
        {
            for (int i = 0; i < v.Length; i++)
            {
                UInt32 digit = GetDigit(v[i]);
                if (digit >= NumberBase)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// get the base numbering system (binary, octal, hex)
        /// </summary>
        /// <returns>The number base. 2=binary, 8=octal, 10=Decimal, 16=Hex</returns>
        public void ComputeBase()
        {
            NumberBase = 10;
            if (sValue.Length > 1 && sValue[0] == '$')
            {
                sValue = sValue.Substring(1);
                NumberBase = 16;
            }
            else if (sValue.Length > 2 && sValue.Substring(0, 2) == "0x")
            {
                sValue = sValue.Substring(2);
                NumberBase = 16;
            }
            else if (sValue.Length > 1 && sValue[sValue.Length - 1] == 'h')
            {
                sValue = sValue.Substring(0, sValue.Length - 1);
                NumberBase = 16;
            }
            else if (sValue.Length > 1 && sValue.ToLower()[sValue.Length - 1] == 'o' || sValue.ToLower()[sValue.Length - 1] == 'q')
            {
                sValue = sValue.Substring(0, sValue.Length - 1);
                NumberBase = 8;
            }

            if (!IsNumeric(sValue, NumberBase))
                NumberBase = 0;
        }

        private void ComputeValue()
        {
            UInt32 val = 0;
            for (int i = 0; i < sValue.Length; i++)
            {
                UInt32 d = GetDigit(sValue[i]);
                if (d >= NumberBase)
                {
                    this.NumberBase = 0;
                    return;
                }

                val = val * NumberBase;
                val = val + d;
            }

            this._value = val;
        }

    }
}
