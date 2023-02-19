using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnitedAdobeEditor.Components;
using UnitedAdobeEditor.Components.Classes.SplashScreenData;
using UnitedAdobeEditor.Components.Enums;
using UnitedAdobeEditor.Components.SplashScreenCreator;
using Wpf.Ui.Controls;
using Color = System.Windows.Media.Color;
using JadColorPicker;
using Image = System.Drawing.Image;
using Microsoft.Win32;
using System.Diagnostics;
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Components.Scripts;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Imaging;
using System.ComponentModel;

namespace UnitedAdobeEditor.Views.Pages
{
    /// <summary>
    /// SplashScreenCreator.xaml etkileşim mantığı
    /// </summary>
    public partial class SplashScreenCreator : UiPage
    {
        
        public Image splashImage { get; set; }
        public Image CustomImage { get; set; }

        public bool IsAdobeTextVisible { get; set; } = true;
        public bool IsCreativeCloudTextVisible { get; set; } = true;

        public Color SSBackgroundColor { get; set; } = Color.FromRgb(255,255,255);
        public SplashScreenCreator()
        {
            InitializeComponent();

            adobetypeSelector.ItemsSource = Enum.GetValues(typeof(AdobeType));
            adobetypeSelector.SelectedIndex = 0;

            bgcolorSelect.OnClick += (s,e) =>
            {
                var color = ColorDialogJ.Show(SSBackgroundColor);
                if (!color.Item1) { return; }

                SSBackgroundColor = color.Item2;
                bgcolorBrush.Color = SSBackgroundColor;
            };

            bgcolorBrush.Color = SSBackgroundColor;
            CustomImage = Image.FromFile("./Images/SplashScreenCreator/Custom Image.png");
            Merge();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //-Create
            Merge();
        }
        BackgroundWorker worker;
        public void Merge()
        {
            Debug.WriteLine("enabled false");
            settingsPanel.IsEnabled = false;
            void Merge1()
            {

                string? logo = Main.Get((AdobeType)adobetypeSelector.SelectedValue)?.LogoName;
                if (String.IsNullOrWhiteSpace(logo))
                {
                    settingsPanel.IsEnabled = true;
                    return;
                }
                var info = new FileInfo($"./Images/SplashScreenCreator/LogoText/{logo}");
                if (!info.Exists)
                {
                    MessageBoxJ.ShowOK("There is not a logo for this application. Please contact with the developer.");
                    settingsPanel.IsEnabled = true;
                    return;
                }
                if (worker != null && worker.IsBusy)
                {
                    return;
                }
                worker = new BackgroundWorker();
                worker.DoWork += (s, e) =>
                {
                    Debug.WriteLine("merging");
                    e.Result = Creator.Merge(CustomImage, System.Drawing.Image.FromFile(info.FullName), SSBackgroundColor.ToDrawingColor(), IsAdobeTextVisible, IsCreativeCloudTextVisible);

                };
                worker.RunWorkerCompleted += (s, e) =>
                {
                    if(e.Result is not Image)
                    {
                        return;
                    }
                    Debug.WriteLine("merge complete");
                    splashImage = (Image)e.Result;
                    using (var ms = new MemoryStream())
                    {
                        splashImage.Save(ms, ImageFormat.Bmp);
                        ms.Seek(0, SeekOrigin.Begin);

                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = ms;
                        bitmapImage.EndInit();

                        imagePreview.Source = bitmapImage;
                    }
                    worker.Dispose();
                    settingsPanel.IsEnabled = true;
                    Debug.WriteLine("enabled true");
                };
                worker.RunWorkerAsync();
              
            }


            Merge1();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Save
            if(splashImage == null)
            {
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Png Files|*.png";
            sfd.FileName = "Adobe Splash Screen";
            sfd.ValidateNames = true;
            sfd.ShowDialog();

            splashImage.Save(sfd.FileName);

        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //Load image 
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Please select your custom image.";
            dialog.Filter = $"Image Files|*.png;";
            dialog.FilterIndex = 0;
            dialog.RestoreDirectory = true;
            dialog.Multiselect = false;

            bool? @bool = dialog.ShowDialog();
            if (@bool.HasValue && @bool.Value)
            {
                string selectedFileName = dialog.FileName;

                Debug.WriteLine(selectedFileName);
                var img = Image.FromFile(selectedFileName);
                if(img.Size != new System.Drawing.Size(829, 938))
                {
                    bool continueResize = false;
                    await MessageBoxJ.ShowYesNo("Image size is not 829x938. Do you want to stretch the image?",
                        (s,e) =>
                        {
                            continueResize = true;
                        });
                    if (!continueResize)
                    {
                        return;
                    }
                }
                CustomImage = img.ResizeImage(new System.Drawing.Size(829 ,938));
            }
            Merge();
        }
    }
}
