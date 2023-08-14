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
using static UnitedAdobeEditor.Components.Firebase.PublicConfigManager;

namespace UnitedAdobeEditor.Views.CustomControls
{
    /// <summary>
    /// OrderSelector.xaml etkileşim mantığı
    /// </summary>
    public partial class OrderSelector : UserControl
    {
        public OrderSelector()
        {
            InitializeComponent();
        }
        public OrderType OrderType;
        public OrderSelector Set(OrderType orderType)
        {
            this.OrderType = orderType;
            string text = "";
            switch (orderType)
            {
                case OrderType.createdAt:
                    text = "Latest Configs";
                    break;
                case OrderType.most_used:
                    text = "Most Used";
                    break;
                default:
                    text = "Most Used";
                    break;
            }
            this.text.Text = text;
            return this;
        }
    }
}
