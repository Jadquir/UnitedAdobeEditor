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
using System.Security.AccessControl;

namespace UnitedAdobeEditor.Views.Pages
{
    /// <summary>
    /// SplashScreenChanger.xaml etkileşim mantığı
    /// </summary>
    public partial class SplashScreenChanger : UiPage
    {
        public SplashScreenChanger()
        {
            InitializeComponent();
            if (CurrentOperation.AppType == AdobeType.Photoshop || CurrentOperation.AppType == AdobeType.PhotoshopBeta)
            {
                colorboxes.Children.Clear();

                static AdvancedColorBox Create(Components.ColorChanger.ColorHolder colorHolder)
                {
                    return new AdvancedColorBox(colorHolder);
                }
                colorboxes.Children.Add(BGColor = Create(SaveData.Instance.SplashScreenColors[SaveData.SplashScreenColor.BackgroundColor]));
                colorboxes.Children.Add(FGColor = Create(SaveData.Instance.SplashScreenColors[SaveData.SplashScreenColor.TextColor]));
                colorboxes.Visibility = Visibility.Visible;
            }
            SaveData.Load();


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
        }
        private AdvancedColorBox BGColor;
        private AdvancedColorBox FGColor;
        public static bool Change(AdobeType AppType, Image? image)
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

            MainWindow.Instance.Navigate(Components.Enums.Page.Loading);
            ChangerType type = app.SplashScreenChangerType;
            switch (type)
            {
                case ChangerType.Photoshop:
                    Components.SplashScreenChanger.Photoshop.Changer.Change(image);
                    break;
                case ChangerType.ResourceHacker:
                    Components.SplashScreenChanger.ResourceHacker.Changer.Change(image);
                    break;
                case ChangerType.Normal:
                    Components.SplashScreenChanger.Normal.Changer.Change(image);
                    break;
                default:
                    break;
            }
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
            Change(CurrentOperation.AppType, imageSelector.SelectedImage);
            //if (SelectedImage == null)
            //{
            //    MessageBoxJ.ShowOK("Please select a splash screen first by clicking on the image.");
            //    return;
            //}
            //AdobeApp? app = Get(CurrentOperation.AppType);
            //if (app == null)
            //{
            //    return;
            //}
            //if (Misc.IsProccessRunning(app.FileName))
            //{
            //    MessageBoxJ.ShowOK($"Adobe Application ({app.FileName}) is running. Please close the application and try again.");
            //    return;
            //}
            //if (CurrentOperation.AppType == AdobeType.Photoshop || CurrentOperation.AppType == AdobeType.PhotoshopBeta)
            //{
            //    SaveData.Instance.SplashScreenColors[SaveData.SplashScreenColor.BackgroundColor] = BGColor.ColorData;
            //    SaveData.Instance.SplashScreenColors[SaveData.SplashScreenColor.TextColor] = FGColor.ColorData;
            //}
            //SaveData.Save();
          
            //MainWindow.Instance.Navigate(Components.Enums.Page.Loading);
            //ChangerType type = app.SplashScreenChangerType;
            //switch (type)
            //{
            //    case ChangerType.Photoshop:
            //        Components.SplashScreenChanger.Photoshop.Changer.Change(SelectedImage);
            //        break;
            //    case ChangerType.ResourceHacker:
            //        Components.SplashScreenChanger.ResourceHacker.Changer.Change(SelectedImage);
            //        break;
            //    case ChangerType.Normal:
            //        Components.SplashScreenChanger.Normal.Changer.Change(SelectedImage);
            //        break;
            //    default:
            //        break;
            //}
            //CurrentOperation.IsConfigActivated = false;
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
