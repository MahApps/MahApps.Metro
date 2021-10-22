// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public sealed class StringIsNullOrEmptyConverter : IValueConverter
    {
        /// <summary> Gets the default instance </summary>
        public static StringIsNullOrEmptyConverter Default { get; } = new StringIsNullOrEmptyConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}