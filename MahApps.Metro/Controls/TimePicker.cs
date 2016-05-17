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

    [TemplatePart(Name = ElementButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementHourHand, Type = typeof(UIElement))]
    [TemplatePart(Name = ElementHourPicker, Type = typeof(Selector))]
    [TemplatePart(Name = ElementMinuteHand, Type = typeof(UIElement))]
    [TemplatePart(Name = ElementSecondHand, Type = typeof(UIElement))]
    [TemplatePart(Name = ElementSecondPicker, Type = typeof(Selector))]
    [TemplatePart(Name = ElementMinutePicker, Type = typeof(Selector))]
    [TemplatePart(Name = ElementAmPmSwitcher, Type = typeof(Selector))]
    public abstract class TimePartPickerBase<T> : Control
  {
    static TimePartPickerBase()
    {

            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePartPickerBase<T>), new FrameworkPropertyMetadata(typeof(TimePartPickerBase<T>)));
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(TimePartPickerBase<T>), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(TimePartPickerBase<T>), new FrameworkPropertyMetadata(HorizontalAlignment.Right));
            LanguageProperty.OverrideMetadata(typeof(TimePartPickerBase<T>), new FrameworkPropertyMetadata(OnCultureChanged));
    }


        public static readonly DependencyProperty SelectedTimeProperty = DependencyProperty.Register(
            "SelectedTime", typeof(TimeSpan?), typeof(TimePartPickerBase<T>), new PropertyMetadata(default(TimeSpan?)));

        public TimeSpan? SelectedTime
        {
            get { return (TimeSpan?)GetValue(SelectedTimeProperty); }
            set { SetValue(SelectedTimeProperty, value); }
        }



        protected CultureInfo SpecificCultureInfo
        {
            get { return Culture ?? this.Language.GetSpecificCulture(); }
        }

        private static void OnCultureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimePartPickerBase<T>)d).ApplyCulture();
        }


        private bool _deactivateRangeBaseEvent;
        public static readonly DependencyProperty CultureProperty = DependencyProperty.Register(
            "Culture",
            typeof(CultureInfo),
            typeof(TimePartPickerBase<T>),
            new PropertyMetadata(null, TimePartPickerBase<DateTime?>.OnCultureChanged));


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

        private Selector _ampmSwitcher;


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
            //if (SelectedDate.HasValue)
            //{
            //    SetHourPartValues(SelectedDate.Value.TimeOfDay);
            //}
            if (SelectedTime.HasValue)
            {
                SetHourPartValues(SelectedTime.Value);
            }
            SetDefaultTimeOfDayValues();
            _deactivateRangeBaseEvent = false;
        }

        protected void SetDefaultTimeOfDayValues()
        {
            SetDefaultTimeOfDayValue(_hourInput);
            SetDefaultTimeOfDayValue(_minuteInput);
            SetDefaultTimeOfDayValue(_secondInput);
            SetDefaultTimeOfDayValue(_ampmSwitcher);
        }
        private void SetHandVisibility(TimePartVisibility visibility)
        {
            SetVisibility(_hourHand, _minuteHand, _secondHand, visibility);
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

        /// <summary>
        /// Gets a value that indicates if the <see cref="DateTimeFormatInfo.AMDesignator"/> that is specified by the <see cref="CultureInfo"/> 
        /// set by the <see cref="Culture"/> (<see cref="FrameworkElement.Language"/> if null) has not a value.
        /// </summary>
        public bool IsMilitaryTime
        {
            get { return string.IsNullOrEmpty(this.SpecificCultureInfo.DateTimeFormat.AMDesignator); }
        }

        private UIElement _hourHand;
        private UIElement _minuteHand;
        private UIElement _secondHand;
        private Selector _hourInput;
        private Selector _minuteInput;
        private Selector _secondInput;
        protected void SetHourPartValues(TimeSpan timeOfDay)
        {
            _deactivateRangeBaseEvent = true;
            if (_hourInput != null)
            {
                if (!IsMilitaryTime)
                {
                    _ampmSwitcher.SelectedValue = timeOfDay.Hours < 12 ? this.SpecificCultureInfo.DateTimeFormat.AMDesignator : this.SpecificCultureInfo.DateTimeFormat.PMDesignator;
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

        private string GetSelectedHourDesignator()
        {
            if (_ampmSwitcher.SelectedItem == null)
            {
                return null;
            }
            return (string)_ampmSwitcher.SelectedItem;
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


        private static void OnHandVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimePartPickerBase<T>)d).SetHandVisibility((TimePartVisibility)e.NewValue);
        }
        private static void OnPickerVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TimePartPickerBase<T>)d).SetPickerVisibility((TimePartVisibility)e.NewValue);
        }

        public static readonly DependencyProperty SourceHoursProperty = DependencyProperty.Register(
          "SourceHours",
          typeof(ICollection<int>),
          typeof(TimePartPickerBase<T>),
          new FrameworkPropertyMetadata(Enumerable.Range(0, 24).ToList(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceSourceHours));
        public static readonly DependencyProperty SourceMinutesProperty = DependencyProperty.Register(
            "SourceMinutes",
            typeof(ICollection<int>),
            typeof(TimePartPickerBase<T>),
            new FrameworkPropertyMetadata(Enumerable.Range(0, 60).ToList(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceSource60));
        public static readonly DependencyProperty SourceSecondsProperty = DependencyProperty.Register(
            "SourceSeconds",
            typeof(ICollection<int>),
            typeof(TimePartPickerBase<T>),
            new FrameworkPropertyMetadata(Enumerable.Range(0, 60).ToList(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceSource60));


        private static object CoerceSource60(DependencyObject d, object basevalue)
        {
            var list = basevalue as ICollection<int>;
            if (list != null)
            {
                return list.Where(i => i >= 0 && i < 60);
            }
            return null;
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
        ///     Occurs when the drop-down date-time-picker is closed.
        /// </summary>
        public event RoutedEventHandler Closed;

        /// <summary>
        ///     Occurs when the drop-down date-time-picker is opened.
        /// </summary>
        public event RoutedEventHandler Opened;

        protected virtual void OnClosed()
        {
            RoutedEventHandler handler = Closed;
            if (null != handler)
            {
                handler(this, new RoutedEventArgs());
            }
        }

        protected virtual void OnOpened()
        {
            RoutedEventHandler handler = Opened;
            if (null != handler)
            {
                handler(this, new RoutedEventArgs());
            }
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


        public static readonly DependencyProperty IsDropDownOpenProperty = DatePicker.IsDropDownOpenProperty.AddOwner(typeof(TimePartPickerBase<T>), new PropertyMetadata(default(bool), OnIsDropDownOpenChanged));
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly",
            typeof(bool),
            typeof(TimePartPickerBase<T>),
            new PropertyMetadata(default(bool)));



        private static bool IsValueSelected(Selector selector)
        {
            return selector != null && selector.SelectedItem != null;
        }
        protected virtual void UnSubscribeEvents()
        {
            UnsubscribeRangeBaseValueChanged(_hourInput, _minuteInput, _secondInput, _ampmSwitcher);

            if (_button != null)
            {
                _button.Click -= OnButtonClicked;
            }
        }

        protected virtual void SubscribeEvents()
        {
            SubscribeRangeBaseValueChanged(_hourInput, _minuteInput, _secondInput, _ampmSwitcher);

            if (_button != null)
            {
                _button.Click += OnButtonClicked;
            }
        }


        private const string ElementHourHand = "PART_HourHand";
        private const string ElementHourPicker = "PART_HourPicker";
        private const string ElementMinuteHand = "PART_MinuteHand";
        private const string ElementMinutePicker = "PART_MinutePicker";
        private const string ElementPopup = "PART_Popup";
        private const string ElementSecondHand = "PART_SecondHand";
        private const string ElementSecondPicker = "PART_SecondPicker";



        private const string ElementAmPmSwitcher = "PART_AmPmSwitcher";
        private const string ElementButton = "PART_Button";

        private Button _button;
        protected internal Popup _popup;

        private void UnsubscribeRangeBaseValueChanged(params Selector[] selectors)
        {
            foreach (var selector in selectors.Where(i => i != null))
            {
                selector.SelectionChanged -= OnRangeBaseValueChanged;
            }
        }


        private void SubscribeRangeBaseValueChanged(params Selector[] selectors)
        {
            foreach (var selector in selectors.Where(i => i != null))
            {
                selector.SelectionChanged += OnRangeBaseValueChanged;
            }
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
        /// <summary>When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.</summary>
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


            SetHandVisibility(HandVisibility);
            SetPickerVisibility(PickerVisibility);

            SetDefaultTimeOfDayValues();
            SubscribeEvents();
            ApplyCulture();
            ApplyBindings();

        }


        private TimeSpan? _timeOfDay;
        private bool _timeOfDayChanged;



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
                    if (GetSelectedHourDesignator() == this.SpecificCultureInfo.DateTimeFormat.AMDesignator)
                    {
                        return -12;
                    }
                }
                else if (GetSelectedHourDesignator() == this.SpecificCultureInfo.DateTimeFormat.PMDesignator)
                {
                    return 12;
                }
            }
            return 0;
        }

        protected TimeSpan GetTimeOfDay()
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
                    //_timeOfDay = SelectedDate.GetValueOrDefault(DateTime.Today).TimeOfDay;
                    _timeOfDay = SelectedTime;
                }
                _timeOfDayChanged = false;
            }
            return _timeOfDay.Value;
        }



        /// <summary>
        ///     Gets or sets a value that indicates whether the drop-down for a <see cref="TimePartPickerBase<T>" /> box is currently
        ///     open.
        /// </summary>
        /// <returns>true if the drop-down is open; otherwise, false. The default is false.</returns>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = !IsDropDownOpen;
            if (_popup != null)
            {
                _popup.IsOpen = IsDropDownOpen;
            }
        }


        protected Binding GetBinding(DependencyProperty property)
        {
            return new Binding(property.Name) { Source = this };
        }

        protected virtual void ApplyBindings()
        {
            if (_popup != null)
            {
                _popup.SetBinding(Popup.IsOpenProperty, GetBinding(IsDropDownOpenProperty));
            }
        }

        protected virtual void OnRangeBaseValueChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_deactivateRangeBaseEvent)
            {
                return;
            }
            _timeOfDayChanged = true;
        }
        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                ((TimePartPickerBase<T>)d).OnOpened();
            }
            else
            {
                ((TimePartPickerBase<T>)d).OnClosed();
            }
        }

        private static object CoerceSourceHours(DependencyObject d, object basevalue)
        {
            var dt = d as TimePartPickerBase<T>;
            var hourList = basevalue as ICollection<int>;
            if (dt != null && !dt.IsMilitaryTime && hourList != null)
            {
                return new Collection<int>(hourList.Where(i => i > 0 && i <= 12).OrderBy(i => i, new AmPmComparer()).ToList());
            }
            return basevalue;
        }

        public static readonly DependencyProperty HandVisibilityProperty = DependencyProperty.Register(
    "HandVisibility",
    typeof(TimePartVisibility),
    typeof(TimePartPickerBase<T>),
    new PropertyMetadata(TimePartVisibility.All, OnHandVisibilityChanged));

        public static readonly DependencyProperty PickerVisibilityProperty = DependencyProperty.Register(
            "PickerVisibility",
            typeof(TimePartVisibility),
            typeof(TimePartPickerBase<T>),
            new PropertyMetadata(TimePartVisibility.All, OnPickerVisibilityChanged));

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
    }


  public class TimePicker : TimePartPickerBase<TimeSpan?>
  {
      static TimePicker()
      {

            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePicker), new FrameworkPropertyMetadata(typeof(TimePicker)));
        }
    }
}