using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.User32
{
    [Flags]
    public enum KEYEVENTF
    {
        /// <summary>
        ///     If specified, the scan code was preceded by a prefix byte that
        ///     has the value 0xE0 (224).
        /// </summary>
        EXTENDEDKEY = 0x0001,

        /// <summary>
        ///     If specified, the key is being released. If not specified, the
        ///     key is being pressed.
        /// </summary>
        KEYUP = 0x0002,

        /// <summary>
        ///     If specified, wScan identifies the key and wVk is ignored. 
        /// </summary>
        SCANCODE = 0x0008,

        /// <summary>
        ///     If specified, the system synthesizes a VK_PACKET keystroke. The
        ///     wVk parameter must be zero. This flag can only be combined with
        ///     the KEYUP flag.
        /// </summary>  
        UNICODE = 0x0004
    }
}
