﻿using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Interop;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Native;
using Microsoft.Windows.Shell;

namespace MahApps.Metro.Behaviours
{
    /// <summary>
    /// With this class we can make custom window styles.
    /// </summary>
    public class BorderlessWindowBehavior : Behavior<Window>
    {
        private IntPtr handle;
        private HwndSource hwndSource;
        private WindowChrome windowChrome;
        private Thickness? savedBorderThickness = null;
        private Brush nonActiveBorderColor;
        private Brush savedBorderBrush = null;

        protected override void OnAttached()
        {
            // maybe this can change to set via window from a dependency property
            this.nonActiveBorderColor = new SolidColorBrush(Colors.Gray);
            this.nonActiveBorderColor.Freeze();

            windowChrome = new WindowChrome();
            windowChrome.ResizeBorderThickness = SystemParameters2.Current.WindowResizeBorderThickness;
            windowChrome.CaptionHeight = 0;
            windowChrome.CornerRadius = new CornerRadius(0);
            windowChrome.GlassFrameThickness = new Thickness(0);
            windowChrome.UseAeroCaptionButtons = false;
            var metroWindow = AssociatedObject as MetroWindow;
            windowChrome.IgnoreTaskbarOnMaximize = metroWindow != null && metroWindow.IgnoreTaskbarOnMaximize;
            AssociatedObject.SetValue(WindowChrome.WindowChromeProperty, windowChrome);

            // no transparany, because it hase more then one unwanted issues
            var windowHandle = new WindowInteropHelper(AssociatedObject).Handle;
            if (!AssociatedObject.IsLoaded && windowHandle == IntPtr.Zero)
            {
                try
                {
                    AssociatedObject.AllowsTransparency = false;
                }
                catch (Exception)
                {
                    //For some reason, we can't determine if the window has loaded or not, so we swallow the exception.
                }
            }
            AssociatedObject.WindowStyle = WindowStyle.None;
            savedBorderThickness = AssociatedObject.BorderThickness;

            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.Unloaded += AssociatedObject_Unloaded;
            AssociatedObject.SourceInitialized += AssociatedObject_SourceInitialized;
            AssociatedObject.StateChanged += AssociatedObject_StateChanged;
            AssociatedObject.Activated += AssociatedObject_Activated;
            AssociatedObject.Deactivated += AssociatedObject_Deactivated;

            // handle size to content (thanks @lynnx)
            var autoSizeToContent = AssociatedObject.SizeToContent == SizeToContent.WidthAndHeight;
            AssociatedObject.SizeToContent = SizeToContent.Height;
            AssociatedObject.SizeToContent = autoSizeToContent ? SizeToContent.WidthAndHeight : SizeToContent.Manual;
            AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;

            // handle resize mode after loading the window
            System.ComponentModel.DependencyPropertyDescriptor.FromProperty(Window.ResizeModeProperty, typeof(Window))
                  .AddValueChanged(AssociatedObject, ResizeModePropertyChangedCallback);
            
            base.OnAttached();
        }

        private void ResizeModePropertyChangedCallback(object sender, EventArgs e)
        {
            var metroWindow = sender as MetroWindow;
            if (metroWindow != null)
            {
                this.HandleResizeMode(metroWindow, metroWindow.ResizeMode);
            }
        }

        /// <summary>
        /// handle the resize mode for the window
        /// </summary>
        private void HandleResizeMode(MetroWindow window, ResizeMode resizeMode)
        {
            switch (resizeMode)
            {
                case ResizeMode.NoResize:
                    window.ShowMaxRestoreButton = false;
                    window.ShowMinButton = false;
                    break;
                case ResizeMode.CanMinimize:
                    window.ShowMaxRestoreButton = false;
                    window.ShowMinButton = true;
                    break;

                case ResizeMode.CanResize:
                case ResizeMode.CanResizeWithGrip:
                    window.ShowMaxRestoreButton = true;
                    window.ShowMinButton = true;
                    break;
            }
        }

        private bool isCleanedUp;

        private void Cleanup()
        {
            if (!isCleanedUp)
            {
                isCleanedUp = true;

                // clean up events
                System.ComponentModel.DependencyPropertyDescriptor.FromProperty(Window.ResizeModeProperty, typeof(Window))
                      .RemoveValueChanged(AssociatedObject, ResizeModePropertyChangedCallback);
                AssociatedObject.Loaded -= AssociatedObject_Loaded;
                AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
                AssociatedObject.SourceInitialized -= AssociatedObject_SourceInitialized;
                AssociatedObject.StateChanged -= AssociatedObject_StateChanged;
                AssociatedObject.Activated -= AssociatedObject_Activated;
                AssociatedObject.Deactivated -= AssociatedObject_Deactivated;
                AssociatedObject.IsVisibleChanged -= AssociatedObject_IsVisibleChanged;
                if (hwndSource != null)
                {
                    hwndSource.RemoveHook(WindowProc);
                }
                windowChrome = null;
            }
        }

