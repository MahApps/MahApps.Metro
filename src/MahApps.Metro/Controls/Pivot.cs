// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        private ScrollViewer? scrollViewer;
        private ListView? headers;
        private PivotItem? selectedItem;
        private ScrollViewerOffsetMediator? mediator;
        internal int internalIndex;

        /// <summary>Identifies the <see cref="SelectionChanged"/> routed event.</summary>
        public static readonly RoutedEvent SelectionChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(SelectionChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(Pivot));

        public event RoutedEventHandler SelectionChanged
        {
            add => this.AddHandler(SelectionChangedEvent, value);
            remove => this.RemoveHandler(SelectionChangedEvent, value);
        }

        /// <summary>Identifies the <see cref="Header"/> dependency property.</summary>
        public static readonly DependencyProperty HeaderProperty
            = DependencyProperty.Register(nameof(Header),
                                          typeof(object),
                                          typeof(Pivot),
                                          new PropertyMetadata(default(string)));

        /// <summary>
        /// Gets or sets the Header of the <see cref="Pivot"/>.
        /// </summary>
        public object? Header
        {
            get => (object?)this.GetValue(HeaderProperty);
            set => this.SetValue(HeaderProperty, value);
        }

        /// <summary>Identifies the <see cref="HeaderTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty HeaderTemplateProperty
            = DependencyProperty.Register(nameof(HeaderTemplate),
                                          typeof(DataTemplate),
                                          typeof(Pivot));

        /// <summary>
        /// Gets or sets the HeaderTemplate of the <see cref="Pivot"/>.
        /// </summary>
        public DataTemplate? HeaderTemplate
        {
            get => (DataTemplate?)this.GetValue(HeaderTemplateProperty);
            set => this.SetValue(HeaderTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedIndex"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedIndexProperty
            = DependencyProperty.Register(nameof(SelectedIndex),
                                          typeof(int),
                                          typeof(Pivot),
                                          new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedIndexPropertyChanged));

        private static void OnSelectedIndexPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is int newSelectedIndex)
            {
                var pivot = (Pivot)dependencyObject;
                if (pivot.internalIndex != pivot.SelectedIndex && newSelectedIndex >= 0 && newSelectedIndex < pivot.Items.Count)
                {
                    var pivotItem = (PivotItem)pivot.Items[newSelectedIndex];
                    // set headers selected item too
                    if (pivot.headers is not null)
                    {
                        pivot.headers.SelectedItem = pivotItem;
                    }

                    pivot.GoToItem(pivotItem);
                }
            }
        }

        public int SelectedIndex

        {
            get => (int)this.GetValue(SelectedIndexProperty);
            set => this.SetValue(SelectedIndexProperty, value);
        }

        public void GoToItem(PivotItem? item)
        {
            if (item is null || item == this.selectedItem)
            {
                return;
            }

            var widthToScroll = 0.0;
            int index;
            for (index = 0; index < this.Items.Count; index++)
            {
                if (ReferenceEquals(this.Items[index], item))
                {
                    this.internalIndex = index;
                    break;
                }

                widthToScroll += ((PivotItem)this.Items[index]).ActualWidth;
            }

            if (this.mediator is not null
                && this.scrollViewer is not null)
            {
                this.mediator.HorizontalOffset = this.scrollViewer.HorizontalOffset;
                var sb = this.mediator.Resources["Storyboard1"] as Storyboard;
                var frame = (EasingDoubleKeyFrame)this.mediator.FindName("edkf");
                frame.Value = widthToScroll;

                if (sb is not null)
                {
                    sb.Completed -= this.OnStoryboardCompleted;
                    sb.Completed += this.OnStoryboardCompleted;
                    sb.Begin();
                }
            }

            this.RaiseEvent(new RoutedEventArgs(SelectionChangedEvent));
        }

        private void OnStoryboardCompleted(object? sender, EventArgs e)
        {
            this.SetCurrentValue(SelectedIndexProperty, this.internalIndex);
        }

        static Pivot()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pivot), new FrameworkPropertyMetadata(typeof(Pivot)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.scrollViewer = this.GetTemplateChild("PART_Scroll") as ScrollViewer;
            this.headers = this.GetTemplateChild("PART_Headers") as ListView;
            this.mediator = this.GetTemplateChild("PART_Mediator") as ScrollViewerOffsetMediator;

            if (this.scrollViewer != null)
            {
                this.scrollViewer.ScrollChanged += this.ScrollViewerScrollChanged;
                this.scrollViewer.PreviewMouseWheel += this.ScrollViewerMouseWheel;
            }

            if (this.headers != null)
            {
                this.headers.SelectionChanged += this.headers_SelectionChanged;
            }
        }

        private void ScrollViewerMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            this.scrollViewer!.ScrollToHorizontalOffset(this.scrollViewer.HorizontalOffset + -e.Delta);
        }

        private void headers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.GoToItem((PivotItem)this.headers!.SelectedItem);
        }

        private void ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var position = 0.0;
            for (var i = 0; i < this.Items.Count; i++)
            {
                var pivotItem = ((PivotItem)this.Items[i]);
                var widthOfItem = pivotItem.ActualWidth;
                if (e.HorizontalOffset <= (position + widthOfItem - 1))
                {
                    this.selectedItem = pivotItem;
                    if (this.headers is not null && this.headers.SelectedItem != this.selectedItem)
                    {
                        this.headers.SelectedItem = this.selectedItem;
                        this.internalIndex = i;
                        this.SelectedIndex = i;

                        this.RaiseEvent(new RoutedEventArgs(SelectionChangedEvent));
                    }

                    break;
                }

                position += widthOfItem;
            }
        }
    }
}