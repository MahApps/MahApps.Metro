// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for SliderPage.xaml
    /// </summary>
    public partial class SliderPage : UserControl
    {
        public SliderPage()
        {
            this.InitializeComponent();

            this.Win10Example.Watch(this.Win10,
                                    RangeBase.ValueProperty,
                                    RangeBase.MinimumProperty,
                                    RangeBase.MaximumProperty,
                                    Slider.OrientationProperty,
                                    Slider.IsDirectionReversedProperty,
                                    Slider.IsMoveToPointEnabledProperty);
            this.Win10Example.Watch("Attached", this.Win10, SliderHelper.EnableMouseWheelProperty);
            this.Win10Example.Watch("Layout", this.Win10, WidthProperty);

            this.PlainExample.Watch(this.Plain,
                                    RangeBase.ValueProperty,
                                    Slider.OrientationProperty,
                                    Slider.IsMoveToPointEnabledProperty);
            this.PlainExample.Watch("Layout", this.Plain, WidthProperty);

            this.ColourExample.Watch(this.Colours, RangeBase.ValueProperty, Slider.OrientationProperty);
            this.ColourExample.Watch("Attached",
                                     this.Colours,
                                     SliderHelper.ThumbFillBrushProperty,
                                     SliderHelper.ThumbFillHoverBrushProperty,
                                     SliderHelper.ThumbFillPressedBrushProperty,
                                     SliderHelper.TrackFillBrushProperty,
                                     SliderHelper.TrackValueFillBrushProperty);
            this.ColourExample.Watch("Layout", this.Colours, WidthProperty);

            this.TicksExample.Watch(this.Ticks,
                                    RangeBase.ValueProperty,
                                    Slider.TickPlacementProperty,
                                    Slider.TickFrequencyProperty,
                                    Slider.IsSnapToTickEnabledProperty,
                                    Slider.OrientationProperty);
            this.TicksExample.Watch("Layout", this.Ticks, WidthProperty);

            this.FlatExample.Watch(this.Flat,
                                   RangeBase.ValueProperty,
                                   Slider.OrientationProperty,
                                   ForegroundProperty,
                                   BackgroundProperty,
                                   BorderBrushProperty,
                                   Slider.TickPlacementProperty,
                                   Slider.TickFrequencyProperty);
            this.FlatExample.Watch("Layout", this.Flat, WidthProperty);
        }
    }
}
