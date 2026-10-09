// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A helper class that provides attached properties for the <see cref="ToolTip"/>.
    /// </summary>
    public static class ToolTipHelper
    {
        // the defaults of ToolTip_Partial.h in microsoft-ui-xaml, DEFAULT_MOUSE_OFFSET and DEFAULT_KEYBOARD_OFFSET
        private const double PointerGap = 20;
        private const double KeyboardGap = 12;

        /// <summary>Identifies the <see cref="P:PlaceLikeWindows"/> attached property.</summary>
        public static readonly DependencyProperty PlaceLikeWindowsProperty
            = DependencyProperty.RegisterAttached("PlaceLikeWindows",
                                                  typeof(bool),
                                                  typeof(ToolTipHelper),
                                                  new PropertyMetadata(BooleanBoxes.FalseBox, OnPlaceLikeWindowsChanged));

        /// <summary>Helper for getting <see cref="PlaceLikeWindowsProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="PlaceLikeWindowsProperty"/> from.</param>
        /// <remarks>Gets whether the tool tip stands above what it explains, the way Windows puts it, rather than below the pointer.</remarks>
        /// <returns>PlaceLikeWindows property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(ToolTip))]
        public static bool GetPlaceLikeWindows(DependencyObject element)
        {
            return (bool)element.GetValue(PlaceLikeWindowsProperty);
        }

        /// <summary>Helper for setting <see cref="PlaceLikeWindowsProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="PlaceLikeWindowsProperty"/> on.</param>
        /// <param name="value">PlaceLikeWindows property value.</param>
        /// <remarks>
        /// Sets whether the tool tip stands above what it explains, the way Windows puts it: centred on the
        /// pointer with 20 between the two, or on the control with 12 when the keyboard opened it, and below
        /// where there is no room above. A placement set through the <see cref="ToolTipService"/> on the
        /// control still wins.
        /// </remarks>
        [AttachedPropertyBrowsableForType(typeof(ToolTip))]
        public static void SetPlaceLikeWindows(DependencyObject element, bool value)
        {
            element.SetValue(PlaceLikeWindowsProperty, BooleanBoxes.Box(value));
        }

        private static void OnPlaceLikeWindowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ToolTip toolTip)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                toolTip.SetCurrentValue(ToolTip.PlacementProperty, PlacementMode.Custom);
                toolTip.SetCurrentValue(ToolTip.CustomPopupPlacementCallbackProperty, new CustomPopupPlacementCallback(new Placer(toolTip).Place));
            }
            else
            {
                toolTip.InvalidateProperty(ToolTip.PlacementProperty);
                toolTip.InvalidateProperty(ToolTip.CustomPopupPlacementCallbackProperty);
            }
        }

        /// <summary>Identifies the <see cref="P:CloseOnScroll"/> attached property.</summary>
        public static readonly DependencyProperty CloseOnScrollProperty
            = DependencyProperty.RegisterAttached("CloseOnScroll",
                                                  typeof(bool),
                                                  typeof(ToolTipHelper),
                                                  new PropertyMetadata(BooleanBoxes.FalseBox, OnCloseOnScrollChanged));

        /// <summary>Helper for getting <see cref="CloseOnScrollProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="CloseOnScrollProperty"/> from.</param>
        /// <remarks>Gets whether the tool tip closes when what it explains is scrolled away under it.</remarks>
        /// <returns>CloseOnScroll property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(ToolTip))]
        public static bool GetCloseOnScroll(DependencyObject element)
        {
            return (bool)element.GetValue(CloseOnScrollProperty);
        }

        /// <summary>Helper for setting <see cref="CloseOnScrollProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="CloseOnScrollProperty"/> on.</param>
        /// <param name="value">CloseOnScroll property value.</param>
        /// <remarks>
        /// Sets whether the tool tip closes when a scroll viewer the control sits in scrolls. The wheel moves
        /// the control away from under a pointer that stands still, and WPF leaves the tool tip where it was
        /// then, while Windows closes it.
        /// </remarks>
        [AttachedPropertyBrowsableForType(typeof(ToolTip))]
        public static void SetCloseOnScroll(DependencyObject element, bool value)
        {
            element.SetValue(CloseOnScrollProperty, BooleanBoxes.Box(value));
        }

        private static readonly DependencyProperty ScrollWatchProperty
            = DependencyProperty.RegisterAttached("ScrollWatch",
                                                  typeof(ScrollWatch),
                                                  typeof(ToolTipHelper),
                                                  new PropertyMetadata(null));

        private static void OnCloseOnScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ToolTip toolTip)
            {
                return;
            }

            toolTip.Opened -= OnToolTipOpened;
            toolTip.Closed -= OnToolTipClosed;
            StopWatching(toolTip);

            if ((bool)e.NewValue)
            {
                toolTip.Opened += OnToolTipOpened;
                toolTip.Closed += OnToolTipClosed;
                if (toolTip.IsOpen)
                {
                    StartWatching(toolTip);
                }
            }
        }

        private static void OnToolTipOpened(object sender, RoutedEventArgs e)
        {
            StartWatching((ToolTip)sender);
        }

        private static void OnToolTipClosed(object sender, RoutedEventArgs e)
        {
            StopWatching((ToolTip)sender);
        }

        private static void StartWatching(ToolTip toolTip)
        {
            StopWatching(toolTip);

            if (toolTip.PlacementTarget is { } target && PresentationSource.FromVisual(target)?.RootVisual is UIElement root)
            {
                toolTip.SetValue(ScrollWatchProperty, new ScrollWatch(toolTip, target, root));
            }
        }

        private static void StopWatching(ToolTip toolTip)
        {
            if (toolTip.GetValue(ScrollWatchProperty) is ScrollWatch watch)
            {
                watch.Stop();
                toolTip.ClearValue(ScrollWatchProperty);
            }
        }

        /// <summary>
        /// Listens on the root of the window for a scroll viewer the control sits in to scroll. The event
        /// bubbles up from the scroll viewer, so the control itself never hears it.
        /// </summary>
        private sealed class ScrollWatch
        {
            private readonly ToolTip toolTip;
            private readonly UIElement target;
            private readonly UIElement root;
            private readonly ScrollChangedEventHandler handler;

            public ScrollWatch(ToolTip toolTip, UIElement target, UIElement root)
            {
                this.toolTip = toolTip;
                this.target = target;
                this.root = root;
                this.handler = this.OnScrollChanged;
                root.AddHandler(ScrollViewer.ScrollChangedEvent, this.handler, true);
            }

            public void Stop()
            {
                this.root.RemoveHandler(ScrollViewer.ScrollChangedEvent, this.handler);
            }

            private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
            {
                // a change of the extent or the viewport moves nothing under the pointer, only the offsets do
                if ((e.VerticalChange != 0 || e.HorizontalChange != 0)
                    && e.OriginalSource is ScrollViewer scrollViewer
                    && this.target.IsDescendantOf(scrollViewer))
                {
                    this.toolTip.SetCurrentValue(ToolTip.IsOpenProperty, BooleanBoxes.FalseBox);
                }
            }
        }

        private sealed class Placer
        {
            private readonly ToolTip toolTip;

            public Placer(ToolTip toolTip)
            {
                this.toolTip = toolTip;
            }

            // WPF hands over the sizes in device pixels and wants the points back in them, while the offsets
            // stay as they were set, so everything that is not one of the two sizes is scaled here
            public CustomPopupPlacement[] Place(Size popupSize, Size targetSize, Point offset)
            {
                var target = this.toolTip.PlacementTarget;
                var scale = target is null ? new DpiScale(1, 1) : VisualTreeHelper.GetDpi(target);
                var sx = scale.DpiScaleX;
                var sy = scale.DpiScaleY;

                // the room a shadow takes around the box is no part of what is placed
                var shadow = VisualTreeHelper.GetChildrenCount(this.toolTip) > 0 && VisualTreeHelper.GetChild(this.toolTip, 0) is FrameworkElement root
                    ? root.Margin
                    : default;

                Rect anchor;
                double gap;
                if (target is not null && target.IsMouseOver)
                {
                    var pointer = Mouse.GetPosition(target);
                    anchor = new Rect(pointer.X * sx, pointer.Y * sy, 0, 0);
                    gap = PointerGap;
                }
                else
                {
                    anchor = new Rect(targetSize);
                    gap = KeyboardGap;
                }

                var boxWidth = popupSize.Width - (shadow.Left + shadow.Right) * sx;
                var x = anchor.X + (anchor.Width - boxWidth) / 2 - shadow.Left * sx + offset.X * sx;
                var above = anchor.Top - gap * sy - (popupSize.Height - shadow.Bottom * sy) + offset.Y * sy;
                var below = anchor.Bottom + gap * sy - shadow.Top * sy + offset.Y * sy;

                return new[]
                       {
                           new CustomPopupPlacement(new Point(x, above), PopupPrimaryAxis.Horizontal),
                           new CustomPopupPlacement(new Point(x, below), PopupPrimaryAxis.Horizontal)
                       };
            }
        }
    }
}
