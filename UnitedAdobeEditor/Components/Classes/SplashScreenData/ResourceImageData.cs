using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitedAdobeEditor.Components.Classes.SplashScreenData
{
    public class ResourceImageData : ImageDataBase
    {
        public ResourceImageData(string name, string parent, Size imageSize) : base(imageSize, name)
        {
            Parent = parent;
            possibleNames.Add(name);
        }
        public ResourceImageData(string name, string parent, Size imageSize, params string[] possibleNames) : base(imageSize, name)
        {
            Parent = parent;
            this.possibleNames.AddRange(possibleNames);
            this.possibleNames.Add(name);
        }
        public string Parent { get; set; }


        List<string> possibleNames = new List<string>();
        List<string> possibleDarkVariantNames = new List<string>();
        // Possible names implemented because Adobe sometimes changes the splash screen name in the resources
        public IEnumerable<ImageInfo> PossibleNames
        {
            get
            {
                foreach (var item in possibleNames)
                {
                    yield return ImageInfo.Light(item);
                }
                foreach (var item in possibleDarkVariantNames)
                {
                    yield return ImageInfo.Dark(item);
                }
            }
        }
        public IEnumerable<ImageInfo> DarkVariantNames
        {
            get
            {
                foreach (var item in possibleDarkVariantNames)
                {
                    yield return ImageInfo.Dark(item);
                }
            }
        }
        public IEnumerable<ImageInfo> LightVariantNames
        {
            get
            {
                foreach (var item in possibleNames)
                {
                    yield return ImageInfo.Light(item);
                }
            }
        }

        public ResourceImageData AddDarkVariants(params string[] variants)
        {
            foreach (var item in variants)
            {
                if (string.IsNullOrEmpty(item)) continue;
                if (possibleDarkVariantNames.Contains(item)) continue;
                possibleDarkVariantNames.Add(item);
            }
            return this;
        }
        internal override bool HasDarkVariant() => possibleDarkVariantNames.Any();
    }
}
