namespace MahApps.Metro.Native
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class MONITORINFO
    {
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

        public RECT rcMonitor;

        public RECT rcWork;

        public int dwFlags;
    }
}