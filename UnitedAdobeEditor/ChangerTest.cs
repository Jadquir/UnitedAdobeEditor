using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitedAdobeEditor.Components;
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Components.Helpers;
using UnitedAdobeEditor.Components.SplashScreenChanger.Photoshop;

using static UnitedAdobeEditor.ChangerTest.Files;
namespace UnitedAdobeEditor
{
    internal class ChangerTest
    {
        public class Files
        {
            public static string ResourcePath
            {
                get
                {
                    string folder = "C:\\Users\\Acer\\Desktop\\PhotoshopTest\\Original";
                    return folder;
                }
            }
            public static string ExtractFolder
            {
                get
                {
                    string folder = "C:\\Users\\Acer\\Desktop\\PhotoshopTest\\ExtractNew";
                    if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
                    return folder;
                }
            }
            public static string PackedFolder
            {
                get
                {
                    string folder = "C:\\Users\\Acer\\Desktop\\PhotoshopTest\\Modified";
                    if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
                    return folder;
                }
            }
        }
        public static async void ChangeFunc()
        {
            Directory.Delete(Files.ExtractFolder, true);
            Directory.Delete(Files.PackedFolder, true);

            var path = Path.Combine(ResourcePath, "IconResources.idx");
            //var hexEditor = new HexEditor(path);
            //var resourcesInfo = hexEditor.LoadResources();

            //var handler = new IconResourceHandler.IconResources(path);

            //var newImagePath = @"C:\Users\Acer\AppData\Roaming\Jadquir\United Adobe Editor\Cache\2C8hXM0KqlE.png";
            //var imageFull = Image.FromFile(newImagePath);
            //;
            //var resized = imageFull.ResizeImage(new System.Drawing.Size(750, 500));

            //var modifiedData = new Dictionary<string, Dictionary<int, byte[]>>
            //{
            //    { "high", new Dictionary<int, byte[]> { { 0, imageFull.ToByteArray() } } },
            //    { "low", new Dictionary<int, byte[]> { { 0, resized.ToByteArray() } } }
            //};

            //var instance = new IconResourceHandler.IconResources.ModifyIconInstance();
            //instance.IconName = "SplashBackground";
            //instance.modifiedData = modifiedData;

            //handler.ModifyIconData(ExtractFolder, instance);
            //resized.Dispose();
            //imageFull.Dispose();
            //handler.Load(ResourcePath);


            //handler.Extract(ExtractFolder);

            //handler.Pack(ExtractFolder);


            //PhotoshopExtractorJadquir.Default.CC2017.FilerFixed.Extract(ResourcePath, ExtractFolder);
            //PhotoshopExtractorJadquir.Default.CC2017.FilerFixed.Pack(ExtractFolder, PackedFolder);
            //PhotoshopExtractorJadquir.P2017.Filer.Extract(ResourcePath, ExtractFolder);
            //PhotoshopExtractorJadquir.P2017.Filer.AskResizeAsync = async () =>
            //{
            //    bool resize = false;
            //    await MessageBoxJ.ShowYesNo("The image you are trying is bigger in size than the original, we have to resize it to fit the splash screen!", (s,e) =>
            //    {
            //        resize = true;
            //    });
            //    return resize;
            //};
            //await PhotoshopExtractorJadquir.P2017.Filer.PackAsync(ResourcePath, ExtractFolder);
            //var manager = new IconResourceManager(ResourcePath);
            //manager.Extract(ResourcePath, ExtractFolder);
            //manager.Pack(ExtractFolder, PackedFolder);
            //Debug.WriteLine("Extract result: " + );

            //Debug.WriteLine("Pack result: " + PackFiles());
            //if (!(CopyFilesToPhotoshopFolder()))
            //{
            //    Misc.SetStatePleaseWait();
            //    return;
            //}
            
            
            
            Debug.WriteLine("Successfully changed Photoshop Splash Screen.");
        }

        private static bool PackFiles()
        {
            try
            {
                Func.New.Pack(ExtractFolder, PackedFolder);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Packing Files Error : " + ex.Message);
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
                    "Couln't copy files to the photoshop folder.\nError: " + ex.Message + "\n" +
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
        private static bool ExtractFile()
        {
            try
            {
                Func.New.Extract(ResourcePath, ExtractFolder);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Extract Files Error : " + ex.Message);
            }
            return false;
        }
    }
}
