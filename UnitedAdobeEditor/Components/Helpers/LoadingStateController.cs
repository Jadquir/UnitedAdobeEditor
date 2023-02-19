using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnitedAdobeEditor.Views.Pages;

namespace UnitedAdobeEditor.Components.Helpers
{
    public static class LoadingStateController
    {
        public enum State
        {
            Backup,
            ExtractingFiles,
            PackingFiles,
            CopyingFiles,
            ChangingColors,
            PleaseWait,
            ChangingImages
        }
        public static string GetText(this State state)
        {
            string text = state.ToString();
            switch (state)
            {
                case State.Backup:
                    text = "Backing up files";
                    break;
                case State.ExtractingFiles:
                    text = "Extracting Files";
                    break;
                case State.PackingFiles:
                    text = "Packing Files";
                    break;
                case State.CopyingFiles:
                    text = "Copying Files";
                    break;
                case State.ChangingColors:
                    text = "Changing Colors";
                    break;
                case State.PleaseWait:
                    text = "Please Wait";
                    break;
                case State.ChangingImages:
                    text = "Changing Images";
                    break;
                default:
                    break;
            }
            return text;
        }
        private static State CurrentState = State.PleaseWait;
        public static void SetState(State state,string sender)
        {
            MainWindow.Instance.Dispatcher.Invoke(() =>
            {
                Debug.WriteLine("Changing state to " + state.GetText() + " sender : " +sender);
                Loading.Instance.ChangeText(state.GetText());
                if (state == State.PleaseWait && CurrentState != State.PleaseWait)
                {
                    MainWindow.Instance.GoBack();
                }
                CurrentState = state;
            });
        }
    }
}
