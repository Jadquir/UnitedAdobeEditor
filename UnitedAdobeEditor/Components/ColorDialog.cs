using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace UnitedAdobeEditor.Components
{
    public class ColorDialogJ
    {
        public static (bool, Color) Show(Color OldColor)
        {
            var dialog = new JadColorPicker.ColorDialog(OldColor);
            dialog.Owner = MainWindow.Instance;
            dialog.ShowDialog();

            return (dialog.IsSelected, dialog.SelectedColor);
        }
    }
}
