using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Interop;
using MahApps.Metro.Controls;
using MahApps.Metro.Native;
using Microsoft.Windows.Shell;

namespace MahApps.Metro.Behaviours
{
    /// <summary>
    /// With this class we can make custom window styles.
    /// </summary>
    public class CustomChromeWindowBehavior : Behavior<Window>
    {
        private IntPtr handle;
        private WindowChrome windowChrome;
        private Thickness? savedBorderThickness = null;

        protected override void OnAttached()
        {
            windowChrome = new WindowChrome();
            windowChrome.ResizeBorderThickness = SystemParameters2.Current.WindowResizeBorderThickness;
            windowChrome.CaptionHeight = 0;
            windowChrome.CornerRadius = new CornerRadius(0);
            windowChrome.GlassFrameThickness = new Thickness(0);
            windowChrome.UseAeroCaptionButtons = false;
            AssociatedObject.SetValue(WindowChrome.WindowChromeProperty, windowChrome);

            // no transparany, because it hase more then one unwanted issues
            AssociatedObject.AllowsTransparency = false;
            AssociatedObject.WindowStyle = WindowStyle.None;
            savedBorderThickness = AssociatedObject.BorderThickness;

            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.SourceInitialized += AssociatedObject_SourceInitialized;
            AssociatedObject.StateChanged += (sender, args) => HandleMaximize();
            AssociatedObject.Activated += (sender, args) => HandleMaximize();

            // handle size to content (thanks @lynnx)
            var autoSizeToContent = AssociatedObject.SizeToContent == SizeToContent.WidthAndHeight;
            AssociatedObject.SizeToContent = SizeToContent.Height;
            AssociatedObject.SizeToContent = autoSizeToContent ? SizeToContent.WidthAndHeight : SizeToContent.Manual;
            AssociatedObject.IsVisibleChanged += (sender, args) => {
                                                     if (args.NewValue != args.OldValue && (bool)args.NewValue)
                                                     {
                                                         AssociatedObject.SizeToContent = SizeToContent.Manual;
                                                     }
                                                 };

            base.OnAttached();
        }

        private System.IntPtr WindowProc(System.IntPtr hwnd, int msg, System.IntPtr wParam, System.IntPtr lParam, ref bool handled)
        {
            var returnval = IntPtr.Zero;

            switch (msg) {
                case Constants.WM_NCPAINT:
                    var metroWindow = AssociatedObject as MetroWindow;
                    var enableDWMDropShadow = metroWindow != null && metroWindow.EnableDWMDropShadow && metroWindow.GlowBrush == null;
                    if (enableDWMDropShadow)
                    {
                        var val = 2;
                        UnsafeNativeMethods.DwmSetWindowAttribute(hwnd, 2, ref val, 4);
                        var m = new MARGINS { bottomHeight = 1, leftWidth = 1, rightWidth = 1, topHeight = 1 };
                        UnsafeNativeMethods.DwmExtendFrameIntoClientArea(hwnd, ref m);
                    }
                    handled = true;
                    break;
                case Constants.WM_GETMINMAXINFO:
                    WmGetMinMaxInfo(hwnd, lParam);
                    /* Setting handled to false enables the application to process it's own Min/Max requirements,
                     * as mentioned by jason.bullard (comment from September 22, 2011) on http://gallery.expression.microsoft.com/ZuneWindowBehavior/ */
                    handled = false;
                    break;
                case Constants.WM_NCACTIVATE:
                    /* As per http://msdn.microsoft.com/en-us/library/ms632633(VS.85).aspx , "-1" lParam "does not repaint the nonclient area to reflect the state change." */
                    returnval = UnsafeNativeMethods.DefWindowProc(hwnd, msg, wParam, new IntPtr(-1));
                    handled = true;
                    break;
            }

            return returnval;
        }

