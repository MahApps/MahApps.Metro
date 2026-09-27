// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for ProgressRingPage.xaml
    /// </summary>
    public partial class ProgressRingPage : UserControl
    {
        public ProgressRingPage()
        {
            this.InitializeComponent();

            this.RingExample.Watch(this.Ring, ProgressRing.IsActiveProperty, ProgressRing.IsLargeProperty, BackgroundProperty);
            this.RingExample.Watch("Layout", this.Ring, WidthProperty, HeightProperty);

            this.Win10Example.Watch(this.Win10, ProgressRing.IsActiveProperty, ProgressRing.IsLargeProperty, BackgroundProperty);
            this.WinUIExample.Watch(this.WinUI, ProgressRing.IsActiveProperty, BackgroundProperty);
            this.WinUIExample.Watch("Layout", this.WinUI, WidthProperty, HeightProperty);

            this.DeterminateExample.Watch(this.Determinate, RangeBase.ValueProperty, RangeBase.MaximumProperty, ProgressRing.IsIndeterminateProperty, BackgroundProperty);
        }
    }
}
