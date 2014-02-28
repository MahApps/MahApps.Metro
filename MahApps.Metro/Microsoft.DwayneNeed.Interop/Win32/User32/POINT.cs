using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.DwayneNeed.Win32;

namespace Microsoft.DwayneNeed.Win32.User32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static POINT FromParam(uint param)
        {
            int x = NativeMacros.GET_X_LPARAM(param);
            int y = NativeMacros.GET_Y_LPARAM(param);

            return new POINT(x, y);
        }

        public uint ToParam()
        {
            uint param_x = unchecked((ushort)(short)x);
            uint param_y = unchecked((ushort)(short)y);

            return (param_y << 16) | param_x;
        }

        public int x;
        public int y;
    }
}
