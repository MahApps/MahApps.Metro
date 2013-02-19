using System;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class TabItemHelper : DependencyObject
    {
        public static readonly DependencyProperty HeaderFontSizeProperty = DependencyProperty.RegisterAttached("HeaderFontSize", typeof(double), typeof(TabItemHelper), new PropertyMetadata(26.67, new PropertyChangedCallback(HeaderFontSizeChanged)));

        private static void HeaderFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((Control)d).IsLoaded) //Check if the tabitem is loaded.
            {
                SetHeaderLabelFontSize(d, e); //The tabitem is loaded. Set the font size immediately.
            }
            else
            {
                //Tabitem hasn't loaded yet. Attach a temporary event handler to set the font size once it loads.
                RoutedEventHandler handler = null; handler = delegate(object sender, RoutedEventArgs args)
                   {
                       SetHeaderLabelFontSize(d, e);

                       ((Control)d).Loaded -= handler;
                   };
                ((Control)d).Loaded += handler;
            }
        }

        private static void SetHeaderLabelFontSize(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = ((Control)d).Template.FindName("root", (FrameworkElement)d) as Label;
            obj.FontSize = (double)e.NewValue;
        }

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
