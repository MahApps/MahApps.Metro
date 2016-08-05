using System;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    /// <summary>
    /// Converts the value from true to false and false to true.
    /// </summary>
    public sealed class IsNullConverter : IValueConverter
    {
        private static IsNullConverter _instance;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static IsNullConverter()
        {
        }

        private IsNullConverter()
        {
        }

        public static IsNullConverter Instance
        {
            get { return _instance ?? (_instance = new IsNullConverter()); }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null == value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}