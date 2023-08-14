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
using UnitedAdobeEditor.Components.Scripts;
using Wpf.Ui.Controls;
using JadUpdate.Class;
using Version = JadUpdate.Class.Version;
using UnitedAdobeEditor.Components;
using System.Diagnostics;
using System.IO;
using UnitedAdobeEditor.Components.Firebase;
using MRA_WPF.Views.Windows.MRA_FirebaseUI;

namespace UnitedAdobeEditor.Views.Pages
{
    /// <summary>
    /// About.xaml etkileşim mantığı
    /// </summary>
    public partial class About : UiPage
    {
        public static About Instance { get; set; }  
        public About()
        {
            InitializeComponent();
            Instance = this;
            youtubeButton.Link = App.YoutubeLink;
            youtubeButton.SetIcon("youtube");

            discordButton.Link = App.DiscordLink;
            discordButton.SetIcon("discord");

            websiteButton.Link = App.WebsiteLink;
            websiteButton.SetIcon("website");

            itchioButton.Link = App.ItchioLink;
            itchioButton.SetIcon("itchio");

            mailButton.Link = App.MailLink;
            mailButton.SetIcon("mail");

            UpdateUIforUpdates();

            tabControl.SelectionChanged += TabControl_SelectionChanged;
            UpdateUIForUser();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (userTabItem.IsSelected)
            {
                UpdateUIForUser();
            }
        }
        private void LoggedInState(bool loggedIn)
        {
            notLoggedinPanel.Visibility = loggedIn.VisibleIfFalse();
            userInfoPanel.Visibility = loggedIn.VisibleIfTrue();
        }
        private void UpdateUIForUser()
        {
            var client = FirebaseAuthControl.Instance?.Client;
            if (client is null)
            {
                LoggedInState(false);
                return;
            }
            var user = client.User;
            if (user is null)
            {
                LoggedInState(false);
                return;
            }
            LoggedInState(true);
            username.Text = user.Info.DisplayName;
            email.Text = user.Info.Email;
        }

        public void UpdateUIforUpdates()
        {
            UpdateText.Text = "Current Version : " + App.Version.GetVersionText();
            Debug.WriteLine("OnView");
            if (UpdateChecker.CurrentUpdate != null)
            {
                Debug.WriteLine("OnView.UpdateChecker.CurrentUpdate.UpdateAvailable");
                /*
                UpdateChecker.CurrentUpdate.UpdateData.UpdateChangeLog.Add(App.Version);
                string file = Newtonsoft.Json.JsonConvert.SerializeObject(UpdateChecker.CurrentUpdate.UpdateData, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText("updates.json", file);
                */
                if (UpdateChecker.CurrentUpdate.UpdateAvailable)
                {
                    UpdateButton.Content = "Download Update";
                    UpdateText.Text += " | New Version : " + UpdateChecker.CurrentUpdate.UpdateData.UpdateChangeLog.GetLatest().GetVersionText();
                }
                else
                {
                    UpdateButton.Content = "Check Updates";
                }
                CreateChangelog();
            }
            Debug.WriteLine("OnView.EnableCheckUpdates");
            EnableCheckUpdates(!UpdateChecker.IsCheckingUpdates);
        }

        private void CheckUpdates(object sender, RoutedEventArgs e)
        {
            if (UpdateChecker.CurrentUpdate != null && UpdateChecker.CurrentUpdate.UpdateAvailable)
            {
                Misc.OpenUrl(UpdateChecker.CurrentUpdate.UpdateData.DownloadLink);
            }

            EnableCheckUpdates(false);

            MainWindow.Instance.CheckUpdates((s,e) =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
               
                UpdateUIforUpdates();
                }));
            });
        }

        public void EnableCheckUpdates(bool enable)
        {
            Debug.WriteLine("EnableCheckUpdates : " + enable);
            if (!enable)
            {
                UpdateButton.IsEnabled = false;
                chekingUpdate.Visibility = Visibility.Visible;
            }
            else
            {
                UpdateButton.IsEnabled = true;
                chekingUpdate.Visibility = Visibility.Collapsed;
            }
        }

        public StackPanel CreateVersion(Version v)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
            stackPanel.Margin = new Thickness(15);

            TextBlock Title = new TextBlock();
            Title.FontSize = 20;
            Title.FontWeight = FontWeight.FromOpenTypeWeight(600);
            Title.Text = v.GetVersionText() + " :";

            StackPanel updateChangelog = new StackPanel();
            updateChangelog.Margin = new Thickness(20, 5, 0, 0);
            updateChangelog.Orientation = System.Windows.Controls.Orientation.Vertical;

            foreach (string item in v.ChangeLog.Split('\n'))
            {
                TextBlock changelog = new TextBlock();
                changelog.FontSize = 15;
                changelog.Text = item;

                updateChangelog.Children.Add(changelog);
            }
            stackPanel.Children.Add(Title);
            stackPanel.Children.Add(updateChangelog);

            return stackPanel;
        }

        public void CreateChangelog()
        {
            if (UpdateChecker.CurrentUpdate == null)
            {
                return;
            }
            changelog.Children.Clear();

            foreach (Version version in UpdateChecker.CurrentUpdate.UpdateData.UpdateChangeLog.Reverse<Version>())
            {
                changelog.Children.Add(CreateVersion(version));
            }
        }

        private async void SignOut(object sender, RoutedEventArgs e)
        {
           await  MessageBoxJ.ShowYesNo("Are you sure you want to sign out?", (s, e) =>
            {
                signout();
                MainWindow.Instance.ShowSnackBar("You have signed out!", "Signed out!", icon: Wpf.Ui.Common.SymbolRegular.ArrowExit20);

            });

            UpdateUIForUser();
        }
        private void signout()
        {
            if (FirebaseAuthControl.Instance?.Client?.User is not null)
                FirebaseAuthControl.Instance?.Client?.SignOut();
            UpdateUIForUser();
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            new MRA_Login().ShowDialog();
            UpdateUIForUser();
        }
    }
}
