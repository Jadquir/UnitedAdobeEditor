using MRA_WPF.Views.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UnitedAdobeEditor.Components;
using UnitedAdobeEditor.Components.Classes;
using UnitedAdobeEditor.Components.Enums;
using UnitedAdobeEditor.Components.Firebase;
using UnitedAdobeEditor.Views.CustomControls;
using UnitedAdobeEditor.Views.Pages.Base;
using Wpf.Ui.Controls;
using static UnitedAdobeEditor.Components.Firebase.PublicConfigManager;
using static UnitedAdobeEditor.Views.Pages.ExploreConfigsPage;
using static Wpf.Ui.Controls.Button;

namespace UnitedAdobeEditor.Views.Pages
{
    public class ExploreConfigsPage : PageBase
    {
        public class PublicConfig
        {
            public class Stats
            {
                public int download_config;
                public int download_ss;
                public int run_config;
            }
            public string Id;
            public string AppType;
            public string OwnerName;
            public string url;
            public string Image;
            public DateTime createdAt;
            public Stats stats;
            private System.Drawing.Image? loadedImage;
            private System.Drawing.Image? resizedImage;
            public async Task<System.Drawing.Image> GetFullSizeImage()
            {
                if (loadedImage == null)
                    loadedImage = await Misc.ImageFromUrlAsync(Image, Id);
                return loadedImage;
            }
            public async Task< System.Drawing.Image> GetResizedImage()
            {
                if (resizedImage == null)
                    resizedImage = (await GetFullSizeImage()).ResizeImage(new System.Drawing.Size(240, 160));
                return resizedImage;
            }
        }
        public class Pagination
        {
            public int pagesize;
            public int nextPage;
            public int currentPage;
            public int max_page;
            public PublicConfig[] configs;
        }
        private Pagination pagination;

        public Wpf.Ui.Controls.Button[] buttons;
        private StackPanel buttonHolder;
        private ExploreFilters filters;
        // private TextBlock EmptyTextBlock;
        private Grid grid;

        protected bool EnableLoading = true;
        public ExploreConfigsPage()
        {
            //EmptyTextBlock = new TextBlock()
            //{
            //    Text = "Sorry :(. It seems there is no match for this search",
            //    Margin = new Thickness(0, 0, 10, 0),
            //    VerticalAlignment = VerticalAlignment.Center,
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    Foreground = new SolidColorBrush(Colors.AliceBlue)
            //};
            SetEmptyText("Sorry :( It seems there is no match for this search");
            pagination = new Pagination();
            pagination.nextPage = 1;

            buttonHolder = new StackPanel();
            buttonHolder.Orientation = Orientation.Horizontal;

            buttonHolder.MinHeight = 50;
            buttonHolder.VerticalAlignment = VerticalAlignment.Center;
            buttonHolder.HorizontalAlignment = HorizontalAlignment.Center;
            filters = new ExploreFilters();
            filters.OnSelectionChanged += (s, e) => 
            {
                pagination.nextPage = 1;
                _ = Reload(); 
            };
            GenerateGrid();

            _ = Reload();
        }

