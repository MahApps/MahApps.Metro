using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace MahApps.Metro.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class ToUpperConverter : MarkupConverter
    {
        private static ToUpperConverter _instance;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ToUpperConverter()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new ToUpperConverter());
        }

        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as string;
            return val != null ? val.ToUpper() : value;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class ToLowerConverter : MarkupConverter
    {
        private static ToLowerConverter _instance;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ToLowerConverter()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new ToLowerConverter());
        }

        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as string;
            return val != null ? val.ToLower() : value;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
