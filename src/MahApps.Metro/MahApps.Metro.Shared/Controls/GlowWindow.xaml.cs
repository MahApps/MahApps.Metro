#pragma warning disable 618
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using ControlzEx;
using ControlzEx.Standard;
using ControlzEx.Native;

namespace MahApps.Metro.Controls
{
    partial class GlowWindow : Window
    {
        private readonly Func<Point, RECT, Cursor> getCursor;
        private readonly Func<Point, RECT, HT> getHitTestValue;
        private readonly Func<RECT, double> getLeft;
        private readonly Func<RECT, double> getTop;
        private readonly Func<RECT, double> getWidth;
        private readonly Func<RECT, double> getHeight;
        private const double edgeSize = 20.0;
        private const double glowSize = 6.0;
        private IntPtr handle;
        private IntPtr ownerHandle;
        private bool closing = false;
        private HwndSource hwndSource;
        private PropertyChangeNotifier resizeModeChangeNotifier;

        private Window _owner;

        public GlowWindow(Window owner, GlowDirection direction)
        {
            this.InitializeComponent();

            this.Owner = owner;
            this._owner = owner;

            this.IsGlowing = true;
            this.AllowsTransparency = true;
            this.Closing += (sender, e) => e.Cancel = !this.closing;

            this.ShowInTaskbar = false;
            this.glow.Visibility = Visibility.Collapsed;

            var b = new Binding("GlowBrush");
            b.Source = owner;
            this.glow.SetBinding(Glow.GlowBrushProperty, b);

            b = new Binding("NonActiveGlowBrush");
            b.Source = owner;
            this.glow.SetBinding(Glow.NonActiveGlowBrushProperty, b);

            b = new Binding("BorderThickness");
            b.Source = owner;
            this.glow.SetBinding(BorderThicknessProperty, b);

            this.glow.Direction = direction;

            switch (direction)
            {
                case GlowDirection.Left:
                    this.glow.Orientation = Orientation.Vertical;
                    this.glow.HorizontalAlignment = HorizontalAlignment.Right;
                    this.getLeft = (rect) => rect.Left - glowSize + 1;
                    this.getTop = (rect) => rect.Top - 2;
                    this.getWidth = (rect) => glowSize;
                    this.getHeight = (rect) => rect.Height + 4;
                    this.getHitTestValue = (p, rect) => new Rect(0, 0, rect.Width, edgeSize).Contains(p)
                        ? HT.TOPLEFT
                        : new Rect(0, rect.Height + 4 - edgeSize, rect.Width, edgeSize).Contains(p)
                            ? HT.BOTTOMLEFT
                            : HT.LEFT;
                    this.getCursor = (p, rect) => new Rect(0, 0, rect.Width, edgeSize).Contains(p)
                        ? Cursors.SizeNWSE
                        : new Rect(0, rect.Height + 4 - edgeSize, rect.Width, edgeSize).Contains(p)
                            ? Cursors.SizeNESW
                            : Cursors.SizeWE;
                    break;
                case GlowDirection.Right:
                    this.glow.Orientation = Orientation.Vertical;
                    this.glow.HorizontalAlignment = HorizontalAlignment.Left;
                    this.getLeft = (rect) => rect.Right - 1;
                    this.getTop = (rect) => rect.Top - 2;
                    this.getWidth = (rect) => glowSize;
                    this.getHeight = (rect) => rect.Height + 4;
                    this.getHitTestValue = (p, rect) => new Rect(0, 0, rect.Width, edgeSize).Contains(p)
                        ? HT.TOPRIGHT
                        : new Rect(0, rect.Height + 4 - edgeSize, rect.Width, edgeSize).Contains(p)
                            ? HT.BOTTOMRIGHT
                            : HT.RIGHT;
                    this.getCursor = (p, rect) => new Rect(0, 0, rect.Width, edgeSize).Contains(p)
                        ? Cursors.SizeNESW
                        : new Rect(0, rect.Height + 4 - edgeSize, rect.Width, edgeSize).Contains(p)
                            ? Cursors.SizeNWSE
                            : Cursors.SizeWE;
                    break;

                case GlowDirection.Top:
                    this.PreviewMouseDoubleClick += (sender, e) =>
                        {
                            if (this.ownerHandle != IntPtr.Zero)
                            {
                                NativeMethods.SendMessage(this.ownerHandle, WM.NCLBUTTONDBLCLK, (IntPtr)HT.TOP, IntPtr.Zero);
                            }
                        };
                    this.glow.Orientation = Orientation.Horizontal;
                    this.glow.VerticalAlignment = VerticalAlignment.Bottom;
                    this.getLeft = (rect) => rect.Left - 2;
                    this.getTop = (rect) => rect.Top - glowSize + 1;
                    this.getWidth = (rect) => rect.Width + 4;
                    this.getHeight = (rect) => glowSize;
                    this.getHitTestValue = (p, rect) => new Rect(0, 0, edgeSize - glowSize, rect.Height).Contains(p)
                        ? HT.TOPLEFT
                        : new Rect(rect.Width + 4 - edgeSize + glowSize, 0, edgeSize - glowSize, rect.Height).Contains(p)
                            ? HT.TOPRIGHT
                            : HT.TOP;
                    this.getCursor = (p, rect) => new Rect(0, 0, edgeSize - glowSize, rect.Height).Contains(p)
                        ? Cursors.SizeNWSE
                        : new Rect(rect.Width + 4 - edgeSize + glowSize, 0, edgeSize - glowSize, rect.Height).Contains(p)
                            ? Cursors.SizeNESW
                            : Cursors.SizeNS;
                    break;
                case GlowDirection.Bottom:
                    this.PreviewMouseDoubleClick += (sender, e) =>
                        {
                            if (this.ownerHandle != IntPtr.Zero)
                            {
                                NativeMethods.SendMessage(this.ownerHandle, WM.NCLBUTTONDBLCLK, (IntPtr)HT.BOTTOM, IntPtr.Zero);
                            }
                        };
                    this.glow.Orientation = Orientation.Horizontal;
                    this.glow.VerticalAlignment = VerticalAlignment.Top;
                    this.getLeft = (rect) => rect.Left - 2;
                    this.getTop = (rect) => rect.Bottom - 1;
                    this.getWidth = (rect) => rect.Width + 4;
                    this.getHeight = (rect) => glowSize;
                    this.getHitTestValue = (p, rect) => new Rect(0, 0, edgeSize - glowSize, rect.Height).Contains(p)
                        ? HT.BOTTOMLEFT
                        : new Rect(rect.Width + 4 - edgeSize + glowSize, 0, edgeSize - glowSize, rect.Height).Contains(p)
                            ? HT.BOTTOMRIGHT
                            : HT.BOTTOM;
                    this.getCursor = (p, rect) => new Rect(0, 0, edgeSize - glowSize, rect.Height).Contains(p)
                        ? Cursors.SizeNESW
                        : new Rect(rect.Width + 4 - edgeSize + glowSize, 0, edgeSize - glowSize, rect.Height).Contains(p)
                            ? Cursors.SizeNWSE
                            : Cursors.SizeNS;
                    break;
            }

            owner.ContentRendered += (sender, e) => this.glow.Visibility = Visibility.Visible;
            owner.Activated += (sender, e) =>
                {
                    this.Update();
                    this.glow.IsGlow = true;
                };
            owner.Deactivated += (sender, e) =>
                {
                    this.glow.IsGlow = false;
                };
            owner.StateChanged += (sender, e) => this.Update();
            owner.IsVisibleChanged += (sender, e) => this.Update();
            owner.Closed += (sender, e) =>
                {
                    this.closing = true;
                    this.Close();
                };
        }

