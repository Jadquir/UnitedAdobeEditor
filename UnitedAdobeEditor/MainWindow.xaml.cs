using JadUpdate.Class;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using UnitedAdobeEditor.Components;
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Components.ColorChanger;
using UnitedAdobeEditor.Components.Enums;
using UnitedAdobeEditor.Components.Helpers;
using UnitedAdobeEditor.Components.Scripts;
using UnitedAdobeEditor.Views.Pages;
using Wpf.Ui.Animations;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Hardware;
using Wpf.Ui.Services;
using WpfScreenHelper;
using ColorChanger = UnitedAdobeEditor.Views.Pages.ColorChanger;
using Page = UnitedAdobeEditor.Components.Enums.Page;

namespace UnitedAdobeEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UiWindow
    {
        public static MainWindow Instance;

        public MainWindow()
        {
            AdminRelaunch.AdminRelauncher();
            if (Instance != null)
            {
                this.Close();
                return;
            }
            Instance = this;

            InitializeComponent();
            Thickness resizeBorderThickness = WindowChrome.GetWindowChrome(this).ResizeBorderThickness;

            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - resizeBorderThickness.Top - resizeBorderThickness.Bottom + 8;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth - resizeBorderThickness.Right - resizeBorderThickness.Left + 10;
            Instances();

            devText.OnClick += (s, e) =>
            {
                Misc.OpenUrl(App.YoutubeLink);
            };
            support.OnClick += (s, e) =>
            {
                Misc.OpenUrl(App.SupportUrl);
            };

            this.Loaded += (s, e) =>
            {
                Navigate(Page.MainMenu);
                CheckUpdates();
            };


            RootNav.Navigated += (o, e) =>
            {
                settingsButton.Visibility = e.Content is MainMenu ? Visibility.Visible : Visibility.Collapsed;
                BackButton.Visibility = RootNav.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
                GoBackDisabled = e.Content is Loading;
                Transitions.ApplyTransition(e.Content, TransitionType.FadeInWithSlide, 150);
                UpdateText();
            };

            this.ContentRendered += MainWindow_ContentRendered;
        }

        private void MainWindow_ContentRendered(object? sender, EventArgs e)
        {
            var bgType = Wpf.Ui.Appearance.Background.IsSupported(BackgroundType.Mica) ?
                BackgroundType.Mica : BackgroundType.None;

            Wpf.Ui.Appearance.Background.Apply(this, bgType);
            Wpf.Ui.Appearance.Accent.ApplySystemAccent();
        }

        private void Instances()
        {
            _ = new Settings();
            _ = new UIColorsData();
            _ = new Components.SplashScreenChanger.SaveData();
        }

        public void ShowSnackBar(string title, string text,
            Wpf.Ui.Common.ControlAppearance appearance = Wpf.Ui.Common.ControlAppearance.Dark,
            Wpf.Ui.Common.SymbolRegular icon = Wpf.Ui.Common.SymbolRegular.CheckmarkCircle32,
            float seconds = 2f)
        {
            snackbar.Appearance = appearance;
            snackbar.Icon = icon;
            snackbar.Timeout = (int)(seconds * 1000);
            snackbar.Show(title, text);
        }
        public Dialog GetDialog()
        {
            dialog.MinHeight = this.Height * (2 / 3);
            dialog.MinWidth = this.Width * (2 / 3);
            
            return dialog;
        }
        public void Navigate(Page page, object[]? param = null)
        {
            UiPage? uiPage = null;
            switch (page)
            {
                case Page.MainMenu:
                    uiPage = new MainMenu();
                    break;
                case Page.Settings:
                    break; 
                case Page.VersionSelector:
                    uiPage = new VersionSelector();
                    break;
                case Page.OperationSelector:
                    if (CurrentOperation.AppType == AdobeType.Photoshop)
                    {
                        uiPage = new OperationSelector();
                    }
                    else
                    {
                        Navigate(Page.SplashScreenChanger, param);
                        return;
                    }
                    break;
                case Page.ColorChanger:
                    CurrentOperation.operationType = OperationType.UIColor;
                    uiPage = new ColorChanger();
                    break;
                case Page.SplashScreenChanger:
                    CurrentOperation.operationType = OperationType.SplashScreen;

                    uiPage = new SplashScreenChanger();
                    break;
                case Page.AdvancedColors:
                    uiPage = new AdvancedColors();
                    break;
                case Page.Loading:
                    uiPage = new Loading();
                    break;
                case Page.About:
                    uiPage = new About();
                    break;
                default:
                    break;
            }
            if (uiPage == null)
            {
                Navigate(Page.MainMenu);
                return;
            }
            RootNav.Navigate(uiPage);
            if(page == Page.Loading)
            {
                MainWindow.GoBackDisabled = true;
            }
            
            GC.Collect();
        }
        public void GoBack()
        {
            MainWindow.GoBackDisabled = false;
            if (RootNav.NavigationService.CanGoBack)
            {
                RootNav.NavigationService.GoBack();
            }
        }
        public void UpdateText()
        {
            currentOpText.Visibility = (RootNav.Content is Loading ||
                RootNav.Content is ColorChanger ||
                RootNav.Content is SplashScreenChanger ||
                RootNav.Content is OperationSelector)
                ? Visibility.Visible : Visibility.Collapsed;


            if (CurrentOperation.SelectedPath == null)
                return;
            string text = CurrentOperation.SelectedPath.Title;
            if ((RootNav.Content is Loading || RootNav.Content is ColorChanger || RootNav.Content is SplashScreenChanger)
                && RootNav.Content is not OperationSelector)
            {
                text += $" | {CurrentOperation.operationType.GetText()}";
            }
            currentOpText.Text = text;
        }
        public void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        public static bool GoBackDisabled = false;
        private void RootNav_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (!GoBackDisabled)
                return;
            if (e.NavigationMode == NavigationMode.Back && RootNav.Content is Loading)
            {
                e.Cancel = true;
            }       
        }
        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            Navigate(Page.About);
        }
        public void CheckUpdates(EventHandler? onCheck = null)
        {
            UpdateChecker.Check((s, e) =>
            {
                UpdateUIforUpdate(e);
                onCheck?.Invoke(this, EventArgs.Empty);
            });
        }
        public void UpdateUIforUpdate(UpdateEventArgs e2)
        {            
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (e2.UpdateAvailable)
                {
                    updateAvaButton.Visibility = Visibility.Visible;
                    updateAvaButton.Click += (s, e1) =>
                    {
                        Misc.OpenUrl(UpdateChecker.CurrentUpdate.UpdateData.DownloadLink);
                    };
                }
                else
                {
                    updateAvaButton.Visibility = Visibility.Collapsed;
                }
            }));
        }
    }
}
