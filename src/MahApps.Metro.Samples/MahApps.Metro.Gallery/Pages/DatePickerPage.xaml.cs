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

            this.Win10Example.Watch(this.Win10, DatePicker.SelectedDateProperty, IsEnabledProperty);
            this.Win10Example.Watch("Attached",
                                    this.Win10,
                                    TextBoxHelper.WatermarkProperty,
                                    TextBoxHelper.ClearTextButtonProperty,
                                    ControlsHelper.CornerRadiusProperty);

            this.WinUIExample.Watch(this.WinUI, DatePicker.SelectedDateProperty, IsEnabledProperty);
            this.WinUIExample.Watch("Attached",
                                    this.WinUI,
                                    TextBoxHelper.WatermarkProperty,
                                    TextBoxHelper.ClearTextButtonProperty,
                                    ControlsHelper.CornerRadiusProperty);
        }
    }
}