        public Storyboard OpacityStoryboard { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.OpacityStoryboard = this.TryFindResource("OpacityStoryboard") as Storyboard;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            this.hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            if (this.hwndSource == null) return;

            var ws = NativeMethods.GetWindowStyle(this.hwndSource.Handle);
            var wsex = NativeMethods.GetWindowStyleEx(this.hwndSource.Handle);

            ws |= WS.POPUP;

            wsex &= ~WS_EX.APPWINDOW;
            wsex |= WS_EX.TOOLWINDOW;

            if (this._owner.ResizeMode == ResizeMode.NoResize || this._owner.ResizeMode == ResizeMode.CanMinimize)
            {
                wsex |= WS_EX.TRANSPARENT;
            }

            NativeMethods.SetWindowStyle(this.hwndSource.Handle, ws);
            NativeMethods.SetWindowStyleEx(this.hwndSource.Handle, wsex);
            this.hwndSource.AddHook(this.WndProc);

            this.handle = this.hwndSource.Handle;
            this.ownerHandle = new WindowInteropHelper(this._owner).Handle;

            this.resizeModeChangeNotifier = new PropertyChangeNotifier(this._owner, ResizeModeProperty);
            this.resizeModeChangeNotifier.ValueChanged += this.ResizeModeChanged;
        }

        private void ResizeModeChanged(object sender, EventArgs e)
        {
            var wsex = NativeMethods.GetWindowStyleEx(this.hwndSource.Handle);
            if (this._owner.ResizeMode == ResizeMode.NoResize || this._owner.ResizeMode == ResizeMode.CanMinimize)
            {
                wsex |= WS_EX.TRANSPARENT;
            }
            else
            {
                wsex ^= WS_EX.TRANSPARENT;
            }
            NativeMethods.SetWindowStyleEx(this.hwndSource.Handle, wsex);
        }

