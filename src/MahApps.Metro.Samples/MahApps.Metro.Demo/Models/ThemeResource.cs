// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using ControlzEx.Theming;

namespace MetroDemo.Models
{
    public class ThemeResource
    {
        public ThemeResource(Theme theme, LibraryTheme libraryTheme, ResourceDictionary resourceDictionary, DictionaryEntry dictionaryEntry)
            : this(theme, libraryTheme, resourceDictionary, dictionaryEntry.Key.ToString(), dictionaryEntry.Value)
        {
        }

        public ThemeResource(Theme theme, LibraryTheme libraryTheme, ResourceDictionary resourceDictionary, string? key, object? value)
        {
            this.Theme = theme;
            this.LibraryTheme = libraryTheme;

            this.Source = (resourceDictionary.Source?.ToString() ?? "Runtime").ToLower();
            this.Source = CultureInfo.InstalledUICulture.TextInfo.ToTitleCase(this.Source)
                                     .Replace("Pack", "pack")
                                     .Replace("Application", "application")
                                     .Replace("Xaml", "xaml");

            this.Key = key;

            this.Value = value switch
            {
                Color color => new SolidColorBrush(color),
                Brush brush => brush,
                _ => null
            };

            this.StringValue = value?.ToString();
        }

        public Theme Theme { get; }

        public LibraryTheme LibraryTheme { get; }

        public string Source { get; }

        public string? Key { get; }

        public Brush? Value { get; }

        public string? StringValue { get; }
    }
}