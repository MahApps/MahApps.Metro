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
                                   HamburgerMenu.CanResizeOpenPaneProperty);
            this.MenuExample.Watch("Layout", this.Menu, WidthProperty, HeightProperty);
        }
    }
}
