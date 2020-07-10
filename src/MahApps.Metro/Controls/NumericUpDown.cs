// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Represents a Windows spin box (also known as an up-down control) that displays numeric values.
    /// </summary>
    [TemplatePart(Name = PART_NumericUp, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PART_NumericDown, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    public class NumericUpDown : Control
    {
        private const string PART_NumericDown = "PART_NumericDown";
        private const string PART_NumericUp = "PART_NumericUp";
        private const string PART_TextBox = "PART_TextBox";
        private const string PART_ContentHost = "PART_ContentHost";
        private const double DefaultInterval = 1d;
        private const int DefaultDelay = 500;

        private static readonly Regex RegexStringFormatHexadecimal = new Regex(@"^(?<complexHEX>.*{\d\s*:[Xx]\d*}.*)?(?<simpleHEX>[Xx]\d*)?$", RegexOptions.Compiled);
        private const string RawRegexNumberString = @"[-+]?(?<![0-9][<DecimalSeparator><GroupSeparator>])[<DecimalSeparator><GroupSeparator>]?[0-9]+(?:[<DecimalSeparator><GroupSeparator>\s][0-9]+)*[<DecimalSeparator><GroupSeparator>]?[0-9]?(?:[eE][-+]?[0-9]+)?(?!\.[0-9])";
        private Regex regexNumber = null;
        private static readonly Regex RegexHexadecimal = new Regex(@"^([a-fA-F0-9]{1,2}\s?)+$", RegexOptions.Compiled);
        private static readonly Regex RegexStringFormat = new Regex(@"\{0\s*(:(?<format>.*))?\}", RegexOptions.Compiled);

        private Lazy<PropertyInfo> handlesMouseWheelScrolling = new Lazy<PropertyInfo>();
        private double internalIntervalMultiplierForCalculation = DefaultInterval;
        private double internalLargeChange = DefaultInterval * 100;
        private double intervalValueSinceReset;
        private bool manualChange;
        private RepeatButton repeatDown;
        private RepeatButton repeatUp;
        private TextBox valueTextBox;
        private ScrollViewer scrollViewer;

        /// <summary>Identifies the <see cref="ValueIncremented"/> routed event.</summary>
        public static readonly RoutedEvent ValueIncrementedEvent
            = EventManager.RegisterRoutedEvent(nameof(ValueIncremented),
                                               RoutingStrategy.Bubble,
                                               typeof(NumericUpDownChangedRoutedEventHandler),
                                               typeof(NumericUpDown));

        /// <summary>
        /// Add / Remove ValueIncrementedEvent handler
        /// Event which will be fired from this NumericUpDown when its value was incremented.
        /// </summary>
        public event NumericUpDownChangedRoutedEventHandler ValueIncremented
        {
            add => this.AddHandler(ValueIncrementedEvent, value);
            remove => this.RemoveHandler(ValueIncrementedEvent, value);
        }

        /// <summary>Identifies the <see cref="ValueDecremented"/> routed event.</summary>
        public static readonly RoutedEvent ValueDecrementedEvent
            = EventManager.RegisterRoutedEvent(nameof(ValueDecremented),
                                               RoutingStrategy.Bubble,
                                               typeof(NumericUpDownChangedRoutedEventHandler),
                                               typeof(NumericUpDown));

        /// <summary>
        /// Add / Remove ValueDecrementedEvent handler
        /// Event which will be fired from this NumericUpDown when its value was decremented.
        /// </summary>
        public event NumericUpDownChangedRoutedEventHandler ValueDecremented
        {
            add => this.AddHandler(ValueDecrementedEvent, value);
            remove => this.RemoveHandler(ValueDecrementedEvent, value);
        }

        /// <summary>Identifies the <see cref="DelayChanged"/> routed event.</summary>
        public static readonly RoutedEvent DelayChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(DelayChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(NumericUpDown));

        /// <summary>
        /// Add / Remove DelayChangedEvent handler
        /// Event which will be fired from this NumericUpDown when its delay value has been changed.
        /// </summary>
        public event RoutedEventHandler DelayChanged
        {
            add => this.AddHandler(DelayChangedEvent, value);
            remove => this.RemoveHandler(DelayChangedEvent, value);
        }

        /// <summary>Identifies the <see cref="MaximumReached"/> routed event.</summary>
        public static readonly RoutedEvent MaximumReachedEvent
            = EventManager.RegisterRoutedEvent(nameof(MaximumReached),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(NumericUpDown));

        /// <summary>
        /// Add / Remove MaximumReachedEvent handler
        /// Event fired from this NumericUpDown when its value has reached the maximum value.
        /// </summary>
        public event RoutedEventHandler MaximumReached
        {
            add => this.AddHandler(MaximumReachedEvent, value);
            remove => this.RemoveHandler(MaximumReachedEvent, value);
        }

        /// <summary>Identifies the <see cref="MinimumReached"/> routed event.</summary>
        public static readonly RoutedEvent MinimumReachedEvent
            = EventManager.RegisterRoutedEvent(nameof(MinimumReached),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(NumericUpDown));

        /// <summary>
        /// Add / Remove MinimumReachedEvent handler
        /// Event fired from this NumericUpDown when its value has reached the minimum value.
        /// </summary>
        public event RoutedEventHandler MinimumReached
        {
            add => this.AddHandler(MinimumReachedEvent, value);
            remove => this.RemoveHandler(MinimumReachedEvent, value);
        }

        /// <summary>Identifies the <see cref="ValueChanged"/> routed event.</summary>
        public static readonly RoutedEvent ValueChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(ValueChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedPropertyChangedEventHandler<double?>),
                                               typeof(NumericUpDown));

        /// <summary>
        /// Add / Remove ValueChangedEvent handler
        /// Event which will be fired from this NumericUpDown when its value has been changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double?> ValueChanged
        {
            add => this.AddHandler(ValueChangedEvent, value);
            remove => this.RemoveHandler(ValueChangedEvent, value);
        }

        /// <summary>Identifies the <see cref="Delay"/> dependency property.</summary>
        public static readonly DependencyProperty DelayProperty
            = DependencyProperty.Register(nameof(Delay),
                                          typeof(int),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(DefaultDelay, OnDelayPropertyChanged),
                                          value => Convert.ToInt32(value) >= 0);

        private static void OnDelayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.OldValue is int oldDelay && e.NewValue is int newDelay && d is NumericUpDown numericUpDown)
            {
                numericUpDown.RaiseChangeDelay();
                numericUpDown.OnDelayChanged(oldDelay, newDelay);
            }
        }

        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, the NumericUpDown waits while the up/down button is pressed
        /// before it starts increasing/decreasing the <see cref="Value" /> for the specified <see cref="Interval" /> .
        /// The value must be non-negative.
        /// </summary>
        [Bindable(true)]
        [DefaultValue(DefaultDelay)]
        [Category("Behavior")]
        public int Delay
        {
            get => (int)this.GetValue(DelayProperty);
            set => this.SetValue(DelayProperty, value);
        }

        /// <summary>Identifies the <see cref="TextAlignment"/> dependency property.</summary>
        public static readonly DependencyProperty TextAlignmentProperty = TextBox.TextAlignmentProperty.AddOwner(typeof(NumericUpDown));

        /// <summary>
        /// Gets or sets the horizontal alignment of the contents inside the text box.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(TextAlignment.Right)]
        public TextAlignment TextAlignment
        {
            get => (TextAlignment)this.GetValue(TextAlignmentProperty);
            set => this.SetValue(TextAlignmentProperty, value);
        }

        /// <summary>Identifies the <see cref="Speedup"/> dependency property.</summary>
        public static readonly DependencyProperty SpeedupProperty
            = DependencyProperty.Register(nameof(Speedup),
                                          typeof(bool),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(BooleanBoxes.TrueBox, OnSpeedupPropertyChanged));

        private static void OnSpeedupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                (d as NumericUpDown)?.OnSpeedupChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the value to be added to or subtracted from <see cref="Value" /> remains
        /// always <see cref="Interval" /> or if it will increase faster after pressing the up/down button/arrow some time.
        /// </summary>
        [Category("Common")]
        [DefaultValue(true)]
        public bool Speedup
        {
            get => (bool)this.GetValue(SpeedupProperty);
            set => this.SetValue(SpeedupProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="IsReadOnly"/> dependency property.</summary>
        public static readonly DependencyProperty IsReadOnlyProperty
            = TextBoxBase.IsReadOnlyProperty.AddOwner(typeof(NumericUpDown),
                                                      new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, OnIsReadOnlyPropertyChanged));

        private static void OnIsReadOnlyPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is bool isReadOnly)
            {
                if (dependencyObject is NumericUpDown numericUpDown)
                {
                    numericUpDown.ToggleReadOnlyMode(isReadOnly);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text can be changed by the use of the up or down buttons only.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool IsReadOnly
        {
            get => (bool)this.GetValue(IsReadOnlyProperty);
            set => this.SetValue(IsReadOnlyProperty, value);
        }

        /// <summary>Identifies the <see cref="StringFormat"/> dependency property.</summary>
        public static readonly DependencyProperty StringFormatProperty
            = DependencyProperty.Register(nameof(StringFormat),
                                          typeof(string),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(string.Empty, OnStringFormatPropertyChanged, CoerceStringFormat));

        private static void OnStringFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && d is NumericUpDown numericUpDown)
            {
                if (numericUpDown.valueTextBox != null && numericUpDown.Value.HasValue)
                {
                    numericUpDown.InternalSetText(numericUpDown.Value);
                }

                if (e.NewValue is string format && !string.IsNullOrEmpty(format) && RegexStringFormatHexadecimal.IsMatch(format))
                {
                    numericUpDown.SetCurrentValue(ParsingNumberStyleProperty, NumberStyles.HexNumber);
                    numericUpDown.SetCurrentValue(NumericInputModeProperty, numericUpDown.NumericInputMode | NumericInput.Decimal);
                }
            }
        }

        private static object CoerceStringFormat(DependencyObject d, object baseValue)
        {
            return baseValue ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the formatting for the displaying <see cref="Value" />
        /// </summary>
        /// <remarks>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings"></see>
        /// </remarks>
        [Category("Common")]
        public string StringFormat
        {
            get => (string)this.GetValue(StringFormatProperty);
            set => this.SetValue(StringFormatProperty, value);
        }

        /// <summary>Identifies the <see cref="InterceptArrowKeys"/> dependency property.</summary>
        public static readonly DependencyProperty InterceptArrowKeysProperty
            = DependencyProperty.Register(nameof(InterceptArrowKeys),
                                          typeof(bool),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets a value indicating whether the user can use the arrow keys <see cref="Key.Up"/> and <see cref="Key.Down"/> to change the value. 
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool InterceptArrowKeys
        {
            get => (bool)this.GetValue(InterceptArrowKeysProperty);
            set => this.SetValue(InterceptArrowKeysProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="InterceptMouseWheel"/> dependency property.</summary>
        public static readonly DependencyProperty InterceptMouseWheelProperty
            = DependencyProperty.Register(nameof(InterceptMouseWheel),
                                          typeof(bool),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets a value indicating whether the user can use the mouse wheel to change the value.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool InterceptMouseWheel
        {
            get => (bool)this.GetValue(InterceptMouseWheelProperty);
            set => this.SetValue(InterceptMouseWheelProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="InterceptManualEnter"/> dependency property.</summary>
        public static readonly DependencyProperty InterceptManualEnterProperty
            = DependencyProperty.Register(nameof(InterceptManualEnter),
                                          typeof(bool),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(BooleanBoxes.TrueBox, OnInterceptManualEnterPropertyChanged));

        private static void OnInterceptManualEnterPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                if (dependencyObject is NumericUpDown numericUpDown)
                {
                    numericUpDown.ToggleReadOnlyMode(numericUpDown.IsReadOnly);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can enter text in the control.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool InterceptManualEnter
        {
            get => (bool)this.GetValue(InterceptManualEnterProperty);
            set => this.SetValue(InterceptManualEnterProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="Value"/> dependency property.</summary>
        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.Register(nameof(Value),
                                          typeof(double?),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(default(double?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValuePropertyChanged, CoerceValue));

        private static void OnValuePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                (dependencyObject as NumericUpDown)?.OnValueChanged((double?)e.OldValue, (double?)e.NewValue);
            }
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

        /// <summary>
        /// Gets or sets the value of the NumericUpDown.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(null)]
        public double? Value
        {
            get => (double?)this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        /// <summary>Identifies the <see cref="Minimum"/> dependency property.</summary>
        public static readonly DependencyProperty MinimumProperty
            = DependencyProperty.Register(nameof(Minimum),
                                          typeof(double),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(double.MinValue, OnMinimumPropertyChanged));

        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDown)d;

            numericUpDown.CoerceValue(MaximumProperty);
            numericUpDown.CoerceValue(ValueProperty);
            numericUpDown.OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
            numericUpDown.EnableDisableUpDown();
        }

        /// <summary>
        /// Minimum restricts the minimum value of the Value property.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(double.MinValue)]
        public double Minimum
        {
            get => (double)this.GetValue(MinimumProperty);
            set => this.SetValue(MinimumProperty, value);
        }

        /// <summary>Identifies the <see cref="Maximum"/> dependency property.</summary>
        public static readonly DependencyProperty MaximumProperty
            = DependencyProperty.Register(nameof(Maximum),
                                          typeof(double),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(double.MaxValue, OnMaximumPropertyChanged, CoerceMaximum));

        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDown)d;

            numericUpDown.CoerceValue(ValueProperty);
            numericUpDown.OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
            numericUpDown.EnableDisableUpDown();
        }

        private static object CoerceMaximum(DependencyObject d, object value)
        {
            double minimum = ((NumericUpDown)d).Minimum;
            double val = (double)value;
            return val < minimum ? minimum : val;
        }

        /// <summary>
        /// Maximum restricts the maximum value of the Value property.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(double.MaxValue)]
        public double Maximum
        {
            get => (double)this.GetValue(MaximumProperty);
            set => this.SetValue(MaximumProperty, value);
        }

        /// <summary>Identifies the <see cref="Interval"/> dependency property.</summary>
        public static readonly DependencyProperty IntervalProperty
            = DependencyProperty.Register(nameof(Interval),
                                          typeof(double),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(DefaultInterval, OnIntervalPropertyChanged));

        private static void OnIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NumericUpDown)?.ResetInternal();
        }

        /// <summary>
        /// Gets or sets the interval value for increasing/decreasing the <see cref="Value" /> .
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(DefaultInterval)]
        public double Interval
        {
            get => (double)this.GetValue(IntervalProperty);
            set => this.SetValue(IntervalProperty, value);
        }

        /// <summary>Identifies the <see cref="TrackMouseWheelWhenMouseOver"/> dependency property.</summary>
        public static readonly DependencyProperty TrackMouseWheelWhenMouseOverProperty
            = DependencyProperty.Register(nameof(TrackMouseWheelWhenMouseOver),
                                          typeof(bool),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets a value indicating whether the control must have the focus in order to change values using the mouse wheel.
        /// </summary>
        /// <remarks>
        /// If the value is true then the value changes when the mouse wheel is over the control. <br/>
        /// If the value is false then the value changes only if the control has the focus. <br/>
        /// If <see cref="InterceptMouseWheel"/> is set to "false" then this property has no effect.
        /// </remarks>
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool TrackMouseWheelWhenMouseOver
        {
            get => (bool)this.GetValue(TrackMouseWheelWhenMouseOverProperty);
            set => this.SetValue(TrackMouseWheelWhenMouseOverProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ButtonsAlignment"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonsAlignmentProperty
            = DependencyProperty.Register(nameof(ButtonsAlignment),
                                          typeof(ButtonsAlignment),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(ButtonsAlignment.Right, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The ButtonsAlignment property specifies horizontal alignment of the up/down buttons.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(ButtonsAlignment.Right)]
        public ButtonsAlignment ButtonsAlignment
        {
            get => (ButtonsAlignment)this.GetValue(ButtonsAlignmentProperty);
            set => this.SetValue(ButtonsAlignmentProperty, value);
        }

        /// <summary>Identifies the <see cref="HideUpDownButtons"/> dependency property.</summary>
        public static readonly DependencyProperty HideUpDownButtonsProperty
            = DependencyProperty.Register(nameof(HideUpDownButtons),
                                          typeof(bool),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets a value indicating whether the up/down button of the control are visible.
        /// </summary>
        /// <remarks>
        /// If the value is false then the <see cref="Value" /> of the control can be changed only if one of the following cases is satisfied:
        /// <list type="bullet">
        ///     <item>
        ///         <description><see cref="InterceptArrowKeys" /> is true.</description>
        ///     </item>
        ///     <item>
        ///         <description><see cref="InterceptMouseWheel" /> is true.</description>
        ///     </item>
        ///     <item>
        ///         <description><see cref="InterceptManualEnter" /> is true.</description>
        ///     </item>
        /// </list>
        /// </remarks>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool HideUpDownButtons
        {
            get => (bool)this.GetValue(HideUpDownButtonsProperty);
            set => this.SetValue(HideUpDownButtonsProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="UpDownButtonsWidth"/> dependency property.</summary>
        public static readonly DependencyProperty UpDownButtonsWidthProperty
            = DependencyProperty.Register(nameof(UpDownButtonsWidth),
                                          typeof(double),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(20d));

        /// <summary>
        /// Gets or sets the width of the up/down buttons.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(20d)]
        public double UpDownButtonsWidth
        {
            get => (double)this.GetValue(UpDownButtonsWidthProperty);
            set => this.SetValue(UpDownButtonsWidthProperty, value);
        }

        /// <summary>Identifies the <see cref="UpDownButtonsFocusable"/> dependency property.</summary>
        public static readonly DependencyProperty UpDownButtonsFocusableProperty
            = DependencyProperty.Register(nameof(UpDownButtonsFocusable),
                                          typeof(bool),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether the up and down buttons will got the focus when using them.
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool UpDownButtonsFocusable
        {
            get => (bool)this.GetValue(UpDownButtonsFocusableProperty);
            set => this.SetValue(UpDownButtonsFocusableProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="SwitchUpDownButtons"/> dependency property.</summary>
        public static readonly DependencyProperty SwitchUpDownButtonsProperty
            = DependencyProperty.Register(nameof(SwitchUpDownButtons),
                                          typeof(bool),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets a value indicating whether the up/down buttons will be switched.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool SwitchUpDownButtons
        {
            get => (bool)this.GetValue(SwitchUpDownButtonsProperty);
            set => this.SetValue(SwitchUpDownButtonsProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ChangeValueOnTextChanged"/> dependency property.</summary>
        public static readonly DependencyProperty ChangeValueOnTextChangedProperty
            = DependencyProperty.Register(nameof(ChangeValueOnTextChanged),
                                          typeof(bool),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets a value indicating whether the value will be changed directly on every TextBox text changed input event or when using the Enter key.
        /// </summary>
        [Category("Behavior")]
        public bool ChangeValueOnTextChanged
        {
            get => (bool)this.GetValue(ChangeValueOnTextChangedProperty);
            set => this.SetValue(ChangeValueOnTextChangedProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="Culture"/> dependency property.</summary>
        public static readonly DependencyProperty CultureProperty
            = DependencyProperty.Register(nameof(Culture),
                                          typeof(CultureInfo),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(null, OnCulturePropertyChanged));

        private static void OnCulturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && d is NumericUpDown numericUpDown)
            {
                numericUpDown.regexNumber = null;
                numericUpDown.OnValueChanged(numericUpDown.Value, numericUpDown.Value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the culture to be used in string formatting and converting operations.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(null)]
        public CultureInfo Culture
        {
            get => (CultureInfo)this.GetValue(CultureProperty);
            set => this.SetValue(CultureProperty, value);
        }

        /// <summary>Identifies the <see cref="NumericInputMode"/> dependency property.</summary>
        public static readonly DependencyProperty NumericInputModeProperty
            = DependencyProperty.Register(nameof(NumericInputMode),
                                          typeof(NumericInput),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(NumericInput.All, OnNumericInputModePropertyChanged));

        private static void OnNumericInputModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is NumericInput numericInput && d is NumericUpDown numericUpDown && numericUpDown.Value != null)
            {
                if (!numericInput.HasFlag(NumericInput.Decimal))
                {
                    numericUpDown.Value = Math.Truncate(numericUpDown.Value.GetValueOrDefault());
                }
            }
        }

        /// <summary>
        /// Gets or sets which numeric input for this NumericUpDown is allowed.
        /// </summary>
        [Category("Common")]
        [DefaultValue(NumericInput.All)]
        public NumericInput NumericInputMode
        {
            get => (NumericInput)this.GetValue(NumericInputModeProperty);
            set => this.SetValue(NumericInputModeProperty, value);
        }

        /// <summary>Identifies the <see cref="DecimalPointCorrection"/> dependency property.</summary>
        public static readonly DependencyProperty DecimalPointCorrectionProperty
            = DependencyProperty.Register(nameof(DecimalPointCorrection),
                                          typeof(DecimalPointCorrectionMode),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(default(DecimalPointCorrectionMode)));

        /// <summary>
        /// Gets or sets the decimal-point correction mode. The default is <see cref="DecimalPointCorrectionMode.Inherits"/>
        /// </summary>
        public DecimalPointCorrectionMode DecimalPointCorrection
        {
            get => (DecimalPointCorrectionMode)this.GetValue(DecimalPointCorrectionProperty);
            set => this.SetValue(DecimalPointCorrectionProperty, value);
        }

        /// <summary>Identifies the <see cref="SnapToMultipleOfInterval"/> dependency property.</summary>
        public static readonly DependencyProperty SnapToMultipleOfIntervalProperty
            = DependencyProperty.Register(nameof(SnapToMultipleOfInterval),
                                          typeof(bool),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(BooleanBoxes.FalseBox, OnSnapToMultipleOfIntervalPropertyChanged));

        private static void OnSnapToMultipleOfIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is bool snap && d is NumericUpDown numericUpDown)
            {
                if (!snap)
                {
                    return;
                }

                if (Math.Abs(numericUpDown.Interval) > 0)
                {
                    var value = numericUpDown.Value.GetValueOrDefault();
                    numericUpDown.Value = Math.Round(value / numericUpDown.Interval) * numericUpDown.Interval;
                }
            }
        }

        /// <summary>
        /// Indicates if the NumericUpDown should round the value to the nearest possible interval when the focus moves to another element.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(false)]
        public bool SnapToMultipleOfInterval
        {
            get => (bool)this.GetValue(SnapToMultipleOfIntervalProperty);
            set => this.SetValue(SnapToMultipleOfIntervalProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ParsingNumberStyle"/> dependency property.</summary>
        public static readonly DependencyProperty ParsingNumberStyleProperty
            = DependencyProperty.Register(nameof(ParsingNumberStyle),
                                          typeof(NumberStyles),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(NumberStyles.Any));

        /// <summary>
        /// Gets or sets the parsing number style for the value from text to numeric value.
        /// </summary>
        [Category("Common")]
        [DefaultValue(NumberStyles.Any)]
        public NumberStyles ParsingNumberStyle
        {
            get => (NumberStyles)this.GetValue(ParsingNumberStyleProperty);
            set => this.SetValue(ParsingNumberStyleProperty, value);
        }

        private CultureInfo SpecificCultureInfo => this.Culture ?? this.Language.GetSpecificCulture();

        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));

            VerticalContentAlignmentProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(HorizontalAlignment.Right));

            EventManager.RegisterClassHandler(typeof(NumericUpDown), GotFocusEvent, new RoutedEventHandler(OnGotFocus));
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
                    // And change the keyboard focus.
                    if (Keyboard.FocusedElement is UIElement elementWithFocus)
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

            this.ToggleReadOnlyMode(this.IsReadOnly);

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

            this.scrollViewer = null;
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
            this.valueTextBox?.SelectAll();
        }

        private void RaiseChangeDelay()
        {
            this.RaiseEvent(new RoutedEventArgs(DelayChangedEvent));
        }

        /// <summary>
        /// This method is invoked when the Delay property changes.
        /// </summary>
        /// <param name="oldDelay">The old value of the Delay property.</param>
        /// <param name="newDelay">The new value of the Delay property.</param>
        protected virtual void OnDelayChanged(int oldDelay, int newDelay)
        {
            // nothing here
        }

        /// <summary>
        /// This method is invoked when the Speedup property changes.
        /// </summary>
        /// <param name="oldSpeedup">The old value of the Speedup property.</param>
        /// <param name="newSpeedup">The new value of the Speedup property.</param>
        protected virtual void OnSpeedupChanged(bool oldSpeedup, bool newSpeedup)
        {
            // nothing here
        }

        /// <summary>
        /// This method is invoked when the Maximum property changes.
        /// </summary>
        /// <param name="oldMaximum">The old value of the Maximum property.</param>
        /// <param name="newMaximum">The new value of the Maximum property.</param>
        protected virtual void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            // nothing here
        }

        /// <summary>
        /// This method is invoked when the Minimum property changes.
        /// </summary>
        /// <param name="oldMinimum">The old value of the Minimum property.</param>
        /// <param name="newMinimum">The new value of the Minimum property.</param>
        protected virtual void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            // nothing here
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

            var sv = this.TryFindScrollViewer();

            if (sv != null && this.handlesMouseWheelScrolling.Value != null)
            {
                if (this.TrackMouseWheelWhenMouseOver)
                {
                    this.handlesMouseWheelScrolling.Value.SetValue(sv, true, null);
                }
                else if (this.InterceptMouseWheel)
                {
                    this.handlesMouseWheelScrolling.Value.SetValue(sv, this.valueTextBox.IsFocused, null);
                }
                else
                {
                    this.handlesMouseWheelScrolling.Value.SetValue(sv, true, null);
                }
            }
        }

        protected void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = (TextBox)sender;
            var fullText = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength).Insert(textBox.CaretIndex, e.Text);
            e.Handled = !this.ValidateText(fullText, out _);
            this.manualChange = true;
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
                if (TryFormatHexadecimal(newValue, format, culture, out string hexValue))
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
            if (this.scrollViewer != null)
            {
                return this.scrollViewer;
            }

            this.valueTextBox.ApplyTemplate();

            this.scrollViewer = this.valueTextBox.Template.FindName(PART_ContentHost, this.valueTextBox) as ScrollViewer;
            if (this.scrollViewer != null)
            {
                this.handlesMouseWheelScrolling = new Lazy<PropertyInfo>(() => this.scrollViewer.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).SingleOrDefault(i => i.Name == "HandlesMouseWheelScrolling"));
            }

            return this.scrollViewer;
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
                this.repeatDown.IsEnabled = this.Value == null || this.Value > this.Minimum;
            }
        }

        private void EnableDisableUp()
        {
            if (this.repeatUp != null)
            {
                this.repeatUp.IsEnabled = this.Value == null || this.Value < this.Maximum;
            }
        }

        private void EnableDisableUpDown()
        {
            this.EnableDisableUp();
            this.EnableDisableDown();
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            this.manualChange = this.manualChange
                                || e.Key == Key.Back
                                || e.Key == Key.Delete
                                || e.Key == Key.Decimal
                                || e.Key == Key.OemComma
                                || e.Key == Key.OemPeriod;

            // Filter the Numpad's decimal-point key only
            if (e.Key == Key.Decimal && this.DecimalPointCorrection != DecimalPointCorrectionMode.Inherits)
            {
                // Mark the event as handled, so no further action will take place
                e.Handled = true;

                // Grab the originating TextBox control...
                var textBox = (TextBoxBase)sender;

                // The current correction mode...
                var correctionMode = this.DecimalPointCorrection;

                // And the culture of the NUD
                var culture = this.SpecificCultureInfo;

                // Surrogate the blocked key pressed
                SimulateDecimalPointKeyPress(textBox, correctionMode, culture);
            }
        }

        /// <summary>
        /// Insertion of the proper decimal-point as part of the TextBox content
        /// </summary>
        /// <param name="textBox">The TextBox which will be used for the correction</param>
        /// <param name="mode">The decimal correction mode.</param>
        /// <param name="culture">The culture with the decimal-point information.</param>
        /// <remarks>
        /// Typical "async-void" pattern as "fire-and-forget" behavior.
        /// </remarks>
        private static async void SimulateDecimalPointKeyPress(TextBoxBase textBox, DecimalPointCorrectionMode mode, CultureInfo culture)
        {
            // Select the proper decimal-point string upon the context
            string replace;
            switch (mode)
            {
                case DecimalPointCorrectionMode.Number:
                    replace = culture.NumberFormat.NumberDecimalSeparator;
                    break;

                case DecimalPointCorrectionMode.Currency:
                    replace = culture.NumberFormat.CurrencyDecimalSeparator;
                    break;

                case DecimalPointCorrectionMode.Percent:
                    replace = culture.NumberFormat.PercentDecimalSeparator;
                    break;

                default:
                    replace = null;
                    break;
            }

            if (string.IsNullOrEmpty(replace) == false)
            {
                // Insert the desired string
                var tc = new TextComposition(InputManager.Current, textBox, replace);

                TextCompositionManager.StartComposition(tc);
            }

            await Task.FromResult(true);
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

                if (this.ValidateText(textBox.Text, out var convertedValue))
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
                if (this.ValidateText(((TextBox)sender).Text, out var convertedValue))
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
            if (!this.ValidateText(newText, out _))
            {
                e.CancelCommand();
            }
            else
            {
                this.manualChange = true;
            }
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