using System;
using System.Globalization;
using System.Windows;

namespace MahApps.Metro.Converters
{
    public class NullToUnsetValueConverter : MarkupConverter
    {
        private static NullToUnsetValueConverter _instance;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static NullToUnsetValueConverter()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new NullToUnsetValueConverter());
        }

        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? DependencyProperty.UnsetValue;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}