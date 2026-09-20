// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for TabControlPage.xaml
    /// </summary>
    public partial class TabControlPage : UserControl
    {
        public TabControlPage()
        {
            this.InitializeComponent();

            this.TabsExample.Watch(this.Tabs,
                                   TabControl.TabStripPlacementProperty,
                                   Selector.SelectedIndexProperty,
                                   BackgroundProperty,
                                   BorderBrushProperty,
                                   BorderThicknessProperty);
            this.TabsExample.Watch("Attached", this.Tabs, HeaderedControlHelper.HeaderFontSizeProperty);
            this.TabsExample.Watch("First tab", this.General, ControlsHelper.ContentCharacterCasingProperty);
            this.TabsExample.Watch("Layout", this.Tabs, WidthProperty, HeightProperty);

            this.UnderlineExample.Watch(this.Underlined, Selector.SelectedIndexProperty, TabControl.TabStripPlacementProperty);
            this.UnderlineExample.Watch("Attached",
                                        this.Underlined,
                                        TabControlHelper.UnderlinedProperty,
                                        TabControlHelper.UnderlineBrushProperty,
                                        TabControlHelper.UnderlineSelectedBrushProperty,
                                        TabControlHelper.UnderlineMouseOverBrushProperty);

            this.AnimatedExample.Watch(this.Animated, Selector.SelectedIndexProperty, TabControl.TabStripPlacementProperty);
            this.AnimatedExample.Watch("Attached", this.Animated, TabControlHelper.TransitionProperty, TabControlHelper.UnderlinedProperty);

            this.SingleRowExample.Watch(this.SingleRow, Selector.SelectedIndexProperty, TabControl.TabStripPlacementProperty);
            this.SingleRowExample.Watch("Layout", this.SingleRow, WidthProperty, HeightProperty);

            this.StudioExample.Watch(this.Studio, Selector.SelectedIndexProperty, TabControl.TabStripPlacementProperty);
            // the close button is on because the item style says so, and a style setter beats the
            // inherited value, so the switch for it belongs on the item rather than on the control
            this.StudioExample.Watch("First tab", this.Document, TabControlHelper.CloseButtonEnabledProperty);
        }
    }
}
