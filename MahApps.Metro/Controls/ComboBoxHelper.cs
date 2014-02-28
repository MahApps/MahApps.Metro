using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A helper class that provides various attached properties for the ComboBox control.
    /// <see cref="ComboBox"/>
    /// </summary>
    public class ComboBoxHelper : DependencyObject
    {
        public static readonly DependencyProperty EnableVirtualizationWithGroupingProperty = DependencyProperty.RegisterAttached("EnableVirtualizationWithGrouping", typeof(bool), typeof(ComboBoxHelper), new FrameworkPropertyMetadata(false));

        public static void SetEnableVirtualizationWithGrouping(DependencyObject obj, bool value)
        {
            if (obj is ComboBox)
            {
#if NET4_5
                ComboBox comboBox = obj as ComboBox;

                comboBox.SetValue(EnableVirtualizationWithGroupingProperty, value);
                comboBox.SetValue(VirtualizingPanel.IsVirtualizingProperty, value);
                comboBox.SetValue(VirtualizingPanel.IsVirtualizingWhenGroupingProperty, value);
#else
                obj.SetValue(EnableVirtualizationWithGroupingProperty, false);
#endif
            }
        }

        public static bool GetEnableVirtualizationWithGrouping(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableVirtualizationWithGroupingProperty);
        }
    }
}
