// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The drop-down of the Windows 10 and the WinUI ToolBar: the commands that do not fit in the bar,
    /// one under the other and each as wide as the widest, the way the overflow of the CommandBar
    /// lists them. The panel it stands on puts items side by side as long as they fit and leaves each
    /// at its own width, so the highlight under the pointer would differ from line to line.
    /// </summary>
    /// <remarks>
    /// The ToolBar finds its drop-down as a <see cref="ToolBarOverflowPanel"/>, and the base class is
    /// still the one that moves the items over from the bar, so only where they go is different here.
    /// </remarks>
    internal sealed class ToolBarOverflowListPanel : ToolBarOverflowPanel
    {
        protected override Size MeasureOverride(Size constraint)
        {
            // takes the items over from the bar and measures them
            base.MeasureOverride(constraint);

            var size = new Size();
            foreach (UIElement child in this.InternalChildren)
            {
                size.Width = Math.Max(size.Width, child.DesiredSize.Width);
                size.Height += child.DesiredSize.Height;
            }

            return size;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var top = 0d;
            foreach (UIElement child in this.InternalChildren)
            {
                var height = child.DesiredSize.Height;
                child.Arrange(new Rect(0, top, arrangeSize.Width, height));
                top += height;
            }

            return arrangeSize;
        }
    }
}
