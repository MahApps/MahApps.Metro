namespace MahApps.Metro.Controls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    ///     Represents a Windows spin box (also known as an up-down control) that displays numeric values.
    /// </summary>
    [TemplatePart(Name = ElementNumericUp, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementNumericDown, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementTextBox, Type = typeof(TextBox))]
    public class NumericUpDown : Control
    {
        public static readonly RoutedEvent ValueIncrementedEvent = EventManager.RegisterRoutedEvent("ValueIncremented", RoutingStrategy.Bubble, typeof(NumericUpDownChangedRoutedEventHandler), typeof(NumericUpDown));
        public static readonly RoutedEvent ValueDecrementedEvent = EventManager.RegisterRoutedEvent("ValueDecremented", RoutingStrategy.Bubble, typeof(NumericUpDownChangedRoutedEventHandler), typeof(NumericUpDown));
        public static readonly RoutedEvent DelayChangedEvent = EventManager.RegisterRoutedEvent("DelayChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericUpDown));
        public static readonly RoutedEvent MaximumReachedEvent = EventManager.RegisterRoutedEvent("MaximumReached", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericUpDown));
        public static readonly RoutedEvent MinimumReachedEvent = EventManager.RegisterRoutedEvent("MinimumReached", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericUpDown));
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double?>), typeof(NumericUpDown));
        
        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register(
            "Delay",
            typeof(int),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(DefaultDelay, OnDelayChanged),
            ValidateDelay);

        public static readonly DependencyProperty TextAlignmentProperty = TextBox.TextAlignmentProperty.AddOwner(typeof(NumericUpDown));

        public static readonly DependencyProperty SpeedupProperty = DependencyProperty.Register(
            "Speedup",
            typeof(bool),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(true, OnSpeedupChanged));

        public static readonly DependencyProperty IsReadOnlyProperty = TextBoxBase.IsReadOnlyProperty.AddOwner(
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, IsReadOnlyPropertyChangedCallback));

        private static void IsReadOnlyPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue != null)
            {
                var numUpDown = (NumericUpDown)dependencyObject;
                numUpDown.ToggleReadOnlyMode((bool)e.NewValue | !numUpDown.InterceptManualEnter);
            }
        }

        public static readonly DependencyProperty StringFormatProperty = DependencyProperty.Register(
            "StringFormat",
            typeof(string),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(string.Empty, OnStringFormatChanged, CoerceStringFormat));

        public static readonly DependencyProperty InterceptArrowKeysProperty = DependencyProperty.Register(
            "InterceptArrowKeys",
            typeof(bool),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(true));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(double?),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(default(double?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, CoerceValue));

        public static readonly DependencyProperty ButtonsAlignmentProperty = DependencyProperty.Register(
           "ButtonsAlignment",
           typeof(ButtonsAlignment),
           typeof(NumericUpDown),
           new FrameworkPropertyMetadata(ButtonsAlignment.Right, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            "Minimum",
            typeof(double),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(double.MinValue, OnMinimumChanged));

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum",
            typeof(double),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(double.MaxValue, OnMaximumChanged, CoerceMaximum));

        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register(
            "Interval",
            typeof(double),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(DefaultInterval, IntervalChanged));

        public static readonly DependencyProperty InterceptMouseWheelProperty = DependencyProperty.Register(
            "InterceptMouseWheel", 
            typeof(bool), 
            typeof(NumericUpDown), 
            new FrameworkPropertyMetadata(true));

        public static readonly DependencyProperty TrackMouseWheelWhenMouseOverProperty = DependencyProperty.Register(
            "TrackMouseWheelWhenMouseOver", 
            typeof(bool), 
            typeof(NumericUpDown), 
            new FrameworkPropertyMetadata(default(bool)));

        public static readonly DependencyProperty HideUpDownButtonsProperty = DependencyProperty.Register(
            "HideUpDownButtons",
            typeof(bool),
            typeof(NumericUpDown),
            new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty UpDownButtonsWidthProperty = DependencyProperty.Register(
            "UpDownButtonsWidth",
            typeof(double),
            typeof(NumericUpDown),
            new PropertyMetadata(20d));

        public static readonly DependencyProperty InterceptManualEnterProperty = DependencyProperty.Register(
            "InterceptManualEnter",
            typeof(bool),
            typeof(NumericUpDown),
            new PropertyMetadata(true, InterceptManualEnterChangedCallback));

        private static void InterceptManualEnterChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue != null)
            {
                var numUpDown = (NumericUpDown)dependencyObject;
                numUpDown.ToggleReadOnlyMode(!(bool)e.NewValue | numUpDown.IsReadOnly);
            }
        }

        public static readonly DependencyProperty CultureProperty = DependencyProperty.Register(
            "Culture",
            typeof(CultureInfo),
            typeof(NumericUpDown),
            new PropertyMetadata(null, (o, e) => {
                                            if (e.NewValue != e.OldValue)
                                            {
                                                var numUpDown = (NumericUpDown) o;
                                                numUpDown.OnValueChanged(numUpDown.Value, numUpDown.Value);
                                            }
                                        }));

        public static readonly DependencyProperty SelectAllOnFocusProperty = DependencyProperty.Register(
            "SelectAllOnFocus",
            typeof(bool),
            typeof(NumericUpDown),
            new PropertyMetadata(true));

        public static readonly DependencyProperty HasDecimalsProperty = DependencyProperty.Register(
            "HasDecimals",
            typeof(bool), 
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(true, OnHasDecimalsChanged));
        
        private const double DefaultInterval = 1d;
        private const int DefaultDelay = 500;
        private const string ElementNumericDown = "PART_NumericDown";
        private const string ElementNumericUp = "PART_NumericUp";
        private const string ElementTextBox = "PART_TextBox";
        private const string ScientificNotationChar = "E";
        private const StringComparison StrComp = StringComparison.InvariantCultureIgnoreCase;

        private Tuple<string, string> _removeFromText = new Tuple<string, string>(string.Empty, string.Empty);
        private Lazy<PropertyInfo> _handlesMouseWheelScrolling = new Lazy<PropertyInfo>();
        private double _internalIntervalMultiplierForCalculation = DefaultInterval;
        private double _internalLargeChange = DefaultInterval * 100;
        private double _intervalValueSinceReset;
        private bool _manualChange;
        private RepeatButton _repeatDown;
        private RepeatButton _repeatUp;
        private TextBox _valueTextBox;
        private ScrollViewer _scrollViewer;

        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));

            VerticalContentAlignmentProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(HorizontalAlignment.Right));

            EventManager.RegisterClassHandler(typeof(NumericUpDown), UIElement.GotFocusEvent, new RoutedEventHandler(OnGotFocus));
        }

        public event RoutedPropertyChangedEventHandler<double?> ValueChanged
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

        public event NumericUpDownChangedRoutedEventHandler ValueIncremented
        {
            add { AddHandler(ValueIncrementedEvent, value); }
            remove { RemoveHandler(ValueIncrementedEvent, value); }
        }

        public event NumericUpDownChangedRoutedEventHandler ValueDecremented
        {
            add { AddHandler(ValueDecrementedEvent, value); }
            remove { RemoveHandler(ValueDecrementedEvent, value); }
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
        ///     Gets or sets a value indicating whether the user can use the arrow keys <see cref="Key.Up"/> and <see cref="Key.Down"/> to change values. 
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool InterceptArrowKeys
        {
            get { return (bool)GetValue(InterceptArrowKeysProperty); }
            set { SetValue(InterceptArrowKeysProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the user can use the mouse wheel to change values.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool InterceptMouseWheel
        {
            get { return (bool)GetValue(InterceptMouseWheelProperty); }
            set { SetValue(InterceptMouseWheelProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the control must have the focus in order to change values using the mouse wheel.
        /// <remarks>
        ///     If the value is true then the value changes when the mouse wheel is over the control. If the value is false then the value changes only if the control has the focus. If <see cref="InterceptMouseWheel"/> is set to "false" then this property has no effect.
        /// </remarks>
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool TrackMouseWheelWhenMouseOver
        {
            get { return (bool)GetValue(TrackMouseWheelWhenMouseOverProperty); }
            set { SetValue(TrackMouseWheelWhenMouseOverProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the user can enter text in the control.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool InterceptManualEnter
        {
            get { return (bool)GetValue(InterceptManualEnterProperty); }
            set { SetValue(InterceptManualEnterProperty, value); }
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
        ///     Gets or sets a value indicating whether the +/- button of the control is visible.
        /// </summary>
        /// <remarks>
        ///     If the value is false then the <see cref="Value" /> of the control can be changed only if one of the following cases is satisfied:
        ///     <list type="bullet">
        ///         <item>
        ///             <description><see cref="InterceptArrowKeys" /> is true.</description>
        ///         </item>
        ///         <item>
        ///             <description><see cref="InterceptMouseWheel" /> is true.</description>
        ///         </item>
        ///         <item>
        ///             <description><see cref="InterceptManualEnter" /> is true.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool HideUpDownButtons
        {
            get { return (bool)GetValue(HideUpDownButtonsProperty); }
            set { SetValue(HideUpDownButtonsProperty, value); }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(20d)]
        public double UpDownButtonsWidth
        {
            get { return (double)GetValue(UpDownButtonsWidthProperty); }
            set { SetValue(UpDownButtonsWidthProperty, value); }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(ButtonsAlignment.Right)]
        public Controls.ButtonsAlignment ButtonsAlignment
        {
            get { return (ButtonsAlignment)GetValue(ButtonsAlignmentProperty); }
            set { SetValue(ButtonsAlignmentProperty, value); }
        }

        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(DefaultInterval)]
        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool SelectAllOnFocus
        {
            get { return (bool)GetValue(SelectAllOnFocusProperty); }
            set { SetValue(SelectAllOnFocusProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the text can be changed by the use of the up or down buttons only.
        /// </summary>

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(double.MaxValue)]
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        [Bindable(true)]
        [Category("Common")]
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
            get { return Culture ?? Language.GetSpecificCulture(); }
        }

        /// <summary>
        ///     Indicates if the NumericUpDown should show the decimal separator or not.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(true)]
        public bool HasDecimals
        {
            get { return (bool)GetValue(HasDecimalsProperty); }
            set { SetValue(HasDecimalsProperty, value); }
        }

        /// <summary> 
        ///     Called when this element or any below gets focus.
        /// </summary>
        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            // When NumericUpDown gets logical focus, select the text inside us.
            NumericUpDown numericUpDown = (NumericUpDown)sender;

            // If we're an editable NumericUpDown, forward focus to the TextBox element
            if (!e.Handled)
            {
                if ((numericUpDown.InterceptManualEnter || numericUpDown.IsReadOnly) && numericUpDown._valueTextBox != null)
                {
                    if (e.OriginalSource == numericUpDown)
                    {
                        numericUpDown._valueTextBox.Focus();
                        e.Handled = true;
                    }
                    else if (e.OriginalSource == numericUpDown._valueTextBox && numericUpDown.SelectAllOnFocus)
                    {
                        numericUpDown._valueTextBox.SelectAll();
                    }
                }
            }
            
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

            this.ToggleReadOnlyMode(this.IsReadOnly | !this.InterceptManualEnter);

            _repeatUp.Click += (o, e) => ChangeValueWithSpeedUp(true);
            _repeatDown.Click += (o, e) => ChangeValueWithSpeedUp(false);

            _repeatUp.PreviewMouseUp += (o, e) => ResetInternal();
            _repeatDown.PreviewMouseUp += (o, e) => ResetInternal();
            
            OnValueChanged(Value, Value);

            _scrollViewer = TryFindScrollViewer();
        }

        private void ToggleReadOnlyMode(bool isReadOnly)
        {
            if (_repeatUp == null || _repeatDown == null || _valueTextBox == null)
            {
                return;
            }
            
            if (isReadOnly)
            {
                _valueTextBox.LostFocus -= OnTextBoxLostFocus;
                _valueTextBox.PreviewTextInput -= OnPreviewTextInput;
                _valueTextBox.PreviewKeyDown -= OnTextBoxKeyDown;
                _valueTextBox.TextChanged -= OnTextChanged;
                DataObject.RemovePastingHandler(_valueTextBox, OnValueTextBoxPaste);
            }
            else
            {
                _valueTextBox.LostFocus += OnTextBoxLostFocus;
                _valueTextBox.PreviewTextInput += OnPreviewTextInput;
                _valueTextBox.PreviewKeyDown += OnTextBoxKeyDown;
                _valueTextBox.TextChanged += OnTextChanged;
                DataObject.AddPastingHandler(_valueTextBox, OnValueTextBoxPaste);
            }
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

        protected virtual void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
        }

        protected virtual void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
        }

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
                    ChangeValueWithSpeedUp(true);
                    e.Handled = true;
                    break;
                case Key.Down:
                    ChangeValueWithSpeedUp(false);
                    e.Handled = true;
                    break;
            }

            if (e.Handled)
            {
                _manualChange = false;
                InternalSetText(Value);
            }
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);

            if (e.Key == Key.Down ||
                e.Key == Key.Up)
            {
                ResetInternal();
            }
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);

            if (InterceptMouseWheel && (IsFocused || _valueTextBox.IsFocused || TrackMouseWheelWhenMouseOver))
            {
                bool increment = e.Delta > 0;
                _manualChange = false;
                ChangeValueInternal(increment);
            }

            if (_scrollViewer != null && _handlesMouseWheelScrolling.Value != null)
            {
                if (TrackMouseWheelWhenMouseOver)
                {
                    _handlesMouseWheelScrolling.Value.SetValue(_scrollViewer, true, null);
                }
                else if (InterceptMouseWheel)
                {
                    _handlesMouseWheelScrolling.Value.SetValue(_scrollViewer, _valueTextBox.IsFocused, null);
                }
                else
                {
                    _handlesMouseWheelScrolling.Value.SetValue(_scrollViewer, true, null);
                }
            }
        }
        
        protected void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
            if (string.IsNullOrWhiteSpace(e.Text) ||
                e.Text.Length != 1)
            {
                return;
            }

            string text = e.Text;

            if (char.IsDigit(text[0]))
            {
                e.Handled = false;
            }
            else
            {
                CultureInfo equivalentCulture = SpecificCultureInfo;
                NumberFormatInfo numberFormatInfo = equivalentCulture.NumberFormat;
                TextBox textBox = (TextBox)sender;
                bool allTextSelected = textBox.SelectedText == textBox.Text;

                if (numberFormatInfo.NumberDecimalSeparator == text)
                {
                    if (textBox.Text.All(i => i.ToString(equivalentCulture) != numberFormatInfo.NumberDecimalSeparator) || allTextSelected)
                    {
                        if (HasDecimals)
                        {
                            e.Handled = false;
                        }
                    }
                }
                else
                {
                    if (numberFormatInfo.NegativeSign == text ||
                        text == numberFormatInfo.PositiveSign)
                    {
                        if (textBox.SelectionStart == 0)
                        {
                            // check if text already has a + or - sign
                            if (textBox.Text.Length > 1)
                            {
                                if (allTextSelected || 
                                    (!textBox.Text.StartsWith(numberFormatInfo.NegativeSign, StrComp) &&
                                    !textBox.Text.StartsWith(numberFormatInfo.PositiveSign, StrComp)))
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
                            if (elementBeforeCaret.Equals(ScientificNotationChar, StrComp))
                            {
                                e.Handled = false;
                            }
                        }
                    }
                    else if (text.Equals(ScientificNotationChar, StrComp) &&
                             textBox.SelectionStart > 0 &&
                             !textBox.Text.Any(i => i.ToString(equivalentCulture).Equals(ScientificNotationChar, StrComp)))
                    {
                        e.Handled = false;
                    }
                }
            }
        }

        protected virtual void OnSpeedupChanged(bool oldSpeedup, bool newSpeedup)
        {
        }

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
            if (!_manualChange)
            {
                if (!newValue.HasValue)
                {
                    if (_valueTextBox != null)
                    {
                        _valueTextBox.Text = null;
                    }
                    if (oldValue != newValue)
                    {
                        this.RaiseEvent(new RoutedPropertyChangedEventArgs<double?>(oldValue, newValue, ValueChangedEvent));
                    }
                    return;
                }

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
                    InternalSetText(newValue);
                }
            }

            if (oldValue != newValue)
            {
                this.RaiseEvent(new RoutedPropertyChangedEventArgs<double?>(oldValue, newValue, ValueChangedEvent));
            }
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

            if (numericUpDown.HasDecimals == false)
            {
                val = Math.Truncate(val);
            }
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

            nud.SetRemoveStringFormatFromText((string)e.NewValue);
            if (nud._valueTextBox != null &&
                nud.Value.HasValue)
            {
                nud.InternalSetText(nud.Value);
            }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDown)d;

            numericUpDown.OnValueChanged((double?)e.OldValue, (double?)e.NewValue);
        }

        private static void OnHasDecimalsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDown)d;
            double? oldValue = numericUpDown.Value;

            if ((bool)e.NewValue == false && numericUpDown.Value != null)
            {
                numericUpDown.Value = Math.Truncate(numericUpDown.Value.GetValueOrDefault());
            }
        }

      private static bool ValidateDelay(object value)
        {
            return Convert.ToInt32(value) >= 0;
        }

        private void InternalSetText(double? newValue)
        {
            if (!newValue.HasValue)
            {
                _valueTextBox.Text = null;
                return;
            }

            CultureInfo culture = SpecificCultureInfo;
            if (string.IsNullOrEmpty(StringFormat))
            {
                _valueTextBox.Text = newValue.Value.ToString(culture);
            }
            else if (!StringFormat.Contains("{")) 
            {
                // then we may have a StringFormat of e.g. "N0"
                _valueTextBox.Text = newValue.Value.ToString(StringFormat, culture);
            }
            else
            {
                _valueTextBox.Text = string.Format(culture, StringFormat, newValue.Value);
            }

            if ((bool)GetValue(TextBoxHelper.IsMonitoringProperty))
            {
                SetValue(TextBoxHelper.TextLengthProperty, _valueTextBox.Text.Length);
            }
        }

        private ScrollViewer TryFindScrollViewer()
        {
            _valueTextBox.ApplyTemplate();
            var scrollViewer = _valueTextBox.Template.FindName("PART_ContentHost", _valueTextBox) as ScrollViewer;
            if (scrollViewer != null)
            {
                _handlesMouseWheelScrolling = new Lazy<PropertyInfo>(() => _scrollViewer.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).SingleOrDefault(i => i.Name == "HandlesMouseWheelScrolling"));
            }
            return scrollViewer;
        }

        private void ChangeValueWithSpeedUp(bool toPositive)
        {
            if (IsReadOnly)
            {
                return;
            }
            
            double direction = toPositive ? 1 : -1;
            if (Speedup)
            {
                double d = Interval * _internalLargeChange;
                if ((_intervalValueSinceReset += Interval * _internalIntervalMultiplierForCalculation) > d)
                {
                    _internalLargeChange *= 10;
                    _internalIntervalMultiplierForCalculation *= 10;
                }

                ChangeValueInternal(direction * _internalIntervalMultiplierForCalculation);
            }
            else
            {
                ChangeValueInternal(direction * Interval);
            }
        }

        private void ChangeValueInternal(bool addInterval)
        {
            ChangeValueInternal(addInterval ? Interval : -Interval);
        }

        private void ChangeValueInternal(double interval)
        {
            if (IsReadOnly)
            {
                return;
            }
            
            NumericUpDownChangedRoutedEventArgs routedEvent = interval > 0 ?
                new NumericUpDownChangedRoutedEventArgs(ValueIncrementedEvent, interval) :
                new NumericUpDownChangedRoutedEventArgs(ValueDecrementedEvent, interval);

            RaiseEvent(routedEvent);

            if (!routedEvent.Handled)
            {
                ChangeValueBy(routedEvent.Interval);
                _valueTextBox.CaretIndex = _valueTextBox.Text.Length;
            }
        }

        private void ChangeValueBy(double difference)
        {
            double newValue = Value.GetValueOrDefault() + difference;
            Value = (double)CoerceValue(this, newValue);
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

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            _manualChange = true;
        }

        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (!InterceptManualEnter)
            {
                return;
            }

            TextBox tb = (TextBox)sender;
            _manualChange = false;

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
            if (string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                Value = null;
            }
            else if (_manualChange)
            {
                double convertedValue;
                if (ValidateText(((TextBox)sender).Text, out convertedValue))
                {
                    Value = (double?)CoerceValue(this, convertedValue);
                    e.Handled = true;
                }
            }
        }

        private void OnValueTextBoxPaste(object sender, DataObjectPastingEventArgs e)
        {
            var textBox = (TextBox)sender;
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
            if (IsReadOnly)
            {
                return;
            }
            
            _internalLargeChange = 100 * Interval;
            _internalIntervalMultiplierForCalculation = Interval;
            _intervalValueSinceReset = 0;
        }

        private bool ValidateText(string text, out double convertedValue)
        {
            text = RemoveStringFormatFromText(text);

            return double.TryParse(text, NumberStyles.Any, SpecificCultureInfo, out convertedValue);
        }

        private string RemoveStringFormatFromText(string text)
        {
            // remove special string formattings in order to be able to parse it to double e.g. StringFormat = "{0:N2} pcs." then remove pcs. from text
            if (!string.IsNullOrEmpty(_removeFromText.Item1))
            {
                text = text.Replace(_removeFromText.Item1, string.Empty);
            }
            if (!string.IsNullOrEmpty(_removeFromText.Item2))
            {
                text = text.Replace(_removeFromText.Item2, string.Empty);
            }
            return text;
        }

        private void SetRemoveStringFormatFromText(string stringFormat)
        {
            string tailing = string.Empty;
            string leading = string.Empty;
            string format = stringFormat;
            int indexOf = format.IndexOf("{", StrComp);
            if (indexOf > -1)
            {
                if (indexOf > 0)
                {
                    // remove beginning e.g.
                    // pcs. from "pcs. {0:N2}"
                    tailing = format.Substring(0, indexOf);
                }

                // remove tailing e.g.
                // pcs. from "{0:N2} pcs."
                leading = new string(format.SkipWhile(i => i != '}').Skip(1).ToArray()).Trim();
            }

            _removeFromText = new Tuple<string, string>(tailing, leading);
        }
    }
}
