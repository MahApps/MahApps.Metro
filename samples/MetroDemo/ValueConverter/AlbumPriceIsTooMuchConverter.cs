using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MetroDemo.Models;

namespace MetroDemo.ValueConverter
{
    public class AlbumPriceIsTooMuchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal)
            {
                var price = (decimal)value;
                if (price > 15 && price < 20)
                {
                    return true;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class AlbumPriceIsReallyTooMuchValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            var bindingGroup = value as BindingGroup;
            if (bindingGroup != null)
            {
                var album = (Album)bindingGroup.Items[0];
                if (album.Price >= 20)
                {
                    return new ValidationResult(false, string.Format("The price {0} of the album '{1}' by '{2}' is too much!", album.Price, album.Title, album.Artist.Name));
                }
            }
            return ValidationResult.ValidResult;
        }
    }

}