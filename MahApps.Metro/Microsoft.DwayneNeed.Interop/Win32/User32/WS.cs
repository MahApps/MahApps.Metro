using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.User32
{
    /// <summary>
    ///     Window Styles
    /// </summary>
    [Flags]
    public enum WS : int
    {
        OVERLAPPED = 00000000,
        POPUP = unchecked((int)0x80000000),
        CHILD = 0x40000000,
        MINIMIZE = 0x20000000,
        VISIBLE = 0x10000000,
        DISABLED = 0x08000000,
        CLIPSIBLINGS = 0x04000000,
        CLIPCHILDREN = 0x02000000,
        MAXIMIZE = 0x01000000,
        CAPTION = BORDER | DLGFRAME,
        BORDER = 0x00800000,
        DLGFRAME = 0x00400000,
        VSCROLL = 0x00200000,
        HSCROLL = 0x00100000,
        SYSMENU = 0x00080000,
        THICKFRAME = 0x00040000,
        GROUP = 0x00020000,
        TABSTOP = 0x00010000,
        MINIMIZEBOX = 0x00020000,
        MAXIMIZEBOX = 0x00010000,
        TILED = OVERLAPPED,
        ICONIC = MINIMIZE,
        SIZEBOX = THICKFRAME,
        TILEDWINDOW = OVERLAPPEDWINDOW,
        OVERLAPPEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX,
        POPUPWINDOW = POPUP | BORDER | SYSMENU,
        CHILDWINDOW = CHILD
    }
}
