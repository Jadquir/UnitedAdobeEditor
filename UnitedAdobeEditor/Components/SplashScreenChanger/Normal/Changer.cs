using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Components.Classes.SplashScreenData;
using UnitedAdobeEditor.Components.Helpers;
using static UnitedAdobeEditor.Components.Classes.SplashScreenData.Main;
using static UnitedAdobeEditor.Views.Pages.SplashScreenChanger;

namespace UnitedAdobeEditor.Components.SplashScreenChanger.Normal
{
    public class Changer
    {
        private static bool[] Backup(List<ImageDataBase> files)
        {
            var list = new bool[files.Count];
            for (int i = 0; i < files.Count; i++)
            {
                string file = ((NormalImageData)files[i]).Path;
                if (!File.Exists(file)) continue;
                string name = Path.GetFileName(file);
                Debug.WriteLine("file : " + file);
                Debug.WriteLine("name : " + name);
                string fullPath = Files.AdobeAppSplashScreenBackup + "\\" + name;
                try
                {
                    if (!File.Exists(fullPath))
                    {
                        File.Copy(file, fullPath, false);
                    }
                    list[i] = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error {i} : {ex.Message} ");
                    list[i] = false;
                }
            }
            return list;
        }
        public static async void Restore()
        {
            await Misc.RunBackground((s, e) =>
            {
                try
                {
                    RestoreFunc();
                }
                catch (Exception ex)
                {
                    Misc.SetStatePleaseWait();
                    MessageBoxJ.ShowOK("Failed to restore the splash screen: " + ex.Message);
                }
            });
        }
        private static void RestoreFunc()
        {
            AdobeApp? app = Get(CurrentOperation.AppType);
            if (app == null)
            {
                Misc.SetStatePleaseWait();
                return;
            }
            LoadingStateController.SetState(LoadingStateController.State.CopyingFiles, "Normal.Changer.Restore");
            List<ImageDataBase> files = app.ResourceData;

            bool error = false;
            for (int i = 0; i < files.Count; i++)
            {
                string file = ((NormalImageData)files[i]).Path;
                string name = Path.GetFileName(file);

                string fullPath = Files.AdobeAppSplashScreenBackup + "\\" + name;
                try
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                        File.Copy(fullPath, file, true);
                    }
                }
                catch (Exception ex)
                {
                    MessageBoxJ.ShowOK("Failed to restore splash screen: " + ex.Message);
                    error = true;
                }
            }

            Misc.SetStatePleaseWait();
            if (error)
            {
                return;
            }
            MessageBoxJ.ShowOK("Splash screen successfully restored to the original!");
        }
        private static string sender = "normal";
        public static async void Change(Image image, ReplaceType replaceType)
        {
            await Misc.RunBackground((s, e) =>
            {
                try
                {
                    ChangeFunc(image, replaceType);
                }
                catch (Exception ex)
                {
                    Misc.SetStatePleaseWait();
                    MessageBoxJ.ShowOK("Failed to change splash screen: " + ex.Message);
                }
            });
        }

        public static async void ChangeFunc(Image image, ReplaceType replaceType)
        {
            AdobeApp? app = Get(CurrentOperation.AppType);
            if (app == null)
            {
                Misc.SetStatePleaseWait();
                return;
            }
            List<ImageDataBase> files = app.ResourceData;

            //List<NormalImageData> files = Main.NormalDataKeys[CurrentOperation.AppType];
            if (files == null || files.Count < 1) { return; }

            LoadingStateController.SetState(LoadingStateController.State.Backup, sender);
            var arr = Backup(files);
            if (!Array.TrueForAll(arr, x => x == true))
            {
                bool continueChanging = false;
                await MessageBoxJ.ShowYesNo("Application couldn't create the backup.\nPlease backup your Splash Screen files.\nDo you want to continue changing the splash screen?",
                    (s, e) => { continueChanging = true; });

                if (!continueChanging)
                {
                    Misc.SetStatePleaseWait();
                    return;
                }
            }

            LoadingStateController.SetState(LoadingStateController.State.ChangingImages, sender);
            try
            {
                for (int i = 0; i < files.Count; i++)
                {
                    var file = ((NormalImageData)files[i]).Path;

                    Image? currentImage = null;
                    if (File.Exists(file))
                    {
                        try
                        {
                            // Load the image into memory, not keeping the file locked
                            using (var temp = Image.FromFile(file))
                            {
                                currentImage = new Bitmap(temp);
                            }
                        }
                        catch { }

                        // Now safe to delete the file
                        File.Delete(file);
                    }

                    if (currentImage == null) continue;

                    // Use helper to prepare the replacement image
                    using var finalImage = Extensions.PrepareReplacementImage(
                        image,                     // replacement image
                        files[i].ImageSize,        // nominal target size
                        currentImage               // original file image (can be null)
                    );

                    currentImage?.Dispose();

                    // Save the final image
                    finalImage.Save(file);
                }

            }

            catch (Exception ex)
            {
                Misc.SetStatePleaseWait();
                MessageBoxJ.ShowOK("Failed to change splash screen: " + ex.Message);
                return;
            }
            Misc.SetStatePleaseWait();
            MessageBoxJ.ShowOK("Splash screen successfully updated!");
        }
        internal static Image? GetImage(bool requireHighResolution = false)
        {
            AdobeApp? app = Get(CurrentOperation.AppType);
            if (app == null) return null;

            List<ImageDataBase> files = app.ResourceData;
            if (files == null || files.Count < 1) return null;

            string maxFile = string.Empty;
            long maxSize = 0;

            for (int i = 0; i < files.Count; i++)
            {
                var file = ((NormalImageData)files[i]).Path;
                if (!File.Exists(file)) continue;

                if (!requireHighResolution)
                {
                    using var temp = Image.FromFile(file);
                    return new Bitmap(temp); // safe, file is immediately released
                }

                var fileInfo = new FileInfo(file);
                if (fileInfo.Length > maxSize)
                {
                    maxSize = fileInfo.Length;
                    maxFile = file;
                }
            }

            if (!string.IsNullOrEmpty(maxFile))
            {
                using var temp = Image.FromFile(maxFile);
                return new Bitmap(temp); // safe
            }

            return null;
        }

    }
}