        protected override void OnDetaching()
        {
            Cleanup();
            base.OnDetaching();
        }

        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            Cleanup();
        }

        private void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && (bool)e.NewValue)
            {
                AssociatedObject.SizeToContent = SizeToContent.Manual;
            }
        }

        private System.IntPtr WindowProc(System.IntPtr hwnd, int msg, System.IntPtr wParam, System.IntPtr lParam, ref bool handled)
        {
            var returnval = IntPtr.Zero;

            switch (msg) {
                case Constants.WM_NCPAINT:
                    var enableDWMDropShadow = EnableDWMDropShadow;
                    var metroWindow = AssociatedObject as MetroWindow;
                    if (metroWindow != null)
                    {
                        enableDWMDropShadow = metroWindow.GlowBrush == null && (metroWindow.EnableDWMDropShadow || EnableDWMDropShadow);
                    }
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

        private void AssociatedObject_Activated(object sender, EventArgs e)
        {
            if (savedBorderBrush != null)
            {
                AssociatedObject.BorderBrush = savedBorderBrush;
            }
            HandleMaximize();
        }

        private void AssociatedObject_Deactivated(object sender, EventArgs e)
        {
            if (AssociatedObject.BorderBrush != null)
            {
                savedBorderBrush = AssociatedObject.BorderBrush;
                AssociatedObject.BorderBrush = this.nonActiveBorderColor;
            }
        }

        private void AssociatedObject_StateChanged(object sender, EventArgs e)
        {
            HandleMaximize();
        }

        private void HandleMaximize()
        {
            if (AssociatedObject.WindowState == WindowState.Maximized)
            {
                // remove resize border and window border, so we can move the window from top monitor position
                windowChrome.ResizeBorderThickness = new Thickness(0);
                AssociatedObject.BorderThickness = new Thickness(0);

                // WindowChrome handles the size false if the main monitor is lesser the monitor where the window is maximized
                // so set the window pos/size twice
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
            hwndSource = HwndSource.FromHwnd(handle);
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

            // handle resize mode
            this.HandleResizeMode(window, window.ResizeMode);

            // non-active border brush
            if (window.NonActiveBorderBrush != null)
            {
                this.nonActiveBorderColor = window.NonActiveBorderBrush;
            }
        }

        public static readonly DependencyProperty EnableDWMDropShadowProperty = DependencyProperty.Register("EnableDWMDropShadow", typeof(bool), typeof(BorderlessWindowBehavior), new PropertyMetadata(false));

        public bool EnableDWMDropShadow
        {
            get { return (bool)GetValue(EnableDWMDropShadowProperty); }
            set { SetValue(EnableDWMDropShadowProperty, value); }
        }

        [Obsolete("This property is obsolete and will be removed in next release.")]
        public static readonly DependencyProperty AllowsTransparencyProperty = DependencyProperty.Register("AllowsTransparency", typeof(bool), typeof(BorderlessWindowBehavior), new PropertyMetadata(false));

        [Obsolete("This property is obsolete and will be removed in next release.")]
        public bool AllowsTransparency
        {
            get { return (bool)GetValue(AllowsTransparencyProperty); }
            set { SetValue(AllowsTransparencyProperty, value); }
        }

        [Obsolete("This property is obsolete and will be removed in next release (use Window.ResizeMode instead to handle this).")]
        public static readonly DependencyProperty ResizeWithGripProperty = DependencyProperty.Register("ResizeWithGrip", typeof(bool), typeof(BorderlessWindowBehavior), new PropertyMetadata(true));

        [Obsolete("This property is obsolete and will be removed in next release (use Window.ResizeMode to handle this).")]
        public bool ResizeWithGrip
        {
            get { return (bool)GetValue(ResizeWithGripProperty); }
            set { SetValue(ResizeWithGripProperty, value); }
        }

        [Obsolete("This property is obsolete and will be removed in next release (use Window.AutoSizeToContent instead to handle this).")]
        public static readonly DependencyProperty AutoSizeToContentProperty = DependencyProperty.Register("AutoSizeToContent", typeof(bool), typeof(BorderlessWindowBehavior), new PropertyMetadata(false));

        [Obsolete("This property is obsolete and will be removed in next release (use Window.AutoSizeToContent instead to handle this).")]
        public bool AutoSizeToContent
        {
            get { return (bool)GetValue(AutoSizeToContentProperty); }
            set { SetValue(AutoSizeToContentProperty, value); }
        }
    }
}
