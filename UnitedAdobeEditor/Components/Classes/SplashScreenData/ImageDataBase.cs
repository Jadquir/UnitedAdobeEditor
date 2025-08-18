using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitedAdobeEditor.Components.Classes.SplashScreenData
{
    public class ImageDataBase
    {
        public ImageDataBase(Size imageSize, string name)
        {
            ImageSize = imageSize;
            Name = name;
        }

        public Size ImageSize { get; set; }
        public string Name { get; set; }

        internal virtual bool HasDarkVariant() => false;
    }
}
