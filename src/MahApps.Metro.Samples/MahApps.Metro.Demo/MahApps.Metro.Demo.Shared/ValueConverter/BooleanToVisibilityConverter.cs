using System;
using System.Globalization;
using System.Windows.Data;

namespace MetroDemo.ValueConverter
{
    class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() != string.Empty)
            {
                bool isChecked = System.Convert.ToBoolean(value);
                if (isChecked)
                { return "Visible"; }
                else
                { return "Collapsed"; }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
