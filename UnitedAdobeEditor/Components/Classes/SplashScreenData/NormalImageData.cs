using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitedAdobeEditor.Components.Classes.SplashScreenData
{
    public class NormalImageData : ResourceImageData
    {
        public NormalImageData(string name, string parent, Size imageSize) : base (name, parent, imageSize) 
        {

        }
        
        public string Path
        {
            get
            {
                string file = Name;
                if (!String.IsNullOrEmpty(Parent)) 
                {
                    file = $"{Parent}\\{Name}";
                }
                return $"{CurrentOperation.SelectedPath.AdobeFolder}\\{file}";
            }
        }
    }
}
