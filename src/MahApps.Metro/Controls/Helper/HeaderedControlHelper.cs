using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    using System.ComponentModel;

    public static class HeaderedControlHelper
    {
        public static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty.RegisterAttached("HeaderForeground", typeof(Brush), typeof(HeaderedControlHelper), new UIPropertyMetadata(Brushes.White));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        public static Brush GetHeaderForeground(UIElement element)
        {
            return (Brush)element.GetValue(HeaderForegroundProperty);
        }

        public static void SetHeaderForeground(UIElement element, Brush value)
        {
            element.SetValue(HeaderForegroundProperty, value);
        }

        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.RegisterAttached("HeaderBackground", typeof(Brush), typeof(HeaderedControlHelper), new UIPropertyMetadata(Panel.BackgroundProperty.DefaultMetadata.DefaultValue));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        public static Brush GetHeaderBackground(UIElement element)
        {
            return (Brush)element.GetValue(HeaderBackgroundProperty);
        }

        public static void SetHeaderBackground(UIElement element, Brush value)
        {
            element.SetValue(HeaderBackgroundProperty, value);
        }
    }
}