using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.DwayneNeed.Extensions
{
    public static class RectExtensions
    {
        public static Rect ConstrainWithin(this Rect rect, Rect constraint)
        {
            // Constrain the size.
            if (rect.Width > constraint.Width)
            {
                rect.Width = constraint.Width;
            }
            if (rect.Height > constraint.Height)
            {
                rect.Height = constraint.Height;
            }

            // Constrain the position.
            if (rect.Left < constraint.Left)
            {
                rect.X = constraint.Left;
            }
            if (rect.Right > constraint.Right)
            {
                rect.X = constraint.Right - rect.Width;
            }
            if (rect.Top < constraint.Top)
            {
                rect.Y = constraint.Top;
            }
            if (rect.Bottom > constraint.Bottom)
            {
                rect.Y = constraint.Bottom - rect.Height;
            }

            return rect;
        }
    }
}
