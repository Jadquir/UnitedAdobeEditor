using Firebase.Auth;
using MRA_WPF.Views.Windows.MRA_FirebaseUI.UI;
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
using System.Windows.Shapes;
using UnitedAdobeEditor;
using UnitedAdobeEditor.Components;
using UnitedAdobeEditor.Components.Firebase;
using Wpf.Ui.Controls;

namespace MRA_WPF.Views.Windows.MRA_FirebaseUI
{
    /// <summary>
    /// MRA_Login.xaml etkileşim mantığı
    /// </summary>
    public partial class MRA_Login : UiWindow
    {
        private readonly Register register;
        private readonly Login login;
        private readonly ResetPassword resetPassword;


        public static MRA_Login Instance;
        public MRA_Login()
        {
            InitializeComponent();
            Instance = this;
            StopProggressBar();

            register = new Register();
            login = new Login();
            resetPassword = new ResetPassword();

            this.ContentRendered += MRA_Login_ContentRendered;

            this.Owner = MainWindow.Instance;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.Topmost = MainWindow.Instance.Topmost;

            FirebaseAuthControl.Instance.Client.AuthStateChanged += Client_AuthStateChanged;
            this.Unloaded += MRA_Login_Unloaded;

            frame.Navigated += Frame_Navigated;

            NavigateToLogin();
        }

        private void MRA_Login_ContentRendered(object? sender, EventArgs e)
        {
            this.ApplyBackground();
        }

        private void Frame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            BackButton.Visibility = e.Content is Login ? Visibility.Collapsed : Visibility.Visible;
            var title = "Login";
            if(e.Content  is ResetPassword) { title = "Reset Password"; }
            else if (e.Content is Register) { title = "Register"; }

            this.Title = $"{App.Name} | {title}";
        }

        private void MRA_Login_Unloaded(object sender, RoutedEventArgs e)
        {
            FirebaseAuthControl.Instance.Client.AuthStateChanged -= Client_AuthStateChanged;
        }


        private void Client_AuthStateChanged(object? sender, UserEventArgs e)
        {
            Debug.WriteLine("User : " + e.User);
            if(e.User != null)
            {
                Debug.WriteLine("User Uid: " + e.User.Uid);
                Debug.WriteLine("User DisplayName: " + e.User.Info.DisplayName);
                Debug.WriteLine("User Email: " + e.User.Info.Email);
                
                Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.Close();
            }));
            }
            

        }

        public void NavigateToRegister()
        {
            frame.Navigate(register);
        }
        public void NavigateToLogin()
        {
            frame.Navigate(login);
        }
        public void NavigateToPasswordReset()
        {
            frame.Navigate(resetPassword);
        }

        internal void StartProggressBar()
        {
            progressbar.Visibility = Visibility.Visible;
            frame.IsEnabled = false;
        }

        internal void StopProggressBar()
        {
            progressbar.Visibility = Visibility.Collapsed;
            frame.IsEnabled = true;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToLogin();
        }
    }
}
