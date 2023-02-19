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
using Wpf.Ui.Controls;

namespace JadColorPicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ColorDialog : UiWindow
    {
        public ColorDialog()
        {
            InitializeComponent();            
            colorPicker.SetOldColor( Color.FromRgb(255,255,255));
        }
        public ColorDialog(Color oldColor)
        {
            InitializeComponent();
            colorPicker.SetOldColor(oldColor);
        }
        public Color SelectedColor
        {
            get { return colorPicker.Color; }
            private set
            {
                colorPicker.Color = value;
            }
        }
        public bool IsSelected { get; private set; } = false;
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            IsSelected = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsSelected = false;
            this.Close();
        }
    }
}
