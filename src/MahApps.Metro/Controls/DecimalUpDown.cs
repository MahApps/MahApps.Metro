// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A control for entering a <see cref="decimal"/>, with buttons to step it up and down.
    /// </summary>
    /// <remarks>
    /// This is the control for money and for anything else where the digits that were typed are the
    /// digits that have to come back out. A decimal holds 0.1 exactly, where a <see cref="double"/>
    /// only holds something very close to it, so a column of prices added up here comes to the amount
    /// on the receipt. It reaches less far than a double and counts slower, neither of which matters
    /// for an amount somebody types.
    /// </remarks>
    public class DecimalUpDown : NumericUpDownBase<decimal>
    {
        static DecimalUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DecimalUpDown), new FrameworkPropertyMetadata(typeof(DecimalUpDown)));

            MinimumProperty.OverrideMetadata(typeof(DecimalUpDown), new FrameworkPropertyMetadata(decimal.MinValue));
            MaximumProperty.OverrideMetadata(typeof(DecimalUpDown), new FrameworkPropertyMetadata(decimal.MaxValue));
            IntervalProperty.OverrideMetadata(typeof(DecimalUpDown), new FrameworkPropertyMetadata(1m));
        }

        /// <inheritdoc />
        /// <remarks>
        /// A decimal cannot be read out of hexadecimal at all, so those digits are read as a whole
        /// number first. That is no loss: nobody writes a fraction in hexadecimal.
        /// </remarks>
        protected override bool TryParse(string text, NumberStyles style, IFormatProvider provider, out decimal value)
        {
            if ((style & NumberStyles.AllowHexSpecifier) != 0)
            {
                var isNumber = long.TryParse(text, style, provider, out var parsed);
                value = parsed;
                return isNumber;
            }

            return decimal.TryParse(text, style, provider, out value);
        }

        /// <inheritdoc />
        protected override decimal Add(decimal left, decimal right)
        {
            return left + right;
        }

        /// <inheritdoc />
        protected override decimal Multiply(decimal value, double factor)
        {
            return value * (decimal)factor;
        }

        /// <inheritdoc />
        protected override decimal Divide(decimal value, double divisor)
        {
            return value / (decimal)divisor;
        }

        /// <inheritdoc />
        protected override decimal Truncate(decimal value)
        {
            return Math.Truncate(value);
        }

        /// <inheritdoc />
        protected override decimal RoundToMultiple(decimal value, decimal multiple)
        {
            return Math.Round(value / multiple) * multiple;
        }

        /// <inheritdoc />
        protected override int Compare(decimal left, decimal right)
        {
            return left.CompareTo(right);
        }

        /// <inheritdoc />
        protected override bool IsZero(decimal value)
        {
            return value == decimal.Zero;
        }

        /// <inheritdoc />
        protected override double ToDouble(decimal value)
        {
            return (double)value;
        }

        /// <inheritdoc />
        protected override decimal FromDouble(double value)
        {
            return (decimal)value;
        }

        /// <inheritdoc />
        protected override string ToPlainString(decimal value, CultureInfo culture)
        {
            // A decimal writes itself out in full and without an exponent, but it also carries the
            // trailing zeroes it was given. The text box shows the number, not how it was arrived at.
            return value.ToString("0.#############################", culture);
        }
    }
}
