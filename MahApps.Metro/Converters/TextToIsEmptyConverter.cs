//------------------------------------------------------------------------------
//  File    : TextToIsEmptyConverter.cs
//  Author  : Mohammad Rahhal
//  Created : 17/8/2014 4:51:27 PM
//------------------------------------------------------------------------------

namespace MahApps.Metro.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Data;

    public class TextToIsEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string text = (string)value;
            return string.IsNullOrWhiteSpace(text);
        }

        // Doesn't need to be implemented.
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}