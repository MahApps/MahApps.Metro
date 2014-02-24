using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.DwayneNeed.Win32.Gdi32
{
    /// <summary>
    /// The type of a GDI object.
    /// </summary>
    public enum OBJ : int
    {
        PEN = 1,
        BRUSH = 2,
        DC = 3,
        METADC = 4,
        PAL = 5,
        FONT = 6,
        BITMAP = 7,
        REGION = 8,
        METAFILE = 9,
        MEMDC = 10,
        EXTPEN = 11,
        ENHMETADC = 12,
        ENHMETAFILE = 13,
        COLORSPACE = 14
    }
}
