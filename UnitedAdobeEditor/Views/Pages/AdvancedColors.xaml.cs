using System;
using System.Collections.Generic;
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
using UnitedAdobeEditor.Components.ColorChanger;
using UnitedAdobeEditor.Views.CustomControls;
using Wpf.Ui.Controls;

namespace UnitedAdobeEditor.Views.Pages
{
    /// <summary>
    /// AdvancedColors.xaml etkileşim mantığı
    /// </summary>
    public partial class AdvancedColors : UiPage
    {
        public AdvancedColors()
        {
            InitializeComponent();

            var list = ColorValues.TxtValues;

            foreach (var item in list)
            {
                var box = new AdvancedColorBox(UIColorsData.GetColorValue(item));

                list1.Items.Add(box);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (AdvancedColorBox item in list1.Items)
            {
                UIColorsData.EditColorHolder(item.ColorData);
            }
            UIColorsData.Save();
            MainWindow.Instance.ShowSnackBar("Success!", "Color values saved successfully!");
        }
    }
}
