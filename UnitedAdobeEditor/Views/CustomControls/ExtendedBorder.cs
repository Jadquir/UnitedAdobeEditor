using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace UnitedAdobeEditor.Views.CustomControls
{
    public class ExtendedBorder : Border
    {        
        public EventHandler OnClick;

        private bool click = false;
        public ExtendedBorder()
        {
            ChangeColor(false, false);
            CornerRadius = new CornerRadius(10);
        }

        bool isDarkMode => Wpf.Ui.Appearance.Theme.GetAppTheme() == Wpf.Ui.Appearance.ThemeType.Dark;
        public void ChangeColor(bool isOver, bool isMouseDown)
        {
            string key = "#00000000";
            if (isMouseDown)
                key = "#08FFFFFF";
            else if (isOver)
                key = "#15FFFFFF";

            this.Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(key));
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            ChangeColor(true, false);
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Left)
            {
                click = true;
            }
            ChangeColor(true, true);

        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.ChangedButton == MouseButton.Left)
            {
                if (click)
                {
                    OnClick?.Invoke(this, EventArgs.Empty);
                }
            }
            ChangeColor(true, false);
            click = false;
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            ChangeColor(false, false);
            base.OnMouseLeave(e);
            click = false;
        }
    }
}
