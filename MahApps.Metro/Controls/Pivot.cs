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
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(Pivot), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(Pivot));
        public static readonly DependencyProperty IsResizingItemsDynamicProperty = DependencyProperty.Register("IsResizingItemsDynamic", typeof(bool), typeof(Pivot), new PropertyMetadata(true));
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Pivot), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedItemChanged));
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Pivot));
        internal int internalIndex;
        private ListView headers;
        private ScrollViewerOffsetMediator mediator;
        private ScrollViewer scroller;
        private PivotItem selectedItem;

        static Pivot()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pivot), new FrameworkPropertyMetadata(typeof(Pivot)));
        }

        public Pivot()
        {
            Loaded += Pivot_Loaded;
        }

        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public bool IsAutoScrolling { get; set; }

        public bool IsManualScrolling { get; set; }

        public bool IsResizingItemsDynamic
        {
            get { return (bool)GetValue(IsResizingItemsDynamicProperty); }
            set { SetValue(IsResizingItemsDynamicProperty, value); }
        }

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public void AutoScrollToItem(PivotItem pivotItem)
        {
            if (mediator == null || scroller == null)
                return;

            IsAutoScrolling = true;

            var widthToScroll = 0.0;
            int index;
            for (index = 0; index < Items.Count; index++)
            {
                if (Items[index] == pivotItem)
                {
                    internalIndex = index;
                    break;
                }
                widthToScroll += ((PivotItem)Items[index]).ActualWidth;
            }

            SelectedIndex = internalIndex;

            headers.SelectedIndex = internalIndex;

            RaiseEvent(new RoutedEventArgs(SelectionChangedEvent));
            var horizontalOffset = scroller.HorizontalOffset;

            mediator.SetCurrentValue(ScrollViewerOffsetMediator.HorizontalOffsetProperty, horizontalOffset);

            var sb = mediator.Resources["Storyboard1"] as Storyboard;
            var frame = (EasingDoubleKeyFrame)mediator.FindName("edkf");

            frame.Value = widthToScroll;
            sb.Completed -= sb_Completed;
            sb.Completed += sb_Completed;
            sb.Begin();
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
                //scroller.PreviewMouseWheel += scroller_MouseWheel;
            }
            if (headers != null)
                headers.SelectionChanged += headers_SelectionChanged;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (sizeInfo.WidthChanged && !IsAutoScrolling)
            {
                ApplyNewWidth(sizeInfo.NewSize.Width);
            }
        }

        private static void SelectedItemChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var p = (Pivot)dependencyObject;
            if (p == null)
                return;

            if (p.IsManualScrolling || p.IsAutoScrolling)
                return;

            // In this case SelectedIndex was set from outside
            if (dependencyPropertyChangedEventArgs.OldValue != dependencyPropertyChangedEventArgs.NewValue)
            {
                if (p.internalIndex != (int)dependencyPropertyChangedEventArgs.NewValue)
                {
                    p.AutoScrollToItem((PivotItem)p.Items[(int)dependencyPropertyChangedEventArgs.NewValue]);
                }
            }
        }

        private void ApplyNewWidth(double newWidth)
        {
            if (IsResizingItemsDynamic)
            {
                var calculatedWidth = newWidth * 0.9;

                var selectedIndexOld = SelectedIndex;

                if (Items.Count > 0)
                {
                    if (Items.Count == 1)
                    {
                        PivotItem pivotItem = (PivotItem)Items[0];
                        pivotItem.Width = ActualWidth;
                    }
                    else if (Items.Count > 1)
                    {
                        bool anyChange = false;
                        foreach (object item in Items)
                        {
                            PivotItem pivotItem = (PivotItem)item;

                            if (calculatedWidth >= pivotItem.MinWidth && calculatedWidth <= pivotItem.MaxWidth)
                            {
                                pivotItem.Width = calculatedWidth;
                                anyChange = true;
                            }
                        }

                        PivotItem lastItem = (PivotItem)Items[Items.Count - 1];

                        if (ActualWidth >= lastItem.MinWidth && ActualWidth <= lastItem.MaxWidth)
                        {
                            lastItem.Width = ActualWidth;
                        }

                        if (anyChange)
                        {
                            if (selectedIndexOld != SelectedIndex)
                            {
                                AutoScrollToItem((PivotItem)Items[selectedIndexOld]);
                            }
                        }
                    }
                }
            }
        }

        void headers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsAutoScrolling && !IsManualScrolling)
            {
                AutoScrollToItem((PivotItem)headers.SelectedItem);
            }
        }

        void Pivot_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyNewWidth(ActualWidth);

            if (Items.Count > 0)
            {
                AutoScrollToItem((PivotItem)Items[SelectedIndex]);
            }
        }

        void sb_Completed(object sender, EventArgs e)
        {
            IsAutoScrolling = false;
        }

        void scroller_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset + -e.Delta);
        }

        void scroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (IsAutoScrolling)
                return;

            IsManualScrolling = true;

            var position = 0.0;
            for (int i = 0; i < Items.Count; i++)
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

            IsManualScrolling = false;
        }
    }
}
