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
using UnitedAdobeEditor.Components.Classes;
using static UnitedAdobeEditor.Components.Firebase.PublicConfigManager;

namespace UnitedAdobeEditor.Views.CustomControls
{
    /// <summary>
    /// ExploreFilters.xaml etkileşim mantığı
    /// </summary>
    public partial class ExploreFilters : UserControl
    {
        public ExploreFilters()
        {
            InitializeComponent(); 
            
            apps.Items.Add(new AppSelectionItem().Set("none", "All"));
            foreach (var item in Config.appTypes)
            {
                apps.Items.Add(new AppSelectionItem().Set(item));
            }
            foreach (OrderType apptype in ((OrderType[])Enum.GetValues(typeof(OrderType))))
            {
                sort.Items.Add(new OrderSelector().Set(apptype));
            }
            apps.SelectedIndex = 0;
            sort.SelectedIndex = 0;

            apps.SelectionChanged += SelectionChanged;
            sort.SelectionChanged += SelectionChanged;

            this.Loaded += ExploreFilters_Loaded;
        }

        private void ExploreFilters_Loaded(object sender, RoutedEventArgs e)
        {
            apps.InvalidateVisual();
            sort.InvalidateVisual();

        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSelectionChanged?.Invoke(sender, e);
        }

        public EventHandler? OnSelectionChanged; 
        
        public string currentFilter
        {
            get
            {
                return (apps.SelectedItem as AppSelectionItem)?.Key ?? "none";
            }
        }
        public string currentOrder
        {
            get
            {
                return ((sort.SelectedItem as OrderSelector)?.OrderType ?? OrderType.most_used).ToString();
            }
        }
    }
}
