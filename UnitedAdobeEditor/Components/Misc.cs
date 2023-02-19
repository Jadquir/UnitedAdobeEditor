using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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
        private static string GetOnlyFolder(string path)
        {
            if (IsDirectory(path)) { return path; }
            string value = System.IO.Path.GetDirectoryName(path);
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
        }/*
        public static System.Drawing.Image ToDrawingImage(this BitmapImage img)
        {
            using(var ms = new MemoryStream())
            {
              //  ms.w
            }
            return null;
        }
        public static BitmapImage ToWpfImage(this System.Drawing.Image img)
        {
            MemoryStream ms = new MemoryStream();  // no using here! BitmapImage will dispose the stream after loading
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

            BitmapImage ix = new BitmapImage();
            ix.BeginInit();
            ix.CacheOption = BitmapCacheOption.OnLoad;
            ix.StreamSource = ms;
            ix.EndInit();
            ix.Freeze();
            return ix;
        }*/

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

