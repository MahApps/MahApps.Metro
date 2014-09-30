using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    public sealed class ResizeModeMinMaxButtonVisibilityConverter : IMultiValueConverter
    {
        private static ResizeModeMinMaxButtonVisibilityConverter _instance;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ResizeModeMinMaxButtonVisibilityConverter()
        {
        }

        private ResizeModeMinMaxButtonVisibilityConverter()
        {
        }

        public static ResizeModeMinMaxButtonVisibilityConverter Instance
        {
            get { return _instance ?? (_instance = new ResizeModeMinMaxButtonVisibilityConverter()); }
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length == 2 && parameter is string)
            {
                var windowPropValue = (bool)values[0];
                var windowResizeMode = (ResizeMode)values[1];
                var whichButton = (string)parameter;
                switch (windowResizeMode)
                {
                    case ResizeMode.NoResize:
                        return Visibility.Collapsed;
                    case ResizeMode.CanMinimize:
                        if (whichButton == "MIN")
                        {
                            return windowPropValue ? Visibility.Visible : Visibility.Collapsed;
                        }
                        return Visibility.Collapsed;
                    case ResizeMode.CanResize:
                    case ResizeMode.CanResizeWithGrip:
                    default:
                        return windowPropValue ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return targetTypes.Select(t => DependencyProperty.UnsetValue).ToArray();
        }
    }
}
