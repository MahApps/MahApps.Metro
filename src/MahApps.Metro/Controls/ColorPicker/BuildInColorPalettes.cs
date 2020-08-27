﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public static class BuildInColorPalettes
    {
        #region Build in Palettes
        public static Color[] StandardColorsPalette { get; } = new []
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
            typeof(Colors).GetProperties().Where(x => x.PropertyType == typeof(Color))
                .Select(x => (Color)x.GetValue(null))
                .OrderBy(c => new HSVColor(c).Hue)
                .ThenBy(c => new HSVColor(c).Saturation)
                .ThenByDescending(c => new HSVColor(c).Value));


        public static ObservableCollection<Color?> RecentColors { get; } = new ObservableCollection<Color?>();

        public static void AddColorToRecentColors(Color? color, IEnumerable recentColors)
        {
            if (recentColors is ObservableCollection<Color?> collection)
            {
                var oldIndex = collection.IndexOf(color);
                if (oldIndex > -1)
                {
                    collection.Move(oldIndex, 0);
                }
                else
                {
                    collection.Insert(0, color);
                }
            }
            
        }


        #endregion

        #region ReduceRecentColors

        public static readonly DependencyProperty MaximumRecentColorsCountProperty = DependencyProperty.RegisterAttached("MaximumRecentColorsCount", typeof(int), typeof(BuildInColorPalettes), new PropertyMetadata(10));

        [AttachedPropertyBrowsableForType(typeof(ColorPicker))]
        public static int GetMaximumRecentColorsCount(DependencyObject obj)
        {
            return (int)obj.GetValue(MaximumRecentColorsCountProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(ColorPicker))]
        public static void SetMaximumRecentColorsCount(DependencyObject obj, int value)
        {
            obj.SetValue(MaximumRecentColorsCountProperty, value);
        }


        public static void ReduceRecentColors(int MaxCount, IEnumerable recentColors)
        {
            if (recentColors is ObservableCollection<Color?> collection)
            {
                while (collection.Count > MaxCount)
                {
                    collection.RemoveAt(MaxCount);
                }
            }
        }
        #endregion
    }
}
