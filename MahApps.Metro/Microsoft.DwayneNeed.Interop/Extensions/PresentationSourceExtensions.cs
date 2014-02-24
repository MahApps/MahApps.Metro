using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.DwayneNeed.Extensions
{
    public static class PresentationSourceExtensions
    {
        /// <summary>
        ///     Convert a point from "client" coordinate space of a window into
        ///     the coordinate space of the specified element of the same window.
        /// </summary>
        public static Point TransformClientToDescendant(this PresentationSource presentationSource, Point point, Visual descendant)
        {
            Point pt = TransformClientToRoot(presentationSource, point);
            return presentationSource.RootVisual.TransformToDescendant(descendant).Transform(pt);
        }

        /// <summary>
        ///     Convert a rectangle from "client" coordinate space of a window
        ///     into the coordinate space of the specified element of the same
        ///     window.
        /// </summary>
        public static Rect TransformClientToDescendant(this PresentationSource presentationSource, Rect rect, Visual descendant)
        {
            // Transform all 4 corners.  Since a rectangle is convex, it will
            // remain convex under affine transforms.
            Point pt1 = TransformClientToDescendant(presentationSource, new Point(rect.Left, rect.Top), descendant);
            Point pt2 = TransformClientToDescendant(presentationSource, new Point(rect.Right, rect.Top), descendant);
            Point pt3 = TransformClientToDescendant(presentationSource, new Point(rect.Right, rect.Bottom), descendant);
            Point pt4 = TransformClientToDescendant(presentationSource, new Point(rect.Left, rect.Bottom), descendant);

            double minX = Math.Min(pt1.X, Math.Min(pt2.X, Math.Min(pt3.X, pt4.X)));
            double minY = Math.Min(pt1.Y, Math.Min(pt2.Y, Math.Min(pt3.Y, pt4.Y)));
            double maxX = Math.Max(pt1.X, Math.Max(pt2.X, Math.Max(pt3.X, pt4.X)));
            double maxY = Math.Max(pt1.Y, Math.Max(pt2.Y, Math.Max(pt3.Y, pt4.Y)));

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        ///     Convert a point from the coordinate space of the specified
        ///     element into the "client" coordinate space of the window.
        /// </summary>
        public static Point TransformDescendantToClient(this PresentationSource presentationSource, Point point, Visual descendant)
        {
            Point pt = descendant.TransformToAncestor(presentationSource.RootVisual).Transform(point);
            return TransformRootToClient(presentationSource, pt);
        }

        /// <summary>
        ///     Convert a rectangle from the coordinate space of the specified
        ///     element into the "client" coordinate space of the window.
        /// </summary>
        public static Rect TransformDescendantToClient(this PresentationSource presentationSource, Rect rect, Visual descendant)
        {
            // Transform all 4 corners.  Since a rectangle is convex, it will
            // remain convex under affine transforms.
            Point pt1 = TransformDescendantToClient(presentationSource, new Point(rect.Left, rect.Top), descendant);
            Point pt2 = TransformDescendantToClient(presentationSource, new Point(rect.Right, rect.Top), descendant);
            Point pt3 = TransformDescendantToClient(presentationSource, new Point(rect.Right, rect.Bottom), descendant);
            Point pt4 = TransformDescendantToClient(presentationSource, new Point(rect.Left, rect.Bottom), descendant);

            double minX = Math.Min(pt1.X, Math.Min(pt2.X, Math.Min(pt3.X, pt4.X)));
            double minY = Math.Min(pt1.Y, Math.Min(pt2.Y, Math.Min(pt3.Y, pt4.Y)));
            double maxX = Math.Max(pt1.X, Math.Max(pt2.X, Math.Max(pt3.X, pt4.X)));
            double maxY = Math.Max(pt1.Y, Math.Max(pt2.Y, Math.Max(pt3.Y, pt4.Y)));

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        ///     Convert a point from "client" coordinate space of a window into
        ///     the coordinate space of the root element of the same window.
        /// </summary>
        public static Point TransformClientToRoot(this PresentationSource presentationSource, Point pt)
        {
            // Convert from pixels into DIPs.
            pt = presentationSource.CompositionTarget.TransformFromDevice.Transform(pt);

            // We need to include the root element's transform.
            pt = ApplyVisualTransform(presentationSource.RootVisual, pt, true);

            return pt;
        }

        /// <summary>
        ///     Convert a point from the coordinate space of the root element
        ///     into the "client" coordinate space of the same window.
        /// </summary>
        public static Point TransformRootToClient(this PresentationSource presentationSource, Point pt)
        {
            // We need to include the root element's transform.
            pt = ApplyVisualTransform(presentationSource.RootVisual, pt, false);

            // Convert from DIPs into pixels.
            pt = presentationSource.CompositionTarget.TransformToDevice.Transform(pt);

            return pt;
        }

        /// <summary>
        ///     Convert a point from "above" the coordinate space of a
        ///     visual into the the coordinate space "below" the visual.
        /// </summary>
        private static Point ApplyVisualTransform(Visual v, Point pt, bool inverse)
        {
            Matrix m = GetVisualTransform(v);

            if (inverse)
            {
                m.Invert();
            }

            return m.Transform(pt);
        }

        /// <summary>
        ///     Gets the matrix that will convert a point 
        ///     from "above" the coordinate space of a visual
        ///     into the the coordinate space "below" the visual.
        /// </summary>
        private static Matrix GetVisualTransform(Visual v)
        {
            Matrix m = Matrix.Identity;

            // A visual can currently have two properties that affect
            // its coordinate space:
            //    1) Transform - any matrix
            //    2) Offset - a simpification for just a 2D offset.
            Transform transform = VisualTreeHelper.GetTransform(v);
            if (transform != null)
            {
                Matrix cm = transform.Value;
                m = Matrix.Multiply(m, cm);
            }

            Vector offset = VisualTreeHelper.GetOffset(v);
            m.Translate(offset.X, offset.Y);

            return m;
        }
    }
}
