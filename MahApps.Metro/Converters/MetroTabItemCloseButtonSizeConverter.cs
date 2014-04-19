using System;
using System.Globalization;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    public class MetroTabItemCloseButtonFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToInt32(value) * 0.7 * 0.85;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToInt32(value) / 0.7 / 0.85;
        }
    }

    public class MetroTabItemCloseButtonWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToInt32(value) * 0.7;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToInt32(value) / 0.7;
        }
    }
}