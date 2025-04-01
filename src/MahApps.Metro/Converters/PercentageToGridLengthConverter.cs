﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    [ValueConversion(typeof(double), typeof(GridLength))]
    public sealed class PercentageToGridLengthConverter : IValueConverter
    {
        /// <summary>
        /// Gets a static default instance of <see cref="PercentageToGridLengthConverter"/>.
        /// </summary>
        public static readonly PercentageToGridLengthConverter Default = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double percent)
            {
                var inverse = parameter as string == bool.TrueString;
                if (inverse)
                {
                    percent = 1 - percent;
                }

                return new GridLength(percent, GridUnitType.Star);
            }

            return Binding.DoNothing;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}