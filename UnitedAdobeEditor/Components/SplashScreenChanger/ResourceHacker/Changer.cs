using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Xps.Packaging;
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Components.Classes.SplashScreenData;
using UnitedAdobeEditor.Components.Helpers;
using Vestris.ResourceLib;
using static UnitedAdobeEditor.Components.Classes.SplashScreenData.Main;
using static UnitedAdobeEditor.Views.Pages.SplashScreenChanger;
using ThreadState = System.Threading.ThreadState;

namespace UnitedAdobeEditor.Components.SplashScreenChanger.ResourceHacker
{
    public class Changer
    {
        static string exe
        {
            get
            {
                AdobeApp? app = Get(CurrentOperation.AppType);
                if (app == null)
                {
                    return null;
                }
                return app.FileName;
            }
        }
        static string BackupPath
        {
            get
            {
                return Files.AdobeAppSplashScreenBackup + "\\" + exe;
            }
        }
        static string SelectedFile
        {
            get
            {
                return CurrentOperation.SelectedPath.AdobeFolder + "\\" + exe;
            }
        }
        public static async void Restore()
        {
            await Misc.RunBackground((s,e)=> {
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
            LoadingStateController.SetState(LoadingStateController.State.CopyingFiles, "ResourceHacker.Changer.Restore");
            if (!File.Exists(BackupPath))
            {
                Debug.WriteLine("CurrentOp : " + CurrentOperation.AppType);
                Debug.WriteLine("Backup file should be in : " + BackupPath);
                Debug.WriteLine("Selected file should be in : " + SelectedFile);
                MessageBoxJ.ShowOK("Application couldn't find the backup files!");
                Misc.SetStatePleaseWait();
                return;
            }
            if (File.Exists(SelectedFile))
            {
                //File.Delete(SelectedFile);
            }

            File.Copy(BackupPath, SelectedFile, true);
            Misc.SetStatePleaseWait();
            MessageBoxJ.ShowOK("Successfully changed splash screen to original!");
        }
        
        public static async Task<bool> Backup()
        {
            if (File.Exists(BackupPath))
            {
                return true;
            }
            try
            {
                File.Copy(SelectedFile, BackupPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while backup : " + ex.Message);
                bool continueOp = false;
                await MessageBoxJ.ShowYesNo(
                    $"Application couldn't backup {exe}.\n Please backup {exe} file.\nDo you want to continue changing splash screen?",
                    (s, e) => { continueOp = true; });
                return continueOp;
            }
            return true;
        }

        private static string sender = "ResourceChanger";

        public static async void Change(Image image, ReplaceType replaceType)
        {
            await Misc.RunBackground((s,e)=> {
                try
                {
                    ChangeFunc(image ,replaceType); 
                }
                catch (Exception ex)
                {
                    Misc.SetStatePleaseWait();
                    MessageBoxJ.ShowOK("Error while changing splash screen : " + ex.Message);
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
            LoadingStateController.SetState(LoadingStateController.State.Backup, sender);
            if (!(await Backup()))
            {
                Misc.SetStatePleaseWait();
                return;
            }

            var value = new List<GenericResource>();

            LoadingStateController.SetState(LoadingStateController.State.ExtractingFiles, sender);
            //List<ResourceImageData> resourceData = UnitedAdobeEditor.Components.Classes.SplashScreenData.Main.ResourceDataKeys[CurrentOperation.AppType];
            List<ImageDataBase> resourceData = app.ResourceData;

            try
            {
                using (ResourceInfo vi = new ResourceInfo())
                {
                    vi.Load(CurrentOperation.SelectedPath.EXEFilePath);

                    // Find all the parent folder inside the resource
                    var id = vi.ResourceTypes.Find(x => x.Name == ((ResourceImageData)resourceData[0]).Parent);

                    // Get all the resource files from the parent if the name matches
                    var list = vi.Resources[id].FindAll(x => resourceData.Any(y => ((ResourceImageData)y).PossibleNames.ContainsName(x.Name.Name)));

                    foreach (GenericResource resource in list)
                    {
                        var data = resourceData.Find(x =>
                        {
                            var data = ((ResourceImageData)x);
                            var list = app.HasDarkVariant ? replaceType switch
                            {
                                ReplaceType.Both => data.PossibleNames,
                                ReplaceType.Dark => data.DarkVariantNames,
                                ReplaceType.Light => data.LightVariantNames,
                                _ => []
                            } : data.PossibleNames;
                            return list.ContainsName(resource.Name.Name);
                        });
                        if (data == null)
                        {
                            Debug.WriteLine("ResourceName : " + resource.Name.Name);
                            continue;
                        }
                        Debug.WriteLine("Changing " + resource.Name.Name);

                        var currentImage = resource.Data.ToImage();

                        // Use helper to handle resizing and padding
                        using var newImage = Extensions.PrepareReplacementImage(
                            image,          // replacement image
                            data.ImageSize,  // nominal target size
                            currentImage     // original image from resource
                        );

                        resource.Data = newImage.ToByteArray();
                        value.Add(resource);

                    }
                }
            }
            catch (Exception ex)
            {
                Misc.SetStatePleaseWait();
                MessageBoxJ.ShowOK("Error while modifying resources : " + ex.Message);
                return;
            }

            LoadingStateController.SetState(LoadingStateController.State.ChangingImages, sender);
            try
            {
                foreach (var item in value)
                {
                    item.SaveTo(CurrentOperation.SelectedPath.EXEFilePath);
                }
            }
            catch (Exception ex)
            {
                Misc.SetStatePleaseWait();
                MessageBoxJ.ShowOK("Error while replacing resources : " + ex.Message);
                return;
            }

            Misc.SetStatePleaseWait();
            MessageBoxJ.ShowOK("Successfully changed the Splash Screen.");
        }

        internal static Image? GetImage(bool requireHighResolution = false, bool isDarkMode = false)
        {
            AdobeApp? app = Get(CurrentOperation.AppType);
            if (app == null)
            {
                return null;
            }
            List<ImageDataBase> resourceData = app.ResourceData;

            try
            {
                using (ResourceInfo vi = new ResourceInfo())
                {
                    vi.Load(CurrentOperation.SelectedPath.EXEFilePath);

                    // Find all the parent folder inside the resource
                    var id = vi.ResourceTypes.Find(x => x.Name == ((ResourceImageData)resourceData[0]).Parent);

                    // Get all the resource files from the parent if the name matches
                    var list = vi.Resources[id].FindAll(x => resourceData.Any(y => ((ResourceImageData)y).PossibleNames.ContainsName(x.Name.Name)));

                    GenericResource? maxSizeSource = null;
                    foreach (GenericResource resource in list)
                    {

                        var data = resourceData.Find(x =>
                        {
                            var data = ((ResourceImageData)x);
                            var list = (app.HasDarkVariant && isDarkMode ? data.DarkVariantNames : data.LightVariantNames);
                            if (list.Any())
                            {
                                return list.ContainsName(resource.Name.Name);
                            }
                            return data.PossibleNames.ContainsName(resource.Name.Name);
                        });
                        if (data == null)
                        {
                            continue;
                        }
                        if (!requireHighResolution)
                        {
                            return resource.Data.ToImage();
                        }

                        if (maxSizeSource == null) maxSizeSource = resource;
                        else if (maxSizeSource.Size <= resource.Size)
                        {
                            maxSizeSource = resource;
                        }
                    }

                    if(maxSizeSource != null)
                    {
                        return maxSizeSource.Data.ToImage();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}
