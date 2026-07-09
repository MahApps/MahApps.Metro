// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Windows.Media;
using System.Globalization;
using System.Windows;
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
        R,
        GMin,
        GMax,
        G,
        BMin,
        BMax,
        B,
        AMin,
        AMax,
        A
    }

    /// <summary>
    /// Converts a given Color to a new Color with the specified Channel turned to the Min or Max Value
    /// </summary>
    [ValueConversion(typeof(Color), typeof(Color), ParameterType = typeof(ColorChannelType))]
    public sealed class ColorChannelMinMaxConverter : IValueConverter
    {
        /// <summary>
        /// Gets a static default instance of <see cref="ColorChannelMinMaxConverter"/>.
        /// </summary>
        public static readonly ColorChannelMinMaxConverter Default = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
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

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts a given Color to a new LinearGradientBrush with the specified Channel.
    /// </summary>
    [ValueConversion(typeof(Color), typeof(Brush))]
    public sealed class ColorChannel2GradientBrushConverter : IValueConverter
    {
        /// <summary> Gets the default instance </summary>
        public static ColorChannel2GradientBrushConverter Default { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is ColorChannelType channel)
            {
                object? minResult;
                object? maxResult;

                switch (channel)
                {
                    case ColorChannelType.R:
                        minResult = ColorChannelMinMaxConverter.Default.Convert(value, targetType, ColorChannelType.RMin, culture);
                        maxResult = ColorChannelMinMaxConverter.Default.Convert(value, targetType, ColorChannelType.RMax, culture);
                        break;
                    case ColorChannelType.G:
                        minResult = ColorChannelMinMaxConverter.Default.Convert(value, targetType, ColorChannelType.GMin, culture);
                        maxResult = ColorChannelMinMaxConverter.Default.Convert(value, targetType, ColorChannelType.GMax, culture);
                        break;
                    case ColorChannelType.B:
                        minResult = ColorChannelMinMaxConverter.Default.Convert(value, targetType, ColorChannelType.BMin, culture);
                        maxResult = ColorChannelMinMaxConverter.Default.Convert(value, targetType, ColorChannelType.BMax, culture);
                        break;
                    case ColorChannelType.A:
                        minResult = ColorChannelMinMaxConverter.Default.Convert(value, targetType, ColorChannelType.AMin, culture);
                        maxResult = ColorChannelMinMaxConverter.Default.Convert(value, targetType, ColorChannelType.AMax, culture);
                        break;
                    default:
                    {
                        Trace.TraceWarning($"Unexpected value {nameof(parameter)} = {channel}");
                        return Binding.DoNothing;
                    }
                }

                if (minResult is Color minColor && maxResult is Color maxColor)
                {
                    var brush = new LinearGradientBrush
                                {
                                    StartPoint = new Point(0, 0.5),
                                    EndPoint = new Point(1, 0.5)
                                };
                    brush.GradientStops.Add(new GradientStop(minColor, 0));
                    brush.GradientStops.Add(new GradientStop(maxColor, 1));
                    brush.Freeze();
                    return brush;
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