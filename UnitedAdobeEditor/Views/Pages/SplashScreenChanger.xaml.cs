using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using UnitedAdobeEditor.Components;
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Components.Enums;
using UnitedAdobeEditor.Components.Firebase;
using UnitedAdobeEditor.Components.SplashScreenChanger;
using UnitedAdobeEditor.Views.CustomControls;
using static UnitedAdobeEditor.Components.Classes.SplashScreenData.Main;
using Image = System.Drawing.Image;

namespace UnitedAdobeEditor.Views.Pages
{
    /// <summary>
    /// SplashScreenChanger.xaml etkileşim mantığı
    /// </summary>
    public partial class SplashScreenChanger : Page
    {
        public enum ReplaceType
        {
            Both,
            Dark,
            Light
        }
        public SplashScreenChanger()
        {
            InitializeComponent();
            colorboxes.Visibility = Visibility.Collapsed;
            colorboxesText.Visibility = Visibility.Collapsed;
            if (CurrentOperation.AppType == AdobeType.Photoshop || CurrentOperation.AppType == AdobeType.PhotoshopBeta)
            {
                colorboxes.Children.Clear();

                static AdvancedColorBox Create(SaveData.SplashScreenColor ssc)
                {
                    return new AdvancedColorBox(SaveData.Instance.SplashScreenColors[ssc]);
                }
                colorboxes.Children.Add(BGColor = Create(SaveData.SplashScreenColor.BackgroundColor));
                colorboxes.Children.Add(FGColor = Create(SaveData.SplashScreenColor.TextColor));
                colorboxes.Visibility = Visibility.Visible;
                colorboxesText.Visibility = Visibility.Visible;
            }
            SaveData.Load();
            imageSelector.ExportDefaultImage = ExportDefaultImage;
            if (CurrentOperation.IsConfigActivated && CurrentOperation.Operation != null)
            {
                if(CurrentOperation.Operation.SplashScreen is null)
                {
                    MessageBoxJ.ShowOK("Something went wrong while loading config image!");
                }
                else
                {
                    imageSelector.SetSelectedImage(CurrentOperation.Operation.SplashScreen);
                }
            }
            imageSelector.SetPreviewImage(null);


            //darkModeAlternativeSelect.Items = SplashScreenReplaceType;
            darkModeAlternativePanel.Visibility = Visibility.Collapsed;
            var app = Get(CurrentOperation.AppType);
            if(app != null && app.HasDarkVariant && CurrentOperation.GetAppYear() >= 2025)
            {
                darkModeAlternativePanel.Visibility = Visibility.Visible;
            }


            this.Loaded += SplashScreenChanger_Loaded;


        }

        private void SplashScreenChanger_Loaded(object sender, RoutedEventArgs e)
        {
            if (!CurrentOperation.IsConfigActivated)
            {
                var selectedPath = CurrentOperation.SelectedPath.EXEFilePath;
                if (imageCache.TryGetValue(selectedPath, out var cachedImage))
                {
                    imageSelector.SetPreviewImage(cachedImage);
                }
                else
                {
                    _ = LoadPreviewImageAsync();
                }
            }
        }

        public void ExportDefaultImage()
        {
            // Get the image (as BitmapSource)
            var image =  GetImage(true).ConvertToBitmapSource();
            if (image == null)
            {
                MessageBoxJ.ShowOK("No image to export.");
                return;
            }

            // Open save file dialog
            var saveDialog = new SaveFileDialog
            {
                Filter = "PNG Image (*.png)|*.png",
                DefaultExt = ".png",
                FileName = CurrentOperation.SelectedPath.Title + " Splash Screen.png",
                Title = "Save Splash Screen"
            };

            if (saveDialog.ShowDialog() == true)
            {
                string path = saveDialog.FileName;

                // Create metadata
                var metadata = new BitmapMetadata("png");
                metadata.Title = CurrentOperation.SelectedPath.Title + " Splash Screen";
                metadata.Author = new System.Collections.ObjectModel.ReadOnlyCollection<string>(new string[] { "United Adobe Editor" });
                metadata.Comment = "Exported using United Adobe Editor";

                // Create a new BitmapFrame with metadata
                var frame = BitmapFrame.Create(image, null, metadata, null);

                // Save the image
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(frame);
                    encoder.Save(stream);
                }

                MessageBoxJ.ShowOK($"Image saved successfully at:\n{path}");
            }
        }
        private async Task LoadPreviewImageAsync()
        {
            var currentImage = await Task.Run(() => GetImage());
            if (currentImage != null)
            {
                imageSelector.SetPreviewImage(currentImage);
            }
        }
        static void RemoveCurrentCache()
        {
            try
            {
                var selectedPath = CurrentOperation.SelectedPath.EXEFilePath;
                imageCache.Remove(selectedPath);

            }
            catch (System.Exception)
            {

            }
        }

