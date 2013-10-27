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
    }
}
