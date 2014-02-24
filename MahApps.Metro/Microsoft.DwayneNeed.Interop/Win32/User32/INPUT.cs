using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.User32
{
    // TODO: User32 names this enum type INPUT, which conflicts with the struct INPUT...
    public enum INPUTTYPE : int
    {
        MOUSE = 0,
        KEYBOARD = 1,
        HARDWARE = 2
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct INPUT
    {
        [FieldOffset(0)]
        public INPUTTYPE type;

        [FieldOffset(4)]
        public MOUSEINPUT mouseInput;

        [FieldOffset(4)]
        public KEYBDINPUT keyboardInput;

        [FieldOffset(4)]
        public HARDWAREINPUT hardwareInput;
    } ;
}
