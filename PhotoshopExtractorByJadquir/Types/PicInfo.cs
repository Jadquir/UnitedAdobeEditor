using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoshopExtractorByJadquir.Types
{
    public class PicInfo
    {
        public int Offset { get; set; }
        public int Size { get; set; }
        public string FileType { get; set; }
        public PicInfo(int offset, int size, string fileType = ".bin")
        {
            Offset = offset;
            Size = size;
            FileType = fileType;
        }
    }

}
