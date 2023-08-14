using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using UnitedAdobeEditor.Views.CustomControls;
using Wpf.Ui.Controls;

namespace UnitedAdobeEditor.Views
{
    /// <summary>
    /// ShareConfigPage.xaml etkileşim mantığı
    /// </summary>
    public partial class ShareConfigPage : UiPage
    {
        public ShareConfigPage()
        {
            InitializeComponent(); 
            foreach (var item in Config.appTypes)
            {
                apps.Items.Add(new AppSelectionItem().Set(item));
            }
            apps.SelectedIndex = 0;
        }

        private async void ShareClick(object sender, RoutedEventArgs e)
        {
            var image = imageSelector.SelectedImage;
            if (image is null)
            {
                MessageBoxJ.ShowOK("Please select an image");
                return;
            }
            var item = apps.SelectedItem as AppSelectionItem;
            if (item is null)
                return;
            bool continueChanging = false;
           await MessageBoxJ.ShowDialogFunc(
               new ShareConfirmationControl() 
               { Margin = new Thickness(0,10,0,0)}
               .Set("Are you sure you want to share this config?",image, item)
               , "Yes", () => { continueChanging = true; return true; },
               "No",() => { continueChanging = false; return true; },heightMultiplier: 5f
               );
            if (continueChanging)
            {
                MainWindow.Instance.SetLoading("Uploading image");
                Debug.WriteLine("creating");
                (bool success,string url,string error) = await PublicConfigManager.CreateConfig(Misc.ImageToBase64(image), item.Key);
                Debug.WriteLine("done");
                MainWindow.Instance.SetLoadingState(false);

                if (success && !string.IsNullOrEmpty(url))
                {
                    await MessageBoxJ.ShowOKDailog(new ShareLinkControl().SetUrl(url));
                }
                if(!success && !string.IsNullOrEmpty(error))
                    await MessageBoxJ.ShowOKDailog(error);
            }
        }
    }
}
