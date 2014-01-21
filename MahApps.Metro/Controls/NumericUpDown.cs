namespace MahApps.Metro.Controls
{
    #region Using Directives

    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    #endregion

    /// <summary>
    ///     Represents a Windows spin box (also known as an up-down control) that displays numeric values.
    /// </summary>
    [TemplatePart(Name = ElementNumericUp, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementNumericDown, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementTextBox, Type = typeof(TextBox))]
    public class NumericUpDown : Control
    {
        #region Readonly

        public static readonly RoutedEvent IncrementValueEvent = EventManager.RegisterRoutedEvent("IncrementValue", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericUpDown));
        public static readonly RoutedEvent DecrementValueEvent = EventManager.RegisterRoutedEvent("DecrementValue", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericUpDown));
        public static readonly RoutedEvent DelayChangedEvent = EventManager.RegisterRoutedEvent("DelayChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericUpDown));
        public static readonly RoutedEvent MaximumReachedEvent = EventManager.RegisterRoutedEvent("MaximumReached", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericUpDown));
        public static readonly RoutedEvent MinimumReachedEvent = EventManager.RegisterRoutedEvent("MinimumReached", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericUpDown));
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericUpDown));

        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register("Delay",
            typeof(int),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(DefaultDelay, OnDelayChanged),
            ValidateDelay);

        public static readonly DependencyProperty TextAlignmentProperty = TextBox.TextAlignmentProperty.AddOwner(typeof(NumericUpDown));

        public static readonly DependencyProperty SpeedupProperty = DependencyProperty.Register("Speedup",
            typeof(bool),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(true, OnSpeedupChanged));

        public static readonly DependencyProperty IsReadOnlyProperty = TextBoxBase.IsReadOnlyProperty.AddOwner(typeof(NumericUpDown),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty StringFormatProperty = DependencyProperty.Register("StringFormat",
            typeof(string),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(string.Empty, OnStringFormatChanged, CoerceStringFormat));

        public static readonly DependencyProperty InterceptArrowKeysProperty = DependencyProperty.Register("InterceptArrowKeys",
            typeof(bool),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(true));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
            typeof(double?),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(default(double?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, CoerceValue));

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum",
            typeof(double),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(double.MinValue, OnMinimumChanged));

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum",
            typeof(double),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(double.MaxValue, OnMaximumChanged, CoerceMaximum));

        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register("Interval",
            typeof(double),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(DefaultInterval, IntervalChanged));

        private const double DefaultInterval = 1d;
        private const int DefaultDelay = 500;
        private const string ElementNumericDown = "PART_NumericDown";
        private const string ElementNumericUp = "PART_NumericUp";
        private const string ElementTextBox = "PART_TextBox";

        #endregion

        private double _internalIntervalMultiplierForCalculation = DefaultInterval;
        private double _internalLargeChange = DefaultInterval * 100;
        private double _intervalValueSinceReset;
        private bool _manualChange;
        private RepeatButton _repeatDown;
        private RepeatButton _repeatUp;
        private TextBox _valueTextBox;

        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));

            VerticalContentAlignmentProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(HorizontalAlignment.Right));
        }

        ~NumericUpDown()
        {
            if (_valueTextBox != null)
            {
                DataObject.RemovePastingHandler(_valueTextBox, OnValueTextBoxPaste);
            }
        }

        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        /// <summary>
        ///     Event fired from this NumericUpDown when its value has reached the maximum value
        /// </summary>
        public event RoutedEventHandler MaximumReached
        {
            add { AddHandler(MaximumReachedEvent, value); }
            remove { RemoveHandler(MaximumReachedEvent, value); }
        }

        /// <summary>
        ///     Event fired from this NumericUpDown when its value has reached the minimum value
        /// </summary>
        public event RoutedEventHandler MinimumReached
        {
            add { AddHandler(MinimumReachedEvent, value); }
            remove { RemoveHandler(MinimumReachedEvent, value); }
        }

        public event RoutedEventHandler IncrementValue
        {
            add { AddHandler(IncrementValueEvent, value); }
            remove { RemoveHandler(IncrementValueEvent, value); }
        }

        public event RoutedEventHandler DecrementValue
        {
            add { AddHandler(DecrementValueEvent, value); }
            remove { RemoveHandler(DecrementValueEvent, value); }
        }

        public event RoutedEventHandler DelayChanged
        {
            add { AddHandler(DelayChangedEvent, value); }
            remove { RemoveHandler(DelayChangedEvent, value); }
        }

        /// <summary>
        ///     Gets or sets the amount of time, in milliseconds, the NumericUpDown waits while the up/down button is pressed
        ///     before it starts increasing/decreasing the
        ///     <see cref="Value" /> for the specified <see cref="Interval" /> . The value must be
        ///     non-negative.
        /// </summary>
        [Bindable(true)]
        [DefaultValue(DefaultDelay)]
        [Category("Behavior")]
        public int Delay
        {
            get { return (int)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the user can use the UP ARROW and DOWN ARROW keys to select values.
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool InterceptArrowKeys
        {
            get { return (bool)GetValue(InterceptArrowKeysProperty); }
            set { SetValue(InterceptArrowKeysProperty, value); }
        }

        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(DefaultInterval)]
        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the text can be changed by the use of the up or down buttons only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(double.MaxValue)]
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(double.MinValue)]
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the value to be added to or subtracted from <see cref="Value" /> remains
        ///     always
        ///     <see cref="Interval" /> or if it will increase faster after pressing the up/down button/arrow some time.
        /// </summary>
        [Category("Common")]
        [DefaultValue(true)]
        public bool Speedup
        {
            get { return (bool)GetValue(SpeedupProperty); }
            set { SetValue(SpeedupProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the formatting for the displaying <see cref="Value" />
        /// </summary>
        /// <remarks>
        ///     <see href="http://msdn.microsoft.com/en-us/library/dwhawy9k.aspx"></see>
        /// </remarks>
        [Category("Common")]
        public string StringFormat
        {
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the horizontal alignment of the contents of the text box.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(TextAlignment.Right)]
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(null)]
        public double? Value
        {
            get { return (double?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private CultureInfo SpecificCultureInfo
        {
            get { return Language.GetSpecificCulture(); }
        }

        /// <summary>
        ///     When overridden in a derived class, is invoked whenever application code or internal processes call
        ///     <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _repeatUp = GetTemplateChild(ElementNumericUp) as RepeatButton;
            _repeatDown = GetTemplateChild(ElementNumericDown) as RepeatButton;

            _valueTextBox = GetTemplateChild(ElementTextBox) as TextBox;

            if (_repeatUp == null ||
                _repeatDown == null ||
                _valueTextBox == null)
            {
                throw new InvalidOperationException(string.Format("You have missed to specify {0}, {1} or {2} in your template", ElementNumericUp, ElementNumericDown, ElementTextBox));
            }

            _valueTextBox.LostFocus += OnTextBoxLostFocus;
            _valueTextBox.PreviewTextInput += OnPreviewTextInput;
            _valueTextBox.IsReadOnly = IsReadOnly;
            _valueTextBox.PreviewKeyDown += OnTextBoxKeyDown;
            _valueTextBox.TextChanged += OnTextChanged;
            DataObject.AddPastingHandler(_valueTextBox, OnValueTextBoxPaste);

            _repeatUp.Click += (o, e) => ChangeValue(true);
            _repeatDown.Click += (o, e) => ChangeValue(false);

            _repeatUp.PreviewMouseUp += (o, e) => ResetInternal();
            _repeatDown.PreviewMouseUp += (o, e) => ResetInternal();
            GotFocus += OnGetFocus;
            OnValueChanged(Value, Value);
        }

        public void SelectAll()
        {
            if (_valueTextBox != null)
            {
                _valueTextBox.SelectAll();
            }
        }

        protected virtual void OnDelayChanged(int oldDelay, int newDelay)
        {
            if (oldDelay != newDelay)
            {
                if (_repeatDown != null)
                {
                    _repeatDown.Delay = newDelay;
                }

                if (_repeatUp != null)
                {
                    _repeatUp.Delay = newDelay;
                }
            }
        }

        protected virtual void OnMaximumChanged(double oldMaximum, double newMaximum) {}
        protected virtual void OnMinimumChanged(double oldMinimum, double newMinimum) {}

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (!InterceptArrowKeys)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Up:
                    ChangeValue(true);
                    e.Handled = true;
                    break;
                case Key.Down:
                    ChangeValue(false);
                    e.Handled = true;
                    break;
            }
        }

        protected void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            const string scientificNotationChar = "E";
            const StringComparison strComp = StringComparison.InvariantCultureIgnoreCase;

            e.Handled = true;
            if (string.IsNullOrWhiteSpace(e.Text) ||
                e.Text.Length != 1)
            {
                return;
            }

            string text = e.Text;

            if (Char.IsDigit(text[0]))
            {
                e.Handled = false;
            }
            else
            {
                CultureInfo equivalentCulture = SpecificCultureInfo;
                NumberFormatInfo numberFormatInfo = equivalentCulture.NumberFormat;
                TextBox textBox = ((TextBox)sender);

                if (numberFormatInfo.NumberDecimalSeparator == text)
                {
                    if (textBox.Text.All(i => i.ToString(equivalentCulture) != numberFormatInfo.NumberDecimalSeparator))
                    {
                        e.Handled = false;
                    }
                }
                else
                {
                    if (numberFormatInfo.NegativeSign == text ||
                        text == numberFormatInfo.PositiveSign)
                    {
                        if (textBox.SelectionStart == 0)
                        {
                            //check if text already has a + or - sign
                            if (textBox.Text.Length > 1)
                            {
                                if (!textBox.Text.StartsWith(numberFormatInfo.NegativeSign, strComp) &&
                                    !textBox.Text.StartsWith(numberFormatInfo.PositiveSign, strComp))
                                {
                                    e.Handled = false;
                                }
                            }
                            else
                            {
                                e.Handled = false;
                            }
                        }
                        else if (textBox.SelectionStart > 0)
                        {
                            string elementBeforeCaret = textBox.Text.ElementAt(textBox.SelectionStart - 1).ToString(equivalentCulture);
                            if (elementBeforeCaret.Equals(scientificNotationChar, strComp))
                            {
                                e.Handled = false;
                            }
                        }
                    }
                    else if (text.Equals(scientificNotationChar, strComp) &&
                             textBox.SelectionStart > 0 &&
                             !textBox.Text.Any(i => i.ToString(equivalentCulture).Equals(scientificNotationChar, strComp)))
                    {
                        e.Handled = false;
                    }
                }
            }
        }

        protected virtual void OnSpeedupChanged(bool oldSpeedup, bool newSpeedup) {}

        /// <summary>
        ///     Raises the <see cref="ValueChanged" /> routed event.
        /// </summary>
        /// <param name="oldValue">
        ///     Old value of the <see cref="Value" /> property
        /// </param>
        /// <param name="newValue">
        ///     New value of the <see cref="Value" /> property
        /// </param>
        protected virtual void OnValueChanged(double? oldValue, double? newValue)
        {
            if (!newValue.HasValue)
            {
                if (_valueTextBox != null)
                {
                    _valueTextBox.Text = null;
                }
                return;
            }

            if (_repeatUp != null &&
                !_repeatUp.IsEnabled)
            {
                _repeatUp.IsEnabled = true;
            }

            if (_repeatDown != null &&
                !_repeatDown.IsEnabled)
            {
                _repeatDown.IsEnabled = true;
            }

            if (newValue <= Minimum)
            {
                if (_repeatDown != null)
                {
                    _repeatDown.IsEnabled = false;
                }

                ResetInternal();

                if (IsLoaded)
                {
                    RaiseEvent(new RoutedEventArgs(MinimumReachedEvent));
                }
            }

            if (newValue >= Maximum)
            {
                if (_repeatUp != null)
                {
                    _repeatUp.IsEnabled = false;
                }

                ResetInternal();
                if (IsLoaded)
                {
                    RaiseEvent(new RoutedEventArgs(MaximumReachedEvent));
                }
            }

            if (_valueTextBox != null)
            {
                CultureInfo culture = SpecificCultureInfo;
                if (string.IsNullOrEmpty(StringFormat))
                {
                    _valueTextBox.Text = newValue.Value.ToString(culture);
                }
                else
                {
                    _valueTextBox.Text = string.Format(culture, StringFormat, newValue.Value);
                }

                if ((bool)GetValue(TextboxHelper.IsMonitoringProperty))
                {
                    SetValue(TextboxHelper.TextLengthProperty, _valueTextBox.Text.Length);
                }
            }

            var eventArgs = new RoutedPropertyChangedEventArgs<double?>(oldValue, newValue) { RoutedEvent = RangeBase.ValueChangedEvent };
            RaiseEvent(eventArgs);
        }

        private static object CoerceMaximum(DependencyObject d, object value)
        {
            double minimum = ((NumericUpDown)d).Minimum;
            double val = (double)value;
            return val < minimum ? minimum : val;
        }

        private static object CoerceStringFormat(DependencyObject d, object basevalue)
        {
            return basevalue ?? string.Empty;
        }

        private static object CoerceValue(DependencyObject d, object value)
        {
            if (value == null)
            {
                return null;
            }

            var numericUpDown = (NumericUpDown)d;
            double val = ((double?)value).Value;

            if (val < numericUpDown.Minimum)
            {
                return numericUpDown.Minimum;
            }
            if (val > numericUpDown.Maximum)
            {
                return numericUpDown.Maximum;
            }
            return val;
        }

        private static void IntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDown)d;

            numericUpDown.ResetInternal();
        }

        private static void OnDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown ctrl = (NumericUpDown)d;

            ctrl.RaiseChangeDelay();
            ctrl.OnDelayChanged((int)e.OldValue, (int)e.NewValue);
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDown)d;

            numericUpDown.CoerceValue(ValueProperty);
            numericUpDown.OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
            numericUpDown.EnableDisableUpDown();
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDown)d;

            numericUpDown.CoerceValue(ValueProperty);
            numericUpDown.CoerceValue(MaximumProperty);
            numericUpDown.OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
            numericUpDown.EnableDisableUpDown();
        }

        private static void OnSpeedupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown ctrl = (NumericUpDown)d;

            ctrl.OnSpeedupChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        private static void OnStringFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown nud = (NumericUpDown)d;

            if (nud._valueTextBox != null &&
                nud.Value.HasValue)
            {
                nud._valueTextBox.Text = nud.Value.Value.ToString((string)e.NewValue);
            }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDown)d;

            numericUpDown.OnValueChanged((double?)e.OldValue, (double?)e.NewValue);
        }

        private static bool ValidateDelay(object value)
        {
            return Convert.ToInt32(value) >= 0;
        }

        private void ChangeValue(bool toPositive)
        {
            RaiseEvent(new RoutedEventArgs(toPositive ? IncrementValueEvent : DecrementValueEvent));

            if (Speedup)
            {
                double d = Interval * _internalLargeChange;
                if ((_intervalValueSinceReset += Interval * _internalIntervalMultiplierForCalculation) > d)
                {
                    _internalLargeChange *= 10;
                    _internalIntervalMultiplierForCalculation *= 10;
                }
            }

            if (toPositive)
            {
                Value = Value.GetValueOrDefault() + _internalIntervalMultiplierForCalculation;
            }
            else
            {
                Value = Value.GetValueOrDefault() - _internalIntervalMultiplierForCalculation;
            }

            _valueTextBox.CaretIndex = _valueTextBox.Text.Length;
        }

        private void EnableDisableDown()
        {
            if (_repeatDown != null)
            {
                _repeatDown.IsEnabled = Value > Minimum;
            }
        }

        private void EnableDisableUp()
        {
            if (_repeatUp != null)
            {
                _repeatUp.IsEnabled = Value < Maximum;
            }
        }

        private void EnableDisableUpDown()
        {
            EnableDisableUp();
            EnableDisableDown();
        }

        private void OnGetFocus(object sender, RoutedEventArgs e)
        {
            _valueTextBox.Focus();
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            _manualChange = true;
        }

        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!_manualChange)
            {
                return;
            }

            double convertedValue;
            if (ValidateText(tb.Text, out convertedValue))
            {
                if (Value == convertedValue)
                {
                    OnValueChanged(Value, Value);
                }
                if (convertedValue > Maximum)
                {
                    if (Value == Maximum)
                    {
                        OnValueChanged(Value, Value);
                    }
                    else
                    {
                        SetValue(ValueProperty, Maximum);
                    }
                }
                else if (convertedValue < Minimum)
                {
                    if (Value == Minimum)
                    {
                        OnValueChanged(Value, Value);
                    }
                    else
                    {
                        SetValue(ValueProperty, Minimum);
                    }
                }
                else
                {
                    SetValue(ValueProperty, convertedValue);
                }
            }
            else
            {
                OnValueChanged(Value, Value);
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(((TextBox)sender).Text))
            {
                Value = null;
            }
        }

        private void OnValueTextBoxPaste(object sender, DataObjectPastingEventArgs e)
        {
            var textBox = ((TextBox)sender);
            string textPresent = textBox.Text;

            var isText = e.SourceDataObject.GetDataPresent(DataFormats.Text, true);
            if (!isText)
            {
                return;
            }

            var text = e.SourceDataObject.GetData(DataFormats.Text) as string;

            string newText = string.Concat(textPresent.Substring(0, textBox.SelectionStart), text, textPresent.Substring(textBox.SelectionStart));
            double number;
            if (!ValidateText(newText, out number))
            {
                e.CancelCommand();
            }
        }

        private void RaiseChangeDelay()
        {
            RaiseEvent(new RoutedEventArgs(DelayChangedEvent));
        }

        private void ResetInternal()
        {
            _internalLargeChange = 100 * Interval;
            _internalIntervalMultiplierForCalculation = Interval;
            _intervalValueSinceReset = 0;
        }

        private bool ValidateText(string text, out double convertedValue)
        {
            return double.TryParse(text, NumberStyles.Any, SpecificCultureInfo, out convertedValue);
        }
    }
}