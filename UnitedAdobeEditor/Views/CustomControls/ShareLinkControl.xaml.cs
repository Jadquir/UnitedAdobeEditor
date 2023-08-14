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

namespace UnitedAdobeEditor.Views.CustomControls
{
    /// <summary>
    /// ShareLinkControl.xaml etkileşim mantığı
    /// </summary>
    public partial class ShareLinkControl : UserControl
    {
        public ShareLinkControl()
        {
            InitializeComponent();
        }
        public ShareLinkControl SetUrl(string url)
        {
            shareUrl.Text = url;
            return this;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(shareUrl.Text);
        }
    }
}
