using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRAsm
{
    class HexFile
    {
        /// <summary>
        /// Amount of data reported on each line of the hex file. Should not be
        /// more than two hex digits (0xFF hex, 255 decimal). 
        /// </summary>
        int DataWidth = 16;
        public enum Formats
        {
            Display,
            Intel,
            //Motorola
        };

        public Formats Format = Formats.Display;

        byte[] _data = null;
        public byte[] Data
        {
            get
            {
                return _data;
            }
        }

        /// <summary>
        /// first address in data to export
        /// </summary>
        public int StartAddress;
        /// <summary>
        /// last address in data to export
        /// </summary>
        public int EndAddress;

        public string GetText(Formats NewFormat)
        {
            if (NewFormat == Formats.Intel)
                DataWidth = 32;
            else
                DataWidth = 16;

            this.Format = NewFormat;

            int pos = 0;
            int col = 0;
            int lineAddress = 0;
            byte[] bytes = new byte[DataWidth + 5];
            StringBuilder line = new StringBuilder();
            StringBuilder allText = new StringBuilder();

            pos = StartAddress;
            if (EndAddress >= Data.Length)
                EndAddress = Data.Length - 1;

            while (pos < EndAddress)
            {
                col = 0;
                lineAddress = pos;
                bytes[col++] = 0; // Length goes here
                bytes[col++] = (byte)(lineAddress >> 8 & 0xFF);
                bytes[col++] = (byte)(lineAddress & 0xFF);
                bytes[col++] = 0x00; // data record = 00

                while (col < bytes.Length - 1 && pos < EndAddress)
                {
                    bytes[col++] = Data[pos++];
                    bytes[0]++;
                }

                bytes[col] = GetChecksum(bytes, col);
                col++;

                if (NewFormat == Formats.Display)
                {
                    //allText.Append(bytes[0].ToString("X2"));
                    allText.Append(" ");
                    for (int i = 1; i < 3 && i < col; i++)
                    {
                        allText.Append(bytes[i].ToString("X2"));
                    }
                    allText.Append("  ");

                    for (int i = 4; i < DataWidth + 4; i++)
                    {
                        if ((i - 4) % 8 == 0)
                            allText.Append(" ");

                        if (i < col - 1)
                        {

                            allText.Append(bytes[i].ToString("X2"));
                            allText.Append(" ");
                        }
                        else
                        {
                            allText.Append("   ");
                        }
                    }

                    for(int i=4; i<DataWidth + 4; i++)
                    {
                        if ((i - 4) % 8 == 0)
                            allText.Append(" ");

                        if (i < col - 1 && bytes[i] >= 32 && bytes[i] <= 126)
                        {
                            allText.Append((char) bytes[i]);
                        }
                        else
                        {
                            allText.Append(".");
                        }
                    }
                    //allText.Append(bytes[col - 1].ToString("X2"));
                }
                else if (NewFormat == Formats.Intel)
                {
                    allText.Append(":");
                    for (int i = 0; i < col; i++)
                    {
                        allText.Append(bytes[i].ToString("X2"));
                    }
                }
                allText.AppendLine();
            }

            if (NewFormat == Formats.Intel)
            {
                allText.AppendLine(":00000001FF");
            }
            return allText.ToString();
        }

        // calculate the checksum of the bytes
        private byte GetChecksum(byte[] data, int Len)
        {
            if (Len > data.Length)
                Len = data.Length;
            int cs = 0;
            for (int i = 0; i < Len; i++)
            {
                cs = cs + data[i];
            }
            cs = -cs & 0xff;
            return (byte)cs;
        }

        public void LoadData(byte[] data, int StartAddress, int EndAddress)
        {
            this._data = data;
            this.StartAddress = StartAddress;
            this.EndAddress = EndAddress;
        }
    }
}
