namespace MahApps.Metro.Controls
{
    #region Using Directives

    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
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
    public class NumericUpDown : RangeBase
    {
        public static readonly RoutedEvent IncrementValueEvent = EventManager.RegisterRoutedEvent("IncrementValue", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericUpDown));
        public static readonly RoutedEvent DecrementValueEvent = EventManager.RegisterRoutedEvent("DecrementValue", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericUpDown));
        public static readonly RoutedEvent DelayChangedEvent = EventManager.RegisterRoutedEvent("DelayChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericUpDown));

        /// <summary>
        ///     Event for "To value has been reached"
        /// </summary>
        public static readonly RoutedEvent MaximumReachedEvent = EventManager.RegisterRoutedEvent("MaximumReached", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericUpDown));

        /// <summary>
        ///     Event for "From value has been reached"
        /// </summary>
        public static readonly RoutedEvent MinimumReachedEvent = EventManager.RegisterRoutedEvent("MinimumReached", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericUpDown));

        /// <summary>
        ///     DependencyProperty for <see cref="Delay" /> property.
        /// </summary>
        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register("Delay",
                                                                                              typeof(int),
                                                                                              typeof(NumericUpDown),
                                                                                              new FrameworkPropertyMetadata(DefaultDelay, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDelayChanged),
                                                                                              ValidateDelay);
        /// <summary>
        ///     DependencyProperty for <see cref="TextAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment",
                                                                                                typeof(TextAlignment),
                                                                                                typeof(NumericUpDown),
                                                                                                new PropertyMetadata(TextAlignment.Right));

        /// <summary>
        ///     DependencyProperty for <see cref="Speedup" /> property.
        /// </summary>
        public static readonly DependencyProperty SpeedupProperty = DependencyProperty.Register("Speedup",
                                                                                                typeof(bool),
                                                                                                typeof(NumericUpDown),
                                                                                                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSpeedupChanged));

        /// <summary>
        ///     DependencyProperty for <see cref="IsReadOnly" /> property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly",
                                                                                                   typeof(bool),
                                                                                                   typeof(NumericUpDown),
                                                                                                   new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsReadOnlyChanged));

        /// <summary>
        ///     DependencyProperty for <see cref="StringFormat" /> property.
        /// </summary>
        public static readonly DependencyProperty StringFormatProperty = DependencyProperty.Register("StringFormat", typeof(string), typeof(NumericUpDown), new FrameworkPropertyMetadata(null, OnStringFormatChanged));

        public static readonly DependencyProperty InterceptArrowKeysProperty = DependencyProperty.Register("InterceptArrowKeys", typeof(bool), typeof(NumericUpDown), new PropertyMetadata(true));

        private const double DefaultInterval = 1d;
        private const int DefaultDelay = 500;
        private const string ElementNumericDown = "PART_NumericDown";
        private const string ElementNumericUp = "PART_NumericUp";
        private const string ElementTextBox = "PART_TextBox";
        private double _internalIntervalMultiplierForCalculation = DefaultInterval;
        private double _intervalValueSinceReset;
        private bool _manualChange;
        private RepeatButton _repeatDown;
        private RepeatButton _repeatUp;
        private TextBox _valueTextBox;

        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
            MinimumProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(double.MinValue));
            SmallChangeProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(DefaultInterval, IntervalChanged));
            LargeChangeProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(100 * DefaultInterval));
            MaximumProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(double.MaxValue));

            VerticalContentAlignmentProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(HorizontalAlignment.Right));
        }

        ~NumericUpDown()
        {
            DataObject.RemovePastingHandler(_valueTextBox, OnValueTextBoxPaste);
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
        ///     Gets or sets the amount of time, in milliseconds, the NumericUpDown waits while the up/down button is pressed before it starts increasing/decreasing the
        ///     <see cref="RangeBase.Value" /> for the specified <see cref="RangeBase.SmallChange" /> . The value must be non-negative.
        /// </summary>
        [DefaultValue(DefaultDelay)]
        [Category("Common")]
        public int Delay
        {
            get { return (int)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the user can use the UP ARROW and DOWN ARROW keys to select values.
        /// </summary>
        public bool InterceptArrowKeys
        {
            get { return (bool)GetValue(InterceptArrowKeysProperty); }
            set { SetValue(InterceptArrowKeysProperty, value); }
        }


        /// <summary>
        ///     Gets or sets a value indicating whether the text can be changed by the use of the up or down buttons only.
        /// </summary>
        [Category("Common")]
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the contents of the text box. 
        /// </summary>
        [Category("Common")]
        [DefaultValue(TextAlignment.Right)]
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the value to be added to or subtracted from remains always
        ///     <see cref="RangeBase.SmallChange" /> or it will change to <see cref="RangeBase.LargeChange" /> and increase this.
        /// </summary>
        [DefaultValue(true)]
        [Category("Common")]
        public bool Speedup
        {
            get { return (bool)GetValue(SpeedupProperty); }
            set { SetValue(SpeedupProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the formatting for the displaying <see cref="RangeBase.Value" />
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
        ///     When overridden in a derived class, is invoked whenever application code or internal processes call
        ///     <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _repeatUp = GetTemplateChild(ElementNumericUp) as RepeatButton;
            _repeatDown = GetTemplateChild(ElementNumericDown) as RepeatButton;

            _valueTextBox = GetTemplateChild(ElementTextBox) as TextBox;

            if (_repeatUp == null || _repeatDown == null || _valueTextBox == null)
            {
                throw new InvalidOperationException(string.Format("You have missed to specify {0}, {1} or {2} in your template", ElementNumericUp, ElementNumericDown, ElementTextBox));
            }

            _valueTextBox.LostFocus += OnTextBoxLostFocus;
            _valueTextBox.PreviewTextInput += OnPreviewTextInput;
            _valueTextBox.IsReadOnly = IsReadOnly;
            _valueTextBox.PreviewKeyDown += OnTextBoxKeyDown;
            DataObject.AddPastingHandler(_valueTextBox, OnValueTextBoxPaste);

            _repeatUp.Click += (o, e) => ChangeValue(true);
            _repeatDown.Click += (o, e) => ChangeValue(false);

            _repeatUp.PreviewMouseUp += (o, e) => ResetInternal();
            _repeatDown.PreviewMouseUp += (o, e) => ResetInternal();
            GotFocus += OnGetFocus;
            OnValueChanged(Value, Value);
        }

        private void OnGetFocus(object sender, RoutedEventArgs e)
        {
            _valueTextBox.Focus();
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.PreviewKeyDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (InterceptArrowKeys)
            {
                if (e.Key == Key.Up)
                {
                    ChangeValue(true);
                    e.Handled = true;
                }
                else if (e.Key == Key.Down)
                {
                    ChangeValue(false);
                    e.Handled = true;
                }
            }
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            _manualChange = true;
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

        protected void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
            if (!string.IsNullOrWhiteSpace(e.Text) && e.Text.Length == 1)
            {
                string text = e.Text;

                if (Char.IsDigit(text[0]))
                {
                    e.Handled = false;
                }
                else if (NumberFormatInfo.CurrentInfo.NumberDecimalSeparator == text)
                {
                    if (!((TextBox)sender).Text.Any(i => i.ToString() == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                    {
                        e.Handled = false;
                    }
                }
                else if (NumberFormatInfo.CurrentInfo.NegativeSign == text || text == NumberFormatInfo.CurrentInfo.PositiveSign)
                {
                    if (((TextBox)sender).SelectionStart == 0)
                    {
                        e.Handled = false;
                    }
                }
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

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);

            if (_repeatUp != null)
            {
                _repeatUp.IsEnabled = Value < newMaximum;
            }
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);

            if (_repeatDown != null)
            {
                _repeatDown.IsEnabled = Value > newMinimum;
            }
        }

        protected virtual void OnSpeedupChanged(bool oldSpeedup, bool newSpeedup) { }

        /// <summary>
        ///     Raises the <see cref="E:System.Windows.Controls.Primitives.RangeBase.ValueChanged" /> routed event.
        /// </summary>
        /// <param name="oldValue">
        ///     Old value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Value" /> property
        /// </param>
        /// <param name="newValue">
        ///     New value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Value" /> property
        /// </param>
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);

            if (_repeatUp != null && !_repeatUp.IsEnabled)
            {
                _repeatUp.IsEnabled = true;
            }

            if (_repeatDown != null && !_repeatDown.IsEnabled)
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
                CultureInfo culture = Language.GetSpecificCulture();
                if (string.IsNullOrEmpty(StringFormat))
                {
                    _valueTextBox.Text = newValue.ToString(culture);
                }
                else
                {
                    _valueTextBox.Text = newValue.ToString(StringFormat, culture);
                }
            }
        }

        private static void IntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDown)d;

            numericUpDown.ResetInternal();
        }

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox valueTextBox = ((NumericUpDown)d)._valueTextBox;

            if (valueTextBox != null)
            {
                valueTextBox.IsReadOnly = (bool)e.NewValue;
            }
        }

        private static void OnDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown ctrl = (NumericUpDown)d;

            ctrl.RaiseChangeDelay();
            ctrl.OnDelayChanged((int)e.OldValue, (int)e.NewValue);
        }

        private static void OnSpeedupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown ctrl = (NumericUpDown)d;

            ctrl.OnSpeedupChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        private static void OnStringFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown nud = (NumericUpDown)d;

            if (nud._valueTextBox != null)
            {
                nud._valueTextBox.Text = nud.Value.ToString((string)e.NewValue);
            }
        }

        private static bool ValidateDelay(object value)
        {
            return Convert.ToInt32(value) >= 0;
        }

        private void RaiseChangeDelay()
        {
            RaiseEvent(new RoutedEventArgs(DelayChangedEvent));
        }

        private void ChangeValue(bool toPositive)
        {
            RaiseEvent(new RoutedEventArgs(toPositive ? IncrementValueEvent : DecrementValueEvent));
            if (Speedup)
            {
                double d = SmallChange * LargeChange;
                if ((_intervalValueSinceReset += SmallChange * _internalIntervalMultiplierForCalculation) > d)
                {
                    LargeChange *= 10;
                    _internalIntervalMultiplierForCalculation *= 10;
                }
            }

            if (toPositive)
            {
                Value = Value + _internalIntervalMultiplierForCalculation;
            }
            else
            {
                Value = Value - _internalIntervalMultiplierForCalculation;
            }
        }

        private bool ValidateText(string text, out double convertedValue)
        {
            return double.TryParse(text, NumberStyles.Any, Language.GetSpecificCulture(), out convertedValue);
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

        private void ResetInternal()
        {
            LargeChange = 100 * SmallChange;
            _internalIntervalMultiplierForCalculation = SmallChange;
            _intervalValueSinceReset = 0;
        }
    }
}