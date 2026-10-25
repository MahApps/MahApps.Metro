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
    /// Gives a number that cannot be below zero, for a size bound to a value that dips under it, such as the
    /// <see cref="System.Windows.Controls.DataGrid.CellsPanelHorizontalOffset"/> a grid with frozen columns rounds to
    /// a fraction below zero while it scrolls sideways.
    /// </summary>
    public sealed class NonNegativeDoubleConverter : IValueConverter
    {
        public static readonly NonNegativeDoubleConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is double number ? Math.Max(0d, number) : DependencyProperty.UnsetValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
