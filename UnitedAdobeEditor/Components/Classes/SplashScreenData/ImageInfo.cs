using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitedAdobeEditor.Components.Classes.SplashScreenData
{
    public struct ImageInfo
    {
        public static ImageInfo Dark(string name)
        {
            return new ImageInfo { Name = name, isDarkVariant = true };
        }
        public static ImageInfo Light(string name)
        {
            return new ImageInfo { Name = name, isDarkVariant = false };
        }
        public string Name;
        public bool isDarkVariant;
    }
    public static class ImageInfoExtensions
    {
        public static bool ContainsName(this IEnumerable<ImageInfo> list, string name) =>
            list.Any(x => x.Name == name);
    }

}
