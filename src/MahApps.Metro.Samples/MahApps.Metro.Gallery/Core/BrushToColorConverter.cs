// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// Puts a brush in front of a colour picker and takes the colour back out as a brush again.
    /// The library has this pair the other way round in ColorToSolidColorBrushConverter, and a
    /// binding cannot run a converter backwards.
    /// </summary>
    public class BrushToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is SolidColorBrush brush ? brush.Color : null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not Color color)
            {
                return null;
            }

            var brush = new SolidColorBrush(color);
            brush.Freeze();

            return brush;
        }
    }
}
