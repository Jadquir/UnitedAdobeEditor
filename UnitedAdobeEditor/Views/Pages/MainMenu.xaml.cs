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
                var operation = config.ToOperation(out string error); 
                if (operation is null)
                {
                    await MessageBoxJ.ShowOKAsync(error);
                    return;
                }
                CurrentOperation.IsConfigActivated = true;
                CurrentOperation.Operation = operation;
                CurrentOperation.operationType = operation.operationType;
                CurrentOperation.AppType = operation.AppType;
                MainWindow.Instance.Navigate(Components.Enums.Page.VersionSelector);

            }
            catch (Exception ex)
            {

                await MessageBoxJ.ShowOKAsync("Something went wrong!\nError: " + ex.Message);
            }

        }
        private void openCreateLink(object sender, RoutedEventArgs e)
        {
            Misc.OpenUrl(App.CreateSplashScreenLink);
        }
    }
}
