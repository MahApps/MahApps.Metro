// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace MahApps.Metro.Converters
{
    [MarkupExtensionReturnType(typeof(ToUpperConverter))]
    [ValueConversion(typeof(object), typeof(object))]
    [ValueConversion(typeof(string), typeof(string))]
    public class ToUpperConverter : MarkupConverter
    {
        /// <inheritdoc />
        protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is string s ? s.ToUpper(culture) : value;
        }

        /// <inheritdoc />
        protected override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}