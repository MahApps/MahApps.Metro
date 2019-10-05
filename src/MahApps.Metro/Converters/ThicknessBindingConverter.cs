using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    /// <summary>
    /// Converts a Thickness to a new Thickness. It's possible to ignore a side with the IgnoreThicknessSide property.
    /// </summary>
    public class ThicknessBindingConverter : IValueConverter
    {
        public ThicknessSideType IgnoreThicknessSide { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Thickness thickness)
            {
                // yes, we can override it with the parameter value
                if (parameter is ThicknessSideType sideType)
                {
                    this.IgnoreThicknessSide = sideType;
                }

                switch (this.IgnoreThicknessSide)
                {
                    case ThicknessSideType.Left:
                        return new Thickness(0, thickness.Top, thickness.Right, thickness.Bottom);
                    case ThicknessSideType.Top:
                        return new Thickness(thickness.Left, 0, thickness.Right, thickness.Bottom);
                    case ThicknessSideType.Right:
                        return new Thickness(thickness.Left, thickness.Top, 0, thickness.Bottom);
                    case ThicknessSideType.Bottom:
                        return new Thickness(thickness.Left, thickness.Top, thickness.Right, 0);
                    default:
                        return thickness;
                }
            }

            return default(Thickness);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // for now no back converting
            return DependencyProperty.UnsetValue;
        }
    }
}