using System;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A helper class that provides various attached properties for the ComboBox control.
    /// <see cref="ComboBox"/>
    /// </summary>
    public class ComboBoxHelper
    {
        public static readonly DependencyProperty EnableVirtualizationWithGroupingProperty = DependencyProperty.RegisterAttached("EnableVirtualizationWithGrouping", typeof(bool), typeof(ComboBoxHelper), new FrameworkPropertyMetadata(false, EnableVirtualizationWithGroupingPropertyChangedCallback));

        private static void EnableVirtualizationWithGroupingPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var comboBox = dependencyObject as ComboBox;
            if (comboBox != null && e.NewValue != e.OldValue)
            {
#if NET4_5
                comboBox.SetValue(VirtualizingStackPanel.IsVirtualizingProperty, e.NewValue);
                comboBox.SetValue(VirtualizingPanel.IsVirtualizingWhenGroupingProperty, e.NewValue);
                comboBox.SetValue(ScrollViewer.CanContentScrollProperty, e.NewValue);
#endif
            }
        }

        public static void SetEnableVirtualizationWithGrouping(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableVirtualizationWithGroupingProperty, value);
        }

        public static bool GetEnableVirtualizationWithGrouping(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableVirtualizationWithGroupingProperty);
        }
    }
}
