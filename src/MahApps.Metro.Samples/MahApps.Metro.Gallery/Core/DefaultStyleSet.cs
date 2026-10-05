// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// The default set of the library, for the samples of a gallery that wears the WinUI set itself.
    /// A sample put inside it finds the implicit styles of the default set before the ones of the
    /// application, so what it shows is what its markup gives anyone who copies it.
    /// </summary>
    public static class DefaultStyleSet
    {
        private static ResourceDictionary? dictionary;

        /// <summary>
        /// The one dictionary every sample shares. The set is large, and loading it once per card
        /// is what a page full of cards would otherwise wait for.
        /// </summary>
        public static ResourceDictionary Dictionary
            => dictionary ??= new ResourceDictionary
                              {
                                  Source = new Uri("pack://application:,,,/MahApps.Metro.Gallery;component/Themes/DefaultStyleSet.xaml", UriKind.Absolute)
                              };

        /// <summary>
        /// Puts the given element and everything under it into the default set. Its own resources
        /// stay in front, so a sample that brings a style of its own keeps it.
        /// </summary>
        public static void Apply(FrameworkElement element)
        {
            if (!element.Resources.MergedDictionaries.Contains(Dictionary))
            {
                element.Resources.MergedDictionaries.Insert(0, Dictionary);
            }
        }
    }
}
