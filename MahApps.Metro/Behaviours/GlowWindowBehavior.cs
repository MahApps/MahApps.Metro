using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Interop;
using MahApps.Metro.Controls;
using System.Windows.Threading;
using Standard;
using WM = MahApps.Metro.Models.Win32.WM;

namespace MahApps.Metro.Behaviours
{
    public class GlowWindowBehavior : Behavior<Window>
    {
        private static readonly TimeSpan GlowTimerDelay = TimeSpan.FromMilliseconds(200); //200 ms delay, the same as VS2013
        private GlowWindow left, right, top, bottom;
        private DispatcherTimer makeGlowVisibleTimer;
        private IntPtr handle;

        private bool IsGlowDisabled
        {
            get
            {
                var metroWindow = this.AssociatedObject as MetroWindow;
                return metroWindow != null && metroWindow.GlowBrush == null;
            }
        }

        private bool IsWindowTransitionsEnabled
        {
            get
            {
                var metroWindow = this.AssociatedObject as MetroWindow;
                return metroWindow != null && metroWindow.WindowTransitionsEnabled;
            }
        }
        
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.SourceInitialized += (o, args) =>
                {
                    this.handle = new WindowInteropHelper(this.AssociatedObject).Handle;
                    var hwndSource = HwndSource.FromHwnd(this.handle);
                    hwndSource?.AddHook(this.AssociatedObjectWindowProc);
                };
            this.AssociatedObject.Loaded += this.AssociatedObjectOnLoaded;
            this.AssociatedObject.Unloaded += this.AssociatedObjectUnloaded;
        }

        private void AssociatedObjectStateChanged(object sender, EventArgs e)
        {
            this.makeGlowVisibleTimer?.Stop();
            if(this.AssociatedObject.WindowState == WindowState.Normal)
            {
                var metroWindow = this.AssociatedObject as MetroWindow;
                var ignoreTaskBar = metroWindow != null && metroWindow.IgnoreTaskbarOnMaximize;
                if (this.makeGlowVisibleTimer != null && SystemParameters.MinimizeAnimation && !ignoreTaskBar)
                {
                    this.makeGlowVisibleTimer.Start();
                }
                else
                {
                    this.RestoreGlow();
                }
            }
            else
            {
                this.HideGlow();
            }
        }

        private void AssociatedObjectUnloaded(object sender, RoutedEventArgs e)
        {
            if (this.makeGlowVisibleTimer == null)
            {
                return;
            }
            this.makeGlowVisibleTimer.Stop();
            this.makeGlowVisibleTimer.Tick -= this.GlowVisibleTimerOnTick;
            this.makeGlowVisibleTimer = null;
        }

        private void GlowVisibleTimerOnTick(object sender, EventArgs e)
        {
            this.makeGlowVisibleTimer?.Stop();
            this.RestoreGlow();
        }

        private void RestoreGlow()
        {
            if (this.left != null) this.left.IsGlowing = true;
            if (this.top != null) this.top.IsGlowing = true;
            if (this.right != null) this.right.IsGlowing = true;
            if (this.bottom != null) this.bottom.IsGlowing = true;
            this.Update();
        }

        private void HideGlow()
        {
            if (this.left != null) this.left.IsGlowing = false;
            if (this.top != null) this.top.IsGlowing = false;
            if (this.right != null) this.right.IsGlowing = false;
            if (this.bottom != null) this.bottom.IsGlowing = false;
            this.Update();
        }

        private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            // No glow effect if GlowBrush not set.
            if (this.IsGlowDisabled)
            {
                return;
            }

            this.AssociatedObject.StateChanged -= this.AssociatedObjectStateChanged;
            this.AssociatedObject.StateChanged += this.AssociatedObjectStateChanged;

            if (this.makeGlowVisibleTimer == null)
            {
                this.makeGlowVisibleTimer = new DispatcherTimer { Interval = GlowTimerDelay };
                this.makeGlowVisibleTimer.Tick += this.GlowVisibleTimerOnTick;
            }

