namespace PhotoshopExtractorByJadquir.Types
{
    public class PicInfoDetail
    {
        public int Index { get; set; }
        public int Offset { get; set; }
        public int Size { get; set; }
        public string FileType { get; set; }
        public bool IsValid => Size > 0 && Offset >= 8;
        public byte[] Data { get; set; }

        public PicInfoDetail(int index, int offset, int size, string fileType = "", byte[] data = null)
        {
            Index = index;
            Offset = offset;
            Size = size;
            FileType = fileType ?? ".bin";
            Data = data;
        }

        public override string ToString()
        {
            return $"Pic{Index}: Offset={Offset}, Size={Size}, Type={FileType}, Valid={IsValid}";
        }
    }

}
