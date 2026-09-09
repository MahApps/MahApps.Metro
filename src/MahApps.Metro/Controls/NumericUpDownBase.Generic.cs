// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using JetBrains.Annotations;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The half of the control that knows what kind of number it holds: the value and its bounds, how
    /// text becomes a number and a number becomes text, and what a step up or down does.
    /// </summary>
    /// <typeparam name="T">The type of the value, for instance <see cref="double"/> or <see cref="decimal"/>.</typeparam>
    /// <remarks>
    /// A type deriving from this has to bring the handful of operations below, since C# has no way to
    /// add two values of an open generic type. It also has to say what its bounds and its step are,
    /// through OverrideMetadata in its static constructor, because the defaults here cannot know them.
    /// </remarks>
    public abstract class NumericUpDownBase<T> : NumericUpDownBase
        where T : struct, IComparable<T>, IFormattable
    {
        private T internalIntervalMultiplierForCalculation;
        private T internalLargeChange;
        private T intervalValueSinceReset;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <remarks>
        /// The three fields above start from the interval, which a field initializer cannot say for
        /// an open generic type: it has no literal one to write and no way to multiply. Without this
        /// they stay at zero until something changes the interval, and every step taken before that,
        /// a held button included, moves the value by nothing at all.
        /// </remarks>
        protected NumericUpDownBase()
        {
            this.ResetSpeedUp();
        }

        #region What the type has to answer

        /// <summary>Reads a value out of text, the way the culture writes it.</summary>
        protected abstract bool TryParse(string text, NumberStyles style, IFormatProvider provider, out T value);

        /// <summary>Adds two values.</summary>
        protected abstract T Add(T left, T right);

        /// <summary>Scales a value, for the steps that grow while a button is held.</summary>
        protected abstract T Multiply(T value, double factor);

        /// <summary>Divides a value, for a format that shows percent or per mille.</summary>
        protected abstract T Divide(T value, double divisor);

        /// <summary>Drops everything behind the decimal separator.</summary>
        protected abstract T Truncate(T value);

        /// <summary>Rounds a value to the nearest multiple of another, for SnapToMultipleOfInterval.</summary>
        protected abstract T RoundToMultiple(T value, T multiple);

        /// <summary>Orders two values, the way <see cref="IComparable{T}.CompareTo"/> does.</summary>
        protected abstract int Compare(T left, T right);

        /// <summary>Whether a value is zero, which is what an interval must not be to snap to.</summary>
        protected abstract bool IsZero(T value);

        /// <summary>Writes a value without a format of its own, for the text box.</summary>
        protected abstract string ToPlainString(T value, CultureInfo culture);

        /// <summary>Carries a value over into a double, for the parts that measure rather than count.</summary>
        protected abstract double ToDouble(T value);

        /// <summary>The other way around.</summary>
        protected abstract T FromDouble(double value);

        #endregion

        #region What the control asks of the value

        /// <inheritdoc />
        protected override bool ValidateText(string text)
        {
            return this.ValidateText(text, out _);
        }

        /// <inheritdoc />
        protected override void ChangeValueFromTextInput(string? text)
        {
            this.ChangeValueFromTextInputCore(text ?? string.Empty);
        }

        /// <inheritdoc />
        protected override void ChangeValueWithSpeedUp(bool toPositive)
        {
            this.ChangeValueWithSpeedUpCore(toPositive);
        }

        /// <inheritdoc />
        protected override void ResetSpeedUp()
        {
            this.internalLargeChange = this.Multiply(this.Interval, 100d);
            this.internalIntervalMultiplierForCalculation = this.Interval;
            this.intervalValueSinceReset = default;
        }

        /// <inheritdoc />
        protected override void RefreshTextFromValue()
        {
            this.InternalSetText(this.Value);
        }

        /// <inheritdoc />
        protected override void RefreshFromCurrentValue()
        {
            this.OnValueChanged(this.Value, this.Value);
        }

        /// <inheritdoc />
        protected override string TakeNumberFrom(string text)
        {
            return this.TryGetNumberFromText(text, this.ReadsHexadecimal);
        }

        /// <inheritdoc />
        public override bool HasValue => this.Value.HasValue;

        /// <inheritdoc />
        public override void Clear()
        {
            this.SetCurrentValue(ValueProperty, this.DefaultValue);
            this.GetBindingExpression(ValueProperty)?.UpdateSource();
        }

        /// <inheritdoc />
        protected override void TruncateValue()
        {
            if (this.Value.HasValue)
            {
                this.SetCurrentValue(ValueProperty, this.Truncate(this.Value.Value));
            }
        }

        /// <inheritdoc />
        protected override void SnapValueToInterval()
        {
            if (this.IsZero(this.Interval))
            {
                return;
            }

            this.SetCurrentValue(ValueProperty, this.RoundToMultiple(this.Value.GetValueOrDefault(), this.Interval));
        }

        /// <summary>
        /// This method is invoked when the Maximum property changes.
        /// </summary>
        /// <param name="oldMaximum">The old value of the Maximum property.</param>
        /// <param name="newMaximum">The new value of the Maximum property.</param>
        protected virtual void OnMaximumChanged(T oldMaximum, T newMaximum)
        {
            // nothing here
        }

        /// <summary>
        /// This method is invoked when the Minimum property changes.
        /// </summary>
        /// <param name="oldMinimum">The old value of the Minimum property.</param>
        /// <param name="newMinimum">The new value of the Minimum property.</param>
        protected virtual void OnMinimumChanged(T oldMinimum, T newMinimum)
        {
            // nothing here
        }

        #endregion

        /// <summary>Identifies the <see cref="ValueChanged"/> routed event.</summary>
        public static readonly RoutedEvent ValueChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(ValueChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedPropertyChangedEventHandler<T?>),
                                               typeof(NumericUpDownBase<T>));

        /// <summary>
        /// Add / Remove ValueChangedEvent handler
        /// Event which will be fired from this NumericUpDownBase when its value has been changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<T?> ValueChanged
        {
            add => this.AddHandler(ValueChangedEvent, value);
            remove => this.RemoveHandler(ValueChangedEvent, value);
        }

        /// <summary>Identifies the <see cref="Value"/> dependency property.</summary>
        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.Register(nameof(Value),
                                          typeof(T?),
                                          typeof(NumericUpDownBase<T>),
                                          new FrameworkPropertyMetadata(default(T?),
                                                                        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                        OnValuePropertyChanged,
                                                                        (o, value) => CoerceValue(o, value).value));

        private static void OnValuePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                (dependencyObject as NumericUpDownBase<T>)?.OnValueChanged((T?)e.OldValue, (T?)e.NewValue);
            }
        }

        [MustUseReturnValue]
        private static (T? value, bool isValid) CoerceValue(DependencyObject d, object? baseValue)
        {
            var numericUpDown = (NumericUpDownBase<T>)d;
            if (baseValue is null)
            {
                return (numericUpDown.DefaultValue, false);
            }

            var value = ((T?)baseValue).Value;

            if (!numericUpDown.NumericInputMode.HasFlag(NumericInput.Decimal))
            {
                value = numericUpDown.Truncate(value);
            }

            if (numericUpDown.Compare(value, numericUpDown.Minimum) < 0)
            {
                return (numericUpDown.Minimum, false);
            }

            if (numericUpDown.Compare(value, numericUpDown.Maximum) > 0)
            {
                return (numericUpDown.Maximum, false);
            }

            return (value, true);
        }

        /// <summary>
        /// Gets or sets the value of the control.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(null)]
        public T? Value
        {
            get => (T?)this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        /// <summary>Identifies the <see cref="DefaultValue"/> dependency property.</summary>
        public static readonly DependencyProperty DefaultValueProperty
            = DependencyProperty.Register(nameof(DefaultValue),
                                          typeof(T?),
                                          typeof(NumericUpDownBase<T>),
                                          new PropertyMetadata(null, OnDefaultValuePropertyChanged, CoerceDefaultValue));

        private static void OnDefaultValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDownBase<T>)d;

            if (!numericUpDown.Value.HasValue && numericUpDown.DefaultValue.HasValue)
            {
                numericUpDown.SetValueTo(numericUpDown.DefaultValue.Value);
            }
        }

        [MustUseReturnValue]
        private static object? CoerceDefaultValue(DependencyObject d, object? baseValue)
        {
            if (baseValue is T val && d is NumericUpDownBase<T> numericUpDown)
            {
                var minimum = numericUpDown.Minimum;
                var maximum = numericUpDown.Maximum;

                if (numericUpDown.Compare(val, minimum) < 0)
                {
                    return minimum;
                }

                if (numericUpDown.Compare(val, maximum) > 0)
                {
                    return maximum;
                }
            }

            return baseValue;
        }

        /// <summary>
        /// Gets or sets the default value of the control which will be used if the <see cref="Value"/> is <see langword="null"/>.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(null)]
        public T? DefaultValue
        {
            get => (T?)this.GetValue(DefaultValueProperty);
            set => this.SetValue(DefaultValueProperty, value);
        }

        /// <summary>Identifies the <see cref="Minimum"/> dependency property.</summary>
        public static readonly DependencyProperty MinimumProperty
            = DependencyProperty.Register(nameof(Minimum),
                                          typeof(T),
                                          typeof(NumericUpDownBase<T>),
                                          new FrameworkPropertyMetadata(default(T), OnMinimumPropertyChanged));

        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDownBase<T>)d;

            numericUpDown.CoerceValue(MaximumProperty);
            numericUpDown.CoerceValue(ValueProperty);
            numericUpDown.CoerceValue(DefaultValueProperty);
            numericUpDown.OnMinimumChanged((T)e.OldValue, (T)e.NewValue);
            numericUpDown.EnableDisableUpDown();
        }

        /// <summary>
        /// Minimum restricts the minimum value of the Value property.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        public T Minimum
        {
            get => (T)this.GetValue(MinimumProperty);
            set => this.SetValue(MinimumProperty, value);
        }

        /// <summary>Identifies the <see cref="Maximum"/> dependency property.</summary>
        public static readonly DependencyProperty MaximumProperty
            = DependencyProperty.Register(nameof(Maximum),
                                          typeof(T),
                                          typeof(NumericUpDownBase<T>),
                                          new FrameworkPropertyMetadata(default(T), OnMaximumPropertyChanged, CoerceMaximum));

        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDownBase<T>)d;

            numericUpDown.CoerceValue(ValueProperty);
            numericUpDown.CoerceValue(DefaultValueProperty);
            numericUpDown.OnMaximumChanged((T)e.OldValue, (T)e.NewValue);
            numericUpDown.EnableDisableUpDown();
        }

