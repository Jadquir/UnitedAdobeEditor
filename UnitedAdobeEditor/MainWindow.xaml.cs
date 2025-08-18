using JadUpdate.Class;
using Newtonsoft.Json;
using ProtoBuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Wpf.Ui;
using Wpf.Ui.Animations;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Hardware;
using WpfScreenHelper;
using ColorChanger = UnitedAdobeEditor.Views.Pages.ColorChanger;
using NavigatingCancelEventArgs = System.Windows.Navigation.NavigatingCancelEventArgs;
using PageEnum = UnitedAdobeEditor.Components.Enums.PageEnum;

namespace UnitedAdobeEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow
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

            snackbarService.SetSnackbarPresenter(this.snackbarPresenter);
            contentDialogService.SetContentPresenter(this.dialogContentPresenter);

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
                Navigate(PageEnum.MainMenu);
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

                Wpf.Ui.Animations.TransitionAnimationProvider.
                ApplyTransition(e.Content, Wpf.Ui.Animations.Transition.FadeInWithSlide, 150);
                UpdateText();
            };

            this.ContentRendered += MainWindow_ContentRendered;

            this.MinWidth = 580;
            this.MinHeight = 450;
        }

        private void MainWindow_ContentRendered(object? sender, EventArgs e)
        {
            this.ApplyBackground();
            //Wpf.Ui.Appearance.Background.Apply(this, GetBackgroundType());
            //Wpf.Ui.Appearance.Accent.ApplySystemAccent();
        }

        private void Instances()
        {
            _ = new Settings();
            _ = new UIColorsData();
            _ = new Components.SplashScreenChanger.SaveData();
        }

        //public void ShowSnackBar(string title, string text,
        //    Wpf.Ui.Common.ControlAppearance appearance = Wpf.Ui.Common.ControlAppearance.Dark,
        //    Wpf.Ui.Common.SymbolRegular icon = Wpf.Ui.Common.SymbolRegular.CheckmarkCircle32,
        //    float seconds = 2f)
        //{
        //    snackbar.Appearance = appearance;
        //    snackbar.Icon = icon;
        //    snackbar.Timeout = (int)(seconds * 1000);
        //    snackbar.Show(title, text);
        //}
        #region Snackbar
        ISnackbarService snackbarService = new SnackbarService();
        public void ShowSnackBar(string title,
            string text,
            Wpf.Ui.Controls.SymbolRegular icon = Wpf.Ui.Controls.SymbolRegular.Empty,
            int timeout = 4200,
            ControlAppearance appearance =
            ControlAppearance.Secondary)
        {
            ShowSnackBar(text, title, new SymbolIcon(icon), timeout, appearance);
        }
        public void ShowSnackBar(string title,
            string text, 
            IconElement? icon,
            int timeout = 4200,
            ControlAppearance appearance =
            ControlAppearance.Secondary)
        {
            ShowSnackBar(title, text, icon, TimeSpan.FromMilliseconds(timeout), appearance);
        }
        public void ShowSnackBar(string title,
            string text,
            IconElement? icon,
            TimeSpan? timeout = null,
            ControlAppearance appearance =
            ControlAppearance.Secondary)
        {
            if (!timeout.HasValue)
                timeout = TimeSpan.FromMilliseconds(1000);
            snackbarService.Show(text, title, appearance, icon, timeout.Value);
        }
        #endregion


        #region Dialog
        IContentDialogService contentDialogService = new ContentDialogService();
        internal IContentDialogService GetDialog()
        {
            return contentDialogService;
        }
        #endregion

        public void Navigate(PageEnum page, object[]? param = null)
        {
            Page? uiPage = null;
            switch (page)
            {
                case PageEnum.MainMenu:
                    uiPage = new MainMenu();
                    break;
                case PageEnum.Settings:
                    break;
                case PageEnum.VersionSelector:
                    uiPage = new VersionSelector();
                    break;
                case PageEnum.ExploreConfigs:
                    uiPage = new ExploreConfigsPage();
                    break;
                case PageEnum.ShareConfigPage:
                    uiPage = new ShareConfigPage();
                    break;
                case PageEnum.OperationSelector:
                    if ((CurrentOperation.AppType == AdobeType.Photoshop || CurrentOperation.AppType == AdobeType.PhotoshopBeta)
                        && !CurrentOperation.IsConfigActivated)
                    {
                        uiPage = new OperationSelector();
                    }
                    else
                    {
                        Navigate(PageEnum.SplashScreenChanger, param);
                        return;
                    }
                    break;
                case PageEnum.ColorChanger:
                    CurrentOperation.operationType = OperationType.UIColor;
                    uiPage = new ColorChanger();
                    break;
                case PageEnum.SplashScreenChanger:
                    CurrentOperation.operationType = OperationType.SplashScreen;

                    uiPage = new SplashScreenChanger();
                    break;
                case PageEnum.AdvancedColors:
                    uiPage = new AdvancedColors();
                    break;
                case PageEnum.CreateConfig:
                    uiPage = new CreateConfig();
                    break;
                case PageEnum.Loading:
                    uiPage = new Loading();
                    break;
                case PageEnum.About:
                    uiPage = new About();
                    break;
                default:
                    break;
            }
            if (uiPage == null)
            {
                Navigate(PageEnum.MainMenu);
                return;
            }
            RootNav.Navigate(uiPage);
            if(page == PageEnum.Loading)
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
            titleHeader.Text = App.Name;

            string text = "UAE";
            if(RootNav.Content is VersionSelector versionSelector)
            {
                text += " | Adobe " + versionSelector.GetAppType().ToFriendlyString();
            }
            else if (RootNav.Content is OperationSelector operationSelector)
            {
                text += " | " + CurrentOperation.SelectedPath.Title;
            }
            if (RootNav.Content is ColorChanger || 
                RootNav.Content is SplashScreenChanger || 
                RootNav.Content is Loading ||
                RootNav.Content is AdvancedColors 
                )
            {

                text += $" | {CurrentOperation.SelectedPath.Title} | {CurrentOperation.operationType.GetText()}";
            }

          
            titleHeader.Text = text;
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
            Navigate(PageEnum.About);
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

        public static WindowBackdropType GetBackgroundType()
        {
            if (WindowBackdrop.IsSupported(WindowBackdropType.Mica))
                return WindowBackdropType.Mica;
            return WindowBackdropType.None;
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

        private void FluentWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = sender as Window;

            //if (window.Width < window.MinWidth)
            //    window.Width = window.MinWidth;

            //if (window.Height < window.MinHeight)
            //    window.Height = window.MinHeight;
        }

        private void RootNav_Navigated(object sender, NavigationEventArgs e)
        {
            ScrollToTop();
        }

        internal void ScrollToTop()
        {
            Dispatcher.Invoke(() =>
            {
                scrollViewer.ScrollToHome();
            });
        }
        //protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        //{
        //    base.OnRenderSizeChanged(sizeInfo);
        //    sizeInfo.WidthChanged
        //}
    }
}
