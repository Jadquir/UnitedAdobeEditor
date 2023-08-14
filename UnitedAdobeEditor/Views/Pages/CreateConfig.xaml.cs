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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnitedAdobeEditor.Components;
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Components.Helpers;
using UnitedAdobeEditor.Views.CustomControls;
using Wpf.Ui.Controls;

namespace UnitedAdobeEditor.Views.Pages
{
    /// <summary>
    /// CreateConfig.xaml etkileşim mantığı
    /// </summary>
    public partial class CreateConfig : UiPage
    {
        public CreateConfig()
        {
            InitializeComponent();
            foreach (var item in Config.appTypes)
            {
                apps.Items.Add(new AppSelectionItem().Set(item));
            }
            apps.SelectedIndex = 0;
            apps.SelectionChanged += Apps_SelectionChanged;
            enableSelectedPath.Checked += EnableSelectedPath_Checked;
            enableSelectedPath.Unchecked += EnableSelectedPath_Checked;
        }

        private void EnableSelectedPath_Checked(object sender, RoutedEventArgs e)
        {
            bool isChecked = enableSelectedPath.IsChecked.GetValueOrDefault(false);
            selectedPathText.Visibility = isChecked
                ? Visibility.Visible : Visibility.Collapsed;
            if (!isChecked)
            {
                selectedPath = null;
            }
            UpdateSelectedPathText();
        }

        private void Apps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedPath = null;
            UpdateSelectedPathText();
        }

        private void UpdateSelectedPathText()
        {
            selectedPathText.Text = $"Selected Path: {selectedPath?.EXEFilePath ?? ""}";
            selectedPathText.Visibility = !string.IsNullOrEmpty(selectedPath?.EXEFilePath) ? Visibility.Visible : Visibility.Collapsed;
        }

        private SelectedPath? selectedPath;
        private void CreateClick(object sender, RoutedEventArgs e)
        {
            var config = new Config();

            var selectedAppType = apps.SelectedItem as AppSelectionItem;
            if(selectedAppType == null )
            {
                MessageBoxJ.ShowOK("Please select an Adobe Application!");
                return;
            }
            if (imageSelector.SelectedImage == null)
            {
                MessageBoxJ.ShowOK("Please select an Image!");
                return;
            }
            if (enableSelectedPath.IsChecked.GetValueOrDefault(false))
            {

                MessageBoxJ.ShowOK("Please select a path or disable \"Run with Selected Path\"!");
                return;
            }
            config.app_type = selectedAppType.Key;
            config.image_base64 = Misc.ImageToBase64(imageSelector.SelectedImage);
            config.is_silent = runSilentCheckBox.IsChecked.GetValueOrDefault(false);
            config.closeAfterChanging = closeAfterChanging.IsChecked.GetValueOrDefault(false);
            config.selected_folder = selectedPath?.EXEFilePath ?? string.Empty;
            SaveConfigFile(config);
        }
        public static void SaveConfigFile(Config config,string filename = "SplashScreenConfig.uae")
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "UAE files (*.uae)|*.uae";
            saveFileDialog.FileName = filename;

            var result = saveFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                string filePath = saveFileDialog.FileName;
                File.WriteAllText(filePath, JsonConvert.SerializeObject(config));
            }

        }
        private void SelectPath(object sender, RoutedEventArgs e)
        {
            var selectedAppType = apps.SelectedItem as AppSelectionItem;
            if (selectedAppType == null)
            {
                MessageBoxJ.ShowOK("Please select an Adobe Application!");
                return;
            }
            SelectedPath selectedPath = PathSelector.Select(selectedAppType.AppType);
            if (selectedPath == null || !selectedPath.SuccessInit)
            {
                return;
            }
            this.selectedPath = selectedPath;
            UpdateSelectedPathText();
        }
    }
}
