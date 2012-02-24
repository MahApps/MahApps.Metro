using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Interop;

namespace MahApps.Metro.Behaviours
{
    /// <summary>
    /// http://gallery.expression.microsoft.com/ZuneWindowBehavior/
    /// Published: 10/18/2010
    /// Created by: jmorrill
    /// Supporting Url: http://jmorrill.hjtcentral.com
    /// Tags: Behavior, WPF, Zune
    /// License Information: 
    /// This contribution is licensed to you under Creative Commons by its owner, not Microsoft. Microsoft does not guarantee the contribution or purport to grant rights to it.
    /// </summary>
    /// 
    public class BorderlessWindowBehavior : Behavior<Window>
    {
        #region NativeStuffs
        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        /// <summary>
        /// POINT aka POINTAPI
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>
            /// x coordinate of point.
            /// </summary>
            public int x;
            /// <summary>
            /// y coordinate of point.
            /// </summary>
            public int y;

            /// <summary>
            /// Construct a point of coordinates (x,y).
            /// </summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            /// <summary>
            /// </summary>            
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            /// <summary>
            /// </summary>            
            public RECT rcMonitor = new RECT();

            /// <summary>
            /// </summary>            
            public RECT rcWork = new RECT();

            /// <summary>
            /// </summary>            
            public int dwFlags = 0;
        }


        /// <summary> Win32 </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            /// <summary> Win32 </summary>
            public int left;
            /// <summary> Win32 </summary>
            public int top;
            /// <summary> Win32 </summary>
            public int right;
            /// <summary> Win32 </summary>
            public int bottom;

            /// <summary> Win32 </summary>
            public static readonly RECT Empty = new RECT();

            /// <summary> Win32 </summary>
            public int Width
            {
                get { return Math.Abs(right - left); }  // Abs needed for BIDI OS
            }
            /// <summary> Win32 </summary>
            public int Height
            {
                get { return bottom - top; }
            }

            /// <summary> Win32 </summary>
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }


            /// <summary> Win32 </summary>
            public RECT(RECT rcSrc)
            {
                this.left = rcSrc.left;
                this.top = rcSrc.top;
                this.right = rcSrc.right;
                this.bottom = rcSrc.bottom;
            }

            /// <summary> Win32 </summary>
            public bool IsEmpty
            {
                get
                {
                    // BUGBUG : On Bidi OS (hebrew arabic) left > right
                    return left >= right || top >= bottom;
                }
            }
            /// <summary> Return a user friendly representation of this struct </summary>
            public override string ToString()
            {
                if (this == RECT.Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
            public override bool Equals(object obj)
            {
                if (!(obj is Rect)) { return false; }
                return (this == (RECT)obj);
            }

            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode()
            {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }


            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
            }

            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }


        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        /// <summary>
        /// 
        /// </summary>
        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {

            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            System.IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != System.IntPtr.Zero)
            {

                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        #endregion

        private const int WM_NCCALCSIZE = 0x83;
        private const int WM_NCPAINT = 0x85;
        private const int WM_NCACTIVATE = 0x86;
        private const int WM_GETMINMAXINFO = 0x24;
        private const int WM_CREATE = 0x1;

        private HwndSource m_hwndSource;
        private IntPtr m_hwnd;

        public static DependencyProperty ResizeWithGripProperty = DependencyProperty.Register("ResizeWithGrip", typeof(bool), typeof(BorderlessWindowBehavior), new PropertyMetadata(true));
        public static DependencyProperty AutoSizeToContentProperty = DependencyProperty.Register("AutoSizeToContent", typeof(bool), typeof(BorderlessWindowBehavior), new PropertyMetadata(false));

        public bool ResizeWithGrip
        {
            get { return (bool)GetValue(ResizeWithGripProperty); }
            set { SetValue(ResizeWithGripProperty, value); }
        }

        public bool AutoSizeToContent
        {
            get { return (bool)GetValue(AutoSizeToContentProperty); }
            set { SetValue(AutoSizeToContentProperty, value); }
        }


        protected override void OnAttached()
        {
            if (HwndSource.FromVisual(AssociatedObject) != null)
                AddHwndHook();
            else
                AssociatedObject.SourceInitialized += AssociatedObject_SourceInitialized;

            AssociatedObject.WindowStyle = WindowStyle.None;
            AssociatedObject.ResizeMode = ResizeWithGrip ? ResizeMode.CanResizeWithGrip : ResizeMode.CanResize;

            if (AutoSizeToContent)
                AssociatedObject.Loaded += (s, e) =>
                                               {
                                                   //Temp fix, thanks @lynnx
                                                   AssociatedObject.SizeToContent = SizeToContent.Height;
                                                   AssociatedObject.SizeToContent = AutoSizeToContent
                                                                                        ? SizeToContent.WidthAndHeight
                                                                                        : SizeToContent.Manual;
                                               };

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            RemoveHwndHook();
            base.OnDetaching();
        }

        private void AddHwndHook()
        {
            m_hwndSource = HwndSource.FromVisual(AssociatedObject) as HwndSource;
            m_hwndSource.AddHook(HwndHook);
            m_hwnd = new WindowInteropHelper(AssociatedObject).Handle;
        }

        private void RemoveHwndHook()
        {
            AssociatedObject.SourceInitialized -= AssociatedObject_SourceInitialized;
            m_hwndSource.RemoveHook(HwndHook);
        }

        private void AssociatedObject_SourceInitialized(object sender, EventArgs e)
        {
            AddHwndHook();
        }

        private IntPtr HwndHook(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            IntPtr returnval = IntPtr.Zero;
            switch (message)
            {
                case WM_CREATE:
                    handled = true;
                    break;

                case WM_NCCALCSIZE:
                    /* Hides the border */
                    handled = true;
                    break;
                case WM_NCPAINT:
                    {
                        if (Environment.OSVersion.Version.Major >= 6)
                        {
                            var m = new MARGINS { bottomHeight = 1, leftWidth = 1, rightWidth = 1, topHeight = 1 };
                            DwmExtendFrameIntoClientArea(m_hwnd, ref m);
                        }
                        handled = true;
                    }
                    break;
                case WM_NCACTIVATE:
                    {
                        /* As per http://msdn.microsoft.com/en-us/library/ms632633(VS.85).aspx , "-1" lParam
                         * "does not repaint the nonclient area to reflect the state change." */
                        returnval = DefWindowProc(hWnd, message, wParam, new IntPtr(-1));
                        handled = true;
                    }
                    break;
                case WM_GETMINMAXINFO:
                    /* From Lester's Blog (thanks @aeoth):  
                     * http://blogs.msdn.com/b/llobo/archive/2006/08/01/maximizing-window-_2800_with-windowstyle_3d00_none_2900_-considering-taskbar.aspx */
                    WmGetMinMaxInfo(hWnd, lParam);
                    
                    /* Setting handled to false enables the application to process iz's own Min/Max requirements,
                     * as mentioned by jason.bullard (comment from September 22, 2011) on http://gallery.expression.microsoft.com/ZuneWindowBehavior/ */
                    handled = false;
                    break;
            }

            return returnval;
        }
    }
}
