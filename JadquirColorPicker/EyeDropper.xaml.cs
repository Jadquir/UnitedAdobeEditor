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
using WpfScreenHelper;
using Color = System.Windows.Media.Color;

namespace JadColorPicker
{
    /// <summary>
    /// EyeDropper.xaml etkileşim mantığı
    /// </summary>
    public partial class EyeDropper : UserControl
    {
        public static DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(EyeDropper), new PropertyMetadata(Colors.Red));

        public EyeDropper()
        {
            InitializeComponent();
        }

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }
        public static Rect CurrentScreenRect()
        {
            return Screen.FromPoint(MouseHelper.MousePosition).Bounds;
        }

        private void ButtonOnClick(object sender, RoutedEventArgs e)
        {
            var window = new EyeDropperWindow(ScreenshotBitmapImage());
            // The EyeDropperWindow spans the entire current screen, so this event gets called whenever
            // the mouse changes from one screen to another.
            window.MouseLeave += (_s, _e) =>
            {
                // Changing the screen rect and image unfortunately causes a short flash of the old
                // image on the new screen. To circumvent this issue, we first set the window to be
                // invisible (with a size of 0x0).
                // This is not ideal but makes the issue less noticable. Actually hiding the window
                // seems to work better but we can't do that because calling `.Hide()` or setting
                // `.Visibility` sets the `DialogResult` to `false`.
                window.SetScreenRect(new Rect(0, 0, 0, 0));
                window.SetImage(ScreenshotBitmapImage(window));
                window.SetScreenRect(CurrentScreenRect());
            };
            window.SetScreenRect(CurrentScreenRect());
            var res = window.ShowDialog();
            if (res.HasValue && res.Value)
            {
                SelectedColor = window.SelectedColor;
            }
        }

        private BitmapImage ScreenshotBitmapImage(Window? window = null)
        {
            var screenshot = TakeScreenshot();

            var bitmapImage = new BitmapImage();
            using (var ms = new MemoryStream())
            {
                screenshot.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Position = 0;

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();
            }

            screenshot.Dispose();

            return bitmapImage;
        }

        private Bitmap TakeScreenshot(Window? window = null)
        {
            Rect screen = CurrentScreenRect();

            // Get DPI scale for the current screen
            var source = PresentationSource.FromVisual(window ?? Application.Current.MainWindow);
            double dpiX = 1.0, dpiY = 1.0;

            if (source != null)
            {
                dpiX = source.CompositionTarget.TransformToDevice.M11;
                dpiY = source.CompositionTarget.TransformToDevice.M22;
            }

            int width = (int)(screen.Width * dpiX);
            int height = (int)(screen.Height * dpiY);

            var screenBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            using (var g = Graphics.FromImage(screenBitmap))
            {
                g.CopyFromScreen((int)(screen.Left * dpiX), (int)(screen.Top * dpiY), 0, 0, new System.Drawing.Size(width, height));
            }

            return screenBitmap;
        }

    }
}
