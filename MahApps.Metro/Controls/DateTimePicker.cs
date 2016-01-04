﻿namespace MahApps.Metro.Controls
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

    [TemplatePart(Name = ElementButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementHourHand, Type = typeof(UIElement))]
    [TemplatePart(Name = ElementMinuteHand, Type = typeof(UIElement))]
    [TemplatePart(Name = ElementSecondHand, Type = typeof(UIElement))]
    [TemplatePart(Name = ElementSecondPicker, Type = typeof(Selector))]
    [TemplatePart(Name = ElementMinutePicker, Type = typeof(Selector))]
    [TemplatePart(Name = ElementHourPicker, Type = typeof(Selector))]
    [TemplatePart(Name = ElementAmPmSwitcher, Type = typeof(Selector))]
    public class DateTimePicker : System.Windows.Controls.Calendar
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
        public static readonly DependencyProperty SourceHoursProperty = DependencyProperty.Register(
            "SourceHours",
            typeof(ICollection<int>),
            typeof(DateTimePicker),
            new FrameworkPropertyMetadata(Enumerable.Range(0, 24).ToList(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceSourceHours));
        public static readonly DependencyProperty SourceMinutesProperty = DependencyProperty.Register(
            "SourceMinutes",
            typeof(ICollection<int>),
            typeof(DateTimePicker),
            new FrameworkPropertyMetadata(Enumerable.Range(0, 60).ToList(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty SourceSecondsProperty = DependencyProperty.Register(
            "SourceSeconds",
            typeof(ICollection<int>),
            typeof(DateTimePicker),
            new FrameworkPropertyMetadata(Enumerable.Range(0, 60).ToList(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private const string ElementAmPmSwitcher = "PART_AmPmSwitcher";
        private const string ElementButton = "PART_Button";
        private const string ElementHourHand = "PART_HourHand";
        private const string ElementHourPicker = "PART_HourPicker";
        private const string ElementMinuteHand = "PART_MinuteHand";
        private const string ElementMinutePicker = "PART_MinutePicker";
        private const string ElementPopup = "PART_Popup";
        private const string ElementSecondHand = "PART_SecondHand";
        private const string ElementSecondPicker = "PART_SecondPicker";
        private Selector _ampmSwitcher;
        private Button _button;
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
        }

        /// <summary>
        ///     Gets or sets the visible clock hands.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(DatePartVisibility.All)]
        public DatePartVisibility HandVisibility
        {
            get { return (DatePartVisibility)GetValue(HandVisibilityProperty); }
            set { SetValue(HandVisibilityProperty, value); }
        }

        public bool IsMilitaryTime
        {
            get { return string.IsNullOrEmpty(SpecificCultureInfo.DateTimeFormat.AMDesignator); }
        }

        /// <summary>
        ///     Gets or sets the visible time-part-selectors.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(DatePartVisibility.All)]
        public DatePartVisibility PickerVisibility
        {
            get { return (DatePartVisibility)GetValue(PickerVisibilityProperty); }
            set { SetValue(PickerVisibilityProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the source that is available for selecting the hours.
        /// </summary>
        /// <remarks>
        ///     The default is a list of interger from 0 to 23 if <see cref="IsMilitaryTime" /> is false or a list of interger from 1 to 12 otherwise.
        /// </remarks>
        [Category("Common")]
        public ICollection<int> SourceHours
        {
            get { return (ICollection<int>)GetValue(SourceHoursProperty); }
            set { SetValue(SourceHoursProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the source that is available for selecting the minutes.
        /// </summary>
        /// <remarks>
        ///     The default is a list of int from 0 to 59.
        /// </remarks>
        [Category("Common")]
        public ICollection<int> SourceMinutes
        {
            get { return (ICollection<int>)GetValue(SourceMinutesProperty); }
            set { SetValue(SourceMinutesProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the source that is available for selecting the seconds.
        /// </summary>
        /// <remarks>
        ///     The default is a list of int from 0 to 59.
        /// </remarks>
        [Category("Common")]
        public ICollection<int> SourceSeconds
        {
            get { return (ICollection<int>)GetValue(SourceSecondsProperty); }
            set { SetValue(SourceSecondsProperty, value); }
        }

        private CultureInfo SpecificCultureInfo
        {
            get { return Language.GetSpecificCulture(); }
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

            if (_ampmSwitcher != null)
            {
                _ampmSwitcher.Visibility = IsMilitaryTime ? Visibility.Collapsed : Visibility.Visible;
                _ampmSwitcher.Items.Add(SpecificCultureInfo.DateTimeFormat.AMDesignator);
                _ampmSwitcher.Items.Add(SpecificCultureInfo.DateTimeFormat.PMDesignator);
            }

            SetHandVisibility(HandVisibility);
            SetPickerVisibility(PickerVisibility);

            SetDefaultTimeOfDayValues();
            SubscribeToEvents();

            SetDatePartValues();
        }

        protected override void OnSelectedDatesChanged(SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                var dt = ((DateTime)e.AddedItems[0]);
                SetHourPartValues(dt.TimeOfDay);
            }
            SetDatePartValues();
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

        private static void OnHandVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimePicker)d)?.SetHandVisibility((DatePartVisibility)e.NewValue);
        }

        private static void OnPickerVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimePicker)d)?.SetPickerVisibility((DatePartVisibility)e.NewValue);
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

        private void SetDatePartValues()
        {
            var dateTime = GetCorrectDateTime();
            DisplayDate = dateTime != DateTime.MinValue ? dateTime : DateTime.Today;
            if (SelectedDate != null)
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