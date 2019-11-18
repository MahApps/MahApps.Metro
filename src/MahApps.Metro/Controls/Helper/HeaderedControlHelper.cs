using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    using System.ComponentModel;
    using System.Windows.Documents;

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


        public static readonly DependencyProperty HeaderFontFamilyProperty =
            DependencyProperty.RegisterAttached("HeaderFontFamily", typeof(FontFamily), typeof(HeaderedControlHelper), new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        [AttachedPropertyBrowsableForType(typeof(Flyout))]
        public static FontFamily GetHeaderFontFamily(UIElement element)
        {
            return (FontFamily)element.GetValue(HeaderFontFamilyProperty);
        }

        public static void SetHeaderFontFamily(UIElement element, FontFamily value)
        {
            element.SetValue(HeaderFontFamilyProperty, value);
        }

        public static readonly DependencyProperty HeaderFontSizeProperty =
            DependencyProperty.RegisterAttached("HeaderFontSize", typeof(double), typeof(HeaderedControlHelper), new FrameworkPropertyMetadata(SystemFonts.MessageFontSize));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        [AttachedPropertyBrowsableForType(typeof(Flyout))]
        public static double GetHeaderFontSize(UIElement element)
        {
            return (double)element.GetValue(HeaderFontSizeProperty);
        }

        public static void SetHeaderFontSize(UIElement element, double value)
        {
            element.SetValue(HeaderFontSizeProperty, value);
        }

        public static readonly DependencyProperty HeaderFontStretchProperty =
            DependencyProperty.RegisterAttached("HeaderFontStretch", typeof(FontStretch), typeof(HeaderedControlHelper), new FrameworkPropertyMetadata(TextElement.FontStretchProperty.DefaultMetadata.DefaultValue));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        [AttachedPropertyBrowsableForType(typeof(Flyout))]
        public static FontStretch GetHeaderFontStretch(UIElement element)
        {
            return (FontStretch)element.GetValue(HeaderFontStretchProperty);
        }

        public static void SetHeaderFontStretch(UIElement element, FontStretch value)
        {
            element.SetValue(HeaderFontStretchProperty, value);
        }

        public static readonly DependencyProperty HeaderFontWeightProperty =
            DependencyProperty.RegisterAttached("HeaderFontWeight", typeof(FontWeight), typeof(HeaderedControlHelper), new FrameworkPropertyMetadata(SystemFonts.MessageFontWeight));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        [AttachedPropertyBrowsableForType(typeof(Flyout))]
        public static FontWeight GetHeaderFontWeight(UIElement element)
        {
            return (FontWeight)element.GetValue(HeaderFontWeightProperty);
        }

        public static void SetHeaderFontWeight(UIElement element, FontWeight value)
        {
            element.SetValue(HeaderFontWeightProperty, value);
        }

        public static readonly DependencyProperty HeaderMarginProperty =
            DependencyProperty.RegisterAttached("HeaderMargin", typeof(Thickness), typeof(HeaderedControlHelper), new UIPropertyMetadata(new Thickness()));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(Flyout))]
        public static Thickness GetHeaderMargin(UIElement element)
        {
            return (Thickness)element.GetValue(HeaderMarginProperty);
        }

        public static void SetHeaderMargin(UIElement element, Thickness value)
        {
            element.SetValue(HeaderMarginProperty, value);
        }

    }
}