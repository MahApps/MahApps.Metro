using System;
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
    public class BorderlessWindowBehavior : Behavior<Window>
    {
        private IntPtr handle;
        private HwndSource hwndSource;
        private WindowChrome windowChrome;
        private Thickness? savedBorderThickness;

        protected override void OnAttached()
        {
            windowChrome = new WindowChrome
            {
                ResizeBorderThickness = SystemParameters2.Current.WindowResizeBorderThickness, 
                CaptionHeight = 0, 
                CornerRadius = new CornerRadius(0), 
                GlassFrameThickness = new Thickness(0), 
                UseAeroCaptionButtons = false
            };

            var metroWindow = AssociatedObject as MetroWindow;
            if (metroWindow != null)
            {
                windowChrome.IgnoreTaskbarOnMaximize = metroWindow.IgnoreTaskbarOnMaximize;
                windowChrome.UseNoneWindowStyle = metroWindow.UseNoneWindowStyle;
                System.ComponentModel.DependencyPropertyDescriptor.FromProperty(MetroWindow.IgnoreTaskbarOnMaximizeProperty, typeof(MetroWindow))
                      .AddValueChanged(AssociatedObject, IgnoreTaskbarOnMaximizePropertyChangedCallback);
                System.ComponentModel.DependencyPropertyDescriptor.FromProperty(MetroWindow.UseNoneWindowStyleProperty, typeof(MetroWindow))
                      .AddValueChanged(AssociatedObject, UseNoneWindowStylePropertyChangedCallback);
            }

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
            AssociatedObject.StateChanged += OnAssociatedObjectHandleMaximize;

            base.OnAttached();
        }

        private void UseNoneWindowStylePropertyChangedCallback(object sender, EventArgs e)
        {
            var metroWindow = sender as MetroWindow;
            if (metroWindow != null && windowChrome != null)
            {
                if (!Equals(windowChrome.UseNoneWindowStyle, metroWindow.UseNoneWindowStyle))
                {
                    windowChrome.UseNoneWindowStyle = metroWindow.UseNoneWindowStyle;
                    this.ForceRedrawWindowFromPropertyChanged();
                }
            }
        }

        private void IgnoreTaskbarOnMaximizePropertyChangedCallback(object sender, EventArgs e)
        {
            var metroWindow = sender as MetroWindow;
            if (metroWindow != null && windowChrome != null)
            {
                if (!Equals(windowChrome.IgnoreTaskbarOnMaximize, metroWindow.IgnoreTaskbarOnMaximize))
                {
                    windowChrome.IgnoreTaskbarOnMaximize = metroWindow.IgnoreTaskbarOnMaximize;
                    this.ForceRedrawWindowFromPropertyChanged();
                }
            }
        }

        private void ForceRedrawWindowFromPropertyChanged()
        {
            this.HandleMaximize();
            if (this.handle != IntPtr.Zero)
            {
                UnsafeNativeMethods.RedrawWindow(this.handle, IntPtr.Zero, IntPtr.Zero, Constants.RedrawWindowFlags.Invalidate | Constants.RedrawWindowFlags.Frame);
            }
        }

        private bool isCleanedUp;

        private void Cleanup()
        {
            if (!isCleanedUp)
            {
                isCleanedUp = true;

                // clean up events
                if (AssociatedObject is MetroWindow)
                {
                    System.ComponentModel.DependencyPropertyDescriptor.FromProperty(MetroWindow.IgnoreTaskbarOnMaximizeProperty, typeof(MetroWindow))
                          .RemoveValueChanged(AssociatedObject, IgnoreTaskbarOnMaximizePropertyChangedCallback);
                    System.ComponentModel.DependencyPropertyDescriptor.FromProperty(MetroWindow.UseNoneWindowStyleProperty, typeof(MetroWindow))
                          .RemoveValueChanged(AssociatedObject, UseNoneWindowStylePropertyChangedCallback);
                }
                AssociatedObject.Loaded -= AssociatedObject_Loaded;
                AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
                AssociatedObject.SourceInitialized -= AssociatedObject_SourceInitialized;
                AssociatedObject.StateChanged -= OnAssociatedObjectHandleMaximize;
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

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var returnval = IntPtr.Zero;
            var metroWindow = AssociatedObject as MetroWindow;

            switch (msg) {
                case Constants.WM_NCPAINT:
                    var enableDWMDropShadow = EnableDWMDropShadow;

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
                case Constants.WM_NCACTIVATE:
                    /* As per http://msdn.microsoft.com/en-us/library/ms632633(VS.85).aspx , "-1" lParam "does not repaint the nonclient area to reflect the state change." */
                    returnval = UnsafeNativeMethods.DefWindowProc(hwnd, msg, wParam, new IntPtr(-1));
                    handled = true;
                    break;
            }

            return returnval;
        }

        private void OnAssociatedObjectHandleMaximize(object sender, EventArgs e)
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

                var metroWindow = AssociatedObject as MetroWindow;
                var ignoreTaskBar = metroWindow != null && metroWindow.IgnoreTaskbarOnMaximize;
                if (ignoreTaskBar)
                {
                    // WindowChrome handles the size false if the main monitor is lesser the monitor where the window is maximized
                    // so set the window pos/size twice
                    IntPtr monitor = UnsafeNativeMethods.MonitorFromWindow(handle, Constants.MONITOR_DEFAULTTONEAREST);
                    if (monitor != IntPtr.Zero)
                    {
                        var monitorInfo = new MONITORINFO();
                        UnsafeNativeMethods.GetMonitorInfo(monitor, monitorInfo);

                        //ignoreTaskBar = metroWindow.IgnoreTaskbarOnMaximize || metroWindow.UseNoneWindowStyle;
                        var x = ignoreTaskBar ? monitorInfo.rcMonitor.left : monitorInfo.rcWork.left;
                        var y = ignoreTaskBar ? monitorInfo.rcMonitor.top : monitorInfo.rcWork.top;
                        var cx = ignoreTaskBar ? Math.Abs(monitorInfo.rcMonitor.right - x) : Math.Abs(monitorInfo.rcWork.right - x);
                        var cy = ignoreTaskBar ? Math.Abs(monitorInfo.rcMonitor.bottom - y) : Math.Abs(monitorInfo.rcWork.bottom - y);
                        UnsafeNativeMethods.SetWindowPos(handle, new IntPtr(-2), x, y, cx, cy, 0x0040);
                    }
                }
            }
            else
            {
                windowChrome.ResizeBorderThickness = SystemParameters2.Current.WindowResizeBorderThickness;
                AssociatedObject.BorderThickness = savedBorderThickness.GetValueOrDefault(new Thickness(0));
                
                // fix nasty TopMost bug
                // - set TopMost="True"
                // - start mahapps demo
                // - TopMost works
                // - maximize window and back to normal
                // - TopMost is gone
                var topMost = AssociatedObject.Topmost;
                AssociatedObject.Topmost = false;
                AssociatedObject.Topmost = topMost;
            }
        }

        private void AssociatedObject_SourceInitialized(object sender, EventArgs e)
        {
            handle = new WindowInteropHelper(AssociatedObject).Handle;
            hwndSource = HwndSource.FromHwnd(handle);
            if (hwndSource != null)
            {
                hwndSource.AddHook(WindowProc);
            }

            // handle size to content (thanks @lynnx)
            var sizeToContent = AssociatedObject.SizeToContent;
            var snapsToDevicePixels = AssociatedObject.SnapsToDevicePixels;
            AssociatedObject.SnapsToDevicePixels = true;
            AssociatedObject.SizeToContent = sizeToContent == SizeToContent.WidthAndHeight ? SizeToContent.Height : SizeToContent.Manual;
            AssociatedObject.SizeToContent = sizeToContent;
            AssociatedObject.SnapsToDevicePixels = snapsToDevicePixels;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = sender as MetroWindow;
            if (window == null)
            {
                return;
            }

            window.SetIsHitTestVisibleInChromeProperty<UIElement>("PART_Icon");
            window.SetIsHitTestVisibleInChromeProperty<UIElement>("PART_TitleBar");
            window.SetIsHitTestVisibleInChromeProperty<ContentPresenter>("PART_LeftWindowCommands");
            window.SetIsHitTestVisibleInChromeProperty<ContentPresenter>("PART_RightWindowCommands");
            window.SetIsHitTestVisibleInChromeProperty<ContentControl>("PART_WindowButtonCommands");
        }

        public static readonly DependencyProperty EnableDWMDropShadowProperty = DependencyProperty.Register("EnableDWMDropShadow", typeof(bool), typeof(BorderlessWindowBehavior), new PropertyMetadata(false));

        public bool EnableDWMDropShadow
        {
            get { return (bool)GetValue(EnableDWMDropShadowProperty); }
            set { SetValue(EnableDWMDropShadowProperty, value); }
        }
    }
}
