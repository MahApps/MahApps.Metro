// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// Black or white, whichever reads better on a brush as it shows over what lies behind it.
    /// The first value is the brush, the second the backdrop; a brush that is partly transparent
    /// is mixed with the backdrop first, since half a white on black is a grey and not a white.
    /// </summary>
    public class ReadableForegroundConverter : IMultiValueConverter
    {
        public object Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture)
        {
            var backdrop = values.Length > 1 && values[1] is SolidColorBrush behind ? behind.Color : Colors.White;
            var color = values.Length > 0 && values[0] is SolidColorBrush brush ? Over(brush.Color, brush.Opacity, backdrop) : backdrop;

            // the contrast against black is (L + 0.05) / 0.05, against white 1.05 / (L + 0.05); they meet at L = 0.179
            return Luminance(color) > 0.179 ? Brushes.Black : Brushes.White;
        }

        public object?[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static Color Over(Color color, double opacity, Color backdrop)
        {
            var alpha = color.A / 255.0 * opacity;
            return Color.FromRgb(Mix(color.R, backdrop.R, alpha), Mix(color.G, backdrop.G, alpha), Mix(color.B, backdrop.B, alpha));
        }

        private static byte Mix(byte front, byte back, double alpha)
        {
            return (byte)Math.Round(front * alpha + back * (1 - alpha));
        }

        private static double Luminance(Color color)
        {
            return 0.2126 * Linear(color.R) + 0.7152 * Linear(color.G) + 0.0722 * Linear(color.B);
        }

        private static double Linear(byte channel)
        {
            var c = channel / 255.0;
            return c <= 0.04045 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
        }
    }
}
