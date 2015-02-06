using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var invisibleMeansHidden = "hidden".Equals(parameter as string, StringComparison.InvariantCultureIgnoreCase);
            var invisibility = invisibleMeansHidden ? Visibility.Hidden : Visibility.Collapsed;

            var str = value as string;
            if (string.IsNullOrEmpty(str))
                return invisibility;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