        public void Update()
        {
            if (this.closing)
            {
                return;
            }

            RECT rect;
            if (this._owner.Visibility == Visibility.Hidden)
            {
                this.Invoke(() => this.glow.Visibility = Visibility.Collapsed);
                this.Invoke(() => this.Visibility = Visibility.Collapsed);
                //Standard.NativeMethods.ShowWindow(this.handle, Standard.SW.HIDE);
                if (this.IsGlowing && this.ownerHandle != IntPtr.Zero && UnsafeNativeMethods.GetWindowRect(this.ownerHandle, out rect))
                {
                    this.UpdateCore(rect);
                }
            }
            else if (this._owner.WindowState == WindowState.Normal)
            {
                this.Invoke(() => this.glow.Visibility = this.IsGlowing ? Visibility.Visible : Visibility.Collapsed);
                this.Invoke(() => this.Visibility = this.IsGlowing ? Visibility.Visible : Visibility.Collapsed);
//                if (this.IsGlowing)
//                {
//                    Standard.NativeMethods.ShowWindow(this.handle, Standard.SW.SHOWNOACTIVATE);
//                }
//                else
//                {
//                    Standard.NativeMethods.ShowWindow(this.handle, Standard.SW.HIDE);
//                }
                if (this.IsGlowing && this.ownerHandle != IntPtr.Zero && UnsafeNativeMethods.GetWindowRect(this.ownerHandle, out rect))
                {
                    this.UpdateCore(rect);
                }
            }
            else
            {
                this.Invoke(() => this.glow.Visibility = Visibility.Collapsed);
                this.Invoke(() => this.Visibility = Visibility.Collapsed);
                //Standard.NativeMethods.ShowWindow(this.handle, Standard.SW.HIDE);
            }
        }

        public bool IsGlowing { set; get; }

        internal bool CanUpdateCore()
        {
            return this.ownerHandle != IntPtr.Zero && this.handle != IntPtr.Zero;
        }

        internal void UpdateCore(RECT rect)
        {
            // we can handle this._owner.WindowState == WindowState.Normal
            // or use NOZORDER too
            NativeMethods.SetWindowPos(this.handle, this.ownerHandle,
                                       (int)(this.getLeft(rect)),
                                       (int)(this.getTop(rect)),
                                       (int)(this.getWidth(rect)),
                                       (int)(this.getHeight(rect)),
                                       SWP.NOACTIVATE | SWP.NOZORDER);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            RECT rect;
            switch ((WM)msg)
            {
                case WM.SHOWWINDOW:
                    if ((int)lParam == 3 && this.Visibility != Visibility.Visible) // 3 == SW_PARENTOPENING
                    {
                        handled = true; //handle this message so window isn't shown until we want it to       
                    }
                    break;
                case WM.MOUSEACTIVATE:
                    handled = true;
                    if (this.ownerHandle != IntPtr.Zero)
                    {
                        NativeMethods.SendMessage(this.ownerHandle, WM.ACTIVATE, wParam, lParam);
                    }
                    return new IntPtr(3);
                case WM.LBUTTONDOWN:
                    if (this.ownerHandle != IntPtr.Zero && UnsafeNativeMethods.GetWindowRect(this.ownerHandle, out rect))
                    {
                        Point pt;
                        if (WinApiHelper.TryGetRelativeMousePosition(this.handle, out pt))
                        {
                            NativeMethods.PostMessage(this.ownerHandle, WM.NCLBUTTONDOWN, (IntPtr)this.getHitTestValue(pt, rect), IntPtr.Zero);
                        }
                    }
                    break;
                case WM.NCHITTEST:
                    Cursor cursor = null;
                    if (this._owner.ResizeMode == ResizeMode.NoResize || this._owner.ResizeMode == ResizeMode.CanMinimize)
                    {
                        cursor = this._owner.Cursor;
                    }
                    else
                    {
                        if (this.ownerHandle != IntPtr.Zero && UnsafeNativeMethods.GetWindowRect(this.ownerHandle, out rect))
                        {
                            Point pt;
                            if (WinApiHelper.TryGetRelativeMousePosition(this.handle, out pt))
                            {
                                cursor = this.getCursor(pt, rect);
                            }
                        }
                    }
                    if (cursor != null && cursor != this.Cursor)
                    {
                        this.Cursor = cursor;
                    }
                    break;
            }
            return IntPtr.Zero;
        }
    }
}