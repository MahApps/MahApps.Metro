// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A control for entering a <see cref="long"/>, with buttons to step it up and down.
    /// </summary>
    /// <remarks>
    /// The same as an <see cref="IntegerUpDown"/>, for the counts that run past two billion: a file
    /// size in bytes, a row number, an identifier out of a database.
    /// </remarks>
    public class LongUpDown : NumericUpDownBase<long>
    {
        static LongUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LongUpDown), new FrameworkPropertyMetadata(typeof(LongUpDown)));

            MinimumProperty.OverrideMetadata(typeof(LongUpDown), new FrameworkPropertyMetadata(long.MinValue));
            MaximumProperty.OverrideMetadata(typeof(LongUpDown), new FrameworkPropertyMetadata(long.MaxValue));
            IntervalProperty.OverrideMetadata(typeof(LongUpDown), new FrameworkPropertyMetadata(1L));
            NumericInputModeProperty.OverrideMetadata(typeof(LongUpDown), new FrameworkPropertyMetadata(NumericInput.Numbers));
        }

        /// <inheritdoc />
        protected override bool TryParse(string text, NumberStyles style, IFormatProvider provider, out long value)
        {
            return long.TryParse(text, style, provider, out value);
        }

        /// <inheritdoc />
        protected override long Add(long left, long right)
        {
            return left + right;
        }

        /// <inheritdoc />
        protected override long Multiply(long value, double factor)
        {
            return (long)(value * factor);
        }

        /// <inheritdoc />
        protected override long Divide(long value, double divisor)
        {
            return (long)(value / divisor);
        }

        /// <inheritdoc />
        protected override long Truncate(long value)
        {
            return value;
        }

        /// <inheritdoc />
        protected override long RoundToMultiple(long value, long multiple)
        {
            return (long)Math.Round((double)value / multiple) * multiple;
        }

        /// <inheritdoc />
        protected override int Compare(long left, long right)
        {
            return left.CompareTo(right);
        }

        /// <inheritdoc />
        protected override bool IsZero(long value)
        {
            return value == 0L;
        }

        /// <inheritdoc />
        protected override double ToDouble(long value)
        {
            return value;
        }

        /// <inheritdoc />
        protected override long FromDouble(double value)
        {
            return (long)value;
        }

        /// <inheritdoc />
        protected override string ToPlainString(long value, CultureInfo culture)
        {
            return value.ToString(culture);
        }
    }
}
