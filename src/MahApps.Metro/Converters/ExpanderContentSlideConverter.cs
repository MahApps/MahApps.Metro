// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MahApps.Metro.Converters
{
    /// <summary>
    /// Turns the <see cref="Controls.ExpanderHelper.ContentSlideProperty"/> of the content site of an expander into
    /// the transform that moves the site that part of its own size, towards the header.
    /// </summary>
    public class ExpanderContentSlideConverter : IMultiValueConverter
    {
        public static readonly ExpanderContentSlideConverter Instance = new();

        /// <summary>
        /// Takes the slide, the width and the height of the site, and the <see cref="ExpandDirection"/> of the expander,
        /// and gives a <see cref="TranslateTransform"/>. The site only moves along the axis the expander opens along.
        /// </summary>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 4
                || values[0] is not double slide
                || values[1] is not double width
                || values[2] is not double height
                || values[3] is not ExpandDirection direction)
            {
                return new TranslateTransform();
            }

            // the header stands above content that opens downwards and left of content that opens to the right
            var towardsHeader = direction is ExpandDirection.Down or ExpandDirection.Right ? -1 : 1;

            return direction is ExpandDirection.Left or ExpandDirection.Right
                ? new TranslateTransform(towardsHeader * slide * width, 0)
                : new TranslateTransform(0, towardsHeader * slide * height);
        }

        public object?[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
