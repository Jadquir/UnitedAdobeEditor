using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using ColorHelper;
using Color = System.Windows.Media.Color;
using ColorConverter = ColorHelper.ColorConverter;

namespace JadColorPicker.Converters
{
    internal class ColorToHexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            return ColorConverter.RgbToHex(new RGB(color.R, color.G, color.B)).Value;   
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (string)value;
            HEX hex = new HEX(val);
            RGB rgb = ColorConverter.HexToRgb(hex);
            return Color.FromRgb(rgb.R, rgb.G, rgb.B);
        }
    }
}
