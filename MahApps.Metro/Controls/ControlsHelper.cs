using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public static class ControlsHelper
    {
        public static readonly DependencyProperty GroupBoxHeaderForegroundProperty =
            DependencyProperty.RegisterAttached("GroupBoxHeaderForeground", typeof(Brush), typeof(ControlsHelper), new UIPropertyMetadata(Brushes.White));

        [AttachedPropertyBrowsableForType(typeof(GroupBox))]
        public static Brush GetGroupBoxHeaderForeground(UIElement element)
        {
            return (Brush)element.GetValue(GroupBoxHeaderForegroundProperty);
        }

        public static void SetGroupBoxHeaderForeground(UIElement element, Brush value)
        {
            element.SetValue(GroupBoxHeaderForegroundProperty, value);
        }

        public static readonly DependencyProperty HeaderFontSizeProperty =
            DependencyProperty.RegisterAttached("HeaderFontSize", typeof(double), typeof(ControlsHelper), new UIPropertyMetadata(26.67, HeaderFontSizePropertyChangedCallback));

        private static void HeaderFontSizePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is double)
            {
                // close button only for MetroTabItem
                var metroTabItem = dependencyObject as MetroTabItem;
                if (metroTabItem == null)
                {
                    return;
                }

                if (metroTabItem.closeButton == null)
                {
                    metroTabItem.ApplyTemplate();
                }

                if (metroTabItem.closeButton != null && metroTabItem.contentSite != null)
                {
                    // punker76: i don't like this! i think this must be done with xaml.
                    var fontDpiSize = (double)e.NewValue;
                    var fontHeight = Math.Ceiling(fontDpiSize * metroTabItem.FontFamily.LineSpacing);
                    var newMargin = (Math.Round(fontHeight) / 2.8)
                                    - metroTabItem.Padding.Top - metroTabItem.Padding.Bottom
                                    - metroTabItem.contentSite.Margin.Top - metroTabItem.contentSite.Margin.Bottom;

                    var previousMargin = metroTabItem.closeButton.Margin;
                    metroTabItem.newButtonMargin = new Thickness(previousMargin.Left, newMargin, previousMargin.Right, previousMargin.Bottom);
                    metroTabItem.closeButton.Margin = metroTabItem.newButtonMargin;

                    metroTabItem.closeButton.UpdateLayout();
                }
            }
        }

        [AttachedPropertyBrowsableForType(typeof(MetroTabItem))]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        [AttachedPropertyBrowsableForType(typeof(GroupBox))]
        public static double GetHeaderFontSize(UIElement element)
        {
            return (double)element.GetValue(HeaderFontSizeProperty);
        }

        public static void SetHeaderFontSize(UIElement element, double value)
        {
            element.SetValue(HeaderFontSizeProperty, value);
        }
    }
}
