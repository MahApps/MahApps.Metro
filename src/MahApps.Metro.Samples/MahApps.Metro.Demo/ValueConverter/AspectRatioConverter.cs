// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace MetroDemo.ValueConverter
{
    /// <summary>
    /// Turns a width into the height that keeps a given shape, so an element bound to its own
    /// ActualWidth grows and shrinks without ever changing its proportions.
    /// </summary>
    public class AspectRatioConverter
        : MarkupExtension, IValueConverter
    {
        /// <summary>How tall the thing is for every unit of width. 0.75 is four by three.</summary>
        public double Ratio { get; set; } = 0.75;

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var width = System.Convert.ToDouble(value, culture);

            return double.IsNaN(width) || double.IsInfinity(width) || width <= 0
                ? double.NaN
                : width * this.Ratio;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("A height only ever comes out of this, never goes back in.");
        }
    }
}
