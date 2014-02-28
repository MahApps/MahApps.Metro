using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.User32
{
    [Flags]
    public enum MOUSEEVENTF
    {
        /// <summary>
        ///     The MOUSEINPUT dx and dy members contain normalized absolute
        ///     coordinates. If the flag is not set, dx and dy contain relative
        ///     data (the change in position since the last reported position).
        ///     This flag can be set, or not set, regardless of what kind of
        ///     mouse or other pointing device, if any, is connected to the
        ///     system. 
        /// </summary>
        ABSOLUTE = 0x8000,
 
        /// <summary>
        ///     The wheel was moved horizontally, if the mouse has a wheel. The
        ///     amount of movement is specified in mouseData. 
        ///     Windows XP/2000: This value is not supported.
        /// </summary>
        HWHEEL = 0x01000,
 
        /// <summary>
        ///     Movement occurred.
        /// </summary>
        MOVE = 0x0001,
 
        /// <summary>
        ///     The WM_MOUSEMOVE messages will not be coalesced. The default
        ///     behavior is to coalesce WM_MOUSEMOVE messages. 
        ///     Windows XP/2000: This value is not supported.
        /// </summary>
        MOVE_NOCOALESCE = 0x2000,
 
        /// <summary>
        ///     The left button was pressed.
        /// </summary>
        LEFTDOWN= 0x0002, 
 
        /// <summary>
        ///     The left button was released.
        /// </summary>
        LEFTUP = 0x0004,
 
        /// <summary>
        ///     The right button was pressed.
        /// </summary>
        RIGHTDOWN = 0x0008,
 
        /// <summary>
        ///     The right button was released.
        /// </summary>
        RIGHTUP = 0x0010,
 
        /// <summary>
        ///     The middle button was pressed.
        /// </summary>
        MIDDLEDOWN = 0x0020,
 
        /// <summary>
        ///     The middle button was released.
        /// </summary>
        MIDDLEUP = 0x0040,
 
        /// <summary>
        ///     Maps coordinates to the entire desktop. Must be used with MOUSEEVENTF_ABSOLUTE.
        /// </summary>
        VIRTUALDESK = 0x4000,
 
        /// <summary>
        ///     The wheel was moved, if the mouse has a wheel. The amount of movement is specified in mouseData. 
        /// </summary>
        WHEEL = 0x0800,
 
        /// <summary>
        ///     An X button was pressed.
        /// </summary>
        XDOWN = 0x0080,

        /// <summary>
        ///     An X button was released.
        /// </summary>
        XUP = 0x0100,
    }
}
