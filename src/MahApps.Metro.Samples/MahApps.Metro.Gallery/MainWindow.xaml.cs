// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Gallery.Core;
using MahApps.Metro.Gallery.Pages;
using MahApps.Metro.IconPacks;

namespace MahApps.Metro.Gallery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private static readonly GalleryPage Settings = new GalleryPage("Settings",
                                                                      "Gallery",
                                                                      typeof(SettingsPage),
                                                                      PackIconMaterialKind.CogOutline);

        private readonly HamburgerMenuItemCollection items = new HamburgerMenuItemCollection();

        public MainWindow()
        {
            this.InitializeComponent();

            this.BuildNavigation();
        }

        /// <summary>
        /// The navigation, the options at the bottom and the search all come out of
        /// <see cref="GalleryPages"/>, so a new page is one entry there and nothing here.
        /// </summary>
        private void BuildNavigation()
        {
            var category = default(string);

            foreach (var page in GalleryPages.All)
            {
                if (page.Category != category)
                {
                    if (category is not null)
                    {
                        this.items.Add(new HamburgerMenuSeparatorItem());
                    }

                    this.items.Add(new HamburgerMenuHeaderItem { Label = page.Category });
                    category = page.Category;
                }

                this.items.Add(ItemFor(page));
            }

            this.Menu.ItemsSource = this.items;
            this.Menu.OptionsItemsSource = new HamburgerMenuItemCollection { ItemFor(Settings) };

            this.Navigate(GalleryPages.All[0]);
        }

        private static HamburgerMenuIconItem ItemFor(GalleryPage page)
        {
            return new HamburgerMenuIconItem
                   {
                       Label = page.Title,
                       Icon = new PackIconMaterial { Kind = page.Icon, Width = 18, Height = 18 },
                       Tag = page
                   };
        }

        private void Navigate(GalleryPage page)
        {
            this.Menu.Content = page;

            var item = this.items
                           .OfType<HamburgerMenuItem>()
                           .FirstOrDefault(candidate => ReferenceEquals(candidate.Tag, page));

            if (item is not null)
            {
                this.Menu.SelectedItem = item;
            }
        }

        private void OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs args)
        {
            if (args.InvokedItem is HamburgerMenuItem { Tag: GalleryPage page })
            {
                this.Menu.Content = page;
            }
        }

        private void OnSearchTextChanged(object sender, RoutedEventArgs e)
        {
            if (sender is not AutoSuggestBox box
                || ((AutoSuggestBoxTextChangedEventArgs)e).Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            {
                return;
            }

            box.ItemsSource = GalleryPages.All.Where(page => page.Matches(box.Text)).Take(8).ToList();
        }

        private void OnSearchQuerySubmitted(object sender, RoutedEventArgs e)
        {
            var args = (AutoSuggestBoxQuerySubmittedEventArgs)e;

            // either the reader picked one from the list, or typed something and hit enter, in which
            // case the first page that matches is the one they meant
            var page = args.ChosenSuggestion as GalleryPage
                       ?? GalleryPages.All.FirstOrDefault(candidate => candidate.Matches(args.QueryText));

            if (page is not null)
            {
                this.Navigate(page);
            }
        }
    }
}