#pragma warning disable WPF0024
        [MustUseReturnValue]
        private static object CoerceMaximum(DependencyObject d, object value)
#pragma warning restore WPF0024
        {
            var numericUpDown = (NumericUpDownBase<T>)d;
            var minimum = numericUpDown.Minimum;
            var val = (T)value;
            return numericUpDown.Compare(val, minimum) < 0 ? minimum : val;
        }

        /// <summary>
        /// Maximum restricts the maximum value of the Value property.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        public T Maximum
        {
            get => (T)this.GetValue(MaximumProperty);
            set => this.SetValue(MaximumProperty, value);
        }

        /// <summary>Identifies the <see cref="Interval"/> dependency property.</summary>
        public static readonly DependencyProperty IntervalProperty
            = DependencyProperty.Register(nameof(Interval),
                                          typeof(T),
                                          typeof(NumericUpDownBase<T>),
                                          new FrameworkPropertyMetadata(default(T), OnIntervalPropertyChanged));

        private static void OnIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NumericUpDownBase<T>)?.ResetInternal();
        }

        /// <summary>
        /// Gets or sets the interval value for increasing/decreasing the <see cref="Value" /> .
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        public T Interval
        {
            get => (T)this.GetValue(IntervalProperty);
            set => this.SetValue(IntervalProperty, value);
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
        protected virtual void OnValueChanged(T? oldValue, T? newValue)
        {
            if (!this.manualChange)
            {
                if (!newValue.HasValue)
                {
                    if (this.valueTextBox != null)
                    {
                        this.valueTextBox.Text = null;
                    }

                    this.EnableDisableUpDown();
                    this.RaiseValueChangedInternal();

                    if (!Nullable.Equals(oldValue, newValue))
                    {
                        this.RaiseEvent(new RoutedPropertyChangedEventArgs<T?>(oldValue, newValue, ValueChangedEvent));
                    }

                    return;
                }

                this.repeatUp?.SetCurrentValue(RepeatButton.IsEnabledProperty, BooleanBoxes.TrueBox);
                this.repeatDown?.SetCurrentValue(RepeatButton.IsEnabledProperty, BooleanBoxes.TrueBox);

                if (this.Compare(newValue.Value, this.Minimum) <= 0)
                {
                    this.repeatDown?.SetCurrentValue(RepeatButton.IsEnabledProperty, BooleanBoxes.FalseBox);

                    this.ResetInternal();

                    if (this.IsLoaded)
                    {
                        this.RaiseEvent(new RoutedEventArgs(MinimumReachedEvent));
                    }
                }

                if (this.Compare(newValue.Value, this.Maximum) >= 0)
                {
                    this.repeatUp?.SetCurrentValue(RepeatButton.IsEnabledProperty, BooleanBoxes.FalseBox);

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
            else if (this.SyncTextWithValueWhileEditing && this.valueTextBox != null)
            {
                var textRepresentsValue = newValue.HasValue
                                          && this.ValidateText(this.valueTextBox.Text, out var textValue)
                                          && this.Compare(this.FormattedValue(textValue, this.StringFormat, this.SpecificCultureInfo), newValue.Value) == 0;

                if (!textRepresentsValue)
                {
                    this.InternalSetText(newValue);

                    if (this.valueTextBox.IsKeyboardFocused)
                    {
                        this.valueTextBox.SelectAll();
                    }
                }
            }

            this.EnableDisableUpDown();
            this.RaiseValueChangedInternal();

            if (!Nullable.Equals(oldValue, newValue))
            {
                this.RaiseEvent(new RoutedPropertyChangedEventArgs<T?>(oldValue, newValue, ValueChangedEvent));
            }
        }

        private void InternalSetText(T? newValue)
        {
            if (!newValue.HasValue)
            {
                if (this.valueTextBox is not null)
                {
                    this.valueTextBox.Text = null;
                }

                return;
            }

            if (this.valueTextBox is not null)
            {
                this.valueTextBox.Text = this.FormattedValueString(newValue.Value, this.StringFormat, this.SpecificCultureInfo);
            }

            if ((bool)this.GetValue(TextBoxHelper.IsMonitoringProperty))
            {
                var textLength = this.valueTextBox?.Text?.Length ?? 0;
                this.SetValue(TextBoxHelper.TextLengthPropertyKey, textLength);
            }
        }

        /// <summary>
        /// Writes a value the way this control would show it, for whoever has to show the same thing
        /// without being one. The two parts only the type can answer, what a value looks like with no
        /// format at all and what it looks like in hexadecimal, are in here and nowhere else.
        /// </summary>
        internal string? TextFor(T? value, string format, CultureInfo culture)
        {
            return value.HasValue ? this.FormattedValueString(value.Value, format, culture) : null;
        }

        private string? FormattedValueString(T newValue, string format, CultureInfo culture)
        {
            format = format.Replace("{}", string.Empty);
            if (!string.IsNullOrWhiteSpace(format))
            {
                if (this.TryFormatHexadecimal(newValue, format, culture, out var hexValue))
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

            return this.ToPlainString(newValue, culture);
        }

        private T FormattedValue(T newValue, string format, CultureInfo culture)
        {
            format = format.Replace("{}", string.Empty);
            if (!string.IsNullOrWhiteSpace(format))
            {
                if (!this.TryFormatHexadecimal(newValue, format, culture, out _))
                {
                    var match = RegexStringFormat.Match(format);
                    if (match.Success)
                    {
                        // we have a format template such as "{0:N0}"
                        return this.ConvertStringFormatValue(newValue, match.Groups["format"].Value);
                    }

                    // we have a format such as "N0"
                    return this.ConvertStringFormatValue(newValue, format);
                }
            }

            return newValue;
        }

        private T ConvertStringFormatValue(T value, string format)
        {
            if (format.ToUpperInvariant().Contains("P") || format.Contains("%"))
            {
                value = this.Divide(value, 100d);
            }
            else if (format.Contains("‰"))
            {
                value = this.Divide(value, 1000d);
            }

            return value;
        }

        private bool TryFormatHexadecimal(T newValue, string format, CultureInfo culture, [NotNullWhen(true)] out string? output)
        {
            var match = RegexStringFormatHexadecimal.Match(format);
            if (match.Success)
            {
                // HEX DOES SUPPORT INTEGRAL TYPES ONLY. Inside the int range the operand stays an
                // int, so negative values keep their 32 bit form (-1 renders as "ffffffff").
                // Outside it, the cast to int saturates, which rendered 3e9 as 7FFFFFFF. A value
                // beyond the long range still saturates, there is no integral type left for it.
                var asDouble = this.ToDouble(newValue);
                var hexOperand = asDouble >= int.MinValue && asDouble <= int.MaxValue
                                     ? (object)(int)asDouble
                                     : (long)asDouble;

                if (match.Groups["simpleHEX"].Success)
                {
                    output = ((IFormattable)hexOperand).ToString(match.Groups["simpleHEX"].Value, culture);
                    return true;
                }

                if (match.Groups["complexHEX"].Success)
                {
                    output = string.Format(culture, match.Groups["complexHEX"].Value, hexOperand);
                    return true;
                }
            }

            output = null;
            return false;
        }

        private void ChangeValueWithSpeedUpCore(bool toPositive)
        {
            if (this.IsReadOnly)
            {
                return;
            }

            if (this.Speedup)
            {
                var largest = this.Multiply(this.internalLargeChange, this.ToDouble(this.Interval));
                this.intervalValueSinceReset = this.Add(this.intervalValueSinceReset, this.Multiply(this.internalIntervalMultiplierForCalculation, this.ToDouble(this.Interval)));

                if (this.Compare(this.intervalValueSinceReset, largest) > 0)
                {
                    this.internalLargeChange = this.Multiply(this.internalLargeChange, 10d);
                    this.internalIntervalMultiplierForCalculation = this.Multiply(this.internalIntervalMultiplierForCalculation, 10d);
                }

                this.ChangeValueInternal(toPositive, this.internalIntervalMultiplierForCalculation);
            }
            else
            {
                this.ChangeValueInternal(toPositive, this.Interval);
            }
        }

        private void ChangeValueInternal(bool toPositive, T amount)
        {
            if (this.IsReadOnly)
            {
                return;
            }

            this.manualChange = false;

var interval = this.ToDouble(amount) * (toPositive ? 1d : -1d);
            var routedEvent = toPositive
                ? new NumericUpDownChangedRoutedEventArgs(ValueIncrementedEvent, interval)
                : new NumericUpDownChangedRoutedEventArgs(ValueDecrementedEvent, interval);

            this.RaiseEvent(routedEvent);

            if (!routedEvent.Handled)
            {
                this.ChangeValueBy(this.FromDouble(routedEvent.Interval));

                this.InternalSetText(this.Value);

                if (this.valueTextBox is not null)
                {
                    this.valueTextBox.CaretIndex = this.valueTextBox.Text.Length;
                }
            }
        }

        private void ChangeValueBy(T difference)
        {
            var newValue = this.Add(this.Value.GetValueOrDefault(), difference);
            this.SetValueTo(newValue);
        }

        private void SetValueTo(T newValue)
        {
            var value = newValue;

            if (this.SnapToMultipleOfInterval && !this.IsZero(this.Interval))
            {
                value = this.RoundToMultiple(newValue, this.Interval);
            }

            if (this.Compare(value, this.Maximum) > 0)
            {
                value = this.Maximum;
            }
            else if (this.Compare(value, this.Minimum) < 0)
            {
                value = this.Minimum;
            }

            this.SetCurrentValue(ValueProperty, CoerceValue(this, value).value);
        }

        protected override void EnableDisableUpDown()
        {
            this.repeatUp?.SetCurrentValue(RepeatButton.IsEnabledProperty, BooleanBoxes.Box(this.Value is null || this.Compare(this.Value.Value, this.Maximum) < 0));
            this.repeatDown?.SetCurrentValue(RepeatButton.IsEnabledProperty, BooleanBoxes.Box(this.Value is null || this.Compare(this.Value.Value, this.Minimum) > 0));
        }

        private void ChangeValueFromTextInputCore(string text)
        {
            if (!this.InterceptManualEnter)
            {
                return;
            }

            var oldValue = this.Value;

            if (string.IsNullOrEmpty(text))
            {
                if (this.DefaultValue.HasValue)
                {
                    this.SetValueTo(this.DefaultValue.Value);
                    if (!this.manualChange)
                    {
                        this.InternalSetText(this.DefaultValue.Value);
                    }
                }
                else
                {
                    this.SetCurrentValue(ValueProperty, null);
                }
            }
            else if (this.manualChange)
            {
                if (this.ValidateText(text, out var convertedValue))
                {
                    convertedValue = this.FormattedValue(convertedValue, this.StringFormat, this.SpecificCultureInfo);
                    this.SetValueTo(convertedValue);
                }
                else if (this.DefaultValue.HasValue)
                {
                    this.SetValueTo(this.DefaultValue.Value);
                    this.InternalSetText(oldValue);
                }
                else
                {
                    this.SetCurrentValue(ValueProperty, null);
                }
            }

            // No OnValueChanged call here: setting ValueProperty above already ran the property
            // changed callback, which raises ValueChanged. Calling it again reported every
            // change to the consumer twice.
            this.manualChange = false;
        }

        /// <summary>
        /// The characters that count as a sign here: the ones the culture uses, and the ASCII hyphen
        /// and plus on top of them. Several cultures sign a number with characters no keyboard has,
        /// Norwegian with U+2212 for one, so what is typed has to be accepted as well.
        /// </summary>
        private string SignCharacters
        {
            get
            {
                var signs = "-+"
                            + this.SpecificCultureInfo.NumberFormat.NegativeSign
                            + this.SpecificCultureInfo.NumberFormat.PositiveSign;

                return new string(signs.Distinct().ToArray());
            }
        }

        private bool IsSign(string text)
        {
            return text.Length == 1 && this.SignCharacters.Contains(text[0]);
        }

        /// <summary>
        /// Puts the sign of the culture in front of a number that carries the hyphen or the plus. What
        /// a runtime accepts for a culture whose sign is neither of those differs between frameworks,
        /// so the text is made to match the culture before it is parsed rather than after.
        /// </summary>
        private string WithSignOfCulture(string text)
        {
            if (text.Length == 0)
            {
                return text;
            }

            var format = this.SpecificCultureInfo.NumberFormat;

            if (text[0] == '-' && format.NegativeSign != "-")
            {
                return format.NegativeSign + text.Substring(1);
            }

            if (text[0] == '+' && format.PositiveSign != "+")
            {
                return format.PositiveSign + text.Substring(1);
            }

            return text;
        }

        private bool ValidateText(string text, out T convertedValue)
        {
            convertedValue = default;

            if (this.IsSign(text))
            {
                return true;
            }

            if (text.Count(c => this.SignCharacters.Contains(c)) > 2)
            {
                return false;
            }

            var isNumeric = this.ReadsHexadecimal
                            || this.ParsingNumberStyle == NumberStyles.Integer
                            || this.ParsingNumberStyle == NumberStyles.Number;

            var number = this.WithSignOfCulture(this.TryGetNumberFromText(text, this.ReadsHexadecimal));

            // If we are only accepting numbers then attempt to parse as an integer.
            if (isNumeric)
            {
                return this.ConvertNumber(number, out convertedValue);
            }

            // A decimal separator on its own is the start of a number typed without its leading zero,
            // and so is one behind a sign.
            var withoutSign = number.Length > 0 && this.IsSign(number.Substring(0, 1)) ? number.Substring(1) : number;

            if (withoutSign == this.SpecificCultureInfo.NumberFormat.NumberDecimalSeparator
                || withoutSign == this.SpecificCultureInfo.NumberFormat.CurrencyDecimalSeparator
                || withoutSign == this.SpecificCultureInfo.NumberFormat.PercentDecimalSeparator
               )
            {
                return true;
            }

            if (!this.TryParse(number, this.ParsingNumberStyle, this.SpecificCultureInfo, out convertedValue))
            {
                return false;
            }

            return true;
        }

        private bool ConvertNumber(string text, out T convertedValue)
        {
            if (text.Any(c => c == this.SpecificCultureInfo.NumberFormat.NumberDecimalSeparator[0]
                              || c == this.SpecificCultureInfo.NumberFormat.PercentDecimalSeparator[0]
                              || c == this.SpecificCultureInfo.NumberFormat.CurrencyDecimalSeparator[0]))
            {
                convertedValue = default;
                return false;
            }

            if (!this.TryParse(text, this.ParsingNumberStyle, this.SpecificCultureInfo, out convertedValue))
            {
                return false;
            }

            convertedValue = this.Truncate(convertedValue);
            return true;
        }

        /// <summary>Whether text is read as hexadecimal, which is a different shape of number.</summary>
        private bool ReadsHexadecimal => this.NumericInputMode == NumericInput.Numbers
                                         || this.ParsingNumberStyle.HasFlag(NumberStyles.AllowHexSpecifier)
                                         || this.ParsingNumberStyle == NumberStyles.HexNumber;

        private string TryGetNumberFromText(string text, bool isHex)
        {
            if (isHex)
            {
                var hexMatches = RegexHexadecimal.Matches(text);
                return hexMatches.Count > 0 ? hexMatches[0].Value : text;
            }

            if (this.regexNumber is null)
            {
                this.regexNumber = new Regex(RawRegexNumberString.Replace("<Sign>", Regex.Escape(this.SignCharacters))
                                                                 .Replace("<DecimalSeparator>", this.SpecificCultureInfo.NumberFormat.NumberDecimalSeparator)
                                                                 .Replace("<GroupSeparator>", this.SpecificCultureInfo.NumberFormat.NumberGroupSeparator),
                                             RegexOptions.Compiled);
            }

            var matches = this.regexNumber.Matches(text);
            return matches.Count > 0 ? matches[0].Value : text;
        }
    }
}
