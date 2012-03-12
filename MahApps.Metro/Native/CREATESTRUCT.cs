namespace MahApps.Metro.Native
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct CREATESTRUCT
    {
        public IntPtr lpCreateParams;

        public IntPtr hInstance;

        public IntPtr hMenu;

        public IntPtr hwndParent;

        public int cy;

        public int cx;

        public int y;

        public int x;

        public int style;

        public string lpszName;

        public string lpszClass;

        public int dwExStyle;
    }
}