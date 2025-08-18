namespace PhotoshopExtractorByJadquir.Types
{
    public class ModifyIconInstance
    {
        public string IconName { get; set; }
        public Dictionary<string, Dictionary<int, byte[]>> ModifiedData { get; set; }

        public ModifyIconInstance()
        {
            IconName = string.Empty;
            ModifiedData = new Dictionary<string, Dictionary<int, byte[]>>();
        }

        public ModifyIconInstance(string name)
        {
            IconName = name;
            ModifiedData = new Dictionary<string, Dictionary<int, byte[]>>();
        }

        public ModifyIconInstance(string name, Dictionary<string, Dictionary<int, byte[]>> data)
        {
            IconName = name;
            ModifiedData = data ?? new Dictionary<string, Dictionary<int, byte[]>>();
        }

        // Helper method to add data for a specific resolution and pic index
        public void AddPicData(string resolution, int picIndex, byte[] data)
        {
            if (picIndex < 0 || picIndex >= 8)
                throw new ArgumentOutOfRangeException(nameof(picIndex), "Pic index must be between 0 and 7");

            resolution = resolution.ToLower();
            if (!ModifiedData.ContainsKey(resolution))
                ModifiedData[resolution] = new Dictionary<int, byte[]>();

            ModifiedData[resolution][picIndex] = data;
        }


        // Helper method to add data from file
        public void AddPicDataFromFile(string resolution, int picIndex, string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var data = File.ReadAllBytes(filePath);
            AddPicData(resolution, picIndex, data);
        }

        // Helper method to remove data for a specific resolution and pic index
        public void RemovePicData(string resolution, int picIndex)
        {
            resolution = resolution.ToLower();
            if (ModifiedData.ContainsKey(resolution))
            {
                ModifiedData[resolution].Remove(picIndex);
                if (!ModifiedData[resolution].Any())
                    ModifiedData.Remove(resolution);
            }
        }

        // Helper method to check if specific pic data exists
        public bool HasPicData(string resolution, int picIndex)
        {
            resolution = resolution.ToLower();
            return ModifiedData.ContainsKey(resolution) && ModifiedData[resolution].ContainsKey(picIndex);
        }

        // Get all modified resolutions
        public string[] GetModifiedResolutions()
        {
            return ModifiedData.Keys.ToArray();
        }

        // Get modified pic indexes for a resolution
        public int[] GetModifiedPicIndexes(string resolution)
        {
            resolution = resolution.ToLower();
            return ModifiedData.ContainsKey(resolution) ? ModifiedData[resolution].Keys.ToArray() : new int[0];
        }

        // Get total count of modified pics across all resolutions
        public int GetTotalModifiedPics()
        {
            return ModifiedData.Values.Sum(dict => dict.Count);
        }

        public override string ToString()
        {
            return $"ModifyIconInstance: {IconName}, Modified pics: {GetTotalModifiedPics()}";
        }
    }
}
