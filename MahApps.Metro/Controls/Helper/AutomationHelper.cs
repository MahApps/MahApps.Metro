using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    using System.ComponentModel;
    using System.Windows.Automation;
    /// <summary>
    /// A helper class that provides a way to set de AutomationId and Automation Name based on the templated parent name
    /// </summary>
    /// <remarks>
    /// If the value is set the AutomationId and Automation name will be set with the format {templated parent name}_{value}
    /// </remarks>
    public class AutomationHelper
    {
        public static readonly DependencyProperty DefaultAutomationIdProperty = DependencyProperty.RegisterAttached("DefaultAutomationId", typeof(string), typeof(AutomationHelper), new FrameworkPropertyMetadata(string.Empty, AutomationIdChanged));
        /// <summary>
        /// Gets if the attached TextBox has text.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Control))]
        public static string GetDefaultAutomationId(DependencyObject obj)
        {
            return (string)obj.GetValue(DefaultAutomationIdProperty);
        }

        public static void SetDefaultAutomationId(DependencyObject obj, string value)
        {
            obj.SetValue(DefaultAutomationIdProperty, value);
        }

        private static void AutomationIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var current = AutomationProperties.GetAutomationId(d);
            if (current.Length > 0)
            {
                return;
            }
            var element = d as FrameworkElement;
            if (element == null)
            {
                return;
            }
            var parent= element.TemplatedParent as FrameworkElement;
            if (parent == null)
            {
                return;
            }
            var parentName = parent.Name;
            if (string.IsNullOrWhiteSpace(parentName))
            {
                return;
            }
            element.SetValue(AutomationProperties.AutomationIdProperty, string.Format("{0}_{1}", parentName, e.NewValue));
            element.SetValue(AutomationProperties.NameProperty, string.Format("{0}_{1}", parentName, e.NewValue));


        }
      
    }
}
