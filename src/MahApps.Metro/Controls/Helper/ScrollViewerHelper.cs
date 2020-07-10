// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.ValueBoxes;

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
                                                new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>Helper for getting <see cref="VerticalScrollBarOnLeftSideProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="VerticalScrollBarOnLeftSideProperty"/> from.</param>
        /// <returns>VerticalScrollBarOnLeftSide property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        public static bool GetVerticalScrollBarOnLeftSide(UIElement element)
        {
            return (bool)element.GetValue(VerticalScrollBarOnLeftSideProperty);
        }

        /// <summary>Helper for setting <see cref="VerticalScrollBarOnLeftSideProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="VerticalScrollBarOnLeftSideProperty"/> on.</param>
        /// <param name="value">VerticalScrollBarOnLeftSide property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        public static void SetVerticalScrollBarOnLeftSide(UIElement element, bool value)
        {
            element.SetValue(VerticalScrollBarOnLeftSideProperty, BooleanBoxes.Box(value));
        }

        /// <summary>
        /// Identifies the IsHorizontalScrollWheelEnabled attached property.
        /// </summary>
        public static readonly DependencyProperty IsHorizontalScrollWheelEnabledProperty =
            DependencyProperty.RegisterAttached("IsHorizontalScrollWheelEnabled",
                                                typeof(bool),
                                                typeof(ScrollViewerHelper),
                                                new PropertyMetadata(BooleanBoxes.FalseBox, OnIsHorizontalScrollWheelEnabledPropertyChangedCallback));

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

        /// <summary>Helper for getting <see cref="IsHorizontalScrollWheelEnabledProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="IsHorizontalScrollWheelEnabledProperty"/> from.</param>
        /// <returns>IsHorizontalScrollWheelEnabled property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetIsHorizontalScrollWheelEnabled(UIElement element)
        {
            return (bool)element.GetValue(IsHorizontalScrollWheelEnabledProperty);
        }

        /// <summary>Helper for setting <see cref="IsHorizontalScrollWheelEnabledProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="IsHorizontalScrollWheelEnabledProperty"/> on.</param>
        /// <param name="value">IsHorizontalScrollWheelEnabled property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetIsHorizontalScrollWheelEnabled(UIElement element, bool value)
        {
            element.SetValue(IsHorizontalScrollWheelEnabledProperty, BooleanBoxes.Box(value));
        }

        /// <summary>
        /// This property can be used to trigger the call to a command when the user reach the end of the vertical scrollable area.
        /// </summary>
        public static readonly DependencyProperty EndOfVerticalScrollReachedCommandProperty
            = DependencyProperty.RegisterAttached("EndOfVerticalScrollReachedCommand",
                                                  typeof(ICommand),
                                                  typeof(ScrollViewerHelper),
                                                  new FrameworkPropertyMetadata(null, EndOfVerticalScrollReachedCommandPropertyChanged));

        /// <summary>Helper for getting <see cref="EndOfVerticalScrollReachedCommandProperty"/> from <paramref name="obj"/>.</summary>
        /// <param name="obj"><see cref="DependencyObject"/> to read <see cref="EndOfVerticalScrollReachedCommandProperty"/> from.</param>
        /// <returns>EndOfVerticalScrollReachedCommand property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
        public static ICommand GetEndOfVerticalScrollReachedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(EndOfVerticalScrollReachedCommandProperty);
        }

        /// <summary>Helper for setting <see cref="EndOfVerticalScrollReachedCommandProperty"/> on <paramref name="obj"/>.</summary>
        /// <param name="obj"><see cref="DependencyObject"/> to set <see cref="EndOfVerticalScrollReachedCommandProperty"/> on.</param>
        /// <param name="value">EndOfVerticalScrollReachedCommand property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
        public static void SetEndOfVerticalScrollReachedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(EndOfVerticalScrollReachedCommandProperty, value);
        }

        private static void EndOfVerticalScrollReachedCommandPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (obj is ItemsControl itemsControl)
                {
                    if (itemsControl.IsLoaded)
                    {
                        itemsControl.ApplyTemplate();
                        var scrollViewer = itemsControl.FindChild<ScrollViewer>();
                        if (scrollViewer != null)
                        {
                            SetEndOfVerticalScrollReachedCommand(scrollViewer, e.NewValue as ICommand);
                        }
                    }
                    else
                    {
                        itemsControl.Loaded -= OnEndOfVerticalScrollReachedCommandItemsControlLoaded;
                        if (e.NewValue != null)
                        {
                            itemsControl.Loaded += OnEndOfVerticalScrollReachedCommandItemsControlLoaded;
                        }
                    }
                }
                else if (obj is ScrollViewer scrollViewer)
                {
                    scrollViewer.Loaded -= OnScrollViewerLoaded;
                    if (e.NewValue != null)
                    {
                        scrollViewer.Loaded += OnScrollViewerLoaded;
                    }
                }
            }
        }

        private static void OnEndOfVerticalScrollReachedCommandItemsControlLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is ItemsControl itemsControl)
            {
                itemsControl.ApplyTemplate();
                var scrollViewer = itemsControl.FindChild<ScrollViewer>();
                if (scrollViewer != null)
                {
                    itemsControl.Loaded -= OnEndOfVerticalScrollReachedCommandItemsControlLoaded;
                    SetEndOfVerticalScrollReachedCommand(scrollViewer, GetEndOfVerticalScrollReachedCommand(itemsControl));
                }
            }
        }

        /// <summary>
        /// This property can be used to trigger the call to a command when the user reach the end of the horizontal scrollable area.
        /// </summary>
        public static readonly DependencyProperty EndOfHorizontalScrollReachedCommandProperty
            = DependencyProperty.RegisterAttached("EndOfHorizontalScrollReachedCommand",
                                                  typeof(ICommand),
                                                  typeof(ScrollViewerHelper),
                                                  new FrameworkPropertyMetadata(null, EndOfHorizontalScrollReachedCommandPropertyChanged));

        /// <summary>Helper for getting <see cref="EndOfHorizontalScrollReachedCommandProperty"/> from <paramref name="obj"/>.</summary>
        /// <param name="obj"><see cref="DependencyObject"/> to read <see cref="EndOfHorizontalScrollReachedCommandProperty"/> from.</param>
        /// <returns>EndOfHorizontalScrollReachedCommand property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
        public static ICommand GetEndOfHorizontalScrollReachedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(EndOfHorizontalScrollReachedCommandProperty);
        }

        /// <summary>Helper for setting <see cref="EndOfHorizontalScrollReachedCommandProperty"/> on <paramref name="obj"/>.</summary>
        /// <param name="obj"><see cref="DependencyObject"/> to set <see cref="EndOfHorizontalScrollReachedCommandProperty"/> on.</param>
        /// <param name="value">EndOfHorizontalScrollReachedCommand property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
        public static void SetEndOfHorizontalScrollReachedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(EndOfHorizontalScrollReachedCommandProperty, value);
        }

        private static void EndOfHorizontalScrollReachedCommandPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (obj is ItemsControl itemsControl)
                {
                    if (itemsControl.IsLoaded)
                    {
                        itemsControl.ApplyTemplate();
                        var scrollViewer = itemsControl.FindChild<ScrollViewer>();
                        if (scrollViewer != null)
                        {
                            SetEndOfHorizontalScrollReachedCommand(scrollViewer, e.NewValue as ICommand);
                        }
                    }
                    else
                    {
                        itemsControl.Loaded -= OnEndOfHorizontalScrollReachedCommandItemsControlLoaded;
                        if (e.NewValue != null)
                        {
                            itemsControl.Loaded += OnEndOfHorizontalScrollReachedCommandItemsControlLoaded;
                        }
                    }
                }
                else if (obj is ScrollViewer scrollViewer)
                {
                    scrollViewer.Loaded -= OnScrollViewerLoaded;
                    if (e.NewValue != null)
                    {
                        scrollViewer.Loaded += OnScrollViewerLoaded;
                    }
                }
            }
        }

        private static void OnEndOfHorizontalScrollReachedCommandItemsControlLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is ItemsControl itemsControl)
            {
                itemsControl.ApplyTemplate();
                var scrollViewer = itemsControl.FindChild<ScrollViewer>();
                if (scrollViewer != null)
                {
                    itemsControl.Loaded -= OnEndOfHorizontalScrollReachedCommandItemsControlLoaded;
                    SetEndOfHorizontalScrollReachedCommand(scrollViewer, GetEndOfHorizontalScrollReachedCommand(itemsControl));
                }
            }
        }

        /// <summary>
        /// This property can be used to provide a command parameter to the command called when reaching the end of the vertical or horizontal scrollable area.
        /// </summary>
        public static readonly DependencyProperty EndOfScrollReachedCommandParameterProperty
            = DependencyProperty.RegisterAttached("EndOfScrollReachedCommandParameter",
                                                  typeof(object),
                                                  typeof(ScrollViewerHelper),
                                                  new FrameworkPropertyMetadata(null, EndOfScrollReachedCommandParameterPropertyChanged));

        /// <summary>Helper for getting <see cref="EndOfScrollReachedCommandParameterProperty"/> from <paramref name="obj"/>.</summary>
        /// <param name="obj"><see cref="DependencyObject"/> to read <see cref="EndOfScrollReachedCommandParameterProperty"/> from.</param>
        /// <returns>EndOfScrollReachedCommandParameter property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
        public static object GetEndOfScrollReachedCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(EndOfScrollReachedCommandParameterProperty);
        }

        /// <summary>Helper for setting <see cref="EndOfScrollReachedCommandParameterProperty"/> on <paramref name="obj"/>.</summary>
        /// <param name="obj"><see cref="DependencyObject"/> to set <see cref="EndOfScrollReachedCommandParameterProperty"/> on.</param>
        /// <param name="value">EndOfScrollReachedCommandParameter property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
        public static void SetEndOfScrollReachedCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(EndOfScrollReachedCommandParameterProperty, value);
        }

        private static void EndOfScrollReachedCommandParameterPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (obj is ItemsControl itemsControl)
                {
                    if (itemsControl.IsLoaded)
                    {
                        itemsControl.ApplyTemplate();
                        var scrollViewer = itemsControl.FindChild<ScrollViewer>();
                        if (scrollViewer != null)
                        {
                            SetEndOfScrollReachedCommandParameter(scrollViewer, e.NewValue);
                        }
                    }
                    else
                    {
                        itemsControl.Loaded -= OnEndOfScrollReachedCommandParameterItemsControlLoaded;
                        if (e.NewValue != null)
                        {
                            itemsControl.Loaded += OnEndOfScrollReachedCommandParameterItemsControlLoaded;
                        }
                    }
                }
            }
        }

        private static void OnEndOfScrollReachedCommandParameterItemsControlLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is ItemsControl itemsControl)
            {
                itemsControl.ApplyTemplate();
                var scrollViewer = itemsControl.FindChild<ScrollViewer>();
                if (scrollViewer != null)
                {
                    itemsControl.Loaded -= OnEndOfScrollReachedCommandParameterItemsControlLoaded;
                    SetEndOfScrollReachedCommandParameter(scrollViewer, GetEndOfScrollReachedCommandParameter(itemsControl));
                }
            }
        }

        private static void OnScrollViewerLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                scrollViewer.Unloaded -= OnScrollViewerUnloaded;
                scrollViewer.Unloaded += OnScrollViewerUnloaded;
                scrollViewer.ScrollChanged -= OnScrollViewerScrollChanged;
                scrollViewer.ScrollChanged += OnScrollViewerScrollChanged;
            }
        }

        private static void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                var endOfVerticalScrollReachedCommand = GetEndOfVerticalScrollReachedCommand(scrollViewer);
                var endOfHorizontalScrollReachedCommand = GetEndOfHorizontalScrollReachedCommand(scrollViewer);
                var commandParameter = GetEndOfScrollReachedCommandParameter(scrollViewer) ?? scrollViewer;

                if (endOfVerticalScrollReachedCommand != null && scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight)
                {
                    if (endOfVerticalScrollReachedCommand.CanExecute(commandParameter))
                    {
                        endOfVerticalScrollReachedCommand.Execute(commandParameter);
                    }
                }

                if (endOfHorizontalScrollReachedCommand != null && scrollViewer.HorizontalOffset >= scrollViewer.ScrollableWidth)
                {
                    if (endOfHorizontalScrollReachedCommand.CanExecute(commandParameter))
                    {
                        endOfHorizontalScrollReachedCommand.Execute(commandParameter);
                    }
                }
            }
        }

        private static void OnScrollViewerUnloaded(object sender, RoutedEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                scrollViewer.Unloaded -= OnScrollViewerUnloaded;
                scrollViewer.ScrollChanged -= OnScrollViewerScrollChanged;
            }
        }
    }
}