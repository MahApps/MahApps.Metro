// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for MetroProgressBarPage.xaml
    /// </summary>
    public partial class MetroProgressBarPage : UserControl
    {
        public MetroProgressBarPage()
        {
            this.InitializeComponent();

            this.ProgressExample.Watch(this.Bar,
                                       RangeBase.ValueProperty,
                                       RangeBase.MinimumProperty,
                                       RangeBase.MaximumProperty,
                                       ProgressBar.IsIndeterminateProperty,
                                       ProgressBar.OrientationProperty);
            this.ProgressExample.Watch("Layout", this.Bar, WidthProperty, HeightProperty);

            this.WaitingExample.Watch(this.Waiting,
                                      ProgressBar.IsIndeterminateProperty,
                                      MetroProgressBar.EllipseDiameterProperty,
                                      MetroProgressBar.EllipseOffsetProperty);
        }
    }
}
