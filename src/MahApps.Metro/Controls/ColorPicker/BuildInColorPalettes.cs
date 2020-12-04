// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public static class BuildInColorPalettes
    {
        public static Color[] StandardColorsPalette { get; } =
            {
                Colors.Transparent,
                Colors.White,
                Colors.LightGray,
                Colors.Gray,
                Colors.Black,
                Colors.DarkRed,
                Colors.Red,
                Colors.Orange,
                Colors.Brown,
                Colors.Yellow,
                Colors.LimeGreen,
                Colors.Green,
                Colors.DarkTurquoise,
                Colors.Aqua,
                Colors.Navy,
                Colors.Blue,
                Colors.Indigo,
                Colors.Purple,
                Colors.Fuchsia
            };

        public static ObservableCollection<Color> WpfColorsPalette { get; } = new ObservableCollection<Color>(
            typeof(Colors)
                .GetProperties()
                .Where(x => x.PropertyType == typeof(Color))
                .Select(x => (Color)x.GetValue(null))
                .OrderBy(c => new HSVColor(c).Hue)
                .ThenBy(c => new HSVColor(c).Saturation)
                .ThenByDescending(c => new HSVColor(c).Value));

        public static ObservableCollection<Color?> RecentColors { get; } = new ObservableCollection<Color?>();

        public static void AddColorToRecentColors(Color? color, IEnumerable recentColors, int maxCount)
        {
            if (recentColors is ObservableCollection<Color?> collection)
            {
                var oldIndex = collection.IndexOf(color);
                if (oldIndex > 0)
                {
                    collection.Move(oldIndex, 0);
                }
                else if (oldIndex < 0)
                {
                    if (collection.Count >= maxCount)
                    {
                        collection.RemoveAt(maxCount - 1);
                    }

                    collection.Insert(0, color);
                }
            }
        }

        public static readonly DependencyProperty MaximumRecentColorsCountProperty
            = DependencyProperty.RegisterAttached(
                "MaximumRecentColorsCount",
                typeof(int),
                typeof(BuildInColorPalettes),
                new PropertyMetadata(10));

        /// <summary>Helper for getting <see cref="MaximumRecentColorsCountProperty"/> from <paramref name="obj"/>.</summary>
        /// <param name="obj"><see cref="DependencyObject"/> to read <see cref="MaximumRecentColorsCountProperty"/> from.</param>
        /// <returns>MaximumRecentColorsCount property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(ColorPickerBase))]
        public static int GetMaximumRecentColorsCount(DependencyObject obj)
        {
            return (int)obj.GetValue(MaximumRecentColorsCountProperty);
        }

        /// <summary>Helper for setting <see cref="MaximumRecentColorsCountProperty"/> on <paramref name="obj"/>.</summary>
        /// <param name="obj"><see cref="DependencyObject"/> to set <see cref="MaximumRecentColorsCountProperty"/> on.</param>
        /// <param name="value">MaximumRecentColorsCount property value.</param>
        [AttachedPropertyBrowsableForType(typeof(ColorPickerBase))]
        public static void SetMaximumRecentColorsCount(DependencyObject obj, int value)
        {
            obj.SetValue(MaximumRecentColorsCountProperty, value);
        }
    }
}