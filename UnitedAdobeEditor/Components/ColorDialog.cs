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
                Wpf.Ui.Appearance.Background.Apply(dialog, Wpf.Ui.Appearance.Background.IsSupported(Wpf.Ui.Appearance.BackgroundType.Mica)
               ? Wpf.Ui.Appearance.BackgroundType.Mica :
               Wpf.Ui.Appearance.BackgroundType.None);
            };
           
            dialog.ShowDialog();

            return (dialog.IsSelected, dialog.SelectedColor);
        }
    }
}
