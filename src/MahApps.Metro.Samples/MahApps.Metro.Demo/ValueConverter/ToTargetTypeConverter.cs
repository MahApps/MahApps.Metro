// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MetroDemo.ValueConverter
{
    /// <summary>
    /// Carries a number into whatever type the target property holds.
    /// </summary>
    /// <remarks>
    /// The settings on the up-down page are doubles, and they are handed to four controls that hold
    /// four different types. WPF converts between them on its own, but it uses Convert.ChangeType,
    /// which throws on a null even when the target is a nullable, and it goes by the declared type
    /// of the source property, which is object for the ones that can be left unset. This does both:
    /// a null reaches a nullable target as a null, and everything else is converted by hand.
    /// </remarks>
    public class ToTargetTypeConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null || value == DependencyProperty.UnsetValue)
            {
                // A nullable target takes the null. Anything else keeps whatever default it has.
                return Nullable.GetUnderlyingType(targetType) is null ? DependencyProperty.UnsetValue : null;
            }

            var wanted = Nullable.GetUnderlyingType(targetType) ?? targetType;

            return wanted.IsInstanceOfType(value)
                ? value
                : System.Convert.ChangeType(value, wanted, culture);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
