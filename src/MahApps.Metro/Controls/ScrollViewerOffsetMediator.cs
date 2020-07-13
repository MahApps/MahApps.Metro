// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class ScrollViewerOffsetMediator : FrameworkElement
    {
        public static readonly DependencyProperty ScrollViewerProperty = DependencyProperty.Register(nameof(ScrollViewer), typeof(ScrollViewer), typeof(ScrollViewerOffsetMediator), new PropertyMetadata(default(ScrollViewer), OnScrollViewerChanged));
        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register(nameof(HorizontalOffset), typeof(double), typeof(ScrollViewerOffsetMediator), new PropertyMetadata(default(double), OnHorizontalOffsetChanged));

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