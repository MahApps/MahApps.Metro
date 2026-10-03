// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The panel a tab control lays its headers out in, with the one thing the WPF panel cannot do:
    /// handing every tab the same width instead of the width of what is written on it.
    /// </summary>
    /// <remarks>
    /// That is what the TabView of WinUI does, and it is what turns a handful of short headers into
    /// a row of tabs rather than a line of words. The room is shared out between the tabs and what
    /// each one gets is held between its own <see cref="FrameworkElement.MinWidth"/> and
    /// <see cref="FrameworkElement.MaxWidth"/>; where that no longer fits, the strip runs past the
    /// edge of the control, which is the point at which the single row templates start scrolling.
    ///
    /// Everything else is left to the panel this one stands on: a strip down either side, a strip
    /// that wraps onto a second row, and the width that follows the content, which is what a tab
    /// control has always done and still does unless somebody asks for the other one.
    /// </remarks>
    public class TabPanelEx : TabPanel
    {
        /// <summary>Identifies the <see cref="TabWidthMode"/> dependency property.</summary>
        public static readonly DependencyProperty TabWidthModeProperty
            = DependencyProperty.Register(nameof(TabWidthMode),
                                          typeof(TabWidthMode),
                                          typeof(TabPanelEx),
                                          new FrameworkPropertyMetadata(TabWidthMode.SizeToContent, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets how wide the tabs in this panel are.
        /// </summary>
        public TabWidthMode TabWidthMode
        {
            get => (TabWidthMode)this.GetValue(TabWidthModeProperty);
            set => this.SetValue(TabWidthModeProperty, value);
        }

        /// <summary>
        /// The width the last measure handed each tab, which the arrange then hands out again.
        /// <see cref="double.NaN"/> while the panel is leaving the layout to the panel below it.
        /// </summary>
        private double sharedWidth = double.NaN;

        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            if (!this.SharesTheRoomOut(out var count))
            {
                this.sharedWidth = double.NaN;
                return base.MeasureOverride(constraint);
            }

            var width = this.WidthForEach(constraint.Width, count);
            var height = 0d;

            foreach (UIElement child in this.InternalChildren)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                child.Measure(new Size(width, constraint.Height));
                height = Math.Max(height, child.DesiredSize.Height);
            }

            this.sharedWidth = width;

            return new Size(width * count, height);
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (double.IsNaN(this.sharedWidth))
            {
                return base.ArrangeOverride(arrangeSize);
            }

            var left = 0d;

            foreach (UIElement child in this.InternalChildren)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                child.Arrange(new Rect(left, 0d, this.sharedWidth, arrangeSize.Height));
                left += this.sharedWidth;
            }

            return arrangeSize;
        }

        /// <summary>
        /// Whether this panel is the one doing the layout, which it is for a strip along the top or
        /// the bottom that was asked to share the room out and has a tab to share it between.
        /// </summary>
        private bool SharesTheRoomOut(out int count)
        {
            count = 0;

            if (this.TabWidthMode != TabWidthMode.Equal)
            {
                return false;
            }

            var placement = (ItemsControl.GetItemsOwner(this) ?? this.TemplatedParent) is TabControl tabControl
                ? tabControl.TabStripPlacement
                : Dock.Top;

            if (placement is Dock.Left or Dock.Right)
            {
                return false;
            }

            foreach (UIElement child in this.InternalChildren)
            {
                if (child.Visibility != Visibility.Collapsed)
                {
                    count++;
                }
            }

            return count > 0;
        }

        /// <summary>
        /// What one tab gets out of the room there is. Without a width to share, which is what a
        /// strip inside a scroll viewer is measured with, each tab takes the width of what is
        /// written on it and the floor and the ceiling still hold.
        /// </summary>
        private double WidthForEach(double available, int count)
        {
            var floor = 0d;
            var ceiling = double.PositiveInfinity;
            var content = 0d;

            foreach (UIElement child in this.InternalChildren)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                if (child is FrameworkElement element)
                {
                    floor = Math.Max(floor, element.MinWidth);
                    ceiling = Math.Min(ceiling, element.MaxWidth);
                }

                if (double.IsInfinity(available))
                {
                    child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    content = Math.Max(content, child.DesiredSize.Width);
                }
            }

            var width = double.IsInfinity(available) ? content : available / count;

            // a floor past the ceiling is somebody saying the tab is this wide and no other
            return Math.Max(floor, Math.Min(width, ceiling));
        }
    }
}
