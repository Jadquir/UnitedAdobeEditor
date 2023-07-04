using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitedAdobeEditor.Components.Enums;
using UnitedAdobeEditor.Components.Scripts;

namespace UnitedAdobeEditor.Components.Classes
{
    public class PathList
    {
        public PathList()
        {
            foreach (AdobeType apptype in (AdobeType[])Enum.GetValues(typeof(AdobeType)))
            {
                selectedPaths[apptype] = Array.Empty<string>();
            }
        }
        public Dictionary<AdobeType, string[]> selectedPaths = new Dictionary<AdobeType, string[]>();

        public void Add(AdobeType appType,string path)
        {
            string[] oldPaths = selectedPaths[appType];

            if (oldPaths.Contains(path)) { return; }

            List<string> newList = oldPaths.ToList();
            newList.Add(path);

            selectedPaths[appType] = newList.ToArray();

            Settings.Instance.Save();
        }
        public string[] GetPaths(AdobeType appType)
        {
            return selectedPaths[appType];
        }
    }
}
