using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoshopExtractorByJadquir.Types
{
    public class IconData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public List<PicInfo> Pics { get; set; }
        public IconData(int width, int height, int x, int y)
        {
            Width = width;
            Height = height;
            X = x;
            Y = y;
            Pics = new List<PicInfo>();
        }
    }

}
