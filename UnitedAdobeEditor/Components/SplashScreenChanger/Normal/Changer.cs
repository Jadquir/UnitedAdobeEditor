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
                    MessageBoxJ.ShowOK("Error while restoring splash screen : " + ex.Message);
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
                    }
                    File.Copy(fullPath, file, true);
                }
                catch (Exception ex)
                {
                    MessageBoxJ.ShowOK("Error while restoring splash screen : " + ex.Message);
                    error = true;
                }
            }

            Misc.SetStatePleaseWait();
            if (error)
            {
                return;
            }
            MessageBoxJ.ShowOK("Successfully changed splash screen to the original!");

        }
        private static string sender = "normal";
        public static async void Change(Image image)
        {
            await Misc.RunBackground((s, e) =>
            {
                try
                {
                    ChangeFunc(image);
                }
                catch (Exception ex)
                {
                    Misc.SetStatePleaseWait();
                    MessageBoxJ.ShowOK("Error while changing splash screen : " + ex.Message);
                }
            });
        }

        public static async void ChangeFunc(Image image)
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

                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }

                    var finalImage = image.ResizeImage(files[i].ImageSize);

                    finalImage.Save(file);
                    finalImage.Dispose();
                }
            }

            catch (Exception ex)
            {
                Misc.SetStatePleaseWait();
                MessageBoxJ.ShowOK("Error while changing Splash Screen : " + ex.Message);
                return;
            }
            Misc.SetStatePleaseWait();
            MessageBoxJ.ShowOK("Successfully changed the splash screen.");

        }
    }
}
