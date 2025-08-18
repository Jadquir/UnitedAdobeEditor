using PhotoshopExtractorByJadquir;
using PhotoshopExtractorByJadquir.Types;
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
using System.Windows.Shapes;
using System.Xml.Linq;
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Components.Classes.SplashScreenData;
using UnitedAdobeEditor.Components.Helpers;
using static System.Net.Mime.MediaTypeNames;
using static UnitedAdobeEditor.Components.Classes.SplashScreenData.Main;
using static UnitedAdobeEditor.Components.SplashScreenChanger.Photoshop.Changer.Files;
using static UnitedAdobeEditor.Views.Pages.SplashScreenChanger;
using Image = System.Drawing.Image;
using Path = System.IO.Path;

namespace UnitedAdobeEditor.Components.SplashScreenChanger.Photoshop
{
    public static class Changer 
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


        public static readonly string[] IconFiles =
        [
            "IconResources.idx",
            "PSIconsHighRes.dat",
            "PSIconsLowRes.dat",
            "PSIconsXHighRes.dat",
            "PSIconsXLowRes.dat",
        ];

        private static readonly int WaitTime = 100;
        private static async Task<bool> Backup()
        {
            try
            {
                foreach (var item in IconFiles)
                {
                    string backup = Path.Combine(Files.BackupFolder, item);
                    var original = Path.Combine(Files.ResourcePath, item);
                    if (!File.Exists(backup))
                    {
                        File.Copy(original, backup, overwrite: false);
                    }
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
        public static async void Change(Image image, ReplaceType replaceType)
        {
            await Misc.RunBackground((s,e) => {
                try
                {
                    ChangeFuncNew(image, replaceType);
                }
                catch (Exception ex)
                {
                    Misc.SetStatePleaseWait();
                    MessageBoxJ.ShowOK("Error while changing splash screen : " + ex.Message);
                }
            });
        }

        private static readonly string sender = "PhotoshopChanger";

        static IEnumerable<string> GetPossibleNameList(List<ImageDataBase> resourceData)
        {
            var nameList = new List<string>();
            foreach (var item in resourceData)
            {
                var data = (ResourceImageData)item;
                if (data == null) { continue; }
                nameList.Add(data.Name);
                foreach (var name in data.PossibleNames)
                {
                    nameList.Add(name.Name);
                    break;
                }
            }
            return nameList.Distinct().Select(x => x.Replace("_s0", "").Replace(".png", "").Replace("_pic0_s", ""));
        } 
        private static async void ChangeFuncNew(Image image, ReplaceType replaceType)
        {
            AdobeApp? app = Get(CurrentOperation.AppType);
            if (app == null)
            {
                return ;
            }
            LoadingStateController.SetState(LoadingStateController.State.Backup, sender);
            if (!(await Backup()))
            {
                Misc.SetStatePleaseWait();
                return;
            }

            LoadingStateController.SetState(LoadingStateController.State.ChangingImages, sender);
            await Task.Delay(WaitTime);
            string idxPath = Path.Combine(ResourcePath, "IconResources.idx");

            // Load icon resources index
            var handler = new IconResources(idxPath);

            // Try to find splash screen resource
            var names = GetPossibleNameList(app.ResourceData);
            ResourceInfo? info = null;
            foreach (var name in names)
            {
                info = handler.GetResourceInfo(name);
                if (info == null) continue;
                break;
            }

            if (info == null)
            {
                Misc.SetStatePleaseWait();
                MessageBoxJ.ShowOK("Splash screen image could not be found. Adobe may have changed the resource name.");
                return;
            }

            // Cache to avoid resizing the same resolution multiple times
            var imageCache = new Dictionary<(int width, int height), byte[]>();

            var changeMap1 = new List<ModifyIconInstance>();
            var resourceInfo = handler.GetResourceInfo(info.IconName);
            var changeinstance = handler.CreateModifyInstance(info.IconName);
            
            foreach (var key in resourceInfo.Resolutions.Keys)
            {
                var resolution = resourceInfo.Resolutions[key];
                if (resolution.Width == 0 || resolution.Height == 0) { continue; }

                //byte[]? imageByte = null;
                //if (!imageCache.TryGetValue((resolution.Width, resolution.Height), out imageByte))
                //{
                //    imageByte = image.ResizeImage(resolution.Width, resolution.Height).ToByteArray();
                //    imageCache[(resolution.Width, resolution.Height)] = imageByte;
                //}

                foreach (var index in resolution.GetValidPicIndexes())
                {
                    // Step 1: get original pic data
                    var originalData = handler.GetPicData(info.IconName, key, index);

                    // Step 2: determine original image (if available)
                    Image? originalImage = null;
                    if (originalData != null && originalData.Length > 0)
                    {
                        using var ms = new MemoryStream(originalData);
                        originalImage = Image.FromStream(ms);
                    }

                    // Step 3: use helper to prepare replacement image
                    var cacheKey = originalImage != null ? (originalImage.Width, originalImage.Height) : (resolution.Width, resolution.Height);
                    if (!imageCache.TryGetValue(cacheKey, out var imageByte))
                    {
                        using var prepared = Extensions.PrepareReplacementImage(image, new Size(resolution.Width, resolution.Height), originalImage);
                        imageByte = prepared.ToByteArray();
                        imageCache[cacheKey] = imageByte;
                    }

                    // Step 4: inject into resource
                    changeinstance.AddPicData(key, index, imageByte);
                }

                changeMap1.Add(changeinstance);
            }

            // Apply modifications
            handler.ModifyIconDataEnhanced(PackedFolder, changeMap1.ToArray());

            imageCache.Clear();
            imageCache = null;

            LoadingStateController.SetState(LoadingStateController.State.CopyingFiles, sender);
            await Task.Delay(WaitTime);
            if (!(CopyFilesToPhotoshopFolder()))
            {
                Misc.SetStatePleaseWait();
                return;
            }

            LoadingStateController.SetState(LoadingStateController.State.ChangingColors, sender);
            await ChangeTextColors();

            Misc.SetStatePleaseWait();
            MessageBoxJ.ShowOK("Splash screen successfully updated.");
        }
        //private static async void ChangeFunc(Image image, bool isPhotoshopVersionLaterThan2024)
        //{
        //    LoadingStateController.SetState(LoadingStateController.State.Backup, sender);
        //    if (!(await Backup()))
        //    {
        //        Misc.SetStatePleaseWait();
        //        return;
        //    }

        //    Directory.Delete(ExtractFolder, true);
        //    Directory.Delete(PackedFolder, true);

        //    LoadingStateController.SetState(LoadingStateController.State.ExtractingFiles, sender);
        //    await Task.Delay(WaitTime);
        //    if (!(ExtractFile()))
        //    {
        //        Misc.SetStatePleaseWait();
        //        return;
        //    }
        //    LoadingStateController.SetState(LoadingStateController.State.ChangingImages, sender);
        //    await Task.Delay(WaitTime);
        //    if (!(SaveImages(image, isPhotoshopVersionLaterThan2024)))
        //    {
        //        Misc.SetStatePleaseWait();
        //        return;
        //    }
        //    LoadingStateController.SetState(LoadingStateController.State.PackingFiles, sender);
        //    await Task.Delay(WaitTime);
        //    if (!(PackFiles()))
        //    {
        //        Misc.SetStatePleaseWait();
        //        return;
        //    }
        //    LoadingStateController.SetState(LoadingStateController.State.CopyingFiles, sender);
        //    await Task.Delay(WaitTime);
        //    if (!(CopyFilesToPhotoshopFolder()))
        //    {
        //        Misc.SetStatePleaseWait();
        //        return;
        //    }


        //    LoadingStateController.SetState(LoadingStateController.State.ChangingColors, sender);
        //    await ChangeTextColors();

        //    Misc.SetStatePleaseWait();
        //    MessageBoxJ.ShowOK("Successfully changed Photoshop Splash Screen.");
        //}

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
            foreach (string item in IconFiles)
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
        private static readonly Dictionary<string, bool> SplashColors = new Dictionary<string, bool>()
        {
            // Value to Change : use TextColor
            {"SplashBoxLoadingText", true },
            {"AboutBoxTextAlt", true },
            {"AboutBoxFill", true },
            {"SplashBoxLegalText", true },
            { "BetaSplashText", true },
            { "BetaSplashFill", false },
            { "BetaAboutBoxText" , true },
            { "BetaAboutBoxFill" , false },
            { "SplashBoxText" , true },
            { "SplashBoxFill" , false },
            {"SplashBoxTextBackground", false },
            {"AboutBoxFillAlt", false },
        };
        private static async Task ChangeTextColors()
        {
            for (int i = 0; i < SplashColors.Count; i++)
            {
                var item = SplashColors.ElementAt(i);

                await ColorChanger.ColorChanger.ChangeInUIColors(
                    item.Key,
                    SaveData.Instance.SplashScreenColors
                    [
                        item.Value ? 
                        SaveData.SplashScreenColor.TextColor : 
                        SaveData.SplashScreenColor.BackgroundColor
                    ],
                    i + 1 == SplashColors.Count);

            }
        }

        private static bool ExtractFile()
        {
            try
            {
                //PhotoshopExtractorJadquir.P2017.Filer.Extract(ResourcePath, ExtractFolder);
                //Func.New.Extract(ResourcePath, ExtractFolder);                
                return true;
            }
            catch (Exception ex)
            {
                MessageBoxJ.ShowOK("Extract Files Error : " + ex.Message);
            }
            return false;
        }

        private static List<ResourceImageData> GetData(string directory)
        {
            if (Directory.Exists(directory))
            {
                AdobeApp? app = Get(CurrentOperation.AppType);
                if (app == null)
                {
                    Misc.SetStatePleaseWait();
                    return null;
                }
                return app.ResourceData.Cast<ResourceImageData>().ToList();
                //foreach (var item in app.ResourceData)
                //{
                //    var data = (ResourceImageData)item;
                //    if(data == null) continue;
                //    foreach (var name in data.PossibleNames)
                //    {
                //        if (File.Exists(directory + "\\" + name))
                //        {
                //            return data;
                //        }
                //    }
                //}
            }
            return null;
        }

        private static bool SaveImages(Image image, bool isPhotoshopVersionLaterThan2024)
        {
            try
            {
                AdobeApp? app = Get(CurrentOperation.AppType);
                if (app == null)
                {
                    return false;
                }
                var resourceData = app.ResourceData;


                foreach (var item in resourceData)
                {
                    var data = (ResourceImageData)item;
                    if (data == null) { continue; }
                    foreach (var name in data.PossibleNames)
                    {
                        var file = Path.Combine(ExtractFolder, data.Parent, name.Name);
                        if (!File.Exists(file)) continue;
                        File.Delete(file);
                        using var resized = image.ResizeImage(data.ImageSize);
                            resized.Save(file, ImageFormat.Png);
                        break;
                    }
                }
                //string fileName = "";
                //File.Delete(ExtractFolder + "\\High\\" + fileName);
                //File.Delete(ExtractFolder + "\\Low\\" + fileName);

                //var high = image.ResizeImage(new Size(1500, 1000));
                //var low = image.ResizeImage(new Size(750, 500));

                //high.Save(ExtractFolder + "\\High\\" + fileName, ImageFormat.Png);
                //low.Save(ExtractFolder + "\\Low\\" + fileName, ImageFormat.Png);

                //high.Dispose();
                //low.Dispose();
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
                //PhotoshopExtractorJadquir.P2017.Filer.Pack(ExtractFolder, PackedFolder);
                //Func.New.Pack(ExtractFolder, PackedFolder);

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
                foreach (var file in IconFiles)
                {
                    CopyFile(Path.Combine(PackedFolder, file), Path.Combine(ResourcePath, file));
                }
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
            if (!File.Exists(SourcePath)) return;
            File.Delete(DestinationPath);
            File.Copy(SourcePath, DestinationPath, overwrite: true);
        }

        internal static Image? GetImage(bool requireHighResolution = false)
        {
            AdobeApp? app = Get(CurrentOperation.AppType);
            if (app == null)
            {
                return null;
            }
            string idxPath = Path.Combine(ResourcePath, "IconResources.idx");

            // Load icon resources index
            var handler = new IconResources(idxPath);

            // Try to find splash screen resource
            var names = GetPossibleNameList(app.ResourceData);
            ResourceInfo? info = null;
            foreach (var name in names)
            {
                info = handler.GetResourceInfo(name);
                if (info == null) continue;
                break;
            }

            if (info == null)
            {
                return null;
            }

            var res = requireHighResolution ? "high" : "low";

            var resourceInfo = handler.GetResourceInfo(info.IconName);
            var validPicIndex = resourceInfo.Resolutions[res].GetValidPicIndexes().FirstOrDefault();
            
            return handler.GetPicData(info.IconName, res, validPicIndex).ToImage() ?? null;
        }
    }
}
