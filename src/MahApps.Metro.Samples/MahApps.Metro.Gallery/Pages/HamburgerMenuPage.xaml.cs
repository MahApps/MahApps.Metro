// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;

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
