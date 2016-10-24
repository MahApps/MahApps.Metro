namespace MahApps.Metro.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    /// <summary>
    ///     Represents a control that allows the user to select a date and a time.
    /// </summary>
    [TemplatePart(Name = ElementCalendar, Type = typeof(Calendar))]
    [DefaultEvent("SelectedDateChanged")]
    public class DateTimePicker : TimePickerBase
    {
        public static readonly DependencyProperty DisplayDateEndProperty = DatePicker.DisplayDateEndProperty.AddOwner(typeof(DateTimePicker));
        public static readonly DependencyProperty DisplayDateProperty = DatePicker.DisplayDateProperty.AddOwner(typeof(DateTimePicker));
        public static readonly DependencyProperty DisplayDateStartProperty = DatePicker.DisplayDateStartProperty.AddOwner(typeof(DateTimePicker));
        public static readonly DependencyProperty FirstDayOfWeekProperty = DatePicker.FirstDayOfWeekProperty.AddOwner(typeof(DateTimePicker));
        public static readonly DependencyProperty IsTodayHighlightedProperty = DatePicker.IsTodayHighlightedProperty.AddOwner(typeof(DateTimePicker));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", 
            typeof(Orientation), 
            typeof(DateTimePicker), 
            new PropertyMetadata(Orientation.Horizontal, null, CoerceOrientation));

        public static readonly RoutedEvent SelectedDateChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectedDateChanged", 
            RoutingStrategy.Direct, 
            typeof(EventHandler<TimePickerBaseSelectionChangedEventArgs<DateTime?>>),
            typeof(DateTimePicker));

        public static readonly DependencyProperty SelectedDateProperty = DatePicker.SelectedDateProperty.AddOwner(typeof(DateTimePicker), new FrameworkPropertyMetadata(default(DateTime?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDateChanged));
      
        private const string ElementCalendar = "PART_Calendar";
        private Calendar _calendar;
        private bool _deactivateWriteValueToTextBox;

        static DateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));
            IsClockVisibleProperty.OverrideMetadata(typeof(DateTimePicker), new PropertyMetadata(OnClockVisibilityChanged));
        }

        /// <summary>
        ///     Occurs when the <see cref="SelectedDate" /> property is changed.
        /// </summary>
        public event EventHandler<TimePickerBaseSelectionChangedEventArgs<DateTime?>> SelectedDateChanged
        {
            add { AddHandler(SelectedDateChangedEvent, value); }
            remove { RemoveHandler(SelectedDateChangedEvent, value); }
        }

        /// <summary>
        ///     Gets or sets the date to display
        /// </summary>
        /// <returns>
        ///     The date to display. The default is <see cref="DateTime.Today" />.
        /// </returns>
        public DateTime? DisplayDate
        {
            get { return (DateTime?)GetValue(DisplayDateProperty); }
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

        /// <summary>
        ///     Gets or sets the currently selected date.
        /// </summary>
        /// <returns>
        ///     The date currently selected. The default is null.
        /// </returns>
        public DateTime? SelectedDate
        {
            get { return (DateTime?)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            _calendar = GetTemplateChild(ElementCalendar) as Calendar;
            base.OnApplyTemplate();
            SetDatePartValues();
        }

        protected virtual void OnSelectedDateChanged(TimePickerBaseSelectionChangedEventArgs<DateTime?> e)
        {
            RaiseEvent(e);
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();

            if (_calendar != null)
            {
                _calendar.SetBinding(Calendar.DisplayDateProperty, GetBinding(DisplayDateProperty));
                _calendar.SetBinding(Calendar.DisplayDateStartProperty, GetBinding(DisplayDateStartProperty));
                _calendar.SetBinding(Calendar.DisplayDateEndProperty, GetBinding(DisplayDateEndProperty));
                _calendar.SetBinding(Calendar.FirstDayOfWeekProperty, GetBinding(FirstDayOfWeekProperty));
                _calendar.SetBinding(Calendar.IsTodayHighlightedProperty, GetBinding(IsTodayHighlightedProperty));
                _calendar.SetBinding(FlowDirectionProperty, GetBinding(FlowDirectionProperty));
                _calendar.SetBinding(Calendar.SelectedDateProperty, GetBinding(SelectedDateProperty));
            }
        }

        protected sealed override void ApplyCulture()
        {
            base.ApplyCulture();

            FirstDayOfWeek = SpecificCultureInfo.DateTimeFormat.FirstDayOfWeek;
        }

        protected override string GetValueForTextBox()
        {
            var formatInfo = SpecificCultureInfo.DateTimeFormat;
            var dateTimeFormat = string.Intern($"{formatInfo.ShortDatePattern} {formatInfo.LongTimePattern}");

            var selectedDateTimeFromGui = this.GetSelectedDateTimeFromGUI();
            var valueForTextBox = selectedDateTimeFromGui?.ToString(dateTimeFormat, this.SpecificCultureInfo);
            return valueForTextBox;
        }

        protected override void OnRangeBaseValueChanged(object sender, SelectionChangedEventArgs e)
        {
            base.OnRangeBaseValueChanged(sender, e);
            
            SetDatePartValues();
        }

        protected override void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            DateTime ts;
            if (DateTime.TryParse(((DatePickerTextBox)sender).Text, SpecificCultureInfo, System.Globalization.DateTimeStyles.None, out ts))
            {
                SelectedDate = ts;
            }
            else
            {
                if (SelectedDate == null)
                {
                    // if already null, overwrite wrong data in textbox
                    WriteValueToTextBox();
                }
                SelectedDate = null;
            }
        }

        protected override void WriteValueToTextBox()
        {
            if (!_deactivateWriteValueToTextBox)
            {
                base.WriteValueToTextBox();
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

        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dateTimePicker = (DateTimePicker)d;

            /* Without deactivating changing SelectedTime would callbase.OnSelectedTimeChanged.
             * This would write too and this would result in duplicate writing.
             * More problematic would be instead that a short amount of time SelectedTime would be as value in TextBox
             */
            dateTimePicker._deactivateWriteValueToTextBox = true; 

            var dt = (DateTime?)e.NewValue;
            if (dt.HasValue)
            {
                dateTimePicker.SelectedTime = dt.Value.TimeOfDay;
                dateTimePicker.OnSelectedDateChanged(new TimePickerBaseSelectionChangedEventArgs<DateTime?>(SelectedDateChangedEvent, (DateTime?)e.OldValue, (DateTime?)e.NewValue));
            }
            else
            {
                dateTimePicker.SetDefaultTimeOfDayValues();
            }

            dateTimePicker._deactivateWriteValueToTextBox = false;

            dateTimePicker.WriteValueToTextBox();
        }

        private DateTime? GetSelectedDateTimeFromGUI()
        {
            // Because Calendar.SelectedDate is bound to this.SelectedDate return this.SelectedDate
            var selectedDate = SelectedDate;

            if (selectedDate != null)
            {
                return selectedDate.Value.Date + GetSelectedTimeFromGUI().GetValueOrDefault();
            }

            return null;
        }

        private void SetDatePartValues()
        {
            var dateTime = GetSelectedDateTimeFromGUI();
            if (dateTime != null)
            {
                DisplayDate = dateTime != DateTime.MinValue ? dateTime : DateTime.Today;
                if (SelectedDate != DisplayDate || (Popup != null && Popup.IsOpen))
                {
                    SelectedDate = DisplayDate;
                }
            }
        }
    }
}