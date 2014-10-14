using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A helper class that provides various attached properties for the GroupBox, TabItem and MetroTabItem controls.
    /// </summary>
    public static class ControlsHelper
    {
        public static readonly DependencyProperty GroupBoxHeaderForegroundProperty =
            DependencyProperty.RegisterAttached("GroupBoxHeaderForeground", typeof(Brush), typeof(ControlsHelper), new UIPropertyMetadata(Brushes.White));

        [AttachedPropertyBrowsableForType(typeof(GroupBox))]
        public static Brush GetGroupBoxHeaderForeground(UIElement element)
        {
            return (Brush)element.GetValue(GroupBoxHeaderForegroundProperty);
        }

        public static void SetGroupBoxHeaderForeground(UIElement element, Brush value)
        {
            element.SetValue(GroupBoxHeaderForegroundProperty, value);
        }

        /// <summary>
        /// Defines whether the underline below the <see cref="TabControl"/> is shown or not.
        /// </summary>
        public static readonly DependencyProperty IsUnderlinedProperty =
            DependencyProperty.RegisterAttached("IsUnderlined", typeof(bool), typeof(ControlsHelper), new PropertyMetadata(false));

        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static bool GetIsUnderlined(UIElement element)
        {
            return (bool)element.GetValue(IsUnderlinedProperty);
        }

        public static void SetIsUnderlined(UIElement element, bool value)
        {
            element.SetValue(IsUnderlinedProperty, value);
        }

        public static readonly DependencyProperty HeaderFontSizeProperty =
            DependencyProperty.RegisterAttached("HeaderFontSize", typeof(double), typeof(ControlsHelper), new FrameworkPropertyMetadata(26.67, HeaderFontSizePropertyChangedCallback){ Inherits = true});

        private static void HeaderFontSizePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is double)
            {
                // close button only for MetroTabItem
                var metroTabItem = dependencyObject as MetroTabItem;
                if (metroTabItem == null)
                {
                    return;
                }

                if (metroTabItem.closeButton == null)
                {
                    metroTabItem.ApplyTemplate();
                }

                if (metroTabItem.closeButton != null && metroTabItem.contentSite != null)
                {
                    // punker76: i don't like this! i think this must be done with xaml.
                    var fontDpiSize = (double)e.NewValue;
                    var fontHeight = Math.Ceiling(fontDpiSize * metroTabItem.FontFamily.LineSpacing);
                    var newMargin = (Math.Round(fontHeight) / 2.8)
                                    - metroTabItem.Padding.Top - metroTabItem.Padding.Bottom
                                    - metroTabItem.contentSite.Margin.Top - metroTabItem.contentSite.Margin.Bottom;

                    var previousMargin = metroTabItem.closeButton.Margin;
                    metroTabItem.newButtonMargin = new Thickness(previousMargin.Left, newMargin, previousMargin.Right, previousMargin.Bottom);
                    metroTabItem.closeButton.Margin = metroTabItem.newButtonMargin;

                    metroTabItem.closeButton.UpdateLayout();
                }
            }
        }

        [AttachedPropertyBrowsableForType(typeof(MetroTabItem))]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        [AttachedPropertyBrowsableForType(typeof(GroupBox))]
        public static double GetHeaderFontSize(UIElement element)
        {
            return (double)element.GetValue(HeaderFontSizeProperty);
        }

        public static void SetHeaderFontSize(UIElement element, double value)
        {
            element.SetValue(HeaderFontSizeProperty, value);
        }

        public static readonly DependencyProperty HeaderFontStretchProperty =
            DependencyProperty.RegisterAttached("HeaderFontStretch", typeof(FontStretch), typeof(ControlsHelper), new UIPropertyMetadata(FontStretches.Normal));

        [AttachedPropertyBrowsableForType(typeof(MetroTabItem))]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        [AttachedPropertyBrowsableForType(typeof(GroupBox))]
        public static FontStretch GetHeaderFontStretch(UIElement element)
        {
            return (FontStretch)element.GetValue(HeaderFontStretchProperty);
        }

        public static void SetHeaderFontStretch(UIElement element, FontStretch value)
        {
            element.SetValue(HeaderFontStretchProperty, value);
        }

        public static readonly DependencyProperty HeaderFontWeightProperty =
            DependencyProperty.RegisterAttached("HeaderFontWeight", typeof(FontWeight), typeof(ControlsHelper), new UIPropertyMetadata(FontWeights.Normal));

        [AttachedPropertyBrowsableForType(typeof(MetroTabItem))]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        [AttachedPropertyBrowsableForType(typeof(GroupBox))]
        public static FontWeight GetHeaderFontWeight(UIElement element)
        {
            return (FontWeight)element.GetValue(HeaderFontWeightProperty);
        }

        public static void SetHeaderFontWeight(UIElement element, FontWeight value)
        {
            element.SetValue(HeaderFontWeightProperty, value);
        }

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
            return (double)obj.GetValue(ButtonWidthProperty);
        }

        public static void SetButtonWidth(DependencyObject obj, double value)
        {
            obj.SetValue(ButtonWidthProperty, value);
        }

        /// <summary>
        /// This property can be used to set vertical scrollbar left side from the tabpanel (look at MetroAnimatedSingleRowTabControl)
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarOnLeftSideProperty =
            DependencyProperty.RegisterAttached("VerticalScrollBarOnLeftSide", typeof(bool), typeof(ControlsHelper),
                                                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        public static bool GetVerticalScrollBarOnLeftSide(DependencyObject obj)
        {
            return (bool)obj.GetValue(VerticalScrollBarOnLeftSideProperty);
        }

        public static void SetVerticalScrollBarOnLeftSide(DependencyObject obj, bool value)
        {
            obj.SetValue(VerticalScrollBarOnLeftSideProperty, value);
        }

        /// <summary>
        /// This property can be used to set the Transition for animated TabControls
        /// </summary>
        public static readonly DependencyProperty TransitionProperty =
            DependencyProperty.RegisterAttached("Transition", typeof(TransitionType), typeof(ControlsHelper),
                                                new FrameworkPropertyMetadata(TransitionType.Default, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        public static TransitionType GetTransition(DependencyObject obj)
        {
            return (TransitionType)obj.GetValue(TransitionProperty);
        }

        public static void SetTransition(DependencyObject obj, TransitionType value)
        {
            obj.SetValue(TransitionProperty, value);
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
            return (FlowDirection)element.GetValue(ContentDirectionProperty);
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
