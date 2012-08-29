using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_Scroll", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_Headers", Type = typeof(ListView))]
    public class Pivot : ItemsControl
    {
        private ScrollViewer scroller;
        private ListView headers;
        private PivotItem selectedItem;

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

            scroller.ScrollToHorizontalOffset(widthToScroll);

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
            headers = (ListView) GetTemplateChild("PART_Headers");

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
                var pivotItem = ((PivotItem) Items[i]);
                var widthOfItem = pivotItem.ActualWidth;
                if (e.HorizontalOffset <= (position + widthOfItem -1))
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
}
