using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A helper class that provides various attached properties for multiple control types.
    /// </summary>
    public static class ControlsHelper
    {
        /// <summary>
        /// This property can be used to set the button width (PART_ClearText) of TextBox, PasswordBox, ComboBox
        /// For multiline TextBox, PasswordBox is this the fallback for the clear text button! so it must set manually!
        /// For normal TextBox, PasswordBox the width is the height. 
        /// </summary>
        public static readonly DependencyProperty ButtonWidthProperty =
            DependencyProperty.RegisterAttached("ButtonWidth", typeof(double), typeof(ControlsHelper),
                                                new FrameworkPropertyMetadata(22d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        public static double GetButtonWidth(DependencyObject obj)
        {
            return (double) obj.GetValue(ButtonWidthProperty);
        }

        public static void SetButtonWidth(DependencyObject obj, double value)
        {
            obj.SetValue(ButtonWidthProperty, value);
        }

        /// <summary>
        /// This property can be used to handle the style for CheckBox and RadioButton
        /// LeftToRight means content left and button right and RightToLeft vise versa
        /// </summary>
        public static readonly DependencyProperty ContentDirectionProperty =
            DependencyProperty.RegisterAttached("ContentDirection", typeof(FlowDirection), typeof(ControlsHelper),
                                                new FrameworkPropertyMetadata(FlowDirection.LeftToRight,
            //FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits,
                                                                              ContentDirectionPropertyChanged));

        /// <summary>
        /// This property can be used to handle the style for CheckBox and RadioButton
        /// LeftToRight means content left and button right and RightToLeft vise versa
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(ToggleButton))]
        public static FlowDirection GetContentDirection(UIElement element)
        {
            return (FlowDirection) element.GetValue(ContentDirectionProperty);
        }

        public static void SetContentDirection(UIElement element, FlowDirection value)
        {
            element.SetValue(ContentDirectionProperty, value);
        }

        private static void ContentDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as ToggleButton;
            if (null == tb) {
                throw new InvalidOperationException("The property 'ContentDirection' may only be set on ToggleButton elements.");
            }
        }
    }
}
