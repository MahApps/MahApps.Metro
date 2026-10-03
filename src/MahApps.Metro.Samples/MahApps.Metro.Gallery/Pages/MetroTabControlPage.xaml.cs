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
    /// Interaction logic for MetroTabControlPage.xaml
    /// </summary>
    /// <remarks>
    /// A card holds the same tabs three times over, once per set the library draws. Only the first
    /// of the three is watched; the other two read those properties off it, so one option moves all
    /// three. The four brushes of TabControlHelper are the exception: each set picks its own and a
    /// card is there to show which, so those stay with the control they are turned on.
    /// </remarks>
    public partial class MetroTabControlPage : UserControl
    {
        public MetroTabControlPage()
        {
            this.InitializeComponent();

            this.TabsExample.Watch(this.Tabs, TabControl.TabStripPlacementProperty, Selector.SelectedIndexProperty);
            this.TabsExample.WatchTheTabControlHelper(this.Tabs);

            this.AnimatedExample.Watch(this.Animated, TabControl.TabStripPlacementProperty, Selector.SelectedIndexProperty);
            this.AnimatedExample.WatchTheTabControlHelper(this.Animated);
        }
    }
}
