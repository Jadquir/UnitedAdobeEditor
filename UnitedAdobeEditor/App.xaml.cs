using JadUpdate;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace UnitedAdobeEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string Name = "United Adobe Editor";

        public static readonly JadUpdate.Class.Version Version = new JadUpdate.Class.Version("1.3", JadUpdate.Class.VersionTag.Beta); 
        public static string UpdateUrl = "https://raw.githubusercontent.com/Jadquir/uae-files/main/updates.json";

        public static string YoutubeLink = "https://www.youtube.com/channel/UCfFGFB1flw00AiHw_4xYvPw?sub_confirmation=1";
        public static string DiscordLink = "https://discord.com/invite/4RsEywFGQn";
        public static string ItchioLink = "https://jadquir.itch.io/";
        public static string WebsiteLink = "https://jadquir.github.io/";
        public static string MailLink = "mailto:jadquircontact@gmail.com";
        public static string SupportUrl = "https://www.buymeacoffee.com/Jadquir";
    }
}
