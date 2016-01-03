using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    using System.ComponentModel;

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

        [Category(AppName.MahApps)]
        public static bool GetEnableVirtualizationWithGrouping(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableVirtualizationWithGroupingProperty);
        }

        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.RegisterAttached("MaxLength", typeof(int), typeof(ComboBoxHelper), new FrameworkPropertyMetadata(0), new ValidateValueCallback(MaxLengthValidateValue));

        private static bool MaxLengthValidateValue(object value)
        {
            return ((int)value) >= 0;
        }

        /// <summary>
        /// Gets the Maximum number of characters the TextBox can accept.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static int GetMaxLength(UIElement obj)
        {
            return (int)obj.GetValue(MaxLengthProperty);
        }

        /// <summary>
        /// Sets the Maximum number of characters the TextBox can accept.
        /// </summary>
        public static void SetMaxLength(UIElement obj, int value)
        {
            obj.SetValue(MaxLengthProperty, value);
        }

        public static readonly DependencyProperty CharacterCasingProperty = DependencyProperty.RegisterAttached("CharacterCasing", typeof(CharacterCasing), typeof(ComboBoxHelper), new FrameworkPropertyMetadata(CharacterCasing.Normal), new ValidateValueCallback(CharacterCasingValidateValue));

        private static bool CharacterCasingValidateValue(object value)
        {
            return (CharacterCasing.Normal <= (CharacterCasing)value && (CharacterCasing)value <= CharacterCasing.Upper);
        }

        /// <summary>
        /// Gets the Character casing of the TextBox.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static CharacterCasing GetCharacterCasing(UIElement obj)
        {
            return (CharacterCasing)obj.GetValue(CharacterCasingProperty);
        }

        /// <summary>
        /// Sets the Character casing of the TextBox.
        /// </summary>
        public static void SetCharacterCasing(UIElement obj, CharacterCasing value)
        {
            obj.SetValue(CharacterCasingProperty, value);
        }
    }
}
