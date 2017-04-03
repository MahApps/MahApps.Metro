using System;
using System.Linq;
using System.Management;
using System.Security;
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
        private PropertyChangeNotifier borderThicknessChangeNotifier;
        private Thickness? savedBorderThickness;
        private Thickness? savedResizeBorderThickness;
        private PropertyChangeNotifier topMostChangeNotifier;
        private bool savedTopMost;
        private bool isWindwos10OrHigher;

        private static bool IsWindows10OrHigher()
        {
            var version = Standard.NtDll.RtlGetVersion();
            if (default(Version) == version)
            {
                // Snippet from Koopakiller https://dotnet-snippets.de/snippet/os-version-name-mit-wmi/4929
                using (var mos = new ManagementObjectSearcher("SELECT Caption, Version FROM Win32_OperatingSystem"))
                {
                    var attribs = mos.Get().OfType<ManagementObject>();
                    //caption = attribs.FirstOrDefault().GetPropertyValue("Caption").ToString() ?? "Unknown";
                    version = new Version((attribs.FirstOrDefault()?.GetPropertyValue("Version") ?? "0.0.0.0").ToString());
                }
            }
            return version >= new Version(10, 0);
        }

        protected override void OnAttached()
        {
            this.isWindwos10OrHigher = IsWindows10OrHigher();

            this.windowChrome = new WindowChrome
                                {
#if NET4_5
                                    ResizeBorderThickness = SystemParameters.WindowResizeBorderThickness, 
#else
                                    ResizeBorderThickness = SystemParameters2.Current.WindowResizeBorderThickness,
#endif
                                    CaptionHeight = 0,
                                    CornerRadius = new CornerRadius(0),
                                    GlassFrameThickness = new Thickness(0),
                                    UseAeroCaptionButtons = false
                                };

            var metroWindow = this.AssociatedObject as MetroWindow;
            if (metroWindow != null)
            {
                this.windowChrome.IgnoreTaskbarOnMaximize = metroWindow.IgnoreTaskbarOnMaximize;
                this.windowChrome.UseNoneWindowStyle = metroWindow.UseNoneWindowStyle;
                System.ComponentModel.DependencyPropertyDescriptor.FromProperty(MetroWindow.IgnoreTaskbarOnMaximizeProperty, typeof(MetroWindow))
                      .AddValueChanged(this.AssociatedObject, this.IgnoreTaskbarOnMaximizePropertyChangedCallback);
                System.ComponentModel.DependencyPropertyDescriptor.FromProperty(MetroWindow.UseNoneWindowStyleProperty, typeof(MetroWindow))
                      .AddValueChanged(this.AssociatedObject, this.UseNoneWindowStylePropertyChangedCallback);
            }

            this.AssociatedObject.SetValue(WindowChrome.WindowChromeProperty, this.windowChrome);

            // no transparany, because it hase more then one unwanted issues
            var windowHandle = new WindowInteropHelper(this.AssociatedObject).Handle;
            if (!this.AssociatedObject.IsLoaded && windowHandle == IntPtr.Zero)
            {
                try
                {
                    this.AssociatedObject.AllowsTransparency = false;
                }
                catch (Exception)
                {
                    //For some reason, we can't determine if the window has loaded or not, so we swallow the exception.
                }
            }
            this.AssociatedObject.WindowStyle = WindowStyle.None;

            this.savedBorderThickness = this.AssociatedObject.BorderThickness;
            this.savedResizeBorderThickness = this.windowChrome.ResizeBorderThickness;
            this.borderThicknessChangeNotifier = new PropertyChangeNotifier(this.AssociatedObject, Control.BorderThicknessProperty);
            this.borderThicknessChangeNotifier.ValueChanged += this.BorderThicknessChangeNotifierOnValueChanged;

            this.savedTopMost = this.AssociatedObject.Topmost;
            this.topMostChangeNotifier = new PropertyChangeNotifier(this.AssociatedObject, Window.TopmostProperty);
            this.topMostChangeNotifier.ValueChanged += this.TopMostChangeNotifierOnValueChanged;

            // #1823 try to fix another nasty issue
            // WindowState = Maximized
            // ResizeMode = NoResize
            if (this.AssociatedObject.ResizeMode == ResizeMode.NoResize)
            {
                this.windowChrome.ResizeBorderThickness = new Thickness(0);
            }

            var topmostHack = new Action(() =>
                                           {
                                               if (this.AssociatedObject.Topmost)
                                               {
                                                   var raiseValueChanged = this.topMostChangeNotifier.RaiseValueChanged;
                                                   this.topMostChangeNotifier.RaiseValueChanged = false;
                                                   this.AssociatedObject.Topmost = false;
                                                   this.AssociatedObject.Topmost = true;
                                                   this.topMostChangeNotifier.RaiseValueChanged = raiseValueChanged;
                                               }
                                           });
            this.AssociatedObject.LostFocus += (sender, args) => { topmostHack(); };
            this.AssociatedObject.Deactivated += (sender, args) => { topmostHack(); };

            this.AssociatedObject.Loaded += this.AssociatedObject_Loaded;
            this.AssociatedObject.Unloaded += this.AssociatedObjectUnloaded;
            this.AssociatedObject.Closed += this.AssociatedObjectClosed;
            this.AssociatedObject.SourceInitialized += this.AssociatedObject_SourceInitialized;
            this.AssociatedObject.StateChanged += this.OnAssociatedObjectHandleMaximize;

            base.OnAttached();
        }

        private void BorderThicknessChangeNotifierOnValueChanged(object sender, EventArgs e)
        {
            // It's bad if the window is null at this point, but we check this here to prevent the possible occurred exception
            var window = this.AssociatedObject;
            if (window != null)
            {
                this.savedBorderThickness = window.BorderThickness;
            }
        }

        private void TopMostChangeNotifierOnValueChanged(object sender, EventArgs e)
        {
            // It's bad if the window is null at this point, but we check this here to prevent the possible occurred exception
            var window = this.AssociatedObject;
            if (window != null)
            {
                this.savedTopMost = window.Topmost;
            }
        }

        private void UseNoneWindowStylePropertyChangedCallback(object sender, EventArgs e)
        {
            var metroWindow = sender as MetroWindow;
            if (metroWindow != null && this.windowChrome != null)
            {
                if (!Equals(this.windowChrome.UseNoneWindowStyle, metroWindow.UseNoneWindowStyle))
                {
                    this.windowChrome.UseNoneWindowStyle = metroWindow.UseNoneWindowStyle;
                    this.ForceRedrawWindowFromPropertyChanged();
                }
            }
        }

        private void IgnoreTaskbarOnMaximizePropertyChangedCallback(object sender, EventArgs e)
        {
            var metroWindow = sender as MetroWindow;
            if (metroWindow != null && this.windowChrome != null)
            {
                if (!Equals(this.windowChrome.IgnoreTaskbarOnMaximize, metroWindow.IgnoreTaskbarOnMaximize))
                {
                    // another special hack to avoid nasty resizing
                    // repro
                    // ResizeMode="NoResize"
                    // WindowState="Maximized"
                    // IgnoreTaskbarOnMaximize="True"
                    // this only happens if we change this at runtime
                    var removed = this._ModifyStyle(Standard.WS.MAXIMIZEBOX | Standard.WS.MINIMIZEBOX | Standard.WS.THICKFRAME, 0);
                    this.windowChrome.IgnoreTaskbarOnMaximize = metroWindow.IgnoreTaskbarOnMaximize;
                    if (removed)
                    {
                        this._ModifyStyle(0, Standard.WS.MAXIMIZEBOX | Standard.WS.MINIMIZEBOX | Standard.WS.THICKFRAME);
                    }
                    this.ForceRedrawWindowFromPropertyChanged();
                }
            }
        }

        /// <summary>Add and remove a native WindowStyle from the HWND.</summary>
        /// <param name="removeStyle">The styles to be removed.  These can be bitwise combined.</param>
        /// <param name="addStyle">The styles to be added.  These can be bitwise combined.</param>
        /// <returns>Whether the styles of the HWND were modified as a result of this call.</returns>
        /// <SecurityNote>
        ///   Critical : Calls critical methods
        /// </SecurityNote>
        [SecurityCritical]
        private bool _ModifyStyle(Standard.WS removeStyle, Standard.WS addStyle)
        {
            if (this.handle == IntPtr.Zero)
            {
                return false;
            }
            var intPtr = Standard.NativeMethods.GetWindowLongPtr(this.handle, Standard.GWL.STYLE);
            var dwStyle = (Standard.WS)(Environment.Is64BitProcess ? intPtr.ToInt64() : intPtr.ToInt32());
            var dwNewStyle = (dwStyle & ~removeStyle) | addStyle;
            if (dwStyle == dwNewStyle)
            {
                return false;
            }

            Standard.NativeMethods.SetWindowLongPtr(this.handle, Standard.GWL.STYLE, new IntPtr((int)dwNewStyle));
            return true;
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
            if (!this.isCleanedUp)
            {
                this.isCleanedUp = true;

                if (GetHandleTaskbar(this.AssociatedObject) && this.isWindwos10OrHigher)
                {
                    this.DeactivateTaskbarFix();
                }

                // clean up events
                if (this.AssociatedObject is MetroWindow)
                {
                    System.ComponentModel.DependencyPropertyDescriptor.FromProperty(MetroWindow.IgnoreTaskbarOnMaximizeProperty, typeof(MetroWindow))
                          .RemoveValueChanged(this.AssociatedObject, this.IgnoreTaskbarOnMaximizePropertyChangedCallback);
                    System.ComponentModel.DependencyPropertyDescriptor.FromProperty(MetroWindow.UseNoneWindowStyleProperty, typeof(MetroWindow))
                          .RemoveValueChanged(this.AssociatedObject, this.UseNoneWindowStylePropertyChangedCallback);
                }
                this.AssociatedObject.Loaded -= this.AssociatedObject_Loaded;
                this.AssociatedObject.Unloaded -= this.AssociatedObjectUnloaded;
                this.AssociatedObject.Closed -= this.AssociatedObjectClosed;
                this.AssociatedObject.SourceInitialized -= this.AssociatedObject_SourceInitialized;
                this.AssociatedObject.StateChanged -= this.OnAssociatedObjectHandleMaximize;
                this.hwndSource?.RemoveHook(this.WindowProc);
                this.windowChrome = null;
            }
        }

        protected override void OnDetaching()
        {
            this.Cleanup();
            base.OnDetaching();
        }

        private void AssociatedObjectUnloaded(object sender, RoutedEventArgs e)
        {
            this.Cleanup();
        }

        private void AssociatedObjectClosed(object sender, EventArgs e)
        {
            this.Cleanup();
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var returnval = IntPtr.Zero;

            switch (msg)
            {
                case Constants.WM_NCPAINT:
                    handled = true;
                    break;
                case Constants.WM_NCACTIVATE:
                    /* As per http://msdn.microsoft.com/en-us/library/ms632633(VS.85).aspx , "-1" lParam "does not repaint the nonclient area to reflect the state change." */
                    returnval = UnsafeNativeMethods.DefWindowProc(hwnd, msg, wParam, new IntPtr(-1));
                    handled = true;
                    break;
                case (int)Standard.WM.WINDOWPOSCHANGING:
                    {
                        var pos = (Standard.WINDOWPOS)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(Standard.WINDOWPOS));
                        if ((pos.flags & (int)Standard.SWP.NOMOVE) != 0)
                        {
                            return IntPtr.Zero;
                        }

                        var wnd = this.AssociatedObject;
                        if (wnd == null || this.hwndSource?.CompositionTarget == null)
                        {
                            return IntPtr.Zero;
                        }

                        bool changedPos = false;

                        // Convert the original to original size based on DPI setting. Need for x% screen DPI.
                        var matrix = this.hwndSource.CompositionTarget.TransformToDevice;

                        var minWidth = wnd.MinWidth * matrix.M11;
                        var minHeight = wnd.MinHeight * matrix.M22;
                        if (pos.cx < minWidth) { pos.cx = (int)minWidth; changedPos = true; }
                        if (pos.cy < minHeight) { pos.cy = (int)minHeight; changedPos = true; }

                        var maxWidth = wnd.MaxWidth * matrix.M11;
                        var maxHeight = wnd.MaxHeight * matrix.M22;
                        if (pos.cx > maxWidth && maxWidth > 0) { pos.cx = (int)Math.Round(maxWidth); changedPos = true; }
                        if (pos.cy > maxHeight && maxHeight > 0) { pos.cy = (int)Math.Round(maxHeight); changedPos = true; }

                        if (!changedPos)
                        {
                            return IntPtr.Zero;
                        }

                        System.Runtime.InteropServices.Marshal.StructureToPtr(pos, lParam, true);
                        handled = true;
                    }
                    break;
            }

            return returnval;
        }

        private void OnAssociatedObjectHandleMaximize(object sender, EventArgs e)
        {
            this.HandleMaximize();
        }

        private void HandleMaximize()
        {
            this.borderThicknessChangeNotifier.RaiseValueChanged = false;
            var raiseValueChanged = this.topMostChangeNotifier.RaiseValueChanged;
            this.topMostChangeNotifier.RaiseValueChanged = false;

            var metroWindow = this.AssociatedObject as MetroWindow;
            var enableDWMDropShadow = this.EnableDWMDropShadow;
            if (metroWindow != null)
            {
                enableDWMDropShadow = metroWindow.GlowBrush == null && (metroWindow.EnableDWMDropShadow || this.EnableDWMDropShadow);
            }

            if (this.AssociatedObject.WindowState == WindowState.Maximized)
            {
                // remove window border, so we can move the window from top monitor position
                this.AssociatedObject.BorderThickness = new Thickness(0);

                var ignoreTaskBar = metroWindow != null && metroWindow.IgnoreTaskbarOnMaximize;
                if (this.handle != IntPtr.Zero)
                {
                    this.windowChrome.ResizeBorderThickness = new Thickness(0);

                    // WindowChrome handles the size false if the main monitor is lesser the monitor where the window is maximized
                    // so set the window pos/size twice
                    IntPtr monitor = UnsafeNativeMethods.MonitorFromWindow(this.handle, Constants.MONITOR_DEFAULTTONEAREST);
                    if (monitor != IntPtr.Zero)
                    {
                        var monitorInfo = new Native.MONITORINFO();
                        UnsafeNativeMethods.GetMonitorInfo(monitor, monitorInfo);

                        var desktopRect = ignoreTaskBar ? monitorInfo.rcMonitor :  monitorInfo.rcWork;
                        var x = desktopRect.left;
                        var y = desktopRect.top;
                        var cx = Math.Abs(desktopRect.right - x);
                        var cy = Math.Abs(desktopRect.bottom - y);

                        if (ignoreTaskBar && this.isWindwos10OrHigher)
                        {
                            this.ActivateTaskbarFix();
                        }

                        UnsafeNativeMethods.SetWindowPos(this.handle, new IntPtr(-2), x, y, cx, cy, 0x0040);
                    }
                }
            }
            else
            {
                if (!enableDWMDropShadow)
                {
                    this.AssociatedObject.BorderThickness = this.savedBorderThickness.GetValueOrDefault(new Thickness(0));
                }
                var resizeBorderThickness = this.savedResizeBorderThickness.GetValueOrDefault(new Thickness(0));
                if (this.windowChrome.ResizeBorderThickness != resizeBorderThickness)
                {
                    this.windowChrome.ResizeBorderThickness = resizeBorderThickness;
                }

                // #2694 make sure the window is not on top after restoring window
                // this issue was introduced after fixing the windows 10 bug with the taskbar and a maximized window that ignores the taskbar
                if (GetHandleTaskbar(this.AssociatedObject) && this.isWindwos10OrHigher)
                {
                    this.DeactivateTaskbarFix();
                }
            }

            // fix nasty TopMost bug
            // - set TopMost="True"
            // - start mahapps demo
            // - TopMost works
            // - maximize window and back to normal
            // - TopMost is gone
            //
            // Problem with minimize animation when window is maximized #1528
            // 1. Activate another application (such as Google Chrome).
            // 2. Run the demo and maximize it.
            // 3. Minimize the demo by clicking on the taskbar button.
            // Note that the minimize animation in this case does actually run, but somehow the other
            // application (Google Chrome in this example) is instantly switched to being the top window,
            // and so blocking the animation view.
            this.AssociatedObject.Topmost = false;
            this.AssociatedObject.Topmost = this.AssociatedObject.WindowState == WindowState.Minimized || this.savedTopMost;

            this.borderThicknessChangeNotifier.RaiseValueChanged = true;
            this.topMostChangeNotifier.RaiseValueChanged = raiseValueChanged;
        }

        private void ActivateTaskbarFix()
        {
            var trayWndHandle = Standard.NativeMethods.FindWindow("Shell_TrayWnd", null);
            if (trayWndHandle != IntPtr.Zero)
            {
                SetHandleTaskbar(this.AssociatedObject, true);
                UnsafeNativeMethods.SetWindowPos(trayWndHandle, Constants.HWND_BOTTOM, 0, 0, 0, 0, Constants.TOPMOST_FLAGS);
                UnsafeNativeMethods.SetWindowPos(trayWndHandle, Constants.HWND_TOP, 0, 0, 0, 0, Constants.TOPMOST_FLAGS);
                UnsafeNativeMethods.SetWindowPos(trayWndHandle, Constants.HWND_NOTOPMOST, 0, 0, 0, 0, Constants.TOPMOST_FLAGS);
            }
        }

        private void DeactivateTaskbarFix()
        {
            var trayWndHandle = Standard.NativeMethods.FindWindow("Shell_TrayWnd", null);
            if (trayWndHandle != IntPtr.Zero)
            {
                SetHandleTaskbar(this.AssociatedObject, false);
                UnsafeNativeMethods.SetWindowPos(trayWndHandle, Constants.HWND_BOTTOM, 0, 0, 0, 0, Constants.TOPMOST_FLAGS);
                UnsafeNativeMethods.SetWindowPos(trayWndHandle, Constants.HWND_TOP, 0, 0, 0, 0, Constants.TOPMOST_FLAGS);
                UnsafeNativeMethods.SetWindowPos(trayWndHandle, Constants.HWND_TOPMOST, 0, 0, 0, 0, Constants.TOPMOST_FLAGS);
            }
        }

        private static readonly DependencyProperty HandleTaskbarProperty
            = DependencyProperty.RegisterAttached(
                "HandleTaskbar",
                typeof(bool),
                typeof(BorderlessWindowBehavior), new FrameworkPropertyMetadata(false));

        private static bool GetHandleTaskbar(UIElement element)
        {
            return (bool)element.GetValue(HandleTaskbarProperty);
        }

        private static void SetHandleTaskbar(UIElement element, bool value)
        {
            element.SetValue(HandleTaskbarProperty, value);
        }

        private void AssociatedObject_SourceInitialized(object sender, EventArgs e)
        {
            this.handle = new WindowInteropHelper(this.AssociatedObject).Handle;
            if (IntPtr.Zero == this.handle)
            {
                throw new MahAppsException("Uups, at this point we really need the Handle from the associated object!");
            }

            if (this.AssociatedObject.SizeToContent != SizeToContent.Manual && this.AssociatedObject.WindowState == WindowState.Normal)
            {
                // Another try to fix SizeToContent
                // without this we get nasty glitches at the borders
                this.AssociatedObject.Invoke(() =>
                    {
                        this.AssociatedObject.InvalidateMeasure();
                        Native.RECT rect;
                        if (UnsafeNativeMethods.GetWindowRect(this.handle, out rect))
                        {
                            UnsafeNativeMethods.SetWindowPos(this.handle, new IntPtr(-2), rect.left, rect.top, rect.Width, rect.Height, 0x0040);
                        }
                    });
            }

            this.hwndSource = HwndSource.FromHwnd(this.handle);
            this.hwndSource?.AddHook(this.WindowProc);

            // handle the maximized state here too (to handle the border in a correct way)
            this.HandleMaximize();
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = sender as MetroWindow;
            if (window == null)
            {
                return;
            }

            if (window.ResizeMode != ResizeMode.NoResize)
            {
                //window.SetIsHitTestVisibleInChromeProperty<Border>("PART_Border");
                window.SetIsHitTestVisibleInChromeProperty<UIElement>("PART_Icon");
                window.SetWindowChromeResizeGripDirection("WindowResizeGrip", ResizeGripDirection.BottomRight);
            }
        }

        [Obsolete(@"This property will be deleted in the next release. You should use BorderThickness=""0"" and a GlowBrush=""Black"" properties in your Window to get a drop shadow around it.")]
        public static readonly DependencyProperty EnableDWMDropShadowProperty = DependencyProperty.Register("EnableDWMDropShadow", typeof(bool), typeof(BorderlessWindowBehavior), new PropertyMetadata(false));

        [Obsolete(@"This property will be deleted in the next release. You should use BorderThickness=""0"" and a GlowBrush=""Black"" properties in your Window to get a drop shadow around it.")]
        public bool EnableDWMDropShadow
        {
            get { return (bool)this.GetValue(EnableDWMDropShadowProperty); }
            set { this.SetValue(EnableDWMDropShadowProperty, value); }
        }
    }
}