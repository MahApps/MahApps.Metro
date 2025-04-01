// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MahApps.Metro.ValueBoxes;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A helper class that provides various attached properties for the <see cref="ComboBox"/> control.
    /// </summary>
    public class ComboBoxHelper
    {
        public static readonly DependencyProperty MaxLengthProperty
            = DependencyProperty.RegisterAttached("MaxLength",
                                                  typeof(int),
                                                  typeof(ComboBoxHelper),
                                                  new FrameworkPropertyMetadata(0),
                                                  value => (int)value >= 0);

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
                                                  value => CharacterCasing.Normal <= (CharacterCasing)value && (CharacterCasing)value <= CharacterCasing.Upper);

        /// <summary>
        /// Gets the Character casing of the inner TextBox.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static CharacterCasing GetCharacterCasing(UIElement obj)
        {
            return (CharacterCasing)obj.GetValue(CharacterCasingProperty);
        }

        /// <summary>
        /// Sets the Character casing of the inner TextBox.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static void SetCharacterCasing(UIElement obj, CharacterCasing value)
        {
            obj.SetValue(CharacterCasingProperty, value);
        }


        /// <summary>Identifies the InterceptMouseWheelSelection attached dependcy property.</summary>
        public static readonly DependencyProperty InterceptMouseWheelSelectionProperty = DependencyProperty.RegisterAttached("InterceptMouseWheelSelection", typeof(bool), typeof(ComboBoxHelper), new PropertyMetadata(BooleanBoxes.TrueBox, OnInterceptMouseWheelSelectionPropertyChangedCallback));


        /// <summary>
        /// Gets if the given <see cref="ComboBox"/> accepts selection via mouse wheel.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static bool GetInterceptMouseWheelSelection(DependencyObject obj)
        {
            return (bool)obj.GetValue(InterceptMouseWheelSelectionProperty);
        }

        /// <summary>
        /// Sets if the given <see cref="ComboBox"/> accepts selection via mouse wheel. The default is true.  
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static void SetInterceptMouseWheelSelection(ComboBox obj, bool value)
        {
            obj.SetValue(InterceptMouseWheelSelectionProperty, BooleanBoxes.Box(value));
        }

        private static void OnInterceptMouseWheelSelectionPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ComboBox comboBox && e.NewValue != e.OldValue && e.NewValue is bool)
            {
                comboBox.PreviewMouseWheel -= ComboBox_PreviewMouseWheel;

                // If this property is set to false we need to handle the MouseWheel before the ComboBox does. 
                if (!((bool)e.NewValue))
                {
                    comboBox.PreviewMouseWheel += ComboBox_PreviewMouseWheel;
                }
            }
        }

        private static void ComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is MultiSelectionComboBox)
            {
                // This will be handled by MultiSelectionComboBox directly so we don't need to do anything here. 
            }
            else if (sender is ComboBox comboBox)
            {
                // mark the event as handled to cancel selection. We should not handle it if the drop down is open. 
                e.Handled = !comboBox.IsDropDownOpen;
            }
        }
    }
}