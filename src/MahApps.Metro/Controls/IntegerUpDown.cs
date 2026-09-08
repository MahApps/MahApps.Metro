// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A control for entering an <see cref="int"/>, with buttons to step it up and down.
    /// </summary>
    /// <remarks>
    /// This is the control for something that is counted rather than measured, a number of copies or
    /// a page. Nothing behind a decimal separator can be typed into it, so its
    /// <see cref="NumericUpDownBase.NumericInputMode"/> starts out at numbers only.
    /// </remarks>
    public class IntegerUpDown : NumericUpDownBase<int>
    {
        static IntegerUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IntegerUpDown), new FrameworkPropertyMetadata(typeof(IntegerUpDown)));

            MinimumProperty.OverrideMetadata(typeof(IntegerUpDown), new FrameworkPropertyMetadata(int.MinValue));
            MaximumProperty.OverrideMetadata(typeof(IntegerUpDown), new FrameworkPropertyMetadata(int.MaxValue));
            IntervalProperty.OverrideMetadata(typeof(IntegerUpDown), new FrameworkPropertyMetadata(1));
            NumericInputModeProperty.OverrideMetadata(typeof(IntegerUpDown), new FrameworkPropertyMetadata(NumericInput.Numbers));
        }

        /// <inheritdoc />
        protected override bool TryParse(string text, NumberStyles style, IFormatProvider provider, out int value)
        {
            return int.TryParse(text, style, provider, out value);
        }

        /// <inheritdoc />
        protected override int Add(int left, int right)
        {
            return left + right;
        }

        /// <inheritdoc />
        protected override int Multiply(int value, double factor)
        {
            return (int)(value * factor);
        }

        /// <inheritdoc />
        protected override int Divide(int value, double divisor)
        {
            return (int)(value / divisor);
        }

        /// <inheritdoc />
        protected override int Truncate(int value)
        {
            return value;
        }

        /// <inheritdoc />
        protected override int RoundToMultiple(int value, int multiple)
        {
            return (int)Math.Round((double)value / multiple) * multiple;
        }

        /// <inheritdoc />
        protected override int Compare(int left, int right)
        {
            return left.CompareTo(right);
        }

        /// <inheritdoc />
        protected override bool IsZero(int value)
        {
            return value == 0;
        }

        /// <inheritdoc />
        protected override double ToDouble(int value)
        {
            return value;
        }

        /// <inheritdoc />
        protected override int FromDouble(double value)
        {
            return (int)value;
        }

        /// <inheritdoc />
        protected override string ToPlainString(int value, CultureInfo culture)
        {
            return value.ToString(culture);
        }
    }
}
