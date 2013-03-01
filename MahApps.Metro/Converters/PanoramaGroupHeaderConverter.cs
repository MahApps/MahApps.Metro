using MahApps.Metro.Controls;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    public class PanoramaGroupHeaderConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new Thickness(10, 0, 0, 20);
            var panorama_group_header = (Label)values[0];
            var panorama_left_edge = panorama_group_header.TryFindParent<ScrollViewer>().Margin.Left;
            var panorama_group = panorama_group_header.TryFindParent<DockPanel>();
            var panorama_group_left_edge = panorama_group.TranslatePoint(new Point(0, 0), null).X;
            var panorama_group_header_max_left = (panorama_group.ActualWidth - panorama_group_header.ActualWidth) - 20;
            var proposed_left_margin = Math.Abs(panorama_group_left_edge - panorama_left_edge) + 10;

            // Return (left = 0) if group left margin is beyond panorama left margin
            if (panorama_group_left_edge < panorama_left_edge)
                // Calculate to group header receive how much content is scrolled ...
                // ... except if new position is beyond group width
                result.Left = proposed_left_margin < panorama_group_header_max_left
                    ? proposed_left_margin
                    : panorama_group_header_max_left;

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
