using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Views.CustomControls;
using Wpf.Ui.Controls;
using Image = System.Drawing.Image;
using UnitedAdobeEditor.Components;
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Components.Classes.SplashScreenData;
using UnitedAdobeEditor.Components.Enums;
using UnitedAdobeEditor.Components.SplashScreenChanger;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using UnitedAdobeEditor.Components.Helpers;
using System.IO;
using static UnitedAdobeEditor.Components.Classes.SplashScreenData.Main;

namespace UnitedAdobeEditor.Views.Pages
{
    /// <summary>
    /// SplashScreenChanger.xaml etkileşim mantığı
    /// </summary>
    public partial class SplashScreenChanger : UiPage
    {
        private Image SelectedImage = null;
        public SplashScreenChanger()
        {
            InitializeComponent();

            this.image.Source = Misc.ImageFromResource("Images/SplashScreenImport.png");
            Debug.WriteLine(image.Source);
            imageHolder.OnClick += async (s, e) =>
            {
                OpenFileDialog dialog = new OpenFileDialog();

                dialog.Title = "Please select the custom splash screen";
                dialog.Filter = "PNG Image (*.png)|*.png";
                dialog.FilterIndex = 0;
                dialog.RestoreDirectory = true;
                dialog.Multiselect = false;

                bool? @bool = dialog.ShowDialog();

                Debug.WriteLine(dialog.FileName);
                if (@bool.HasValue && @bool.Value)
                {
                    Image image = System.Drawing.Image.FromFile(dialog.FileName);
                    if(image.Size != new System.Drawing.Size(1500, 1000))
                    {
                        bool continueSelecting = false;
                        await MessageBoxJ.ShowYesNo("The selected image is not 1500x1000.\nImage may be distorted.\nDo you want to continue?",
                            (s, e) => { continueSelecting = true; });
                        if (!continueSelecting)
                        {
                            return;
                        }
                    }
                    this.image.Source = Misc.ImageFromFile(dialog.FileName);
                    SelectedImage = (Image)image.Clone();
                    image.Dispose();
                }
                else
                {
                    Debug.WriteLine("Selecting image failed.");
                }

                

            };
            if (CurrentOperation.AppType == AdobeType.Photoshop)
            {
                colorboxes.Children.Clear();
                colorboxes.Children.Add(BGColor = new AdvancedColorBox(SaveData.Instance.SplashScreenColors[SaveData.SplashScreenColor.BackgroundColor]));
                colorboxes.Children.Add(FGColor = new AdvancedColorBox(SaveData.Instance.SplashScreenColors[SaveData.SplashScreenColor.TextColor]));
                colorboxes.Visibility = Visibility.Visible;
            }
            SaveData.Load();
        }
        private AdvancedColorBox BGColor;
        private AdvancedColorBox FGColor;
        private void applyButton_Click(object sender1, RoutedEventArgs e)
        {
            if(SelectedImage == null)
            {
                MessageBoxJ.ShowOK("Please select a splash screen first by clicking on the image.");
                return;
            }
            AdobeApp? app = Get(CurrentOperation.AppType);
            if (app == null)
            {
                return;
            }
            /*
            if (Misc.IsProccessRunning(Main.FileNames[CurrentOperation.AppType]))
            {
                MessageBoxJ.ShowOK("Adobe Application is running. Please close the application and try again.");
                return;
            }*/
            if (Misc.IsProccessRunning(app.FileName))
            {
                MessageBoxJ.ShowOK($"Adobe Application ({app.FileName}) is running. Please close the application and try again.");
                return;
            }
            if (CurrentOperation.AppType == AdobeType.Photoshop)
            {
                SaveData.Instance.SplashScreenColors[SaveData.SplashScreenColor.BackgroundColor] = BGColor.ColorData;
                SaveData.Instance.SplashScreenColors[SaveData.SplashScreenColor.TextColor] = FGColor.ColorData;
            }
            SaveData.Save();
          
            MainWindow.Instance.Navigate(Components.Enums.Page.Loading);

            // ChangerType type = Main.SplashScreenChangerType[CurrentOperation.AppType];
            ChangerType type = app.SplashScreenChangerType;
            switch (type)
            {
                case ChangerType.Photoshop:
                    Components.SplashScreenChanger.Photoshop.Changer.Change(SelectedImage);
                    break;
                case ChangerType.ResourceHacker:
                    Components.SplashScreenChanger.ResourceHacker.Changer.Change(SelectedImage);
                    break;
                case ChangerType.Normal:
                    Components.SplashScreenChanger.Normal.Changer.Change(SelectedImage);
                    break;
                default:
                    break;
            }
        }
        
        private async void restoreButton_Click(object sender1, RoutedEventArgs e)
        {
            MainWindow.Instance.Navigate(Components.Enums.Page.Loading);
            AdobeApp? app = Get(CurrentOperation.AppType);            
            if (app == null)
            {
                Misc.SetStatePleaseWait();
                return;
            }
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
    }
}
