using JadUpdate;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UnitedAdobeEditor.Components;

namespace UnitedAdobeEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string Name = "United Adobe Editor";

        public static readonly JadUpdate.Class.Version Version = new JadUpdate.Class.Version("1.4", JadUpdate.Class.VersionTag.Beta); 
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
            string? applicationPath = System.Environment.ProcessPath;
            loadedFile = e.Args.FirstOrDefault();
            if (!string.IsNullOrEmpty(applicationPath))
            {
                Misc.AssociateFile(".uae", "uae_config", Name, applicationPath);
            }
        }
    }
}
