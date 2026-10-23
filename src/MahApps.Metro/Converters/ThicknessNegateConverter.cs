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
    /// Turns a <see cref="Thickness"/> into the same one pointing the other way, every side negated.
    /// A margin of that kind lets an element stand flush with the edge of a container whose padding
    /// it is given, the editors of a DataGrid cell for instance.
    /// </summary>
    [ValueConversion(typeof(Thickness), typeof(Thickness))]
    public sealed class ThicknessNegateConverter : IValueConverter
    {
        /// <summary>
        /// Gets a static default instance of <see cref="ThicknessNegateConverter"/>.
        /// </summary>
        public static readonly ThicknessNegateConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is Thickness thickness
                ? new Thickness(-thickness.Left, -thickness.Top, -thickness.Right, -thickness.Bottom)
                : default(Thickness);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
