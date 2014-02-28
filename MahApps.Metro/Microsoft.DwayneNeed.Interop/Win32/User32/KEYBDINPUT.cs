using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.User32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        public short wVk;
        public short wScan;
        public KEYEVENTF dwFlags;
        public int time;
        public IntPtr dwExtraInfo;
    } ;
}
