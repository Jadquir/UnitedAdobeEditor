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
            PossibleNames.Add(name);
        }
        public ResourceImageData(string name, string parent, Size imageSize, params string[] possibleNames) : base(imageSize, name)
        {
            Parent = parent;
            PossibleNames.AddRange(possibleNames);
            PossibleNames.Add(name);
        }
        public string Parent { get; set; }

        // Possible names implemented because Adobe sometimes changes the splash screen name in the resources
        public List<string> PossibleNames { get; set; } = new List<string>();
    }
}
