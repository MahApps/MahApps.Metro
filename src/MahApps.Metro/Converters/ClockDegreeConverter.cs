namespace MahApps.Metro.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Converts a double representing either hour/minute/second to the corresponding angle.
    /// </summary>
    [ValueConversion(typeof(double), typeof(double))]
    public class ClockDegreeConverter : IValueConverter
    {
        public double TotalParts { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }
            if (value is DateTime)
            {
                var dateTime = (DateTime)value;

                switch ((string)parameter)
                {
                    case "h":
                        return 360.0 / 12 * dateTime.TimeOfDay.TotalHours;
                    case "m":
                        return 360.0 / 60 * dateTime.TimeOfDay.TotalMinutes;
                    case "s":
                        return 360.0 / 60 * dateTime.TimeOfDay.Seconds;
                    default:
                        throw new ArgumentException("must be \"h\", \"m\", or \"s", nameof(parameter));
                }
            }

            if (value is int)
            {
                return 360 / TotalParts * (int)value;
            }
            return 360 / TotalParts * (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}