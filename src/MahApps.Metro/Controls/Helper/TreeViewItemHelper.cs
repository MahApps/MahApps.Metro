using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public static class TreeViewItemHelper
    {
        public static readonly DependencyProperty ToggleButtonStyleProperty
            = DependencyProperty.RegisterAttached(
                "ToggleButtonStyle",
                typeof(Style),
                typeof(TreeViewItemHelper),
                new FrameworkPropertyMetadata((Style)null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets the toggle button style used for the TreeViewItem expander.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static Style GetToggleButtonStyle(UIElement element)
        {
            return (Style)element.GetValue(ToggleButtonStyleProperty);
        }

        /// <summary>
        /// Sets the toggle button style used for the TreeViewItem expander.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
        public static void SetToggleButtonStyle(UIElement element, Style value)
        {
            element.SetValue(ToggleButtonStyleProperty, value);
        }
    }
}