// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for ProgressBarPage.xaml
    /// </summary>
    public partial class ProgressBarPage : UserControl
    {
        public ProgressBarPage()
        {
            this.InitializeComponent();

            this.ValueExample.Watch(this.Bar,
                                    RangeBase.ValueProperty,
                                    RangeBase.MinimumProperty,
                                    RangeBase.MaximumProperty,
                                    ProgressBar.OrientationProperty,
                                    ProgressBar.IsIndeterminateProperty);
            this.ValueExample.Watch("Layout", this.Bar, WidthProperty, HeightProperty);

            this.BusyExample.Watch(this.Busy,
                                   ProgressBar.IsIndeterminateProperty,
                                   ProgressBar.OrientationProperty);
            this.BusyExample.Watch("Layout", this.Busy, WidthProperty, HeightProperty);
        }
    }
}
