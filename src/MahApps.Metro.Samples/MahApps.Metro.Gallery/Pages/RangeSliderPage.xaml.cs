// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for RangeSliderPage.xaml
    /// </summary>
    public partial class RangeSliderPage : UserControl
    {
        public RangeSliderPage()
        {
            this.InitializeComponent();

            this.RangeExample.Watch(this.Range,
                                    RangeSlider.LowerValueProperty,
                                    RangeSlider.UpperValueProperty,
                                    RangeSlider.MinRangeProperty,
                                    RangeSlider.MoveWholeRangeProperty,
                                    RangeSlider.ExtendedModeProperty,
                                    RangeSlider.OrientationProperty);
            this.RangeExample.Watch("Layout", this.Range, WidthProperty);

            this.TicksExample.Watch(this.Ticks,
                                    RangeSlider.LowerValueProperty,
                                    RangeSlider.UpperValueProperty,
                                    RangeSlider.TickFrequencyProperty,
                                    RangeSlider.TickPlacementProperty,
                                    RangeSlider.IsSnapToTickEnabledProperty,
                                    RangeSlider.AutoToolTipPlacementProperty);
        }
    }
}
