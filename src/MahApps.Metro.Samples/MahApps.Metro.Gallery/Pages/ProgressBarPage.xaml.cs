// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;

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

            this.Win10Example.Watch(this.Win10,
                                    RangeBase.ValueProperty,
                                    ProgressBar.IsIndeterminateProperty,
                                    ProgressBar.OrientationProperty);
            this.Win10Example.Watch("Attached",
                                    this.Win10,
                                    ProgressBarHelper.ShowPausedProperty,
                                    ProgressBarHelper.ShowErrorProperty,
                                    ProgressBarHelper.EllipseDiameterProperty,
                                    ProgressBarHelper.EllipseOffsetProperty);

            this.WinUIExample.Watch(this.WinUI,
                                    RangeBase.ValueProperty,
                                    ProgressBar.IsIndeterminateProperty,
                                    ProgressBar.OrientationProperty);
            this.WinUIExample.Watch("Attached",
                                    this.WinUI,
                                    ProgressBarHelper.ShowPausedProperty,
                                    ProgressBarHelper.ShowErrorProperty);
        }
    }
}
