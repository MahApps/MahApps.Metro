using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Interop;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Native;
#if NET_4
using Microsoft.Windows.Shell;
#else
using System.Windows.Shell;
#endif

namespace MahApps.Metro.Behaviours
{
    public class BorderlessWindowBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty ResizeWithGripProperty = DependencyProperty.Register("ResizeWithGrip", typeof(bool), typeof(BorderlessWindowBehavior), new PropertyMetadata(true));
        public static readonly DependencyProperty AutoSizeToContentProperty = DependencyProperty.Register("AutoSizeToContent", typeof(bool), typeof(BorderlessWindowBehavior), new PropertyMetadata(false));
        public static readonly DependencyProperty EnableDWMDropShadowProperty = DependencyProperty.Register("EnableDWMDropShadow", typeof(bool), typeof(BorderlessWindowBehavior), new PropertyMetadata(false, new PropertyChangedCallback((obj, args) =>
        {
            var behaviorClass = ((BorderlessWindowBehavior)obj);

            if (behaviorClass.AssociatedObject != null)
                if ((bool)args.NewValue && behaviorClass.AssociatedObject.AllowsTransparency)
                    throw new InvalidOperationException("EnableDWMDropShadow cannot be set to True when AllowsTransparency is True.");
        })));

        public static readonly DependencyProperty AllowsTransparencyProperty =
            DependencyProperty.Register("AllowsTransparency", typeof(bool), typeof(BorderlessWindowBehavior), new PropertyMetadata(true, new PropertyChangedCallback((obj, args) =>
            {
                var behaviorClass = ((BorderlessWindowBehavior)obj);

                if (behaviorClass.AssociatedObject != null)
                {
                    if ((bool)args.NewValue && behaviorClass.EnableDWMDropShadow)
                        throw new InvalidOperationException("AllowsTransparency cannot be set to True when EnableDWMDropShadow is True.");
                    else
                        behaviorClass.AssociatedObject.AllowsTransparency = (bool)args.NewValue;
                }
            })));

        public bool AllowsTransparency
        {
            get { return (bool)GetValue(AllowsTransparencyProperty); }
            set { SetValue(AllowsTransparencyProperty, value); }
        }

        public bool EnableDWMDropShadow
        {
            get { return (bool)GetValue(EnableDWMDropShadowProperty); }
            set { SetValue(EnableDWMDropShadowProperty, value); }
        }

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

        public Border Border { get; set; }

        private HwndSource _mHWNDSource;
        private IntPtr _mHWND;

