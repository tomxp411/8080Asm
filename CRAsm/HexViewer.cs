using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRAsm
{
    class HexDisplay
    {
        int DataWidth = 16;
        public string[] Lines = null;

        byte[] _data = null;
        public byte[] Data
        {
            get
            {
                return _data;
            }
        }

        public void GenerateText()
        {
            int dl = (int)(Data.Length / DataWidth) + 1;
            StringBuilder hexs = new StringBuilder();
            StringBuilder text = new StringBuilder();
            Lines = new string[dl];
            int pos = 0;
            int sPos = 0;
            byte b;

            for (int j = 0; j < dl; j++)
            {
                sPos = pos;
                for (int i = 0; i < DataWidth; i++)
                {
                    if (i % 8 == 0)
                        hexs.Append(" ");

                    b = 0;
                    if(pos < Data.Length)
                        b = Data[pos];
                    hexs.Append(b.ToString("X2"));
                    if (b >= 32 && b <= 127)
                        text.Append(Convert.ToChar(b));
                    else
                        text.Append(".");
                    hexs.Append(" ");

                    pos += 1;
                }

                Lines[j] = sPos.ToString("X4") + "  " + hexs.ToString() + "  " + text.ToString();
                hexs.Clear();
                text.Clear();
            }

            //while (i % DataWidth != 0)
            //{
            //    if (i % 8 == 0)
            //        hexs.Append(" ");

            //    hexs.Append("   ");
            //    i++;
            //}

            //if (ln >= 0)
            //{
            //    Lines[ln] = hexs.ToString() + "  " + text.ToString();
            //    hexs.Clear();
            //}
        }

        public void LoadData(byte[] data, int Length)
        {
            this._data = new byte[Length];
            Array.Copy(data, this.Data, Length);
            GenerateText();
        }
    }
}
