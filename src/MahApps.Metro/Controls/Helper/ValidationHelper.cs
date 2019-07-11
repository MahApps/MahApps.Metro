using System.ComponentModel;
using System.Windows;

namespace MahApps.Metro.Controls
{
    public static class ValidationHelper
    {
        /// <summary>
        /// Identifies the ShowValidationErrorOnMouseOver attached property.
        /// </summary>
        public static readonly DependencyProperty ShowValidationErrorOnMouseOverProperty
            = DependencyProperty.RegisterAttached("ShowValidationErrorOnMouseOver",
                                                  typeof(bool),
                                                  typeof(ValidationHelper),
                                                  new PropertyMetadata(false));

        /// <summary>
        /// Gets whether the validation error text will be shown when hovering the validation triangle.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetShowValidationErrorOnMouseOver(UIElement element)
        {
            return (bool)element.GetValue(ShowValidationErrorOnMouseOverProperty);
        }

        /// <summary>
        /// Sets whether the validation error text will be shown when hovering the validation triangle.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetShowValidationErrorOnMouseOver(UIElement element, bool value)
        {
            element.SetValue(ShowValidationErrorOnMouseOverProperty, value);
        }
    }
}