using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MahApps.Metro.Converters
{
    /// <summary>
    /// Converts a given <see cref="Color"/> into a <see cref="SolidColorBrush"/>.
    /// </summary>
    [ValueConversion (typeof(Color), typeof(SolidColorBrush)) ]
    public class ColorToSolidColorBrushConverter : MarkupConverter
    {
        static ColorToSolidColorBrushConverter _DefaultInstance;

        /// <summary>
        /// returns a static instance if needed.
        /// </summary>
        public static ColorToSolidColorBrushConverter DefaultInstance => _DefaultInstance ??= new ColorToSolidColorBrushConverter();

        /// <summary>
        /// Gets or Sets the FallbackBrush which should be used if the conversion fails. 
        /// </summary>
        public SolidColorBrush FallbackBrush { get; set; }

        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                var brush = new SolidColorBrush(color);
                brush.Freeze();
                return brush;
            }
            else
            {
                return FallbackBrush;
            }
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                return brush.Color;
            }
            else
            {
                return null;
            }
        }
    }
}
