// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for AnalogClockPage.xaml
    /// </summary>
    public partial class AnalogClockPage : UserControl
    {
        public AnalogClockPage()
        {
            this.InitializeComponent();

            // A clock with nothing to show stands at twelve, which says nothing about it. The time
            // of day is what one of these is for, and it is where a binding would put it anyway.
            this.Face.Time = DateTime.Now;
            this.Large.Time = DateTime.Now;
            this.Look.Time = DateTime.Now;

            this.FaceExample.Watch(this.Face,
                                   AnalogClock.TimeProperty,
                                   AnalogClock.HandVisibilityProperty);

            this.SizeExample.Watch(this.Large,
                                   AnalogClock.TimeProperty,
                                   AnalogClock.HandVisibilityProperty);
            this.SizeExample.Watch("Layout", this.Large, WidthProperty, HeightProperty);

            this.LookExample.Watch(this.Look,
                                   BackgroundProperty,
                                   BorderBrushProperty,
                                   BorderThicknessProperty,
                                   ForegroundProperty);
        }
    }
}
