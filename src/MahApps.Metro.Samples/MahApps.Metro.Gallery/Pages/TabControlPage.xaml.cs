// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;
using MahApps.Metro.Gallery.Core;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for TabControlPage.xaml
    /// </summary>
    /// <remarks>
    /// A card holds the same tabs three times over, once per set the library draws. Only the first
    /// of the three is watched; the other two read those properties off it, so one option moves all
    /// three. The four brushes of TabControlHelper are the exception: each set picks its own and a
    /// card is there to show which, so those stay with the control they are turned on.
    /// </remarks>
    public partial class TabControlPage : UserControl
    {
        public TabControlPage()
        {
            this.InitializeComponent();

            this.TabsExample.Watch(this.Tabs,
                                   TabControl.TabStripPlacementProperty,
                                   Selector.SelectedIndexProperty);
            this.TabsExample.Watch("Attached", this.Tabs, HeaderedControlHelper.HeaderFontSizeProperty);
            this.TabsExample.WatchTheTabControlHelper(this.Tabs);
            this.TabsExample.Watch("First tab", this.General, ControlsHelper.ContentCharacterCasingProperty);
            this.TabsExample.Watch("Layout", this.Tabs, WidthProperty, HeightProperty);

            this.UnderlineExample.Watch(this.Underlined, Selector.SelectedIndexProperty, TabControl.TabStripPlacementProperty);
            this.UnderlineExample.WatchTheTabControlHelper(this.Underlined);

            this.AnimatedExample.Watch(this.Animated, Selector.SelectedIndexProperty, TabControl.TabStripPlacementProperty);
            this.AnimatedExample.WatchTheTabControlHelper(this.Animated);

            this.SingleRowExample.Watch(this.SingleRow, Selector.SelectedIndexProperty, TabControl.TabStripPlacementProperty);
            this.SingleRowExample.WatchTheTabControlHelper(this.SingleRow);
            this.SingleRowExample.Watch("Layout", this.SingleRow, WidthProperty, HeightProperty);

            this.StudioExample.Watch(this.Studio, Selector.SelectedIndexProperty, TabControl.TabStripPlacementProperty);
            // the close button is on because the item style says so, and a style setter beats the
            // inherited value, so the switch for it belongs on the item rather than on the control
            this.StudioExample.WatchTheTabControlHelper(this.Studio, theCloseButtonToo: false);
            this.StudioExample.Watch("First tab", this.Document, TabControlHelper.CloseButtonEnabledProperty);
        }
    }
}
