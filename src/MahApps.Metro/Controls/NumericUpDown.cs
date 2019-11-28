namespace MahApps.Metro.Controls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    ///     Represents a Windows spin box (also known as an up-down control) that displays numeric values.
    /// </summary>
    [TemplatePart(Name = PART_NumericUp, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PART_NumericDown, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
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

        public static readonly DependencyProperty UpDownButtonsFocusableProperty = DependencyProperty.Register(
            "UpDownButtonsFocusable",
            typeof(bool),
            typeof(NumericUpDown),
            new PropertyMetadata(true));

        public static readonly DependencyProperty InterceptManualEnterProperty = DependencyProperty.Register(
            "InterceptManualEnter",
            typeof(bool),
            typeof(NumericUpDown),
            new PropertyMetadata(true, InterceptManualEnterChangedCallback));

        public static readonly DependencyProperty ChangeValueOnTextChangedProperty = DependencyProperty.Register(
            "ChangeValueOnTextChanged",
            typeof(bool),
            typeof(NumericUpDown),
            new PropertyMetadata(true));

        public static readonly DependencyProperty CultureProperty = DependencyProperty.Register(
            "Culture",
            typeof(CultureInfo),
            typeof(NumericUpDown),
            new PropertyMetadata(null, (o, e) =>
                {
                    if (e.NewValue != e.OldValue)
                    {
                        var numUpDown = (NumericUpDown)o;
                        numUpDown.regexNumber = null;
                        numUpDown.OnValueChanged(numUpDown.Value, numUpDown.Value);
                    }
                }));

        public static readonly DependencyProperty NumericInputModeProperty = DependencyProperty.Register(
            "NumericInputMode",
            typeof(NumericInput),
            typeof(NumericUpDown),
            new FrameworkPropertyMetadata(NumericInput.All, OnNumericInputModeChanged));

        public static readonly DependencyProperty SnapToMultipleOfIntervalProperty = DependencyProperty.Register(
            "SnapToMultipleOfInterval",
            typeof(bool),
            typeof(NumericUpDown),
            new PropertyMetadata(default(bool), OnSnapToMultipleOfIntervalChanged));

        public static readonly DependencyProperty ParsingNumberStyleProperty = DependencyProperty.Register(
            "ParsingNumberStyle",
            typeof(NumberStyles),
            typeof(NumericUpDown),
            new PropertyMetadata(NumberStyles.Any));

        public static readonly DependencyProperty SwitchUpDownButtonsProperty = DependencyProperty.Register(
            "SwitchUpDownButtons",
            typeof(bool),
            typeof(NumericUpDown),
            new PropertyMetadata(false));

        private static void IsReadOnlyPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue != null)
            {
                var numUpDown = (NumericUpDown)dependencyObject;
                var isReadOnly = (bool)e.NewValue;
                numUpDown.ToggleReadOnlyMode(isReadOnly || !numUpDown.InterceptManualEnter);
            }
        }

        private static void InterceptManualEnterChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue != null)
            {
                var numUpDown = (NumericUpDown)dependencyObject;
                var interceptManualEnter = (bool)e.NewValue;
                numUpDown.ToggleReadOnlyMode(!interceptManualEnter || numUpDown.IsReadOnly);
            }
        }

        private static readonly Regex RegexStringFormatHexadecimal = new Regex(@"^(?<complexHEX>.*{\d\s*:[Xx]\d*}.*)?(?<simpleHEX>[Xx]\d*)?$", RegexOptions.Compiled);
        private const string RawRegexNumberString = @"[-+]?(?<![0-9][<DecimalSeparator><GroupSeparator>])[<DecimalSeparator><GroupSeparator>]?[0-9]+(?:[<DecimalSeparator><GroupSeparator>\s][0-9]+)*[<DecimalSeparator><GroupSeparator>]?[0-9]?(?:[eE][-+]?[0-9]+)?(?!\.[0-9])";
        private Regex regexNumber = null;
        private static readonly Regex RegexHexadecimal = new Regex(@"^([a-fA-F0-9]{1,2}\s?)+$", RegexOptions.Compiled);
        private static readonly Regex RegexStringFormat = new Regex(@"\{0\s*(:(?<format>.*))?\}", RegexOptions.Compiled);

        private const double DefaultInterval = 1d;
        private const int DefaultDelay = 500;
        private const string PART_NumericDown = "PART_NumericDown";
        private const string PART_NumericUp = "PART_NumericUp";
        private const string PART_TextBox = "PART_TextBox";
        private const string PART_ContentHost = "PART_ContentHost";

        private Lazy<PropertyInfo> handlesMouseWheelScrolling = new Lazy<PropertyInfo>();
        private double internalIntervalMultiplierForCalculation = DefaultInterval;
        private double internalLargeChange = DefaultInterval * 100;
        private double intervalValueSinceReset;
        private bool manualChange;
        private RepeatButton repeatDown;
        private RepeatButton repeatUp;
        private TextBox valueTextBox;
        private ScrollViewer scrollViewer;

        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));

            VerticalContentAlignmentProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(HorizontalAlignment.Right));

            EventManager.RegisterClassHandler(typeof(NumericUpDown), GotFocusEvent, new RoutedEventHandler(OnGotFocus));
        }

        public event RoutedPropertyChangedEventHandler<double?> ValueChanged
        {
            add { this.AddHandler(ValueChangedEvent, value); }
            remove { this.RemoveHandler(ValueChangedEvent, value); }
        }

        /// <summary>
        ///     Event fired from this NumericUpDown when its value has reached the maximum value
        /// </summary>
        public event RoutedEventHandler MaximumReached
        {
            add { this.AddHandler(MaximumReachedEvent, value); }
            remove { this.RemoveHandler(MaximumReachedEvent, value); }
        }

        /// <summary>
        ///     Event fired from this NumericUpDown when its value has reached the minimum value
        /// </summary>
        public event RoutedEventHandler MinimumReached
        {
            add { this.AddHandler(MinimumReachedEvent, value); }
            remove { this.RemoveHandler(MinimumReachedEvent, value); }
        }

        public event NumericUpDownChangedRoutedEventHandler ValueIncremented
        {
            add { this.AddHandler(ValueIncrementedEvent, value); }
            remove { this.RemoveHandler(ValueIncrementedEvent, value); }
        }

        public event NumericUpDownChangedRoutedEventHandler ValueDecremented
        {
            add { this.AddHandler(ValueDecrementedEvent, value); }
            remove { this.RemoveHandler(ValueDecrementedEvent, value); }
        }

        public event RoutedEventHandler DelayChanged
        {
            add { this.AddHandler(DelayChangedEvent, value); }
            remove { this.RemoveHandler(DelayChangedEvent, value); }
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
            get { return (int)this.GetValue(DelayProperty); }
            set { this.SetValue(DelayProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the user can use the arrow keys <see cref="Key.Up"/> and <see cref="Key.Down"/> to change values. 
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool InterceptArrowKeys
        {
            get { return (bool)this.GetValue(InterceptArrowKeysProperty); }
            set { this.SetValue(InterceptArrowKeysProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the user can use the mouse wheel to change values.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool InterceptMouseWheel
        {
            get { return (bool)this.GetValue(InterceptMouseWheelProperty); }
            set { this.SetValue(InterceptMouseWheelProperty, value); }
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
            get { return (bool)this.GetValue(TrackMouseWheelWhenMouseOverProperty); }
            set { this.SetValue(TrackMouseWheelWhenMouseOverProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the user can enter text in the control.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool InterceptManualEnter
        {
            get { return (bool)this.GetValue(InterceptManualEnterProperty); }
            set { this.SetValue(InterceptManualEnterProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the value will be changed directly on every TextBox text changed event or when using the Enter key.
        /// </summary>
        [Category("Behavior")]
        public bool ChangeValueOnTextChanged
        {
            get { return (bool)this.GetValue(ChangeValueOnTextChangedProperty); }
            set { this.SetValue(ChangeValueOnTextChangedProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating the culture to be used in string formatting operations.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(null)]
        public CultureInfo Culture
        {
            get { return (CultureInfo)this.GetValue(CultureProperty); }
            set { this.SetValue(CultureProperty, value); }
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
            get { return (bool)this.GetValue(HideUpDownButtonsProperty); }
            set { this.SetValue(HideUpDownButtonsProperty, value); }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(20d)]
        public double UpDownButtonsWidth
        {
            get { return (double)this.GetValue(UpDownButtonsWidthProperty); }
            set { this.SetValue(UpDownButtonsWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the up and down buttons will got the focus when using them.
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool UpDownButtonsFocusable
        {
            get { return (bool)this.GetValue(UpDownButtonsFocusableProperty); }
            set { this.SetValue(UpDownButtonsFocusableProperty, value); }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(ButtonsAlignment.Right)]
        public ButtonsAlignment ButtonsAlignment
        {
            get { return (ButtonsAlignment)this.GetValue(ButtonsAlignmentProperty); }
            set { this.SetValue(ButtonsAlignmentProperty, value); }
        }

        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(DefaultInterval)]
        public double Interval
        {
            get { return (double)this.GetValue(IntervalProperty); }
            set { this.SetValue(IntervalProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the text can be changed by the use of the up or down buttons only.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool IsReadOnly
        {
            get { return (bool)this.GetValue(IsReadOnlyProperty); }
            set { this.SetValue(IsReadOnlyProperty, value); }
        }

        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(double.MaxValue)]
        public double Maximum
        {
            get { return (double)this.GetValue(MaximumProperty); }
            set { this.SetValue(MaximumProperty, value); }
        }

        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(double.MinValue)]
        public double Minimum
        {
            get { return (double)this.GetValue(MinimumProperty); }
            set { this.SetValue(MinimumProperty, value); }
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
            get { return (bool)this.GetValue(SpeedupProperty); }
            set { this.SetValue(SpeedupProperty, value); }
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
            get { return (string)this.GetValue(StringFormatProperty); }
            set { this.SetValue(StringFormatProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the horizontal alignment of the contents of the text box.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(TextAlignment.Right)]
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)this.GetValue(TextAlignmentProperty); }
            set { this.SetValue(TextAlignmentProperty, value); }
        }

        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(null)]
        public double? Value
        {
            get { return (double?)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        private CultureInfo SpecificCultureInfo
        {
            get { return this.Culture ?? this.Language.GetSpecificCulture(); }
        }

        /// <summary>
        /// Gets or sets which numeric input for the NumericUpDown is allowed.
        /// </summary>
        [Category("Common")]
        [DefaultValue(NumericInput.All)]
        public NumericInput NumericInputMode
        {
            get { return (NumericInput)this.GetValue(NumericInputModeProperty); }
            set { this.SetValue(NumericInputModeProperty, value); }
        }

        /// <summary>
        ///     Indicates if the NumericUpDown should round the value to the nearest possible interval when the focus moves to another element.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(false)]
        public bool SnapToMultipleOfInterval
        {
            get { return (bool)this.GetValue(SnapToMultipleOfIntervalProperty); }
            set { this.SetValue(SnapToMultipleOfIntervalProperty, value); }
        }

        /// <summary>
        /// Gets or sets the parsing number style for the value from text to numeric.
        /// </summary>
        [Category("Common")]
        [DefaultValue(NumberStyles.Any)]
        public NumberStyles ParsingNumberStyle
        {
            get { return (NumberStyles)this.GetValue(ParsingNumberStyleProperty); }
            set { this.SetValue(ParsingNumberStyleProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the up down buttons are switched.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool SwitchUpDownButtons
        {
            get { return (bool)this.GetValue(SwitchUpDownButtonsProperty); }
            set { this.SetValue(SwitchUpDownButtonsProperty, value); }
        }

        /// <summary> 
        ///     Called when this element or any below gets focus.
        /// </summary>
        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            // When NumericUpDown gets logical focus, select the text inside us.
            // If we're an editable NumericUpDown, forward focus to the TextBox element
            if (!e.Handled)
            {
                NumericUpDown numericUpDown = (NumericUpDown)sender;
                if ((numericUpDown.InterceptManualEnter || numericUpDown.IsReadOnly) && numericUpDown.Focusable && e.OriginalSource == numericUpDown)
                {
                    // MoveFocus takes a TraversalRequest as its argument.
                    var request = new TraversalRequest((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next);
                    // Gets the element with keyboard focus.
                    var elementWithFocus = Keyboard.FocusedElement as UIElement;
                    // Change keyboard focus.
                    if (elementWithFocus != null)
                    {
                        elementWithFocus.MoveFocus(request);
                    }
                    else
                    {
                        numericUpDown.Focus();
                    }

                    e.Handled = true;
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

            this.repeatUp = this.GetTemplateChild(PART_NumericUp) as RepeatButton;
            this.repeatDown = this.GetTemplateChild(PART_NumericDown) as RepeatButton;

            this.valueTextBox = this.GetTemplateChild(PART_TextBox) as TextBox;

            if (this.repeatUp == null || this.repeatDown == null || this.valueTextBox == null)
            {
                throw new InvalidOperationException($"You have missed to specify {PART_NumericUp}, {PART_NumericDown} or {PART_TextBox} in your template!");
            }

            this.ToggleReadOnlyMode(this.IsReadOnly || !this.InterceptManualEnter);

            this.repeatUp.Click += (o, e) =>
                {
                    this.ChangeValueWithSpeedUp(true);

                    if (!this.UpDownButtonsFocusable)
                    {
                        this.manualChange = false;
                        this.InternalSetText(this.Value);
                    }
                };
            this.repeatDown.Click += (o, e) =>
                {
                    this.ChangeValueWithSpeedUp(false);

                    if (!this.UpDownButtonsFocusable)
                    {
                        this.manualChange = false;
                        this.InternalSetText(this.Value);
                    }
                };

            this.repeatUp.PreviewMouseUp += (o, e) => this.ResetInternal();
            this.repeatDown.PreviewMouseUp += (o, e) => this.ResetInternal();

            this.OnValueChanged(this.Value, this.Value);

            this.scrollViewer = this.TryFindScrollViewer();
        }

        private void ToggleReadOnlyMode(bool isReadOnly)
        {
            if (this.repeatUp == null || this.repeatDown == null || this.valueTextBox == null)
            {
                return;
            }

            if (isReadOnly)
            {
                this.valueTextBox.LostFocus -= this.OnTextBoxLostFocus;
                this.valueTextBox.PreviewTextInput -= this.OnPreviewTextInput;
                this.valueTextBox.PreviewKeyDown -= this.OnTextBoxKeyDown;
                this.valueTextBox.TextChanged -= this.OnTextChanged;
                DataObject.RemovePastingHandler(this.valueTextBox, this.OnValueTextBoxPaste);
            }
            else
            {
                this.valueTextBox.LostFocus += this.OnTextBoxLostFocus;
                this.valueTextBox.PreviewTextInput += this.OnPreviewTextInput;
                this.valueTextBox.PreviewKeyDown += this.OnTextBoxKeyDown;
                this.valueTextBox.TextChanged += this.OnTextChanged;
                DataObject.AddPastingHandler(this.valueTextBox, this.OnValueTextBoxPaste);
            }
        }

        public void SelectAll()
        {
            if (this.valueTextBox != null)
            {
                this.valueTextBox.SelectAll();
            }
        }

        protected virtual void OnDelayChanged(int oldDelay, int newDelay)
        {
            if (oldDelay != newDelay)
            {
                if (this.repeatDown != null)
                {
                    this.repeatDown.Delay = newDelay;
                }

                if (this.repeatUp != null)
                {
                    this.repeatUp.Delay = newDelay;
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

            if (!this.InterceptArrowKeys)
            {
                return;
            }

            if (e.Key == Key.Enter)
            {
                if (!this.ChangeValueOnTextChanged)
                {
                    this.ChangeValueFromTextInput(e.OriginalSource as TextBox);
                }
            }
            else if (e.Key == Key.Up)
            {
                this.ChangeValueWithSpeedUp(true);
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                this.ChangeValueWithSpeedUp(false);
                e.Handled = true;
            }

            if (e.Handled)
            {
                this.manualChange = false;
                this.InternalSetText(this.Value);
            }
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);

            if (e.Key == Key.Down ||
                e.Key == Key.Up)
            {
                this.ResetInternal();
            }
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);

            if (this.InterceptMouseWheel && (this.IsFocused || this.valueTextBox.IsFocused || this.TrackMouseWheelWhenMouseOver))
            {
                bool increment = e.Delta > 0;
                this.manualChange = false;
                this.ChangeValueInternal(increment);
            }

            if (this.scrollViewer != null && this.handlesMouseWheelScrolling.Value != null)
            {
                if (this.TrackMouseWheelWhenMouseOver)
                {
                    this.handlesMouseWheelScrolling.Value.SetValue(this.scrollViewer, true, null);
                }
                else if (this.InterceptMouseWheel)
                {
                    this.handlesMouseWheelScrolling.Value.SetValue(this.scrollViewer, this.valueTextBox.IsFocused, null);
                }
                else
                {
                    this.handlesMouseWheelScrolling.Value.SetValue(this.scrollViewer, true, null);
                }
            }
        }

        protected void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = ((TextBox)sender);
            var fullText = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength).Insert(textBox.CaretIndex, e.Text);
            double convertedValue;
            e.Handled = !this.ValidateText(fullText, out convertedValue);
            this.manualChange = true;
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
            if (!this.manualChange)
            {
                if (!newValue.HasValue)
                {
                    if (this.valueTextBox != null)
                    {
                        this.valueTextBox.Text = null;
                    }

                    if (oldValue != newValue)
                    {
                        this.RaiseEvent(new RoutedPropertyChangedEventArgs<double?>(oldValue, newValue, ValueChangedEvent));
                    }

                    return;
                }

                if (this.repeatUp != null && !this.repeatUp.IsEnabled)
                {
                    this.repeatUp.IsEnabled = true;
                }

                if (this.repeatDown != null && !this.repeatDown.IsEnabled)
                {
                    this.repeatDown.IsEnabled = true;
                }

                if (newValue <= this.Minimum)
                {
                    if (this.repeatDown != null)
                    {
                        this.repeatDown.IsEnabled = false;
                    }

                    this.ResetInternal();

                    if (this.IsLoaded)
                    {
                        this.RaiseEvent(new RoutedEventArgs(MinimumReachedEvent));
                    }
                }

                if (newValue >= this.Maximum)
                {
                    if (this.repeatUp != null)
                    {
                        this.repeatUp.IsEnabled = false;
                    }

                    this.ResetInternal();
                    if (this.IsLoaded)
                    {
                        this.RaiseEvent(new RoutedEventArgs(MaximumReachedEvent));
                    }
                }

                if (this.valueTextBox != null)
                {
                    this.InternalSetText(newValue);
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

            if (!numericUpDown.NumericInputMode.HasFlag(NumericInput.Decimal))
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

            numericUpDown.CoerceValue(MaximumProperty);
            numericUpDown.CoerceValue(ValueProperty);
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

            if (nud.valueTextBox != null && nud.Value.HasValue)
            {
                nud.InternalSetText(nud.Value);
            }

            var format = (string)e.NewValue;

            if (!string.IsNullOrEmpty(format) && RegexStringFormatHexadecimal.IsMatch(format))
            {
                nud.SetCurrentValue(ParsingNumberStyleProperty, NumberStyles.HexNumber);
                nud.SetCurrentValue(NumericInputModeProperty, nud.NumericInputMode | NumericInput.Decimal);
            }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDown)d;

            numericUpDown.OnValueChanged((double?)e.OldValue, (double?)e.NewValue);
        }

        private static void OnNumericInputModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDown)d;
            if (e.NewValue != e.OldValue && e.NewValue is NumericInput && numericUpDown.Value != null)
            {
                var numericInput = (NumericInput)e.NewValue;

                if (!numericInput.HasFlag(NumericInput.Decimal))
                {
                    numericUpDown.Value = Math.Truncate(numericUpDown.Value.GetValueOrDefault());
                }
            }
        }

        private static void OnSnapToMultipleOfIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDown)d;
            var value = numericUpDown.Value.GetValueOrDefault();

            if ((bool)e.NewValue && Math.Abs(numericUpDown.Interval) > 0)
            {
                numericUpDown.Value = Math.Round(value / numericUpDown.Interval) * numericUpDown.Interval;
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
                this.valueTextBox.Text = null;
                return;
            }

            this.valueTextBox.Text = FormattedValueString(newValue.Value, this.StringFormat, this.SpecificCultureInfo);

            if ((bool)this.GetValue(TextBoxHelper.IsMonitoringProperty))
            {
                this.SetValue(TextBoxHelper.TextLengthProperty, this.valueTextBox.Text.Length);
            }
        }

        private static string FormattedValueString(double newValue, string format, CultureInfo culture)
        {
            format = format.Replace("{}", string.Empty);
            if (!string.IsNullOrWhiteSpace(format))
            {
                if(TryFormatHexadecimal(newValue, format, culture, out string hexValue))
                {
                    return hexValue;
                }
                else
                {
                    var match = RegexStringFormat.Match(format);
                    if (match.Success)
                    {
                        // we have a format template such as "{0:N0}"
                        return string.Format(culture, format, newValue);
                    }
                    // we have a format such as "N0"
                    return newValue.ToString(format, culture);
                }
            }

            return newValue.ToString(culture);
        }        

        private static double FormattedValue(double newValue, string format, CultureInfo culture)
        {
            format = format.Replace("{}", string.Empty);
            if (!string.IsNullOrWhiteSpace(format))
            {
                if (!TryFormatHexadecimal(newValue, format, culture, out string hexValue))
                {
                    var match = RegexStringFormat.Match(format);
                    if (match.Success)
                    {
                        // we have a format template such as "{0:N0}"
                        return ConvertStringFormatValue(newValue, match.Groups["format"].Value);
                    }

                    // we have a format such as "N0"
                    return ConvertStringFormatValue(newValue, format);
                }
            }

            return newValue;
        }        

        private static double ConvertStringFormatValue(double value, string format)
        {
            if (format.ToUpperInvariant().Contains("P") || format.Contains("%"))
            {
                value /= 100d;
            }
            else if (format.Contains("‰"))
            {
                value /= 1000d;
            }
            return value;
        }        

        private static bool TryFormatHexadecimal(double newValue, string format, CultureInfo culture, out string output)
        {
            var match = RegexStringFormatHexadecimal.Match(format);
            if (match.Success)
            {
                if (match.Groups["simpleHEX"].Success)
                {
                    // HEX DOES SUPPORT INT ONLY.
                    output = ((int)newValue).ToString(match.Groups["simpleHEX"].Value, culture);
                    return true;
                }

                if (match.Groups["complexHEX"].Success)
                {
                    output = string.Format(culture, match.Groups["complexHEX"].Value, (int)newValue);
                    return true;
                }
            }
            output = null;
            return false;
        }

        private ScrollViewer TryFindScrollViewer()
        {
            this.valueTextBox.ApplyTemplate();

            var scrollViewerFromTemplate = this.valueTextBox.Template.FindName(PART_ContentHost, this.valueTextBox) as ScrollViewer;
            if (scrollViewerFromTemplate != null)
            {
                this.handlesMouseWheelScrolling = new Lazy<PropertyInfo>(() => this.scrollViewer.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).SingleOrDefault(i => i.Name == "HandlesMouseWheelScrolling"));
            }

            return scrollViewerFromTemplate;
        }

        private void ChangeValueWithSpeedUp(bool toPositive)
        {
            if (this.IsReadOnly)
            {
                return;
            }

            double direction = toPositive ? 1 : -1;
            if (this.Speedup)
            {
                double d = this.Interval * this.internalLargeChange;
                if ((this.intervalValueSinceReset += this.Interval * this.internalIntervalMultiplierForCalculation) > d)
                {
                    this.internalLargeChange *= 10;
                    this.internalIntervalMultiplierForCalculation *= 10;
                }

                this.ChangeValueInternal(direction * this.internalIntervalMultiplierForCalculation);
            }
            else
            {
                this.ChangeValueInternal(direction * this.Interval);
            }
        }

        private void ChangeValueInternal(bool addInterval)
        {
            this.ChangeValueInternal(addInterval ? this.Interval : -this.Interval);
        }

        private void ChangeValueInternal(double interval)
        {
            if (this.IsReadOnly)
            {
                return;
            }

            NumericUpDownChangedRoutedEventArgs routedEvent = interval > 0 ? new NumericUpDownChangedRoutedEventArgs(ValueIncrementedEvent, interval) : new NumericUpDownChangedRoutedEventArgs(ValueDecrementedEvent, interval);

            this.RaiseEvent(routedEvent);

            if (!routedEvent.Handled)
            {
                this.ChangeValueBy(routedEvent.Interval);
                this.valueTextBox.CaretIndex = this.valueTextBox.Text.Length;
            }
        }

        private void ChangeValueBy(double difference)
        {
            var newValue = this.Value.GetValueOrDefault() + difference;
            this.SetValueTo(newValue);
        }

        private void SetValueTo(double newValue)
        {
            var value = newValue;

            if (this.SnapToMultipleOfInterval && Math.Abs(this.Interval) > 0)
            {
                value = Math.Round(newValue / this.Interval) * this.Interval;
            }

            if (value > this.Maximum)
            {
                value = this.Maximum;
            }
            else if (value < this.Minimum)
            {
                value = this.Minimum;
            }

            this.SetCurrentValue(ValueProperty, CoerceValue(this, value));
        }

        private void EnableDisableDown()
        {
            if (this.repeatDown != null)
            {
                this.repeatDown.IsEnabled = this.Value > this.Minimum;
            }
        }

        private void EnableDisableUp()
        {
            if (this.repeatUp != null)
            {
                this.repeatUp.IsEnabled = this.Value < this.Maximum;
            }
        }

        private void EnableDisableUpDown()
        {
            this.EnableDisableUp();
            this.EnableDisableDown();
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            this.manualChange = this.manualChange || e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Decimal || e.Key == Key.OemComma || e.Key == Key.OemPeriod;
        }

        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            this.ChangeValueFromTextInput(sender as TextBox);
        }

        private void ChangeValueFromTextInput(TextBox textBox)
        {
            if (textBox is null || !this.InterceptManualEnter)
            {
                return;
            }

            if (this.manualChange)
            {
                this.manualChange = false;

                double convertedValue;
                if (this.ValidateText(textBox.Text, out convertedValue))
                {
                    convertedValue = FormattedValue(convertedValue, this.StringFormat, this.SpecificCultureInfo);
                    this.SetValueTo(convertedValue);
                }
            }

            this.OnValueChanged(this.Value, this.Value);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.ChangeValueOnTextChanged)
            {
                return;
            }

            if (string.IsNullOrEmpty(((TextBox)sender).Text))
            {
                this.Value = null;
            }
            else if (this.manualChange || e.UndoAction == UndoAction.Undo || e.UndoAction == UndoAction.Redo)
            {
                double convertedValue;
                if (this.ValidateText(((TextBox)sender).Text, out convertedValue))
                {
                    convertedValue = FormattedValue(convertedValue, this.StringFormat, this.SpecificCultureInfo);
                    this.SetValueTo(convertedValue);
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
                e.CancelCommand();
                return;
            }

            var text = e.SourceDataObject.GetData(DataFormats.Text) as string;

            string newText = string.Concat(textPresent.Substring(0, textBox.SelectionStart), text, textPresent.Substring(textBox.SelectionStart + textBox.SelectionLength));
            double convertedValue;
            if (!this.ValidateText(newText, out convertedValue))
            {
                e.CancelCommand();
            }
            else
            {
                this.manualChange = true;
            }
        }

        private void RaiseChangeDelay()
        {
            this.RaiseEvent(new RoutedEventArgs(DelayChangedEvent));
        }

        private void ResetInternal()
        {
            if (this.IsReadOnly)
            {
                return;
            }

            this.internalLargeChange = 100 * this.Interval;
            this.internalIntervalMultiplierForCalculation = this.Interval;
            this.intervalValueSinceReset = 0;
        }

        private bool ValidateText(string text, out double convertedValue)
        {
            convertedValue = 0d;

            if (text == this.SpecificCultureInfo.NumberFormat.PositiveSign
                || text == this.SpecificCultureInfo.NumberFormat.NegativeSign)
            {
                return true;
            }

            if (text.Count(c => c == this.SpecificCultureInfo.NumberFormat.PositiveSign[0]) > 2
                || text.Count(c => c == this.SpecificCultureInfo.NumberFormat.NegativeSign[0]) > 2
                || text.Count(c => c == this.SpecificCultureInfo.NumberFormat.NumberGroupSeparator[0]) > 1)
            {
                return false;
            }

            var isNumeric = this.NumericInputMode == NumericInput.Numbers
                            || this.ParsingNumberStyle.HasFlag(NumberStyles.AllowHexSpecifier)
                            || this.ParsingNumberStyle == NumberStyles.HexNumber
                            || this.ParsingNumberStyle == NumberStyles.Integer
                            || this.ParsingNumberStyle == NumberStyles.Number;

            var isHex = this.NumericInputMode == NumericInput.Numbers
                        || this.ParsingNumberStyle.HasFlag(NumberStyles.AllowHexSpecifier)
                        || this.ParsingNumberStyle == NumberStyles.HexNumber;

            var number = this.TryGetNumberFromText(text, isHex);

            // If we are only accepting numbers then attempt to parse as an integer.
            if (isNumeric)
            {
                return this.ConvertNumber(number, out convertedValue);
            }

            if (number == this.SpecificCultureInfo.NumberFormat.NumberDecimalSeparator
                || number == this.SpecificCultureInfo.NumberFormat.CurrencyDecimalSeparator
                || number == this.SpecificCultureInfo.NumberFormat.PercentDecimalSeparator)
            {
                return true;
            }

            if (!double.TryParse(number, this.ParsingNumberStyle, this.SpecificCultureInfo, out convertedValue))
            {
                return false;
            }

            return true;
        }

        private bool ConvertNumber(string text, out double convertedValue)
        {
            if (text.Any(c => c == this.SpecificCultureInfo.NumberFormat.NumberDecimalSeparator[0]
                              || c == this.SpecificCultureInfo.NumberFormat.PercentDecimalSeparator[0]
                              || c == this.SpecificCultureInfo.NumberFormat.CurrencyDecimalSeparator[0]))
            {
                convertedValue = 0d;
                return false;
            }

            if (!long.TryParse(text, this.ParsingNumberStyle, this.SpecificCultureInfo, out var convertedInt))
            {
                convertedValue = convertedInt;
                return false;
            }

            convertedValue = convertedInt;
            return true;
        }

        private string TryGetNumberFromText(string text, bool isHex)
        {
            if (isHex)
            {
                var hexMatches = RegexHexadecimal.Matches(text);
                return hexMatches.Count > 0 ? hexMatches[0].Value : text;
            }

            if (this.regexNumber == null)
            {
                this.regexNumber = new Regex(RawRegexNumberString.Replace("<DecimalSeparator>", this.SpecificCultureInfo.NumberFormat.NumberDecimalSeparator)
                                                                 .Replace("<GroupSeparator>", this.SpecificCultureInfo.NumberFormat.NumberGroupSeparator),
                                             RegexOptions.Compiled);
            }

            var matches = this.regexNumber.Matches(text);
            return matches.Count > 0 ? matches[0].Value : text;
        }
    }
}