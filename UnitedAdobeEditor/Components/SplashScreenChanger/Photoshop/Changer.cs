using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Components.Helpers;
using Wpf.Ui.Dpi;
using static UnitedAdobeEditor.Components.SplashScreenChanger.Photoshop.Changer.Files;

namespace UnitedAdobeEditor.Components.SplashScreenChanger.Photoshop
{
    public class Changer
    {
        public class Files
        {
            public static string ResourcePath
            {
                get
                {
                    string folder =CurrentOperation.SelectedPath.AdobeFolder + "\\Resources\\";
                    return folder;
                }
            }
            public static string BackupFolder
            {
                get
                {
                    string folder = Helpers.Files.AdobeAppSplashScreenBackup + "\\Backup\\";
                    if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
                    return folder;
                }
            }
            public static string ExtractFolder
            {
                get
                {
                    string folder = Helpers.Files.AdobeAppSplashScreenBackup + "\\Extracted\\";
                    if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
                    return folder;
                }
            }
            public static string PackedFolder
            {
                get
                {
                    string folder = Helpers.Files.AdobeAppSplashScreenBackup + "\\Packed\\";
                    if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
                    return folder;
                }
            }
        }


        private static readonly int WaitTime = 100;
        private static async Task<bool> Backup()
        {
            try
            {
                string text = Files.BackupFolder + "\\IconResources.idx";
                string text2 = Files.BackupFolder + "\\PSIconsHighRes.dat";
                string text3 = Files.BackupFolder + "\\PSIconsLowRes.dat";
                if (!File.Exists(text))
                {
                    File.Copy(Files.ResourcePath + "\\IconResources.idx", text, overwrite: false);
                }
                if (!File.Exists(text2))
                {
                    File.Copy(Files.ResourcePath + "\\PSIconsHighRes.dat", text2, overwrite: false);
                }
                if (!File.Exists(text3))
                {
                    File.Copy(Files.ResourcePath + "\\PSIconsLowRes.dat", text3, overwrite: false);
                }

                return true;
            }
            catch (Exception ex)
            {
                bool overrideBackup = false;
                await MessageBoxJ.ShowYesNo("Application couldn't backup the files.\nPlease manually backup your files ( PhotoshopFolder/Resources/ ).\n" +
                    "Error : " + ex.Message + ".\nAre you sure you want to continue?",
                    (s,e)=> { overrideBackup = true; }
                    );
                return overrideBackup;
            }
        }
        public static async void Change(Image image)
        {
            await Misc.RunBackground((s,e) => {
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

        private static string sender = "PhotoshopChanger";
        private static async void ChangeFunc(Image image)
        {
            LoadingStateController.SetState(LoadingStateController.State.Backup, sender);
            if (!(await Backup()))
            {
                Misc.SetStatePleaseWait();
                return;
            }

            Directory.Delete(ExtractFolder, true);
            Directory.Delete(PackedFolder, true);

            LoadingStateController.SetState(LoadingStateController.State.ExtractingFiles, sender);
            await Task.Delay(WaitTime);
            if (!(ExtractFile()))
            {
                Misc.SetStatePleaseWait();
                return;
            }
            LoadingStateController.SetState(LoadingStateController.State.ChangingImages, sender);
            await Task.Delay(WaitTime);
            if (!(SaveImages(image)))
            {
                Misc.SetStatePleaseWait();
                return;
            }
            LoadingStateController.SetState(LoadingStateController.State.PackingFiles, sender);
            await Task.Delay(WaitTime);
            if (!(PackFiles()))
            {
                Misc.SetStatePleaseWait();
                return;
            }
            LoadingStateController.SetState(LoadingStateController.State.CopyingFiles, sender);
            await Task.Delay(WaitTime);
            if (!(CopyFilesToPhotoshopFolder()))
            {
                Misc.SetStatePleaseWait();
                return;
            }


            LoadingStateController.SetState(LoadingStateController.State.ChangingColors, sender);
            ChangeTextColors();

            Misc.SetStatePleaseWait();
            MessageBoxJ.ShowOK("Successfully changed Photoshop Splash Screen.");
        }

        public static async void Restore()
        {
            await Misc.RunBackground((s, e) => {
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
        public static void RestoreFunc()
        {
            string errors = "";
            LoadingStateController.SetState(LoadingStateController.State.CopyingFiles, "Photoshop.Changer.Restore");
            foreach (string item in new List<string> { "\\IconResources.idx", "\\PSIconsHighRes.dat", "\\PSIconsLowRes.dat" })
            {
                try
                {
                    string sourceFileName = BackupFolder + item;
                    string text2 = ResourcePath + item;
                    if (File.Exists(text2))
                    {
                        File.Delete(text2);
                    }
                    File.Copy(sourceFileName, text2, overwrite: true);
                }
                catch (Exception ex)
                {
                    errors += ex.Message + "\n";
                }

            }
            Misc.SetStatePleaseWait();
            if (!String.IsNullOrEmpty(errors))
            {
                MessageBoxJ.ShowOK("Error while restoring files : " + errors);
                return;
            }
            MessageBoxJ.ShowOK("Successfully changed splash screen to the original!");
        }

        private static async void ChangeTextColors()
        {
            string valueToChange = "SplashBoxLoadingText";
            await ColorChanger.ColorChanger.ChangeInUIColors(valueToChange, SaveData.Instance.SplashScreenColors[SaveData.SplashScreenColor.TextColor]);
            
            valueToChange = "SplashBoxTextBackground"; 
            await ColorChanger.ColorChanger.ChangeInUIColors(valueToChange, SaveData.Instance.SplashScreenColors[SaveData.SplashScreenColor.BackgroundColor]);

            valueToChange = "SplashBoxLegalText"; 
            await ColorChanger.ColorChanger.ChangeInUIColors(valueToChange, 
                SaveData.Instance.SplashScreenColors[SaveData.SplashScreenColor.TextColor], true);
        }

        private static bool ExtractFile()
        {
            try
            {
                Func.Extract(ResourcePath, ExtractFolder);                
                return true;
            }
            catch (Exception ex)
            {
                MessageBoxJ.ShowOK("Extract Files Error : " + ex.Message);
            }
            return false;
        }

        private static string GetFileName(string directory)
        {
            if (Directory.Exists(directory))
            {
                string text = "Splash1080Background_s0.png";
                if (File.Exists(directory + "\\" + text))
                {
                    return text;
                }
                text = "SplashBackground_s0.png";
                if (File.Exists(directory + "\\" + text))
                {
                    return text;
                }
            }
            return null;
        }

        private static bool SaveImages(Image image)
        {
            try
            {
                string fileName = GetFileName(ExtractFolder + "\\High\\");
                File.Delete(ExtractFolder + "\\High\\" + fileName);
                File.Delete(ExtractFolder + "\\Low\\" + fileName);

                var high = image.ResizeImage(new Size(1500, 1000));
                var low = image.ResizeImage(new Size(750, 500));

                high.Save(ExtractFolder + "\\High\\" + fileName, ImageFormat.Png);
                low.Save(ExtractFolder + "\\Low\\" + fileName, ImageFormat.Png);

                high.Dispose();
                low.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                MessageBoxJ.ShowOK("Replace Splash Screen Error : " + ex.Message);
            }
            return false;
        }

        private static bool PackFiles()
        {
            try
            {
                Func.Pack(ExtractFolder, PackedFolder);

                return true;
            }
            catch (Exception ex)
            {
                MessageBoxJ.ShowOK("Packing Files Error : " + ex.Message);
                return false;
            }
          
        }
        private static bool CopyFilesToPhotoshopFolder()
        {
            try
            {
                CopyFile(PackedFolder + "\\IconResources.idx", ResourcePath + "\\IconResources.idx");
                CopyFile(PackedFolder + "\\PSIconsHighRes.dat", ResourcePath + "\\PSIconsHighRes.dat");
                CopyFile(PackedFolder + "\\PSIconsLowRes.dat", ResourcePath + "\\PSIconsLowRes.dat");
                return true;
            }
            catch (Exception ex)
            {
                MessageBoxJ.ShowOK(
                    "Couln't copy files to the photoshop folder.\nError: "+ ex.Message + "\n" +
                    $"You can copy manually from {PackedFolder} to {ResourcePath}"
                    );
            }
            return false;
        }
        private static void CopyFile(string SourcePath, string DestinationPath)
        {
            File.Delete(DestinationPath);
            File.Copy(SourcePath, DestinationPath, overwrite: true);
        }
    }
}
