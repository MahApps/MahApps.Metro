// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for NumericUpDownPage.xaml
    /// </summary>
    /// <remarks>
    /// Every card holds the same sample three times, once per style set, and only the first of the
    /// three is watched. The other two follow it through bindings written into the markup, so one
    /// option turns all three and the shown XAML stays the one box it was.
    /// </remarks>
    public partial class NumericUpDownPage : UserControl
    {
        public NumericUpDownPage()
        {
            this.InitializeComponent();

            this.NumberExample.Watch(this.Number,
                                     NumericUpDown.ValueProperty,
                                     NumericUpDown.MinimumProperty,
                                     NumericUpDown.MaximumProperty,
                                     NumericUpDown.IntervalProperty,
                                     NumericUpDown.NumericInputModeProperty,
                                     NumericUpDown.HideUpDownButtonsProperty);
            this.NumberExample.Watch("Layout", this.Number, WidthProperty);

            this.FormattedExample.Watch(this.Formatted,
                                        NumericUpDown.ValueProperty,
                                        NumericUpDown.StringFormatProperty,
                                        NumericUpDown.IntervalProperty,
                                        NumericUpDown.SnapToMultipleOfIntervalProperty);
            this.FormattedExample.Watch("Attached", this.Formatted, TextBoxHelper.ClearTextButtonProperty);

            this.ButtonsExample.Watch(this.Buttons,
                                      NumericUpDown.ValueProperty,
                                      NumericUpDown.HideUpDownButtonsProperty,
                                      NumericUpDown.ButtonsAlignmentProperty,
                                      NumericUpDown.SwitchUpDownButtonsProperty,
                                      IsEnabledProperty);
            this.ButtonsExample.Watch("Attached", this.Buttons, TextBoxHelper.WatermarkProperty, TextBoxHelper.ClearTextButtonProperty);

            this.TimeSpanExample.Watch(this.Duration,
                                       TimeSpanUpDown.ValueProperty,
                                       TimeSpanUpDown.IntervalProperty,
                                       TimeSpanUpDown.MinimumProperty,
                                       TimeSpanUpDown.MaximumProperty,
                                       TimeSpanUpDown.HideUpDownButtonsProperty);
            this.TimeSpanExample.Watch("Attached", this.Duration, TextBoxHelper.ClearTextButtonProperty);
        }
    }
}
