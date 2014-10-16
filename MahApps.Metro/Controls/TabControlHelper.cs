using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public static class TabControlHelper
    {
        /// <summary>
        /// Defines whether the underline below the <see cref="TabControl"/> is shown or not.
        /// </summary>
        public static readonly DependencyProperty IsUnderlinedProperty =
            DependencyProperty.RegisterAttached("IsUnderlined", typeof(bool), typeof(TabControlHelper), new PropertyMetadata(false));

        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static bool GetIsUnderlined(UIElement element)
        {
            return (bool)element.GetValue(IsUnderlinedProperty);
        }

        public static void SetIsUnderlined(UIElement element, bool value)
        {
            element.SetValue(IsUnderlinedProperty, value);
        }
    }
}
