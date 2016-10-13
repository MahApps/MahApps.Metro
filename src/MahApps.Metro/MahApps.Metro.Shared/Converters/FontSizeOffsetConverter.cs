using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    // this converter is only used by DatePicker to convert the font size to width and height of the icon button
    public class FontSizeOffsetConverter : IValueConverter
    {
        private static FontSizeOffsetConverter _instance;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static FontSizeOffsetConverter()
        {
        }

        private FontSizeOffsetConverter()
        {
        }

        public static FontSizeOffsetConverter Instance
        {
            get { return _instance ?? (_instance = new FontSizeOffsetConverter()); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double && parameter is double) {
                var offset = (double)parameter;
                var orgValue = (double)value;
                return Math.Round(orgValue + offset);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}