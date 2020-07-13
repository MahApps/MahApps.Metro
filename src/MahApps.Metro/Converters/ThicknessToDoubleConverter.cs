// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    [ValueConversion(typeof(Thickness), typeof(double), ParameterType = typeof(ThicknessSideType))]
    public class ThicknessToDoubleConverter : IValueConverter
    {
        public ThicknessSideType TakeThicknessSide { get; set; } = ThicknessSideType.None;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Thickness thickness)
            {
                var takeThicknessSide = this.TakeThicknessSide;

                // yes, we can override it with the parameter value
                if (parameter is ThicknessSideType sideType)
                {
                    takeThicknessSide = sideType;
                }

                switch (takeThicknessSide)
                {
                    case ThicknessSideType.Left:
                        return thickness.Left;
                    case ThicknessSideType.Top:
                        return thickness.Top;
                    case ThicknessSideType.Right:
                        return thickness.Right;
                    case ThicknessSideType.Bottom:
                        return thickness.Bottom;
                    default:
                        return default(double);
                }
            }

            return default(double);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}