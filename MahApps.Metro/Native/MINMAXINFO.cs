namespace MahApps.Metro.Native
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public POINT ptReserved;

        public POINT ptMaxSize;

        public POINT ptMaxPosition;

        public POINT ptMinTrackSize;

        public POINT ptMaxTrackSize;
    };
}