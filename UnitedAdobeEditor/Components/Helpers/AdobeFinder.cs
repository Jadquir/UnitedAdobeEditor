using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Shapes;
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Components.Classes.SplashScreenData;
using UnitedAdobeEditor.Components.Enums;
using Path = System.IO.Path;

namespace UnitedAdobeEditor.Components.Helpers
{
    public static class AdobeFinder
    {
        static string GetAdobeDrive()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (!drive.IsReady) continue;

                string adobePath = Path.Combine(drive.RootDirectory.FullName, "Program Files", "Adobe");
                if (Directory.Exists(adobePath)) return drive.RootDirectory.FullName;

                adobePath = Path.Combine(drive.RootDirectory.FullName, "Program Files (x86)", "Adobe");
                if (Directory.Exists(adobePath)) return drive.RootDirectory.FullName;
            }
            return null;
        }

        static bool CanSelect(AdobeType adobeApp, string directory, out string path, int searchDepth = 2)
        {
            path = null;

            if (searchDepth < 0 || string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
                return false;

            var app = Main.Get(adobeApp);
            if (app == null) return false;

            // Check current directory
            var filePath = Path.Combine(directory, app.FileName);
            if (File.Exists(filePath))
            {
                path = directory;
                return true;
            }

            // Safely get subdirectories
            string[] subDirs;
            try
            {
                subDirs = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly);
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (PathTooLongException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }

            // Recursively search in subdirectories
            foreach (var dir in subDirs)
            {
                if (CanSelect(adobeApp, dir, out path, searchDepth - 1))
                {
                    return true;
                }
            }

            return false;
        }
        internal static async Task FindAppsAsync(AdobeType appType, Action<List<string>> callback)
        {
            await Task.Run(() =>
            {
                var app = Main.Get(appType);
                if(app == null) { return; }
                string adobeDrive = GetAdobeDrive();
                if (adobeDrive == null)
                {
                    Debug.WriteLine("Adobe folder not found.");
                    callback(new List<string>());
                    return;
                }

                string basePath = Path.Combine(adobeDrive, "Program Files", "Adobe");
                List<string> foundFolders = new List<string>();
                string appFolder = $"Adobe {appType.ToFriendlyString()} ";


                // Look for matching apps in this directory
                var matchingDirs = Directory.GetDirectories(basePath, appFolder + "*", SearchOption.TopDirectoryOnly);

                foreach (var item in matchingDirs)
                {
                    if (CanSelect(appType, item, out var selected, 1))
                    {
                        foundFolders.AddRange(selected);
                    }
                }

                callback(foundFolders);
            });
        }
    }

}
