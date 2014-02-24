using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.User32
{
    /// <summary>
    ///     Class field offsets for GetClassLong
    /// </summary>
    public enum GCL : int
    {
        MENUNAME = -8,
        HBRBACKGROUND = -10,
        HCURSOR = -12,
        HICON = -14,
        HMODULE = -16,
        CBWNDEXTRA = -18,
        CBCLSEXTRA = -20,
        WNDPROC = -24,
        STYLE = -26,
        HICONSM = -34
    }
}
