using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.Gdi32
{
    /// <summary>
    ///     Constants for the BITMAPINFO biCompression field
    /// </summary>
    public enum BI : int
    {
        RGB = 0,
        RLE8 = 1,
        RLE4 = 2,
        BITFIELDS = 3,
        JPEG = 4,
        PNG = 5
    }
}
