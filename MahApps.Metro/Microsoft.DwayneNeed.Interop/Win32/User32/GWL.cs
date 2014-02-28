using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.User32
{
    /// <summary>
    ///     Window field offsets for GetWindowLong
    /// </summary>
    public enum GWL : int
    {
        WNDPROC = -4,
        HINSTANCE = -6,
        HWNDPARENT = -8,
        STYLE = -16,
        EXSTYLE = -20,
        USERDATA = -21,
        ID = -12
    }
}
