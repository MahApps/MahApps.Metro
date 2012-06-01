using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    public class PanoramaGroupWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var itemBox = double.Parse(values[0].ToString());
            var groupHeight = double.Parse(values[1].ToString());

            double ratio = groupHeight / itemBox;
            var list = (ListBox)values[2];

            double width = Math.Ceiling(list.Items.Count / ratio);
            width *= itemBox;
            return width < itemBox ? itemBox : width;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
