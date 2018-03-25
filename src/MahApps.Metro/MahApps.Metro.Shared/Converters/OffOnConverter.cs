using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Converters
{
    [Obsolete(@"This converter will be deleted in the next release.")]
    public class OffOnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var t = (ToggleSwitch)parameter;

            return t.IsChecked == true ? t.OnLabel : t.OffLabel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}