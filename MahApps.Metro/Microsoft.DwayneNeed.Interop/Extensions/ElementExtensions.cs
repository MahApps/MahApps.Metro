using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using Microsoft.DwayneNeed.Extensions;

namespace Microsoft.DwayneNeed.Extensions
{
    public static class ElementExtensions
    {
        /// <summary>
        /// WPF will use software rendering if the machine is not capable of
        /// hardware rendering, or if the render mode on the HwndTarget
        /// specifies SoftwareOnly.  WPF may choose software rendering at other
        /// times too, such as when a recoverable error happens on the render
        /// thread.  Note that we can only detect some situations.
        /// </summary>
        public static bool IsSoftwareRendering(this UIElement @this)
        {
            bool isSoftwareRendering = false;

            HwndSource hwndSource = PresentationSource.FromVisual(@this) as HwndSource;
            if (hwndSource != null)
            {
                HwndTarget hwndTarget = (HwndTarget)hwndSource.CompositionTarget;

                isSoftwareRendering = hwndTarget.RenderMode == RenderMode.SoftwareOnly ||
                                      (System.Windows.Media.RenderCapability.Tier >> 16) == 0;
            }

            return isSoftwareRendering;
        }

        public static T FindAncestor<T>(this UIElement @this) where T : UIElement
        {
            DependencyObject e = @this;

            do
            {
                DependencyObject p = VisualTreeHelper.GetParent(e);
                if (p == null && e is FrameworkElement)
                {
                    p = ((FrameworkElement)e).Parent;
                }
                e = p;
            } while (!(e is T) && e != null);

            return (T)e;
        }

        public static Rect TransformElementToElement(this UIElement @this, Rect rect, UIElement target)
        {
            // Find the HwndSource for this element and use it to transform
            // the rectangle up into screen coordinates.
            HwndSource hwndSource = (HwndSource)PresentationSource.FromVisual(@this);
            rect = hwndSource.TransformDescendantToClient(rect, @this);
            rect = hwndSource.TransformClientToScreen(rect);

            // Find the HwndSource for the target element and use it to
            // transform the rectangle from screen coordinates down to the
            // target elemnent.
            HwndSource targetHwndSource = (HwndSource)PresentationSource.FromVisual(target);
            rect = targetHwndSource.TransformScreenToClient(rect);
            rect = targetHwndSource.TransformClientToDescendant(rect, target);

            return rect;
        }

        public static Point TransformElementToElement(this UIElement @this, Point pt, UIElement target)
        {
            // Find the HwndSource for this element and use it to transform
            // the point up into screen coordinates.
            HwndSource hwndSource = (HwndSource)PresentationSource.FromVisual(@this);
            pt = hwndSource.TransformDescendantToClient(pt, @this);
            pt = hwndSource.TransformClientToScreen(pt);

            // Find the HwndSource for the target element and use it to
            // transform the rectangle from screen coordinates down to the
            // target elemnent.
            HwndSource targetHwndSource = (HwndSource)PresentationSource.FromVisual(target);
            pt = targetHwndSource.TransformScreenToClient(pt);
            pt = targetHwndSource.TransformClientToDescendant(pt, target);

            return pt;
        }

        public static Vector TransformElementToElement(this UIElement @this, Vector v, UIElement target)
        {
            Point ptOrigin = TransformElementToElement(@this, new Point(0, 0), target);
            Point ptDelta = TransformElementToElement(@this, new Point(v.X, v.Y), target);
            return new Vector(ptDelta.X - ptOrigin.X, ptDelta.Y - ptOrigin.Y);
        }

        public static void DisposeSubTree(this UIElement @this)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(@this);
            for (int iChild = 0; iChild < childrenCount; iChild++)
            {
                UIElement child = VisualTreeHelper.GetChild(@this, iChild) as UIElement;
                if (child != null)
                {
                    if (child is IDisposable)
                    {
                        ((IDisposable)child).Dispose();

                        // Don't descend into the visual tree of an element we
                        // just disposed.  We rely on the element to properly
                        // dispose its content.
                    }
                    else
                    {
                        DisposeSubTree(child);
                    }
                }
            }
        }
    }
}
