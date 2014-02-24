using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Microsoft.DwayneNeed.Win32.Gdi32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFO
    {
        public Int32 biSize;
        public Int32 biWidth;
        public Int32 biHeight;
        public Int16 biPlanes;
        public Int16 biBitCount;
        public BI biCompression;
        public Int32 biSizeImage;
        public Int32 biXPelsPerMeter;
        public Int32 biYPelsPerMeter;
        public Int32 biClrUsed;
        public Int32 biClrImportant;
    }
}