        private void HandleMaximize()
        {
            if (AssociatedObject.WindowState == WindowState.Maximized)
            {
                // remove resize border and window border, so we can move the window from top monitor position
                windowChrome.ResizeBorderThickness = new Thickness(0);
                AssociatedObject.BorderThickness = new Thickness(0);
                // set the glass frame to 1, so we can have a full screen window that ignores the taskbar
                windowChrome.GlassFrameThickness = new Thickness(1);
                
                IntPtr monitor = UnsafeNativeMethods.MonitorFromWindow(handle, Constants.MONITOR_DEFAULTTONEAREST);
                if (monitor != IntPtr.Zero) {
                    var monitorInfo = new MONITORINFO();
                    UnsafeNativeMethods.GetMonitorInfo(monitor, monitorInfo);
                    var metroWindow = AssociatedObject as MetroWindow;
                    var ignoreTaskBar = metroWindow != null && (metroWindow.IgnoreTaskbarOnMaximize || metroWindow.UseNoneWindowStyle);
                    var x = ignoreTaskBar ? monitorInfo.rcMonitor.left : monitorInfo.rcWork.left;
                    var y = ignoreTaskBar ? monitorInfo.rcMonitor.top : monitorInfo.rcWork.top;
                    var cx = ignoreTaskBar ? Math.Abs(monitorInfo.rcMonitor.right - x) : Math.Abs(monitorInfo.rcWork.right - x);
                    var cy = ignoreTaskBar ? Math.Abs(monitorInfo.rcMonitor.bottom - y) : Math.Abs(monitorInfo.rcWork.bottom - y);
                    UnsafeNativeMethods.SetWindowPos(handle, new IntPtr(-2), x, y, cx, cy, 0x0040);
                }
            }
            else
            {
                windowChrome.GlassFrameThickness = new Thickness(0);
                windowChrome.ResizeBorderThickness = SystemParameters2.Current.WindowResizeBorderThickness;
                AssociatedObject.BorderThickness = savedBorderThickness.GetValueOrDefault(new Thickness(0));
            }
        }

        private void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {
            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            System.IntPtr monitor = UnsafeNativeMethods.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != System.IntPtr.Zero)
            {

                MONITORINFO monitorInfo = new MONITORINFO();
                UnsafeNativeMethods.GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
//                mmi.ptMaxSize.X = Math.Abs(rcWorkArea.right - rcWorkArea.left);
//                mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);

                var metroWindow = AssociatedObject as MetroWindow;
                var ignoreTaskBar = metroWindow != null && (metroWindow.IgnoreTaskbarOnMaximize || metroWindow.UseNoneWindowStyle);
                var x = ignoreTaskBar ? monitorInfo.rcMonitor.left : monitorInfo.rcWork.left;
                var y = ignoreTaskBar ? monitorInfo.rcMonitor.top : monitorInfo.rcWork.top;
                mmi.ptMaxSize.X = ignoreTaskBar ? Math.Abs(monitorInfo.rcMonitor.right - x) : Math.Abs(monitorInfo.rcWork.right - x);
                mmi.ptMaxSize.Y = ignoreTaskBar ? Math.Abs(monitorInfo.rcMonitor.bottom - y) : Math.Abs(monitorInfo.rcWork.bottom - y);

                // only do this on maximize
                if (!ignoreTaskBar && AssociatedObject.WindowState == WindowState.Maximized) {
                    mmi.ptMaxTrackSize.X = mmi.ptMaxSize.X;
                    mmi.ptMaxTrackSize.Y = mmi.ptMaxSize.Y;
                    mmi = AdjustWorkingAreaForAutoHide(monitor, mmi);
                }
            }

            Marshal.StructureToPtr(mmi, lParam, true);
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
            {
                return mmi;
            }

            var abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = hwnd;
            UnsafeNativeMethods.SHAppBarMessage((int)ABMsg.ABM_GETTASKBARPOS, ref abd);
            int uEdge = GetEdge(abd.rc);
            bool autoHide = Convert.ToBoolean(UnsafeNativeMethods.SHAppBarMessage((int)ABMsg.ABM_GETSTATE, ref abd));

            if (!autoHide)
            {
                return mmi;
            }

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

        private void AssociatedObject_SourceInitialized(object sender, EventArgs e)
        {
            handle = new WindowInteropHelper(AssociatedObject).Handle;
            var hwndSource = HwndSource.FromHwnd(handle);
            if (hwndSource != null)
            {
                hwndSource.AddHook(new HwndSourceHook(WindowProc));
            }
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = sender as MetroWindow;
            if (window == null)
            {
                return;
            }

            var icon = window.GetPart<UIElement>("PART_Icon");
            if (icon != null)
            {
                icon.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, true);
            }
            
            var titleBar = window.GetPart<UIElement>("PART_TitleBar");
            if (titleBar != null)
            {
                titleBar.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, true);
            }
            
            var leftWindowCommands = window.GetPart<ContentPresenter>("PART_LeftWindowCommands");
            if (leftWindowCommands != null)
            {
                leftWindowCommands.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, true);
            }
            
            var windowCommands = window.GetPart<ContentPresenter>("PART_RightWindowCommands");
            if (windowCommands != null)
            {
                windowCommands.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, true);
            }
            
            var windowButtonCommands = window.GetPart<ContentControl>("PART_WindowButtonCommands");
            if (windowButtonCommands != null)
            {
                windowButtonCommands.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, true);
            }
        }
    }
}
