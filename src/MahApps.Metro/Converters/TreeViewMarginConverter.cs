// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MahApps.Metro.Converters
{
    [ValueConversion(typeof(TreeViewItem), typeof(Thickness))]
    public class TreeViewMarginConverter : IValueConverter
    {
        public double Length { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is TreeViewItem item)
            {
                return new Thickness(this.Length * item.GetDepth(), 0, 0, 0);
            }

            return new Thickness(0);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public static class TreeViewItemExtensions
    {
        public static int GetDepth(this TreeViewItem item)
        {
            // Use non linq version for better performance (maybe).
            return CountAncestors<TreeView>(item, x => x is TreeViewItem);
        }

        /// <summary>
        /// Counts the ancestors until the type T was found.
        /// </summary>
        /// <typeparam name="T">The type until the search should work.</typeparam>
        /// <param name="child">The start child.</param>
        /// <param name="countFor">A filter function which can be null to count all ancestors.</param>
        /// <returns></returns>
        internal static int CountAncestors<T>(this DependencyObject child, Func<DependencyObject, bool> countFor)
            where T : DependencyObject
        {
            var count = 0;
            var parent = VisualTreeHelper.GetParent(child);

            while (parent is not null && parent is not T)
            {
                if (countFor is null || countFor(parent))
                {
                    count++;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return count;
        }
    }
}