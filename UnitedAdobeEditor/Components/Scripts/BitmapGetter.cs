using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace UnitedAdobeEditor.Components.Scripts
{
    public class BitmapGetter
    {
        private static readonly Dictionary<string, Image> LoadedImages = new Dictionary<string, Image>(); 
        
        public static Image? ImageFromBase64String(string base64String)
        {
            try
            {
                if (LoadedImages.TryGetValue(base64String, out var image))
                    return image;
                // Remove the "data:image/png;base64," part from the base64 string
                base64String = base64String.Replace("data:image/png;base64,", "");

                // Convert the base64 string to a byte array
                byte[] imageBytes = Convert.FromBase64String(base64String);

                // Create a MemoryStream from the byte array
                using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                {
                    // Create the image from the MemoryStream
                    var value = Image.FromStream(memoryStream);
                    LoadedImages[base64String] = value;
                    return value;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error ImageFromBase64String: " + ex.Message);
                return null;
            }
        }
    }
}
