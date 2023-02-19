using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace UnitedAdobeEditor.Views.CustomControls
{
    public class ExtendedBorder : Border
    {        
        public EventHandler OnClick;

        private bool click = false;
        public ExtendedBorder()
        {
            
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Left)
            {
                click = true;
            }

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
            click = false;
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            click = false;
        }
    }
}
