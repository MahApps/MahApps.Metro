// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A helper class that provides various attached properties for the <see cref="ComboBox"/> control.
    /// </summary>
    public class ComboBoxHelper
    {
        public static readonly DependencyProperty DropDownBackgroundProperty
            = DependencyProperty.RegisterAttached("DropDownBackground",
                                                  typeof(Brush),
                                                  typeof(ComboBoxHelper),
                                                  new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the brush the drop-down is filled with.
        /// </summary>
        /// <remarks>
        /// The list hangs over whatever is behind it, so it carries a fill and a frame of its own,
        /// and both belong to the box rather than to controls in general: a set that gives its text
        /// boxes a chrome of their own wants the same one here.
        /// </remarks>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static Brush? GetDropDownBackground(DependencyObject d)
        {
            return (Brush?)d.GetValue(DropDownBackgroundProperty);
        }

        /// <summary>
        /// Sets the brush the drop-down is filled with.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static void SetDropDownBackground(DependencyObject obj, Brush? value)
        {
            obj.SetValue(DropDownBackgroundProperty, value);
        }

        public static readonly DependencyProperty DropDownBorderBrushProperty
            = DependencyProperty.RegisterAttached("DropDownBorderBrush",
                                                  typeof(Brush),
                                                  typeof(ComboBoxHelper),
                                                  new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the brush of the frame around the drop-down.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static Brush? GetDropDownBorderBrush(DependencyObject d)
        {
            return (Brush?)d.GetValue(DropDownBorderBrushProperty);
        }

        /// <summary>
        /// Sets the brush of the frame around the drop-down.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static void SetDropDownBorderBrush(DependencyObject obj, Brush? value)
        {
            obj.SetValue(DropDownBorderBrushProperty, value);
        }

        public static readonly DependencyProperty DropDownCornerRadiusProperty
            = DependencyProperty.RegisterAttached("DropDownCornerRadius",
                                                  typeof(CornerRadius),
                                                  typeof(ComboBoxHelper),
                                                  new FrameworkPropertyMetadata(new CornerRadius()));

        /// <summary>
        /// Gets the corner radius of the drop-down.
        /// </summary>
        /// <remarks>
        /// A flyout is rounded a little more than the control it hangs under, so this is a knob of
        /// its own rather than the corner radius of the box.
        /// </remarks>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static CornerRadius GetDropDownCornerRadius(DependencyObject d)
        {
            return (CornerRadius)d.GetValue(DropDownCornerRadiusProperty);
        }

        /// <summary>
        /// Sets the corner radius of the drop-down.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static void SetDropDownCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(DropDownCornerRadiusProperty, value);
        }

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

        /// <summary>Identifies the OpenDropDownOnEnterAndSpace attached dependency property.</summary>
        public static readonly DependencyProperty OpenDropDownOnEnterAndSpaceProperty
            = DependencyProperty.RegisterAttached("OpenDropDownOnEnterAndSpace",
                                                  typeof(bool),
                                                  typeof(ComboBoxHelper),
                                                  new PropertyMetadata(BooleanBoxes.FalseBox, OnOpenDropDownOnEnterAndSpaceChanged));

        /// <summary>Helper for getting <see cref="OpenDropDownOnEnterAndSpaceProperty"/> from <paramref name="obj"/>.</summary>
        /// <param name="obj"><see cref="DependencyObject"/> to read <see cref="OpenDropDownOnEnterAndSpaceProperty"/> from.</param>
        /// <remarks>Gets whether Enter and the space bar open the list of a closed box that cannot be typed into.</remarks>
        /// <returns>OpenDropDownOnEnterAndSpace property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static bool GetOpenDropDownOnEnterAndSpace(DependencyObject obj)
        {
            return (bool)obj.GetValue(OpenDropDownOnEnterAndSpaceProperty);
        }

        /// <summary>Helper for setting <see cref="OpenDropDownOnEnterAndSpaceProperty"/> on <paramref name="obj"/>.</summary>
        /// <param name="obj"><see cref="DependencyObject"/> to set <see cref="OpenDropDownOnEnterAndSpaceProperty"/> on.</param>
        /// <param name="value">OpenDropDownOnEnterAndSpace property value.</param>
        /// <remarks>
        /// Sets whether Enter and the space bar open the list of a closed box that cannot be typed
        /// into, which is what the ComboBox of UWP and WinUI does. WPF opens it with F4 and with Alt
        /// and an arrow only. A box that can be typed into takes the space as a character and Enter
        /// as the end of what was typed, so it is left alone, and so is a key held with Alt or Ctrl.
        /// The Windows 10 and the WinUI styles turn this on.
        /// </remarks>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static void SetOpenDropDownOnEnterAndSpace(DependencyObject obj, bool value)
        {
            obj.SetValue(OpenDropDownOnEnterAndSpaceProperty, BooleanBoxes.Box(value));
        }

        private static void OnOpenDropDownOnEnterAndSpaceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ComboBox comboBox)
            {
                return;
            }

            comboBox.PreviewKeyDown -= ComboBox_PreviewKeyDown;

            if (e.NewValue is true)
            {
                comboBox.PreviewKeyDown += ComboBox_PreviewKeyDown;
            }
        }

        private static void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // only the box itself, not a row of its list or the text box of an editable one
            if (sender is not ComboBox comboBox
                || !ReferenceEquals(e.OriginalSource, comboBox)
                || comboBox.IsEditable
                || comboBox.IsDropDownOpen
                || (e.Key != Key.Enter && e.Key != Key.Space)
                || (e.KeyboardDevice.Modifiers & (ModifierKeys.Alt | ModifierKeys.Control)) != ModifierKeys.None)
            {
                return;
            }

            comboBox.SetCurrentValue(ComboBox.IsDropDownOpenProperty, BooleanBoxes.TrueBox);
            e.Handled = true;
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