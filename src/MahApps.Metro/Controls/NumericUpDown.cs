// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A control for entering a <see cref="double"/>, with buttons to step it up and down.
    /// </summary>
    /// <remarks>
    /// This is the control MahApps has always had, and it holds its value as a <see cref="double"/>.
    /// A double cannot hold 0.1 exactly, and the error shows up once such values are added up, so a
    /// price or an amount of money is better off in a <see cref="DecimalUpDown"/>, which keeps every
    /// digit it was given.
    /// </remarks>
    public class NumericUpDown : NumericUpDownBase<double>
    {
        /// <summary>
        /// The lowest and the highest magnitude that reads better written out than as an exponent.
        /// Past the upper one a decimal format would drop digits, since it rounds at the fifteenth
        /// significant one; below the lower one the zeroes in front of the number take over.
        /// </summary>
        private const double SmallestPlainValue = 1e-15;

        private const double LargestPlainValue = 1e15;

        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));

            MinimumProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(double.MinValue));
            MaximumProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(double.MaxValue));
            IntervalProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(1d));
        }

        /// <inheritdoc />
        /// <remarks>
        /// A double cannot be read out of hexadecimal at all, so those digits are read as a whole
        /// number first. That is no loss: nobody writes a fraction in hexadecimal.
        /// </remarks>
        protected override bool TryParse(string text, NumberStyles style, IFormatProvider provider, out double value)
        {
            if ((style & NumberStyles.AllowHexSpecifier) != 0)
            {
                var isNumber = long.TryParse(text, style, provider, out var parsed);
                value = parsed;
                return isNumber;
            }

            return double.TryParse(text, style, provider, out value);
        }

        /// <inheritdoc />
        protected override double Add(double left, double right)
        {
            return left + right;
        }

        /// <inheritdoc />
        protected override double Multiply(double value, double factor)
        {
            return value * factor;
        }

        /// <inheritdoc />
        protected override double Divide(double value, double divisor)
        {
            return value / divisor;
        }

        /// <inheritdoc />
        protected override double Truncate(double value)
        {
            return Math.Truncate(value);
        }

        /// <inheritdoc />
        protected override double RoundToMultiple(double value, double multiple)
        {
            return Math.Round(value / multiple) * multiple;
        }

        /// <inheritdoc />
        protected override int Compare(double left, double right)
        {
            return left.CompareTo(right);
        }

        /// <inheritdoc />
        protected override bool IsZero(double value)
        {
            return Math.Abs(value) <= 0d;
        }

        /// <inheritdoc />
        protected override double ToDouble(double value)
        {
            return value;
        }

        /// <inheritdoc />
        protected override double FromDouble(double value)
        {
            return value;
        }

        /// <summary>
        /// Writes the value the way it would be typed, for as long as that reads better than an
        /// exponent. Left to itself a double answers 5E-05 for a small number and every digit a
        /// calculation left behind for the rest, neither of which belongs in a text box that was
        /// never asked to format anything.
        /// </summary>
        protected override string ToPlainString(double value, CultureInfo culture)
        {
            var magnitude = Math.Abs(value);

            if (magnitude > 0 && (magnitude < SmallestPlainValue || magnitude >= LargestPlainValue))
            {
                // A whole number that still fits in a long is written out in full. Neither of the
                // two ways below gets there: the format further down rounds at the fifteenth
                // significant digit, and what the framework answers on its own differs per target,
                // .NET Framework reaching for an exponent where .NET does not.
                if (magnitude < long.MaxValue && Math.Abs(value - Math.Truncate(value)) <= 0)
                {
                    return ((long)value).ToString(culture);
                }

                return value.ToString(culture);
            }

            return value.ToString("0.############################", culture);
        }
    }
}
