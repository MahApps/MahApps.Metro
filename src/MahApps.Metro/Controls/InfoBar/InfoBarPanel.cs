// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// Ported from InfoBarPanel.cpp of microsoft-ui-xaml, Copyright (c) Microsoft Corporation, MIT License.

using System;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Lays out the title, the message and the action of an <see cref="InfoBar"/>: side by side while they fit
    /// on one line, one under the other once they do not, or once one of them wraps.
    /// </summary>
    public class InfoBarPanel : Panel
    {
        private bool isVertical;

        /// <summary>Identifies the <see cref="HorizontalOrientationPadding"/> dependency property.</summary>
        public static readonly DependencyProperty HorizontalOrientationPaddingProperty
            = DependencyProperty.Register(nameof(HorizontalOrientationPadding),
                                          typeof(Thickness),
                                          typeof(InfoBarPanel),
                                          new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the padding around the items while they stand side by side.
        /// </summary>
        public Thickness HorizontalOrientationPadding
        {
            get => (Thickness)this.GetValue(HorizontalOrientationPaddingProperty);
            set => this.SetValue(HorizontalOrientationPaddingProperty, value);
        }

        /// <summary>Identifies the <see cref="VerticalOrientationPadding"/> dependency property.</summary>
        public static readonly DependencyProperty VerticalOrientationPaddingProperty
            = DependencyProperty.Register(nameof(VerticalOrientationPadding),
                                          typeof(Thickness),
                                          typeof(InfoBarPanel),
                                          new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the padding around the items while they stand one under the other.
        /// </summary>
        public Thickness VerticalOrientationPadding
        {
            get => (Thickness)this.GetValue(VerticalOrientationPaddingProperty);
            set => this.SetValue(VerticalOrientationPaddingProperty, value);
        }

        /// <summary>Identifies the HorizontalOrientationMargin attached property.</summary>
        public static readonly DependencyProperty HorizontalOrientationMarginProperty
            = DependencyProperty.RegisterAttached("HorizontalOrientationMargin",
                                                  typeof(Thickness),
                                                  typeof(InfoBarPanel),
                                                  new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>Gets the margin of an item while the items stand side by side.</summary>
        public static Thickness GetHorizontalOrientationMargin(UIElement element)
        {
            return (Thickness)element.GetValue(HorizontalOrientationMarginProperty);
        }

        /// <summary>Sets the margin of an item while the items stand side by side.</summary>
        public static void SetHorizontalOrientationMargin(UIElement element, Thickness value)
        {
            element.SetValue(HorizontalOrientationMarginProperty, value);
        }

        /// <summary>Identifies the VerticalOrientationMargin attached property.</summary>
        public static readonly DependencyProperty VerticalOrientationMarginProperty
            = DependencyProperty.RegisterAttached("VerticalOrientationMargin",
                                                  typeof(Thickness),
                                                  typeof(InfoBarPanel),
                                                  new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>Gets the margin of an item while the items stand one under the other.</summary>
        public static Thickness GetVerticalOrientationMargin(UIElement element)
        {
            return (Thickness)element.GetValue(VerticalOrientationMarginProperty);
        }

        /// <summary>Sets the margin of an item while the items stand one under the other.</summary>
        public static void SetVerticalOrientationMargin(UIElement element, Thickness value)
        {
            element.SetValue(VerticalOrientationMarginProperty, value);
        }

        private static bool Counts(UIElement child)
        {
            return child.DesiredSize.Width > 0 && child.DesiredSize.Height > 0;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var children = this.InternalChildren;
            var last = LastCounting(children, measureFirst: true, availableSize);

            double totalWidth = 0;
            double totalHeight = 0;
            double widthOfWidest = 0;
            double heightOfTallest = 0;
            double heightOfTallestInHorizontal = 0;
            var count = 0;

            for (var i = 0; i < children.Count; i++)
            {
                var child = children[i];
                if (!Counts(child))
                {
                    continue;
                }

                var desired = child.DesiredSize;
                var horizontalMargin = GetHorizontalOrientationMargin(child);
                var verticalMargin = GetVerticalOrientationMargin(child);

                count++;

                // the first item has no margin before it and the last none after it
                totalWidth += desired.Width + (count > 1 ? horizontalMargin.Left : 0) + (i != last ? horizontalMargin.Right : 0);
                totalHeight += desired.Height + (count > 1 ? verticalMargin.Top : 0) + (i != last ? verticalMargin.Bottom : 0);

                widthOfWidest = Math.Max(widthOfWidest, desired.Width);
                heightOfTallest = Math.Max(heightOfTallest, desired.Height);
                heightOfTallestInHorizontal = Math.Max(heightOfTallestInHorizontal, desired.Height + horizontalMargin.Top + horizontalMargin.Bottom);
            }

            // a line taller than the bar is high means something wrapped, and then the items go one under the other
            var minHeight = this.Parent is FrameworkElement parent ? parent.MinHeight - (this.Margin.Top + this.Margin.Bottom) : 0;

            this.isVertical = count == 1
                              || totalWidth > availableSize.Width
                              || (minHeight > 0 && heightOfTallestInHorizontal > minHeight);

            if (this.isVertical)
            {
                var padding = this.VerticalOrientationPadding;
                return new Size(widthOfWidest + padding.Left + padding.Right, totalHeight + padding.Top + padding.Bottom);
            }
            else
            {
                var padding = this.HorizontalOrientationPadding;
                return new Size(totalWidth + padding.Left + padding.Right, heightOfTallest + padding.Top + padding.Bottom);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var children = this.InternalChildren;
            var last = LastCounting(children, measureFirst: false, finalSize);
            var first = true;

            if (this.isVertical)
            {
                var padding = this.VerticalOrientationPadding;
                var y = padding.Top;

                for (var i = 0; i < children.Count; i++)
                {
                    var child = children[i];
                    if (!Counts(child))
                    {
                        child.Arrange(new Rect());
                        continue;
                    }

                    var margin = GetVerticalOrientationMargin(child);
                    y += first ? 0 : margin.Top;
                    child.Arrange(new Rect(padding.Left + margin.Left, y, child.DesiredSize.Width, child.DesiredSize.Height));
                    y += child.DesiredSize.Height + margin.Bottom;
                    first = false;
                }
            }
            else
            {
                var padding = this.HorizontalOrientationPadding;
                var x = padding.Left;

                for (var i = 0; i < children.Count; i++)
                {
                    var child = children[i];
                    if (!Counts(child))
                    {
                        child.Arrange(new Rect());
                        continue;
                    }

                    var margin = GetHorizontalOrientationMargin(child);
                    x += first ? 0 : margin.Left;

                    // the last item takes what is left of the line, so an action can sit at its right end
                    var width = i == last ? Math.Max(child.DesiredSize.Width, finalSize.Width - x) : child.DesiredSize.Width;
                    child.Arrange(new Rect(x, padding.Top + margin.Top, width, child.DesiredSize.Height));
                    x += width + margin.Right;
                    first = false;
                }
            }

            return finalSize;
        }

        private static int LastCounting(UIElementCollection children, bool measureFirst, Size availableSize)
        {
            var last = -1;
            for (var i = 0; i < children.Count; i++)
            {
                if (measureFirst)
                {
                    children[i].Measure(availableSize);
                }

                if (Counts(children[i]))
                {
                    last = i;
                }
            }

            return last;
        }
    }
}
