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
using UnitedAdobeEditor.Components.Enums;
using UnitedAdobeEditor.Views.CustomControls;
using Wpf.Ui.Controls;

namespace UnitedAdobeEditor.Views.Pages
{
    /// <summary>
    /// MainMenu.xaml etkileşim mantığı
    /// </summary>
    public partial class MainMenu : UiPage
    {
        public MainMenu()
        {
            InitializeComponent();

            foreach (AdobeType apptype in ((AdobeType[])Enum.GetValues(typeof(AdobeType))))
            {
                var box = new AppSelector(apptype);
                list1.Items.Add(box);
            }
            /*
            list1.Items.Add(new VersionCard(new Components.Classes.SelectedPath()));
            list1.Items.Add(new VersionCard(new Components.Classes.SelectedPath()));
            list1.Items.Add(new VersionCard(new Components.Classes.SelectedPath()));
            list1.Items.Add(new VersionCard(new Components.Classes.SelectedPath()));*/
        }
    }
}
