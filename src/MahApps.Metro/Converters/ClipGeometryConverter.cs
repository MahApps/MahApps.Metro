// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MahApps.Metro.Converters
{
    public class ClipGeometryConverter : IMultiValueConverter
    {
        public static readonly ClipGeometryConverter Instance = new();

        /// <summary>
        /// Builds the geometry for a <see cref="UIElement.Clip"/> on the content of a border. It takes
        /// the width and the height of the element carrying the clip, and optionally the corner radius,
        /// the border thickness and the padding of the border around it.
        /// </summary>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2
                || values[0] is not double width
                || values[1] is not double height
                || width <= 0
                || height <= 0)
            {
                return DependencyProperty.UnsetValue;
            }

            if (width < 1.0 || height < 1.0)
            {
                return Geometry.Empty;
            }

            var cornerRadius = values.Length > 2 && values[2] is CornerRadius radius ? radius : default;
            var borderThickness = values.Length > 3 && values[3] is Thickness thickness ? thickness : default;
            var padding = values.Length > 4 && values[4] is Thickness thicknessPadding ? thicknessPadding : default;

            // Half of the border and the whole padding, which is how the WPF Border draws the inner edge
            // of its own frame. Taking the whole thickness would leave a gap between the frame and the
            // content along every rounded corner.
            var inset = new Thickness(0.5 * borderThickness.Left + padding.Left,
                                      0.5 * borderThickness.Top + padding.Top,
                                      0.5 * borderThickness.Right + padding.Right,
                                      0.5 * borderThickness.Bottom + padding.Bottom);

            var geometry = GetRoundRectangle(new Rect(0, 0, width, height), inset, cornerRadius);
            geometry.Freeze();

            return geometry;
        }

        public object?[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();

        // Started from https://wpfspark.wordpress.com/2011/06/08/clipborder-a-wpf-border-that-clips/
        private static Geometry GetRoundRectangle(Rect baseRect, Thickness inset, CornerRadius cornerRadius)
        {
            // The clip sits on the content of the border, so every corner loses what the frame takes away
            // from it on the two sides that meet there. A corner is an ellipse rather than a circle
            // whenever those two differ.
            var topLeftRect = CornerRect(cornerRadius.TopLeft - inset.Left, cornerRadius.TopLeft - inset.Top);
            var topRightRect = CornerRect(cornerRadius.TopRight - inset.Right, cornerRadius.TopRight - inset.Top);
            var bottomRightRect = CornerRect(cornerRadius.BottomRight - inset.Right, cornerRadius.BottomRight - inset.Bottom);
            var bottomLeftRect = CornerRect(cornerRadius.BottomLeft - inset.Left, cornerRadius.BottomLeft - inset.Bottom);

            // Put each of them in its own corner of the rectangle. Size first, then position, so that a
            // corner which was cut back to nothing still sits on the edge instead of beyond it.
            topLeftRect.Location = baseRect.TopLeft;
            topRightRect.Location = new Point(baseRect.Right - topRightRect.Width, baseRect.Top);
            bottomRightRect.Location = new Point(baseRect.Right - bottomRightRect.Width, baseRect.Bottom - bottomRightRect.Height);
            bottomLeftRect.Location = new Point(baseRect.Left, baseRect.Bottom - bottomLeftRect.Height);

            // Two corners sharing a side can ask for more than the side has. Then they share it in
            // proportion instead of overlapping.
            if (topLeftRect.Right > topRightRect.Left)
            {
                var newWidth = topLeftRect.Width / (topLeftRect.Width + topRightRect.Width) * baseRect.Width;
                topLeftRect = new Rect(topLeftRect.Location, new Size(newWidth, topLeftRect.Height));
                topRightRect = new Rect(new Point(baseRect.Left + newWidth, topRightRect.Top),
                                        new Size(Math.Max(0.0, baseRect.Width - newWidth), topRightRect.Height));
            }

            if (topRightRect.Bottom > bottomRightRect.Top)
            {
                var newHeight = topRightRect.Height / (topRightRect.Height + bottomRightRect.Height) * baseRect.Height;
                topRightRect = new Rect(topRightRect.Location, new Size(topRightRect.Width, newHeight));
                bottomRightRect = new Rect(new Point(bottomRightRect.Left, baseRect.Top + newHeight),
                                           new Size(bottomRightRect.Width, Math.Max(0.0, baseRect.Height - newHeight)));
            }

            if (bottomRightRect.Left < bottomLeftRect.Right)
            {
                var newWidth = bottomLeftRect.Width / (bottomLeftRect.Width + bottomRightRect.Width) * baseRect.Width;
                bottomLeftRect = new Rect(bottomLeftRect.Location, new Size(newWidth, bottomLeftRect.Height));
                bottomRightRect = new Rect(new Point(baseRect.Left + newWidth, bottomRightRect.Top),
                                           new Size(Math.Max(0.0, baseRect.Width - newWidth), bottomRightRect.Height));
            }

            if (bottomLeftRect.Top < topLeftRect.Bottom)
            {
                var newHeight = topLeftRect.Height / (topLeftRect.Height + bottomLeftRect.Height) * baseRect.Height;
                topLeftRect = new Rect(topLeftRect.Location, new Size(topLeftRect.Width, newHeight));
                bottomLeftRect = new Rect(new Point(bottomLeftRect.Left, baseRect.Top + newHeight),
                                          new Size(bottomLeftRect.Width, Math.Max(0.0, baseRect.Height - newHeight)));
            }

            StreamGeometry roundedRectGeometry = new();

            using (var context = roundedRectGeometry.Open())
            {
                // Begin from the bottom of the top left arc and proceed clockwise
                context.BeginFigure(topLeftRect.BottomLeft, true, true);

                context.ArcTo(topLeftRect.TopRight, topLeftRect.Size, 0, false, SweepDirection.Clockwise, true, true);
                context.LineTo(topRightRect.TopLeft, true, true);
                context.ArcTo(topRightRect.BottomRight, topRightRect.Size, 0, false, SweepDirection.Clockwise, true, true);
                context.LineTo(bottomRightRect.TopRight, true, true);
                context.ArcTo(bottomRightRect.BottomLeft, bottomRightRect.Size, 0, false, SweepDirection.Clockwise, true, true);
                context.LineTo(bottomLeftRect.BottomRight, true, true);
                context.ArcTo(bottomLeftRect.TopLeft, bottomLeftRect.Size, 0, false, SweepDirection.Clockwise, true, true);
            }

            return roundedRectGeometry;
        }

        private static Rect CornerRect(double width, double height)
        {
            return new Rect(0, 0, Math.Max(0.0, width), Math.Max(0.0, height));
        }
    }
}