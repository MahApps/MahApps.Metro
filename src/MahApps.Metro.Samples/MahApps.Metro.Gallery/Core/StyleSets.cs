// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// The three sets of styles the library comes with.
    /// </summary>
    public enum GalleryStyleSet
    {
        /// <summary>The default set, Styles/Controls.xaml.</summary>
        Metro,

        /// <summary>The set of Windows 10, Styles/Win10/Controls.xaml.</summary>
        Win10,

        /// <summary>The set of WinUI, Styles/WinUI/Controls.xaml.</summary>
        WinUI
    }

    /// <summary>
    /// Puts a sample into one of the three sets, so that a card can show the same markup in each of them.
    /// Like <see cref="DefaultStyleSet"/> every set is loaded once and shared by all the samples in it.
    /// </summary>
    public static class StyleSets
    {
        private static ResourceDictionary? win10;
        private static ResourceDictionary? winUI;

        /// <summary>Identifies the StyleSet attached property.</summary>
        public static readonly DependencyProperty StyleSetProperty
            = DependencyProperty.RegisterAttached("StyleSet",
                                                  typeof(GalleryStyleSet),
                                                  typeof(StyleSets),
                                                  new FrameworkPropertyMetadata(GalleryStyleSet.Metro, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the set the samples under the given element stand in. It is inherited, so a page sets it
        /// once and every card on it follows.
        /// </summary>
        public static GalleryStyleSet GetStyleSet(DependencyObject element)
        {
            return (GalleryStyleSet)element.GetValue(StyleSetProperty);
        }

        /// <summary>
        /// Sets the set the samples under the given element stand in.
        /// </summary>
        public static void SetStyleSet(DependencyObject element, GalleryStyleSet value)
        {
            element.SetValue(StyleSetProperty, value);
        }

        private static ResourceDictionary Win10
            => win10 ??= new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Win10/Controls.xaml", UriKind.Absolute) };

        private static ResourceDictionary WinUI
            => winUI ??= new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/WinUI/Controls.xaml", UriKind.Absolute) };

        /// <summary>
        /// The dictionary of the given set.
        /// </summary>
        public static ResourceDictionary Get(GalleryStyleSet set)
        {
            return set switch
            {
                GalleryStyleSet.Win10 => Win10,
                GalleryStyleSet.WinUI => WinUI,
                _ => DefaultStyleSet.Dictionary
            };
        }

        /// <summary>
        /// The name a reader knows the set by.
        /// </summary>
        public static string NameOf(GalleryStyleSet set)
        {
            return set switch
            {
                GalleryStyleSet.Win10 => "Windows 10",
                GalleryStyleSet.WinUI => "WinUI",
                _ => "Metro"
            };
        }

        /// <summary>
        /// Puts the given element and everything under it into the given set, in place of any other
        /// set it was in. Its own resources stay in front, as with <see cref="DefaultStyleSet.Apply"/>.
        /// </summary>
        public static void Apply(FrameworkElement element, GalleryStyleSet set)
        {
            var merged = element.Resources.MergedDictionaries;
            var wanted = Get(set);

            // only the sets already loaded can be in there, so asking for the others would load them for nothing
            foreach (var other in new[] { DefaultStyleSet.Dictionary, win10, winUI })
            {
                if (other is not null && !ReferenceEquals(other, wanted))
                {
                    merged.Remove(other);
                }
            }

            if (!merged.Contains(wanted))
            {
                merged.Insert(0, wanted);
            }
        }
    }
}
