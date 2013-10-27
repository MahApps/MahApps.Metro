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

                if (metroTabItem.CloseButton == null)
                {
                    metroTabItem.ApplyTemplate();
                }

                if (metroTabItem.CloseButton != null && metroTabItem.Label != null)
                {
                    var fontDpiSize = (double)e.NewValue;
                    var fontHeight = Math.Ceiling(fontDpiSize * metroTabItem.Label.FontFamily.LineSpacing);
                    var newMargin = (Math.Round(fontHeight) / 2.2) - (metroTabItem.Label.Padding.Top);

                    var previousMargin = metroTabItem.CloseButton.Margin;
                    metroTabItem.newButtonMargin = new Thickness(previousMargin.Left, newMargin, previousMargin.Right, previousMargin.Bottom);
                    metroTabItem.CloseButton.Margin = metroTabItem.newButtonMargin;

                    metroTabItem.CloseButton.UpdateLayout();
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
