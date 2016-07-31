using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using MahApps.Metro.Models.Win32;
using MahApps.Metro.Native;
using NativeMethods = MahApps.Metro.Models.Win32.NativeMethods;
using RECT = MahApps.Metro.Native.RECT;
using SWP = MahApps.Metro.Models.Win32.SWP;

namespace MahApps.Metro.Controls
{
    partial class GlowWindow : Window
    {
        private readonly Func<Point, RECT, Cursor> getCursor;
        private readonly Func<Point, RECT, HitTestValues> getHitTestValue;
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
                    this.getLeft = (rect) => rect.left - glowSize + 1;
                    this.getTop = (rect) => rect.top - 2;
                    this.getWidth = (rect) => glowSize;
                    this.getHeight = (rect) => rect.Height + 4;
                    this.getHitTestValue = (p, rect) => new Rect(0, 0, rect.Width, edgeSize).Contains(p)
                        ? HitTestValues.HTTOPLEFT
                        : new Rect(0, rect.Height + 4 - edgeSize, rect.Width, edgeSize).Contains(p)
                            ? HitTestValues.HTBOTTOMLEFT
                            : HitTestValues.HTLEFT;
                    this.getCursor = (p, rect) => new Rect(0, 0, rect.Width, edgeSize).Contains(p)
                        ? Cursors.SizeNWSE
                        : new Rect(0, rect.Height + 4 - edgeSize, rect.Width, edgeSize).Contains(p)
                            ? Cursors.SizeNESW
                            : Cursors.SizeWE;
                    break;
                case GlowDirection.Right:
                    this.glow.Orientation = Orientation.Vertical;
                    this.glow.HorizontalAlignment = HorizontalAlignment.Left;
                    this.getLeft = (rect) => rect.right - 1;
                    this.getTop = (rect) => rect.top - 2;
                    this.getWidth = (rect) => glowSize;
                    this.getHeight = (rect) => rect.Height + 4;
                    this.getHitTestValue = (p, rect) => new Rect(0, 0, rect.Width, edgeSize).Contains(p)
                        ? HitTestValues.HTTOPRIGHT
                        : new Rect(0, rect.Height + 4 - edgeSize, rect.Width, edgeSize).Contains(p)
                            ? HitTestValues.HTBOTTOMRIGHT
                            : HitTestValues.HTRIGHT;
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
                                Standard.NativeMethods.SendMessage(this.ownerHandle, Standard.WM.NCLBUTTONDBLCLK, (IntPtr)Standard.HT.TOP, IntPtr.Zero);
                            }
                        };
                    this.glow.Orientation = Orientation.Horizontal;
                    this.glow.VerticalAlignment = VerticalAlignment.Bottom;
                    this.getLeft = (rect) => rect.left - 2;
                    this.getTop = (rect) => rect.top - glowSize + 1;
                    this.getWidth = (rect) => rect.Width + 4;
                    this.getHeight = (rect) => glowSize;
                    this.getHitTestValue = (p, rect) => new Rect(0, 0, edgeSize - glowSize, rect.Height).Contains(p)
                        ? HitTestValues.HTTOPLEFT
                        : new Rect(rect.Width + 4 - edgeSize + glowSize, 0, edgeSize - glowSize, rect.Height).Contains(p)
                            ? HitTestValues.HTTOPRIGHT
                            : HitTestValues.HTTOP;
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
                                Standard.NativeMethods.SendMessage(this.ownerHandle, Standard.WM.NCLBUTTONDBLCLK, (IntPtr)Standard.HT.BOTTOM, IntPtr.Zero);
                            }
                        };
                    this.glow.Orientation = Orientation.Horizontal;
                    this.glow.VerticalAlignment = VerticalAlignment.Top;
                    this.getLeft = (rect) => rect.left - 2;
                    this.getTop = (rect) => rect.bottom - 1;
                    this.getWidth = (rect) => rect.Width + 4;
                    this.getHeight = (rect) => glowSize;
                    this.getHitTestValue = (p, rect) => new Rect(0, 0, edgeSize - glowSize, rect.Height).Contains(p)
                        ? HitTestValues.HTBOTTOMLEFT
                        : new Rect(rect.Width + 4 - edgeSize + glowSize, 0, edgeSize - glowSize, rect.Height).Contains(p)
                            ? HitTestValues.HTBOTTOMRIGHT
                            : HitTestValues.HTBOTTOM;
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

            var ws = this.hwndSource.Handle.GetWindowLong();
            var wsex = this.hwndSource.Handle.GetWindowLongEx();

            ws |= WS.POPUP;

            wsex &= ~WSEX.APPWINDOW;
            wsex |= WSEX.TOOLWINDOW;

            if (this._owner.ResizeMode == ResizeMode.NoResize || this._owner.ResizeMode == ResizeMode.CanMinimize)
            {
                wsex |= WSEX.TRANSPARENT;
            }

            this.hwndSource.Handle.SetWindowLong(ws);
            this.hwndSource.Handle.SetWindowLongEx(wsex);
            this.hwndSource.AddHook(this.WndProc);

            this.handle = this.hwndSource.Handle;
            this.ownerHandle = new WindowInteropHelper(this._owner).Handle;

            this.resizeModeChangeNotifier = new PropertyChangeNotifier(this._owner, ResizeModeProperty);
            this.resizeModeChangeNotifier.ValueChanged += this.ResizeModeChanged;
        }

        private void ResizeModeChanged(object sender, EventArgs e)
        {
            var wsex = this.hwndSource.Handle.GetWindowLongEx();
            if (this._owner.ResizeMode == ResizeMode.NoResize || this._owner.ResizeMode == ResizeMode.CanMinimize)
            {
                wsex |= WSEX.TRANSPARENT;
            }
            else
            {
                wsex ^= WSEX.TRANSPARENT;
            }
            this.hwndSource.Handle.SetWindowLongEx(wsex);
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

        internal void UpdateCore(Native.RECT rect)
        {
            NativeMethods.SetWindowPos(this.handle, this.ownerHandle,
                                       (int)(this.getLeft(rect)),
                                       (int)(this.getTop(rect)),
                                       (int)(this.getWidth(rect)),
                                       (int)(this.getHeight(rect)),
                                       SWP.NOACTIVATE);
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
                        Standard.NativeMethods.SendMessage(this.ownerHandle, Standard.WM.ACTIVATE, wParam, lParam);
                    }
                    return new IntPtr(3);
                case WM.LBUTTONDOWN:
                    if (this.ownerHandle != IntPtr.Zero && UnsafeNativeMethods.GetWindowRect(this.ownerHandle, out rect))
                    {
                        var pt = this.GetRelativeMousePosition();
                        NativeMethods.PostMessage(this.ownerHandle, (uint)WM.NCLBUTTONDOWN, (IntPtr)this.getHitTestValue(pt, rect), IntPtr.Zero);
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
                            var pt = this.GetRelativeMousePosition();
                            cursor = this.getCursor(pt, rect);
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

        private Point GetRelativeMousePosition()
        {
            if (this.handle == IntPtr.Zero)
            {
                return new Point();
            }
            var point = Standard.NativeMethods.GetCursorPos();
            Standard.NativeMethods.ScreenToClient(this.handle, ref point);
            return new Point(point.x, point.y);
        }
    }
}