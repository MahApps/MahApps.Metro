using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.User32
{
    public enum RDW : int
    {
        INVALIDATE = 0x0001,
        INTERNALPAINT = 0x0002,
        ERASE = 0x0004,
        VALIDATE = 0x0008,
        NOINTERNALPAINT = 0x0010,
        NOERASE = 0x0020,
        NOCHILDREN = 0x0040,
        ALLCHILDREN = 0x0080,
        UPDATENOW = 0x0100,
        ERASENOW = 0x0200,
        FRAME = 0x0400,
        NOFRAME = 0x0800
    }
}
