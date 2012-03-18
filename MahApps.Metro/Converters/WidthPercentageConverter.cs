namespace MahApps.Metro.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class WidthPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var percentage = Double.Parse(parameter.ToString(), new CultureInfo("en-US"));
            return ((double) value)*percentage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}