// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace MahApps.Metro.Controls
{
    internal static class ColorEyePreview
    {
        public static ToolTip GetPreviewToolTip(ColorEyeDropper target)
        {
            var toolTip = new ToolTip
                          {
                              PlacementTarget = target,
                              Focusable = false,
                              Placement = PlacementMode.Relative,
                              StaysOpen = true,
                              HorizontalOffset = -9999,
                              VerticalOffset = -9999,
                              IsHitTestVisible = false,
                              AllowDrop = false,
                              IsOpen = false,
                              Visibility = Visibility.Collapsed,
                              DataContext = target,
                              Content = target.previewData
                          };
            BindingOperations.SetBinding(toolTip, ContentControl.ContentTemplateProperty, new Binding { Path = new PropertyPath(ColorEyeDropper.PreviewContentTemplateProperty), Source = target });
            return toolTip;
        }

        public static void Show(this ToolTip toolTip)
        {
            toolTip.Visibility = Visibility.Visible;
            toolTip.IsOpen = true;
        }

        public static void Hide(this ToolTip toolTip)
        {
            toolTip.IsOpen = false;
            toolTip.Visibility = Visibility.Collapsed;
        }

        public static void Move(this ToolTip toolTip, Point point, Point offset)
        {
            var translationX = point.X + offset.X;
            var translationY = point.Y + offset.Y;

            toolTip.Placement = PlacementMode.Relative;
            toolTip.SetCurrentValue(ToolTip.HorizontalOffsetProperty, translationX);
            toolTip.SetCurrentValue(ToolTip.VerticalOffsetProperty, translationY);
        }
    }
}