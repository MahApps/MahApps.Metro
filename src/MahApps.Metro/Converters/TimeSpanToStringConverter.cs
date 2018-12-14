namespace MahApps.Metro.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    [ValueConversion(typeof(TimeSpan?), typeof(string))]
    internal class TimeSpanToStringConverter : IValueConverter
    {
        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan? timeSpan = value as TimeSpan?;
            if (timeSpan == null)
            {
                return null;
            }

            var convert = DateTime.MinValue.Add(timeSpan.GetValueOrDefault()).ToString(culture.DateTimeFormat.LongTimePattern, culture);
            return convert;
        }

        /// <summary>Converts a value. </summary>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dateTime;
            if (DateTime.TryParseExact(value.ToString(), culture.DateTimeFormat.LongTimePattern, culture, DateTimeStyles.None, out dateTime))
            {
                return dateTime.TimeOfDay;
            }

            return null;
        }
    }
}