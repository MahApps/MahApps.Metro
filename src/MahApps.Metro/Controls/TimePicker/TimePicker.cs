// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;

namespace MahApps.Metro.Controls
{
    /// <summary>
    ///     Represents a control that allows the user to select a time.
    /// </summary>
    public class TimePicker : TimePickerBase
    {
        static TimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePicker), new FrameworkPropertyMetadata(typeof(TimePicker)));
        }

        public TimePicker()
        {
            this.IsDatePickerVisible = false;
        }

        /// <inheritdoc />
        protected override void FocusElementAfterIsDropDownOpenChanged()
        {
            if (this.hourInput is null)
            {
                return;
            }

            // When the popup is opened set focus to the hour input.
            // Do this asynchronously because the IsDropDownOpen could
            // have been set even before the template for the DatePicker is
            // applied. And this would mean that the visuals wouldn't be available yet.

            this.Dispatcher.BeginInvoke(DispatcherPriority.Input, (Action)delegate
                {
                    // setting the focus to the calendar will focus the correct date.
                    this.hourInput.Focus();
                });
        }

        /// <inheritdoc />
        protected override void SetSelectedDateTime()
        {
            if (this.textBox is null)
            {
                return;
            }

            const DateTimeStyles dateTimeParseStyle = DateTimeStyles.AllowWhiteSpaces
                                                      & DateTimeStyles.AssumeLocal
                                                      & DateTimeStyles.NoCurrentDateDefault;

            if (DateTime.TryParse(this.textBox.Text, this.SpecificCultureInfo, dateTimeParseStyle, out var timeSpan))
            {
                this.SetCurrentValue(SelectedDateTimeProperty, this.SelectedDateTime.GetValueOrDefault().Date + timeSpan.TimeOfDay);
            }
            else
            {
                this.SetCurrentValue(SelectedDateTimeProperty, null);
                if (this.SelectedDateTime == null)
                {
                    // if already null, overwrite wrong data in textbox
                    this.WriteValueToTextBox();
                }
            }
        }
    }
}