using System;
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
        internal int internalIndex;
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Pivot));
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(Pivot), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(Pivot));
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Pivot), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedItemChanged));

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

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        public void GoToItem(PivotItem item)
        {
            if (item == null || item == selectedItem)
            {
                return;
            }

            var widthToScroll = 0.0;
            int index;
            for (index = 0; index < Items.Count; index++)
            {
                if (Items[index] == item)
                {
                    internalIndex = index;
                    break;
                }
                widthToScroll += ((PivotItem)Items[index]).ActualWidth;
            }

            mediator.HorizontalOffset = scroller.HorizontalOffset;
            var sb = mediator.Resources["Storyboard1"] as Storyboard;
            var frame = (EasingDoubleKeyFrame)mediator.FindName("edkf");
            frame.Value = widthToScroll;
            sb.Completed -= sb_Completed;
            sb.Completed += sb_Completed;
            sb.Begin();

            RaiseEvent(new RoutedEventArgs(SelectionChangedEvent));
        }

        private void sb_Completed(object sender, EventArgs e)
        {
            SelectedIndex = internalIndex;
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
            {
                scroller.ScrollChanged += scroller_ScrollChanged;
                scroller.PreviewMouseWheel += scroller_MouseWheel;
            }
            if (headers != null)
            {
                headers.SelectionChanged += headers_SelectionChanged;
            }
        }

        private void scroller_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset + -e.Delta);
        }

        private void headers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GoToItem((PivotItem)headers.SelectedItem);
        }

        private void scroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var position = 0.0;
            for (var i = 0; i < Items.Count; i++)
            {
                var pivotItem = ((PivotItem)Items[i]);
                var widthOfItem = pivotItem.ActualWidth;
                if (e.HorizontalOffset <= (position + widthOfItem - 1))
                {
                    selectedItem = pivotItem;
                    if (headers.SelectedItem != selectedItem)
                    {
                        headers.SelectedItem = selectedItem;
                        internalIndex = i;
                        SelectedIndex = i;

                        RaiseEvent(new RoutedEventArgs(SelectionChangedEvent));
                    }
                    break;
                }
                position += widthOfItem;
            }
        }

        private static void SelectedItemChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var pivot = (Pivot)dependencyObject;
                var newSelectedIndex = (int)e.NewValue;
                if (pivot.internalIndex != pivot.SelectedIndex && newSelectedIndex >= 0 && newSelectedIndex < pivot.Items.Count)
                {
                    var pivotItem = (PivotItem)pivot.Items[newSelectedIndex];
                    // set headers selected item too
                    pivot.headers.SelectedItem = pivotItem;
                    pivot.GoToItem(pivotItem);
                }
            }
        }
    }
}