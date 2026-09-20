// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for DatePickerPage.xaml
    /// </summary>
    public partial class DatePickerPage : UserControl
    {
        public DatePickerPage()
        {
            this.InitializeComponent();

            this.PickExample.Watch(this.Pick,
                                   DatePicker.SelectedDateProperty,
                                   DatePicker.SelectedDateFormatProperty,
                                   DatePicker.FirstDayOfWeekProperty,
                                   DatePicker.IsTodayHighlightedProperty,
                                   IsEnabledProperty);
            this.PickExample.Watch("Attached",
                                   this.Pick,
                                   TextBoxHelper.WatermarkProperty,
                                   TextBoxHelper.ClearTextButtonProperty,
                                   TextBoxHelper.UseFloatingWatermarkProperty,
                                   ControlsHelper.CornerRadiusProperty);
            this.PickExample.Watch("Layout", this.Pick, WidthProperty);
        }
    }
}
