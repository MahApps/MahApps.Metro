// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace MahApps.Metro.Converters
{
    /// <summary>
    /// This Converter converts a given height or width of an control to a CornerRadius
    /// </summary>
    [ValueConversion(typeof(double), typeof(CornerRadius))]
    [MarkupExtensionReturnType(typeof(SizeToCornerRadiusConverter))]
    public class SizeToCornerRadiusConverter : MarkupConverter
    {
        /// <inheritdoc />
        protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is double dValue ? new CornerRadius(dValue / 2) : new CornerRadius();
        }

        /// <inheritdoc />
        protected override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}