using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using UnitedAdobeEditor.Components.ColorChanger;
using UnitedAdobeEditor.Components.Enums;
using Path = System.IO.Path;

namespace UnitedAdobeEditor.Components.Classes
{
    public class SelectedPath
    {
        public SelectedPath(string path, bool showMessageBox = true)
        {
            SuccessInit = false;
            if (!File.Exists(path))
            {
                return;
            }
            try
            {
                EXEFilePath = path;
                if (CurrentOperation.AppType == AdobeType.Photoshop ||
                    CurrentOperation.AppType == AdobeType.PhotoshopBeta
                    )
                {
                    (bool, string) path1 = GetUIColorsFile.GetPath(AdobeFolder);
                    if (!path1.Item1)
                    {
                        if (showMessageBox)
                        {
                            MessageBoxJ.ShowOK(path1.Item2);
                        }

                        return;
                    }
                    uiColorsPath = path1.Item2;
                }
                SuccessInit = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while selecting path : " + ex.Message);
                SuccessInit = false;
            }
            
        }
        
        public SelectedPath()
        {

        }
        public bool SuccessInit { get; private set; }
        
        public string Title 
        { 
            get 
            { 
                string title = Misc.GetFolderFromPath(EXEFilePath);

                int maxLoop = 30;
                int cur = 0;

                string final = null;
                if (!Misc.GetFolderName(title).Contains("Adobe"))
                {
                    while (true)
                    {
                        title = Misc.GetFolderFromPath(Path.GetFullPath(Path.Combine(title, @"..\")));

                        if (Misc.GetFolderName(title).Contains("Adobe")) 
                        {
                            Debug.WriteLine("folder name adobe");
                            final = title;
                            break; 
                        }
                        if (title.Length < 4)
                        {
                            Debug.WriteLine("path length too short");
                            break;
                        }
                        cur++;
                        if(cur >= maxLoop)
                        {
                            Debug.WriteLine("loop should end");
                            break;
                        }
                    }
                }
                if(final == null)
                {
                    final = Misc.GetFolderFromPath(EXEFilePath); 
                }
                return Misc.GetFolderName(final);
            } 
        }

        private string uiColorsPath;
        public string UIColorsPath {
            get
            {
                if (uiColorsPath == null)
                {
                    (bool, string) path1 = GetUIColorsFile.GetPath(AdobeFolder);
                    if (!path1.Item1)
                    {
                        MessageBoxJ.ShowOK(path1.Item2);
                        return null;
                    }
                    uiColorsPath = path1.Item2;
                }
                return uiColorsPath;
            }

        }
        public string AdobeFolder
        {
            get
            {
                return new DirectoryInfo(System.IO.Path.GetDirectoryName(EXEFilePath)).FullName;
            }
        }
        public string EXEFilePath;
    }
}
