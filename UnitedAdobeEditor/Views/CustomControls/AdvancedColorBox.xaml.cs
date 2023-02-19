using System;
using System.Collections.Generic;
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
using UnitedAdobeEditor.Components.ColorChanger;
using UnitedAdobeEditor.Components.SplashScreenChanger;
using Wpf.Ui.Controls;

namespace UnitedAdobeEditor.Views.CustomControls
{
    /// <summary>
    /// AdvancedColorBox.xaml etkileşim mantığı
    /// </summary>
    public partial class AdvancedColorBox : UserControl
    {
        public ColorHolder ColorData { get; private set; }
        public AdvancedColorBox(ColorHolder colorData)
        {
            InitializeComponent();

            colorData.ValidateOpacity();
            ColorData = (ColorHolder)colorData.Clone();

            title1.Text = colorData.Name;

            colorSelect.Background = new SolidColorBrush(colorData.Color.ToMediaColor());

            
            numberBox.Value = colorData.Opacity;

            colorSelect.OnClick += (o, e) =>
            {
                var color = ColorDialogJ.Show(ColorData.Color.ToMediaColor());
                if (!color.Item1) { return; }

                ColorData.Color = color.Item2.ToDrawingColor();
                colorSelect.Background = new SolidColorBrush(color.Item2);
            };

        }
    }
}
