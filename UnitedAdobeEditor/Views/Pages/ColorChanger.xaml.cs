using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using UnitedAdobeEditor.Components;
using UnitedAdobeEditor.Components.ColorChanger;
using UnitedAdobeEditor.Components.Enums;
using Wpf.Ui.Controls;
using Color = System.Windows.Media.Color;

namespace UnitedAdobeEditor.Views.Pages
{
    /// <summary>
    /// ColorChanger.xaml etkileşim mantığı
    /// </summary>
    public partial class ColorChanger : UiPage
    {
        private Color MainColor;
        public ColorChanger()
        {
            InitializeComponent();

            mainColorHolder.OnClick += (s, e) =>
            {
                var color = ColorDialogJ.Show(MainColor);
                if (!color.Item1) { return; }
                Debug.WriteLine("changing bg");
                mainColorHolder.Background = new SolidColorBrush(color.Item2);
                MainColor = color.Item2;
            };

            MainColor = UIColorsData.Instance.MainColor.Color.ToMediaColor();
            mainColorHolder.Background = new SolidColorBrush(MainColor);

            advancedActiveCheckBox.IsChecked = UIColorsData.Instance.IsAdvancedActivated;
            CheckBox_Checked(this, null);

            photoshopThemeComboBox.ItemsSource = Enum.GetValues(typeof(DesiredTheme));
            photoshopThemeComboBox.SelectedIndex = (int)UIColorsData.Instance.desiredTheme - 2;            
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!advancedActiveCheckBox.IsChecked.HasValue)
            {
                advancedActiveCheckBox.IsChecked = false;
            }
            advancedEditButton.IsEnabled = advancedActiveCheckBox.IsChecked.Value;
        }

        private void advancedEditButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Navigate(Components.Enums.Page.AdvancedColors);
        }

        private void SaveSettings()
        {
            UIColorsData.Instance.IsAdvancedActivated = advancedActiveCheckBox.IsChecked.Value;
            UIColorsData.Instance.MainColor.Color = MainColor.ToDrawingColor();
            UIColorsData.Instance.desiredTheme = (DesiredTheme)(photoshopThemeComboBox.SelectedIndex + 2);

            UIColorsData.Save();
        }
        private async void applyButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();

            Debug.WriteLine("Changing to " + MainColor.ToString()); 
            await Components.ColorChanger.ColorChanger.Change();
        }

        private void restoreButton_Click(object sender, RoutedEventArgs e)
        {
            (bool, string) restore = Components.ColorChanger.ColorChanger.Restore();
            if (!restore.Item1)
            {
                MainWindow.Instance.ShowSnackBar("Error", restore.Item2, Wpf.Ui.Common.ControlAppearance.Danger, Wpf.Ui.Common.SymbolRegular.ErrorCircle24);
                return;
            }
            MessageBoxJ.ShowOK(restore.Item2);
            
        }
    }
}
