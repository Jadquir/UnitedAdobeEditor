using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace JadColorPicker.Converters
{
    internal class ColorToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            return color.ToRgbText();    
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = ((string)value).Split(',');
            var colorVal = new Byte[3];
            Array.Fill<byte>(colorVal, 0);
            for (int i = 0; i < str.Length; i++)
            {
                byte def = 0;
                byte.TryParse(str[i], out def);
                colorVal[i] = def;  
            }
            return Color.FromRgb(colorVal[0], colorVal[1], colorVal[2]);
        }
    }
}
