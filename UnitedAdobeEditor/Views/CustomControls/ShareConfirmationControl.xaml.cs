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
using UnitedAdobeEditor.Components;

namespace UnitedAdobeEditor.Views.CustomControls
{
    /// <summary>
    /// ShareConfirmationControl.xaml etkileşim mantığı
    /// </summary>
    public partial class ShareConfirmationControl : UserControl
    {
        public ShareConfirmationControl()
        {
            InitializeComponent();
        }

        internal ShareConfirmationControl Set(string text ,System.Drawing.Image image, AppSelectionItem appSelectionItem)
        {
            this.text.Text = text;
            this.image.Source = Misc.ConvertDrawingImageToBitmapImage(image);
            appSelectedItem.Set(new KeyValuePair<string, Components.Enums.AdobeType>(appSelectionItem.Key, appSelectionItem.AppType));
            return this;
        }
    }
}
