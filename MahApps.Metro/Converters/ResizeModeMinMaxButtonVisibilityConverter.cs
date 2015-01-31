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
            var whichButton = parameter as string;
            if (values != null && !string.IsNullOrEmpty(whichButton))
            {
                var showButton = values.Length > 0 && (bool) values[0];
                var useNoneWindowStyle = values.Length > 1 && (bool)values[1];
                var windowResizeMode = values.Length > 2 ? (ResizeMode)values[2] : ResizeMode.CanResize;

                if (whichButton == "CLOSE")
                {
                    if (useNoneWindowStyle)
                    {
                        return Visibility.Collapsed;
                    }
                    return showButton ? Visibility.Visible : Visibility.Collapsed;
                }

                switch (windowResizeMode)
                {
                    case ResizeMode.NoResize:
                        return Visibility.Collapsed;
                    case ResizeMode.CanMinimize:
                        if (whichButton == "MIN")
                        {
                            if (useNoneWindowStyle)
                            {
                                return Visibility.Collapsed;
                            }
                            return showButton ? Visibility.Visible : Visibility.Collapsed;
                        }
                        return Visibility.Collapsed;
                    case ResizeMode.CanResize:
                    case ResizeMode.CanResizeWithGrip:
                    default:
                        if (useNoneWindowStyle)
                        {
                            return Visibility.Collapsed;
                        }
                        return showButton ? Visibility.Visible : Visibility.Collapsed;
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
