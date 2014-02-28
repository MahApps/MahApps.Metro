using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Microsoft.DwayneNeed.Win32.User32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public int width { get { return right - left; } }
        public int height { get { return bottom - top; } }

        public override string ToString()
        {
            return "(" + left + ", " + top + "), (" + width + " x " + height + ")";
        }
    }
}
