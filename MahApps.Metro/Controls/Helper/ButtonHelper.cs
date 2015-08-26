﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MahApps.Metro.Controls
{
    public static class ButtonHelper
    {
        public static readonly DependencyProperty PreserveTextCaseProperty =
            DependencyProperty.RegisterAttached("PreserveTextCase", typeof(bool), typeof(ButtonHelper),
                                                new FrameworkPropertyMetadata(
													false,
													FrameworkPropertyMetadataOptions.Inherits |
													FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Overrides the text case behavior for certain buttons.
        /// When set to <c>true</c>, the text case will be preserved and won't be changed to upper or lower case.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        [AttachedPropertyBrowsableForType(typeof(ToggleButton))]
        public static bool GetPreserveTextCase(UIElement element)
        {
            return (bool)element.GetValue(PreserveTextCaseProperty);
        }

        public static void SetPreserveTextCase(UIElement element, bool value)
        {
            element.SetValue(PreserveTextCaseProperty, value);
        }

		public static readonly DependencyProperty ShowSeparatorProperty =
            DependencyProperty.RegisterAttached("ShowSeparator", typeof(bool), typeof(ButtonHelper),
                                                new FrameworkPropertyMetadata(
													true,
													FrameworkPropertyMetadataOptions.AffectsMeasure |
													FrameworkPropertyMetadataOptions.AffectsRender));
		
        /// <summary>
        /// Overrides separator visibility for WindowCommands' Buttons/ToggleButtons.
        /// When set to <c>false</c>, the separator will become invisible.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        [AttachedPropertyBrowsableForType(typeof(ToggleButton))]
        public static bool GetShowSeparator(UIElement element)
        {
            return (bool)element.GetValue(ShowSeparatorProperty);
        }

        public static void SetShowSeparator(UIElement element, bool value)
        {
            element.SetValue(ShowSeparatorProperty, value);
        }

        /// <summary>
        /// DependencyProperty for <see cref="CornerRadius" /> property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ButtonHelper),
                                                new FrameworkPropertyMetadata(
													new CornerRadius(),
													FrameworkPropertyMetadataOptions.AffectsMeasure |
													FrameworkPropertyMetadataOptions.AffectsRender));
        
        /// <summary> 
        /// The CornerRadius property allows users to control the roundness of the button corners independently by 
        /// setting a radius value for each corner. Radius values that are too large are scaled so that they
        /// smoothly blend from corner to corner. (Can be used e.g. at MetroButton style)
        /// Description taken from original Microsoft description :-D
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(Button))]
        [AttachedPropertyBrowsableForType(typeof(ToggleButton))]
        public static CornerRadius GetCornerRadius(UIElement element)
        {
            return (CornerRadius)element.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(UIElement element, CornerRadius value)
        {
            element.SetValue(CornerRadiusProperty, value);
        }
    }
}
