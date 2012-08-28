using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    //http://blogs.msdn.com/b/delay/archive/2009/08/04/scrolling-so-smooth-like-the-butter-on-a-muffin-how-to-animate-the-horizontal-verticaloffset-properties-of-a-scrollviewer.aspx
    public class ScrollViewerOffsetMediator : FrameworkElement
    {
        public static readonly DependencyProperty ScrollViewerProperty = DependencyProperty.Register("ScrollViewer", typeof(ScrollViewer), typeof(ScrollViewerOffsetMediator), new PropertyMetadata(OnScrollViewerChanged));
        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(ScrollViewerOffsetMediator), new PropertyMetadata(OnHorizontalOffsetChanged));
        public static readonly DependencyProperty ScrollableWidthMultiplierProperty = DependencyProperty.Register("ScrollableWidthMultiplier", typeof(double), typeof(ScrollViewerOffsetMediator), new PropertyMetadata(OnScrollableWidthMultiplierChanged));

        public ScrollViewer ScrollViewer
        {
            get { return (ScrollViewer)GetValue(ScrollViewerProperty); }
            set { SetValue(ScrollViewerProperty, value); }
        }

        public double ScrollableWidthMultiplier
        {
            get { return (double)GetValue(ScrollableWidthMultiplierProperty); }
            set { SetValue(ScrollableWidthMultiplierProperty, value); }
        }

        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        private static void OnScrollViewerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var mediator = (ScrollViewerOffsetMediator)o;
            var scrollViewer = (ScrollViewer)(e.NewValue);
            if (null != scrollViewer)
            {
                scrollViewer.ScrollToHorizontalOffset(mediator.HorizontalOffset);
            }
        }

        public static void OnHorizontalOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var mediator = (ScrollViewerOffsetMediator)o;
            if (null != mediator.ScrollViewer)
            {
                mediator.ScrollViewer.ScrollToHorizontalOffset((double)(e.NewValue));
            }
        }
        
        public static void OnScrollableWidthMultiplierChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var mediator = (ScrollViewerOffsetMediator)o;
            var scrollViewer = mediator.ScrollViewer;
            if (null != scrollViewer)
            {
                scrollViewer.ScrollToHorizontalOffset((double)(e.NewValue) * scrollViewer.ScrollableWidth);
            }
        }
    }
}