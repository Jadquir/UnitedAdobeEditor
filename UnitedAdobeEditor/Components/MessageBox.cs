using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace UnitedAdobeEditor.Components
{
    internal class MessageBoxJ
    {
        public static void ShowOK(object content, string? title = null, bool LeftPrimary = false, Window? owner = null)
        {
            var ok = "Ok";
            Show(content, ok, null, ok, null, LeftPrimary, title, owner);
        }
        public static async Task ShowOKDailog(object content, string? title = null, bool LeftPrimary = false, 
            double heightMultiplier = 1,
            double widthMultiplier = 1)
        {
            var ok = "Ok";
            await ShowDialogFunc(content, "", ()=>true, ok, () => true, title, LeftPrimary, heightMultiplier, widthMultiplier);
        }
        public static void Show(
            object message,

            string LeftButtonText = "OK",
            RoutedEventHandler? LeftButtonClick = null,

            string RightButtonText = "Cancel",
            RoutedEventHandler? RightButtonClick = null,

            bool LeftPrimary = true,
            string? title = null,
            Window? owner = null)
        {
            Wpf.Ui.Controls.MessageBox mb = new Wpf.Ui.Controls.MessageBox();

            mb.ButtonLeftName = LeftButtonText;
            mb.ButtonRightName = RightButtonText;
            mb.Content = null;
            mb.Content = message is string messagestr ? new TextBlock() { Text = messagestr } : message ;

            title ??= App.Name;
            mb.Owner = owner ?? MainWindow.Instance;
            mb.Title = title;

            LeftButtonClick += (s, e) => { mb.Close(); };
            RightButtonClick += (s, e) => { mb.Close(); };

            mb.ButtonLeftClick += LeftButtonClick;
            mb.ButtonRightClick += RightButtonClick;

            mb.ButtonRightAppearance = LeftPrimary ? Wpf.Ui.Common.ControlAppearance.Secondary : Wpf.Ui.Common.ControlAppearance.Transparent;
            mb.ButtonLeftAppearance = LeftPrimary ? Wpf.Ui.Common.ControlAppearance.Primary : Wpf.Ui.Common.ControlAppearance.Secondary;

            mb.ShowDialog();
        }


        private static RoutedEventHandler? Old1,Old2;

        public static async Task ShowYesNo(string message,
            RoutedEventHandler? YesClick = null,
            RoutedEventHandler? NoClick = null,
            bool YesPrimary = true,
            string? title = null)
        {
            await ShowDialog(message, "No", NoClick, "Yes", YesClick, YesPrimary, title);
        }

        public static void ShowOK(string message)
        {
            _ = ShowDialog(message);
        }
        public static async Task ShowOKAsync(string message)
        {
            await ShowDialog(message);
        }
        public static async Task ShowDialog(string message,

            string RightButtonText = "OK",
            RoutedEventHandler? RightButtonClick = null,

            string LeftButtonText = "",
            RoutedEventHandler? LeftButtonClick = null,

            bool LeftPrimary = false,
            string? title = null)
        {
            await MainWindow.Instance.Dispatcher.Invoke(async () => {
                Dialog dialog = MainWindow.Instance.GetDialog();

                dialog.ButtonLeftName = LeftButtonText;
                if (String.IsNullOrEmpty(dialog.ButtonLeftName))
                {
                    dialog.ButtonLeftVisibility = Visibility.Collapsed;
                }
                dialog.ButtonRightName = RightButtonText;
                if (String.IsNullOrEmpty(dialog.ButtonRightName))
                {
                    dialog.ButtonRightVisibility = Visibility.Collapsed;
                }
                dialog.Content = null; 
                dialog.Message = "";
                dialog.Message = message;


                title ??= App.Name;
                dialog.Title = title;

                LeftButtonClick += (s, e) => { Debug.WriteLine("Left Click"); dialog.Hide(); };
                RightButtonClick += (s, e) => { Debug.WriteLine("Right Click"); dialog.Hide(); };

                if (Old1 != null) { dialog.ButtonLeftClick -= Old1; }
                dialog.ButtonLeftClick += LeftButtonClick;

                if (Old2 != null) { dialog.ButtonRightClick -= Old2; }
                dialog.ButtonRightClick += RightButtonClick;

                Old1 = LeftButtonClick;
                Old2 = RightButtonClick;

                dialog.ButtonRightAppearance = LeftPrimary ? Wpf.Ui.Common.ControlAppearance.Secondary : Wpf.Ui.Common.ControlAppearance.Primary;
                dialog.ButtonLeftAppearance = LeftPrimary ? Wpf.Ui.Common.ControlAppearance.Primary : Wpf.Ui.Common.ControlAppearance.Secondary;

                await dialog.ShowAndWaitAsync();
            });
            
        }


        public static async Task ShowDialogTask(
            object content,
            string LeftButtonText, Task<bool>? LeftButtonClick,
            string RightButtonText, Task<bool>? RightButtonClick,
            string? title = null,
            bool LeftPrimary = false,
            double heightMultiplier = 1,
            double widthMultiplier = 1
            )
        {
            if (!MainWindow.Instance.Dispatcher.CheckAccess())
            {
                await MainWindow.Instance.Dispatcher.Invoke(async () =>
                {
                    await ShowDialogTask(content, LeftButtonText, LeftButtonClick, RightButtonText, RightButtonClick, title, LeftPrimary);
                });
                return;
            }
            //MusicRecognitionAPI.Log.Add($"SettingsPage.ShowConfirmation");
            Wpf.Ui.Controls.Dialog mb = MainWindow.Instance.GetDialog();

            mb.ButtonLeftName = LeftButtonText;
            mb.ButtonRightName = RightButtonText;
            mb.Content = null;
            mb.Message = "";
            mb.Content = content is string messagestr ? new TextBlock() { Text = messagestr } : content;
            title ??= App.Name;
            mb.Title = title;

            mb.MinHeight = 200.0;
            mb.MinWidth = 400.0;
            mb.DialogWidth = mb.MinWidth + ((MainWindow.Instance.Width / 8.3) * widthMultiplier);
            mb.DialogHeight = mb.MinHeight + ((MainWindow.Instance.Height / 11.3) * heightMultiplier);

            if (LeftPrimary)
            {
                mb.ButtonRightAppearance = Wpf.Ui.Common.ControlAppearance.Secondary;
                mb.ButtonLeftAppearance = Wpf.Ui.Common.ControlAppearance.Primary;
            }
            else
            {
                mb.ButtonRightAppearance = Wpf.Ui.Common.ControlAppearance.Primary;
                mb.ButtonLeftAppearance = Wpf.Ui.Common.ControlAppearance.Secondary;
            }


            bool result = false;
            while (!result)
            {
                mb.Hide();
                var button = await mb.ShowAndWaitAsync();
                if (LeftButtonClick is null && button == Wpf.Ui.Controls.Interfaces.IDialogControl.ButtonPressed.Left)
                {
                    mb.Hide();
                    result = true;
                }
                else if (RightButtonClick is null && button == Wpf.Ui.Controls.Interfaces.IDialogControl.ButtonPressed.Right)
                {
                    mb.Hide();
                    result = true;
                }
                else if (button == Wpf.Ui.Controls.Interfaces.IDialogControl.ButtonPressed.Left && LeftButtonClick != null)
                {
                    result = await LeftButtonClick;
                    if (result)
                        mb.Hide();
                }
                else if (RightButtonClick is not null && button == Wpf.Ui.Controls.Interfaces.IDialogControl.ButtonPressed.Right)
                {
                    result = await RightButtonClick;
                    if (result)
                        mb.Hide();
                }
            }
        }
        public static async Task ShowDialogFunc(
            object content,
            string LeftButtonText, Func<bool>? LeftButtonClick,
            string RightButtonText, Func<bool>? RightButtonClick,
            string? title = null,
            bool LeftPrimary = false,
            double heightMultiplier = 1,
            double widthMultiplier = 1
            )
        {
            if (!MainWindow.Instance.Dispatcher.CheckAccess())
            {
                await MainWindow.Instance.Dispatcher.Invoke(async () =>
                {
                    await ShowDialogFunc(content, LeftButtonText, LeftButtonClick, RightButtonText, RightButtonClick, title, LeftPrimary);
                });
                return;
            }
            //MusicRecognitionAPI.Log.Add($"SettingsPage.ShowConfirmation");
            Wpf.Ui.Controls.Dialog mb = MainWindow.Instance.GetDialog();

            mb.ButtonLeftName = LeftButtonText;
            mb.ButtonRightName = RightButtonText;
            mb.ButtonLeftVisibility = string.IsNullOrEmpty(LeftButtonText) ? Visibility.Collapsed : Visibility.Visible;
            mb.ButtonRightVisibility = string.IsNullOrEmpty(RightButtonText) ? Visibility.Collapsed : Visibility.Visible;

            mb.Content = null;
            mb.Message = "";
            mb.Content = content is string messagestr ? new TextBlock() { Text = messagestr } : content;
            title ??= App.Name;
            mb.Title = title;

            mb.MinHeight = 200.0;
            mb.MinWidth = 400.0;
            mb.DialogWidth = mb.MinWidth + ((MainWindow.Instance.Width / 8.3) * widthMultiplier);
            mb.DialogHeight = mb.MinHeight + ((MainWindow.Instance.Height / 11.3) * heightMultiplier);

            if (LeftPrimary)
            {
                mb.ButtonRightAppearance = Wpf.Ui.Common.ControlAppearance.Secondary;
                mb.ButtonLeftAppearance = Wpf.Ui.Common.ControlAppearance.Primary;
            }
            else
            {
                mb.ButtonRightAppearance = Wpf.Ui.Common.ControlAppearance.Primary;
                mb.ButtonLeftAppearance = Wpf.Ui.Common.ControlAppearance.Secondary;
            }

            bool result = false;
            while (!result)
            {
                mb.Hide();
                var button = await mb.ShowAndWaitAsync();
                if (LeftButtonClick is null && button == Wpf.Ui.Controls.Interfaces.IDialogControl.ButtonPressed.Left)
                {
                    mb.Hide();
                    result = true;
                }
                else if (RightButtonClick is null && button == Wpf.Ui.Controls.Interfaces.IDialogControl.ButtonPressed.Right)
                {
                    mb.Hide();
                    result = true;
                }
                else if (button == Wpf.Ui.Controls.Interfaces.IDialogControl.ButtonPressed.Left && LeftButtonClick != null)
                {
                    result = LeftButtonClick.Invoke();
                    if (result)
                        mb.Hide();
                }
                else if (RightButtonClick is not null && button == Wpf.Ui.Controls.Interfaces.IDialogControl.ButtonPressed.Right)
                {
                    result = RightButtonClick.Invoke();
                    if (result)
                        mb.Hide();
                }
            }
        }
    }
}

