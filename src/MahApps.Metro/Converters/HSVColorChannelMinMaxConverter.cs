using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MahApps.Metro.Converters
{
    /// <summary>
    /// Converts a given Color to a new Color with the specified Channel turned to the Min or Max Value
    /// </summary>
    [ValueConversion(typeof(HSVColor), typeof(Color))]
    public class HSVColorChannelMinMaxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (value is HSVColor hsv && parameter is string channel)
            {
                switch (channel.ToLowerInvariant())
                {
                    case "smin":
                        return (new HSVColor(hsv.Hue, 0, hsv.Value)).ToColor();

                    case "smax":
                        return (new HSVColor(hsv.Hue, 1, hsv.Value)).ToColor();

                    case "vmin":
                        return (new HSVColor(hsv.Hue, hsv.Saturation, 0)).ToColor();

                    case "vmax":
                        return (new HSVColor(hsv.Hue, hsv.Saturation, 1)).ToColor();

                    case "svmax":
                        return (new HSVColor(hsv.Hue, 1, 1)).ToColor();
                }
            }
            throw new InvalidOperationException("Unable to convert the given input");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
