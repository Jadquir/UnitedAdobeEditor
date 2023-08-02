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

namespace UnitedAdobeEditor.Views.CustomControls
{
    /// <summary>
    /// VersionCard.xaml etkileşim mantığı
    /// </summary>
    public partial class VersionCard : ExtendedUserControl
    {
        private readonly SelectedPath selectedPath;
        public VersionCard(SelectedPath path)
        {
            InitializeComponent();

            this.title.Text = path.Title;
            this.path.Text = path.EXEFilePath;
            this.selectedPath = path;

            OnClick += (o, e) =>
            {
                Click();
            };
        }
        public void Click()
        {
            CurrentOperation.SelectedPath = selectedPath;
            MainWindow.Instance.Navigate(Components.Enums.Page.OperationSelector);
        }
    }
}
