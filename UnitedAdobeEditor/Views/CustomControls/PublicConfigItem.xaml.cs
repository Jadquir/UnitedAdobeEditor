using Microsoft.Win32;
using Newtonsoft.Json;
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
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Components.Firebase;
using UnitedAdobeEditor.Views.Pages;
using Wpf.Ui.Controls;

namespace UnitedAdobeEditor.Views.CustomControls
{
    /// <summary>
    /// PublicConfigItem.xaml etkileşim mantığı
    /// </summary>
    public partial class PublicConfigItem : UserControl
    {
        private ExploreConfigsPage.PublicConfig config;
        public PublicConfigItem()
        {
            InitializeComponent(); isLoaded = false;
        }
        public bool isLoaded { get;private set; }
        internal PublicConfigItem Set(ExploreConfigsPage.PublicConfig item)
        {
            config = item;
            UpdateUIInfo();
            return this;
        }
        private static readonly TimeSpan ms = TimeSpan.FromMilliseconds(10);
        public async Task WaitForIsLoadedAsync()
        {
            while (!isLoaded)
            {
                // You might want to introduce a delay here to avoid busy-waiting
                await Task.Delay(ms);
            }
        }
        private void UpdateUIInfo()
        {
            Dispatcher.Invoke(async () =>
            {
                Owner.Text = "Created By: " + config.OwnerName;
                downloadCount.Text = config.stats.run_config.ToString();
                app_type.Set(Config.appTypes.FirstOrDefault(x => x.Key == config.AppType));
                SplashScreen.Source = Misc.ConvertDrawingImageToBitmapImage( await config.GetResizedImage());
                copyMenuItem.Visibility = string.IsNullOrEmpty(config.url).VisibleIfFalse();
                isLoaded = true;
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Wpf.Ui.Controls.Button;
            if(button != null)
            {
                button.ContextMenu.IsOpen = !button.ContextMenu.IsOpen;
            }
        }

        private void Copy_click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(config.url); 
            MainWindow.Instance.ShowSnackBar("Copied shareable link!", "Copied!", icon: Wpf.Ui.Common.SymbolRegular.Share16);
        }

        private void DownloadConfigClick(object sender, RoutedEventArgs e)
        {
            _ = PublicConfigManager.UpdateStat(config.Id, PublicConfigManager.StatType.download_config);
            CreateConfig.SaveConfigFile(new Config() { app_type = config.AppType , image =  config.Image }, $"Splash Screen Config by {config.OwnerName}.uae");
        }
        private bool isGetting;
        private async void DownloadSplashSCreenClick(object sender, RoutedEventArgs e)
        {
            if (isGetting)
                return;
            isGetting = true;
            var image = Task.Run(()=> config.GetFullSizeImage());

            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG files (*.png)|*.png";
            saveFileDialog.FileName = $"Splash Screen by {config.OwnerName}.png";

            var result = saveFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                string filePath = saveFileDialog.FileName;
                (await image).Save(filePath);
                _ = PublicConfigManager.UpdateStat(config.Id, PublicConfigManager.StatType.download_ss);
            }
            isGetting = false;
        }
        public class PublicCOnfig : Config
        {
            public System.Drawing.Image SplashScreenImage;
            public override System.Drawing.Image? GetImage()
            {
                return SplashScreenImage;
            }
        }
        private async void RunConfigClick(object sender, RoutedEventArgs e)
        {
           await RunConfig(config);
        }
        public static async Task RunConfig(ExploreConfigsPage.PublicConfig config)
        {
            CurrentOperation.isRunStat = true;
            CurrentOperation.runStatId = config.Id;
            await MainMenu.RunConfig(new PublicCOnfig()
            {
                app_type = config.AppType,
                SplashScreenImage = await config.GetFullSizeImage()
            });
        }
    }
}
