// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    /// <summary>
    /// Converts a double representing either hour/minute/second to the corresponding angle.
    /// </summary>
    [ValueConversion(typeof(double), typeof(double))]
    public sealed class ClockDegreeConverter : IValueConverter
    {
        public double TotalParts { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return 0;
            }

            if (value is DateTime dateTime && parameter is string timePart)
            {
                return timePart switch
                {
                    "h" => 360.0 / 12 * dateTime.TimeOfDay.TotalHours,
                    "m" => 360.0 / 60 * dateTime.TimeOfDay.TotalMinutes,
                    "s" => 360.0 / 60 * dateTime.TimeOfDay.Seconds,
                    _ => throw new ArgumentException("must be \"h\", \"m\", or \"s", nameof(parameter))
                };
            }

            if (this.TotalParts != 0)
            {
                switch (value)
                {
                    case int valueAsInt when valueAsInt != 0:
                        return 360 / this.TotalParts * valueAsInt;
                    case double valueAsDouble when valueAsDouble != 0:
                        return 360 / this.TotalParts * valueAsDouble;
                }
            }

            return 0;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}