using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MahApps.Metro.Converters
{
    public class BackgroundToForegroundConverter : IValueConverter, IMultiValueConverter
    {
        private static BackgroundToForegroundConverter _instance;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static BackgroundToForegroundConverter()
        {
        }

        private BackgroundToForegroundConverter()
        {
        }

        public static BackgroundToForegroundConverter Instance => _instance ?? (_instance = new BackgroundToForegroundConverter());

        /// <summary>
        /// Determining Ideal Text Color Based on Specified Background Color
        /// http://www.codeproject.com/KB/GDI-plus/IdealTextColor.aspx
        /// </summary>
        /// <param name = "bg">The background color.</param>
        /// <returns></returns>
        private static Color IdealTextColor(Color bg)
        {
            const int nThreshold = 105; //86;//105;
            var bgDelta = System.Convert.ToInt32((bg.R * 0.299) + (bg.G * 0.587) + (bg.B * 0.114));
            var foreColor = (255 - bgDelta < nThreshold) ? Colors.Black : Colors.White;
            return foreColor;
        }

        /// Second alpha value over first alpha value.
        private static byte CompositeAlpha(byte a1, byte a2)
        {
            return System.Convert.ToByte(255 - ((255 - a2) * (255 - a1)) / 255);
        }

        /// <summary>
        /// For a single R/G/B component. a = precomputed CompositeAlpha(a1, a2)
        /// </summary>
        private static byte CompositeColorComponent(byte c1, byte a1, byte c2, byte a2, byte a)
        {
            // Handle the singular case of both layers fully transparent.
            if (a == 0)
            {
                return 0;
            }

            return System.Convert.ToByte((((255 * c2 * a2) + (c1 * a1 * (255 - a2))) / a) / 255);
        }

        /// <summary>
        /// Second Color over first Color. No range checking.
        /// </summary>
        private static Color CompositeColor(Color firstColor, Color secondColor)
        {
            var firstColorAlpha = firstColor.A;
            var secondColorAlpha = secondColor.A;

            var alpha = CompositeAlpha(firstColorAlpha, secondColorAlpha);

            var r = CompositeColorComponent(firstColor.R, firstColorAlpha, secondColor.R, secondColorAlpha, alpha);
            var g = CompositeColorComponent(firstColor.G, firstColorAlpha, secondColor.G, secondColorAlpha, alpha);
            var b = CompositeColorComponent(firstColor.B, firstColorAlpha, secondColor.B, secondColorAlpha, alpha);

            return Color.FromArgb(alpha, r, g, b);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case Color color:
                {
                    if (parameter is SolidColorBrush bgBrush2)
                    {
                        color = CompositeColor(bgBrush2.Color, color);
                    }
                    else if (parameter is Color color2)
                    {
                        color = CompositeColor(color2, color);
                    }

                    var foreGroundBrush = new SolidColorBrush(IdealTextColor(color));
                    foreGroundBrush.Freeze();
                    return foreGroundBrush;
                }

                case SolidColorBrush brush:
                {
                    var color = brush.Color;

                    if (parameter is SolidColorBrush bgBrush2)
                    {
                        color = CompositeColor(bgBrush2.Color, color);
                    }
                    else if (parameter is Color color2)
                    {
                        color = CompositeColor(color2, color);
                    }

                    var foreGroundBrush = new SolidColorBrush(IdealTextColor(color));
                    foreGroundBrush.Freeze();
                    return foreGroundBrush;
                }

                default:
                    return Brushes.White;
            }
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > 1)
            {
                if (values[1] is SolidColorBrush titleBrush)
                {
                    return titleBrush;
                }

                if (values[0] is SolidColorBrush bgBrush && parameter is SolidColorBrush bgBrush2)
                {
                    var compositeColor = CompositeColor(bgBrush2.Color, bgBrush.Color);
                    return this.Convert(compositeColor, targetType, parameter, culture);
                }
            }

            return this.Convert(values.ElementAtOrDefault(0), targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return targetTypes.Select(t => DependencyProperty.UnsetValue).ToArray();
        }
    }
}