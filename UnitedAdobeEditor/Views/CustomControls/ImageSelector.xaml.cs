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
        public System.Drawing.Image? SelectedImage { get; private set; }
        public ImageSelector()
        {
            InitializeComponent();
            imageHolder.OnClick += OnClick;
        }
        public static string CreateSplashScreenLink => App.CreateSplashScreenLink;
        public void SetSelectedImage(System.Drawing.Image image)
        {
            Dispatcher.Invoke(() =>
            {
                SelectedImage = image;
                try
                {

                    this.image.Source = Misc.ConvertDrawingImageToBitmapImage(SelectedImage);
                }
                catch (Exception)
                {

                }
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
                if (image.Size != new System.Drawing.Size(1500, 1000))
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
                SelectedImage = (System.Drawing.Image)image.Clone();
                image.Dispose();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Navigate(Components.Enums.Page.ExploreConfigs);
        }
    }
}
