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
    /// AppSelectionItem.xaml etkileşim mantığı
    /// </summary>
    public partial class AppSelectionItem : UserControl
    {
        public AppSelectionItem()
        {
            InitializeComponent();
        }
        public string Key { get; private set; }
        public  Components.Enums.AdobeType AppType{ get; private set; }
        public AppSelectionItem Set(KeyValuePair<string, Components.Enums.AdobeType> item)
        {
            Key = item.Key;
            AppType = item.Value;

            var app = UnitedAdobeEditor.Components.Classes.SplashScreenData.Main.Get(AppType);
            if (app != null)
            {
                string name = app.LogoName;

                if (!String.IsNullOrWhiteSpace(name))
                {
                    string file = "Images/Logos/" + name;
                    icon.Source = Misc.ImageFromResource(file);
                }
            }
            icon.Visibility = (Key == "none").VisibleIfFalse();
            appName.Text = item.Value.ToFriendlyString();


            return this;
        }
        public AppSelectionItem Set(string key, string text)
        {
            Key = key;
            AppType = Components.Enums.AdobeType.Photoshop;

            icon.Visibility = (Key == "none").VisibleIfFalse();
            appName.Text = text;
            return this;
        }
    }
}
