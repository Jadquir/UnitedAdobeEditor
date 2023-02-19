using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using UnitedAdobeEditor.Components.Classes;

namespace UnitedAdobeEditor.Components.Helpers
{
    public static class Files
    {
        public static string JadquirFolder
        {
            get
            {
                string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string folder = appdata + "\\Jadquir\\";
                var di = new DirectoryInfo(folder);
                if (!di.Exists)
                {
                    di.Create();
                }

                return di.FullName;
            }
        }
        public static string AppFolder
        {
            get
            {                
                string folder = JadquirFolder + "\\" + App.Name + "\\";
                var di = new DirectoryInfo(folder);
                if (!di.Exists)
                {
                    di.Create();
                }
                return di.FullName;
            }
        }

        public static string SettingsPath
        {
            get
            {
                return AppFolder + "\\Settings.json";
            }
        }

        public static string BackupFolder
        {
            get
            {
                string path= AppFolder + "\\ApplicationBackups\\";
                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;

            }
        }

        public static string UIColorsDataPath
        {
            get
            {
                return AppFolder + "\\UIColorsData.json";
            }
        }
        public static string SplashScreenDataPath
        {
            get
            {
                return  AppFolder + "\\SplashScreenData.json";
            }
        }
        public static string AdobeAppBackupFolder
        {
            get
            {
                string folder = BackupFolder + "\\" + CurrentOperation.SelectedPath.Title + "\\";
                if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
                return folder;
            }
        }
        public static string AdobeAppSplashScreenBackup
        {
            get
            {
                string folder = AdobeAppBackupFolder + "\\SplashScreenBackup\\";
                if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
                return folder;
            }
        }
    }
}
