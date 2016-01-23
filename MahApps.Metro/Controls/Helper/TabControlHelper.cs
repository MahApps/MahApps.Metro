using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    using System.ComponentModel;

    public static class TabControlHelper
    {
        /// <summary>
        /// Defines whether the underline below the <see cref="TabControl"/> is shown or not.
        /// </summary>
        public static readonly DependencyProperty IsUnderlinedProperty =
            DependencyProperty.RegisterAttached("IsUnderlined", typeof(bool), typeof(TabControlHelper), new PropertyMetadata(false));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        public static bool GetIsUnderlined(UIElement element)
        {
            return (bool)element.GetValue(IsUnderlinedProperty);
        }

        public static void SetIsUnderlined(UIElement element, bool value)
        {
            element.SetValue(IsUnderlinedProperty, value);
        }

        /// <summary>
        /// This property can be used to set the Transition for animated TabControls
        /// </summary>
        public static readonly DependencyProperty TransitionProperty =
            DependencyProperty.RegisterAttached("Transition", typeof(TransitionType), typeof(TabControlHelper),
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
