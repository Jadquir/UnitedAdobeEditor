using System;
using System.Collections.Generic;
using System.Drawing;
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
using UnitedAdobeEditor.Components.Enums;
using static System.Net.Mime.MediaTypeNames;
using Page = UnitedAdobeEditor.Components.Enums.Page;

namespace UnitedAdobeEditor.Views.CustomControls
{
    /// <summary>
    /// OperationCard.xaml etkileşim mantığı
    /// </summary>
    public partial class OperationCard : ExtendedUserControl
    {
        private OperationType OperationType;
        public OperationCard(OperationType type)
        {
            InitializeComponent();
            string text = "colorchanger";
            text1.Text = "Change UI Color";
            if (type == OperationType.SplashScreen)
            {
                text1.Text = "Change Splash Screen";
                text = "splashscreen";
            }
            icon1.Data = (Geometry)this.TryFindResource(text);
            OperationType = type;
            OnClick += (s, e) =>
            {
                Page page = type == OperationType.UIColor ? Page.ColorChanger : Page.SplashScreenChanger;
                MainWindow.Instance.Navigate(page);
            };
        }
    }
}
