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
        }
        public string Parent { get; set; }
    }
}
