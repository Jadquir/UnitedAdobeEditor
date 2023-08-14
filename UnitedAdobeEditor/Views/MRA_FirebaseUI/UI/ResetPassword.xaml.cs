using Firebase.Auth;
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
using UnitedAdobeEditor.Components.Firebase;
using Wpf.Ui.Controls;

namespace MRA_WPF.Views.Windows.MRA_FirebaseUI.UI
{
    /// <summary>
    /// Login.xaml etkileşim mantığı
    /// </summary>
    public partial class ResetPassword : UiPage
    {
        public ResetPassword()
        {
            InitializeComponent();
        }
        private string GetEmail() => email_textbox.Text;

        private async Task SendResetRequest()
        {
            var email = GetEmail();
            if (!email.IsMail())
            {
                FirebaseErrors.ShowValidEmail();
                return;
            }
            try
            {
                await FirebaseAuthControl.Instance.Client.ResetEmailPasswordAsync(email);
                MessageBoxJ.ShowOK("Please check your email for instructions.", owner: MRA_Login.Instance);
            }
            catch(FirebaseAuthHttpException ex)
            {
                FirebaseErrors.ShowError(ex);
            }
            catch (Exception)
            {

            }
        }
        private async void ResetClick(object sender, RoutedEventArgs e)
        {
            MRA_Login.Instance.StartProggressBar();
            await SendResetRequest();
            MRA_Login.Instance.StopProggressBar();
        }

        private void email_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ResetClick(sender, e);
            }
        }
    }
}
