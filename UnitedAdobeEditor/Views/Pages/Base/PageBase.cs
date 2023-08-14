using MRA_WPF.Views.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace UnitedAdobeEditor.Views.Pages.Base
{
    public abstract class PageBase : LoadablePage
    {
        protected readonly VirtualizingItemsControl PlaylistHolder;

        protected bool AddCreatePlaylistButton = true;
        protected bool EnableLoading = true;
        public PageBase()
        {
            PlaylistHolder = new VirtualizingItemsControl()
            {
                CacheLengthUnit = VirtualizationCacheLengthUnit.Pixel,
            };
            MainContent.Content = PlaylistHolder;
            if (EnableLoading)
                SetLoadingState(true, false);
        }
        protected abstract IAsyncEnumerable<object> GetItems();
        protected async Task Reload()
        {
            if (EnableLoading)
                SetLoadingState(true, true);
            PlaylistHolder.Items.Clear();
            await foreach (var item in GetItems())
            {
                PlaylistHolder.Items.Add(item);
            }
            if (EnableLoading)
                SetLoadingState(false);
        }
    }
}
