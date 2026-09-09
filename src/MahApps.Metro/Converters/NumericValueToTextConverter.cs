// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Converters
{
    /// <summary>
    /// Turns the value of a cell into the text an up-down control would have shown for it, given a
    /// format and a culture.
    /// </summary>
    /// <typeparam name="T">The type of the value, for instance <see cref="double"/> or <see cref="decimal"/>.</typeparam>
    /// <remarks>
    /// The text is written by a control, not by this converter, because two parts of it are things
    /// only the type can answer: what a value looks like with no format at all, and what it looks
    /// like in hexadecimal, which a double cannot write on its own at all. Whoever builds one of
    /// these hands over a control to ask, and one is enough for a whole column.
    /// </remarks>
    internal sealed class NumericValueToTextConverter<T> : IMultiValueConverter
        where T : struct, IComparable<T>, IFormattable
    {
        private readonly NumericUpDownBase<T> formatter;

        public NumericValueToTextConverter(NumericUpDownBase<T> formatter)
        {
            this.formatter = formatter;
        }

        /// <summary>
        /// Takes the value, the format and the culture, in that order, and writes them out as one
        /// string.
        /// </summary>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3 || values[0] == DependencyProperty.UnsetValue)
            {
                return string.Empty;
            }

            var value = values[0] switch
                        {
                            T typed => typed,
                            null => (T?)null,
                            IConvertible convertible => (T)System.Convert.ChangeType(convertible, typeof(T), culture),
                            _ => (T?)null
                        };

            var format = values[1] as string ?? string.Empty;
            var itsCulture = values[2] as CultureInfo ?? culture;

            return this.formatter.TextFor(value, format, itsCulture) ?? string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
