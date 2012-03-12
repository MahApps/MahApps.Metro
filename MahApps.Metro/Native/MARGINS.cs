namespace MahApps.Metro.Native
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public int leftWidth;

        public int rightWidth;

        public int topHeight;

        public int bottomHeight;
    }
}