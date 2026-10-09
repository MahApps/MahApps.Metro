// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    /// <summary>
    /// Turns the <see cref="Controls.ExpanderHelper.ContentSlideProperty"/> of the content site of an expander into
    /// the distance that site is moved along one axis: that part of its own size, towards the header.
    /// </summary>
    public class ExpanderContentSlideConverter : IMultiValueConverter
    {
        public static readonly ExpanderContentSlideConverter Instance = new();

        /// <summary>
        /// Takes the slide, the size of the site along the axis given as the <see cref="Orientation"/> parameter, and the
        /// <see cref="ExpandDirection"/> of the expander. An expander that opens along the other axis does not move along this one.
        /// </summary>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3
                || values[0] is not double slide
                || values[1] is not double size
                || values[2] is not ExpandDirection direction
                || parameter is not Orientation axis)
            {
                return 0d;
            }

            var horizontal = direction is ExpandDirection.Left or ExpandDirection.Right;
            if (horizontal != (axis == Orientation.Horizontal))
            {
                return 0d;
            }

            // the header stands above content that opens downwards and left of content that opens to the right
            var towardsHeader = direction is ExpandDirection.Down or ExpandDirection.Right ? -1 : 1;

            return towardsHeader * slide * size;
        }

        public object?[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
