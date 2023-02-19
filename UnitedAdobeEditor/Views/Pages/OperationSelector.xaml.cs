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
using UnitedAdobeEditor.Views.CustomControls;
using Wpf.Ui.Controls;

namespace UnitedAdobeEditor.Views.Pages
{
    /// <summary>
    /// OperationSelector.xaml etkileşim mantığı
    /// </summary>
    public partial class OperationSelector : UiPage
    {
        public OperationSelector()
        {
            InitializeComponent();

            list1.Items.Add(new OperationCard(Components.Enums.OperationType.UIColor));
            list1.Items.Add(new OperationCard(Components.Enums.OperationType.SplashScreen));
        }
    }
}
