namespace MahApps.Metro.Native
{
    public static class Constants
    {
        public const int MONITOR_DEFAULTTONEAREST = 0x00000002;
        public const int WM_NCLBUTTONDOWN = 0x00A1;
        public const int WM_NCCALCSIZE = 0x83;
        public const int WM_NCPAINT = 0x85;
        public const int WM_NCACTIVATE = 0x86;
        public const int WM_GETMINMAXINFO = 0x24;
        public const int WM_CREATE = 0x0001;
        public const long WS_MAXIMIZE = 0x01000000;
        public const int GCLP_HBRBACKGROUND = -0x0A;
        public const int WM_NCHITTEST = 0x84;
        public const int HT_CAPTION = 0x2;
        public const int HTLEFT = 0x0A;
        public const int HTRIGHT = 0x0B;
        public const int HTTOP = 0x0C;
        public const int HTTOPLEFT = 0x0D;
        public const int HTTOPRIGHT = 0x0E;
        public const int HTBOTTOM = 0x0F;
        public const int HTBOTTOMLEFT = 0x10;
        public const int HTBOTTOMRIGHT = 0x11;
        public const uint TPM_RETURNCMD = 0x0100;
        public const uint TPM_LEFTBUTTON = 0x0;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOWMINIMIZED = 2;
        public const uint SYSCOMMAND = 0x0112;
        public const int WM_INITMENU = 0x116;

        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_RESTORE = 0xF120;
        public const int SC_MOVE = 0xF010;
        public const int MF_GRAYED = 0x00000001;
        public const int MF_BYCOMMAND = 0x00000000;
        public const int MF_ENABLED = 0x00000000;

        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOZORDER = 0x0004;
        public const uint SWP_NOREDRAW = 0x0008;
        public const uint SWP_NOACTIVATE = 0x0010;

        public const uint SWP_FRAMECHANGED = 0x0020; /* The frame changed: send WM_NCCALCSIZE */
        public const uint SWP_SHOWWINDOW = 0x0040;
        public const uint SWP_HIDEWINDOW = 0x0080;
        public const uint SWP_NOCOPYBITS = 0x0100;
        public const uint SWP_NOOWNERZORDER = 0x0200; /* Don’t do owner Z ordering */
        public const uint SWP_NOSENDCHANGING = 0x0400; /* Don’t send WM_WINDOWPOSCHANGING */

        public const int WM_MOVE = 0x0003;

        public const uint TOPMOST_FLAGS = SWP_NOACTIVATE | SWP_NOOWNERZORDER | SWP_NOSIZE | SWP_NOMOVE | SWP_NOREDRAW | SWP_NOSENDCHANGING;
    }
}
