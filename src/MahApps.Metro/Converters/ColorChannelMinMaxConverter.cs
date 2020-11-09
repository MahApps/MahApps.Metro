// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{


    /// <summary>
    /// Converts a given Color to a new Color with the specified Channel turned to the Min or Max Value
    /// </summary>
    [ValueConversion(typeof(Color), typeof(Color))]
    public class ColorChannelMinMaxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return Binding.DoNothing;
            }

            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (value is Color color && parameter is string channel)
            {
                switch (channel.ToLowerInvariant())
                {
                    case "rmin":
                        return Color.FromRgb(0, color.G, color.B);

                    case "rmax":
                        return Color.FromRgb(255, color.G, color.B);

                    case "gmin":
                        return Color.FromRgb(color.R, 0, color.B);

                    case "gmax":
                        return Color.FromRgb(color.R, 255, color.B);

                    case "bmin":
                        return Color.FromRgb(color.R, color.G, 0);

                    case "bmax":
                        return Color.FromRgb(color.R, color.G, 255);

                    case "amin":
                        return Color.FromArgb(0, color.R, color.G, color.B);

                    case "amax":
                        return Color.FromArgb(255, color.R, color.G, color.B);
                    default:
                        throw new InvalidOperationException($"Unexpected value {nameof(parameter)} = {parameter}");
                }
            }
            throw new InvalidOperationException("Unable to convert the given input");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
