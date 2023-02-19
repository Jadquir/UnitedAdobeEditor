using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Components.Classes.SplashScreenData;
using UnitedAdobeEditor.Components.Enums;
using UnitedAdobeEditor.Components.Scripts;
using static UnitedAdobeEditor.Components.Classes.SplashScreenData.Main;

namespace UnitedAdobeEditor.Components.Helpers
{
    public class PathSelector
    {
        public static SelectedPath Select(AdobeType type)
        {
            AdobeApp? app = Get(CurrentOperation.AppType);
            if (app == null)
            {
                return null;
            }
            string title = app.FileName;
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "Please select " + title; 
            if(app.PathSelectorComment != null)
            {
                dialog.Title += " (" + app.PathSelectorComment+")";
            }
            dialog.Filter = $"{title} ({title}) | {title}";
            dialog.FilterIndex = 0;
            dialog.RestoreDirectory = true;
            dialog.Multiselect = false;
            
            bool? @bool = dialog.ShowDialog();
            if (@bool.HasValue && @bool.Value)
            {
                string selectedFileName = dialog.FileName;
                
                Debug.WriteLine(selectedFileName);

                // D:\Adobe\Adobe Photoshop 2022\Photoshop.exe
                var path = new SelectedPath(selectedFileName);
                if (path.SuccessInit) 
                {
                    Settings.Instance.SelectedPaths.Add(type,path.EXEFilePath);
                    return path;

                }
            }
            return null;
        }
    }
}
