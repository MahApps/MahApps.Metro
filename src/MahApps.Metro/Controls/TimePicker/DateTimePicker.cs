using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace MahApps.Metro.Controls
{
    /// <summary>
    ///     Represents a control that allows the user to select a date and a time.
    /// </summary>
    [TemplatePart(Name = ElementCalendar, Type = typeof(Calendar))]
    public class DateTimePicker : TimePickerBase
    {
        private const string ElementCalendar = "PART_Calendar";
        private Calendar _calendar;

        public static readonly DependencyProperty DisplayDateEndProperty = DatePicker.DisplayDateEndProperty.AddOwner(typeof(DateTimePicker));
        
        public static readonly DependencyProperty DisplayDateProperty = DatePicker.DisplayDateProperty.AddOwner(typeof(DateTimePicker));
        
        public static readonly DependencyProperty DisplayDateStartProperty = DatePicker.DisplayDateStartProperty.AddOwner(typeof(DateTimePicker));
        
        public static readonly DependencyProperty FirstDayOfWeekProperty = DatePicker.FirstDayOfWeekProperty.AddOwner(typeof(DateTimePicker));
        
        public static readonly DependencyProperty IsTodayHighlightedProperty = DatePicker.IsTodayHighlightedProperty.AddOwner(typeof(DateTimePicker));
        
        public static readonly DependencyProperty SelectedDateFormatProperty
            = DatePicker.SelectedDateFormatProperty.AddOwner(typeof(DateTimePicker), 
                                                             new FrameworkPropertyMetadata(DatePickerFormat.Short, OnSelectedDateFormatChanged));

        public static readonly DependencyProperty OrientationProperty
            = DependencyProperty.Register("Orientation", 
                                          typeof(Orientation), 
                                          typeof(DateTimePicker), 
                                          new PropertyMetadata(Orientation.Horizontal, null, CoerceOrientation));

        static DateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));
            IsClockVisibleProperty.OverrideMetadata(typeof(DateTimePicker), new PropertyMetadata(OnClockVisibilityChanged));
        }

        public DateTimePicker()
        {
        }

        /// <inheritdoc />
        protected override void FocusElementAfterIsDropDownOpenChanged()
        {
            if (this._calendar is null)
            {
                return;
            }
            
            // When the popup is opened set focus to the DisplayDate button.
            // Do this asynchronously because the IsDropDownOpen could
            // have been set even before the template for the DatePicker is
            // applied. And this would mean that the visuals wouldn't be available yet.

            this.Dispatcher.BeginInvoke(DispatcherPriority.Input, (Action)delegate ()
                {
                    // setting the focus to the calendar will focus the correct date.
                    this._calendar.Focus();
                });
        }

        /// <summary>
        ///     Gets or sets the date to display
        /// </summary>
        /// <returns>
        ///     The date to display. The default is <see cref="DateTime.Today" />.
        /// </returns>
        public DateTime DisplayDate
        {
            get { return (DateTime)GetValue(DisplayDateProperty); }
            set { SetValue(DisplayDateProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the last date to be displayed.
        /// </summary>
        /// <returns>The last date to display.</returns>
        public DateTime? DisplayDateEnd
        {
            get { return (DateTime?)GetValue(DisplayDateEndProperty); }
            set { SetValue(DisplayDateEndProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the first date to be displayed.
        /// </summary>
        /// <returns>The first date to display.</returns>
        public DateTime? DisplayDateStart
        {
            get { return (DateTime?)GetValue(DisplayDateStartProperty); }
            set { SetValue(DisplayDateStartProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the day that is considered the beginning of the week.
        /// </summary>
        /// <returns>
        ///     A <see cref="DayOfWeek" /> that represents the beginning of the week. The default is the
        ///     <see cref="System.Globalization.DateTimeFormatInfo.FirstDayOfWeek" /> that is determined by the current culture.
        /// </returns>
        public DayOfWeek FirstDayOfWeek
        {
            get { return (DayOfWeek)GetValue(FirstDayOfWeekProperty); }
            set { SetValue(FirstDayOfWeekProperty, value); }
        }

        /// <summary>
        /// Gets or sets the format that is used to display the selected date.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(DatePickerFormat.Short)]
        public DatePickerFormat SelectedDateFormat
        {
            get { return (DatePickerFormat)GetValue(SelectedDateFormatProperty); }
            set { SetValue(SelectedDateFormatProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value that indicates whether the current date will be highlighted.
        /// </summary>
        /// <returns>true if the current date is highlighted; otherwise, false. The default is true. </returns>
        public bool IsTodayHighlighted
        {
            get { return (bool)GetValue(IsTodayHighlightedProperty); }
            set { SetValue(IsTodayHighlightedProperty, value); }
        }

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
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            if (_calendar != null)
            {
                _calendar.PreviewKeyDown -= this.CalendarPreviewKeyDown;
                _calendar.DisplayDateChanged -= this.CalendarDisplayDateChanged;
                _calendar.SelectedDatesChanged -= this.CalendarSelectedDateChanged;
                _calendar.PreviewMouseUp -= CalendarPreviewMouseUp;
            }

            base.OnApplyTemplate();

            _calendar = GetTemplateChild(ElementCalendar) as Calendar;

            if (_calendar != null)
            {
                _calendar.PreviewKeyDown += this.CalendarPreviewKeyDown;
                _calendar.DisplayDateChanged += this.CalendarDisplayDateChanged;
                _calendar.SelectedDatesChanged += this.CalendarSelectedDateChanged;
                _calendar.PreviewMouseUp += CalendarPreviewMouseUp;
                
                _calendar.SetBinding(Calendar.SelectedDateProperty, GetBinding(SelectedDateTimeProperty, BindingMode.OneWay));
                _calendar.SetBinding(Calendar.DisplayDateProperty, GetBinding(DisplayDateProperty));
                _calendar.SetBinding(Calendar.DisplayDateStartProperty, GetBinding(DisplayDateStartProperty));
                _calendar.SetBinding(Calendar.DisplayDateEndProperty, GetBinding(DisplayDateEndProperty));
                _calendar.SetBinding(Calendar.FirstDayOfWeekProperty, GetBinding(FirstDayOfWeekProperty));
                _calendar.SetBinding(Calendar.IsTodayHighlightedProperty, GetBinding(IsTodayHighlightedProperty));
                _calendar.SetBinding(FlowDirectionProperty, GetBinding(FlowDirectionProperty));
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
                SetCurrentValue(DisplayDateProperty, addedDate);
            }
        }

        private void CalendarPreviewKeyDown(object sender, RoutedEventArgs e)
        {
            var calendar = sender as Calendar;
            var keyEventArgs = (KeyEventArgs)e;

            Debug.Assert(calendar != null);
            Debug.Assert(keyEventArgs != null);

            if (keyEventArgs.Key == Key.Escape || ((keyEventArgs.Key == Key.Enter || keyEventArgs.Key == Key.Space) && calendar.DisplayMode == CalendarMode.Month))
            {
                this.SetCurrentValue(IsDropDownOpenProperty, false);
                if (keyEventArgs.Key == Key.Escape)
                {
                    SetCurrentValue(SelectedDateTimeProperty, _originalSelectedDateTime);
                }
            }
        }

        protected override void OnPopUpOpened()
        {
            if (this._calendar != null)
            {
                this._calendar.DisplayMode = CalendarMode.Month;
                this._calendar.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }
        }

        protected override void OnPopUpClosed()
        {
            if (_calendar.IsKeyboardFocusWithin)
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

        protected override void ApplyCulture()
        {
            base.ApplyCulture();

            SetCurrentValue(FirstDayOfWeekProperty, SpecificCultureInfo.DateTimeFormat.FirstDayOfWeek);
        }

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

        protected override void SetSelectedDateTime()
        {
            if (this._textBox is null)
            {
                return;
            }


            if (DateTime.TryParse(_textBox.Text, SpecificCultureInfo, System.Globalization.DateTimeStyles.None, out var dateTime))
            {
                this.SetCurrentValue(SelectedDateTimeProperty, dateTime);
                this.SetCurrentValue(DisplayDateProperty, dateTime);
            }
            else
            {
                this.SetCurrentValue(SelectedDateTimeProperty, null);
                if (SelectedDateTime == null)
                {
                    // if already null, overwrite wrong data in textbox
                    WriteValueToTextBox();
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

        protected override void OnSelectedDateTimeChanged(DateTime? oldValue, DateTime? newValue)
        {
            this._calendar?.SetCurrentValue(Calendar.SelectedDateProperty, newValue);

            base.OnSelectedDateTimeChanged(oldValue, newValue);
        }

        private DateTime? GetSelectedDateTimeFromGUI()
        {
            // Because Calendar.SelectedDate is bound to this.SelectedDate return this.SelectedDate
            var selectedDate = SelectedDateTime;
            if (selectedDate != null)
            {
                return selectedDate.Value.Date + GetSelectedTimeFromGUI().GetValueOrDefault();
            }

            return null;
        }
    }
}