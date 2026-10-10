// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ControlzEx.Theming;

namespace MahApps.Metro.Theming
{
    /// <summary>
    /// Provides theme resources from MahApps.Metro.
    /// </summary>
    public class MahAppsLibraryThemeProvider : LibraryThemeProvider
    {
        // ahead of the instance below: the provider looks for its themes as soon as it is made
        private static readonly string AssemblyName = typeof(MahAppsLibraryThemeProvider).Assembly.GetName().Name!;

        public static readonly MahAppsLibraryThemeProvider DefaultInstance = new MahAppsLibraryThemeProvider();

        /// <summary>Where the dictionaries live that the themes of one base colour share.</summary>
        private const string ThemeBaseFolder = "/Styles/ThemeBase/";

        /// <inheritdoc cref="LibraryThemeProvider" />
        public MahAppsLibraryThemeProvider()
            : base(true)
        {
        }

        /// <summary>
        /// Takes a dictionary from the assembly for a theme. ControlzEx turns down any dictionary that
        /// merges others, so that one which only gathers themes is not taken for a theme itself. Ours
        /// merge the base of their base colour, which holds what does not change with the accent, so a
        /// theme dictionary that merges nothing but that base is taken as well.
        /// </summary>
        public override LibraryTheme? GetLibraryTheme(DictionaryEntry dictionaryEntry)
        {
            if (this.IsPotentialThemeResourceDictionary(dictionaryEntry) == false
                || dictionaryEntry.Key is not string key
                || string.IsNullOrEmpty(key))
            {
                return null;
            }

            var resourceDictionary = new ResourceDictionary
                                     {
                                         Source = new Uri($"pack://application:,,,/{AssemblyName};component/{key.Replace(".baml", ".xaml")}")
                                     };

            if (resourceDictionary.MergedDictionaries.All(IsThemeBase)
                && ThemeManager.Current.IsThemeDictionary(resourceDictionary))
            {
                return new LibraryTheme(resourceDictionary, this);
            }

            return null;
        }

        private static bool IsThemeBase(ResourceDictionary resourceDictionary)
        {
            return resourceDictionary.Source?.OriginalString.IndexOf(ThemeBaseFolder, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public override void FillColorSchemeValues(Dictionary<string, string> values, RuntimeThemeColorValues colorValues)
        {
            values.Add("MahApps.Colors.AccentBase", colorValues.AccentBaseColor.ToString());
            values.Add("MahApps.Colors.Accent", colorValues.AccentColor80.ToString());
            values.Add("MahApps.Colors.Accent2", colorValues.AccentColor60.ToString());
            values.Add("MahApps.Colors.Accent3", colorValues.AccentColor40.ToString());
            values.Add("MahApps.Colors.Accent4", colorValues.AccentColor20.ToString());

            values.Add("MahApps.Colors.Highlight", colorValues.HighlightColor.ToString());
            values.Add("MahApps.Colors.IdealForeground", colorValues.IdealForegroundColor.ToString());
        }
    }
}
