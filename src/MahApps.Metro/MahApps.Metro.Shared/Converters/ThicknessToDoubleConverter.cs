namespace MahApps.Metro.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof(Thickness), typeof(double), ParameterType = typeof(ThicknessSideType))]
    public class ThicknessToDoubleConverter : IValueConverter
    {
        public ThicknessSideType TakeThicknessSide { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Thickness)
            {
                // yes, we can override it with the parameter value
                if (parameter is ThicknessSideType)
                {
                    this.TakeThicknessSide = (ThicknessSideType)parameter;
                }
                var orgThickness = (Thickness)value;
                switch (this.TakeThicknessSide)
                {
                    case ThicknessSideType.Left:
                        return orgThickness.Left;
                    case ThicknessSideType.Top:
                        return orgThickness.Top;
                    case ThicknessSideType.Right:
                        return orgThickness.Right;
                    case ThicknessSideType.Bottom:
                        return orgThickness.Bottom;
                    default:
                        return default(double);
                }
            }

            return default(double);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}