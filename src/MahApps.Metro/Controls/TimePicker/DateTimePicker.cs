// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    ///     Represents a control that allows the user to select a date and a time.
    /// </summary>
    [TemplatePart(Name = PART_PopupContainer, Type = typeof(StackPanel))]
    [TemplatePart(Name = PART_Calendar, Type = typeof(ContentPresenter))]
    [StyleTypedProperty(Property = nameof(CalendarStyle), StyleTargetType = typeof(Calendar))]
    public class DateTimePicker : TimePickerBase
    {
        private const string PART_PopupContainer = "PART_PopupContainer";
        private const string PART_Calendar = "PART_Calendar";

        private FrameworkElement popupContainer;
        private ContentPresenter popupCalendarPresenter;
        private Calendar calendar;

        /// <summary>Identifies the <see cref="DisplayDateEnd"/> dependency property.</summary>
        public static readonly DependencyProperty DisplayDateEndProperty = DatePicker.DisplayDateEndProperty.AddOwner(typeof(DateTimePicker));

        /// <summary>
        ///     Gets or sets the last date to be displayed.
        /// </summary>
        /// <returns>The last date to display.</returns>
        public DateTime? DisplayDateEnd
        {
            get => (DateTime?)this.GetValue(DisplayDateEndProperty);
            set => this.SetValue(DisplayDateEndProperty, value);
        }

        /// <summary>Identifies the <see cref="DisplayDate"/> dependency property.</summary>
        public static readonly DependencyProperty DisplayDateProperty = DatePicker.DisplayDateProperty.AddOwner(typeof(DateTimePicker));

        /// <summary>
        ///     Gets or sets the date to display
        /// </summary>
        /// <returns>
        ///     The date to display. The default is <see cref="DateTime.Today" />.
        /// </returns>
        public DateTime DisplayDate
        {
            get => (DateTime)this.GetValue(DisplayDateProperty);
            set => this.SetValue(DisplayDateProperty, value);
        }

        /// <summary>Identifies the <see cref="DisplayDateStart"/> dependency property.</summary>
        public static readonly DependencyProperty DisplayDateStartProperty = DatePicker.DisplayDateStartProperty.AddOwner(typeof(DateTimePicker));

        /// <summary>
        ///     Gets or sets the first date to be displayed.
        /// </summary>
        /// <returns>The first date to display.</returns>
        public DateTime? DisplayDateStart
        {
            get => (DateTime?)this.GetValue(DisplayDateStartProperty);
            set => this.SetValue(DisplayDateStartProperty, value);
        }

        /// <summary>Identifies the <see cref="FirstDayOfWeek"/> dependency property.</summary>
        public static readonly DependencyProperty FirstDayOfWeekProperty = DatePicker.FirstDayOfWeekProperty.AddOwner(typeof(DateTimePicker));

        /// <summary>
        ///     Gets or sets the day that is considered the beginning of the week.
        /// </summary>
        /// <returns>
        ///     A <see cref="DayOfWeek" /> that represents the beginning of the week. The default is the
        ///     <see cref="System.Globalization.DateTimeFormatInfo.FirstDayOfWeek" /> that is determined by the current culture.
        /// </returns>
        public DayOfWeek FirstDayOfWeek
        {
            get => (DayOfWeek)this.GetValue(FirstDayOfWeekProperty);
            set => this.SetValue(FirstDayOfWeekProperty, value);
        }

        /// <summary>Identifies the <see cref="IsTodayHighlighted"/> dependency property.</summary>
        public static readonly DependencyProperty IsTodayHighlightedProperty = DatePicker.IsTodayHighlightedProperty.AddOwner(typeof(DateTimePicker));

        /// <summary>
        ///     Gets or sets a value that indicates whether the current date will be highlighted.
        /// </summary>
        /// <returns>true if the current date is highlighted; otherwise, false. The default is true. </returns>
        public bool IsTodayHighlighted
        {
            get => (bool)this.GetValue(IsTodayHighlightedProperty);
            set => this.SetValue(IsTodayHighlightedProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="SelectedDateFormat"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedDateFormatProperty = DatePicker.SelectedDateFormatProperty.AddOwner(typeof(DateTimePicker), new FrameworkPropertyMetadata(DatePickerFormat.Short, OnSelectedDateFormatChanged));

        /// <summary>
        /// Gets or sets the format that is used to display the selected date.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(DatePickerFormat.Short)]
        public DatePickerFormat SelectedDateFormat
        {
            get => (DatePickerFormat)this.GetValue(SelectedDateFormatProperty);
            set => this.SetValue(SelectedDateFormatProperty, value);
        }

        /// <summary>Identifies the <see cref="Orientation"/> dependency property.</summary>
        public static readonly DependencyProperty OrientationProperty
            = DependencyProperty.Register(nameof(Orientation),
                                          typeof(Orientation),
                                          typeof(DateTimePicker),
                                          new PropertyMetadata(Orientation.Horizontal, null, CoerceOrientation));

        /// <summary>
        ///     Gets or sets a value that indicates the dimension by which calendar and clock are stacked.
        /// </summary>
        /// <returns>
        ///     The <see cref="System.Windows.Controls.Orientation" /> of the calendar and clock. The default is
        ///     <see cref="System.Windows.Controls.Orientation.Horizontal" />.
        /// </returns>
        [Category("Layout")]
        public Orientation Orientation
        {
            get => (Orientation)this.GetValue(OrientationProperty);
            set => this.SetValue(OrientationProperty, value);
        }

        /// <summary>Identifies the <see cref="CalendarStyle"/> dependency property.</summary>
        public static readonly DependencyProperty CalendarStyleProperty
            = DependencyProperty.Register(nameof(CalendarStyle),
                                          typeof(Style),
                                          typeof(DateTimePicker));

        /// <summary>
        /// Gets or sets the style that is used when rendering the calendar.
        /// </summary>
        public Style CalendarStyle
        {
            get => (Style)this.GetValue(CalendarStyleProperty);
            set => this.SetValue(CalendarStyleProperty, value);
        }

        /// <summary>
        /// Gets the days that are not selectable.
        /// </summary>
        public CalendarBlackoutDatesCollection BlackoutDates => this.calendar.BlackoutDates;

        static DateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));
            IsClockVisibleProperty.OverrideMetadata(typeof(DateTimePicker), new PropertyMetadata(OnClockVisibilityChanged));
        }

        public DateTimePicker()
        {
            this.InitializeCalendar();

            this.SetCurrentValue(DisplayDateProperty, DateTime.Today);
        }

        /// <inheritdoc />
        protected override void FocusElementAfterIsDropDownOpenChanged()
        {
            if (this.calendar is null)
            {
                return;
            }

            // When the popup is opened set focus to the DisplayDate button.
            // Do this asynchronously because the IsDropDownOpen could
            // have been set even before the template for the DatePicker is
            // applied. And this would mean that the visuals wouldn't be available yet.

            this.Dispatcher.BeginInvoke(DispatcherPriority.Input, (Action)delegate
                {
                    // setting the focus to the calendar will focus the correct date.
                    this.calendar.Focus();
                });
        }

        private void InitializeCalendar()
        {
            this.calendar = new Calendar();

            this.calendar.PreviewKeyDown += this.CalendarPreviewKeyDown;
            this.calendar.DisplayDateChanged += this.CalendarDisplayDateChanged;
            this.calendar.SelectedDatesChanged += this.CalendarSelectedDateChanged;
            this.calendar.PreviewMouseUp += CalendarPreviewMouseUp;

            this.calendar.HorizontalAlignment = HorizontalAlignment.Left;
            this.calendar.VerticalAlignment = VerticalAlignment.Top;

            this.calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            this.calendar.SetBinding(ForegroundProperty, this.GetBinding(ForegroundProperty));
            this.calendar.SetBinding(StyleProperty, this.GetBinding(CalendarStyleProperty));
            this.calendar.SetBinding(Calendar.IsTodayHighlightedProperty, this.GetBinding(IsTodayHighlightedProperty));
            this.calendar.SetBinding(Calendar.FirstDayOfWeekProperty, this.GetBinding(FirstDayOfWeekProperty));
            this.calendar.SetBinding(Calendar.SelectedDateProperty, this.GetBinding(SelectedDateTimeProperty, BindingMode.OneWay));
            this.calendar.SetBinding(Calendar.DisplayDateProperty, this.GetBinding(DisplayDateProperty));
            this.calendar.SetBinding(Calendar.DisplayDateStartProperty, this.GetBinding(DisplayDateStartProperty));
            this.calendar.SetBinding(Calendar.DisplayDateEndProperty, this.GetBinding(DisplayDateEndProperty));
            this.calendar.SetBinding(FontFamilyProperty, this.GetBinding(FontFamilyProperty));
            this.calendar.SetBinding(FontSizeProperty, this.GetBinding(FontSizeProperty));
            this.calendar.SetBinding(FlowDirectionProperty, this.GetBinding(FlowDirectionProperty));

            RenderOptions.SetClearTypeHint(this.calendar, ClearTypeHint.Enabled);
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            if (!(this.popupCalendarPresenter is null))
            {
                this.popupCalendarPresenter.Content = null;
            }

            base.OnApplyTemplate();

            this.popupContainer = this.GetTemplateChild(PART_PopupContainer) as StackPanel;
            this.popupContainer?.SetBinding(StackPanel.OrientationProperty, this.GetBinding(OrientationProperty));

            this.popupCalendarPresenter = this.GetTemplateChild(PART_Calendar) as ContentPresenter;
            if (!(this.popupCalendarPresenter is null))
            {
                this.popupCalendarPresenter.Content = this.calendar;
            }
        }

        private static void CalendarPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.Captured is CalendarItem)
            {
                Mouse.Capture(null);
            }
        }

        private void CalendarDisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            if (e.AddedDate is DateTime addedDate && addedDate != this.DisplayDate)
            {
                this.SetCurrentValue(DisplayDateProperty, addedDate);
            }
        }

        private void CalendarPreviewKeyDown(object sender, RoutedEventArgs e)
        {
            var keyEventArgs = (KeyEventArgs)e;

            Debug.Assert(keyEventArgs != null);

            if (keyEventArgs.Key == Key.Escape || ((keyEventArgs.Key == Key.Enter || keyEventArgs.Key == Key.Space) && this.calendar.DisplayMode == CalendarMode.Month))
            {
                this.SetCurrentValue(IsDropDownOpenProperty, BooleanBoxes.FalseBox);
                if (keyEventArgs.Key == Key.Escape)
                {
                    this.SetCurrentValue(SelectedDateTimeProperty, this.originalSelectedDateTime);
                }
            }
        }

        /// <inheritdoc />
        protected override void OnPopUpOpened()
        {
            if (this.calendar != null)
            {
                this.calendar.DisplayMode = CalendarMode.Month;
                this.calendar.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }
        }

        /// <inheritdoc />
        protected override void OnPopUpClosed()
        {
            if (this.calendar.IsKeyboardFocusWithin)
            {
                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }
        }

        private static void OnSelectedDateFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateTimePicker dateTimePicker)
            {
                dateTimePicker.WriteValueToTextBox();
            }
        }

        /// <inheritdoc />
        protected override void ApplyCulture()
        {
            base.ApplyCulture();

            this.SetCurrentValue(FirstDayOfWeekProperty, this.SpecificCultureInfo.DateTimeFormat.FirstDayOfWeek);
        }

        /// <inheritdoc />
        protected override string GetValueForTextBox()
        {
            var formatInfo = this.SpecificCultureInfo.DateTimeFormat;
            var timeFormat = this.SelectedTimeFormat == TimePickerFormat.Long ? formatInfo.LongTimePattern : formatInfo.ShortTimePattern;
            var dateFormat = this.SelectedDateFormat == DatePickerFormat.Long ? formatInfo.LongDatePattern : formatInfo.ShortDatePattern;

            var dateTimeFormat = string.Intern($"{dateFormat} {timeFormat}");

            var selectedDateTimeFromGui = this.GetSelectedDateTimeFromGUI();
            var valueForTextBox = selectedDateTimeFromGui?.ToString(dateTimeFormat, this.SpecificCultureInfo);
            return valueForTextBox;
        }

        /// <inheritdoc />
        protected override void SetSelectedDateTime()
        {
            if (this.textBox is null)
            {
                return;
            }

            if (DateTime.TryParse(this.textBox.Text, this.SpecificCultureInfo, System.Globalization.DateTimeStyles.None, out var dateTime))
            {
                this.SetCurrentValue(SelectedDateTimeProperty, dateTime);
                this.SetCurrentValue(DisplayDateProperty, dateTime);
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

        private static object CoerceOrientation(DependencyObject d, object basevalue)
        {
            if (((DateTimePicker)d).IsClockVisible)
            {
                return basevalue;
            }

            return Orientation.Vertical;
        }

        private static void OnClockVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(OrientationProperty);
        }

        private void CalendarSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && this.SelectedDateTime.HasValue && DateTime.Compare((DateTime)e.AddedItems[0], this.SelectedDateTime.Value) != 0)
            {
                this.SetCurrentValue(SelectedDateTimeProperty, (DateTime?)e.AddedItems[0] + this.GetSelectedTimeFromGUI());
            }
            else
            {
                if (e.AddedItems.Count == 0)
                {
                    this.SetCurrentValue(SelectedDateTimeProperty, (DateTime?)null);
                    return;
                }

                if (!this.SelectedDateTime.HasValue)
                {
                    if (e.AddedItems.Count > 0)
                    {
                        this.SetCurrentValue(SelectedDateTimeProperty, (DateTime?)e.AddedItems[0] + this.GetSelectedTimeFromGUI());
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void OnSelectedDateTimeChanged(DateTime? oldValue, DateTime? newValue)
        {
            this.calendar?.SetCurrentValue(Calendar.SelectedDateProperty, newValue);

            base.OnSelectedDateTimeChanged(oldValue, newValue);
        }

        private DateTime? GetSelectedDateTimeFromGUI()
        {
            // Because Calendar.SelectedDate is bound to this.SelectedDate return this.SelectedDate
            var selectedDate = this.SelectedDateTime;
            if (selectedDate != null)
            {
                return selectedDate.Value.Date + this.GetSelectedTimeFromGUI().GetValueOrDefault();
            }

            return null;
        }
    }
}