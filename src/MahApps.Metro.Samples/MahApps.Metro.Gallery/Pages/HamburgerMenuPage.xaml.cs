// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using MahApps.Metro.Gallery.Navigation;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for HamburgerMenuPage.xaml
    /// </summary>
    public partial class HamburgerMenuPage : UserControl
    {
        public HamburgerMenuPage()
        {
            this.InitializeComponent();

            this.MenuExample.Watch(this.Menu,
                                   HamburgerMenu.DisplayModeProperty,
                                   HamburgerMenu.PanePlacementProperty,
                                   HamburgerMenu.IsPaneOpenProperty,
                                   HamburgerMenu.OpenPaneLengthProperty,
                                   HamburgerMenu.CompactPaneLengthProperty,
                                   HamburgerMenu.HamburgerVisibilityProperty,
                                   HamburgerMenu.ShowSelectionIndicatorProperty,
                                   HamburgerMenu.CanResizeOpenPaneProperty,
                                   HamburgerMenu.VerticalScrollBarVisibilityProperty,
                                   HamburgerMenu.OptionsVisibilityProperty);
            this.MenuExample.Watch("Layout", this.Menu, WidthProperty, HeightProperty);

            this.SpeakIn("en");

            this.TransitionExample.Watch(this.TransitionFrame, TransitioningContentControl.TransitionProperty);

            // Where the two frames start. A Page of its own rather than a Uri, so that the pages
            // find each other next door whichever assembly they end up in.
            this.JournalFrame.Navigate(new ReleasePage());
            this.TransitionFrame.Navigate(new ReleasePage());
        }

        /// <summary>
        /// The items and the options are two lists, and picking in one of them lets go of the other,
        /// so what is on the right comes from whichever row was invoked rather than from a binding
        /// to one of the two.
        /// </summary>
        private void OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            this.Menu.Content = (e.InvokedItem as HamburgerMenuItemBase)?.Tag;
        }

        /// <summary>
        /// A row of the pane stands for a page, and that page is what the frame is told to show.
        /// Nothing happens when it is the one already showing, since the pane and the frame keep
        /// each other in step and would otherwise push one another round in circles.
        /// </summary>
        private static void Show(Frame frame, HamburgerMenuItemInvokedEventArgs e)
        {
            if ((e.InvokedItem as HamburgerMenuItemBase)?.Tag is Type page
                && frame.Content?.GetType() != page
                && Activator.CreateInstance(page) is { } content)
            {
                frame.Navigate(content);
            }
        }

        /// <summary>
        /// The journal decides what shows, so the pane follows it rather than the other way round:
        /// a step back lights the row that page belongs to, and the two lists let go of each other
        /// on their own, since picking in one of them clears the other.
        /// </summary>
        private static void Follow(HamburgerMenu menu, NavigationEventArgs e)
        {
            var page = e.Content?.GetType();

            menu.SetCurrentValue(HamburgerMenu.SelectedItemProperty, RowFor(menu.ItemsSource, page));
            menu.SetCurrentValue(HamburgerMenu.SelectedOptionsItemProperty, RowFor(menu.OptionsItemsSource, page));

            static object? RowFor(object? rows, Type? page)
            {
                return (rows as IEnumerable)?.OfType<HamburgerMenuItemBase>().FirstOrDefault(row => row.Tag as Type == page);
            }
        }

        private void OnJournalItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            Show(this.JournalFrame, e);
        }

        private void OnJournalNavigated(object sender, NavigationEventArgs e)
        {
            Follow(this.JournalMenu, e);
        }

        private void OnJournalBack(object sender, RoutedEventArgs e)
        {
            this.JournalFrame.GoBack();
        }

        private void OnJournalForward(object sender, RoutedEventArgs e)
        {
            this.JournalFrame.GoForward();
        }

        private void OnTransitionItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            Show(this.TransitionFrame, e);
        }

        private void OnTransitionNavigated(object sender, NavigationEventArgs e)
        {
            Follow(this.TransitionMenu, e);
        }

        private void OnTransitionBack(object sender, RoutedEventArgs e)
        {
            this.TransitionFrame.GoBack();
        }

        private void OnTransitionForward(object sender, RoutedEventArgs e)
        {
            this.TransitionFrame.GoForward();
        }

        /// <summary>
        /// The dictionary this page currently speaks out of. Only one of them is merged at a time, so
        /// swapping it is what a language change comes down to.
        /// </summary>
        private ResourceDictionary? spoken;

        private void OnLanguageChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.LanguagePicker is null)
            {
                return;
            }

            this.SpeakIn(this.LanguagePicker.SelectedIndex switch
                         {
                             1 => "de",
                             2 => "fr",
                             _ => "en"
                         });
        }

        /// <summary>
        /// Puts the dictionary of the given language in place of the one that was there. Every label
        /// written as a DynamicResource follows on its own, the menu items included, since the menu
        /// keeps them in its logical tree where the change reaches them.
        /// </summary>
        private void SpeakIn(string language)
        {
            var next = new ResourceDictionary
                       {
                           Source = new Uri($"pack://application:,,,/MahApps.Metro.Gallery;component/Pages/Languages/Strings.{language}.xaml", UriKind.Absolute)
                       };

            if (this.spoken is not null)
            {
                this.Resources.MergedDictionaries.Remove(this.spoken);
            }

            this.Resources.MergedDictionaries.Add(next);
            this.spoken = next;
        }
    }
}
