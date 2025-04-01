// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MahApps.Metro.Converters
{
    public enum HSVColorChannelType
    {
        SMin,
        SMax,
        S,
        VMin,
        VMax,
        V,
        SVMax
    }

    /// <summary>
    /// Converts a given Color to a new Color with the specified Channel turned to the Min or Max Value
    /// </summary>
    [ValueConversion(typeof(HSVColor), typeof(Color), ParameterType = typeof(HSVColorChannelType))]
    public sealed class HSVColorChannelMinMaxConverter : IValueConverter
    {
        /// <summary>
        /// Gets a static default instance of <see cref="HSVColorChannelMinMaxConverter"/>.
        /// </summary>
        public static readonly HSVColorChannelMinMaxConverter Default = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is HSVColor hsv && parameter is HSVColorChannelType channel)
            {
                switch (channel)
                {
                    case HSVColorChannelType.SMin: return new HSVColor(hsv.Hue, 0, hsv.Value).ToColor();
                    case HSVColorChannelType.SMax: return new HSVColor(hsv.Hue, 1, hsv.Value).ToColor();
                    case HSVColorChannelType.S: return new HSVColor(hsv.Hue, hsv.Saturation, hsv.Value).ToColor();
                    case HSVColorChannelType.VMin: return new HSVColor(hsv.Hue, hsv.Saturation, 0).ToColor();
                    case HSVColorChannelType.VMax: return new HSVColor(hsv.Hue, hsv.Saturation, 1).ToColor();
                    case HSVColorChannelType.V: return new HSVColor(hsv.Hue, hsv.Saturation, hsv.Value).ToColor();
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

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts a given HSVColor to a new SolidColorBrush with the specified Channel.
    /// </summary>
    [ValueConversion(typeof(HSVColor), typeof(Brush), ParameterType = typeof(HSVColorChannelType))]
    public sealed class HSVColorChannel2BrushConverter : IValueConverter
    {
        /// <summary> Gets the default instance </summary>
        public static HSVColorChannel2BrushConverter Default { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = HSVColorChannelMinMaxConverter.Default.Convert(value, targetType, parameter, culture);
            if (result is Color color)
            {
                var brush = new SolidColorBrush(color);
                brush.Freeze();
                return brush;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts a given HSVColor to a new LinearGradientBrush with the specified Channel.
    /// </summary>
    [ValueConversion(typeof(HSVColor), typeof(Brush), ParameterType = typeof(HSVColorChannelType))]
    public sealed class HSVColorChannel2GradientBrushConverter : IValueConverter
    {
        /// <summary> Gets the default instance </summary>
        public static HSVColorChannel2GradientBrushConverter Default { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is HSVColorChannelType channel)
            {
                object? minResult;
                object? maxResult;

                switch (channel)
                {
                    case HSVColorChannelType.S:
                        minResult = HSVColorChannelMinMaxConverter.Default.Convert(value, targetType, HSVColorChannelType.SMin, culture);
                        maxResult = HSVColorChannelMinMaxConverter.Default.Convert(value, targetType, HSVColorChannelType.SMax, culture);
                        break;
                    case HSVColorChannelType.V:
                        minResult = HSVColorChannelMinMaxConverter.Default.Convert(value, targetType, HSVColorChannelType.VMin, culture);
                        maxResult = HSVColorChannelMinMaxConverter.Default.Convert(value, targetType, HSVColorChannelType.VMax, culture);
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