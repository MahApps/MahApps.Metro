using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using JetBrains.Annotations;

namespace MahApps.Metro.Converters
{
    /// <summary>
    /// Converts a given <see cref="Color"/> into a <see cref="SolidColorBrush"/>.
    /// </summary>
    [ValueConversion(typeof(Color), typeof(SolidColorBrush))]
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        private static ColorToSolidColorBrushConverter defaultInstance;

        /// <summary>
        /// Gets a static instance of the converter if needed.
        /// </summary>
        public static ColorToSolidColorBrushConverter DefaultInstance => defaultInstance ??= new ColorToSolidColorBrushConverter();

        /// <summary>
        /// Gets or Sets the brush which will be used if the conversion fails.
        /// </summary>
        [CanBeNull]
        public SolidColorBrush FallbackBrush { get; set; }

        /// <summary>
        /// Gets or Sets the color which will be used if the conversion fails.
        /// </summary>
        [CanBeNull]
        public Color? FallbackColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                var brush = new SolidColorBrush(color);
                brush.Freeze();
                return brush;
            }

            return this.FallbackBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is SolidColorBrush brush ? brush.Color : this.FallbackColor;
        }
    }
}