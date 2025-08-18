namespace PhotoshopExtractorByJadquir.Types
{
    public class ResolutionInfo
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public List<PicInfoDetail> Pics { get; set; }

        public ResolutionInfo()
        {
            Pics = new List<PicInfoDetail>();
        }

        public ResolutionInfo(IconData iconData)
        {
            Width = iconData.Width;
            Height = iconData.Height;
            X = iconData.X;
            Y = iconData.Y;
            Pics = new List<PicInfoDetail>();

            for (int i = 0; i < iconData.Pics.Count; i++)
            {
                var pic = iconData.Pics[i];
                Pics.Add(new PicInfoDetail(i, pic.Offset, pic.Size, pic.FileType));
            }
        }

        public IEnumerable<PicInfoDetail> GetValidPics()
        {
            foreach (var pic in Pics)
            {
                if (pic.IsValid)
                {
                    yield return pic;
                }
            }
        }
        public IEnumerable<int> GetValidPicIndexes()
        {
            for (int i = 0; i < Pics.Count; i++)
            {
                var pic = Pics[i];
                if (pic.IsValid)
                {
                    yield return i;
                }
            }
        }
    }

}
