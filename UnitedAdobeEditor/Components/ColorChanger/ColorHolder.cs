using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitedAdobeEditor.Components.ColorChanger
{
    public class ColorHolder : ICloneable
    {
        public string Name { get; private set; }
        public Color Color { get; set; }

        public float Opacity { get; set; }

        public ColorHolder(string name, Color color,float opacity)
        {
            Name = name;
            Color = color;
            Opacity = opacity; 
        }
        private string GetOpacity()
        {
            ValidateOpacity();
            return Opacity.ToString("0.0").Replace(',', '.');
        }
        public void ValidateOpacity()
        {
            if (Opacity > 1)
            {
                Opacity = 1;
            }
            if (Opacity < 0)
            {
                Opacity = 0;
            }
        }
        public string GetTxtValue()
        {
            string color = $"{this.Color.R}, {this.Color.G}, {this.Color.B}";
            return $"		[ {color}, {GetOpacity()} ]";
        }

        public string GetCelValue()
        {
            string r = this.Color.R.ToString("X2");
            string g = this.Color.G.ToString("X2");
            string b = this.Color.B.ToString("X2");
            string color = $"0x{r}{r}, 0x{g}{g}, 0x{b}{b}";
            return $"		[ {color}, {GetOpacity()} ]";
        }

        public object Clone()
        {
            return new ColorHolder(Name, Color, Opacity);
        }
    }
}
