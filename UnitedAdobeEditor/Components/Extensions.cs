using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using UnitedAdobeEditor.Components.Enums;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;
using Size = System.Drawing.Size;

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
        public static void ApplyBackground(this Window window)
        {
            try
            {
                window.Dispatcher.Invoke(() =>
                {
                    WindowBackdrop.RemoveBackground(window);

                    var bgType = MainWindow.GetBackgroundType();
                    if (bgType == WindowBackdropType.None)
                    {
                        ApplicationThemeManager.Apply(ApplicationThemeManager.GetAppTheme(), bgType, true);
                    }
                    else
                    {

                        WindowBackdrop.ApplyBackdrop(window, bgType);
                    }
                });
            }
            catch (Exception)
            {

            }
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
        //public static void SetWindowBackdropType(this UiWindow window)
        //{
        //    BackgroundType type = BackgroundType.Auto;
        //    if (Wpf.Ui.Appearance.Background.IsSupported(BackgroundType.Mica))
        //    {
        //        type = BackgroundType.Mica;
        //    }
        //    else
        //    {
        //        type = BackgroundType.None;
        //    }

        //    try
        //    {
        //        Debug.WriteLine("Applying Backdrop type : " + type.ToString());
        //        Wpf.Ui.Appearance.Background.Apply(window, type);
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    Wpf.Ui.Appearance.Accent.ApplySystemAccent();
        //}
        public static BitmapSource? ConvertToBitmapSource(this Image? img)
        {
            if(img == null) return null;
            using (var ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze(); // Optional, makes it cross-thread safe
                return bitmap;
            }
        }
        public static Bitmap ResizeImage(this Image image, int width, int height)
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
        public static Bitmap ExpandImage(this Image image1, int size)
        {
            if (image1 == null) return null;

            int newWidth = image1.Width + size;
            int newHeight = image1.Height + size;

            // Create new bitmap with transparency
            var expandedImage = new Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (var graphics = Graphics.FromImage(expandedImage))
            {
                graphics.Clear(Color.Transparent);

                // Calculate offset to center the image
                int offsetX = (newWidth - image1.Width) / 2;
                int offsetY = (newHeight - image1.Height) / 2;

                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(image1, offsetX, offsetY, image1.Width, image1.Height);
            }

            return expandedImage;
        }
        public static bool HasTransparentPadding(this Bitmap bmp)
        {
            if (bmp == null) return false;

            int minX = bmp.Width, minY = bmp.Height;
            int maxX = 0, maxY = 0;
            bool hasNonTransparent = false;

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    var pixel = bmp.GetPixel(x, y);
                    if (pixel.A > 0) // Non-transparent pixel
                    {
                        hasNonTransparent = true;
                        if (x < minX) minX = x;
                        if (y < minY) minY = y;
                        if (x > maxX) maxX = x;
                        if (y > maxY) maxY = y;
                    }
                }
            }

            if (!hasNonTransparent)
                return false; // whole image is transparent, treat as no content

            // Bounding box of content
            int contentWidth = maxX - minX + 1;
            int contentHeight = maxY - minY + 1;

            // If content bounding box is smaller than image size, padding exists
            return (contentWidth < bmp.Width || contentHeight < bmp.Height);
        }

        public static Bitmap ExpandToSize(this Image image1, Size targetSize)
        {
            if (image1 == null) return null;

            int newWidth = targetSize.Width;
            int newHeight = targetSize.Height;

            // Create new bitmap with transparency
            var expandedImage = new Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (var graphics = Graphics.FromImage(expandedImage))
            {
                graphics.Clear(Color.Transparent);

                // Calculate offset to center the image
                int offsetX = (newWidth - image1.Width) / 2;
                int offsetY = (newHeight - image1.Height) / 2;

                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(image1, offsetX, offsetY, image1.Width, image1.Height);
            }

            return expandedImage;
        }
        /// <summary>
        /// Prepares a replacement image to match the original.
        /// Handles resizing and transparent padding correctly.
        /// </summary>
        public static Image PrepareReplacementImage(Image replacement, Size targetResolution, Image? originalImage)
        {
            Size originalSize = targetResolution;
            bool originalHadPadding = false;

            if (originalImage != null)
            {
                originalSize = originalImage.Size;
                originalHadPadding = (originalSize != targetResolution);
            }

            // ✅ Case 1: Replacement already matches original size → use it directly
            if (replacement.Size == originalSize)
                return (Image)replacement.Clone();

            // ✅ Case 2: Replacement is exactly nominal size
            if (replacement.Size == targetResolution)
            {
                if (originalHadPadding)
                {
                    // expand nominal image to original size
                    return replacement.ExpandToSize(originalSize);
                }
                return (Image)replacement.Clone();
            }

            // ❌ Otherwise: replacement is some other size → resize to nominal
            var resized = replacement.ResizeImage(targetResolution);

            if (originalHadPadding &&
                (resized.Size != originalSize))
            {
                var expanded = resized.ExpandToSize(originalSize);
                resized.Dispose();
                return expanded;
            }

            return resized;
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
        public static Image? ToImage(this byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
                return null;

            using var ms = new MemoryStream(byteArray);
            return Image.FromStream(ms);
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
