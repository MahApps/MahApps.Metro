using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    public class MetroAnimatedSingleRowTabControl : BaseMetroTabControl
    {
        public MetroAnimatedSingleRowTabControl()
        {
            DefaultStyleKey = typeof(MetroAnimatedSingleRowTabControl);
        }
    }

    public class MetroAnimatedSingleRowTabControlVerticalScrollMargin : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Visibility)value) == Visibility.Collapsed ? new Thickness(0, 2, 0, 2) : new Thickness(0, 15, 0, 15);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MetroAnimatedSingleRowTabControlHorizontalScrollMargin : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Visibility)value) == Visibility.Collapsed ? new Thickness(2, 2, 2, 0) : new Thickness(15, 2, 15, 0);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
