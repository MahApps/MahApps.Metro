namespace MahApps.Metro.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class PartToDegreeConverter : IValueConverter
    {
        public double TotalParts { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 360 / TotalParts * (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}