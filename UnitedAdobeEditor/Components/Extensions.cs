using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.IO;
using System.Diagnostics;
using UnitedAdobeEditor.Components.Enums;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace UnitedAdobeEditor.Components
{
    public static class Extensions
    {
        public static string ToFriendlyString(this AdobeType me)
        {
            string value = me.ToString();
            switch (me)
            {
                case AdobeType.AfterEffects:
                    value = "After Effects";
                    break;
                case AdobeType.PremierePro:
                    value = "Premiere Pro";
                    break;
                case AdobeType.MediaEncoder:
                    value = "Media Encoder";
                    break;
                case AdobeType.LightroomClassic:
                    value = "Lightroom Classic";
                    break;
                case AdobeType.CharacterAnimator:
                    value = "Character Animator";
                    break;
                default:
                    break;
            }
            return value;
        }
        public static System.Drawing.Color ToDrawingColor(this System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }
        public static string GetText(this OperationType type)
        {
            return type switch
            {
                OperationType.UIColor => "Change UI Color",
                OperationType.SplashScreen => "Change Splash Screen",
                _ => "",
            };
        }
        public static System.Windows.Media.Color ToMediaColor(this System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromRgb(color.R, color.G, color.B );
        }
        public static void SetWindowBackdropType(this UiWindow window)
        {
            BackgroundType type = BackgroundType.Auto;
            if (Wpf.Ui.Appearance.Background.IsSupported(BackgroundType.Mica))
            {
                type = BackgroundType.Mica;
            }
            else
            {
                type = BackgroundType.None;
            }

            try
            {
                Debug.WriteLine("Applying Backdrop type : " + type.ToString());
                Wpf.Ui.Appearance.Background.Apply(window, type);
            }
            catch (Exception)
            {

            }
            Wpf.Ui.Appearance.Accent.ApplySystemAccent();
        }
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            return ResizeImage(image, new Size(width, height));  
        }
        public static Bitmap ResizeImage(this Image image1, Size size)
        {
            Debug.WriteLine("resize image : " + image1);
            var image = (Image)image1.Clone();
            int width = size.Width;
            int height = size.Height;
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        public static byte[] ToByteArray(this Image imageIn)
        {
            Debug.WriteLine("Image to Byte : " + imageIn);
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}
