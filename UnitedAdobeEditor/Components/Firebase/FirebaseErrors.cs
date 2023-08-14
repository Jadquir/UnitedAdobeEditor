using Firebase.Auth;
using MRA_WPF.Views.Windows.MRA_FirebaseUI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitedAdobeEditor.Components.Firebase
{
    public class FirebaseErrors
    {
        private static void Show(string text)
        {
            MainWindow.Instance.Dispatcher.Invoke(() =>
            {
                MessageBoxJ.ShowOK(text, owner: MRA_Login.Instance);
            });

        }
        public static void ShowError(FirebaseAuthHttpException ex)
        {
            var token = JToken.Parse(ex.ResponseData);
            var message = token["error"]?.Value<string>("message");
            var text = GetErrorMessage(message);
            if (!string.IsNullOrEmpty(text))
                Show(text);

        }

        private static string GetErrorMessage(string? message)
        {
            if (message == null) return  string.Empty;
            if (message == "Permission Denied!") return message;
            var str = message switch
            {
                "EMAIL_EXISTS" => "Email already exists. Please try another email.",
                "EMAIL_NOT_FOUND" => "Email not found. Please create an account.",
                "INVALID_PASSWORD" => "Password was incorrect. Please try again.",
                _ => message
            };
            return str;
        }
        public static void ShowFillTheBlanks()
        {
            Show("Please fill all the blank areas.");
        }
        public static void ShowValidEmail()
        {
            Show("Please enter a valid email address!");
        }

        internal static void EnterPasswordLongerThan(int v)
        {
            Show(string.Format("Please enter a password that is longer than {0} characters!", v));
        }

        internal static void EnterUsernameLongerThan(int v)
        {
            Show(string.Format("Please enter a username that is longer than {0} characters!",v));
        }

    }

}
