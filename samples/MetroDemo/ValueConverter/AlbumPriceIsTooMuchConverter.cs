using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MetroDemo.ValueConverter
{
    public class AlbumPriceIsTooMuchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal)
            {
                var price = (decimal)value;
                if (price > 15)
                {
                    return true;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}