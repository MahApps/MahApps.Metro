// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using JetBrains.Annotations;
using MahApps.Metro.Automation.Peers;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Represents a Windows spin box (also known as an up-down control) that displays numeric values.
    /// </summary>
    [TemplatePart(Name = PART_NumericUp, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PART_NumericDown, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    [StyleTypedProperty(Property = nameof(SpinButtonStyle), StyleTargetType = typeof(ButtonBase))]
    public abstract class NumericUpDownBase : Control
    {
        private const string PART_NumericDown = "PART_NumericDown";
        private const string PART_NumericUp = "PART_NumericUp";
        private const string PART_TextBox = "PART_TextBox";
        private const string PART_ContentHost = "PART_ContentHost";
        private const int DefaultDelay = 500;

        private protected static readonly Regex RegexStringFormatHexadecimal = new Regex(@"^(?<complexHEX>.*{\d\s*:[Xx]\d*}.*)?(?<simpleHEX>[Xx]\d*)?$", RegexOptions.Compiled);
        private protected const string RawRegexNumberString = @"[<Sign>]?(?<![0-9][<DecimalSeparator><GroupSeparator>])[<DecimalSeparator><GroupSeparator>]?[0-9]+(?:[<DecimalSeparator><GroupSeparator>\s][0-9]+)*[<DecimalSeparator><GroupSeparator>]?[0-9]?(?:[eE][-+]?[0-9]+)?(?!\.[0-9])";
        private protected Regex? regexNumber = null;
        private protected static readonly Regex RegexHexadecimal = new Regex(@"^([a-fA-F0-9]{1,2}\s?)+$", RegexOptions.Compiled);
        private protected static readonly Regex RegexStringFormat = new Regex(@"\{0\s*(:(?<format>.*))?\}", RegexOptions.Compiled);

        private Lazy<PropertyInfo?> handlesMouseWheelScrolling = new Lazy<PropertyInfo?>();
        /// <summary>Whether the value is being changed by someone typing rather than from the outside.</summary>
        protected bool manualChange;
        /// <summary>The button that steps the value down.</summary>
        protected RepeatButton? repeatDown;
        /// <summary>The button that steps the value up.</summary>
        protected RepeatButton? repeatUp;
        /// <summary>The text box the value is shown in, once the template has been applied.</summary>
        protected TextBox? valueTextBox;
        private ScrollViewer? scrollViewer;

        /// <summary>Identifies the <see cref="ValueIncremented"/> routed event.</summary>
        public static readonly RoutedEvent ValueIncrementedEvent
            = EventManager.RegisterRoutedEvent(nameof(ValueIncremented),
                                               RoutingStrategy.Bubble,
                                               typeof(NumericUpDownChangedRoutedEventHandler),
                                               typeof(NumericUpDownBase));

        /// <summary>
        /// Add / Remove ValueIncrementedEvent handler
        /// Event which will be fired from this NumericUpDownBase when its value was incremented.
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
                                               typeof(NumericUpDownBase));

        /// <summary>
        /// Add / Remove ValueDecrementedEvent handler
        /// Event which will be fired from this NumericUpDownBase when its value was decremented.
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
                                               typeof(NumericUpDownBase));

        /// <summary>
        /// Add / Remove DelayChangedEvent handler
        /// Event which will be fired from this NumericUpDownBase when its delay value has been changed.
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
                                               typeof(NumericUpDownBase));

        /// <summary>
        /// Add / Remove MaximumReachedEvent handler
        /// Event fired from this NumericUpDownBase when its value has reached the maximum value.
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
                                               typeof(NumericUpDownBase));

        /// <summary>
        /// Add / Remove MinimumReachedEvent handler
        /// Event fired from this NumericUpDownBase when its value has reached the minimum value.
        /// </summary>
        public event RoutedEventHandler MinimumReached
        {
            add => this.AddHandler(MinimumReachedEvent, value);
            remove => this.RemoveHandler(MinimumReachedEvent, value);
        }

        /// <summary>Identifies the <see cref="Delay"/> dependency property.</summary>
        public static readonly DependencyProperty DelayProperty
            = DependencyProperty.Register(nameof(Delay),
                                          typeof(int),
                                          typeof(NumericUpDownBase),
                                          new FrameworkPropertyMetadata(DefaultDelay, OnDelayPropertyChanged),
                                          value => Convert.ToInt32(value) >= 0);

        private static void OnDelayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.OldValue is int oldDelay && e.NewValue is int newDelay && d is NumericUpDownBase numericUpDown)
            {
                numericUpDown.RaiseChangeDelay();
                numericUpDown.OnDelayChanged(oldDelay, newDelay);
            }
        }

        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, the NumericUpDownBase waits while the up/down button is pressed
        /// before it starts increasing/decreasing the value by the interval.
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
        public static readonly DependencyProperty TextAlignmentProperty = TextBox.TextAlignmentProperty.AddOwner(typeof(NumericUpDownBase));

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
                                          typeof(NumericUpDownBase),
                                          new FrameworkPropertyMetadata(BooleanBoxes.TrueBox, OnSpeedupPropertyChanged));

        private static void OnSpeedupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                (d as NumericUpDownBase)?.OnSpeedupChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the amount added to or subtracted from the value stays
        /// at the interval or grows the longer the up/down button or arrow key is held.
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
            = TextBoxBase.IsReadOnlyProperty.AddOwner(typeof(NumericUpDownBase),
                                                      new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.Inherits, OnIsReadOnlyPropertyChanged));

        private static void OnIsReadOnlyPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is bool isReadOnly)
            {
                if (dependencyObject is NumericUpDownBase numericUpDown)
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
            set => this.SetValue(IsReadOnlyProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="StringFormat"/> dependency property.</summary>
        public static readonly DependencyProperty StringFormatProperty
            = DependencyProperty.Register(nameof(StringFormat),
                                          typeof(string),
                                          typeof(NumericUpDownBase),
                                          new FrameworkPropertyMetadata(string.Empty, OnStringFormatPropertyChanged, CoerceStringFormat));

        private static void OnStringFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && d is NumericUpDownBase numericUpDown)
            {
                if (numericUpDown.valueTextBox != null && numericUpDown.HasValue)
                {
                    numericUpDown.RefreshTextFromValue();
                }

                if (e.NewValue is string format && !string.IsNullOrEmpty(format) && RegexStringFormatHexadecimal.IsMatch(format))
                {
                    numericUpDown.SetCurrentValue(ParsingNumberStyleProperty, NumberStyles.HexNumber);
                    numericUpDown.SetCurrentValue(NumericInputModeProperty, numericUpDown.NumericInputMode | NumericInput.Decimal);
                }
            }
        }

        [MustUseReturnValue]
        private static object CoerceStringFormat(DependencyObject d, object? baseValue)
        {
            return baseValue ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the formatting for the displayed value
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
                                          typeof(NumericUpDownBase),
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
                                          typeof(NumericUpDownBase),
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
                                          typeof(NumericUpDownBase),
                                          new PropertyMetadata(BooleanBoxes.TrueBox, OnInterceptManualEnterPropertyChanged));

        private static void OnInterceptManualEnterPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                if (dependencyObject is NumericUpDownBase numericUpDown)
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

        /// <summary>Identifies the <see cref="SyncTextWithValueWhileEditing"/> dependency property.</summary>
        public static readonly DependencyProperty SyncTextWithValueWhileEditingProperty
            = DependencyProperty.Register(nameof(SyncTextWithValueWhileEditing),
                                          typeof(bool),
                                          typeof(NumericUpDownBase),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets a value indicating whether the displayed text is kept in sync with the
        /// value while the control is being edited (has keyboard focus), when the value
        /// is changed from an external source such as a binding. When set to <see langword="false"/>
        /// (the default), the current behavior is kept: the text is only refreshed from the value once
        /// the control loses focus.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool SyncTextWithValueWhileEditing
        {
            get => (bool)this.GetValue(SyncTextWithValueWhileEditingProperty);
            set => this.SetValue(SyncTextWithValueWhileEditingProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="TrackMouseWheelWhenMouseOver"/> dependency property.</summary>
        public static readonly DependencyProperty TrackMouseWheelWhenMouseOverProperty
            = DependencyProperty.Register(nameof(TrackMouseWheelWhenMouseOver),
                                          typeof(bool),
                                          typeof(NumericUpDownBase),
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

        /// <summary>Identifies the <see cref="SpinButtonStyle"/> dependency property.</summary>
        public static readonly DependencyProperty SpinButtonStyleProperty
            = DependencyProperty.Register(nameof(SpinButtonStyle),
                                          typeof(Style),
                                          typeof(NumericUpDownBase),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="FrameworkElement.Style"/> for the spin buttons.
        /// </summary>
        public Style? SpinButtonStyle
        {
            get => (Style?)this.GetValue(SpinButtonStyleProperty);
            set => this.SetValue(SpinButtonStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="ButtonsAlignment"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonsAlignmentProperty
            = DependencyProperty.Register(nameof(ButtonsAlignment),
                                          typeof(ButtonsAlignment),
                                          typeof(NumericUpDownBase),
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
                                          typeof(NumericUpDownBase),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets a value indicating whether the up/down button of the control are visible.
        /// </summary>
        /// <remarks>
        /// If the value is false then the value of the control can be changed only if one of the following cases is satisfied:
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
                                          typeof(NumericUpDownBase),
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
                                          typeof(NumericUpDownBase),
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
                                          typeof(NumericUpDownBase),
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

        /// <summary>Identifies the <see cref="ButtonUpContent"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonUpContentProperty
            = DependencyProperty.Register(nameof(ButtonUpContent),
                                          typeof(object),
                                          typeof(NumericUpDownBase),
                                          new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Provides the object content that should be displayed on the Up Button.
        /// </summary>
        [Category(AppName.MahApps)]
        public object? ButtonUpContent
        {
            get => (object?)this.GetValue(ButtonUpContentProperty);
            set => this.SetValue(ButtonUpContentProperty, value);
        }

        /// <summary>Identifies the <see cref="ButtonUpContentTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonUpContentTemplateProperty
            = DependencyProperty.Register(nameof(ButtonUpContentTemplate),
                                          typeof(DataTemplate),
                                          typeof(NumericUpDownBase));

        /// <summary>
        /// Gets or sets the DataTemplate used to display the Up button's content.
        /// </summary>
        [Category(AppName.MahApps)]
        public DataTemplate? ButtonUpContentTemplate
        {
            get => (DataTemplate?)this.GetValue(ButtonUpContentTemplateProperty);
            set => this.SetValue(ButtonUpContentTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="ButtonUpContentStringFormat"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonUpContentStringFormatProperty
            = DependencyProperty.Register(nameof(ButtonUpContentStringFormat),
                                          typeof(string),
                                          typeof(NumericUpDownBase),
                                          new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets a composite string that specifies how to format the ButtonUpContent property if it is displayed as a string.
        /// </summary>
        /// <remarks>
        /// This property is ignored if <seealso cref="ButtonUpContentTemplate"/> is set.
        /// </remarks>
        [Bindable(true)]
        [Category(AppName.MahApps)]
        public string? ButtonUpContentStringFormat
        {
            get => (string?)this.GetValue(ButtonUpContentStringFormatProperty);
            set => this.SetValue(ButtonUpContentStringFormatProperty, value);
        }

        /// <summary>Identifies the <see cref="ButtonDownContent"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonDownContentProperty
            = DependencyProperty.Register(nameof(ButtonDownContent),
                                          typeof(object),
                                          typeof(NumericUpDownBase),
                                          new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Provides the object content that should be displayed on the Down Button.
        /// </summary>
        [Category(AppName.MahApps)]
        public object? ButtonDownContent
        {
            get => (object?)this.GetValue(ButtonDownContentProperty);
            set => this.SetValue(ButtonDownContentProperty, value);
        }

        /// <summary>Identifies the <see cref="ButtonDownContentTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonDownContentTemplateProperty
            = DependencyProperty.Register(nameof(ButtonDownContentTemplate),
                                          typeof(DataTemplate),
                                          typeof(NumericUpDownBase));

        /// <summary>
        /// Gets or sets the DataTemplate used to display the Down button's content.
        /// </summary>
        [Category(AppName.MahApps)]
        public DataTemplate? ButtonDownContentTemplate
        {
            get => (DataTemplate?)this.GetValue(ButtonDownContentTemplateProperty);
            set => this.SetValue(ButtonDownContentTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="ButtonDownContentStringFormat"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonDownContentStringFormatProperty
            = DependencyProperty.Register(nameof(ButtonDownContentStringFormat),
                                          typeof(string),
                                          typeof(NumericUpDownBase),
                                          new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets a composite string that specifies how to format the ButtonDownContent property if it is displayed as a string.
        /// </summary>
        /// <remarks>
        /// This property is ignored if <seealso cref="ButtonDownContentTemplate"/> is set.
        /// </remarks>
        [Bindable(true)]
        [Category(AppName.MahApps)]
        public string? ButtonDownContentStringFormat
        {
            get => (string?)this.GetValue(ButtonDownContentStringFormatProperty);
            set => this.SetValue(ButtonDownContentStringFormatProperty, value);
        }

        /// <summary>Identifies the <see cref="Culture"/> dependency property.</summary>
        public static readonly DependencyProperty CultureProperty
            = DependencyProperty.Register(nameof(Culture),
                                          typeof(CultureInfo),
                                          typeof(NumericUpDownBase),
                                          new PropertyMetadata(null, OnCulturePropertyChanged));

        private static void OnCulturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && d is NumericUpDownBase numericUpDown)
            {
                numericUpDown.regexNumber = null;
                numericUpDown.RefreshFromCurrentValue();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the culture to be used in string formatting and converting operations.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(null)]
        public CultureInfo? Culture
        {
            get => (CultureInfo?)this.GetValue(CultureProperty);
            set => this.SetValue(CultureProperty, value);
        }

        /// <summary>Identifies the <see cref="NumericInputMode"/> dependency property.</summary>
        public static readonly DependencyProperty NumericInputModeProperty
            = DependencyProperty.Register(nameof(NumericInputMode),
                                          typeof(NumericInput),
                                          typeof(NumericUpDownBase),
                                          new FrameworkPropertyMetadata(NumericInput.All, OnNumericInputModePropertyChanged));

        private static void OnNumericInputModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is NumericInput numericInput && d is NumericUpDownBase numericUpDown)
            {
                if (!numericInput.HasFlag(NumericInput.Decimal))
                {
                    numericUpDown.TruncateValue();
                }
            }
        }

        /// <summary>
        /// Gets or sets which numeric input for this NumericUpDownBase is allowed.
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
                                          typeof(NumericUpDownBase),
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
                                          typeof(NumericUpDownBase),
                                          new PropertyMetadata(BooleanBoxes.FalseBox, OnSnapToMultipleOfIntervalPropertyChanged));

        private static void OnSnapToMultipleOfIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is bool snap && d is NumericUpDownBase numericUpDown)
            {
                if (!snap)
                {
                    return;
                }

                numericUpDown.SnapValueToInterval();
            }
        }

        /// <summary>
        /// Indicates if the NumericUpDownBase should round the value to the nearest possible interval when the focus moves to another element.
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
                                          typeof(NumericUpDownBase),
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

        private protected CultureInfo SpecificCultureInfo => this.Culture ?? this.Language.GetSpecificCulture();

        static NumericUpDownBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDownBase), new FrameworkPropertyMetadata(typeof(NumericUpDownBase)));

            VerticalContentAlignmentProperty.OverrideMetadata(typeof(NumericUpDownBase), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(NumericUpDownBase), new FrameworkPropertyMetadata(HorizontalAlignment.Right));

        }

        /// <summary>
        /// What <see cref="Control.IsTabStop"/> was before the text box took the focus, kept so it can be
        /// given back exactly as the consumer left it.
        /// </summary>
        private bool? tabStopWhileTheTextBoxHadTheFocus;

        /// <summary>
        /// Sends the keyboard focus on to the text box, since that is the only part of this control
        /// anything can be typed into, and takes the control out of the tab order for as long as the
        /// text box holds it.
        /// </summary>
        /// <remarks>
        /// This is answered on the way in rather than after the fact. Handling GotFocus only catches
        /// the focus entering from outside, and the focus manager putting it back on the control when
        /// a window is activated is not that: it never left. It also used to move on from whatever
        /// held the focus at the time, which is not necessarily this control, so the focus could land
        /// anywhere at all.
        /// </remarks>
        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnPreviewGotKeyboardFocus(e);

            if (!e.Handled
                && ReferenceEquals(e.NewFocus, this)
                && this.valueTextBox is not null
                && (this.InterceptManualEnter || this.IsReadOnly)
                )
            {
                e.Handled = true;
                Keyboard.Focus(this.valueTextBox);
                return;
            }

            this.StandInTheTabOrder(!ReferenceEquals(e.NewFocus, this.valueTextBox));
        }

        /// <inheritdoc />
        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);

            if (!(bool)e.NewValue)
            {
                this.StandInTheTabOrder(true);
            }
        }

        /// <summary>
        /// Whether tab navigation may stop on the control itself. It may not while the text box has the
        /// focus, or tab and shift tab would carry the focus out of the text box, up to the control, and
        /// straight back into the text box by the line above. Out of the tab order, the control is
        /// passed over and the focus reaches the neighbour, which is where it was going.
        /// </summary>
        private void StandInTheTabOrder(bool stand)
        {
            if (stand)
            {
                if (this.tabStopWhileTheTextBoxHadTheFocus is { } asItWas)
                {
                    this.tabStopWhileTheTextBoxHadTheFocus = null;
                    this.SetCurrentValue(IsTabStopProperty, BooleanBoxes.Box(asItWas));
                }
            }
            else if (this.tabStopWhileTheTextBoxHadTheFocus is null)
            {
                this.tabStopWhileTheTextBoxHadTheFocus = this.IsTabStop;
                this.SetCurrentValue(IsTabStopProperty, BooleanBoxes.FalseBox);
            }
        }

        /// <summary>
        ///     When overridden in a derived class, is invoked whenever application code or internal processes call
        ///     <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        /// <summary>
        /// What the control needs from the type it holds. Everything above this line describes how the
        /// control looks and behaves and says nothing about numbers.
        /// </summary>
        protected abstract bool ValidateText(string text);

        /// <summary>Takes what is in the text box and turns it into the value.</summary>
        protected abstract void ChangeValueFromTextInput(string? text);

        /// <summary>Steps the value up or down, faster the longer a button is held.</summary>
        protected abstract void ChangeValueWithSpeedUp(bool toPositive);

        /// <summary>Puts the speed up back to where it starts.</summary>
        protected abstract void ResetSpeedUp();

        /// <summary>Writes the value into the text box.</summary>
        protected abstract void RefreshTextFromValue();

        /// <summary>Called once the template is in place and the parts are known.</summary>
        /// <summary>Runs the current value through again, after something around it changed.</summary>
        protected abstract void RefreshFromCurrentValue();

        /// <summary>
        /// Reads the part of a text that looks like a number, the way the culture writes one, and
        /// hands back what it found. A text with nothing number-shaped in it comes back unchanged.
        /// </summary>
        protected abstract string TakeNumberFrom(string text);

        /// <summary>Whether a value is set at all. The value itself is a matter for the typed half.</summary>
        public abstract bool HasValue { get; }

        /// <summary>Sets the value back to the default value, which is what the clear button does.</summary>
        public abstract void Clear();

        /// <summary>
        /// Raised whenever the value changed, saying nothing about what it now is. The typed half has
        /// an event carrying the value, but that one is a different event per type, so the parts that
        /// only need to know that something happened cannot subscribe to it.
        /// </summary>
        internal event EventHandler? ValueChangedInternal;

        /// <summary>Tells everyone listening that the value changed.</summary>
        protected void RaiseValueChangedInternal()
        {
            this.ValueChangedInternal?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Drops everything behind the decimal separator, for a control that takes no decimals.</summary>
        protected abstract void TruncateValue();

        /// <summary>Rounds the value to the nearest multiple of the interval.</summary>
        protected abstract void SnapValueToInterval();

        /// <summary>Turns the up and down buttons on or off for where the value stands.</summary>
        protected abstract void EnableDisableUpDown();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Detach the handlers of a previous template pass. Without this, every further pass
            // leaves another handler on the same button and one click changes the value once per pass.
            if (this.repeatUp is not null)
            {
                this.repeatUp.Click -= this.OnRepeatUpClick;
                this.repeatUp.PreviewMouseUp -= this.OnRepeatButtonPreviewMouseUp;
            }

            if (this.repeatDown is not null)
            {
                this.repeatDown.Click -= this.OnRepeatDownClick;
                this.repeatDown.PreviewMouseUp -= this.OnRepeatButtonPreviewMouseUp;
            }

            this.repeatUp = this.GetTemplateChild(PART_NumericUp) as RepeatButton;
            this.repeatDown = this.GetTemplateChild(PART_NumericDown) as RepeatButton;

            this.valueTextBox = this.GetTemplateChild(PART_TextBox) as TextBox;

            if (this.repeatUp is null || this.repeatDown is null || this.valueTextBox is null)
            {
                throw new InvalidOperationException($"You have missed to specify {PART_NumericUp}, {PART_NumericDown} or {PART_TextBox} in your template!");
            }

            this.ToggleReadOnlyMode(this.IsReadOnly);

            // Named methods, so that a later template pass can detach them again.
            this.repeatUp.Click += this.OnRepeatUpClick;
            this.repeatDown.Click += this.OnRepeatDownClick;

            this.repeatUp.PreviewMouseUp += this.OnRepeatButtonPreviewMouseUp;
            this.repeatDown.PreviewMouseUp += this.OnRepeatButtonPreviewMouseUp;

            this.RefreshFromCurrentValue();

            this.scrollViewer = null;
        }

        private void OnRepeatUpClick(object sender, RoutedEventArgs e)
        {
            this.ChangeValueWithSpeedUp(true);
        }

        private void OnRepeatDownClick(object sender, RoutedEventArgs e)
        {
            this.ChangeValueWithSpeedUp(false);
        }

        private void OnRepeatButtonPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.ResetInternal();
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new NumericUpdDownAutomationPeer(this);
        }

        private void ToggleReadOnlyMode(bool isReadOnly)
        {
            if (this.repeatUp is null || this.repeatDown is null || this.valueTextBox is null)
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

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (!this.InterceptArrowKeys)
            {
                return;
            }

            if (e.Key == Key.Up)
            {
                this.ChangeValueWithSpeedUp(true);
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                this.ChangeValueWithSpeedUp(false);
                e.Handled = true;
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

            if (this.InterceptMouseWheel && (this.IsFocused || this.valueTextBox?.IsFocused == true || this.TrackMouseWheelWhenMouseOver))
            {
                bool increment = e.Delta > 0;
                this.ChangeValueWithSpeedUp(increment);
            }

            var sv = this.TryFindScrollViewer();

            if (sv != null && this.handlesMouseWheelScrolling.Value is not null)
            {
                if (this.TrackMouseWheelWhenMouseOver)
                {
                    this.handlesMouseWheelScrolling.Value.SetValue(sv, true, null);
                }
                else if (this.InterceptMouseWheel)
                {
                    this.handlesMouseWheelScrolling.Value.SetValue(sv, this.valueTextBox?.IsFocused == true, null);
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
            var textIsValid = this.ValidateText(fullText);
            // Only the text format decides here. A number that is out of range on its own is
            // still the beginning of an in-range one, so the keystroke must not be swallowed.
            // Coercion happens when the value is committed.
            e.Handled = !textIsValid;
            this.manualChange = !e.Handled;
        }

        private ScrollViewer? TryFindScrollViewer()
        {
            if (this.scrollViewer != null)
            {
                return this.scrollViewer;
            }

            this.valueTextBox?.ApplyTemplate();

            this.scrollViewer = this.valueTextBox?.Template.FindName(PART_ContentHost, this.valueTextBox) as ScrollViewer;
            if (this.scrollViewer != null)
            {
                this.handlesMouseWheelScrolling = new Lazy<PropertyInfo?>(() => this.scrollViewer.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).SingleOrDefault(i => i.Name == "HandlesMouseWheelScrolling"));
            }

            return this.scrollViewer;
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
        private static void SimulateDecimalPointKeyPress(TextBoxBase textBox, DecimalPointCorrectionMode mode, CultureInfo culture)
        {
            // Select the proper decimal-point string upon the context
            string? replace;
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
        }

        private void OnTextBoxLostFocus(object? sender, RoutedEventArgs e)
        {
            if (this.valueTextBox is not null)
            {
                this.manualChange = false;
                this.valueTextBox.TextChanged -= this.OnTextChanged;
                this.RefreshTextFromValue();
                this.valueTextBox.TextChanged += this.OnTextChanged;
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var text = ((TextBox)sender).Text;
            this.ChangeValueFromTextInput(text);
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

            var text = e.SourceDataObject.GetData(DataFormats.Text) as string ?? string.Empty;

            // Only the number is taken out of what arrives. The rest would sit in the field until the
            // focus leaves and then be dropped anyway, since the value never held it.
            var number = this.TakeNumberFrom(text);

            var newText = string.Concat(textPresent.Substring(0, textBox.SelectionStart), number, textPresent.Substring(textBox.SelectionStart + textBox.SelectionLength));
            if (!this.ValidateText(newText))
            {
                e.CancelCommand();
                return;
            }

            if (!string.Equals(number, text, StringComparison.Ordinal))
            {
                e.DataObject = new DataObject(DataFormats.Text, number);
            }

            this.manualChange = true;
        }

        /// <summary>Puts the speed up back to where it starts, unless the control is read only.</summary>
        protected void ResetInternal()
        {
            if (this.IsReadOnly)
            {
                return;
            }

            this.ResetSpeedUp();
        }

    }
}