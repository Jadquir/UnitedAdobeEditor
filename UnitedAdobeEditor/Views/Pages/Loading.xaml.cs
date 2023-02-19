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

namespace UnitedAdobeEditor.Views.Pages
{
    /// <summary>
    /// Loading.xaml etkileşim mantığı
    /// </summary>
    public partial class Loading : UiPage
    {
        public static Loading Instance;
        public Loading()
        {
            InitializeComponent();
            
            Instance = this;
        }
        public void ChangeText(string text)
        {
            label1.Text = text;
        }

    }
}
