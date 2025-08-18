using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
using UnitedAdobeEditor.Components.Classes;

namespace UnitedAdobeEditor.Views.CustomControls
{
    /// <summary>
    /// ImageSelector.xaml etkileşim mantığı
    /// </summary>
    public partial class ImageSelector : UserControl
    {
        static BitmapImage? defaultImage;
        public System.Drawing.Image? SelectedImage { get; private set; }
        public System.Drawing.Image? previewImage { get; private set; }
        public ImageSelector()
        {
            InitializeComponent();
            imageHolder.OnClick += OnClick;
            saveCurrentImageBorder.Visibility = (ExportDefaultImage != null).VisibleIfTrue();

            if (defaultImage == null) {

                // The URI string
                string uriString = "pack://application:,,,/Images/SplashScreenImport.png";

                // Create a BitmapImage from the URI
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(uriString, UriKind.Absolute);
                bitmap.EndInit();

                defaultImage = bitmap;
            }

        }
        void SetImage(BitmapSource? image)
        {
            this.image.Source = (image ?? defaultImage);
        }
        void UpdateImage()
        {
            SetImage((SelectedImage ?? previewImage).ConvertToBitmapSource());

            saveCurrentImageBorder.Visibility = (previewImage != null).VisibleIfTrue();
        }
        public static string CreateSplashScreenLink => App.CreateSplashScreenLink;

        public Action? ExportDefaultImage { get; internal set; }

        public void SetSelectedImage(System.Drawing.Image image)
        {
            Dispatcher.Invoke(() =>
            {
                SelectedImage = image;
                UpdateImage();
            });
        }
        private void SaveCurrentImageClick(object sender, RoutedEventArgs e)
        {
            ExportDefaultImage?.Invoke();
        }
        public void SetPreviewImage(System.Drawing.Image? image)
        {
            Dispatcher.Invoke(() =>
            {
                previewImage = image;
                UpdateImage();
            });
        }
        private async void OnClick(object? sender, EventArgs e)
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
                System.Drawing.Image image = System.Drawing.Image.FromFile(dialog.FileName);

                const float defaultAspect = 15 / 10; 
                const float epsilon = 0.01f;

                var imageAspectRatio = image.Size.Width / image.Size.Height;
                if (Math.Abs(imageAspectRatio - defaultAspect) >= epsilon)
                {
                    bool continueSelecting = false;
                    await MessageBoxJ.ShowYesNo(
                        $"The selected image does not match the recommended aspect ratio of {defaultAspect} (1500×1000).\n" +
                        "Using this image may cause distortion.\n" +
                        "Do you want to continue anyway?",
                        (s, e) => { continueSelecting = true; }
                    );

                    if (!continueSelecting)
                    {
                        return;
                    }
                }
                this.image.Source = Misc.ImageFromFile(dialog.FileName);
                SelectedImage = (System.Drawing.Image)image.Clone();
                image.Dispose();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Navigate(Components.Enums.PageEnum.ExploreConfigs);
        }

    }
}
