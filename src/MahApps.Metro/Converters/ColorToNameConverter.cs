// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace MahApps.Metro.Converters
{
    [MarkupExtensionReturnType(typeof(ColorToNameConverter))]
    [ValueConversion(typeof(Color?), typeof(string))]
    public class ColorToNameConverter : MarkupMultiConverter
    {
        /// <summary>
        /// Converts a given <see cref="Color"/> to its Name
        /// </summary>
        /// <param name="value">Needed: The <see cref="Color"/>. </param>
        /// <param name="targetType"></param>
        /// <param name="parameter">Optional: A <see cref="Dictionary{TKey, TValue}"/></param>
        /// <param name="culture"></param>
        /// <returns>The name of the color or the Hex-Code if no name is available</returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ColorHelper.GetColorName(value as Color?, parameter as Dictionary<Color?, string>);
        }

        /// <summary>
        /// Converts a given <see cref="Color"/> to its Name
        /// </summary>
        /// <param name="values">Needed: The <see cref="Color"/>. Optional: A <see cref="Dictionary{TKey, TValue}"/></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>The name of the color or the Hex-Code if no name is available</returns>
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var color = values?.FirstOrDefault(x => x?.GetType() == typeof(Color)) as Color?;
            var colorNamesDictionary = values?.FirstOrDefault(x => x?.GetType() == typeof(Dictionary<Color?, string>)) as Dictionary<Color?, string>;

            return ColorHelper.GetColorName(color, colorNamesDictionary);
        }

        /// <summary>
        /// Converts a given <see cref="string"/> back to a <see cref="Color"/>
        /// </summary>
        /// <param name="value">The name of the <see cref="Color"/></param>
        /// <param name="targetType"></param>
        /// <param name="parameter">Optional: A <see cref="Dictionary{TKey, TValue}"/></param>
        /// <param name="culture"></param>
        /// <returns><see cref="Color"/></returns>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                return ColorHelper.ColorFromString(text, parameter as Dictionary<Color?, string>) ?? Binding.DoNothing;
            }
            else
            {
                Trace.TraceError($"Unable to convert the provided value '{value}' to System.Windows.Media.Color");
                return Binding.DoNothing;
            }
        }

        /// <summary>
        /// The ConvertBack-Method is not available inside a <see cref="MultiBinding"/>. Use a <see cref="Binding"/> with the optional <see cref="Binding.ConverterParameter"/> instead.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <throws><see cref="NotSupportedException"/></throws>
        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}