
using Newtonsoft.Json.Linq;
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
    /// Register.xaml etkileşim mantığı
    /// </summary>
    public partial class Register : UiPage
    {
        public Register()
        {
            InitializeComponent();
        }

        private void password_KeyDown(
            object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Button_Click(sender, e);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await RegisterAsync();
        }
        private async Task RegisterAsync()
        {
            var e = email.Text;
            var username = fullname.Text;
            var pass = password.Password;
            if(String.IsNullOrWhiteSpace(e) || String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(pass))
            {
                FirebaseErrors.ShowFillTheBlanks();
                return;
            }
            if (!e.IsMail())
            {
                FirebaseErrors.ShowValidEmail();
                return;
            }
            if(pass.Length <= 6)
            {
                FirebaseErrors.EnterPasswordLongerThan(6);
               return;
            }
            if(username.Length < 3)
            {
                FirebaseErrors.EnterUsernameLongerThan(2);
                return;
            }
            MRA_Login.Instance.StartProggressBar();
            try
            {
                await FirebaseAuthControl.Instance.Client.CreateUserWithEmailAndPasswordAsync(e, pass, username);
            }
            catch (Firebase.Auth.FirebaseAuthHttpException ex)
            {
                FirebaseErrors.ShowError(ex);
            }
            MRA_Login.Instance.StopProggressBar();
        }
    }
}
