// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    }
}
