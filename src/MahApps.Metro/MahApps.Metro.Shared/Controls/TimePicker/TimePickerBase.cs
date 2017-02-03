namespace MahApps.Metro.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Markup;

    /// <summary>
    ///     Represents a base-class for time picking.
    /// </summary>
    [TemplatePart(Name = ElementButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementHourHand, Type = typeof(UIElement))]
    [TemplatePart(Name = ElementHourPicker, Type = typeof(Selector))]
    [TemplatePart(Name = ElementMinuteHand, Type = typeof(UIElement))]
    [TemplatePart(Name = ElementSecondHand, Type = typeof(UIElement))]
    [TemplatePart(Name = ElementSecondPicker, Type = typeof(Selector))]
    [TemplatePart(Name = ElementMinutePicker, Type = typeof(Selector))]
    [TemplatePart(Name = ElementAmPmSwitcher, Type = typeof(Selector))]
    [TemplatePart(Name = ElementTextBox, Type = typeof(DatePickerTextBox))]
    public abstract class TimePickerBase : Control
    {
        public static readonly DependencyProperty SourceHoursProperty = DependencyProperty.Register(
          "SourceHours",
          typeof(IEnumerable<int>),
          typeof(TimePickerBase),
          new FrameworkPropertyMetadata(Enumerable.Range(0, 24), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceSourceHours));

        public static readonly DependencyProperty SourceMinutesProperty = DependencyProperty.Register(
            "SourceMinutes",
            typeof(IEnumerable<int>),
            typeof(TimePickerBase),
            new FrameworkPropertyMetadata(Enumerable.Range(0, 60), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceSource60));

        public static readonly DependencyProperty SourceSecondsProperty = DependencyProperty.Register(
            "SourceSeconds",
            typeof(IEnumerable<int>),
            typeof(TimePickerBase),
            new FrameworkPropertyMetadata(Enumerable.Range(0, 60), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceSource60));

        public static readonly DependencyProperty IsDropDownOpenProperty = DatePicker.IsDropDownOpenProperty.AddOwner(typeof(TimePickerBase), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsClockVisibleProperty = DependencyProperty.Register(
            "IsClockVisible",
            typeof(bool),
            typeof(TimePickerBase),
            new PropertyMetadata(true));

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly",
            typeof(bool),
            typeof(TimePickerBase),
            new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty HandVisibilityProperty = DependencyProperty.Register(
            "HandVisibility",
            typeof(TimePartVisibility),
            typeof(TimePickerBase),
            new PropertyMetadata(TimePartVisibility.All, OnHandVisibilityChanged));

        public static readonly DependencyProperty CultureProperty = DependencyProperty.Register(
            "Culture",
            typeof(CultureInfo),
            typeof(TimePickerBase),
            new PropertyMetadata(null, OnCultureChanged));

        public static readonly DependencyProperty PickerVisibilityProperty = DependencyProperty.Register(
            "PickerVisibility",
            typeof(TimePartVisibility),
            typeof(TimePickerBase),
            new PropertyMetadata(TimePartVisibility.All, OnPickerVisibilityChanged));

        public static readonly RoutedEvent SelectedTimeChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectedTimeChanged", 
            RoutingStrategy.Direct,
            typeof(EventHandler<TimePickerBaseSelectionChangedEventArgs<TimeSpan?>>), 
            typeof(TimePickerBase));

        public static readonly DependencyProperty SelectedTimeProperty = DependencyProperty.Register(
            "SelectedTime",
            typeof(TimeSpan?),
            typeof(TimePickerBase),
            new FrameworkPropertyMetadata(default(TimeSpan?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedTimeChanged, CoerceSelectedTime));

        private const string ElementAmPmSwitcher = "PART_AmPmSwitcher";
        private const string ElementButton = "PART_Button";
        private const string ElementHourHand = "PART_HourHand";
        private const string ElementHourPicker = "PART_HourPicker";
        private const string ElementMinuteHand = "PART_MinuteHand";
        private const string ElementMinutePicker = "PART_MinutePicker";
        private const string ElementPopup = "PART_Popup";
        private const string ElementSecondHand = "PART_SecondHand";
        private const string ElementSecondPicker = "PART_SecondPicker";
        private const string ElementTextBox = "PART_TextBox";

        #region Do not change order of fields inside this region

        /// <summary>
        /// This readonly dependency property is to control whether to show the date-picker (in case of <see cref="DateTimePicker"/>) or hide it (in case of <see cref="TimePicker"/>.
        /// </summary>
        private static readonly DependencyPropertyKey IsDatePickerVisiblePropertyKey = DependencyProperty.RegisterReadOnly(
          "IsDatePickerVisible", typeof(bool), typeof(TimePickerBase), new PropertyMetadata(true));

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess",Justification = "Otherwise we have \"Static member initializer refers to static member below or in other type part\" and thus resulting in having \"null\" as value")]
        public static readonly DependencyProperty IsDatePickerVisibleProperty = IsDatePickerVisiblePropertyKey.DependencyProperty;

        #endregion

        /// <summary>
        /// Represents the time 00:00:00; 12:00:00 AM respectively
        /// </summary>
        private static readonly TimeSpan MinTimeOfDay = TimeSpan.Zero;

        /// <summary>
        /// Represents the time 23:59:59.9999999; 11:59:59.9999999 PM respectively
        /// </summary>
        private static readonly TimeSpan MaxTimeOfDay = TimeSpan.FromDays(1) - TimeSpan.FromTicks(1);

        /// <summary>
        /// This list contains values from 0 to 55 with an interval of 5. It can be used to bind to <see cref="SourceMinutes"/> and <see cref="SourceSeconds"/>.
        /// </summary>
        /// <example>
        /// <code>&lt;MahApps:TimePicker SourceSeconds="{x:Static MahApps:TimePickerBase.IntervalOf5}" /&gt;</code>
        /// <code>&lt;MahApps:DateTimePicker SourceSeconds="{x:Static MahApps:TimePickerBase.IntervalOf5}" /&gt;</code>
        /// </example>
        /// <returns>
        /// Returns a list containing {0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55}.
        /// </returns>
        public static readonly IEnumerable<int> IntervalOf5 = CreateValueList(5);

        /// <summary>
        /// This list contains values from 0 to 50 with an interval of 10. It can be used to bind to <see cref="SourceMinutes"/> and <see cref="SourceSeconds"/>.
        /// </summary>
        /// <example>
        /// <code>&lt;MahApps:TimePicker SourceSeconds="{x:Static MahApps:TimePickerBase.IntervalOf10}" /&gt;</code>
        /// <code>&lt;MahApps:DateTimePicker SourceSeconds="{x:Static MahApps:TimePickerBase.IntervalOf10}" /&gt;</code>
        /// </example>
        /// <returns>
        /// Returns a list containing {0, 10, 20, 30, 40, 50}.
        /// </returns>
        public static readonly IEnumerable<int> IntervalOf10 = CreateValueList(10);

        /// <summary>
        /// This list contains values from 0 to 45 with an interval of 15. It can be used to bind to <see cref="SourceMinutes"/> and <see cref="SourceSeconds"/>.
        /// </summary>
        /// <example>
        /// <code>&lt;MahApps:TimePicker SourceSeconds="{x:Static MahApps:TimePickerBase.IntervalOf15}" /&gt;</code>
        /// <code>&lt;MahApps:DateTimePicker SourceSeconds="{x:Static MahApps:TimePickerBase.IntervalOf15}" /&gt;</code>
        /// </example>
        /// <returns>
        /// Returns a list containing {0, 15, 30, 45}.
        /// </returns>
        public static readonly IEnumerable<int> IntervalOf15 = CreateValueList(15);

        private Selector _ampmSwitcher;
        private Button _button;
        private bool _deactivateRangeBaseEvent;
        private bool _deactivateTextChangedEvent;
        private bool _textInputChanged;
        private UIElement _hourHand;
        private Selector _hourInput;
        private UIElement _minuteHand;
        private Selector _minuteInput;
        private Popup _popup;
        private UIElement _secondHand;
        private Selector _secondInput;
        protected DatePickerTextBox _textBox;

        static TimePickerBase()
        {
            EventManager.RegisterClassHandler(typeof(TimePickerBase), UIElement.GotFocusEvent, new RoutedEventHandler(OnGotFocus));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePickerBase), new FrameworkPropertyMetadata(typeof(TimePickerBase)));
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(TimePickerBase), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            LanguageProperty.OverrideMetadata(typeof(TimePickerBase), new FrameworkPropertyMetadata(OnCultureChanged));
        }

        protected TimePickerBase()
        {
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, this.OutsideCapturedElementHandler);
        }

        /// <summary>
        ///     Occurs when the <see cref="SelectedTime" /> property is changed.
        /// </summary>
        public event EventHandler<TimePickerBaseSelectionChangedEventArgs<TimeSpan?>> SelectedTimeChanged
        {
            add { AddHandler(SelectedTimeChangedEvent, value); }
            remove { RemoveHandler(SelectedTimeChangedEvent, value); }
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
        ///     Gets or sets a value indicating the visibility of the clock hands in the user interface (UI).
        /// </summary>
        /// <returns>
        ///     The visibility definition of the clock hands. The default is <see cref="TimePartVisibility.All" />.
        /// </returns>
        [Category("Appearance")]
        [DefaultValue(TimePartVisibility.All)]
        public TimePartVisibility HandVisibility
        {
            get { return (TimePartVisibility)GetValue(HandVisibilityProperty); }
            set { SetValue(HandVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the date can be selected or not. This property is read-only.
        /// </summary>
        public bool IsDatePickerVisible
        {
            get { return (bool)GetValue(IsDatePickerVisibleProperty); }
            protected set { SetValue(IsDatePickerVisiblePropertyKey, value); }
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

        /// <summary>
        ///     Gets or sets a value indicating whether the drop-down for a <see cref="TimePickerBase"/> box is currently
        ///         open.
        /// </summary>
        /// <returns>true if the drop-down is open; otherwise, false. The default is false.</returns>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the contents of the <see cref="TimePickerBase" /> are not editable.
        /// </summary>
        /// <returns>
        ///     true if the <see cref="TimePickerBase" /> is read-only; otherwise, false. The default is false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating the visibility of the selectable date-time-parts in the user interface (UI).
        /// </summary>
        /// <returns>
        ///     visibility definition of the selectable date-time-parts. The default is <see cref="TimePartVisibility.All" />.
        /// </returns>
        [Category("Appearance")]
        [DefaultValue(TimePartVisibility.All)]
        public TimePartVisibility PickerVisibility
        {
            get { return (TimePartVisibility)GetValue(PickerVisibilityProperty); }
            set { SetValue(PickerVisibilityProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the currently selected time.
        /// </summary>
        /// <returns>
        ///     The time currently selected. The default is null.
        /// </returns>
        public TimeSpan? SelectedTime
        {
            get { return (TimeSpan?)GetValue(SelectedTimeProperty); }
            set { SetValue(SelectedTimeProperty, value); }
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
        public IEnumerable<int> SourceHours
        {
            get { return (IEnumerable<int>)GetValue(SourceHoursProperty); }
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
        public IEnumerable<int> SourceMinutes
        {
            get { return (IEnumerable<int>)GetValue(SourceMinutesProperty); }
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
        public IEnumerable<int> SourceSeconds
        {
            get { return (IEnumerable<int>)GetValue(SourceSecondsProperty); }
            set { SetValue(SourceSecondsProperty, value); }
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="DateTimeFormatInfo.AMDesignator" /> that is specified by the
        ///     <see cref="CultureInfo" />
        ///     set by the <see cref="Culture" /> (<see cref="FrameworkElement.Language" /> if null) has not a value.
        /// </summary>
        public bool IsMilitaryTime
        {
            get { return string.IsNullOrEmpty(SpecificCultureInfo.DateTimeFormat.AMDesignator); }
        }

        protected internal Popup Popup
        {
            get { return _popup; }
        }

        protected CultureInfo SpecificCultureInfo
        {
            get { return Culture ?? Language.GetSpecificCulture(); }
        }

        /// <summary>
        ///     When overridden in a derived class, is invoked whenever application code or internal processes call
        ///     <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UnSubscribeEvents();

            _popup = GetTemplateChild(ElementPopup) as Popup;
            _button = GetTemplateChild(ElementButton) as Button;
            _hourInput = GetTemplateChild(ElementHourPicker) as Selector;
            _minuteInput = GetTemplateChild(ElementMinutePicker) as Selector;
            _secondInput = GetTemplateChild(ElementSecondPicker) as Selector;
            _hourHand = GetTemplateChild(ElementHourHand) as FrameworkElement;
            _ampmSwitcher = GetTemplateChild(ElementAmPmSwitcher) as Selector;
            _minuteHand = GetTemplateChild(ElementMinuteHand) as FrameworkElement;
            _secondHand = GetTemplateChild(ElementSecondHand) as FrameworkElement;
            _textBox = GetTemplateChild(ElementTextBox) as DatePickerTextBox;

            SetHandVisibility(HandVisibility);
            SetPickerVisibility(PickerVisibility);

            SetHourPartValues(SelectedTime.GetValueOrDefault());
            WriteValueToTextBox();

            SetDefaultTimeOfDayValues();
            SubscribeEvents();
            ApplyCulture();
            ApplyBindings();

        }

        protected virtual void ApplyBindings()
        {
            if (Popup != null)
            {
                Popup.SetBinding(Popup.IsOpenProperty, GetBinding(IsDropDownOpenProperty));
            }
        }

        protected virtual void ApplyCulture()
        {
            _deactivateRangeBaseEvent = true;
            if (_ampmSwitcher != null)
            {
                _ampmSwitcher.Items.Clear();
                if (!string.IsNullOrEmpty(SpecificCultureInfo.DateTimeFormat.AMDesignator))
                {
                    _ampmSwitcher.Items.Add(SpecificCultureInfo.DateTimeFormat.AMDesignator);
                }

                if (!string.IsNullOrEmpty(SpecificCultureInfo.DateTimeFormat.PMDesignator))
                {
                    _ampmSwitcher.Items.Add(SpecificCultureInfo.DateTimeFormat.PMDesignator);
                }
            }

            SetAmPmVisibility();

            CoerceValue(SourceHoursProperty);

            if (SelectedTime.HasValue)
            {
                SetHourPartValues(SelectedTime.Value);
            }

            SetDefaultTimeOfDayValues();
            _deactivateRangeBaseEvent = false;
        }

        protected Binding GetBinding(DependencyProperty property)
        {
            return new Binding(property.Name) { Source = this };
        }

        protected virtual string GetValueForTextBox()
        {
            var valueForTextBox = (DateTime.MinValue + SelectedTime)?.ToString(string.Intern(SpecificCultureInfo.DateTimeFormat.LongTimePattern), SpecificCultureInfo);
            return valueForTextBox;
        }

        protected virtual void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            TimeSpan ts;
            if (TimeSpan.TryParse(((DatePickerTextBox)sender).Text, SpecificCultureInfo, out ts))
            {
                SelectedTime = ts;
            }
            else
            {
                if (SelectedTime == null)
                {
                    // if already null, overwrite wrong data in textbox
                    WriteValueToTextBox();
                }
                SelectedTime = null;
            }
        }

        protected virtual void OnRangeBaseValueChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedTime = this.GetSelectedTimeFromGUI();
        }

        protected virtual void OnSelectedTimeChanged(TimePickerBaseSelectionChangedEventArgs<TimeSpan?> e)
        {
            RaiseEvent(e);
        }

        protected void SetDefaultTimeOfDayValues()
        {
            SetDefaultTimeOfDayValue(_hourInput);
            SetDefaultTimeOfDayValue(_minuteInput);
            SetDefaultTimeOfDayValue(_secondInput);
            SetDefaultTimeOfDayValue(_ampmSwitcher);
        }

        protected virtual void SubscribeEvents()
        {
            SubscribeRangeBaseValueChanged(_hourInput, _minuteInput, _secondInput, _ampmSwitcher);

            if (_button != null)
            {
                _button.Click += OnButtonClicked;
            }
            if (_textBox != null)
            {
                _textBox.TextChanged += OnTextChanged;
                _textBox.LostFocus += InternalOnTextBoxLostFocus;
            }
        }

        protected virtual void UnSubscribeEvents()
        {
            UnsubscribeRangeBaseValueChanged(_hourInput, _minuteInput, _secondInput, _ampmSwitcher);

            if (_button != null)
            {
                _button.Click -= OnButtonClicked;
            }
            if (_textBox != null)
            {
                _textBox.TextChanged -= OnTextChanged;
                _textBox.LostFocus -= InternalOnTextBoxLostFocus;
            }
        }

        protected virtual void WriteValueToTextBox()
        {
            if (_textBox != null)
            {
                _deactivateTextChangedEvent = true;
                _textBox.Text = GetValueForTextBox();
                _deactivateTextChangedEvent = false;
            }
        }

        private static IList<int> CreateValueList(int interval)
        {
            return Enumerable.Repeat(interval, 60 / interval)
                             .Select((value, index) => value * index)
                             .ToList();
        }

        private static object CoerceSelectedTime(DependencyObject d, object basevalue)
        {
            var timeOfDay = (TimeSpan?)basevalue;

            if (timeOfDay < MinTimeOfDay)
            {
                return MinTimeOfDay;
            }
            else if (timeOfDay > MaxTimeOfDay)
            {
                return MaxTimeOfDay;
            }

            return timeOfDay;
        }

        private static object CoerceSource60(DependencyObject d, object basevalue)
        {
            var list = basevalue as IEnumerable<int>;
            if (list != null)
            {
                return list.Where(i => i >= 0 && i < 60);
            }

            return Enumerable.Empty<int>();
        }

        private static object CoerceSourceHours(DependencyObject d, object basevalue)
        {
            var timePickerBase = d as TimePickerBase;
            var hourList = basevalue as IEnumerable<int>;
            if (timePickerBase != null && hourList != null)
            {
                if (!timePickerBase.IsMilitaryTime)
                {
                    return hourList.Where(i => i > 0 && i <= 12).OrderBy(i => i, new AmPmComparer());
                }
                return hourList.Where(i => i >= 0 && i < 24);
            }
            return Enumerable.Empty<int>();
        }

        private void InternalOnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (_textInputChanged)
            {
                _textInputChanged = false;

                OnTextBoxLostFocus(sender, e);
            }
        }

        private void InternalOnRangeBaseValueChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_deactivateRangeBaseEvent)
            {
                OnRangeBaseValueChanged(sender, e);
            }
        }

        private static void OnCultureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timePartPickerBase = (TimePickerBase)d;

            if (e.NewValue is XmlLanguage)
            {
                timePartPickerBase.Language = (XmlLanguage)e.NewValue;
            }
            else if (e.NewValue is CultureInfo)
            {
                timePartPickerBase.Language = XmlLanguage.GetLanguage(((CultureInfo)e.NewValue).IetfLanguageTag);
            }
            else
            {
                timePartPickerBase.Language = XmlLanguage.Empty;
            }

            timePartPickerBase.ApplyCulture();
        }

        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);
            // To hide the popup when the user e.g. alt+tabs, monitor for when the window becomes a background window.
            if (!(bool)e.NewValue)
            {
                this.IsDropDownOpen = false;
            }
        }

        private void OutsideCapturedElementHandler(object sender, MouseButtonEventArgs e)
        {
            this.IsDropDownOpen = false;
        }

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            TimePickerBase picker = (TimePickerBase)sender;
            if (!e.Handled && picker.Focusable && (picker._textBox != null))
            {
                if (Equals(e.OriginalSource, picker))
                {
                    // MoveFocus takes a TraversalRequest as its argument.
                    var request = new TraversalRequest((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next);
                    // Gets the element with keyboard focus.
                    var elementWithFocus = Keyboard.FocusedElement as UIElement;
                    // Change keyboard focus.
                    elementWithFocus?.MoveFocus(request);
                    e.Handled = true;
                }
                else if (Equals(e.OriginalSource, picker._textBox))
                {
                    picker._textBox.SelectAll();
                    e.Handled = true;
                }
            }
        }

        private static void OnHandVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimePickerBase)d).SetHandVisibility((TimePartVisibility)e.NewValue);
        }

        private static void OnPickerVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimePickerBase)d).SetPickerVisibility((TimePartVisibility)e.NewValue);
        }

        private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timePartPickerBase = (TimePickerBase)d;

            if (timePartPickerBase._deactivateRangeBaseEvent)
            {
                return;
            }

            timePartPickerBase.SetHourPartValues((e.NewValue as TimeSpan?).GetValueOrDefault(TimeSpan.Zero));

            timePartPickerBase.OnSelectedTimeChanged(new TimePickerBaseSelectionChangedEventArgs<TimeSpan?>(SelectedTimeChangedEvent, (TimeSpan?)e.OldValue, (TimeSpan?)e.NewValue));

            timePartPickerBase.WriteValueToTextBox();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_deactivateTextChangedEvent)
            {
                _textInputChanged = true;
            }
        }

        private static void SetVisibility(UIElement partHours, UIElement partMinutes, UIElement partSeconds, TimePartVisibility visibility)
        {
            if (partHours != null)
            {
                partHours.Visibility = visibility.HasFlag(TimePartVisibility.Hour) ? Visibility.Visible : Visibility.Collapsed;
            }

            if (partMinutes != null)
            {
                partMinutes.Visibility = visibility.HasFlag(TimePartVisibility.Minute) ? Visibility.Visible : Visibility.Collapsed;
            }

            if (partSeconds != null)
            {
                partSeconds.Visibility = visibility.HasFlag(TimePartVisibility.Second) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private static bool IsValueSelected(Selector selector)
        {
            return selector != null && selector.SelectedItem != null;
        }

        private static void SetDefaultTimeOfDayValue(Selector selector)
        {
            if (selector != null)
            {
                if (selector.SelectedValue == null)
                {
                    selector.SelectedIndex = 0;
                }
            }
        }

        protected TimeSpan? GetSelectedTimeFromGUI()
        {
            {
                if (IsValueSelected(_hourInput) &&
                    IsValueSelected(_minuteInput) &&
                    IsValueSelected(_secondInput))
                {
                    var hours = (int)_hourInput.SelectedItem;
                    var minutes = (int)_minuteInput.SelectedItem;
                    var seconds = (int)_secondInput.SelectedItem;

                    hours += GetAmPmOffset(hours);

                    return new TimeSpan(hours, minutes, seconds);
                }

                return SelectedTime;
            }
        }

        /// <summary>
        ///     Gets the offset from the selected <paramref name="currentHour" /> to use it in <see cref="TimeSpan" /> as hour
        ///     parameter.
        /// </summary>
        /// <param name="currentHour">The current hour.</param>
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
                    if (Equals(_ampmSwitcher.SelectedItem, SpecificCultureInfo.DateTimeFormat.AMDesignator))
                    {
                        return -12;
                    }
                }
                else if (Equals(_ampmSwitcher.SelectedItem, SpecificCultureInfo.DateTimeFormat.PMDesignator))
                {
                    return 12;
                }
            }

            return 0;
        }

        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = !IsDropDownOpen;
            if (Popup != null)
            {
                Popup.IsOpen = IsDropDownOpen;
            }
        }

        private void SetAmPmVisibility()
        {
            if (_ampmSwitcher != null)
            {
                if (!PickerVisibility.HasFlag(TimePartVisibility.Hour))
                {
                    _ampmSwitcher.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _ampmSwitcher.Visibility = IsMilitaryTime ? Visibility.Collapsed : Visibility.Visible;
                }
            }
        }

        private void SetHandVisibility(TimePartVisibility visibility)
        {
            SetVisibility(_hourHand, _minuteHand, _secondHand, visibility);
        }

        private void SetHourPartValues(TimeSpan timeOfDay)
        {
            if (this._deactivateRangeBaseEvent)
            {
                return;
            }

            _deactivateRangeBaseEvent = true;
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

            _deactivateRangeBaseEvent = false;
        }

        private void SetPickerVisibility(TimePartVisibility visibility)
        {
            SetVisibility(_hourInput, _minuteInput, _secondInput, visibility);
            SetAmPmVisibility();
        }

        private void SubscribeRangeBaseValueChanged(params Selector[] selectors)
        {
            foreach (var selector in selectors.Where(i => i != null))
            {
                selector.SelectionChanged += InternalOnRangeBaseValueChanged;
            }
        }

        private void UnsubscribeRangeBaseValueChanged(params Selector[] selectors)
        {
            foreach (var selector in selectors.Where(i => i != null))
            {
                selector.SelectionChanged -= InternalOnRangeBaseValueChanged;
            }
        }
    }
}