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
using System.Net.Mail;
using System.Windows.Media.Animation;
using System.Windows;

namespace UnitedAdobeEditor.Components
{
    public static class Extensions
    {
        public static bool IsMail(this string s)
        {
            try
            {
                new MailAddress(s);
                return true;
            }
            catch (Exception)
            {

            }
            return false;
        }
        public static void ApplyBackground(this UiWindow window)
        {
            Wpf.Ui.Appearance.Background.Apply(window, MainWindow.GetBackgroundType());
            Wpf.Ui.Appearance.Accent.ApplySystemAccent();
        }
        public static string ToFriendlyString(this AdobeType me)
        {
            string value = me.ToString();
            switch (me)
            {
                case AdobeType.AfterEffects:
                    value = "After Effects";
                    break;
                case AdobeType.PhotoshopBeta:
                    value = "Photoshop Beta";
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
            return ResizeImage(image, new System.Drawing.Size(width, height));  
        }
        public static Bitmap ResizeImage(this Image image1, System.Drawing.Size size)
        {
            if (image1 is null) return null;
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
        public static void FadeIn(this FrameworkElement element, int animationDuration, float MaxOpacity = 1f)
        {
            if (!MainWindow.Instance.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    FadeIn(element, animationDuration, MaxOpacity);
                }));
                return;
            }

            Storyboard? currentStoryboard;
            elements.TryGetValue(element, out currentStoryboard);

            if (currentStoryboard is not null)
            {
                currentStoryboard.SkipToFill();
                currentStoryboard.Stop();
            }
            var fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = MaxOpacity,
                Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration))
            };

            Storyboard.SetTarget(fadeInAnimation, element);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(FrameworkElement.OpacityProperty));


            element.Visibility = Visibility.Visible;
            currentStoryboard = new Storyboard();
            currentStoryboard.Children.Add(fadeInAnimation);
            currentStoryboard.Begin();

            elements[element] = currentStoryboard;
        }
        private static Dictionary<FrameworkElement, Storyboard> elements = new Dictionary<FrameworkElement, Storyboard>();
        public static void FadeOut(this FrameworkElement element, int animationDuration, float MaxOpacity = 1f)
        {
            if (!MainWindow.Instance.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    FadeOut(element, animationDuration, MaxOpacity);
                }));
                return;
            }

            Storyboard? currentStoryboard;
            elements.TryGetValue(element, out currentStoryboard);

            if (currentStoryboard != null)
            {
                currentStoryboard.SkipToFill();
                currentStoryboard.Stop();
            }



            var fadeOutAnimation = new DoubleAnimation
            {
                From = MaxOpacity,
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration))
            };
            fadeOutAnimation.Completed += (s, e) =>
            {
                element.Visibility = Visibility.Collapsed;
                //IsAnimating = false;
            };
            Storyboard.SetTarget(fadeOutAnimation, element);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(FrameworkElement.OpacityProperty));

            element.Visibility = Visibility.Visible;
            currentStoryboard = new Storyboard();
            currentStoryboard.Children.Add(fadeOutAnimation);
            currentStoryboard.Begin();

            elements[element] = currentStoryboard;
        }
        public static Visibility VisibleIfTrue(this bool value)
        {
            return value ? Visibility.Visible : Visibility.Collapsed;
        }
        public static Visibility VisibleIfFalse(this bool value)
        {
            return value ? Visibility.Collapsed : Visibility.Visible;
        }
        public static Visibility VisibleIfTrue(this bool? value)
        {
            return (value.HasValue && value.Value).VisibleIfTrue();
        }
        public static Visibility VisibleIfFalse(this bool? value)
        {
            return (value.HasValue && value.Value).VisibleIfFalse();
        }
    }
}
