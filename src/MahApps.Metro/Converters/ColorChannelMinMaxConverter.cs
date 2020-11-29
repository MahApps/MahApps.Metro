// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    /// <summary>
    /// Channel type for ColorChannelMinMaxConverter to pass in in as parameter.
    /// </summary>
    public enum ColorChannelType
    {
        RMin,
        RMax,
        GMin,
        GMax,
        BMin,
        BMax,
        AMin,
        AMax
    }

    /// <summary>
    /// Converts a given Color to a new Color with the specified Channel turned to the Min or Max Value
    /// </summary>
    [ValueConversion(typeof(Color), typeof(Color))]
    public sealed class ColorChannelMinMaxConverter : IValueConverter
    {
        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
        static ColorChannelMinMaxConverter()
        {
        }

        /// <summary> Gets the default instance </summary>
        public static ColorChannelMinMaxConverter Default { get; } = new ColorChannelMinMaxConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color && parameter is ColorChannelType channel)
            {
                switch (channel)
                {
                    case ColorChannelType.RMin: return Color.FromRgb(0, color.G, color.B);
                    case ColorChannelType.RMax: return Color.FromRgb(255, color.G, color.B);
                    case ColorChannelType.GMin: return Color.FromRgb(color.R, 0, color.B);
                    case ColorChannelType.GMax: return Color.FromRgb(color.R, 255, color.B);
                    case ColorChannelType.BMin: return Color.FromRgb(color.R, color.G, 0);
                    case ColorChannelType.BMax: return Color.FromRgb(color.R, color.G, 255);
                    case ColorChannelType.AMin: return Color.FromArgb(0, color.R, color.G, color.B);
                    case ColorChannelType.AMax: return Color.FromArgb(255, color.R, color.G, color.B);
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