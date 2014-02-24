using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.User32
{
    [Flags]
    public enum DT : int
    {
        TOP = 0x00000000,
        LEFT = 0x00000000,
        CENTER = 0x00000001,
        RIGHT = 0x00000002,
        VCENTER = 0x00000004,
        BOTTOM = 0x00000008,
        WORDBREAK = 0x00000010,
        SINGLELINE = 0x00000020,
        EXPANDTABS = 0x00000040,
        TABSTOP = 0x00000080,
        NOCLIP = 0x00000100,
        EXTERNALLEADING = 0x00000200,
        CALCRECT = 0x00000400,
        NOPREFIX = 0x00000800,
        INTERNAL = 0x00001000
    }
}
