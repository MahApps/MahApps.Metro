// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MahApps.Metro.Converters
{
    [ValueConversion(typeof(SolidColorBrush), typeof(SolidColorBrush))]
    public sealed class BackgroundToForegroundConverter : IValueConverter, IMultiValueConverter
    {
        /// <summary>
        /// Gets a static default instance of <see cref="BackgroundToForegroundConverter"/>.
        /// </summary>
        public static readonly BackgroundToForegroundConverter Instance = new();

        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
        static BackgroundToForegroundConverter()
        {
        }

        /// <summary>
        /// Determining Ideal Text Color Based on Specified Background Color
        /// http://www.codeproject.com/KB/GDI-plus/IdealTextColor.aspx
        /// </summary>
        /// <param name = "background">The background color.</param>
        /// <returns></returns>
        private static Color IdealTextColor(Color background)
        {
            const int nThreshold = 86; //105;
            var bgDelta = System.Convert.ToInt32((background.R * 0.299) + (background.G * 0.587) + (background.B * 0.114));
            var foreColor = (255 - bgDelta < nThreshold) ? Colors.Black : Colors.White;
            return foreColor;
        }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush backgroundBrush)
            {
                var idealForegroundColor = IdealTextColor(backgroundBrush.Color);
                var foregroundBrush = new SolidColorBrush(idealForegroundColor);
                foregroundBrush.Freeze();
                return foregroundBrush;
            }

            return Brushes.White;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        public object? Convert(object[]? values, Type targetType, object? parameter, CultureInfo culture)
        {
            var titleBrush = values?.Length > 1 ? values[1] as Brush : null;
            if (titleBrush is not null)
            {
                return titleBrush;
            }

            var backgroundBrush = values?.Length > 0 ? values[0] as Brush : null;
            return this.Convert(backgroundBrush, targetType, parameter, culture);
        }

        public object[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            return targetTypes.Select(t => DependencyProperty.UnsetValue).ToArray();
        }
    }
}