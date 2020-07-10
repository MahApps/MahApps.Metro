// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace MetroDemo.ValueConverter
{
    [ValueConversion(typeof(int), typeof(string))]
    [MarkupExtensionReturnType(typeof(ItemCountConverter))]
    public class ItemCountConverter : MarkupExtension, IValueConverter
    {
        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int itemCount)
            {
                if (itemCount > 1)
                {
                    return $"({itemCount} items)";
                }

                if (itemCount == 1)
                {
                    return $"({itemCount} item)";
                }
            }

            return string.Empty;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}