// Copyright (c) 2017 Ratish Philip 
//
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions: 
// 
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software. 
// 
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE. 
//
// This file is part of the WPFSpark project: https://github.com/ratishphilip/wpfspark
//
// WPFSpark v1.3.1
// 

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Represents a border whose contents are clipped within the bounds
    /// of the border. The border may have rounded corners.
    /// </summary>
    public sealed class ClipBorder : Decorator
    {
        #region Fields

        private StreamGeometry _backgroundGeometryCache;
        private StreamGeometry _borderGeometryCache;

        #endregion

        #region Dependency Properties

        #region BorderThickness

        /// <summary>
        /// BorderThickness Dependency Property
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(ClipBorder),
                new FrameworkPropertyMetadata(new Thickness(),
                                              FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
                                              OnValidateThickness);

        /// <summary>
        /// Gets or sets the BorderThickness property. This dependency property 
        /// indicates the BorderThickness.
        /// </summary>
        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        /// <summary>
        /// Checks if the given Thickness is valid or not
        /// </summary>
        /// <param name="value">Thickness</param>
        /// <returns></returns>
        private static bool OnValidateThickness(object value)
        {
            var th = (Thickness)value;
            return th.IsValid(false, false, false, false);
        }

        //      /// <summary>
        //      /// Provides derived classes an opportunity to handle changes to the BorderThickness property.
        //      /// </summary>
        ///// <param name="oldBorderThickness">Old Value</param>
        ///// <param name="newBorderThickness">New Value</param>
        //      void OnBorderThicknessChanged(Thickness oldBorderThickness, Thickness newBorderThickness)
        //      {

        //      }

        #endregion

        #region Padding

        /// <summary>
        /// Padding Dependency Property
        /// </summary>
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(Thickness), typeof(ClipBorder),
                                        new FrameworkPropertyMetadata(new Thickness(),
                                              FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender), 
                                        OnValidateThickness);

        /// <summary>
        /// Gets or sets the Padding property. This dependency property 
        /// indicates the Padding.
        /// </summary>
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        //      /// <summary>
        //      /// Provides derived classes an opportunity to handle changes to the Padding property.
        //      /// </summary>
        ///// <param name="oldPadding">Old Value</param>
        ///// <param name="newPadding">New Value</param>
        //      void OnPaddingChanged(Thickness oldPadding, Thickness newPadding)
        //      {

        //      }

        #endregion

        #region CornerRadius

        /// <summary>
        /// CornerRadius Dependency Property
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ClipBorder),
                new FrameworkPropertyMetadata(new CornerRadius(),
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender), 
                OnValidateCornerRadius);

        /// <summary>
        /// Gets or sets the CornerRadius property. This dependency property 
        /// indicates the CornerRadius of the border.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Checks if the given CornerRadius is valid or not
        /// </summary>
        /// <param name="value">CornerRadius</param>
        /// <returns></returns>
        private static bool OnValidateCornerRadius(object value)
        {
            var cr = (CornerRadius)value;
            return cr.IsValid(false, false, false, false);
        }

        //      /// <summary>
        //      /// Provides derived classes an opportunity to handle changes to the CornerRadius property.
        //      /// </summary>
        ///// <param name="oldCornerRadius">Old Value</param>
        ///// <param name="newCornerRadius">New Value</param>
        //      void OnCornerRadiusChanged(CornerRadius oldCornerRadius, CornerRadius newCornerRadius)
        //      {

        //      }

        #endregion

        #region BorderBrush

        /// <summary>
        /// BorderBrush Dependency Property
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(ClipBorder),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        /// <summary>
        /// Gets or sets the BorderBrush property. This dependency property 
        /// indicates the BorderBrush with which the Border is drawn.
        /// </summary>
        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        #endregion

        #region Background

        /// <summary>
        /// Background Dependency Property
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(ClipBorder),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        /// <summary>
        /// Gets or sets the Background property. This dependency property 
        /// indicates the Background with which the Background is drawn.
        /// </summary>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        #endregion

        #region OptimizeClipRendering

        /// <summary>
        /// OptimizeClipRendering Dependency Property
        /// </summary>
        public static readonly DependencyProperty OptimizeClipRenderingProperty =
            DependencyProperty.Register("OptimizeClipRendering", typeof(bool), typeof(ClipBorder),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the OptimizeClipRendering property. This dependency property 
        /// indicates whether the rendering of the clip should be optimized. When set to true,
        /// In order to optimize the rendering of the clipped Child,
        /// the background is rendered with the same brush as the border. Any other brush set for
        /// the background will be ignored. The Child will be rendered on top of it. 
        /// This is done to prevent any gaps between the border the the clipped Child (this is 
        /// evidently visible if both the Border and the Child are of same color).
        /// This works best when the Child does not have any level of transparency and is opaque.
        /// </summary>
        public bool OptimizeClipRendering
        {
            get { return (bool)GetValue(OptimizeClipRenderingProperty); }
            set { SetValue(OptimizeClipRenderingProperty, value); }
        }

        #endregion

        #endregion

        #region Protected Methods

        /// <summary>
        /// Updates DesiredSize of the ClipBorder.  Called by parent UIElement.  This is the first pass of layout.
        /// </summary>
        /// <remarks>
        /// Border determines its desired size it needs from the specified border the child: its sizing
        /// properties, margin, and requested size.
        /// </remarks>
        /// <param name="constraint">Constraint size is an "upper limit" that the return value should not exceed.</param>
        /// <returns>The Decorator's desired size.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            var child = Child;
            var desiredSize = new Size();
            var borders = BorderThickness;

            // Compute the total size required
            var borderSize = borders.CollapseThickness();
            var paddingSize = Padding.CollapseThickness();

            // Does the ClipBorder have a child?
            if (child != null)
            {
                // Combine into total decorating size
                var combined = new Size(borderSize.Width + paddingSize.Width, borderSize.Height + paddingSize.Height);

                // Remove size of border only from child's reference size.
                var childConstraint = new Size(Math.Max(0.0, constraint.Width - combined.Width),
                                                Math.Max(0.0, constraint.Height - combined.Height));


                child.Measure(childConstraint);
                var childSize = child.DesiredSize;

                // Now use the returned size to drive our size, by adding back the margins, etc.
                desiredSize.Width = childSize.Width + combined.Width;
                desiredSize.Height = childSize.Height + combined.Height;
            }
            else
            {
                // Since there is no child, the border requires only the size occupied by the BorderThickness
                // and the Padding
                desiredSize = new Size(borderSize.Width + paddingSize.Width, borderSize.Height + paddingSize.Height);
            }

            return desiredSize;
        }

        /// <summary>
        /// ClipBorder computes the position of its single child and applies its child's alignments to the child.
        /// 
        /// </summary>
        /// <param name="finalSize">The size reserved for this element by the parent</param>
        /// <returns>The actual ink area of the element, typically the same as finalSize</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var borders = BorderThickness;
            var boundRect = new Rect(finalSize);
            var innerRect = boundRect.Deflate(borders);
            var corners = CornerRadius;
            var padding = Padding;
            var childRect = innerRect.Deflate(padding);

            //  calculate border rendering geometry
            if (!boundRect.Width.IsZero() && !boundRect.Height.IsZero())
            {
                var outerBorderInfo = new BorderInfo(corners, borders, new Thickness(), true);
                var borderGeometry = new StreamGeometry();

                using (var ctx = borderGeometry.Open())
                {
                    GenerateGeometry(ctx, boundRect, outerBorderInfo);
                }
                // Freeze the geometry for better perfomance
                borderGeometry.Freeze();
                _borderGeometryCache = borderGeometry;
            }
            else
            {
                _borderGeometryCache = null;
            }

            //  calculate background rendering geometry
            if (!innerRect.Width.IsZero() && !innerRect.Height.IsZero())
            {
                var innerBorderInfo = new BorderInfo(corners, borders, new Thickness(), false);
                var backgroundGeometry = new StreamGeometry();

                using (var ctx = backgroundGeometry.Open())
                {
                    GenerateGeometry(ctx, innerRect, innerBorderInfo);
                }
                // Freeze the geometry for better perfomance
                backgroundGeometry.Freeze();
                _backgroundGeometryCache = backgroundGeometry;
            }
            else
            {
                _backgroundGeometryCache = null;
            }

            //  Arrange the Child and set its clip
            var child = Child;
            if (child != null)
            {
                child.Arrange(childRect);
                // Calculate the Clipping Geometry
                var clipGeometry = new StreamGeometry();
                var childBorderInfo = new BorderInfo(corners, borders, padding, false);
                using (var ctx = clipGeometry.Open())
                {
                    GenerateGeometry(ctx, new Rect(0, 0, childRect.Width, childRect.Height), childBorderInfo);
                }
                // Freeze the geometry for better perfomance
                clipGeometry.Freeze();
                // Apply the clip to the Child
                child.Clip = clipGeometry;
            }

            return finalSize;
        }

        /// <summary>
        /// Here the ClipBorder's Child, Border and Background are rendered.
        /// </summary>
        /// <param name="dc">Drawing Context</param>
        protected override void OnRender(DrawingContext dc)
        {
            var borders = BorderThickness;
            var borderBrush = BorderBrush;
            var bgBrush = Background;
            var borderGeometry = _borderGeometryCache;
            var backgroundGeometry = _backgroundGeometryCache;
            var optimizeClipRendering = OptimizeClipRendering;

            // First check if the user wants optimized rendering of the clipped Child
            if (optimizeClipRendering)
            {
                // In order to optimize the rendering of the clipped Child,
                // just draw the borderGeometry filled with BorderBrush. The Child
                // will be rendered on top of it. This is done to prevent any gaps
                // between the border the the clipped Child (this is evidently visible
                // if both the Border and the Child are of same color)
                dc.DrawGeometry(borderBrush, null, borderGeometry);

                return;
            }

            // If both Border and Background are valid
            if ((borderBrush != null) && (!borders.IsZero()) && (bgBrush != null))
            {
                // If both the background and border brushes are same,
                // just draw the filled borderGeometry
                if (borderBrush.IsEqualTo(bgBrush))
                {
                    dc.DrawGeometry(borderBrush, null, borderGeometry);
                }
                // If both are opaque SolidColorBrushes, first draw the borderGeometry filled
                // with borderbrush and then draw the backgroundGeometry filled with background brush
                else if (borderBrush.IsOpaqueSolidColorBrush() && bgBrush.IsOpaqueSolidColorBrush())
                {
                    dc.DrawGeometry(borderBrush, null, borderGeometry);
                    dc.DrawGeometry(bgBrush, null, backgroundGeometry);
                }
                // If only the border is opaque, then first draw the borderGeometry filled with
                // background brush and then draw ONLY the borderOutlineGeometry (obtained by excluding 
                // backgroundGeometry from borderGeometry) with the border brush.
                // This will prevent gaps between the border and the background while rendering.
                else if (borderBrush.IsOpaqueSolidColorBrush())
                {
                    if ((borderGeometry == null) || (backgroundGeometry == null))
                        return;

                    var borderOutlinePath = borderGeometry.GetOutlinedPathGeometry();
                    var backgroundOutlinePath = backgroundGeometry.GetOutlinedPathGeometry();
                    var borderOutlineGeometry = Geometry.Combine(borderOutlinePath, backgroundOutlinePath,
                        GeometryCombineMode.Exclude, null);

                    dc.DrawGeometry(bgBrush, null, borderGeometry);
                    dc.DrawGeometry(borderBrush, null, borderOutlineGeometry);
                }
                // If none of the above, then it means that the border and the background must be separately drawn.
                // This might result in small gaps between the border and the background
                // Draw the borderOutlineGeometry and backgroundGeometry separately with their respective brushes
                else
                {
                    if ((borderGeometry == null) || (backgroundGeometry == null))
                        return;

                    var borderOutlinePath = borderGeometry.GetOutlinedPathGeometry();
                    var backgroundOutlinePath = backgroundGeometry.GetOutlinedPathGeometry();
                    var borderOutlineGeometry = Geometry.Combine(borderOutlinePath, backgroundOutlinePath,
                        GeometryCombineMode.Exclude, null);

                    dc.DrawGeometry(borderBrush, null, borderOutlineGeometry);
                    dc.DrawGeometry(bgBrush, null, backgroundGeometry);
                }

                return;
            }

            // Only Border is valid
            if ((borderBrush != null) && (!borders.IsZero()))
            {
                if ((borderGeometry != null) && (backgroundGeometry != null))
                {
                    var borderOutlinePath = borderGeometry.GetOutlinedPathGeometry();
                    var backgroundOutlinePath = backgroundGeometry.GetOutlinedPathGeometry();
                    var borderOutlineGeometry = Geometry.Combine(borderOutlinePath, backgroundOutlinePath,
                        GeometryCombineMode.Exclude, null);

                    dc.DrawGeometry(borderBrush, null, borderOutlineGeometry);
                }
                else
                {
                    dc.DrawGeometry(borderBrush, null, borderGeometry);
                }
            }

            // Only Background is valid
            if (bgBrush != null)
            {
                dc.DrawGeometry(bgBrush, null, backgroundGeometry);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        ///     Generates a StreamGeometry.
        /// </summary>
        /// <param name="ctx">An already opened StreamGeometryContext.</param>
        /// <param name="rect">Rectangle for geomentry conversion.</param>
        /// <param name="borderInfo">The core points of the border which needs to be used to create
        /// the geometry</param>
        /// <returns>Result geometry.</returns>
        private static void GenerateGeometry(StreamGeometryContext ctx, Rect rect, BorderInfo borderInfo)
        {
            //  compute the coordinates of the key points
            var leftTop = new Point(borderInfo.LeftTop, 0);
            var rightTop = new Point(rect.Width - borderInfo.RightTop, 0);
            var topRight = new Point(rect.Width, borderInfo.TopRight);
            var bottomRight = new Point(rect.Width, rect.Height - borderInfo.BottomRight);
            var rightBottom = new Point(rect.Width - borderInfo.RightBottom, rect.Height);
            var leftBottom = new Point(borderInfo.LeftBottom, rect.Height);
            var bottomLeft = new Point(0, rect.Height - borderInfo.BottomLeft);
            var topLeft = new Point(0, borderInfo.TopLeft);

            //  check keypoints for overlap and resolve by partitioning corners according to
            //  the percentage of each one.  

            //  top edge
            if (leftTop.X > rightTop.X)
            {
                var v = (borderInfo.LeftTop) / (borderInfo.LeftTop + borderInfo.RightTop) * rect.Width;
                leftTop.X = v;
                rightTop.X = v;
            }

            //  right edge
            if (topRight.Y > bottomRight.Y)
            {
                var v = (borderInfo.TopRight) / (borderInfo.TopRight + borderInfo.BottomRight) * rect.Height;
                topRight.Y = v;
                bottomRight.Y = v;
            }

            //  bottom edge
            if (leftBottom.X > rightBottom.X)
            {
                var v = (borderInfo.LeftBottom) / (borderInfo.LeftBottom + borderInfo.RightBottom) * rect.Width;
                rightBottom.X = v;
                leftBottom.X = v;
            }

            // left edge
            if (topLeft.Y > bottomLeft.Y)
            {
                var v = (borderInfo.TopLeft) / (borderInfo.TopLeft + borderInfo.BottomLeft) * rect.Height;
                bottomLeft.Y = v;
                topLeft.Y = v;
            }

            // Apply offset
            var offsetX = rect.TopLeft.X;
            var offsetY = rect.TopLeft.Y;
            var offset = new Vector(offsetX, offsetY);
            leftTop += offset;
            rightTop += offset;
            topRight += offset;
            bottomRight += offset;
            rightBottom += offset;
            leftBottom += offset;
            bottomLeft += offset;
            topLeft += offset;

            //  create the border geometry
            ctx.BeginFigure(leftTop, true /* is filled */, true /* is closed */);

            // Top line
            ctx.LineTo(rightTop, true /* is stroked */, false /* is smooth join */);

            // Upper-right corners
            var radiusX = rect.TopRight.X - rightTop.X;
            var radiusY = topRight.Y - rect.TopRight.Y;
            if (!radiusX.IsZero() || !radiusY.IsZero())
            {
                ctx.ArcTo(topRight, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);
            }

            // Right line
            ctx.LineTo(bottomRight, true /* is stroked */, false /* is smooth join */);

            // Lower-right corners
            radiusX = rect.BottomRight.X - rightBottom.X;
            radiusY = rect.BottomRight.Y - bottomRight.Y;
            if (!radiusX.IsZero() || !radiusY.IsZero())
            {
                ctx.ArcTo(rightBottom, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);
            }

            // Bottom line
            ctx.LineTo(leftBottom, true /* is stroked */, false /* is smooth join */);

            // Lower-left corners
            radiusX = leftBottom.X - rect.BottomLeft.X;
            radiusY = rect.BottomLeft.Y - bottomLeft.Y;
            if (!radiusX.IsZero() || !radiusY.IsZero())
            {
                ctx.ArcTo(bottomLeft, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);
            }

            // Left line
            ctx.LineTo(topLeft, true /* is stroked */, false /* is smooth join */);

            // Upper-left corners
            radiusX = leftTop.X - rect.TopLeft.X;
            radiusY = topLeft.Y - rect.TopLeft.Y;
            if (!radiusX.IsZero() || !radiusY.IsZero())
            {
                ctx.ArcTo(leftTop, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);
            }
        }

        #endregion

        #region Private Structures

        private struct BorderInfo
        {
            #region Fields

            internal readonly double LeftTop;
            internal readonly double TopLeft;
            internal readonly double TopRight;
            internal readonly double RightTop;
            internal readonly double RightBottom;
            internal readonly double BottomRight;
            internal readonly double BottomLeft;
            internal readonly double LeftBottom;

            #endregion

            #region Construction / Initialization

            /// <summary>
            /// Encapsulates the details of each of the core points of the border which is calculated
            /// based on the given CornerRadius, BorderThickness, Padding and a flag to indicate whether
            /// the inner or outer border is to be calculated.
            /// </summary>
            /// <param name="corners">CornerRadius</param>
            /// <param name="borders">BorderThickness</param>
            /// <param name="padding">Padding</param>
            /// <param name="isOuterBorder">Flag to indicate whether outer or inner border needs 
            /// to be calculated</param>
            internal BorderInfo(CornerRadius corners, Thickness borders, Thickness padding, bool isOuterBorder)
            {
                var left = 0.5 * borders.Left + padding.Left;
                var top = 0.5 * borders.Top + padding.Top;
                var right = 0.5 * borders.Right + padding.Right;
                var bottom = 0.5 * borders.Bottom + padding.Bottom;

                if (isOuterBorder)
                {
                    if (corners.TopLeft.IsZero())
                    {
                        LeftTop = TopLeft = 0.0;
                    }
                    else
                    {
                        LeftTop = corners.TopLeft + left;
                        TopLeft = corners.TopLeft + top;
                    }
                    if (corners.TopRight.IsZero())
                    {
                        TopRight = RightTop = 0.0;
                    }
                    else
                    {
                        TopRight = corners.TopRight + top;
                        RightTop = corners.TopRight + right;
                    }
                    if (corners.BottomRight.IsZero())
                    {
                        RightBottom = BottomRight = 0.0;
                    }
                    else
                    {
                        RightBottom = corners.BottomRight + right;
                        BottomRight = corners.BottomRight + bottom;
                    }
                    if (corners.BottomLeft.IsZero())
                    {
                        BottomLeft = LeftBottom = 0.0;
                    }
                    else
                    {
                        BottomLeft = corners.BottomLeft + bottom;
                        LeftBottom = corners.BottomLeft + left;
                    }
                }
                else
                {
                    LeftTop = Math.Max(0.0, corners.TopLeft - left);
                    TopLeft = Math.Max(0.0, corners.TopLeft - top);
                    TopRight = Math.Max(0.0, corners.TopRight - top);
                    RightTop = Math.Max(0.0, corners.TopRight - right);
                    RightBottom = Math.Max(0.0, corners.BottomRight - right);
                    BottomRight = Math.Max(0.0, corners.BottomRight - bottom);
                    BottomLeft = Math.Max(0.0, corners.BottomLeft - bottom);
                    LeftBottom = Math.Max(0.0, corners.BottomLeft - left);
                }
            }

            #endregion
        }

        #endregion 
    }
}