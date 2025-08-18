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
        public static async void Show(
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

            mb.PrimaryButtonAppearance = LeftPrimary ? ControlAppearance.Primary : ControlAppearance.Secondary;
            mb.SecondaryButtonAppearance = !LeftPrimary ? ControlAppearance.Primary : ControlAppearance.Secondary;
        
            mb.PrimaryButtonText = LeftButtonText;
            mb.SecondaryButtonText = RightButtonText;

            mb.IsSecondaryButtonEnabled = !string.IsNullOrEmpty(RightButtonText);
            mb.IsPrimaryButtonEnabled = !string.IsNullOrEmpty(LeftButtonText);

            mb.Content = null;
            mb.Content = message is string messagestr ? new Wpf.Ui.Controls.TextBlock() { Text = messagestr } : message ;

            title ??= App.Name;
            mb.Owner = owner ?? MainWindow.Instance;
            mb.Title = title;

            //LeftButtonClick += (s, e) => { mb.Close(); };
            //RightButtonClick += (s, e) => { mb.Close(); };
            mb.IsCloseButtonEnabled = !string.IsNullOrEmpty(RightButtonText);

            var result = await mb.ShowDialogAsync();
            if(result == Wpf.Ui.Controls.MessageBoxResult.Primary && LeftPrimary)
            {
                LeftButtonClick?.Invoke(null, null);
            }
            else if(result == Wpf.Ui.Controls.MessageBoxResult.Secondary && LeftPrimary)
            {
                RightButtonClick?.Invoke(null, null);
            }
            else if (result == Wpf.Ui.Controls.MessageBoxResult.Primary && !LeftPrimary)
            {
                RightButtonClick?.Invoke(null, null);
            }
            else if (result == Wpf.Ui.Controls.MessageBoxResult.Secondary && !LeftPrimary)
            {
                LeftButtonClick?.Invoke(null, null);
            }
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
                await ShowDialogFunc(message, LeftButtonText, () =>
                {
                    LeftButtonClick?.Invoke(null, null);
                    return true;
                }, RightButtonText, () =>
                {
                    RightButtonClick?.Invoke(null, null);
                    return true;
                }, title, LeftPrimary);
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
            var service = MainWindow.Instance.GetDialog();
            var contentDialog = new ContentDialog();
            if (content is string str)
            {
                contentDialog.Content = new Wpf.Ui.Controls.TextBlock()
                {
                    Text = str,
                    MinWidth = 350,
                    TextWrapping = TextWrapping.WrapWithOverflow,
                };
            }
            else
            {
                contentDialog.Content = content;
            }
            title ??= App.Name;
            contentDialog.Title = title;
            contentDialog.PrimaryButtonAppearance = LeftPrimary ? ControlAppearance.Primary : ControlAppearance.Secondary;
            contentDialog.SecondaryButtonAppearance = !LeftPrimary ? ControlAppearance.Primary : ControlAppearance.Secondary;

            contentDialog.PrimaryButtonText = LeftButtonText;
            contentDialog.SecondaryButtonText = RightButtonText;


            contentDialog.IsSecondaryButtonEnabled = !string.IsNullOrEmpty(RightButtonText);
            contentDialog.IsPrimaryButtonEnabled = !string.IsNullOrEmpty(LeftButtonText);

            contentDialog.CloseButtonText = "Close";

            contentDialog.MinHeight = 200.0;
            contentDialog.MinWidth = 400.0;
            if (contentDialog.Content is FrameworkElement element)
            {
                contentDialog.DialogHeight = element.DesiredSize.Height;
                contentDialog.DialogWidth = element.DesiredSize.Width * 1.2d;
            }
            bool result = false;
            while (!(result))
            {
                var button = await service.ShowAsync(contentDialog, default);

                bool? isLeft = null;
                isLeft = button == ContentDialogResult.Primary ?
                    LeftPrimary :
                    (button == ContentDialogResult.Secondary ?
                   !LeftPrimary :
                    null);
                if (isLeft == true && LeftButtonClick != null)
                {
                    result = await LeftButtonClick;
                }
                else if (isLeft == false && RightButtonClick != null)
                    result = await RightButtonClick;

                else
                {
                    result = true;
                }
                //if (button == ContentDialogResult.Primary)
                //{
                //    result = await DialogParameters.Invoke(parameters.Left);
                //}
                //else if (button == ContentDialogResult.Secondary)
                //{
                //    result = await DialogParameters.Invoke(parameters.Right);
                //}
                //else if (button == ContentDialogResult.None)
                //{
                //    result = true;
                //}
            }
            //while (!result)
            //{
            //    mb.Hide();
            //    var button = await mb.ShowAndWaitAsync();
            //    if (LeftButtonClick is null && button == Wpf.Ui.Controls.Interfaces.IDialogControl.ButtonPressed.Left)
            //    {
            //        mb.Hide();
            //        result = true;
            //    }
            //    else if (RightButtonClick is null && button == Wpf.Ui.Controls.Interfaces.IDialogControl.ButtonPressed.Right)
            //    {
            //        mb.Hide();
            //        result = true;
            //    }
            //    else if (button == Wpf.Ui.Controls.Interfaces.IDialogControl.ButtonPressed.Left && LeftButtonClick != null)
            //    {
            //        result = await LeftButtonClick;
            //        if (result)
            //            mb.Hide();
            //    }
            //    else if (RightButtonClick is not null && button == Wpf.Ui.Controls.Interfaces.IDialogControl.ButtonPressed.Right)
            //    {
            //        result = await RightButtonClick;
            //        if (result)
            //            mb.Hide();
            //    }
            //}
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
            var service = MainWindow.Instance.GetDialog();
            var contentDialog = new ContentDialog();
            if (content is string str)
            {
                contentDialog.Content = new Wpf.Ui.Controls.TextBlock()
                {
                    Text = str,
                    MinWidth = 350,
                    TextWrapping = TextWrapping.WrapWithOverflow,
                };
            }
            else
            {
                contentDialog.Content = content;
            }
            title ??= App.Name;
            contentDialog.Title = title;
            contentDialog.PrimaryButtonAppearance = LeftPrimary ? ControlAppearance.Primary : ControlAppearance.Secondary;
            contentDialog.SecondaryButtonAppearance = !LeftPrimary ? ControlAppearance.Primary : ControlAppearance.Secondary;

            contentDialog.PrimaryButtonText = LeftButtonText;
            contentDialog.SecondaryButtonText = RightButtonText;


            contentDialog.IsSecondaryButtonEnabled = !string.IsNullOrEmpty(RightButtonText);
            contentDialog.IsPrimaryButtonEnabled = !string.IsNullOrEmpty(LeftButtonText);

            contentDialog.CloseButtonText = "Close";

            contentDialog.MinHeight = 200.0;
            contentDialog.MinWidth = 400.0;
            if (contentDialog.Content is FrameworkElement element)
            {
                contentDialog.DialogHeight = element.DesiredSize.Height;
                contentDialog.DialogWidth = element.DesiredSize.Width * 1.2d;
            }
            bool result = false;
            while (!(result))
            {
                var button = await service.ShowAsync(contentDialog, default);

                bool? isLeft = null;
                isLeft = button == ContentDialogResult.Primary ?
                    LeftPrimary :
                    (button == ContentDialogResult.Secondary ?
                   !LeftPrimary :
                    null);
                if (isLeft == true && LeftButtonClick != null)
                {
                    result =  LeftButtonClick.Invoke();
                }
                else if (isLeft == false && RightButtonClick != null)
                    result = RightButtonClick.Invoke();

                else
                {
                    result = true;
                }
                //if (button == ContentDialogResult.Primary)
                //{
                //    result = await DialogParameters.Invoke(parameters.Left);
                //}
                //else if (button == ContentDialogResult.Secondary)
                //{
                //    result = await DialogParameters.Invoke(parameters.Right);
                //}
                //else if (button == ContentDialogResult.None)
                //{
                //    result = true;
                //}
            }
            //while (!result)
            //{
            //    mb.Hide();
            //    var button = await mb.ShowAndWaitAsync();
            //    if (LeftButtonClick is null && button == Wpf.Ui.Controls.Interfaces.IDialogControl.ButtonPressed.Left)
            //    {
            //        mb.Hide();
            //        result = true;
            //    }
            //    else if (RightButtonClick is null && button == Wpf.Ui.Controls.Interfaces.IDialogControl.ButtonPressed.Right)
            //    {
            //        mb.Hide();
            //        result = true;
            //    }
            //    else if (button == Wpf.Ui.Controls.Interfaces.IDialogControl.ButtonPressed.Left && LeftButtonClick != null)
            //    {
            //        result = await LeftButtonClick;
            //        if (result)
            //            mb.Hide();
            //    }
            //    else if (RightButtonClick is not null && button == Wpf.Ui.Controls.Interfaces.IDialogControl.ButtonPressed.Right)
            //    {
            //        result = await RightButtonClick;
            //        if (result)
            //            mb.Hide();
            //    }
            //}
        }
    }
}

