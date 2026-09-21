// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for DateTimePickerPage.xaml
    /// </summary>
    public partial class DateTimePickerPage : UserControl
    {
        public DateTimePickerPage()
        {
            this.InitializeComponent();

            this.BothExample.Watch(this.Both,
                                   DateTimePicker.SelectedDateTimeProperty,
                                   DateTimePicker.IsClockVisibleProperty,
                                   DateTimePicker.OrientationProperty,
                                   DateTimePicker.SelectedTimeFormatProperty,
                                   DateTimePicker.SelectedDateFormatProperty,
                                   DateTimePicker.HandVisibilityProperty,
                                   DateTimePicker.IsReadOnlyProperty);
            this.BothExample.Watch("Layout", this.Both, WidthProperty);

            this.Win10Example.Watch(this.Win10,
                                    DateTimePicker.SelectedDateTimeProperty,
                                    DateTimePicker.IsClockVisibleProperty,
                                    DateTimePicker.OrientationProperty,
                                    IsEnabledProperty);

            this.WinUIExample.Watch(this.WinUI,
                                    DateTimePicker.SelectedDateTimeProperty,
                                    DateTimePicker.IsClockVisibleProperty,
                                    DateTimePicker.OrientationProperty,
                                    IsEnabledProperty);
        }
    }
}