        static readonly Dictionary<string, Image> imageCache = new Dictionary<string, Image>();
        bool IsDarkMode;
        Image? GetImage(bool requireHighResolution = false)
        {
            try
            {
                var selectedPath = CurrentOperation.SelectedPath.EXEFilePath;
                var app = Get(CurrentOperation.AppType);
                if (app == null) return null;

                if (!requireHighResolution && imageCache.TryGetValue(selectedPath, out var img) && img != null)
                    return img;

                ChangerType type = app.SplashScreenChangerType;
                Image? image = type switch
                {
                    ChangerType.Photoshop => Components.SplashScreenChanger.Photoshop.Changer.GetImage(requireHighResolution),
                    ChangerType.ResourceHacker => Components.SplashScreenChanger.ResourceHacker.Changer.GetImage(requireHighResolution, IsDarkMode),
                    ChangerType.Normal => Components.SplashScreenChanger.Normal.Changer.GetImage(requireHighResolution),
                    _ => null,
                };
                if(image != null)
                {
                    imageCache[selectedPath] = image;
                }

                return image;
            }
            catch (System.Exception)
            {
                return null;
            }
          
        }
        private AdvancedColorBox BGColor;
        private AdvancedColorBox FGColor;
        public static bool Change(AdobeType AppType, Image? image, ReplaceType replaceType)
        {
            if (image == null)
            {
                MessageBoxJ.ShowOK("Please select a splash screen first by clicking on the image.");
                return false;
            }
            AdobeApp? app = Get(AppType);
            if (app == null)
            {
                return false;
            }
            if (Misc.IsProccessRunning(app.FileName))
            {
                MessageBoxJ.ShowOK($"Adobe Application ({app.FileName}) is running. Please close the application and try again.");

                return false;
            }
            SaveData.Save();
            RemoveCurrentCache();
            MainWindow.Instance.Navigate(Components.Enums.PageEnum.Loading);
            ChangerType type = app.SplashScreenChangerType;
            switch (type)
            {
                case ChangerType.Photoshop:
                    Components.SplashScreenChanger.Photoshop.Changer.Change(image, replaceType);
                    break;
                case ChangerType.ResourceHacker:
                    Components.SplashScreenChanger.ResourceHacker.Changer.Change(image, replaceType);
                    break;
                case ChangerType.Normal:
                    Components.SplashScreenChanger.Normal.Changer.Change(image, replaceType);
                    break;
                default:
                    break;
            }
            if (CurrentOperation.isRunStat && !string.IsNullOrEmpty(CurrentOperation.runStatId))
            {
                _ = PublicConfigManager.UpdateStat(CurrentOperation.runStatId, PublicConfigManager.StatType.run_config);
            }
            CurrentOperation.isRunStat = false;
            CurrentOperation.runStatId = "";
            CurrentOperation.IsConfigActivated = false;
            return true;
        }
        private void applyButton_Click(object sender1, RoutedEventArgs e)
        {
            if (CurrentOperation.AppType == AdobeType.Photoshop || CurrentOperation.AppType == AdobeType.PhotoshopBeta)
            {
                SaveData.Instance.SplashScreenColors[SaveData.SplashScreenColor.BackgroundColor] = BGColor.ColorData;
                SaveData.Instance.SplashScreenColors[SaveData.SplashScreenColor.TextColor] = FGColor.ColorData;
            }
            var selectedIndexReplaceAll = darkModeAlternativeSelect.SelectedIndex;
            Change(CurrentOperation.AppType, imageSelector.SelectedImage, (ReplaceType)selectedIndexReplaceAll);
        }
        
        private async void restoreButton_Click(object sender1, RoutedEventArgs e)
        {
            MainWindow.Instance.Navigate(Components.Enums.PageEnum.Loading);
            AdobeApp? app = Get(CurrentOperation.AppType);            
            if (app == null)
            {
                Misc.SetStatePleaseWait();
                return;
            }
            RemoveCurrentCache();
            //ChangerType type = Main.SplashScreenChangerType[CurrentOperation.AppType];
            ChangerType type = app.SplashScreenChangerType;
            switch (type)
            {
                case ChangerType.Photoshop:
                    Components.SplashScreenChanger.Photoshop.Changer.Restore();
                    break;
                case ChangerType.ResourceHacker:
                    Components.SplashScreenChanger.ResourceHacker.Changer.Restore();
                    break;
                case ChangerType.Normal:
                    Components.SplashScreenChanger.Normal.Changer.Restore();
                    break;
                default:
                    break;
            }


        }
        private static string sender = "SSChangerPage";

        private void darkModeAlternativeSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (darkModeAlternativePanel.Visibility == Visibility.Collapsed) return;

            if(darkModeAlternativeSelect.SelectedIndex < 0 || darkModeAlternativeSelect.SelectedIndex > +darkModeAlternativeSelect.Items.Count)
            {
                darkModeAlternativeSelect.SelectedIndex = 0;
                return;
            }
            IsDarkMode = darkModeAlternativeSelect.SelectedIndex == 1;
            RemoveCurrentCache();
            _ = LoadPreviewImageAsync();


        }
    }
}
