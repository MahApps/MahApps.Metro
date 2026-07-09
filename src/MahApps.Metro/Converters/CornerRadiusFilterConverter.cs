// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    /// <summary>
    /// Filters a CornerRadius by the given Filter property. Result can be a new CornerRadius or a value of it's 4 corners.
    /// </summary>
    [ValueConversion(typeof(CornerRadius), typeof(CornerRadius), ParameterType = typeof(RadiusType))]
    [ValueConversion(typeof(CornerRadius), typeof(double), ParameterType = typeof(RadiusType))]
    public class CornerRadiusFilterConverter : IValueConverter
    {
        public RadiusType Filter { get; set; } = RadiusType.None;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is CornerRadius cornerRadius)
            {
                var filter = this.Filter;

                // yes, we can override it with the parameter value
                if (parameter is RadiusType radiusType)
                {
                    filter = radiusType;
                }

                return filter switch
                {
                    RadiusType.Left => new CornerRadius(cornerRadius.TopLeft, 0, 0, cornerRadius.BottomLeft),
                    RadiusType.Top => new CornerRadius(cornerRadius.TopLeft, cornerRadius.TopRight, 0, 0),
                    RadiusType.Right => new CornerRadius(0, cornerRadius.TopRight, cornerRadius.BottomRight, 0),
                    RadiusType.Bottom => new CornerRadius(0, 0, cornerRadius.BottomRight, cornerRadius.BottomLeft),
                    RadiusType.TopLeft => cornerRadius.TopLeft,
                    RadiusType.TopRight => cornerRadius.TopRight,
                    RadiusType.BottomRight => cornerRadius.BottomRight,
                    RadiusType.BottomLeft => cornerRadius.BottomLeft,
                    _ => cornerRadius
                };
            }

            return Binding.DoNothing;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // for now no back converting
            return DependencyProperty.UnsetValue;
        }
    }
}