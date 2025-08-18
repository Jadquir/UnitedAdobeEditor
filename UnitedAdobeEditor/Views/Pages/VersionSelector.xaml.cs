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
using UnitedAdobeEditor.Components.Classes.SplashScreenData;
using UnitedAdobeEditor.Components.Enums;
using UnitedAdobeEditor.Components.Helpers;
using UnitedAdobeEditor.Components.Scripts;
using UnitedAdobeEditor.Views.CustomControls;
using Wpf.Ui.Controls;
using Path = System.IO.Path;

namespace UnitedAdobeEditor.Views.Pages
{
    /// <summary>
    /// VersionSelector.xaml etkileşim mantığı
    /// </summary>
    public partial class VersionSelector : Page
    {
        static readonly List<AdobeType> searchedApps = new List<AdobeType>(); 
        AdobeType currentType;

        internal AdobeType GetAppType()
        {
            return currentType;
        }
        public VersionSelector()
        {
            currentType = CurrentOperation.AppType;
            InitializeComponent();
            
            
            UpdateVersions();

            appLogo.Set(new KeyValuePair<string, AdobeType>("", currentType));

            // Search for apps
            bool alreadySearched = searchedApps.Contains(currentType);
            loadingRing.Visibility = alreadySearched.VisibleIfFalse();
            if (!alreadySearched || true)
            {
                _ = AdobeFinder.FindAppsAsync(currentType, (list) => Dispatcher.BeginInvoke(new Action(() => ProcessFoundApps(list))));
            }
        }
        void ProcessFoundApps(List<string> list)
        {
            var app = Main.Get(currentType);
            if (app == null) return;

            foreach (var possibleFolder in list)
            {
                if (possibleFolder.Contains("(Beta)") && currentType != AdobeType.PhotoshopBeta) continue;
                var exePath = Path.Combine(possibleFolder, app.FileName);
                if (File.Exists(exePath))
                {
                    var path = new SelectedPath(exePath, false);
                    Settings.Instance.SelectedPaths.Add(currentType, path.EXEFilePath);
                }
            }
            UpdateVersions();

            searchedApps.Add(currentType);
            loadingRing.FadeOut(300);
        }

        private void UpdateVersions()
        {
            string[] oldPaths = Settings.Instance.SelectedPaths.GetPaths(currentType);

            list1.Items.Clear();
            for (int i = 0; i < oldPaths.Length; i++)
            {
                var item = oldPaths[i];
                var path = new SelectedPath(item, false);
                if (!path.SuccessInit) { continue; }
                var version = new VersionCard(path);
                list1.Items.Add(version);
            }
            emptyText.Visibility = (list1.Items.Count > 0).VisibleIfFalse();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectedPath selectedPath = PathSelector.Select(currentType);
            if(selectedPath == null)
            {
                return;
            }
            if(!selectedPath.SuccessInit)
            {
                return;
            }

            UpdateVersions();

            CurrentOperation.AppType = currentType;
            CurrentOperation.SelectedPath = selectedPath;
            MainWindow.Instance.Navigate(Components.Enums.PageEnum.OperationSelector);
        }
    }
}
