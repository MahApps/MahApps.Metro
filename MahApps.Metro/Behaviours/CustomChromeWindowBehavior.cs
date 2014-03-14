using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;
using System.Windows.Interop;
using MahApps.Metro.Controls;
using MahApps.Metro.Native;
#if NET_4
using Microsoft.Windows.Shell;
#else
using System.Windows.Shell;
#endif

namespace MahApps.Metro.Behaviours
{
    public class CustomChromeWindowBehavior : Behavior<Window>
    {
        private AdornerDecorator rootElement;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.WindowStyle = WindowStyle.None;
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.SourceInitialized += AssociatedObject_SourceInitialized;

            var windowChrome = new WindowChrome();
            windowChrome.ResizeBorderThickness = new Thickness(6);
            windowChrome.CaptionHeight = 0;
            windowChrome.CornerRadius = new CornerRadius(0);
            windowChrome.GlassFrameThickness = new Thickness(0);

            AssociatedObject.SetValue(WindowChrome.WindowChromeProperty, windowChrome);
        }

        private System.IntPtr WindowProc(System.IntPtr hwnd, int msg, System.IntPtr wParam, System.IntPtr lParam, ref bool handled)
        {
            var returnval = IntPtr.Zero;
            
            switch (msg) {
                case Constants.WM_GETMINMAXINFO:
                    WmGetMinMaxInfo(hwnd, lParam);
                    /* Setting handled to false enables the application to process it's own Min/Max requirements,
                     * as mentioned by jason.bullard (comment from September 22, 2011) on http://gallery.expression.microsoft.com/ZuneWindowBehavior/ */
                    handled = false;
                    break;
            }

            return returnval;
        }

        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {
            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            System.IntPtr monitor = UnsafeNativeMethods.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != System.IntPtr.Zero) {

                MONITORINFO monitorInfo = new MONITORINFO();
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

        private void AssociatedObject_SourceInitialized(object sender, EventArgs e)
        {
            var handle = new WindowInteropHelper(AssociatedObject).Handle;
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

            rootElement = window.GetPart<AdornerDecorator>("PART_ROOT");

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
