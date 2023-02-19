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
using UnitedAdobeEditor.Components.DebugScripts;
using UnitedAdobeEditor.Components.Enums;
using UnitedAdobeEditor.Components.Helpers;
using UnitedAdobeEditor.Components.Scripts;
using UnitedAdobeEditor.Views.Pages;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Hardware;
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

        DoubleAnimation PageAnimation = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 400)));
        ThicknessAnimation PageAnimation1 = new ThicknessAnimation()
        {
            Duration = TimeSpan.FromSeconds(0.3),
            DecelerationRatio = 0.7,
            To = new Thickness(0, 0, 0, 0),
            EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
        };

        public MainWindow()
        {
            AdminRelaunch.AdminRelauncher();
            if(Instance != null)
            {
                return;
            }
            Instance = this;

            InitializeComponent();
            Thickness resizeBorderThickness = WindowChrome.GetWindowChrome(this).ResizeBorderThickness ;
            
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - resizeBorderThickness.Top - resizeBorderThickness.Bottom + 8;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth - resizeBorderThickness.Right - resizeBorderThickness.Left + 10;
            Instances();

            devText.OnClick += (s, e) =>
            {
                Misc.OpenUrl(App.YoutubeLink);
            };

            this.Loaded += (s, e) =>
            {
                Storyboard.SetTargetName(PageAnimation, "grid");
                RootNav.Navigated += (s, e) =>
                {
                    UiPage page = (e.Content as UiPage);
                    if (page == null)
                        return;
                    page.BeginAnimation(Grid.OpacityProperty, PageAnimation);
                };
                RootNav.Navigating += (s, e) =>
                {
                    UiPage page = (e.Content as UiPage);
                    if (page == null)
                        return;
                    var ta = PageAnimation1;
                    if (e.NavigationMode == NavigationMode.New)
                    {
                        ta.From = new Thickness(20, 0, 0, 0);
                    }
                    else if (e.NavigationMode == NavigationMode.Back)
                    {
                        ta.From = new Thickness(0, 0, 20, 0);
                    }
                    page.BeginAnimation(MarginProperty, ta);
                };

                Navigate(Page.MainMenu);
                //Navigate(Page.SSCreator);

                CheckUpdates();

                combobox1.ItemsSource = Enum.GetValues(typeof(BackgroundType));
                
                BackgroundType type = BackgroundType.Auto;
                if (Wpf.Ui.Appearance.Background.IsSupported(BackgroundType.Mica))
                {
                    type = BackgroundType.Mica;
                }
                else
                {
                    type = BackgroundType.None;
                }
                type = BackgroundType.None;

                try
                {
                    Debug.WriteLine("Applying Backdrop type : " + type.ToString());
                    Wpf.Ui.Appearance.Background.Apply(this, type);
                }
                catch (Exception)
                {

                }
                Wpf.Ui.Appearance.Accent.ApplySystemAccent();

            };
            RootNav.Navigated += (o, e) => 
            {
                settingsButton.Visibility = e.Content is MainMenu ? Visibility.Visible : Visibility.Collapsed;
                BackButton.Visibility = RootNav.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
                if (e.Content is Loading)
                {
                    BackButton.Visibility = Visibility.Collapsed;
                }

                UpdateText(e.Content);
            };

            
        }
        private void Instances()
        {
            new Settings();
            new UIColorsData();
            new Components.SplashScreenChanger.SaveData();
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
        public void Navigate(Page page, object[] param = null)
        {
            UiPage uiPage = null;
            Type type = null;
            switch (page)
            {
                case Page.MainMenu:
                    uiPage = new MainMenu();
                    type = typeof(MainMenu);
                    break;
                case Page.Settings:
                    break;
                case Page.VersionSelector:
                    uiPage = new VersionSelector();
                    type = typeof(VersionSelector);
                    break;
                case Page.OperationSelector:
                    if (CurrentOperation.AppType == AdobeType.Photoshop)
                    {
                        uiPage = new OperationSelector();
                        type = typeof(OperationSelector);
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
                    type = typeof(ColorChanger);
                    break;
                case Page.SplashScreenChanger:
                    CurrentOperation.operationType = OperationType.SplashScreen;

                    uiPage = new SplashScreenChanger();
                    type = typeof(SplashScreenChanger);
                    break;
                case Page.AdvancedColors:
                    uiPage = new AdvancedColors();
                    type = typeof(AdvancedColors);
                    break;
                case Page.Loading:
                    uiPage = new Loading();
                    type = typeof(Loading);
                    break;
                case Page.About:
                    uiPage = new About();
                    type = typeof(About);
                    break;

                case Page.SSCreator:
                    uiPage = new SplashScreenCreator();
                    type = typeof(SplashScreenCreator);
                    break;
                default:
                    break;
            }
            if (uiPage == null)
            {
                Navigate(Page.MainMenu);
                return;
            }
            Debug.WriteLine("Navigating Type : " + type.ToString());
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
        public void UpdateText(object currentPage)
        {
            currentOpText.Visibility = Visibility.Collapsed;
            if (CurrentOperation.SelectedPath == null)
                return;
            string text = CurrentOperation.SelectedPath.Title;
            if ((RootNav.Content is Loading || RootNav.Content is ColorChanger || RootNav.Content is SplashScreenChanger)
                && RootNav.Content is not OperationSelector)
            {
                text += $" | {CurrentOperation.operationType.GetText()}";
            }
            currentOpText.Text = text;

            if (RootNav.Content is Loading || 
                RootNav.Content is ColorChanger ||
                RootNav.Content is SplashScreenChanger ||
                RootNav.Content is  OperationSelector)
            {
                currentOpText.Visibility = Visibility.Visible;
            }
        }
        public void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        private void debugButton_Click(object sender, RoutedEventArgs e)
        {
           /// var dialog = ColorDialogJ.Show();

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
        public void CheckUpdates(EventHandler onCheck = null)
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

        private void debugComboBox_Selected(object sender, RoutedEventArgs e)
        {
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BackgroundType type = (BackgroundType)combobox1.SelectedItem;
            try
            {
                Debug.WriteLine("Applying Backdrop type : " + type.ToString());
                Wpf.Ui.Appearance.Background.Apply(this, type);
            }
            catch (Exception)
            {

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ResourceChecker.check("C:\\Program Files\\Adobe\\Adobe Photoshop 2022");
            return;
            foreach (AdobeType apptype in ((AdobeType[])Enum.GetValues(typeof(AdobeType))))
            {
                string name = "";
                switch (apptype)
                {
                    case AdobeType.Photoshop:
                        name = "photoshop";
                        break;
                    case AdobeType.Illustrator:
                        name = "illustrator";
                        break;
                    case AdobeType.PremierePro:
                        name = "premiere_pro";
                        break;
                    case AdobeType.AfterEffects:
                        name = "after_effects";
                        break;
                    case AdobeType.Animate:
                        name = "animate";
                        break;
                    case AdobeType.Lightroom:
                        name = "lightroom";
                        break;
                    case AdobeType.LightroomClassic:
                        name = "lightroomclassic";
                        break;
                    case AdobeType.MediaEncoder:
                        name = "mediaencoder";
                        break;
                    case AdobeType.Audition:
                        name = "audition";
                        break;
                    case AdobeType.InDesign:
                        name = "indesign";
                        break;
                    case AdobeType.Dreamweaver:
                        name = "dreamweaver";
                        break;
                    case AdobeType.InCopy:
                        name = "incopy";
                        break;
                    case AdobeType.CharacterAnimator:
                        name = "ch_animator";
                        break;
                    default:
                        break;
                }
                if (!String.IsNullOrWhiteSpace(name))
                {
                    string fileName = name + ".png";
                    string file = "./Images/" + fileName;
                    string newFile = "./Images/Resized_100/";
                    if (!Directory.Exists(newFile))
                    {
                        Directory.CreateDirectory(newFile);
                    }
                    newFile += fileName;
                    System.Drawing.Image.FromFile(file).ResizeImage(new System.Drawing.Size(100,100)).Save(newFile);

                }
            }
        }
    }
}
