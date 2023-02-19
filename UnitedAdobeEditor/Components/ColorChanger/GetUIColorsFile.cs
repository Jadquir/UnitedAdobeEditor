using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitedAdobeEditor.Components.ColorChanger
{
    public class GetUIColorsFile
    {
        public static (bool,string) GetPath(string path)
        {
            if (path == null || string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
            {
                return (false, "Selected path is empty.");
            }

            string text = path + "\\Required\\UIColors.txt";
            string text2 = path + "\\Contents\\Required\\UIColors.txt";
            string text3 = path + "\\Required\\UIColors.cel";
            string text4 = path + "\\Contents\\Required\\UIColors.cel";

            string value =  null;
            if (File.Exists(text))
            {
                value = text;
            }
            else if (File.Exists(text2))
            {
                value = text2;
            }
            else if (File.Exists(text3))
            {
                value = text3;
            }
            else if (File.Exists(text4))
            {
                value = text4;
            }

            if(value == null)
            {
                //value = Search(path);
            }

            if (value != null)
            {
                return (true, value);
            }

            return (false, "Application couldn't find UIColors.txt/.cel in your selected directory.");
        }

        private static string Search(string path)
        {
            var info = new DirectoryInfo(path);
            if (!info.Exists)
            {
                return null;
            }

            string[] allfiles = Directory.GetFiles(path, "UIColors.*", SearchOption.AllDirectories);

            if(allfiles.Length == 0) { return null; }

            string celFile = null;
            for (int i = 0; i < allfiles.Length; i++)
            {
                FileInfo fileInfo = new FileInfo(allfiles[i]);

                if(fileInfo.Extension == "cel") { celFile = fileInfo.FullName; }
                if(fileInfo.Extension == "txt") { return fileInfo.FullName; }
            }
            return celFile;
        }
    }
}
