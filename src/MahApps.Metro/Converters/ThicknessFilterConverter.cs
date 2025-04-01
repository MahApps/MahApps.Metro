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
    /// Filters a Thickness by the given Filter property. Result is a new Thickness with the filtered side.
    /// </summary>
    [ValueConversion(typeof(Thickness), typeof(Thickness), ParameterType = typeof(ThicknessSideType))]
    public class ThicknessFilterConverter : IValueConverter
    {
        public ThicknessSideType Filter { get; set; } = ThicknessSideType.None;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Thickness thickness)
            {
                var filter = this.Filter;

                // yes, we can override it with the parameter value
                if (parameter is ThicknessSideType sideType)
                {
                    filter = sideType;
                }

                return filter switch
                {
                    ThicknessSideType.Left => new Thickness(thickness.Left, 0, 0, 0),
                    ThicknessSideType.Top => new Thickness(0, thickness.Top, 0, 0),
                    ThicknessSideType.Right => new Thickness(0, 0, thickness.Right, 0),
                    ThicknessSideType.Bottom => new Thickness(0, 0, 0, thickness.Bottom),
                    _ => thickness
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