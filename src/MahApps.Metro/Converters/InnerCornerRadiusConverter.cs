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
    /// The corners of what fills a border, for content that rounds its own corners rather than being
    /// clipped to the ones the frame leaves over. It takes the corner radius of the border, and
    /// optionally its border thickness and its padding, and takes off what <see cref="ClipGeometryConverter"/>
    /// takes off, so the two meet the frame in the same place.
    /// </summary>
    /// <remarks>
    /// A clip has to follow the size of what it cuts, and every clip that changes sends the layout
    /// round once more. Where a list carries many items that is enough for WPF to put the rest of
    /// the layout off, so content that is only a fill is better off with these corners.
    /// </remarks>
    public class InnerCornerRadiusConverter : IMultiValueConverter
    {
        public static readonly InnerCornerRadiusConverter Instance = new();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 1 || values[0] is not CornerRadius cornerRadius)
            {
                return DependencyProperty.UnsetValue;
            }

            var borderThickness = values.Length > 1 && values[1] is Thickness thickness ? thickness : default;
            var padding = values.Length > 2 && values[2] is Thickness thicknessPadding ? thicknessPadding : default;
            var inset = ClipGeometryConverter.Inset(borderThickness, padding);

            // a CornerRadius has one value per corner, so a corner whose two sides lose different
            // amounts takes the larger, which keeps the fill inside the frame
            return new CornerRadius(Corner(cornerRadius.TopLeft, inset.Left, inset.Top),
                                    Corner(cornerRadius.TopRight, inset.Right, inset.Top),
                                    Corner(cornerRadius.BottomRight, inset.Right, inset.Bottom),
                                    Corner(cornerRadius.BottomLeft, inset.Left, inset.Bottom));
        }

        public object?[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();

        private static double Corner(double radius, double one, double other) => Math.Max(0.0, radius - Math.Max(one, other));
    }
}
