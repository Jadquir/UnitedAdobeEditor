using JadColorPicker;
using Color = System.Windows.Media.Color;

namespace UnitedAdobeEditor.Components
{
    public class ColorDialogJ
    {
        public static (bool, Color) Show(Color OldColor)
        {
            var dialog = new JadColorPicker.ColorDialog(OldColor);
            dialog.Owner = MainWindow.Instance;
            dialog.ContentRendered += (s, e) =>
            {
                ColorDialog.ApplyBackground(dialog);
            };
           
            dialog.ShowDialog();

            return (dialog.IsSelected, dialog.SelectedColor);
        }
    }
}
