// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// The null of a nullable choice on its way to a combo box and back.
    /// <para>
    /// A combo box takes a null item for no selection at all, so a click on it does nothing and the
    /// box keeps showing what was picked before. The choice therefore offers <see cref="NotSet"/> in
    /// its place, and this turns the one into the other.
    /// </para>
    /// </summary>
    public sealed class NotSetConverter : IValueConverter
    {
        /// <summary>
        /// What a choice offers for leaving the property unset.
        /// </summary>
        public static readonly object NotSet = new NotSetValue();

        /// <inheritdoc />
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value ?? NotSet;
        }

        /// <inheritdoc />
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return ReferenceEquals(value, NotSet) ? null : value;
        }

        private sealed class NotSetValue
        {
            public override string ToString() => "not set";
        }
    }
}
