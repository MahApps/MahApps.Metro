// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// A number of whatever kind on its way to an editor that only takes a double.
    /// <para>
    /// A watched property can be an int, a float or a double, and WPF will not convert an int into
    /// the double the editor wants, so that is done here, and a NaN becomes an empty box. On the way
    /// back the value is handed over as it is, because ExampleProperty puts it into the type its
    /// property actually holds and knows what an empty box has to mean there.
    /// </para>
    /// </summary>
    public sealed class NumberConverter : IValueConverter
    {
        /// <inheritdoc />
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return null;
            }

            try
            {
                var number = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);

                // Width and Height are NaN until somebody sets them, and a NumericUpDown handed a
                // NaN clamps it to its minimum, which is how -1.79E+308 turned up in the editor. An
                // empty box is what "nobody has said" looks like.
                return double.IsNaN(number) ? null : number;
            }
            catch (Exception exception) when (exception is InvalidCastException or FormatException or OverflowException)
            {
                return null;
            }
        }

        /// <inheritdoc />
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
