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


        public bool EnableChangeBgColor
        {
            get { return (bool)GetValue(EnableChangeBgColorProperty); }
            set { SetValue(EnableChangeBgColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableChangeBgColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableChangeBgColorProperty =
            DependencyProperty.Register("EnableChangeBgColor", typeof(bool), typeof(ExtendedBorder), new PropertyMetadata(true));


        bool isDarkMode => Wpf.Ui.Appearance.Theme.GetAppTheme() == Wpf.Ui.Appearance.ThemeType.Dark;
        public void ChangeColor(bool isOver, bool isMouseDown)
        {
            if (!EnableChangeBgColor) return;
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
