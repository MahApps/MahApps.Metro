using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public static class ButtonHelper
    {
        public static readonly DependencyProperty PreserveTextCaseProperty =
                DependencyProperty.RegisterAttached("PreserveTextCase", typeof(bool), typeof(ButtonHelper), new FrameworkPropertyMetadata(false, 
                    FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));

        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static bool GetPreserveTextCase(UIElement element)
        {
            return (bool)element.GetValue(PreserveTextCaseProperty);
        }

        public static void SetPreserveTextCase(UIElement element, bool value)
        {
            element.SetValue(PreserveTextCaseProperty, value);
        }
    }
}
