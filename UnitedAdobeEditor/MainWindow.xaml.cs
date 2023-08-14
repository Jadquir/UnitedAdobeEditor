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
using UnitedAdobeEditor.Components.Firebase;
using UnitedAdobeEditor.Components.Helpers;
using UnitedAdobeEditor.Components.Scripts;
using UnitedAdobeEditor.Views;
using UnitedAdobeEditor.Views.CustomControls;
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

        public  string CreateSplashScreenLink = App.CreateSplashScreenLink;
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

                if (!string.IsNullOrEmpty(App.loadedFile))
                {
                    _ = MainMenu.LoadFile(App.loadedFile);
                }
                if (!string.IsNullOrEmpty(App.ConfigId) && App.NavigateToConfig)
                {
                    _ = UseConfig();
                }
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
            Wpf.Ui.Appearance.Background.Apply(this, GetBackgroundType());
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
                case Page.ExploreConfigs:
                    uiPage = new ExploreConfigsPage();
                    break;
                case Page.ShareConfigPage:
                    uiPage = new ShareConfigPage();
                    break;
                case Page.OperationSelector:
                    if ((CurrentOperation.AppType == AdobeType.Photoshop || CurrentOperation.AppType == AdobeType.PhotoshopBeta)
                        && !CurrentOperation.IsConfigActivated)
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
                case Page.CreateConfig:
                    uiPage = new CreateConfig();
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
            CurrentOperation.IsConfigActivated = false;
            CurrentOperation.isRunStat = false;
            MainWindow.GoBackDisabled = false;
            if (RootNav.NavigationService.CanGoBack)
            {
                RootNav.NavigationService.GoBack();
            }
        }
        public void ChangeVisibility(bool isVisible)
        {
            WindowState = isVisible ? (WindowState == WindowState.Maximized ? WindowState.Maximized : WindowState.Normal) : WindowState.Minimized;
            Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            if(isVisible)
            {
                Show();
                Topmost = true;
                Topmost = false;
            }
            else
            {
                Hide();
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
            if (e.NavigationMode == NavigationMode.Back )
            {
                CurrentOperation.isRunStat = false;
                CurrentOperation.IsConfigActivated = false;
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

        internal static BackgroundType GetBackgroundType()
        {
            return Wpf.Ui.Appearance.Background.IsSupported(BackgroundType.Mica) ?
                BackgroundType.Mica : BackgroundType.None;
        }

        internal void NavigateBack()
        {
            if (RootNav.NavigationService.CanGoBack)
            {
                RootNav.NavigationService.GoBack();
            }
        }
        public void SetLoadingTexts(string? upperText = "", string? lowerText = "")
        {
            loadingText.Text = upperText ?? "";
            loadingText1.Text = lowerText ?? "";

            loadingText.Visibility = string.IsNullOrWhiteSpace(upperText).VisibleIfFalse();
            loadingText1.Visibility = string.IsNullOrWhiteSpace(lowerText).VisibleIfFalse();
        }
        public void SetLoading(string text = "", bool fade = true)
        {
            SetLoadingTexts(text);
            SetLoadingState(true, fade);
        }
        public void SetLoadingState(bool isLoading, bool fade = true)
        {
            if (!MainWindow.Instance.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    SetLoadingState(isLoading, fade);
                }));
                return;
            }

            if (isLoading)
            {
                if (fade)
                {
                    if (LoadingPanel.Visibility == Visibility.Collapsed)
                        LoadingPanel.FadeIn(150);
                }
                else
                {
                    LoadingPanel.Visibility = Visibility.Visible;
                }
                grid.Effect = new System.Windows.Media.Effects.BlurEffect()
                {
                    Radius = 25,
                    KernelType = System.Windows.Media.Effects.KernelType.Gaussian,
                };
            }
            else
            {
                if (fade)
                {
                    if (LoadingPanel.Visibility == Visibility.Visible)
                        LoadingPanel.FadeOut(150);
                }
                else
                {
                    LoadingPanel.Visibility = Visibility.Collapsed;
                }
                grid.Effect = null;
            }
        }

        internal async Task UseConfig()
        {
            try
            {
                if (string.IsNullOrEmpty(App.ConfigId) || !App.NavigateToConfig)
                {
                    return;
                }
                var config = await PublicConfigManager.GetConfig(App.ConfigId);
                if (config == null)
                    return;

               
                var item = new AppSelectionItem().Set(new KeyValuePair<string, AdobeType>(config.AppType, Config.appTypes[config.AppType]));
                var image = await config.GetResizedImage();
                bool continueChanging = false;
                await MessageBoxJ.ShowDialogFunc(
                    new ShareConfirmationControl()
                    { Margin = new Thickness(0, 10, 0, 0) }
                    .Set("Are you sure you want to use this config?", image, item)
                    , "Yes", () => { continueChanging = true; return true; },
                    "No", () => { continueChanging = false; return true; }, heightMultiplier: 5f,LeftPrimary:true
                    );

                if(continueChanging)
                {
                    await PublicConfigItem.RunConfig(config);
                }
            }
            catch (Exception)
            {

            }
          
        }
    }
}
