using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A helper class that provides various attached properties for the Expander control.
    /// <see cref="Expander"/>
    /// </summary>
    public static class ExpanderHelper
    {
        public static readonly DependencyProperty HeaderUpStyleProperty = DependencyProperty.RegisterAttached("HeaderUpStyle", typeof(Style), typeof(ExpanderHelper), new FrameworkPropertyMetadata((Style)null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets the toggle button style used for the ExpandDirection Up.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static Style GetHeaderUpStyle(UIElement element)
        {
            return (Style)element.GetValue(HeaderUpStyleProperty);
        }

        /// <summary>
        /// Sets the toggle button style used for the ExpandDirection Up.
        /// </summary>
        public static void SetHeaderUpStyle(UIElement element, Style value)
        {
            element.SetValue(HeaderUpStyleProperty, value);
        }

        public static readonly DependencyProperty HeaderDownStyleProperty = DependencyProperty.RegisterAttached("HeaderDownStyle", typeof(Style), typeof(ExpanderHelper), new FrameworkPropertyMetadata((Style)null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets the toggle button style used for the ExpandDirection Down.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static Style GetHeaderDownStyle(UIElement element)
        {
            return (Style)element.GetValue(HeaderDownStyleProperty);
        }

        /// <summary>
        /// Sets the toggle button style used for the ExpandDirection Down.
        /// </summary>
        public static void SetHeaderDownStyle(UIElement element, Style value)
        {
            element.SetValue(HeaderDownStyleProperty, value);
        }

        public static readonly DependencyProperty HeaderLeftStyleProperty = DependencyProperty.RegisterAttached("HeaderLeftStyle", typeof(Style), typeof(ExpanderHelper), new FrameworkPropertyMetadata((Style)null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets the toggle button style used for the ExpandDirection Left.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static Style GetHeaderLeftStyle(UIElement element)
        {
            return (Style)element.GetValue(HeaderLeftStyleProperty);
        }

        /// <summary>
        /// Sets the toggle button style used for the ExpandDirection Left.
        /// </summary>
        public static void SetHeaderLeftStyle(UIElement element, Style value)
        {
            element.SetValue(HeaderLeftStyleProperty, value);
        }

        public static readonly DependencyProperty HeaderRightStyleProperty = DependencyProperty.RegisterAttached("HeaderRightStyle", typeof(Style), typeof(ExpanderHelper), new FrameworkPropertyMetadata((Style)null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets the toggle button style used for the ExpandDirection Right.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static Style GetHeaderRightStyle(UIElement element)
        {
            return (Style)element.GetValue(HeaderRightStyleProperty);
        }

        /// <summary>
        /// Sets the toggle button style used for the ExpandDirection Right.
        /// </summary>
        public static void SetHeaderRightStyle(UIElement element, Style value)
        {
            element.SetValue(HeaderRightStyleProperty, value);
        }
    }
}