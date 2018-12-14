using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public static class ScrollViewerHelper
    {
        /// <summary>
        /// Identifies the VerticalScrollBarOnLeftSide attached property.
        /// This property can be used to set vertical scrollbar left side from the tabpanel (look at MetroAnimatedSingleRowTabControl)
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarOnLeftSideProperty =
            DependencyProperty.RegisterAttached("VerticalScrollBarOnLeftSide",
                                                typeof(bool),
                                                typeof(ScrollViewerHelper),
                                                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets whether the vertical ScrollBar is on the left side or not.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        public static bool GetVerticalScrollBarOnLeftSide(UIElement element)
        {
            return (bool)element.GetValue(VerticalScrollBarOnLeftSideProperty);
        }

        /// <summary>
        /// Sets whether the vertical ScrollBar should be on the left side or not.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        public static void SetVerticalScrollBarOnLeftSide(UIElement element, bool value)
        {
            element.SetValue(VerticalScrollBarOnLeftSideProperty, value);
        }

        /// <summary>
        /// Identifies the IsHorizontalScrollWheelEnabled attached property.
        /// </summary>
        public static readonly DependencyProperty IsHorizontalScrollWheelEnabledProperty =
            DependencyProperty.RegisterAttached("IsHorizontalScrollWheelEnabled",
                                                typeof(bool),
                                                typeof(ScrollViewerHelper),
                                                new PropertyMetadata(false, OnIsHorizontalScrollWheelEnabledPropertyChangedCallback));

        private static void OnIsHorizontalScrollWheelEnabledPropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = o as ScrollViewer;
            if (scrollViewer != null && e.NewValue != e.OldValue && e.NewValue is bool)
            {
                scrollViewer.PreviewMouseWheel -= ScrollViewerOnPreviewMouseWheel;
                if ((bool)e.NewValue)
                {
                    scrollViewer.PreviewMouseWheel += ScrollViewerOnPreviewMouseWheel;
                }
            }
        }

        private static void ScrollViewerOnPreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null && scrollViewer.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled)
            {
                if (e.Delta > 0)
                {
                    scrollViewer.LineLeft();
                }
                else
                {
                    scrollViewer.LineRight();
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Gets whether the ScrollViewer is scrolling horizontal by using the mouse wheel.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetIsHorizontalScrollWheelEnabled(UIElement element)
        {
            return (bool)element.GetValue(IsHorizontalScrollWheelEnabledProperty);
        }

        /// <summary>
        /// Sets whether the ScrollViewer should be scroll horizontal by using the mouse wheel.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetIsHorizontalScrollWheelEnabled(UIElement element, bool value)
        {
            element.SetValue(IsHorizontalScrollWheelEnabledProperty, value);
        }
    }
}
