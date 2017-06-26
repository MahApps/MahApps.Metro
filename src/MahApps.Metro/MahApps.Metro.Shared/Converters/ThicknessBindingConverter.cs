using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    /// <summary>
    /// Converts a Thickness to a new Thickness. It's possible to ignore a side With the IgnoreThicknessSide property.
    /// </summary>
    public class ThicknessBindingConverter : IValueConverter
    {
        public ThicknessSideType IgnoreThicknessSide { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Thickness)
            {
                // yes, we can override it with the parameter value
                if (parameter is ThicknessSideType)
                {
                    this.IgnoreThicknessSide = (ThicknessSideType)parameter;
                }
                var orgThickness = (Thickness)value;
                switch (this.IgnoreThicknessSide)
                {
                    case ThicknessSideType.Left:
                        return new Thickness(0, orgThickness.Top, orgThickness.Right, orgThickness.Bottom);
                    case ThicknessSideType.Top:
                        return new Thickness(orgThickness.Left, 0, orgThickness.Right, orgThickness.Bottom);
                    case ThicknessSideType.Right:
                        return new Thickness(orgThickness.Left, orgThickness.Top, 0, orgThickness.Bottom);
                    case ThicknessSideType.Bottom:
                        return new Thickness(orgThickness.Left, orgThickness.Top, orgThickness.Right, 0);
                    default:
                        return orgThickness;
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

    [ValueConversion(typeof(Thickness), typeof(double), ParameterType = typeof(ThicknessSideType))]
    public class BorderThicknessToStrokeThicknessConverter : IValueConverter
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
            throw new InvalidOperationException("Cannot convert back to source");
        }
    }

    public enum ThicknessSideType
    {
        /// <summary>
        /// Use all sides.
        /// </summary>
        None,
        /// <summary>
        /// Ignore the left side.
        /// </summary>
        Left,
        /// <summary>
        /// Ignore the top side.
        /// </summary>
        Top,
        /// <summary>
        /// Ignore the right side.
        /// </summary>
        Right,
        /// <summary>
        /// Ignore the bottom side.
        /// </summary>
        Bottom
    }
}