        public void GenerateGrid()
        {
            grid = new Grid();
            // Create the second row with "Auto" height
            RowDefinition rowDefinition3 = new RowDefinition();
            rowDefinition3.Height = new GridLength(1, GridUnitType.Auto);
            grid.RowDefinitions.Add(rowDefinition3);

            // Create the first row with "*" height
            RowDefinition rowDefinition1 = new RowDefinition();
            rowDefinition1.Height = new GridLength(1, GridUnitType.Star);
            grid.RowDefinitions.Add(rowDefinition1);

            // Create the second row with "Auto" height
            RowDefinition rowDefinition2 = new RowDefinition();
            rowDefinition2.Height = new GridLength(1, GridUnitType.Auto);
            grid.RowDefinitions.Add(rowDefinition2);


            Grid.SetRow(filters, 0);
            Grid.SetRow(PlaylistHolder, 1);
            Grid.SetRow(buttonHolder, 2);
            //Grid.SetRow(EmptyTextBlock, 1);
            grid.Children.Add(PlaylistHolder);
            grid.Children.Add(buttonHolder);
          //  grid.Children.Add(EmptyTextBlock);
            grid.Children.Add(filters);
            MainContent.Content = grid;
        }
        private const int buttonsToShow = 5; // Number of buttons to show (excluding previous and next)
        private Wpf.Ui.Controls.Button Create(string content)
        {
            return new Wpf.Ui.Controls.Button
            {
                Content = content,
                Margin = new Thickness(5),
                Width = 35,
                Height = 35,
                Cursor = Cursors.Hand
            };
        }
        public void GenerateButtons()
        {
            buttonHolder.Children.Clear();
            // Add the "Previous" button
            var previousButton = Create("<");
            previousButton.Click += (sender, e) => GoToPreviousPage();
            buttonHolder.Children.Add(previousButton);

            // Generate buttons from (currentPage - buttonsToShow) to (currentPage + buttonsToShow)
            for (int i = pagination.currentPage - buttonsToShow; i <= pagination.currentPage + buttonsToShow; i++)
            {
                if (i > 0 && i <= pagination.max_page)
                {
                    int page = i;  // Create a local copy of 'i'

                    bool isCurrentActive = page == Math.Max((pagination.currentPage), 1);
                    Wpf.Ui.Controls.Button button = Create(page.ToString());
                    button.Appearance =
                       isCurrentActive ? Wpf.Ui.Common.ControlAppearance.Primary : Wpf.Ui.Common.ControlAppearance.Secondary;
                    if (!isCurrentActive)
                        button.Click += (sender, e) => GoToPage(page);
                    else
                    {
                        Debug.WriteLine("Active page : " + page);
                    }
                    buttonHolder.Children.Add(button);
                }
            }

            // Add the "Next" button
            Wpf.Ui.Controls.Button nextButton = Create(">");
            nextButton.Click += (sender, e) => GoToNextPage();
            buttonHolder.Children.Add(nextButton);
        }
        private void GoToPage(int pageNumber)
        {
            pagination.nextPage = pageNumber;
            _ = Reload();
        }

        private void GoToPreviousPage()
        {
            var page = pagination.currentPage-1;
            if (page > 1)
            {
                pagination.nextPage = page;
                _ = Reload();
            }
        }

        private void GoToNextPage()
        {
            if (pagination.currentPage + 1 < pagination.max_page)
            {
                var page = pagination.currentPage+1;
                pagination.nextPage = pagination.currentPage;
                _ = Reload();
            }
        }
      
        private bool isEmpty;
        protected override async IAsyncEnumerable<object> GetItems()
        {
            Pagination? pagination1 = null;
            try
            {
                pagination1 = await PublicConfigManager.Paginate(this.pagination.nextPage, filters.currentFilter, filters.currentOrder);

            }
            catch (Exception)
            {

            }
            if (pagination1 is not null && pagination1.configs is not null)
            {
                Dispatcher.Invoke(() =>
                {
                    isEmpty = pagination1.configs.Length <= 0;
                    this.pagination = pagination1;
                    RunOnUI();
                });

                var ordered = (filters.currentOrder == "most_used" ? 
                    this.pagination.configs/*.OrderByDescending(x => x.stats.run_config)*/ :
                    this.pagination.configs/*.OrderBy(x => x.createdAt)*/).ToArray();
                var arr = new PublicConfigItem[ordered.Length];
                for (int i = 0; i < ordered.Length; i++)
                {
                    var item = ordered[i]; 
                    var citem = new PublicConfigItem().Set(item);
                    yield return citem;
                    arr[i] = citem;
                }
                await WaitForAllIsLoadedAsync(arr);
            }
            else
            {
                isEmpty = true;
                //MainWindow.Instance.NavigateBack();
            }
            RunOnUI();
        }
        async Task WaitForAllIsLoadedAsync(PublicConfigItem[] items)
        {
            // Create a list to keep track of incomplete items
            var incompleteItems = items.ToList();

            var tasks = incompleteItems.Select(x => Task.Run(() => x.WaitForIsLoadedAsync()));
            await Task.WhenAll(tasks);
        }
        public override Visibility IsEmptySignVisible => isEmpty.VisibleIfTrue();
        private void RunOnUI()
        {
            Dispatcher.Invoke(() =>
            {
                NotifyListChanged();
                GenerateButtons();
            });
        }
    }
}
