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
    /// LinkButton.xaml etkileşim mantığı
    /// </summary>
    public partial class LinkButton : UserControl
    {
        public LinkButton()
        {
            InitializeComponent();
        }
        public string Link;
        public void SetIcon(string text)
        {
            Icon1.Data = (Geometry)this.TryFindResource(text);

            string toolTipText = "Contact";
            switch (text)
            {
                case "discord":
                    toolTipText = "Discord Server";
                    break;
                case "youtube":
                    toolTipText = "Youtube";
                    break;
                case "mail":
                    toolTipText = "Mail";
                    break;
                case "itchio":
                    toolTipText = "Itch.io";
                    break;
                case "website":
                    toolTipText = "Website";
                    break;
                default:
                    break;
            }
            SetTooltip(toolTipText);
        }
        public void SetTooltip(string text)
        {
            this.ToolTip = text;
            border.ToolTip = text;
            Icon1.ToolTip = text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Misc.OpenUrl(Link);
        }
    }
}
