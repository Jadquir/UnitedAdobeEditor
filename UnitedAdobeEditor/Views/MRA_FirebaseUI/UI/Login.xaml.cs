using Firebase.Auth;
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
using UnitedAdobeEditor.Components.Firebase;
using Wpf.Ui.Controls;

namespace MRA_WPF.Views.Windows.MRA_FirebaseUI.UI
{
    /// <summary>
    /// Login.xaml etkileşim mantığı
    /// </summary>
    public partial class Login : UiPage
    {
        public Login()
        {
            InitializeComponent();
        }
        private string GetEmail() => email_textbox.Text;
        private string GetPassword() => password_textbox.Password;

        private async Task LoginAsync()
        {
            var email = GetEmail();
            if (!email.IsMail())
            {
                //DialogManager.ShowOK(,owner: MRA_Login.Instance)
                FirebaseErrors.ShowValidEmail();
                return;
            }
            var password = GetPassword();
            if (password.Length <= 6)
            {
                FirebaseErrors.EnterPasswordLongerThan(6);
                return;
            }
            try
            {
                await FirebaseAuthControl.Instance.Client.SignInWithEmailAndPasswordAsync(email, password);
            }
            catch(Firebase.Auth.FirebaseAuthHttpException e)
            {
                FirebaseErrors.ShowError(e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while logging in. Error:" + ex.Message);
            }

        }

        private void reset_passwordButton_Click(object sender, RoutedEventArgs e)
        {
            MRA_Login.Instance.NavigateToPasswordReset();
        }

        private async void login_button_Click(object sender, RoutedEventArgs e)
        {
            MRA_Login.Instance.StartProggressBar();
            await LoginAsync();
            MRA_Login.Instance.StopProggressBar();
        }

        private void register_button_Click(object sender, RoutedEventArgs e)
        {
            MRA_Login.Instance.NavigateToRegister();
        }

        private void password_textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                login_button_Click(sender, e);
            }
        }

        private void login_with_google_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
