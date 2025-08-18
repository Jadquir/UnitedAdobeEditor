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
    public partial class ColorDialog : FluentWindow
    {
        public ColorDialog()
        {
            InitializeComponent();            
            colorPicker.SetOldColor( Color.FromRgb(255,255,255));
            ApplyBackground(this);
        }
        public ColorDialog(Color oldColor)
        {
            InitializeComponent();
            colorPicker.SetOldColor(oldColor);
            ApplyBackground(this);

        }

        public static void ApplyBackground(Window window)
        {
            try
            {
                window.Dispatcher.Invoke(() =>
                {
                    WindowBackdrop.RemoveBackground(window);

                    var bgType = WindowBackdrop.IsSupported(WindowBackdropType.Mica) ? WindowBackdropType.Mica : WindowBackdropType.None;
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
