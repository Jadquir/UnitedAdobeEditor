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
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Components.Enums;
using UnitedAdobeEditor.Components.Helpers;
using UnitedAdobeEditor.Components.Scripts;
using UnitedAdobeEditor.Views.CustomControls;
using Wpf.Ui.Controls;

namespace UnitedAdobeEditor.Views.Pages
{
    /// <summary>
    /// VersionSelector.xaml etkileşim mantığı
    /// </summary>
    public partial class VersionSelector : UiPage
    {
        public VersionSelector()
        {
            InitializeComponent();
            UpdateVersions();
            appLogo.Set(new KeyValuePair<string, AdobeType>("", CurrentOperation.AppType));
        }
        private void UpdateVersions()
        {
            string[] oldPaths = Settings.Instance.SelectedPaths.GetPaths(CurrentOperation.AppType);

            list1.Items.Clear();
            for (int i = 0; i < oldPaths.Length; i++)
            {
                var item = oldPaths[i];
                var path = new SelectedPath(item, false);
                if (!path.SuccessInit) { continue; }
                var version = new VersionCard(path);
                list1.Items.Add(version);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectedPath selectedPath = PathSelector.Select(CurrentOperation.AppType);
            if(selectedPath == null)
            {
                return;
            }
            if(!selectedPath.SuccessInit)
            {
                return;
            }

            UpdateVersions();

            CurrentOperation.SelectedPath = selectedPath;
            MainWindow.Instance.Navigate(Components.Enums.Page.OperationSelector);
        }
    }
}
