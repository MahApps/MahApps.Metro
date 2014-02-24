using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.Kernel32
{
    [Flags]
    public enum SEC : int
    {
        NONE = 0, // optional
        FILE = 0x800000,
        IMAGE = 0x1000000,
        PROTECTED_IMAGE = 0x2000000,
        RESERVE = 0x4000000,
        COMMIT = 0x8000000,
        NOCACHE = 0x10000000,
        WRITECOMBINE = 0x40000000,
        LARGE_PAGES = unchecked((int)0x80000000)
    }
}
