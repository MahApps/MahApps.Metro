// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        /// <summary>
        /// The submenus, one per category, as they turn up. A page is picked in one of them at a
        /// time, so the one that has just been picked in tells the others to let go.
        /// </summary>
        private readonly List<HamburgerMenuListBox> subMenus = new List<HamburgerMenuListBox>();

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
            var categories = new HamburgerMenuItemCollection();
            var category = default(GalleryCategoryItem);

            foreach (var page in GalleryPages.All)
            {
                if (category is null || category.Label != page.Category)
                {
                    category = new GalleryCategoryItem
                               {
                                   Label = page.Category,
                                   Icon = new PackIconMaterial { Kind = IconFor(page.Category), Width = 18, Height = 18 }
                               };
                    categories.Add(category);
                }

                category.Pages.Add(ItemFor(page));
            }

            this.Menu.ItemsSource = categories;
            this.Menu.OptionsItemsSource = new HamburgerMenuItemCollection { ItemFor(Settings) };

            this.Navigate(GalleryPages.All[0]);
        }

        /// <summary>
        /// The icon a category wears in the pane. There are two of them, and a third would want a
        /// line of its own here.
        /// </summary>
        private static PackIconMaterialKind IconFor(string category)
        {
            return category switch
                   {
                       "Styles" => PackIconMaterialKind.Brush,
                       _ => PackIconMaterialKind.ShapeOutline
                   };
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

        /// <summary>
        /// A closed pane is 48 pixels wide, where a category is an icon and nothing under it is to
        /// be seen, so reaching for one there opens the pane along with it.
        /// </summary>
        private void OnCategoryToggled(object sender, RoutedEventArgs e)
        {
            if (this.Menu.IsPaneOpen)
            {
                return;
            }

            this.Menu.IsPaneOpen = true;

            if (sender is Expander expander)
            {
                expander.IsExpanded = true;
            }
        }

        private void OnSubMenuLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is not HamburgerMenuListBox list || this.subMenus.Contains(list))
            {
                return;
            }

            this.subMenus.Add(list);

            // the pane is built after the first page is shown, so the row that page belongs to is
            // picked here rather than back then
            if (this.Menu.Content is GalleryPage page)
            {
                var item = ItemIn(list, page);
                if (item is not null)
                {
                    list.SelectedItem = item;
                }
            }
        }

        /// <summary>
        /// A submenu brings a scroll viewer of its own which has nothing to scroll, and one of those
        /// swallows the wheel rather than letting it past, so the pane above is handed it instead.
        /// </summary>
        private void OnSubMenuMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is not UIElement list)
            {
                return;
            }

            e.Handled = true;

            list.RaiseEvent(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                            {
                                RoutedEvent = MouseWheelEvent,
                                Source = list
                            });
        }

        private static HamburgerMenuItem? ItemIn(ItemsControl list, GalleryPage page)
        {
            return list.Items
                       .OfType<HamburgerMenuItem>()
                       .FirstOrDefault(candidate => ReferenceEquals(candidate.Tag, page));
        }

        /// <summary>
        /// A submenu is a list of its own, so the menu above knows nothing about what happens in it.
        /// Showing the page, firing the command of the item and clearing the row that was picked
        /// somewhere else is therefore the job of whoever builds the submenu, which is this.
        /// </summary>
        private void OnSubMenuSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not HamburgerMenuListBox list || list.SelectedItem is not HamburgerMenuItem item)
            {
                return;
            }

            this.ClearSelection(list);

            if (item.Tag is GalleryPage page)
            {
                this.Menu.Content = page;
            }

            item.RaiseCommand();
        }

        /// <summary>
        /// Takes the picked row off every list of the pane but the one given, since the pane shows
        /// one page and should say so in one place.
        /// </summary>
        private void ClearSelection(object? keep)
        {
            foreach (var other in this.subMenus)
            {
                if (!ReferenceEquals(other, keep))
                {
                    other.SelectedIndex = -1;
                }
            }

            if (!ReferenceEquals(this.Menu, keep))
            {
                this.Menu.SelectedOptionsIndex = -1;
            }
        }

        private void Navigate(GalleryPage page)
        {
            this.Menu.Content = page;

            foreach (var list in this.subMenus)
            {
                var item = ItemIn(list, page);

                list.SelectedItem = item;

                if (item is not null && list.DataContext is GalleryCategoryItem category)
                {
                    category.IsExpanded = true;
                }
            }
        }

        private void OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs args)
        {
            if (args.InvokedItem is GalleryCategoryItem)
            {
                // a category is not a page: the expander does the work and the row itself stays
                // unpicked, which is what handling the event tells the menu to do
                args.Handled = true;
                return;
            }

            if (args.InvokedItem is HamburgerMenuItem { Tag: GalleryPage page })
            {
                this.ClearSelection(this.Menu);
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
