using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{    
    public class InvertedBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            var shouldBeCollapsed = false;
            
            if(value is bool)
            {
                shouldBeCollapsed = (bool)value;
            }
            else if(value is bool?)
            {
                var valueAsNullable = (bool?)value;
                shouldBeCollapsed = valueAsNullable.HasValue ? valueAsNullable.Value : false;
            }

            return shouldBeCollapsed ? Visibility.Collapsed : Visibility.Hidden;
            

        }
     
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                return (Visibility)value == Visibility.Collapsed;
            }
            else
            {
                return false;
            }
        }
    }
}
