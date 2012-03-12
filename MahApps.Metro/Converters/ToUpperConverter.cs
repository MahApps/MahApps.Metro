namespace MahApps.Metro.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class ToUpperConverter : MarkupConverter
    {
        #region Methods

        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string ? ((string)value).ToUpper() : value;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        #endregion
    }

    public class ToLowerConverter : MarkupConverter
    {
        #region Methods

        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string ? ((string)value).ToLower() : value;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        #endregion
    }
}