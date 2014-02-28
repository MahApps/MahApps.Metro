using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.User32
{
    /// <summary>
    ///     The "real" ancestor window
    /// </summary>
    public enum GA : int
    {
        PARENT = 1,
        ROOT = 2,
        ROOTOWNER = 3
    }
}
