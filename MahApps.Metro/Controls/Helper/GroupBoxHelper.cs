using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    using System.ComponentModel;

    public static class GroupBoxHelper
    {
        public static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty.RegisterAttached("HeaderForeground", typeof(Brush), typeof(GroupBoxHelper), new UIPropertyMetadata(Brushes.White));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(GroupBox))]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static Brush GetHeaderForeground(UIElement element)
        {
            return (Brush)element.GetValue(HeaderForegroundProperty);
        }

        public static void SetHeaderForeground(UIElement element, Brush value)
        {
            element.SetValue(HeaderForegroundProperty, value);
        }
    }
}
