    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnitedAdobeEditor.Views.CustomControls
{
    public class ExtendedUserControl : UserControl
    {
        public EventHandler OnClick;

        private bool click = false;
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if(e.ChangedButton == MouseButton.Left)
            {
                click = true;
            }
            
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if(e.ChangedButton == MouseButton.Left)
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
