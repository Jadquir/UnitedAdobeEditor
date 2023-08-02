using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using UnitedAdobeEditor.Components.Enums;
using UnitedAdobeEditor.Components.Helpers;
using ThreadState = System.Threading.ThreadState;

namespace UnitedAdobeEditor.Components
{
    public class Misc
    {
        public static void OpenUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return;
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        public static bool IsProccessRunning(string proccessName)
        {
            Process[] pname = Process.GetProcessesByName(proccessName);
            return pname.Length != 0;
        }
        public static void SetStatePleaseWait()
        {
            LoadingStateController.SetState(LoadingStateController.State.PleaseWait, "misc");
        }
        public static void AssociateFile(string extension, string progId, string appName, string appPath)
        {
            // Create a new registry key for the file extension.
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(extension))
            {
                key.SetValue("", progId);
            }

            // Create a new registry key for the application.
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(progId))
            {
                key.SetValue("", appName);
                key.CreateSubKey("DefaultIcon").SetValue("", "\"" + appPath + "\",0");
                key.CreateSubKey(@"Shell\Open\Command").SetValue("", "\"" + appPath + "\" \"%1\"");
            }
        }
        public static Image? ImageFromBase64String(string base64String)
        {
            try
            {
                // Remove the "data:image/png;base64," part from the base64 string
                base64String = base64String.Replace("data:image/png;base64,", "");

                // Convert the base64 string to a byte array
                byte[] imageBytes = Convert.FromBase64String(base64String);

                // Create a MemoryStream from the byte array
                using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                {
                    // Create the image from the MemoryStream
                    return Image.FromStream(memoryStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
        private static string GetOnlyFolder(string path)
        {
            if (IsDirectory(path)) { return path; }
            string value = System.IO.Path.GetDirectoryName(path) ?? "";
            return value;

        }
        public static string GetFolderFromPath(string path)
        {
            string value = new DirectoryInfo(GetOnlyFolder(path)).FullName;
            return value;
        }
        public static string GetFolderName(string path)
        {
            string value = new DirectoryInfo(GetOnlyFolder(path)).Name;
            return value;
        }

        public static bool IsDirectory(string path)
        {
            FileAttributes attr = File.GetAttributes(@path);
            //detect whether its a directory or file
            return (attr & FileAttributes.Directory) == FileAttributes.Directory;
        }


        private static Dictionary<string, BitmapImage> LoadedImages = new Dictionary<string, BitmapImage>();
        public static BitmapImage ImageFromFile(string filepath)
        {
            if (LoadedImages.TryGetValue(filepath, out BitmapImage image)) { return image; }

            var bitmap = new BitmapImage();
            var stream = File.OpenRead(filepath);

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            stream.Close();
            stream.Dispose();
            bitmap.Freeze();
            LoadedImages[filepath] = bitmap;
            return bitmap;
        }
        public static BitmapImage ConvertDrawingImageToBitmapImage(System.Drawing.Image drawingImage)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Save the System.Drawing.Image to the MemoryStream in the desired format
                drawingImage.Save(memoryStream, ImageFormat.Png); // You can change the format if needed (e.g., JPEG, BMP, etc.)

                // Create a new BitmapImage
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // Freeze the BitmapImage to make it read-only and thread-safe

                return bitmapImage;
            }
        }
        public static BitmapImage ImageFromResource(string relativePath)
        {
            if (LoadedImages.TryGetValue(relativePath, out BitmapImage image)) { return image; }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri($"pack://application:,,,/{relativePath}");
                bitmap.EndInit();
                LoadedImages[relativePath] = bitmap;
                return bitmap;
            }
            catch (Exception ex)
            {
                // handle the exception here
                Debug.WriteLine("An error occurred FromResource: " + ex.Message);
                return null;
            }
        }
        public static async Task RunBackground(Task action)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, e) =>
            {
                Task.Factory.StartNew(() => action);
                //Task.Run(() => action);
            };

            worker.RunWorkerAsync();

        }
        public static async Task RunBackground(DoWorkEventHandler action)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += action;
            worker.RunWorkerAsync();

        }
    }
}

