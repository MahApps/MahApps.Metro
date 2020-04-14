using MahApps.Metro.Controls.ColorPicker;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MahApps.Metro.Converters
{
    public class ColorToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return ColorHelper.GetColorName(color);
            }
            else
            {
                throw new InvalidCastException("Unable to convert the provided value to System.Windows.Media.Color");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
