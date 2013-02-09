using System;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class TabItemHelper : DependencyObject
    {
        public static readonly DependencyProperty HeaderFontSizeProperty = DependencyProperty.RegisterAttached("HeaderFontSize", typeof(double), typeof(TabItemHelper), new PropertyMetadata(26.67));

        public static double GetHeaderFontSize(DependencyObject obj)
        {
            return (double)obj.GetValue(HeaderFontSizeProperty);
        }

        public static void SetHeaderFontSize(DependencyObject obj, double value)
        {
            obj.SetValue(HeaderFontSizeProperty, value);
        }
    }
}
