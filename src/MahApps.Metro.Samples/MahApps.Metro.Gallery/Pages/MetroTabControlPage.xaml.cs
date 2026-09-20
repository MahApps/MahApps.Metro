// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for MetroTabControlPage.xaml
    /// </summary>
    public partial class MetroTabControlPage : UserControl
    {
        public MetroTabControlPage()
        {
            this.InitializeComponent();

            this.TabsExample.Watch(this.Tabs, TabControl.TabStripPlacementProperty, Selector.SelectedIndexProperty);
            this.TabsExample.Watch("Attached",
                                   this.Tabs,
                                   TabControlHelper.UnderlinedProperty,
                                   TabControlHelper.UnderlineBrushProperty);

            this.AnimatedExample.Watch(this.Animated, TabControl.TabStripPlacementProperty, Selector.SelectedIndexProperty);
            this.AnimatedExample.Watch("Attached", this.Animated, TabControlHelper.UnderlinedProperty);
        }
    }
}
