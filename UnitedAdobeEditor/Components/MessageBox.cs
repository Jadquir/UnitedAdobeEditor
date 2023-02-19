using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Controls;

namespace UnitedAdobeEditor.Components
{
    internal class MessageBoxJ 
    {
        public static void Show(
            string message,

            string LeftButtonText = "OK",
            RoutedEventHandler LeftButtonClick = null,

            string RightButtonText = "Cancel",
            RoutedEventHandler RightButtonClick = null,

            bool LeftPrimary = true,
            string title = null)
        {
            Wpf.Ui.Controls.MessageBox mb = new Wpf.Ui.Controls.MessageBox();

            mb.ButtonLeftName = LeftButtonText;
            mb.ButtonRightName = RightButtonText;

            mb.Content = message;
            if (title == null)
                title = App.Name;
            mb.Title = title;

            LeftButtonClick += (s, e) => { mb.Close(); };
            RightButtonClick += (s, e) => { mb.Close(); };

            mb.ButtonLeftClick += LeftButtonClick;
            mb.ButtonRightClick += RightButtonClick;

            mb.ButtonRightAppearance = LeftPrimary ? Wpf.Ui.Common.ControlAppearance.Secondary : Wpf.Ui.Common.ControlAppearance.Transparent;
            mb.ButtonLeftAppearance = LeftPrimary ? Wpf.Ui.Common.ControlAppearance.Primary : Wpf.Ui.Common.ControlAppearance.Secondary;

            mb.ShowDialog();
        }


        private static RoutedEventHandler Old1,Old2;

        public static async Task ShowYesNo(string message,
            RoutedEventHandler YesClick = null,
            RoutedEventHandler NoClick = null,
            bool YesPrimary = true,
            string title = null)
        {
            await ShowDialog(message, "No", NoClick, "Yes", YesClick, YesPrimary, title);
        }

        public static void ShowOK(string message)
        {
            _ = ShowDialog(message);
        }
        public static async Task ShowDialog(string message,

            string RightButtonText = "OK",
            RoutedEventHandler RightButtonClick = null,

            string LeftButtonText = "",
            RoutedEventHandler LeftButtonClick = null,

            bool LeftPrimary = false,
            string title = null)
        {
            bool continueWorking = false;
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
                dialog.Message = message;


                if (title == null)
                {
                    title = App.Name;
                }
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


                continueWorking = true;
            });
            
        }

    }
}
