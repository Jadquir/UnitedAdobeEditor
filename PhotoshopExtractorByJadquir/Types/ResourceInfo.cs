namespace PhotoshopExtractorByJadquir.Types
{
    public class ResourceInfo
    {
        public string IconName { get; set; }
        public Dictionary<string, ResolutionInfo> Resolutions { get; set; }

        public ResourceInfo(string iconName)
        {
            IconName = iconName;
            Resolutions = new Dictionary<string, ResolutionInfo>
            {
                { "low", new ResolutionInfo() },
                { "high", new ResolutionInfo() },
                { "xlow", new ResolutionInfo() },
                { "xhigh", new ResolutionInfo() }
            };
        }

        public ResolutionInfo GetResolution(string resolution)
        {
            return Resolutions.TryGetValue(resolution.ToLower(), out var res) ? res : null;
        }

        public bool HasValidPics(string resolution)
        {
            var res = GetResolution(resolution);
            return res?.Pics?.Any(p => p.IsValid) ?? false;
        }

        public int GetTotalValidPics()
        {
            return Resolutions.Values.Sum(r => r.Pics?.Count(p => p.IsValid) ?? 0);
        }
    }


}
