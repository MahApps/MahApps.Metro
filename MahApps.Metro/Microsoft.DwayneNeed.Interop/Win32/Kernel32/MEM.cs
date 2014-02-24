using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.Kernel32
{
    public enum MEM : int
    {
        COMMIT = 0x1000,
        RESERVE = 0x2000,
        DECOMMIT = 0x4000,
        RELEASE = 0x8000,
        FREE = 0x10000,
        PRIVATE = 0x20000,
        MAPPED = 0x40000,
        RESET = 0x80000,
        TOP_DOWN = 0x100000,
        WRITE_WATCH = 0x200000,
        PHYSICAL = 0x400000,
        ROTATE = 0x800000,
        IMAGE = 0x1000000,
        LARGE_PAGES = 0x20000000,
        FOUR_MB_PAGES = unchecked((int)0x80000000) // slight rename because it can't start with a digit
    }
}
