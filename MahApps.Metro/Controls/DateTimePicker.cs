namespace MahApps.Metro.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    ///     Represents a control that allows the user to select a date and a time.
    /// </summary>
    [TemplatePart(Name = ElementButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementHourHand, Type = typeof(UIElement))]
    [TemplatePart(Name = ElementMinuteHand, Type = typeof(UIElement))]
    [TemplatePart(Name = ElementSecondHand, Type = typeof(UIElement))]
    [TemplatePart(Name = ElementSecondPicker, Type = typeof(Selector))]
    [TemplatePart(Name = ElementMinutePicker, Type = typeof(Selector))]
    [TemplatePart(Name = ElementHourPicker, Type = typeof(Selector))]
    [TemplatePart(Name = ElementAmPmSwitcher, Type = typeof(Selector))]
    [DefaultEvent("SelectedDateChanged")]
    public class DateTimePicker : Control
    {
        public static readonly DependencyProperty HandVisibilityProperty = DependencyProperty.Register(
            "HandVisibility",
            typeof(DatePartVisibility),
            typeof(DateTimePicker),
            new PropertyMetadata(DatePartVisibility.All, OnHandVisibilityChanged));
        public static readonly DependencyProperty PickerVisibilityProperty = DependencyProperty.Register(
            "PickerVisibility",
            typeof(DatePartVisibility),
            typeof(DateTimePicker),
            new PropertyMetadata(DatePartVisibility.All, OnPickerVisibilityChanged));
        public static readonly DependencyProperty DisplayDateEndProperty = DatePicker.DisplayDateEndProperty.AddOwner(typeof(DateTimePicker));
        public static readonly DependencyProperty DisplayDateProperty = DatePicker.DisplayDateProperty.AddOwner(typeof(DateTimePicker));
        public static readonly DependencyProperty DisplayDateStartProperty = DatePicker.DisplayDateStartProperty.AddOwner(typeof(DateTimePicker));
        public static readonly DependencyProperty FirstDayOfWeekProperty = DatePicker.FirstDayOfWeekProperty.AddOwner(typeof(DateTimePicker));
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly",
            typeof(bool),
            typeof(DateTimePicker),
            new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty IsTodayHighlightedProperty = DatePicker.IsTodayHighlightedProperty.AddOwner(typeof(DateTimePicker));
        public static readonly DependencyProperty SelectedDateProperty = DatePicker.SelectedDateProperty.AddOwner(typeof(DateTimePicker), new PropertyMetadata(OnSelectedDateChanged));
        public static readonly DependencyProperty SourceHoursProperty = DependencyProperty.Register(
            "SourceHours",
            typeof(ICollection<int>),
            typeof(DateTimePicker),
            new FrameworkPropertyMetadata(Enumerable.Range(0, 24).ToList(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceSourceHours));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation",
            typeof(Orientation),
            typeof(DateTimePicker),
            new PropertyMetadata(Orientation.Horizontal, null, CoerceOrientation));
        public static readonly RoutedEvent SelectedDateChangedEvent = DatePicker.SelectedDateChangedEvent.AddOwner(typeof(DateTimePicker));
        public static readonly DependencyProperty SourceMinutesProperty = DependencyProperty.Register(
            "SourceMinutes",
            typeof(ICollection<int>),
            typeof(DateTimePicker),
            new FrameworkPropertyMetadata(Enumerable.Range(0, 60).ToList(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceSource60));
        public static readonly DependencyProperty SourceSecondsProperty = DependencyProperty.Register(
            "SourceSeconds",
            typeof(ICollection<int>),
            typeof(DateTimePicker),
            new FrameworkPropertyMetadata(Enumerable.Range(0, 60).ToList(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceSource60));
        public static readonly DependencyProperty IsClockVisibleProperty = DependencyProperty.Register(
            "IsClockVisible",
            typeof(bool),
            typeof(DateTimePicker),
            new PropertyMetadata(true, OnClockVisibilityChanged));
        public static readonly DependencyProperty CultureProperty = DependencyProperty.Register(
            "Culture",
            typeof(CultureInfo),
            typeof(DateTimePicker),
            new PropertyMetadata(null, OnCultureChanged));
        private const string ElementAmPmSwitcher = "PART_AmPmSwitcher";
        private const string ElementButton = "PART_Button";
        private const string ElementCalendar = "PART_Calendar";
        private const string ElementHourHand = "PART_HourHand";
        private const string ElementHourPicker = "PART_HourPicker";
        private const string ElementMinuteHand = "PART_MinuteHand";
        private const string ElementMinutePicker = "PART_MinutePicker";
        private const string ElementPopup = "PART_Popup";
        private const string ElementSecondHand = "PART_SecondHand";
        private const string ElementSecondPicker = "PART_SecondPicker";
        private Selector _ampmSwitcher;
        private Button _button;
        private System.Windows.Controls.Calendar _calendar;
        private UIElement _hourHand;
        private Selector _hourInput;
        private UIElement _minuteHand;
        private Selector _minuteInput;
        private Popup _popup;
        private UIElement _secondHand;
        private Selector _secondInput;
        private TimeSpan? _timeOfDay;
        private bool _timeOfDayChanged;

        static DateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(HorizontalAlignment.Right));
            LanguageProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(OnLanguageChanged));
        }

        /// <summary>
        ///     Occurs when the <see cref="SelectedDate" /> property is changed.
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectedDateChanged
        {
            add { AddHandler(SelectedDateChangedEvent, value); }
            remove { RemoveHandler(SelectedDateChangedEvent, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating the culture to be used in string formatting operations.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(null)]
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
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
        ///     Gets or sets a value indicating the visibility of the clock hands in the user interface (UI).
        /// </summary>
        /// <returns>
        ///     The visibility definition of the clock hands. The default is <see cref="DatePartVisibility.All" />.
        /// </returns>
        [Category("Appearance")]
        [DefaultValue(DatePartVisibility.All)]
        public DatePartVisibility HandVisibility
        {
            get { return (DatePartVisibility)GetValue(HandVisibilityProperty); }
            set { SetValue(HandVisibilityProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the clock of this control is visible in the user interface (UI). This is a
        ///     dependency property.
        /// </summary>
        /// <remarks>
        ///     If this value is set to false then <see cref="Orientation" /> is set to
        ///     <see cref="System.Windows.Controls.Orientation.Vertical" />
        /// </remarks>
        /// <returns>
        ///     true if the clock is visible; otherwise, false. The default value is true.
        /// </returns>
        [Category("Appearance")]
        public bool IsClockVisible
        {
            get { return (bool)GetValue(IsClockVisibleProperty); }
            set { SetValue(IsClockVisibleProperty, value); }
        }

        public bool IsMilitaryTime
        {
            get { return string.IsNullOrEmpty(SpecificCultureInfo.DateTimeFormat.AMDesignator); }
        }

        /// <summary>
        ///     Gets or sets a value that enables visible-only mode, in which the contents of the <see cref="DateTimePicker" /> are
        ///     visible but not editable.
        /// </summary>
        /// <returns>
        ///     true if the <see cref="DateTimePicker" /> is read-only; otherwise, false. The default is false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
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
        ///     Gets or sets a value indicating the visibility of the selectable date-time-parts in the user interface (UI).
        /// </summary>
        /// <returns>
        ///     visibility definition of the selectable date-time-parts. The default is <see cref="DatePartVisibility.All" />.
        /// </returns>
        [Category("Appearance")]
        [DefaultValue(DatePartVisibility.All)]
        public DatePartVisibility PickerVisibility
        {
            get { return (DatePartVisibility)GetValue(PickerVisibilityProperty); }
            set { SetValue(PickerVisibilityProperty, value); }
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

        /// <summary>
        ///     Gets or sets a collection used to generate the content for selecting the hours.
        /// </summary>
        /// <returns>
        ///     A collection that is used to generate the content for selecting the hours. The default is a list of interger from 0
        ///     to 23 if <see cref="IsMilitaryTime" /> is false or a list of interger from
        ///     1 to 12 otherwise..
        /// </returns>
        [Category("Common")]
        public ICollection<int> SourceHours
        {
            get { return (ICollection<int>)GetValue(SourceHoursProperty); }
            set { SetValue(SourceHoursProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a collection used to generate the content for selecting the minutes.
        /// </summary>
        /// <returns>
        ///     A collection that is used to generate the content for selecting the minutes. The default is a list of int from
        ///     0 to 59.
        /// </returns>
        [Category("Common")]
        public ICollection<int> SourceMinutes
        {
            get { return (ICollection<int>)GetValue(SourceMinutesProperty); }
            set { SetValue(SourceMinutesProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a collection used to generate the content for selecting the seconds.
        /// </summary>
        /// <returns>
        ///     A collection that is used to generate the content for selecting the minutes. The default is a list of int from
        ///     0 to 59.
        /// </returns>
        [Category("Common")]
        public ICollection<int> SourceSeconds
        {
            get { return (ICollection<int>)GetValue(SourceSecondsProperty); }
            set { SetValue(SourceSecondsProperty, value); }
        }

        private CultureInfo SpecificCultureInfo
        {
            get { return Culture ?? Language.GetSpecificCulture(); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _popup = GetTemplateChild(ElementPopup) as Popup;
            _button = GetTemplateChild(ElementButton) as Button;
            _hourInput = GetTemplateChild(ElementHourPicker) as Selector;
            _minuteInput = GetTemplateChild(ElementMinutePicker) as Selector;
            _secondInput = GetTemplateChild(ElementSecondPicker) as Selector;
            _hourHand = GetTemplateChild(ElementHourHand) as FrameworkElement;
            _ampmSwitcher = GetTemplateChild(ElementAmPmSwitcher) as Selector;
            _minuteHand = GetTemplateChild(ElementMinuteHand) as FrameworkElement;
            _secondHand = GetTemplateChild(ElementSecondHand) as FrameworkElement;
            _calendar = GetTemplateChild(ElementCalendar) as System.Windows.Controls.Calendar;

            if (_calendar != null)
            {
                _calendar.SetBinding(System.Windows.Controls.Calendar.DisplayDateProperty, GetBinding(DisplayDateProperty));
                _calendar.SetBinding(System.Windows.Controls.Calendar.DisplayDateStartProperty, GetBinding(DisplayDateStartProperty));
                _calendar.SetBinding(System.Windows.Controls.Calendar.DisplayDateEndProperty, GetBinding(DisplayDateEndProperty));
                _calendar.SetBinding(System.Windows.Controls.Calendar.FirstDayOfWeekProperty, GetBinding(FirstDayOfWeekProperty));
                _calendar.SetBinding(System.Windows.Controls.Calendar.IsTodayHighlightedProperty, GetBinding(IsTodayHighlightedProperty));
                _calendar.SetBinding(FlowDirectionProperty, GetBinding(FlowDirectionProperty));
                _calendar.SelectedDatesChanged += OnSelectedDatesChanged;
            }

            SetHandVisibility(HandVisibility);
            SetPickerVisibility(PickerVisibility);

            SetDefaultTimeOfDayValues();
            SubscribeToEvents();
            ApplyCulture();
        }

        protected virtual void OnSelectedDateChanged()
        {
            RaiseEvent(new RoutedEventArgs(SelectedDateChangedEvent));
        }

        private static object CoerceOrientation(DependencyObject d, object basevalue)
        {
            if (((DateTimePicker)d).IsClockVisible)
            {
                return basevalue;
            }
            return Orientation.Vertical;
        }

        private static object CoerceSource60(DependencyObject d, object basevalue)
        {
            var list = basevalue as ICollection<int>;
            if (list != null)
            {
                return list.Where(i => i >= 0 && i < 60);
            }
            return null;
        }

        private static object CoerceSourceHours(DependencyObject d, object basevalue)
        {
            var dt = d as DateTimePicker;
            var hourList = basevalue as ICollection<int>;
            if (dt != null && !dt.IsMilitaryTime && hourList != null)
            {
                return new Collection<int>(hourList.Where(i => i > 0 && i <= 12).OrderBy(i => i, new AmPmComparer()).ToList());
            }
            return basevalue;
        }

        private static bool IsValueSelected(Selector selector)
        {
            return selector != null && selector.SelectedItem != null;
        }

        private static void OnClockVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(OrientationProperty);
        }

        private static void OnCultureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimePicker)d).ApplyCulture();
        }

        private static void OnHandVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimePicker)d).SetHandVisibility((DatePartVisibility)e.NewValue);
        }

        private static void OnLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimePicker)d).ApplyCulture();
        }

        private static void OnPickerVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimePicker)d).SetPickerVisibility((DatePartVisibility)e.NewValue);
        }

        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dateTimePicker = (DateTimePicker)d;
            var dt = (DateTime?)e.NewValue;
            dateTimePicker.OnSelectedDateChanged(dt);
        }

        private static void SetDefaultTimeOfDayValue(Selector selector)
        {
            if (selector != null)
            {
                selector.SelectedValue = 0;
            }
        }

        private static void SetVisibility(UIElement partHours, UIElement partMinutes, UIElement partSeconds, DatePartVisibility visibility)
        {
            if (partHours != null)
            {
                partHours.Visibility = visibility.HasFlag(DatePartVisibility.Hour) ? Visibility.Visible : Visibility.Collapsed;
            }
            if (partMinutes != null)
            {
                partMinutes.Visibility = visibility.HasFlag(DatePartVisibility.Minute) ? Visibility.Visible : Visibility.Collapsed;
            }
            if (partSeconds != null)
            {
                partSeconds.Visibility = visibility.HasFlag(DatePartVisibility.Second) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void ApplyCulture()
        {
            if (_ampmSwitcher != null)
            {
                _ampmSwitcher.Visibility = IsMilitaryTime ? Visibility.Collapsed : Visibility.Visible;
                _ampmSwitcher.Items.Clear();
                _ampmSwitcher.Items.Add(SpecificCultureInfo.DateTimeFormat.AMDesignator);
                _ampmSwitcher.Items.Add(SpecificCultureInfo.DateTimeFormat.PMDesignator);
            }
            if (_calendar != null)
            {
                _calendar.Language = XmlLanguage.GetLanguage(SpecificCultureInfo.IetfLanguageTag);
            }
            SetDatePartValues();

            CoerceValue(SourceHoursProperty);
            FirstDayOfWeek = SpecificCultureInfo.DateTimeFormat.FirstDayOfWeek;
        }

        /// <summary>
        ///     Gets the offset from the selected <paramref name="currentHour" /> to use it in <see cref="TimeSpan" /> as hour
        ///     parameter.
        /// </summary>
        /// <param name="currentHour"></param>
        /// <returns>
        ///     An integer representing the offset to add to the hour that is selected in the hour-picker for setting the correct
        ///     <see cref="DateTime.TimeOfDay" />. The offset is determined as follows:
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Condition</term><description>Offset</description>
        ///         </listheader>
        ///         <item>
        ///             <term><see cref="IsMilitaryTime" /> is false</term><description>0</description>
        ///         </item>
        ///         <item>
        ///             <term>Selected hour is between 1 AM and 11 AM</term><description>0</description>
        ///         </item>
        ///         <item>
        ///             <term>Selected hour is 12 AM</term><description>-12h</description>
        ///         </item>
        ///         <item>
        ///             <term>Selected hour is between 12 PM and 11 PM</term><description>+12h</description>
        ///         </item>
        ///     </list>
        /// </returns>
        private int GetAmPmOffset(int currentHour)
        {
            if (!IsMilitaryTime)
            {
                if (currentHour == 12)
                {
                    if (GetSelectedHourDesignator() == SpecificCultureInfo.DateTimeFormat.AMDesignator)
                    {
                        return -12;
                    }
                }
                else if (GetSelectedHourDesignator() == SpecificCultureInfo.DateTimeFormat.PMDesignator)
                {
                    return 12;
                }
            }
            return 0;
        }

        private Binding GetBinding(DependencyProperty property)
        {
            return new Binding(property.Name) { Source = this };
        }

        private DateTime GetCorrectDateTime()
        {
            return SelectedDate.GetValueOrDefault(DateTime.Today).Date + GetTimeOfDay();
        }

        private string GetSelectedHourDesignator()
        {
            if (_ampmSwitcher.SelectedItem == null)
            {
                return null;
            }
            return (string)_ampmSwitcher.SelectedItem;
        }

        private TimeSpan GetTimeOfDay()
        {
            if (_timeOfDayChanged || !_timeOfDay.HasValue)
            {
                if (IsValueSelected(_hourInput) &&
                    IsValueSelected(_minuteInput) &&
                    IsValueSelected(_secondInput))
                {
                    var hours = (int)_hourInput.SelectedItem;
                    var minutes = (int)_minuteInput.SelectedItem;
                    var seconds = (int)_secondInput.SelectedItem;

                    hours += GetAmPmOffset(hours);

                    _timeOfDay = new TimeSpan(hours, minutes, seconds);
                }
                else
                {
                    _timeOfDay = SelectedDate.GetValueOrDefault(DateTime.Today).TimeOfDay;
                }
                _timeOfDayChanged = false;
            }
            return _timeOfDay.Value;
        }

        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            if (_popup != null)
            {
                _popup.IsOpen = true;
            }
        }

        private void OnRangeBaseValueChanged(object sender, SelectionChangedEventArgs e)
        {
            _timeOfDayChanged = true;
            SetDatePartValues();
        }

        private void OnSelectedDateChanged(DateTime? toDate)
        {
            if (toDate.HasValue)
            {
                SetHourPartValues(toDate.Value.TimeOfDay);
            }
            else
            {
                SetDefaultTimeOfDayValues();
            }
            SetDatePartValues();
            OnSelectedDateChanged();
        }

        private void OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                var dt = (DateTime)e.AddedItems[0];
                dt = dt.Add(GetTimeOfDay());
                SelectedDate = dt;
            }
        }

        private void SetDatePartValues()
        {
            var dateTime = GetCorrectDateTime();
            DisplayDate = dateTime != DateTime.MinValue ? dateTime : DateTime.Today;
            if (SelectedDate != null)
            {
                SelectedDate = DisplayDate;
            }
            else if (SelectedDate == null && _popup != null && _popup.IsOpen)
            {
                SelectedDate = DisplayDate;
            }

            SetHourPartValues(dateTime.TimeOfDay);
        }

        private void SetDefaultTimeOfDayValues()
        {
            SetDefaultTimeOfDayValue(_hourInput);
            SetDefaultTimeOfDayValue(_minuteInput);
            SetDefaultTimeOfDayValue(_secondInput);
        }

        private void SetHandVisibility(DatePartVisibility visibility)
        {
            SetVisibility(_hourHand, _minuteHand, _secondHand, visibility);
        }

        private void SetHourPartValues(TimeSpan timeOfDay)
        {
            if (_hourInput != null)
            {
                if (!IsMilitaryTime)
                {
                    _ampmSwitcher.SelectedValue = timeOfDay.Hours < 12 ? SpecificCultureInfo.DateTimeFormat.AMDesignator : SpecificCultureInfo.DateTimeFormat.PMDesignator;
                    if (timeOfDay.Hours == 0 || timeOfDay.Hours == 12)
                    {
                        _hourInput.SelectedValue = 12;
                    }
                    else
                    {
                        _hourInput.SelectedValue = timeOfDay.Hours % 12;
                    }
                }
                else
                {
                    _hourInput.SelectedValue = timeOfDay.Hours;
                }
            }
            if (_minuteInput != null)
            {
                _minuteInput.SelectedValue = timeOfDay.Minutes;
            }
            if (_secondInput != null)
            {
                _secondInput.SelectedValue = timeOfDay.Seconds;
            }
        }

        private void SetPickerVisibility(DatePartVisibility visibility)
        {
            SetVisibility(_hourInput, _minuteInput, _secondInput, visibility);
        }

        private void SubscribeTimePartSelectionChanged(Selector selector)
        {
            if (selector != null)
            {
                selector.SelectionChanged += OnRangeBaseValueChanged;
            }
        }

        private void SubscribeToEvents()
        {
            SubscribeTimePartSelectionChanged(_hourInput);
            SubscribeTimePartSelectionChanged(_minuteInput);
            SubscribeTimePartSelectionChanged(_secondInput);

            if (_button != null)
            {
                _button.Click += OnButtonClicked;
            }
            if (_ampmSwitcher != null)
            {
                _ampmSwitcher.SelectionChanged += OnRangeBaseValueChanged;
            }
        }
    }
}