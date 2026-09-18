// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for CalendarPage.xaml
    /// </summary>
    public partial class CalendarPage : UserControl
    {
        public CalendarPage()
        {
            this.InitializeComponent();

            // the weekend after next, blacked out from here rather than from the markup because
            // dates written into a page go stale the day after they are written
            var saturday = DateTime.Today.AddDays(((int)DayOfWeek.Saturday - (int)DateTime.Today.DayOfWeek + 7) % 7 + 7);

            this.Range.BlackoutDates.Add(new CalendarDateRange(saturday, saturday.AddDays(1)));

            this.MonthExample.Watch(this.Month,
                                    Calendar.DisplayModeProperty,
                                    Calendar.SelectedDateProperty,
                                    Calendar.FirstDayOfWeekProperty,
                                    Calendar.IsTodayHighlightedProperty,
                                    Calendar.SelectionModeProperty);

            this.RangeExample.Watch(this.Range,
                                    Calendar.SelectionModeProperty,
                                    Calendar.DisplayDateStartProperty,
                                    Calendar.DisplayDateEndProperty,
                                    Calendar.IsTodayHighlightedProperty);
        }
    }
}
