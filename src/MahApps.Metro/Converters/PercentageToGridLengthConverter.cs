﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    public class PercentageToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            bool inverse = (parameter as string)?.ToLowerInvariant() == "true";

            if (value is double)
            {
                if (inverse)
                {
                    value = 1 - (double)value;
                }
                return new GridLength((double)value, GridUnitType.Star);
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