            this.left = new GlowWindow(this.AssociatedObject, GlowDirection.Left);
            this.right = new GlowWindow(this.AssociatedObject, GlowDirection.Right);
            this.top = new GlowWindow(this.AssociatedObject, GlowDirection.Top);
            this.bottom = new GlowWindow(this.AssociatedObject, GlowDirection.Bottom);

            this.Show();
            this.Update();

            if (!this.IsWindowTransitionsEnabled)
            {
                // no storyboard so set opacity to 1
                this.AssociatedObject.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => this.SetOpacityTo(1)));
            }
            else
            {
                // start the opacity storyboard 0->1
                this.StartOpacityStoryboard();
                // hide the glows if window get invisible state
                this.AssociatedObject.IsVisibleChanged += this.AssociatedObjectIsVisibleChanged;
                // closing always handled
                this.AssociatedObject.Closing += (o, args) =>
                {
                    if (!args.Cancel)
                    {
                        this.AssociatedObject.IsVisibleChanged -= this.AssociatedObjectIsVisibleChanged;
                    }
                };
            }
        }

        private WINDOWPOS prevWindowPos;

        private IntPtr AssociatedObjectWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ((WM)msg)
            {
                case WM.WINDOWPOSCHANGED:
                case WM.WINDOWPOSCHANGING:
                    Assert.IsNotDefault(lParam);
                    var wp = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                    if (!wp.Equals(this.prevWindowPos))
                    {
                        this.UpdateCore();
                    }
                    this.prevWindowPos = wp;
                    break;
                case WM.SIZE:
                case WM.SIZING:
                    this.UpdateCore();
                    break;
            }
            return IntPtr.Zero;
        }

        private void AssociatedObjectIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!this.AssociatedObject.IsVisible)
            {
                // the associated owner got invisible so set opacity to 0 to start the storyboard by 0 for the next visible state
                this.SetOpacityTo(0);
            }
            else
            {
                this.StartOpacityStoryboard();
            }
        }

        /// <summary>
        /// Updates all glow windows (visible, hidden, collapsed)
        /// </summary>
        private void Update()
        {
            this.left?.Update();
            this.right?.Update();
            this.top?.Update();
            this.bottom?.Update();
        }

        private void UpdateCore()
        {
            Native.RECT rect;
            if (this.handle != IntPtr.Zero && Native.UnsafeNativeMethods.GetWindowRect(this.handle, out rect))
            {
                this.left?.UpdateCore(rect);
                this.right?.UpdateCore(rect);
                this.top?.UpdateCore(rect);
                this.bottom?.UpdateCore(rect);
            }
        }

        /// <summary>
        /// Sets the opacity to all glow windows
        /// </summary>
        private void SetOpacityTo(double newOpacity)
        {
            if (this.left != null) this.left.Opacity = newOpacity;
            if (this.right != null) this.right.Opacity = newOpacity;
            if (this.top != null) this.top.Opacity = newOpacity;
            if (this.bottom != null) this.bottom.Opacity = newOpacity;
        }

        /// <summary>
        /// Starts the opacity storyboard 0 -> 1
        /// </summary>
        private void StartOpacityStoryboard()
        {
            if (this.left?.OpacityStoryboard != null) this.left.BeginStoryboard(this.left.OpacityStoryboard);
            if (this.right?.OpacityStoryboard != null) this.right.BeginStoryboard(this.right.OpacityStoryboard);
            if (this.top?.OpacityStoryboard != null) this.top.BeginStoryboard(this.top.OpacityStoryboard);
            if (this.bottom?.OpacityStoryboard != null) this.bottom.BeginStoryboard(this.bottom.OpacityStoryboard);
        }

        /// <summary>
        /// Shows all glow windows
        /// </summary>
        private void Show()
        {
            this.left?.Show();
            this.right?.Show();
            this.top?.Show();
            this.bottom?.Show();
        }
    }
}
