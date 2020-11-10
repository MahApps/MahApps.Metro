// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MahApps.Metro.Lang;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A Helper class for the Color-Struct
    /// </summary>
    public static class ColorHelper
    {
        static ColorHelper()
        {
            ColorNamesDictionary = new Dictionary<Color?, string>();

            var rm = new ResourceManager(typeof(ColorNames));
            var resourceSet = rm.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            if (resourceSet != null)
            {
                foreach (var entry in resourceSet.OfType<DictionaryEntry>())
                {
                    try
                    {
                        if (ColorConverter.ConvertFromString(entry.Key.ToString()) is Color color)
                        {
                            ColorNamesDictionary.Add(color, entry.Value.ToString());
                        }
                    }
                    catch (Exception)
                    {
                        Trace.TraceError($"{entry.Key} is not a valid color key!");
                    }
                }
            }
        }

        /// <summary>
        /// This function tries to convert a given string into a Color in the following order:
        ///    1. If the string starts with '#' the function tries to get the color from the hex-code
        ///    2. else the function tries to find the color in the color names Dictionary
        ///    3. If 1. + 2. were not successful the function adds a '#' sign and tries 1. + 2. again
        /// </summary>
        /// <param name="colorName">The localized name of the color, the hex-code of the color or the internal color name</param>
        /// <param name="colorNamesDictionary">Optional: The dictionary where the ColorName should be looked up</param>
        /// <returns>the Color if successful, else null</returns>
        public static Color? ColorFromString(string colorName, Dictionary<Color?, string> colorNamesDictionary)
        {
            Color? result = null;

            try
            {
                // if we don't have a string, we cannot have any Color
                if (string.IsNullOrWhiteSpace(colorName))
                {
                    return null;
                }

                colorNamesDictionary ??= ColorNamesDictionary;

                if (!colorName.StartsWith("#"))
                {
                    result = colorNamesDictionary?.FirstOrDefault(x => string.Equals(x.Value, colorName, StringComparison.OrdinalIgnoreCase)).Key;
                }

                result ??= ColorConverter.ConvertFromString(colorName) as Color?;
            }
            catch (FormatException)
            {
                if (colorName != null && !result.HasValue && !colorName.StartsWith("#"))
                {
                    result = ColorFromString("#" + colorName);
                }
            }

            return result;
        }

        /// <summary>
        /// This function tries to convert a given string into a Color in the following order:
        ///    1. If the string starts with '#' the function tries to get the color from the hex-code
        ///    2. else the function tries to find the color in the default <see cref="ColorNamesDictionary"/>
        ///    3. If 1. + 2. were not successful the function adds a '#' sign and tries 1. + 2. again
        /// </summary>
        /// <param name="colorName">The localized name of the color, the hex-code of the color or the internal color name</param>
        /// <returns>the Color if successful, else null</returns>
        public static Color? ColorFromString(string colorName)
        {
            return ColorFromString(colorName, null);
        }

        /// <summary>
        /// A Dictionary with localized Color Names
        /// </summary>
        public static Dictionary<Color?, string> ColorNamesDictionary { get; set; }

        /// <summary>
        /// Searches for the localized name of a given <paramref name="color"/>
        /// </summary>
        /// <param name="color">color</param>
        /// <param name="colorNamesDictionary">Optional: The dictionary where the ColorName should be looked up</param>
        /// <returns>the local color name or null if the given color doesn't have a name</returns>
        public static string GetColorName(Color? color, Dictionary<Color?, string> colorNamesDictionary)
        {
            if (color is null)
            {
                return null;
            }

            colorNamesDictionary ??= ColorNamesDictionary;

            return colorNamesDictionary.TryGetValue(color, out string name) ? $"{name} ({color})" : $"{color}";
        }

        /// <summary>
        /// Searches for the localized name of a given <paramref name="color"/> by using the default <see cref="ColorNamesDictionary"/>
        /// </summary>
        /// <param name="color">color</param>
        /// <returns>the local color name or null if the given color doesn't have a name</returns>
        public static string GetColorName(Color? color)
        {
            return GetColorName(color, null);
        }
    }
}