using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Specifies the underline position of a TabControl.
    /// </summary>
    public enum UnderlinedType
    {
        None,
        TabItems,
        SelectedTabItem,
        TabPanel
    }

    public static class TabControlHelper
    {
        /// <summary>
        /// Defines whether the underline below the <see cref="TabItem"/> is shown or not.
        /// </summary>
        [Obsolete(@"This property will be deleted in the next release. You should now use the Underlined attached property.")]
        public static readonly DependencyProperty IsUnderlinedProperty =
            DependencyProperty.RegisterAttached("IsUnderlined",
                                                typeof(bool),
                                                typeof(TabControlHelper),
                                                new PropertyMetadata(false,
                                                                     (o, e) =>
                                                                         {
                                                                             var element = o as UIElement;
                                                                             if (element != null && e.OldValue != e.NewValue && e.NewValue is bool)
                                                                             {
                                                                                 TabControlHelper.SetUnderlined(element, (bool)e.NewValue ? UnderlinedType.TabItems : UnderlinedType.None);
                                                                             }
                                                                         }));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        [Obsolete(@"This property will be deleted in the next release. You should now use the Underlined attached property.")]
        public static bool GetIsUnderlined(UIElement element)
        {
            return (bool)element.GetValue(IsUnderlinedProperty);
        }

        [Obsolete(@"This property will be deleted in the next release. You should now use the Underlined attached property.")]
        public static void SetIsUnderlined(UIElement element, bool value)
        {
            element.SetValue(IsUnderlinedProperty, value);
        }

        /// <summary>
        /// Defines whether the underline below the <see cref="TabItem"/> or <see cref="TabPanel"/> is shown or not.
        /// </summary>
        public static readonly DependencyProperty UnderlinedProperty =
            DependencyProperty.RegisterAttached("Underlined",
                                                typeof(UnderlinedType),
                                                typeof(TabControlHelper),
                                                new PropertyMetadata(UnderlinedType.None));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static UnderlinedType GetUnderlined(UIElement element)
        {
            return (UnderlinedType)element.GetValue(UnderlinedProperty);
        }

        public static void SetUnderlined(UIElement element, UnderlinedType value)
        {
            element.SetValue(UnderlinedProperty, value);
        }

        /// <summary>
        /// Defines the underline brush below the <see cref="TabItem"/> or <see cref="TabPanel"/>.
        /// </summary>
        public static readonly DependencyProperty UnderlineBrushProperty =
            DependencyProperty.RegisterAttached("UnderlineBrush",
                                                typeof(Brush),
                                                typeof(TabControlHelper),
                                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static Brush GetUnderlineBrush(UIElement element)
        {
            return (Brush)element.GetValue(UnderlineBrushProperty);
        }

        public static void SetUnderlineBrush(UIElement element, Brush value)
        {
            element.SetValue(UnderlineBrushProperty, value);
        }

        /// <summary>
        /// Defines the underline brush below the <see cref="TabItem"/> or <see cref="TabPanel"/> of an selected item.
        /// </summary>
        public static readonly DependencyProperty UnderlineSelectedBrushProperty =
            DependencyProperty.RegisterAttached("UnderlineSelectedBrush",
                                                typeof(Brush),
                                                typeof(TabControlHelper),
                                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static Brush GetUnderlineSelectedBrush(UIElement element)
        {
            return (Brush)element.GetValue(UnderlineSelectedBrushProperty);
        }

        public static void SetUnderlineSelectedBrush(UIElement element, Brush value)
        {
            element.SetValue(UnderlineSelectedBrushProperty, value);
        }

        /// <summary>
        /// Defines the underline brush below the <see cref="TabItem"/> or <see cref="TabPanel"/> if the mouse is over an item.
        /// </summary>
        public static readonly DependencyProperty UnderlineMouseOverBrushProperty =
            DependencyProperty.RegisterAttached("UnderlineMouseOverBrush",
                                                typeof(Brush),
                                                typeof(TabControlHelper),
                                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static Brush GetUnderlineMouseOverBrush(UIElement element)
        {
            return (Brush)element.GetValue(UnderlineMouseOverBrushProperty);
        }

        public static void SetUnderlineMouseOverBrush(UIElement element, Brush value)
        {
            element.SetValue(UnderlineMouseOverBrushProperty, value);
        }

        /// <summary>
        /// Defines the underline brush below the <see cref="TabItem"/> or <see cref="TabPanel"/> if the mouse is over a selected item.
        /// </summary>
        public static readonly DependencyProperty UnderlineMouseOverSelectedBrushProperty =
            DependencyProperty.RegisterAttached("UnderlineMouseOverSelectedBrush",
                                                typeof(Brush),
                                                typeof(TabControlHelper),
                                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static Brush GetUnderlineMouseOverSelectedBrush(UIElement element)
        {
            return (Brush)element.GetValue(UnderlineMouseOverSelectedBrushProperty);
        }

        public static void SetUnderlineMouseOverSelectedBrush(UIElement element, Brush value)
        {
            element.SetValue(UnderlineMouseOverSelectedBrushProperty, value);
        }

        /// <summary>
        /// This property can be used to set the Transition for animated TabControls
        /// </summary>
        public static readonly DependencyProperty TransitionProperty =
            DependencyProperty.RegisterAttached("Transition",
                                                typeof(TransitionType),
                                                typeof(TabControlHelper),
                                                new FrameworkPropertyMetadata(TransitionType.Default, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        [Category(AppName.MahApps)]
        public static TransitionType GetTransition(DependencyObject obj)
        {
            return (TransitionType)obj.GetValue(TransitionProperty);
        }

        public static void SetTransition(DependencyObject obj, TransitionType value)
        {
            obj.SetValue(TransitionProperty, value);
        }
    }
}
