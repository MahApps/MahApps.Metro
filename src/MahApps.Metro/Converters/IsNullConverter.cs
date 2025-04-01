﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public sealed class IsNullConverter : IValueConverter
    {
        /// <summary>
        /// Gets a static default instance of <see cref="IsNullConverter"/>.
        /// </summary>
        public static readonly IsNullConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            return value is null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}