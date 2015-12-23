using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace MahApps.Metro.Controls
{
    using System.ComponentModel;
    using System.Windows.Controls;

    public static class ToggleButtonHelper
    {
        /// <summary>
        /// This property can be used to handle the style for CheckBox and RadioButton
        /// LeftToRight means content left and button right and RightToLeft vise versa
        /// </summary>
        public static readonly DependencyProperty ContentDirectionProperty =
            DependencyProperty.RegisterAttached("ContentDirection", typeof(FlowDirection), typeof(ToggleButtonHelper),
                                                new FrameworkPropertyMetadata(FlowDirection.LeftToRight,
            //FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits,
                                                                              ContentDirectionPropertyChanged));

        /// <summary>
        /// This property can be used to handle the style for CheckBox and RadioButton
        /// LeftToRight means content left and button right and RightToLeft vise versa
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(ToggleButton))]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        [Category(AppName.MahApps)]
        public static FlowDirection GetContentDirection(UIElement element)
        {
            return (FlowDirection)element.GetValue(ContentDirectionProperty);
        }

        public static void SetContentDirection(UIElement element, FlowDirection value)
        {
            element.SetValue(ContentDirectionProperty, value);
        }

        private static void ContentDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as ToggleButton;
            if (null == tb)
            {
                throw new InvalidOperationException("The property 'ContentDirection' may only be set on ToggleButton elements.");
            }
        }
    }
}
