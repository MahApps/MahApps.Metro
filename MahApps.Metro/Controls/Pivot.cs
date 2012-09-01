using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_Scroll", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_Headers", Type = typeof(ListView))]
    [TemplatePart(Name = "PART_Mediator", Type = typeof(ScrollViewerOffsetMediator))]
    public class Pivot : ItemsControl
    {
        private ScrollViewer scroller;
        private ListView headers;
        private PivotItem selectedItem;
        private ScrollViewerOffsetMediator mediator;
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Pivot));
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(Pivot), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(Pivot));

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        public void GoToItem(PivotItem item)
        {
            if (item == null || item == selectedItem)
                return;

            var widthToScroll = 0.0;
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] == item)
                    break;

                widthToScroll += ((PivotItem)Items[i]).ActualWidth;
            }

            mediator.HorizontalOffset = scroller.HorizontalOffset;
            var sb = mediator.Resources["Storyboard1"] as Storyboard;
            var frame = (EasingDoubleKeyFrame)mediator.FindName("edkf");
            frame.Value = widthToScroll;
            sb.Begin();
            RaiseEvent(new RoutedEventArgs(SelectionChangedEvent));
        }

        static Pivot()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pivot), new FrameworkPropertyMetadata(typeof(Pivot)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            scroller = (ScrollViewer)GetTemplateChild("PART_Scroll");
            headers = (ListView)GetTemplateChild("PART_Headers");
            mediator = GetTemplateChild("PART_Mediator") as ScrollViewerOffsetMediator;

            if (scroller != null)
                scroller.ScrollChanged += scroller_ScrollChanged;

            if (headers != null)
                headers.SelectionChanged += headers_SelectionChanged;
        }

        void headers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GoToItem((PivotItem)headers.SelectedItem);
        }

        void scroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var position = 0.0;
            for (int i = 0; i < Items.Count; i++)
            {
                var pivotItem = ((PivotItem)Items[i]);
                var widthOfItem = pivotItem.ActualWidth;
                if (e.HorizontalOffset <= (position + widthOfItem - 1))
                {
                    selectedItem = pivotItem;
                    if (headers.SelectedItem != selectedItem)
                        headers.SelectedItem = selectedItem;
                    break;
                }
                position += widthOfItem;
            }
        }
    }

    public class ScrollViewerOffsetMediator : FrameworkElement
    {
        public static readonly DependencyProperty ScrollViewerProperty = DependencyProperty.Register("ScrollViewer", typeof(ScrollViewer), typeof(ScrollViewerOffsetMediator), new PropertyMetadata(default(ScrollViewer), OnScrollViewerChanged));
        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(ScrollViewerOffsetMediator), new PropertyMetadata(default(double), OnHorizontalOffsetChanged));

        public ScrollViewer ScrollViewer
        {
            get { return (ScrollViewer)GetValue(ScrollViewerProperty); }
            set { SetValue(ScrollViewerProperty, value); }
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
            if (scrollViewer != null)
                scrollViewer.ScrollToHorizontalOffset(mediator.HorizontalOffset);
        }

        private static void OnHorizontalOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var mediator = (ScrollViewerOffsetMediator)o;
            if (mediator.ScrollViewer != null)
                mediator.ScrollViewer.ScrollToHorizontalOffset((double)(e.NewValue));
        }
    }
}
