
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using UnitedAdobeEditor;
using UnitedAdobeEditor.Components;
using Wpf.Ui.Controls;

namespace MRA_WPF.Views.Pages
{
    /// <summary>
    /// LoadablePage.xaml etkileşim mantığı
    /// </summary>
    public partial class LoadablePage : UiPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void NotifyListChanged()
        {
            NotifyPropertyChanged(nameof(IsEmptySignVisible));
        }
        public virtual Visibility IsEmptySignVisible
        {
            get
            {
                return Visibility.Collapsed;
            }
        }

        public LoadablePage()
        {
            InitializeComponent();
            SetLoadingState(false, false);
            SetLoadingTexts();
        }
        public void SetLoadingTexts(string? upperText = "", string? lowerText = "")
        {
            loadingText.Text = upperText ?? "";
            loadingText1.Text = lowerText ?? "";

            loadingText.Visibility = string.IsNullOrWhiteSpace(upperText).VisibleIfFalse();
            loadingText1.Visibility = string.IsNullOrWhiteSpace(lowerText).VisibleIfFalse();
        }
        public void SetLoadingState(bool isLoading, bool fade = true)
        {
            if (!MainWindow.Instance.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    SetLoadingState(isLoading, fade);
                }));
                return;
            }

            if (isLoading)
            {
                if (fade)
                {
                    if (LoadingPanel.Visibility == Visibility.Collapsed)
                        LoadingPanel.FadeIn(150);
                }
                else
                {
                    LoadingPanel.Visibility = Visibility.Visible;
                }
                grid.Effect = new System.Windows.Media.Effects.BlurEffect()
                {
                    Radius = 25,
                    KernelType = System.Windows.Media.Effects.KernelType.Gaussian,
                };
            }
            else
            {
                if (fade)
                {
                    if (LoadingPanel.Visibility == Visibility.Visible)
                        LoadingPanel.FadeOut(150);
                }
                else
                {
                    LoadingPanel.Visibility = Visibility.Collapsed;
                }
                grid.Effect = null;
            }
        }

        public void SetEmptyText(string? upperText = "", string? lowerText = "")
        {
            text1.Text = upperText ?? "";
            text2.Text = lowerText ?? "";

            text1.Visibility = string.IsNullOrWhiteSpace(upperText).VisibleIfFalse();
            text2.Visibility = string.IsNullOrWhiteSpace(lowerText).VisibleIfFalse();

        }
        
    }
}
