using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoshopExtractorByJadquir.Types
{
    internal class ResourceIcon
    {
        private const int NameSize = 48;
        private const int DataSize = 320;
        public string Key { get; set; }
        public IconData Low { get; set; }
        public IconData High { get; set; }
        public IconData XLow { get; set; }
        public IconData XHigh { get; set; }

        public ResourceIcon(byte[] buffer)
        {
            Key = Encoding.ASCII.GetString(buffer.Take(NameSize).ToArray()).TrimEnd('\0');
            var data = new List<int>();
            for (int j = 0; j < DataSize / 4; j++)
            {
                data.Add(BitConverter.ToInt32(buffer, NameSize + j * 4));
            }
            Low = new IconData(data[0], data[4], data[8], data[12]);
            for (int i = 0; i < 8; i++)
            {
                int offset = data[16 + i];
                int size = data[48 + i];
                Low.Pics.Add(new PicInfo(offset, size));
            }
            High = new IconData(data[1], data[5], data[9], data[13]);
            for (int i = 0; i < 8; i++)
            {
                int offset = data[24 + i];
                int size = data[56 + i];
                High.Pics.Add(new PicInfo(offset, size));
            }
            XLow = new IconData(data[2], data[6], data[10], data[14]);
            for (int i = 0; i < 8; i++)
            {
                int offset = data[32 + i];
                int size = data[64 + i];
                XLow.Pics.Add(new PicInfo(offset, size));
            }
            XHigh = new IconData(data[3], data[7], data[11], data[15]);
            for (int i = 0; i < 8; i++)
            {
                int offset = data[40 + i];
                int size = data[72 + i];
                XHigh.Pics.Add(new PicInfo(offset, size));
            }
        }

        public byte[] ToByteArray()
        {
            var buf = new List<byte>();
            var keyBytes = Encoding.ASCII.GetBytes(Key);
            buf.AddRange(keyBytes);
            for (int i = 0; i < NameSize - keyBytes.Length; i++)
            {
                buf.Add(0);
            }
            buf.AddRange(BitConverter.GetBytes(Low.Width));
            buf.AddRange(BitConverter.GetBytes(High.Width));
            buf.AddRange(BitConverter.GetBytes(XLow.Width));
            buf.AddRange(BitConverter.GetBytes(XHigh.Width));
            buf.AddRange(BitConverter.GetBytes(Low.Height));
            buf.AddRange(BitConverter.GetBytes(High.Height));
            buf.AddRange(BitConverter.GetBytes(XLow.Height));
            buf.AddRange(BitConverter.GetBytes(XHigh.Height));
            buf.AddRange(BitConverter.GetBytes(Low.X));
            buf.AddRange(BitConverter.GetBytes(High.X));
            buf.AddRange(BitConverter.GetBytes(XLow.X));
            buf.AddRange(BitConverter.GetBytes(XHigh.X));
            buf.AddRange(BitConverter.GetBytes(Low.Y));
            buf.AddRange(BitConverter.GetBytes(High.Y));
            buf.AddRange(BitConverter.GetBytes(XLow.Y));
            buf.AddRange(BitConverter.GetBytes(XHigh.Y));
            for (int i = 0; i < 8; i++) buf.AddRange(BitConverter.GetBytes(Low.Pics[i].Offset));
            for (int i = 0; i < 8; i++) buf.AddRange(BitConverter.GetBytes(High.Pics[i].Offset));
            for (int i = 0; i < 8; i++) buf.AddRange(BitConverter.GetBytes(XLow.Pics[i].Offset));
            for (int i = 0; i < 8; i++) buf.AddRange(BitConverter.GetBytes(XHigh.Pics[i].Offset));
            for (int i = 0; i < 8; i++) buf.AddRange(BitConverter.GetBytes(Low.Pics[i].Size));
            for (int i = 0; i < 8; i++) buf.AddRange(BitConverter.GetBytes(High.Pics[i].Size));
            for (int i = 0; i < 8; i++) buf.AddRange(BitConverter.GetBytes(XLow.Pics[i].Size));
            for (int i = 0; i < 8; i++) buf.AddRange(BitConverter.GetBytes(XHigh.Pics[i].Size));
            return buf.ToArray();
        }
    }

}
