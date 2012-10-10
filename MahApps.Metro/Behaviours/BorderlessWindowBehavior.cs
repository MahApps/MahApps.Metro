using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Interop;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Native;

namespace MahApps.Metro.Behaviours
{
    public class BorderlessWindowBehavior : Behavior<Window>
    {
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

        public Border Border { get; set; }

        private HwndSource _mHWNDSource;
        private IntPtr _mHWND;

        private static IntPtr SetClassLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size > 4)
                return UnsafeNativeMethods.SetClassLongPtr64(hWnd, nIndex, dwNewLong);

            return new IntPtr(UnsafeNativeMethods.SetClassLongPtr32(hWnd, nIndex, unchecked((uint)dwNewLong.ToInt32())));
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
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
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        protected override void OnAttached()
        {
            if (PresentationSource.FromVisual(AssociatedObject) != null)
                AddHwndHook();
            else
                AssociatedObject.SourceInitialized += AssociatedObject_SourceInitialized;

            AssociatedObject.WindowStyle = WindowStyle.None;
            AssociatedObject.StateChanged += AssociatedObjectStateChanged;

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
                                                   AssociatedObject.SizeToContent = AutoSizeToContent
                                                                                        ? SizeToContent.WidthAndHeight
                                                                                        : SizeToContent.Manual;
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
                var cx = ignoreTaskBar ? monitorInfo.rcWork.right : Math.Abs(monitorInfo.rcWork.right - x);
                var cy = ignoreTaskBar ? monitorInfo.rcMonitor.bottom : Math.Abs(monitorInfo.rcWork.bottom - y);
                UnsafeNativeMethods.SetWindowPos(_mHWND, new IntPtr(-2), x, y, cx, cy, 0x0040);
            }
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

            if (!UnsafeNativeMethods.DwmIsCompositionEnabled())
                return true;

            return false;
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
                        if (!ShouldHaveBorder())
                        {
                            var val = 2;
                            UnsafeNativeMethods.DwmSetWindowAttribute(_mHWND, 2, ref val, 4);
                            var m = new MARGINS { bottomHeight = 1, leftWidth = 1, rightWidth = 1, topHeight = 1 };
                            UnsafeNativeMethods.DwmExtendFrameIntoClientArea(_mHWND, ref m);

                            if (Border != null)
                                Border.BorderThickness = new Thickness(0);
                        }
                        else
                        {
                            AddBorder();
                        }
                        handled = true;
                    }
                    break;
                case Constants.WM_NCACTIVATE:
                    {
                        /* As per http://msdn.microsoft.com/en-us/library/ms632633(VS.85).aspx , "-1" lParam
                         * "does not repaint the nonclient area to reflect the state change." */
                        returnval = UnsafeNativeMethods.DefWindowProc(hWnd, message, wParam, new IntPtr(-1));

                        if (!ShouldHaveBorder())

                            if (wParam == IntPtr.Zero)
                                AddBorder();
                            else
                                RemoveBorder();

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