        private static IntPtr SetClassLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size > 4)
                return UnsafeNativeMethods.SetClassLongPtr64(hWnd, nIndex, dwNewLong);

            return new IntPtr(UnsafeNativeMethods.SetClassLongPtr32(hWnd, nIndex, unchecked((uint)dwNewLong.ToInt32())));
        }

        private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            IntPtr monitor = UnsafeNativeMethods.MonitorFromWindow(hwnd, Constants.MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                UnsafeNativeMethods.GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.X = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);

                bool ignoreTaskBar = AssociatedObject as MetroWindow != null && ((MetroWindow)this.AssociatedObject).IgnoreTaskbarOnMaximize;

                if (!ignoreTaskBar)
                {
                    mmi.ptMaxTrackSize.X = mmi.ptMaxSize.X;
                    mmi.ptMaxTrackSize.Y = mmi.ptMaxSize.Y;
                    mmi = AdjustWorkingAreaForAutoHide(monitor, mmi);
                }
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        protected override void OnAttached()
        {
            if (PresentationSource.FromVisual(AssociatedObject) != null)
                AddHwndHook();
            else
                AssociatedObject.SourceInitialized += AssociatedObject_SourceInitialized;

            if (AllowsTransparency && EnableDWMDropShadow)
                throw new InvalidOperationException("EnableDWMDropShadow cannot be set to True when AllowsTransparency is True.");

            AssociatedObject.WindowStyle = WindowStyle.None;
            AssociatedObject.AllowsTransparency = AllowsTransparency;
            AssociatedObject.StateChanged += AssociatedObjectStateChanged;
            AssociatedObject.SetValue(WindowChrome.GlassFrameThicknessProperty, new Thickness(-1));

            if (AssociatedObject is MetroWindow)
            {
                var window = ((MetroWindow)AssociatedObject);
                //MetroWindow already has a border we can use
                AssociatedObject.Loaded += (s, e) =>
                {
                    var ancestors = window.GetPart<Border>("PART_Border");
                    Border = ancestors;
                    if (ShouldHaveBorder())
                        AddBorder();
                    var titleBar = window.GetPart<Grid>("PART_TitleBar");
                    titleBar.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, true);
                    var windowCommands = window.GetPart<ContentPresenter>("PART_WindowCommands");
                    windowCommands.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, true);
                };

                switch (AssociatedObject.ResizeMode)
                {
                    case ResizeMode.NoResize:
                        window.ShowMaxRestoreButton = false;
                        window.ShowMinButton = false;
                        ResizeWithGrip = false;
                        break;
                    case ResizeMode.CanMinimize:
                        window.ShowMaxRestoreButton = false;
                        ResizeWithGrip = false;
                        break;
                    case ResizeMode.CanResize:
                        ResizeWithGrip = false;
                        break;
                    case ResizeMode.CanResizeWithGrip:
                        ResizeWithGrip = true;
                        break;
                }
            }
            else
            {
                //Other windows may not, easiest to just inject one!
                var content = (UIElement)AssociatedObject.Content;
                AssociatedObject.Content = null;

                Border = new Border
                {
                    Child = content,
                    BorderBrush = new SolidColorBrush(Colors.Black)
                };

                AssociatedObject.Content = Border;
            }

            if (ResizeWithGrip)
                AssociatedObject.ResizeMode = ResizeMode.CanResizeWithGrip;

            if (AutoSizeToContent)
                AssociatedObject.Loaded += (s, e) =>
                {
                    //Temp fix, thanks @lynnx
                    AssociatedObject.SizeToContent = SizeToContent.Height;
                    AssociatedObject.SizeToContent = AutoSizeToContent ? 
                        SizeToContent.WidthAndHeight : SizeToContent.Manual;
                };

            base.OnAttached();
        }

        private void AssociatedObjectStateChanged(object sender, EventArgs e)
        {
            if (AssociatedObject.WindowState == WindowState.Maximized)
            {
                HandleMaximize();
            }
        }

        private void HandleMaximize()
        {
            IntPtr monitor = UnsafeNativeMethods.MonitorFromWindow(_mHWND, Constants.MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                UnsafeNativeMethods.GetMonitorInfo(monitor, monitorInfo);
                bool ignoreTaskBar = AssociatedObject as MetroWindow != null && ((MetroWindow)this.AssociatedObject).IgnoreTaskbarOnMaximize;
                var x = ignoreTaskBar ? monitorInfo.rcMonitor.left : monitorInfo.rcWork.left;
                var y = ignoreTaskBar ? monitorInfo.rcMonitor.top : monitorInfo.rcWork.top;
                var cx = ignoreTaskBar ? Math.Abs(monitorInfo.rcMonitor.right - x) : Math.Abs(monitorInfo.rcWork.right - x);
                var cy = ignoreTaskBar ? Math.Abs(monitorInfo.rcMonitor.bottom - y) : Math.Abs(monitorInfo.rcWork.bottom - y);
                UnsafeNativeMethods.SetWindowPos(_mHWND, new IntPtr(-2), x, y, cx, cy, 0x0040);
            }
        }

        private static int GetEdge(RECT rc)
        {
            int uEdge;
            if (rc.top == rc.left && rc.bottom > rc.right)
                uEdge = (int)ABEdge.ABE_LEFT;
            else if (rc.top == rc.left && rc.bottom < rc.right)
                uEdge = (int)ABEdge.ABE_TOP;
            else if (rc.top > rc.left)
                uEdge = (int)ABEdge.ABE_BOTTOM;
            else
                uEdge = (int)ABEdge.ABE_RIGHT;
            return uEdge;
        }

        /// <summary>
        /// This method handles the window size if the taskbar is set to auto-hide.
        /// </summary>
        private static MINMAXINFO AdjustWorkingAreaForAutoHide(IntPtr monitorContainingApplication, MINMAXINFO mmi)
        {
            IntPtr hwnd = UnsafeNativeMethods.FindWindow("Shell_TrayWnd", null);
            IntPtr monitorWithTaskbarOnIt = UnsafeNativeMethods.MonitorFromWindow(hwnd, Constants.MONITOR_DEFAULTTONEAREST);

            if (!monitorContainingApplication.Equals(monitorWithTaskbarOnIt))
                return mmi;

            var abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = hwnd;
            UnsafeNativeMethods.SHAppBarMessage((int)ABMsg.ABM_GETTASKBARPOS, ref abd);
            int uEdge = GetEdge(abd.rc);
            bool autoHide = Convert.ToBoolean(UnsafeNativeMethods.SHAppBarMessage((int)ABMsg.ABM_GETSTATE, ref abd));

            if (!autoHide)
                return mmi;

            switch (uEdge)
            {
                case (int)ABEdge.ABE_LEFT:
                    mmi.ptMaxPosition.X += 2;
                    mmi.ptMaxTrackSize.X -= 2;
                    mmi.ptMaxSize.X -= 2;
                    break;
                case (int)ABEdge.ABE_RIGHT:
                    mmi.ptMaxSize.X -= 2;
                    mmi.ptMaxTrackSize.X -= 2;
                    break;
                case (int)ABEdge.ABE_TOP:
                    mmi.ptMaxPosition.Y += 2;
                    mmi.ptMaxTrackSize.Y -= 2;
                    mmi.ptMaxSize.Y -= 2;
                    break;
                case (int)ABEdge.ABE_BOTTOM:
                    mmi.ptMaxSize.Y -= 2;
                    mmi.ptMaxTrackSize.Y -= 2;
                    break;
                default:
                    return mmi;
            }
            return mmi;
        }

        protected override void OnDetaching()
        {
            RemoveHwndHook();
            base.OnDetaching();
        }

        private void AddHwndHook()
        {
            _mHWNDSource = PresentationSource.FromVisual(AssociatedObject) as HwndSource;
            if (_mHWNDSource != null)
                _mHWNDSource.AddHook(HwndHook);

            _mHWND = new WindowInteropHelper(AssociatedObject).Handle;
        }

        private void RemoveHwndHook()
        {
            AssociatedObject.SourceInitialized -= AssociatedObject_SourceInitialized;
            _mHWNDSource.RemoveHook(HwndHook);
        }

        private void AssociatedObject_SourceInitialized(object sender, EventArgs e)
        {
            AddHwndHook();
            SetDefaultBackgroundColor();
        }

        private bool ShouldHaveBorder()
        {
            if (Environment.OSVersion.Version.Major < 6)
                return true;

            return !UnsafeNativeMethods.DwmIsCompositionEnabled();
        }

        readonly SolidColorBrush _borderColour = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080"));

        private void AddBorder()
        {
            if (Border == null)
                return;

            Border.BorderThickness = new Thickness(1);
            Border.BorderBrush = _borderColour;
        }

        private void RemoveBorder()
        {
            if (Border == null)
                return;

            Border.BorderThickness = new Thickness(0);
            Border.BorderBrush = null;
        }

        private void SetDefaultBackgroundColor()
        {
            var bgSolidColorBrush = AssociatedObject.Background as SolidColorBrush;

            if (bgSolidColorBrush != null)
            {
                var rgb = bgSolidColorBrush.Color.R | (bgSolidColorBrush.Color.G << 8) | (bgSolidColorBrush.Color.B << 16);

                // set the default background color of the window -> this avoids the black stripes when resizing
                var hBrushOld = SetClassLong(_mHWND, Constants.GCLP_HBRBACKGROUND, UnsafeNativeMethods.CreateSolidBrush(rgb));

                if (hBrushOld != IntPtr.Zero)
                    UnsafeNativeMethods.DeleteObject(hBrushOld);
            }
        }

        private IntPtr HwndHook(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            IntPtr returnval = IntPtr.Zero;
            switch (message)
            {
                case Constants.WM_NCCALCSIZE:
                    /* Hides the border */
                    handled = true;
                    break;
                case Constants.WM_NCPAINT:
                    {
                        if (ShouldHaveBorder())
                        {
                            this.AddBorder();
                        }
                        else if (EnableDWMDropShadow)
                        {
                            var metroWindow = AssociatedObject as MetroWindow;
                            if (!(metroWindow != null && metroWindow.GlowBrush != null))
                            {
                                var val = 2;
                                UnsafeNativeMethods.DwmSetWindowAttribute(_mHWND, 2, ref val, 4);
                                var m = new MARGINS { bottomHeight = 1, leftWidth = 1, rightWidth = 1, topHeight = 1 };
                                UnsafeNativeMethods.DwmExtendFrameIntoClientArea(_mHWND, ref m);
                            }
                        }

                        handled = true;
                    }
                    break;
                case Constants.WM_NCACTIVATE:
                    {
                        /* As per http://msdn.microsoft.com/en-us/library/ms632633(VS.85).aspx , "-1" lParam
                         * "does not repaint the nonclient area to reflect the state change." */
                        returnval = UnsafeNativeMethods.DefWindowProc(hWnd, message, wParam, new IntPtr(-1));

                        MetroWindow w = AssociatedObject as MetroWindow;
                        if ((w != null && w.GlowBrush != null) || ShouldHaveBorder())
                        {
                            if (wParam == IntPtr.Zero)
                                AddBorder();
                            else
                                RemoveBorder();
                        }

                        handled = true;
                    }
                    break;
                case Constants.WM_GETMINMAXINFO:
                    /* http://blogs.msdn.com/b/llobo/archive/2006/08/01/maximizing-window-_2800_with-windowstyle_3d00_none_2900_-considering-taskbar.aspx */
                    WmGetMinMaxInfo(hWnd, lParam);

                    /* Setting handled to false enables the application to process it's own Min/Max requirements,
                     * as mentioned by jason.bullard (comment from September 22, 2011) on http://gallery.expression.microsoft.com/ZuneWindowBehavior/ */
                    handled = false;
                    break;
                case Constants.WM_NCHITTEST:

                    // don't process the message on windows that can't be resized
                    var resizeMode = AssociatedObject.ResizeMode;
                    if (resizeMode == ResizeMode.CanMinimize || resizeMode == ResizeMode.NoResize || AssociatedObject.WindowState == WindowState.Maximized)
                        break;

                    // get X & Y out of the message                   
                    var screenPoint = new Point(UnsafeNativeMethods.GET_X_LPARAM(lParam), UnsafeNativeMethods.GET_Y_LPARAM(lParam));

                    // convert to window coordinates
                    var windowPoint = AssociatedObject.PointFromScreen(screenPoint);
                    var windowSize = AssociatedObject.RenderSize;
                    var windowRect = new Rect(windowSize);
                    windowRect.Inflate(-6, -6);

                    // don't process the message if the mouse is outside the 6px resize border
                    if (windowRect.Contains(windowPoint))
                        break;

                    var windowHeight = (int)windowSize.Height;
                    var windowWidth = (int)windowSize.Width;

                    // create the rectangles where resize arrows are shown
                    var topLeft = new Rect(0, 0, 6, 6);
                    var top = new Rect(6, 0, windowWidth - 12, 6);
                    var topRight = new Rect(windowWidth - 6, 0, 6, 6);

                    var left = new Rect(0, 6, 6, windowHeight - 12);
                    var right = new Rect(windowWidth - 6, 6, 6, windowHeight - 12);

                    var bottomLeft = new Rect(0, windowHeight - 6, 6, 6);
                    var bottom = new Rect(6, windowHeight - 6, windowWidth - 12, 6);
                    var bottomRight = new Rect(windowWidth - 6, windowHeight - 6, 6, 6);

                    // check if the mouse is within one of the rectangles
                    if (topLeft.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTTOPLEFT;
                    else if (top.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTTOP;
                    else if (topRight.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTTOPRIGHT;
                    else if (left.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTLEFT;
                    else if (right.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTRIGHT;
                    else if (bottomLeft.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTBOTTOMLEFT;
                    else if (bottom.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTBOTTOM;
                    else if (bottomRight.Contains(windowPoint))
                        returnval = (IntPtr)Constants.HTBOTTOMRIGHT;

                    if (returnval != IntPtr.Zero)
                        handled = true;

                    break;

                case Constants.WM_INITMENU:
                    var window = AssociatedObject as MetroWindow;

                    if (window != null)
                    {
                        if (!window.ShowMaxRestoreButton)
                            UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_MAXIMIZE, Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                        else
                            if (window.WindowState == WindowState.Maximized)
                            {
                                UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_MAXIMIZE, Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                                UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_RESTORE, Constants.MF_ENABLED | Constants.MF_BYCOMMAND);
                                UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_MOVE, Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                            }
                            else
                            {
                                UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_MAXIMIZE, Constants.MF_ENABLED | Constants.MF_BYCOMMAND);
                                UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_RESTORE, Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                                UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_MOVE, Constants.MF_ENABLED | Constants.MF_BYCOMMAND);
                            }

                        if (!window.ShowMinButton)
                            UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_MINIMIZE, Constants.MF_GRAYED | Constants.MF_BYCOMMAND);

                        if (AssociatedObject.ResizeMode == ResizeMode.NoResize || window.WindowState == WindowState.Maximized)
                            UnsafeNativeMethods.EnableMenuItem(UnsafeNativeMethods.GetSystemMenu(hWnd, false), Constants.SC_SIZE, Constants.MF_GRAYED | Constants.MF_BYCOMMAND);
                    }
                    break;
            }

            return returnval;
        }
    }
}
