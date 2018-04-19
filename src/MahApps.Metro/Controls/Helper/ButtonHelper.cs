using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MahApps.Metro.Controls
{
    using System.ComponentModel;

    public static class ButtonHelper
    {
        [Obsolete(@"This property will be deleted in the next release. You should use ContentCharacterCasing attached property located in ControlsHelper.")]
        public static readonly DependencyProperty PreserveTextCaseProperty =
            DependencyProperty.RegisterAttached("PreserveTextCase", typeof(bool), typeof(ButtonHelper),
                                                new FrameworkPropertyMetadata(
                                                    false,
                                                    FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                    (o, e) =>
                                                        {
                                                            var button = o as ButtonBase;
                                                            if (button != null && e.NewValue is bool)
                                                            {
                                                                ControlsHelper.SetContentCharacterCasing(button, (bool)e.NewValue ? CharacterCasing.Normal : CharacterCasing.Upper);
                                                            }
                                                        }));

        /// <summary>
        /// Overrides the text case behavior for certain buttons.
        /// When set to <c>true</c>, the text case will be preserved and won't be changed to upper or lower case.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        [Obsolete(@"This property will be deleted in the next release. You should use ContentCharacterCasing attached property located in ControlsHelper.")]
        public static bool GetPreserveTextCase(UIElement element)
        {
            return (bool)element.GetValue(PreserveTextCaseProperty);
        }

        [Obsolete(@"This property will be deleted in the next release. You should use ContentCharacterCasing attached property located in ControlsHelper.")]
        public static void SetPreserveTextCase(UIElement element, bool value)
        {
            element.SetValue(PreserveTextCaseProperty, value);
        }

        /// <summary>
        /// DependencyProperty for <see cref="CornerRadius" /> property.
        /// </summary>
        [Obsolete(@"This property will be deleted in the next release. You should use CornerRadius attached property located in ControlsHelper.")]
        public static readonly DependencyProperty CornerRadiusProperty
            = DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ButtonHelper),
                                                  new FrameworkPropertyMetadata(
                                                      new CornerRadius(-1), // this is not valid, but this property is obsolete, set this to -1 to get the fired property changed callback
                                                      FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                                                      (o, e) =>
                                                          {
                                                              var element = o as UIElement;
                                                              if (element != null && e.OldValue != e.NewValue && e.NewValue is CornerRadius)
                                                              {
                                                                  ControlsHelper.SetCornerRadius(element, (CornerRadius)e.NewValue);
                                                              }
                                                          }));

        /// <summary> 
        /// The CornerRadius property allows users to control the roundness of the button corners independently by 
        /// setting a radius value for each corner. Radius values that are too large are scaled so that they
        /// smoothly blend from corner to corner. (Can be used e.g. at MetroButton style)
        /// Description taken from original Microsoft description :-D
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        [AttachedPropertyBrowsableForType(typeof(ToggleButton))]
        [Obsolete(@"This property will be deleted in the next release. You should use CornerRadius attached property located in ControlsHelper.")]
        public static CornerRadius GetCornerRadius(UIElement element)
        {
            return (CornerRadius)element.GetValue(CornerRadiusProperty);
        }

        [Obsolete(@"This property will be deleted in the next release. You should use CornerRadius attached property located in ControlsHelper.")]
        public static void SetCornerRadius(UIElement element, CornerRadius value)
        {
            element.SetValue(CornerRadiusProperty, value);
        }
    }
}
