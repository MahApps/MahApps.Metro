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
    /// Converts a Thickness to a new Thickness. It's possible to ignore a side with the IgnoreThicknessSide property.
    /// </summary>
    [ValueConversion(typeof(Thickness), typeof(Thickness), ParameterType = typeof(ThicknessSideType))]
    public class ThicknessBindingConverter : IValueConverter
    {
        public ThicknessSideType IgnoreThicknessSide { get; set; } = ThicknessSideType.None;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Thickness thickness)
            {
                var ignoreThickness = this.IgnoreThicknessSide;

                // yes, we can override it with the parameter value
                if (parameter is ThicknessSideType sideType)
                {
                    ignoreThickness = sideType;
                }

                return ignoreThickness switch
                {
                    ThicknessSideType.Left => new Thickness(0, thickness.Top, thickness.Right, thickness.Bottom),
                    ThicknessSideType.Top => new Thickness(thickness.Left, 0, thickness.Right, thickness.Bottom),
                    ThicknessSideType.Right => new Thickness(thickness.Left, thickness.Top, 0, thickness.Bottom),
                    ThicknessSideType.Bottom => new Thickness(thickness.Left, thickness.Top, thickness.Right, 0),
                    _ => thickness
                };
            }

            return default(Thickness);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}