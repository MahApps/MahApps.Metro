using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MahApps.Metro.Converters
{
    /// <summary>
    /// Converts a given <see cref="Color"/> into a <see cref="SolidColorBrush"/>.
    /// </summary>
    [ValueConversion(typeof(Color), typeof(SolidColorBrush))]
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        /// <summary>
        /// Gets a static default instance of <see cref="ColorToSolidColorBrushConverter"/>.
        /// </summary>
        public static readonly ColorToSolidColorBrushConverter DefaultInstance = new();

        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
        static ColorToSolidColorBrushConverter()
        {
        }

        /// <summary>
        /// Gets or Sets the brush which will be used if the conversion fails.
        /// </summary>
        public SolidColorBrush? FallbackBrush { get; set; }

        /// <summary>
        /// Gets or Sets the color which will be used if the conversion fails.
        /// </summary>
        public Color? FallbackColor { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is Color color)
            {
                var brush = new SolidColorBrush(color);
                brush.Freeze();
                return brush;
            }

            return this.FallbackBrush;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            return value is SolidColorBrush brush ? brush.Color : this.FallbackColor;
        }
    }
}