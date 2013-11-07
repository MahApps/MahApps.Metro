using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    public class PanoramaGroupHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var itemBox = double.Parse(values[0].ToString());
            var groupHeight = double.Parse(values[1].ToString());
            var headerHeight = double.Parse(values[2].ToString());

            return (Math.Floor((groupHeight - headerHeight) / itemBox) * itemBox);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return targetTypes.Select(t => DependencyProperty.UnsetValue).ToArray();
        }
    }
}