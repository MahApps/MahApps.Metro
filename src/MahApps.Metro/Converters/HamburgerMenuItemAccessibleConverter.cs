// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    [ValueConversion(typeof(object), typeof(object))]
    internal sealed class HamburgerMenuItemAccessibleConverter : IMultiValueConverter
    {
        /// <summary>
        /// Gets a static default instance of <see cref="HamburgerMenuItemAccessibleConverter"/>.
        /// </summary>
        internal static readonly HamburgerMenuItemAccessibleConverter Default = new();

        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
        static HamburgerMenuItemAccessibleConverter()
        {
        }

        public object? Convert(object[]? values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values is null)
            {
                return Binding.DoNothing;
            }

            var automationPropertiesValue = values.ElementAtOrDefault(1) as string;
            if (!string.IsNullOrEmpty(automationPropertiesValue))
            {
                return automationPropertiesValue;
            }

            var menuItemValue = values.ElementAtOrDefault(0) as string;
            if (!string.IsNullOrEmpty(menuItemValue))
            {
                return menuItemValue;
            }

            return Binding.DoNothing;
        }

        public object[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            return targetTypes.Select(t => Binding.DoNothing).ToArray();
        }
    }
}