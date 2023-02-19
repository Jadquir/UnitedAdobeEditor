using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Wpf.Ui.Appearance;
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
            SetWindowBackdropType(this);
        }
        public ColorDialog(Color oldColor)
        {
            InitializeComponent();
            colorPicker.SetOldColor(oldColor);
            SetWindowBackdropType(this);

        }

        public static void SetWindowBackdropType(UiWindow window)
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
