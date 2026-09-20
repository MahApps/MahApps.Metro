// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for TimePickerPage.xaml
    /// </summary>
    public partial class TimePickerPage : UserControl
    {
        public TimePickerPage()
        {
            this.InitializeComponent();

            this.TimeExample.Watch(this.Time,
                                   TimePicker.SelectedDateTimeProperty,
                                   TimePicker.SelectedTimeFormatProperty,
                                   TimePicker.IsClockVisibleProperty,
                                   TimePicker.IsNowButtonVisibleProperty,
                                   TimePicker.NowButtonContentProperty,
                                   TimePicker.IsReadOnlyProperty);
            this.TimeExample.Watch("Layout", this.Time, WidthProperty);

            this.PartsExample.Watch(this.Parts,
                                    TimePicker.PickerVisibilityProperty,
                                    TimePicker.HandVisibilityProperty,
                                    TimePicker.SelectedTimeFormatProperty,
                                    TimePicker.SelectedDateTimeProperty);

            this.ClockExample.Watch(this.BigClock,
                                    TimePicker.ClockSizeProperty,
                                    TimePicker.HandVisibilityProperty,
                                    TimePicker.IsClockVisibleProperty);
        }
    }
}
