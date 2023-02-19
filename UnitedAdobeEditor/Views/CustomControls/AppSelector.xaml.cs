using System;
using System.Collections.Generic;
using System.IO;
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
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Components.Enums;

namespace UnitedAdobeEditor.Views.CustomControls
{
    /// <summary>
    /// AppSelector.xaml etkileşim mantığı
    /// </summary>
    public partial class AppSelector : ExtendedUserControl
    {
        private AdobeType type;
        public AppSelector(AdobeType type)
        {
            InitializeComponent();
            text1.Text = type.ToFriendlyString();

            this.type = type;

            OnClick += (s, e) =>
            {
                CurrentOperation.AppType = type;
                MainWindow.Instance.Navigate(Components.Enums.Page.VersionSelector);
            };
            string? name = UnitedAdobeEditor.Components.Classes.SplashScreenData.Main.Get(type)?.LogoName;
           
            if (!String.IsNullOrWhiteSpace(name))
            {
                string file = "./Images/Logos/" + name;
                if (!(new FileInfo(file).Exists))
                {
                    return;
                }
                image.ImageSource = Misc.ImageFromFile(file);
            }

        }
    }
}
