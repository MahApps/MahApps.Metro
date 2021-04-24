// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    // this converter is only used by DatePicker to convert the font size to width and height of the icon button
    [ValueConversion(typeof(double), typeof(double), ParameterType = typeof(double))]
    [ValueConversion(typeof(object), typeof(object), ParameterType = typeof(double))]
    public class FontSizeOffsetConverter : IValueConverter
    {
        /// <summary>
        /// Gets a static default instance of <see cref="FontSizeOffsetConverter"/>.
        /// </summary>
        public static readonly FontSizeOffsetConverter Instance = new();

        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
        static FontSizeOffsetConverter()
        {
        }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double orgValue && parameter is double offset)
            {
                return Math.Round(orgValue + offset);
            }

            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}