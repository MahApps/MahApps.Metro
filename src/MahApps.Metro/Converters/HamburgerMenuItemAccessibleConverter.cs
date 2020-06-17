using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace MahApps.Metro.Converters
{
    [ValueConversion(typeof(object), typeof(object))]
    [MarkupExtensionReturnType(typeof(HamburgerMenuItemAccessibleConverter))]
    public class HamburgerMenuItemAccessibleConverter : IValueConverter, IMultiValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return Binding.DoNothing;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null)
            {
                return Binding.DoNothing;
            }

            var automationPropertiesValue = values.ElementAtOrDefault(1) as string;
            if (!string.IsNullOrEmpty(automationPropertiesValue))
            {
                return automationPropertiesValue;
            }

            return this.Convert(values.ElementAtOrDefault(0), targetType, parameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return targetTypes.Select(t => Binding.DoNothing).ToArray();
        }
    }
}