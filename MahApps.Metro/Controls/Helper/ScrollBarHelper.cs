using System.Windows;

namespace MahApps.Metro.Controls
{
    public static class ScrollBarHelper
    {
        /// <summary>
        /// This property can be used to set vertical scrollbar left side from the tabpanel (look at MetroAnimatedSingleRowTabControl)
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarOnLeftSideProperty =
            DependencyProperty.RegisterAttached("VerticalScrollBarOnLeftSide", typeof(bool), typeof(ScrollBarHelper),
                                                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        public static bool GetVerticalScrollBarOnLeftSide(DependencyObject obj)
        {
            return (bool)obj.GetValue(VerticalScrollBarOnLeftSideProperty);
        }

        public static void SetVerticalScrollBarOnLeftSide(DependencyObject obj, bool value)
        {
            obj.SetValue(VerticalScrollBarOnLeftSideProperty, value);
        }
    }
}
