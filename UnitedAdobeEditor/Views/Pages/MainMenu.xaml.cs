using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
using UnitedAdobeEditor.Components.Firebase;
using UnitedAdobeEditor.Components.Helpers;
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
                list1.Items.Add(new AppSelector(apptype));
            }
        }

        private void LoadConfig(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "UAE files (*.uae)|*.uae",
                Multiselect = false
            };

            var result = dialog.ShowDialog();
            if(result.HasValue && result.Value)
            {
                _ = LoadFile(dialog.FileName);
            }
        }
        public static async Task LoadFile(string file)
        {
            try
            {
                var json = File.ReadAllText(file);
                var config = JsonConvert.DeserializeObject<Config>(json);
                if(config is null)
                {
                    await MessageBoxJ.ShowOKAsync("Couldn't read the config file!");
                    return;
                }
                await RunConfig(config);

            }
            catch (Exception ex)
            {

                await MessageBoxJ.ShowOKAsync("Something went wrong!\nError: " + ex.Message);
            }

        }
        public static async Task RunConfig(Config config)
        {
            var operation = config.ToOperation(out string error);
            if (operation is null)
            {
                await MessageBoxJ.ShowOKAsync(error);
                return;
            }
            void Failed()
            {
                CurrentOperation.IsConfigActivated = false;
                CurrentOperation.isRunStat = false;
                CurrentOperation.Operation = null;
                CurrentOperation.SelectedPath = null;
                CurrentOperation.operationType = OperationType.SplashScreen;
                CurrentOperation.AppType = AdobeType.Photoshop;

            }
            CurrentOperation.IsConfigActivated = true;
            CurrentOperation.Operation = operation;
            CurrentOperation.operationType = operation.operationType;
            CurrentOperation.AppType = operation.AppType;
            CurrentOperation.SelectedPath = null;

            if (config.is_silent)
            {
                MainWindow.Instance.ChangeVisibility(false);
            }
            else
            {
                bool continueChanging = false;
                await MessageBoxJ.ShowDialogFunc(
                    new ShareConfirmationControl()
                    { Margin = new Thickness(0, 10, 0, 0) }
                    .Set("Are you sure you want to use this config?", 
                    CurrentOperation.Operation.SplashScreen, 
                    new AppSelectionItem().Set(new KeyValuePair<string, AdobeType>("", CurrentOperation.AppType)))
                    , "Yes", () => { continueChanging = true; return true; },
                    "No", () => { continueChanging = false; return true; }, heightMultiplier: 5f, LeftPrimary: true
                    );
                if(!continueChanging)
                {
                    Failed();
                    return;
                }
            }
            if (!string.IsNullOrWhiteSpace(config.selected_folder))
            {
                SelectedPath selectedPath = PathSelector.Select(operation.AppType);
                if (selectedPath == null || !selectedPath.SuccessInit)
                {
                    MainWindow.Instance.ChangeVisibility(true);
                    Failed();
                    return;
                }
                CurrentOperation.SelectedPath = selectedPath;
            }
            if (CurrentOperation.SelectedPath != null)
            {
                var isSuccess = SplashScreenChanger.Change(CurrentOperation.AppType, CurrentOperation.Operation.SplashScreen);
                if (isSuccess && config.closeAfterChanging)
                {
                    Application.Current.Shutdown();
                }
                else
                {
                    MainWindow.Instance.ChangeVisibility(true);
                }
            }
            else
            {
                MainWindow.Instance.ChangeVisibility(true);
                MainWindow.Instance.Navigate(Components.Enums.Page.VersionSelector);
            }
        }
        private void openCreateLink(object sender, RoutedEventArgs e)
        {
            Misc.OpenUrl(App.CreateSplashScreenLink);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            MainWindow.Instance.Navigate(Components.Enums.Page.CreateConfig);
        }

        private async void shareConfig(object sender, RoutedEventArgs e)
        {
            if(FirebaseAuthControl.Instance.Client.User == null)
            {
                bool continueLogin = false;
                await MessageBoxJ.ShowYesNo(
                     "You have to be logged in in order to share configs. Do you want to login/register right now?"
                     , (s, e) => { continueLogin = true; }, (s, e) => { continueLogin = false; });
                if (!continueLogin) { return; }
                new MRA_WPF.Views.Windows.MRA_FirebaseUI.MRA_Login().ShowDialog();
                if (FirebaseAuthControl.Instance.Client.User == null)
                {
                    return;
                }
            }
            MainWindow.Instance.Navigate(Components.Enums.Page.ShareConfigPage);
        }

        private void exploreConfigs(object sender, RoutedEventArgs e)
        {

            MainWindow.Instance.Navigate(Components.Enums.Page.ExploreConfigs);
        }
    }
}
