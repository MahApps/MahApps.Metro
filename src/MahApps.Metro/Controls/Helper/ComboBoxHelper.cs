using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A helper class that provides various attached properties for the <see cref="ComboBox"/> control.
    /// </summary>
    public class ComboBoxHelper
    {
        // TODO remove this
        public static readonly DependencyProperty EnableVirtualizationWithGroupingProperty
            = DependencyProperty.RegisterAttached("EnableVirtualizationWithGrouping",
                                                  typeof(bool),
                                                  typeof(ComboBoxHelper),
                                                  new FrameworkPropertyMetadata(false, OnEnableVirtualizationWithGroupingChanged));

        private static void OnEnableVirtualizationWithGroupingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var comboBox = dependencyObject as ComboBox;
            if (comboBox != null && e.NewValue != e.OldValue)
            {
                comboBox.SetCurrentValue(VirtualizingStackPanel.IsVirtualizingProperty, e.NewValue);
                comboBox.SetCurrentValue(VirtualizingPanel.IsVirtualizingWhenGroupingProperty, e.NewValue);
                comboBox.SetCurrentValue(ScrollViewer.CanContentScrollProperty, e.NewValue);
            }
        }

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static void SetEnableVirtualizationWithGrouping(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableVirtualizationWithGroupingProperty, value);
        }

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static bool GetEnableVirtualizationWithGrouping(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableVirtualizationWithGroupingProperty);
        }

        public static readonly DependencyProperty MaxLengthProperty
            = DependencyProperty.RegisterAttached("MaxLength",
                                                  typeof(int),
                                                  typeof(ComboBoxHelper),
                                                  new FrameworkPropertyMetadata(0),
                                                  ValidateMaxLength);

        private static bool ValidateMaxLength(object value)
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
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static void SetMaxLength(UIElement obj, int value)
        {
            obj.SetValue(MaxLengthProperty, value);
        }

        public static readonly DependencyProperty CharacterCasingProperty
            = DependencyProperty.RegisterAttached("CharacterCasing",
                                                  typeof(CharacterCasing),
                                                  typeof(ComboBoxHelper),
                                                  new FrameworkPropertyMetadata(CharacterCasing.Normal),
                                                  ValidateCharacterCasing);

        private static bool ValidateCharacterCasing(object value)
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
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static void SetCharacterCasing(UIElement obj, CharacterCasing value)
        {
            obj.SetValue(CharacterCasingProperty, value);
        }
    }
}