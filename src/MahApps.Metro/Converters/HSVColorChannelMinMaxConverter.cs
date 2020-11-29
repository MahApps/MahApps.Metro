// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MahApps.Metro.Converters
{
    public enum HSVColorChannelType
    {
        SMin,
        SMax,
        VMin,
        VMax,
        SVMax
    }

    /// <summary>
    /// Converts a given Color to a new Color with the specified Channel turned to the Min or Max Value
    /// </summary>
    [ValueConversion(typeof(HSVColor), typeof(Color))]
    public sealed class HSVColorChannelMinMaxConverter : IValueConverter
    {
        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
        static HSVColorChannelMinMaxConverter()
        {
        }

        /// <summary> Gets the default instance </summary>
        public static HSVColorChannelMinMaxConverter Default { get; } = new HSVColorChannelMinMaxConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is HSVColor hsv && parameter is HSVColorChannelType channel)
            {
                switch (channel)
                {
                    case HSVColorChannelType.SMin: return new HSVColor(hsv.Hue, 0, hsv.Value).ToColor();
                    case HSVColorChannelType.SMax: return new HSVColor(hsv.Hue, 1, hsv.Value).ToColor();
                    case HSVColorChannelType.VMin: return new HSVColor(hsv.Hue, hsv.Saturation, 0).ToColor();
                    case HSVColorChannelType.VMax: return new HSVColor(hsv.Hue, hsv.Saturation, 1).ToColor();
                    case HSVColorChannelType.SVMax: return new HSVColor(hsv.Hue, 1, 1).ToColor();
                    default:
                    {
                        Trace.TraceWarning($"Unexpected value {nameof(parameter)} = {channel}");
                        return Binding.DoNothing;
                    }
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}