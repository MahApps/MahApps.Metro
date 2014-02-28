using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.DwayneNeed.Extensions
{
    public static class Int32RectExtensions
    {
        public static Int32Rect Union(this Int32Rect rect1, Int32Rect rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2;
            }
            else if (rect2.IsEmpty)
            {
                return rect1;
            }
            else
            {
                int left = rect1.X < rect2.X ? rect1.X : rect2.X;
                int top = rect1.Y < rect2.Y ? rect1.Y : rect2.Y;
                int right = (rect1.X + rect1.Width) > (rect2.X + rect2.Width) ? (rect1.X + rect1.Width) : (rect2.X + rect2.Width);
                int bottom = (rect1.Y + rect1.Height) > (rect2.Y + rect2.Height) ? (rect1.Y + rect1.Height) : (rect2.Y + rect2.Height);

                return new Int32Rect(left, top, right - left, bottom - top);
            }
        }

        public static int GetArea(this Int32Rect rect)
        {
            if (rect.IsEmpty)
            {
                return 0;
            }
            else
            {
                return rect.Width * rect.Height;
            }
        }
    }
}
