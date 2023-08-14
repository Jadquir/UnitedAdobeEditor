using JadUpdate;
using Microsoft.Win32;
using SingleInstanceCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using UnitedAdobeEditor.Components;
using UnitedAdobeEditor.Views.Pages;

namespace UnitedAdobeEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstance
    {
        public static readonly string Name = "United Adobe Editor";

        public static readonly JadUpdate.Class.Version Version = new JadUpdate.Class.Version("1.5", JadUpdate.Class.VersionTag.Beta); 
        public static string UpdateUrl = "https://raw.githubusercontent.com/Jadquir/uae-files/main/updates.json";

        public static readonly string YoutubeLink = "https://www.youtube.com/channel/UCfFGFB1flw00AiHw_4xYvPw?sub_confirmation=1";
        public static readonly string DiscordLink = "https://discord.com/invite/4RsEywFGQn";
        public static readonly string ItchioLink = "https://jadquir.itch.io/";
        public static readonly string WebsiteLink = "https://jadquir.github.io/";
        public static readonly string MailLink = "mailto:jadquircontact@gmail.com";
        public static readonly string SupportUrl = "https://www.buymeacoffee.com/Jadquir";
        public static readonly string CreateSplashScreenLink = "https://bit.ly/47jaqS7";


        public static string? loadedFile;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool isFirstInstance = this.InitializeAsFirstInstance(Name);
            if (!isFirstInstance)
            {
                TryKillApp();
            }
            string? applicationPath = System.Environment.ProcessPath;
            HandleArgs(e.Args,false);
            //loadedFile = e.Args.FirstOrDefault();
            if (!string.IsNullOrEmpty(applicationPath))
            {
                Misc.AssociateFile(".uae", "uae_config", Name, applicationPath);
                RegistUrlScheme("uae", applicationPath);
            }
            SetUseConfig(e.Args);
        }
        public static bool NavigateToConfig { get; private set; }
        public static string ConfigId { get; private set; } = "";
        private static void SetUseConfig(string[] e)
        {
            var match = e.FirstOrDefault(x => x.StartsWith("uae://config/"));
            if (string.IsNullOrEmpty(match))
            {
                NavigateToConfig = false;
                return;
            }

            string[] parts = match.Split('/');
            string id = parts[parts.Length - 1];
            ConfigId = id;
            NavigateToConfig = true;
        }
        private static void RegistUrlScheme(string uriScheme, string applicationPath)
        {
            try
            {
                //var mainKey = Registry.ClassesRoot.CreateSubKey(protocolName);
                var mainKey = Registry.CurrentUser.CreateSubKey($"Software\\Classes\\{uriScheme}");
                mainKey.SetValue("", $"URL:" + App.Name);
                mainKey.SetValue("URL Protocol", "");

                var defaultIcon = mainKey.CreateSubKey("DefaultIcon");
                defaultIcon.SetValue("", Application.Current.StartupUri.ToString());
                defaultIcon.SetValue("", $"\"{applicationPath}\"");

                var shellKey = mainKey.CreateSubKey("shell");
                var openKey = shellKey.CreateSubKey("open");
                var commandKey = openKey.CreateSubKey("command");
                commandKey.SetValue("", $"\"{applicationPath}\" \"%1\"");
            }
            catch (UnauthorizedAccessException ex)
            {

            }
            catch (Exception ex)
            {
            }
        }
        public static void TryKillApp()
        {
            Application.Current.Shutdown();

            SingleInstance.Cleanup();
            Process.GetCurrentProcess().Kill();

            SingleInstance.Cleanup();
            Process.GetCurrentProcess().Kill();
        }


        private void HandleArgs(string[]? args,bool runNow)
        {
            loadedFile = null;
            var file = args?.FirstOrDefault();
            if( file != null && File.Exists(file)) 
            {
                loadedFile = file;
            }
            if(runNow && loadedFile != null)
            {
                _ = MainMenu.LoadFile(loadedFile);
            }
        }
        public void OnInstanceInvoked(string[] args_arr)
        {
            UnitedAdobeEditor.MainWindow.Instance.Dispatcher.Invoke(() =>
            {
                UnitedAdobeEditor.MainWindow.Instance.ChangeVisibility(true);
                int newSize = args_arr.Length - 1;
                if (newSize <= 0)
                {
                    return;
                }
                var new_args = new string[newSize];
                Array.Copy(args_arr, 1, new_args, 0, newSize);
                HandleArgs(new_args, true);
                SetUseConfig(new_args);
                if (NavigateToConfig)
                {
                    _ = UnitedAdobeEditor.MainWindow.Instance.UseConfig();
                }
            });
           
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            SingleInstance.Cleanup();

        }
    }
}
