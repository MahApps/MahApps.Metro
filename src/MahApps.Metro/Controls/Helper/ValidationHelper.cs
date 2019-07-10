using System.ComponentModel;
using System.Windows;

namespace MahApps.Metro.Controls
{
    public static class ValidationHelper
    {
        public static readonly DependencyProperty ShowValidationErrorOnMouseOverProperty
            = DependencyProperty.RegisterAttached("ShowValidationErrorOnMouseOver",
                                                  typeof(bool),
                                                  typeof(ValidationHelper),
                                                  new PropertyMetadata(false));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetShowValidationErrorOnMouseOver(UIElement element)
        {
            return (bool)element.GetValue(ShowValidationErrorOnMouseOverProperty);
        }

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetShowValidationErrorOnMouseOver(UIElement element, bool value)
        {
            element.SetValue(ShowValidationErrorOnMouseOverProperty, value);
        }
    }
}