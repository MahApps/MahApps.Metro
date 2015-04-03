using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using MahApps.Metro.Models.Win32;
using MahApps.Metro.Native;
using Standard;
using NativeMethods = MahApps.Metro.Models.Win32.NativeMethods;
using RECT = MahApps.Metro.Native.RECT;
using SWP = MahApps.Metro.Models.Win32.SWP;
using WM = MahApps.Metro.Models.Win32.WM;
using WS = MahApps.Metro.Models.Win32.WS;

namespace MahApps.Metro.Controls
{
    partial class GlowWindow : Window
    {
        private readonly Func<Point, Cursor> getCursor;
        private readonly Func<Point, HitTestValues> getHitTestValue;
        private readonly Func<double, RECT, double> getLeft;
        private readonly Func<double, RECT, double> getTop;
        private readonly Func<double, RECT, double> getWidth;
        private readonly Func<double, RECT, double> getHeight;
        private const double edgeSize = 20.0;
        private const double glowSize = 9.0;
        private IntPtr handle;
        private IntPtr ownerHandle;
        private static double? _dpiFactor = null;
        private bool closing = false;

        public GlowWindow(Window owner, GlowDirection direction)
        {
            InitializeComponent();

            this.IsGlowing = true;
            this.AllowsTransparency = true;
            this.Closing += (sender, e) => e.Cancel = !closing;

            this.Owner = owner;
            glow.Visibility = Visibility.Collapsed;

            var b = new Binding("GlowBrush");
            b.Source = owner;
            glow.SetBinding(Glow.GlowBrushProperty, b);

            b = new Binding("NonActiveGlowBrush");
            b.Source = owner;
            glow.SetBinding(Glow.NonActiveGlowBrushProperty, b);

            b = new Binding("BorderThickness");
            b.Source = owner;
            glow.SetBinding(Glow.BorderThicknessProperty, b);

            switch (direction)
            {
                case GlowDirection.Left:
                    glow.Orientation = Orientation.Vertical;
                    glow.HorizontalAlignment = HorizontalAlignment.Right;
                    getLeft = (dpi, rect) => Math.Round((rect.left - glowSize) * dpi);
                    getTop = (dpi, rect) => Math.Round((rect.top - glowSize) * dpi);
                    getWidth = (dpi, rect) => glowSize * dpi;
                    getHeight = (dpi, rect) => Math.Round((rect.Height + glowSize * 2.0) * dpi);
                    getHitTestValue = p => new Rect(0, 0, ActualWidth, edgeSize).Contains(p)
                                               ? HitTestValues.HTTOPLEFT
                                               : new Rect(0, ActualHeight - edgeSize, ActualWidth, edgeSize).Contains(p)
                                                     ? HitTestValues.HTBOTTOMLEFT
                                                     : HitTestValues.HTLEFT;
                    getCursor = p =>
                    {
                        return (owner.ResizeMode == ResizeMode.NoResize || owner.ResizeMode == ResizeMode.CanMinimize)
                                    ? owner.Cursor
                                    : new Rect(0, 0, ActualWidth, edgeSize).Contains(p)
                                         ? Cursors.SizeNWSE
                                         : new Rect(0, ActualHeight - edgeSize, ActualWidth, edgeSize).Contains(p)
                                               ? Cursors.SizeNESW
                                               : Cursors.SizeWE;
                    };
                    break;
                case GlowDirection.Right:
                    glow.Orientation = Orientation.Vertical;
                    glow.HorizontalAlignment = HorizontalAlignment.Left;
                    getLeft = (dpi, rect) => Math.Round(rect.right * dpi);
                    getTop = (dpi, rect) => Math.Round((rect.top - glowSize) * dpi);
                    getWidth = (dpi, rect) => glowSize * dpi;
                    getHeight = (dpi, rect) => Math.Round((rect.Height + glowSize * 2.0) * dpi);
                    getHitTestValue = p => new Rect(0, 0, ActualWidth, edgeSize).Contains(p)
                                               ? HitTestValues.HTTOPRIGHT
                                               : new Rect(0, ActualHeight - edgeSize, ActualWidth, edgeSize).Contains(p)
                                                     ? HitTestValues.HTBOTTOMRIGHT
                                                     : HitTestValues.HTRIGHT;
                    getCursor = p =>
                    {
                        return (owner.ResizeMode == ResizeMode.NoResize || owner.ResizeMode == ResizeMode.CanMinimize)
                                    ? owner.Cursor
                                    : new Rect(0, 0, ActualWidth, edgeSize).Contains(p)
                                         ? Cursors.SizeNESW
                                         : new Rect(0, ActualHeight - edgeSize, ActualWidth, edgeSize).Contains(p)
                                               ? Cursors.SizeNWSE
                                               : Cursors.SizeWE;
                    };
                    break;
                case GlowDirection.Top:
                    glow.Orientation = Orientation.Horizontal;
                    glow.VerticalAlignment = VerticalAlignment.Bottom;
                    getLeft = (dpi, rect) => Math.Round(rect.left * dpi);
                    getTop = (dpi, rect) => Math.Round((rect.top - glowSize) * dpi);
                    getWidth = (dpi, rect) => Math.Round(rect.Width * dpi);
                    getHeight = (dpi, rect) => glowSize * dpi;
                    getHitTestValue = p => new Rect(0, 0, edgeSize - glowSize, ActualHeight).Contains(p)
                                               ? HitTestValues.HTTOPLEFT
                                               : new Rect(Width - edgeSize + glowSize, 0, edgeSize - glowSize,
                                                          ActualHeight).Contains(p)
                                                     ? HitTestValues.HTTOPRIGHT
                                                     : HitTestValues.HTTOP;
                    getCursor = p =>
                    {
                        return (owner.ResizeMode == ResizeMode.NoResize || owner.ResizeMode == ResizeMode.CanMinimize)
                                    ? owner.Cursor
                                    : new Rect(0, 0, edgeSize - glowSize, ActualHeight).Contains(p)
                                         ? Cursors.SizeNWSE
                                         : new Rect(Width - edgeSize + glowSize, 0, edgeSize - glowSize, ActualHeight).
                                               Contains(p)
                                               ? Cursors.SizeNESW
                                               : Cursors.SizeNS;
                    };
                    break;
                case GlowDirection.Bottom:
                    glow.Orientation = Orientation.Horizontal;
                    glow.VerticalAlignment = VerticalAlignment.Top;
                    getLeft = (dpi, rect) => Math.Round(rect.left * dpi);
                    getTop = (dpi, rect) => Math.Round(rect.bottom * dpi);
                    getWidth = (dpi, rect) => Math.Round(rect.Width * dpi);
                    getHeight = (dpi, rect) => glowSize * dpi;
                    getHitTestValue = p => new Rect(0, 0, edgeSize - glowSize, ActualHeight).Contains(p)
                                               ? HitTestValues.HTBOTTOMLEFT
                                               : new Rect(Width - edgeSize + glowSize, 0, edgeSize - glowSize,
                                                          ActualHeight).Contains(p)
                                                     ? HitTestValues.HTBOTTOMRIGHT
                                                     : HitTestValues.HTBOTTOM;
                    getCursor = p =>
                    {
                        return (owner.ResizeMode == ResizeMode.NoResize || owner.ResizeMode == ResizeMode.CanMinimize)
                                    ? owner.Cursor
                                    : new Rect(0, 0, edgeSize - glowSize, ActualHeight).Contains(p)
                                         ? Cursors.SizeNESW
                                         : new Rect(Width - edgeSize + glowSize, 0, edgeSize - glowSize, ActualHeight).
                                               Contains(p)
                                               ? Cursors.SizeNWSE
                                               : Cursors.SizeNS;
                    };
                    break;
            }

            owner.ContentRendered += (sender, e) => glow.Visibility = Visibility.Visible;
            owner.Activated += (sender, e) =>
            {
                Update();
                glow.IsGlow = true;
            };
            owner.Deactivated += (sender, e) => glow.IsGlow = false;
            //owner.LocationChanged += (sender, e) => Update();
            //owner.SizeChanged += (sender, e) => Update();
            owner.StateChanged += (sender, e) => Update();
            owner.IsVisibleChanged += (sender, e) => Update();
            owner.Closed += (sender, e) =>
            {
                closing = true;
                Close();
            };
        }

        public double DpiFactor
        {
            get
            {
                if (_dpiFactor == null)
                {
                    double dpiX = 96.0, dpiY = 96.0;

                    // #652, #752 check if Owner not null
                    var owner = this.Owner ?? (Application.Current != null ? Application.Current.MainWindow : null);
                    var source = owner != null ? PresentationSource.FromVisual(owner) : null;
                    if (source != null && source.CompositionTarget != null)
                    {
                        dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                        dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
                    }

                    _dpiFactor = dpiX == dpiY ? dpiX / 96.0 : 1;
                }
                return _dpiFactor.Value;
            }

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

            var source = (HwndSource)PresentationSource.FromVisual(this);
            WS ws = source.Handle.GetWindowLong();
            WSEX wsex = source.Handle.GetWindowLongEx();

            //ws |= WS.POPUP;
            wsex ^= WSEX.APPWINDOW;
            wsex |= WSEX.NOACTIVATE;
            wsex |= WSEX.TRANSPARENT;

            source.Handle.SetWindowLong(ws);
            source.Handle.SetWindowLongEx(wsex);
            source.AddHook(WndProc);

            handle = source.Handle;

            ownerHandle = new WindowInteropHelper(Owner).Handle;
            var hwndSource = HwndSource.FromHwnd(ownerHandle);
            if (hwndSource != null)
            {
                hwndSource.AddHook(OwnerWindowProc);
            }
        }

        public void Update()
        {
            if (Owner.Visibility == Visibility.Hidden)
            {
                Visibility = Visibility.Hidden;

                UpdateCore();
            }
            else if (Owner.WindowState == WindowState.Normal)
            {
                if (this.closing) return;

                Visibility = IsGlowing ? Visibility.Visible : Visibility.Collapsed;
                glow.Visibility = IsGlowing ? Visibility.Visible : Visibility.Collapsed;

                UpdateCore();
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        public bool IsGlowing
        {
            set;
            get;
        }

        private WINDOWPOS _previousWP;
        
        private IntPtr OwnerWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ((WM) msg)
            {
                case WM.WINDOWPOSCHANGED:
                case WM.WINDOWPOSCHANGING:
                    Assert.IsNotDefault(lParam);
                    var wp = (WINDOWPOS) Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                    if (!wp.Equals(_previousWP))
                    {
                        this.UpdateCore();
                    }
                    _previousWP = wp;
                    break;
                case WM.SIZE:
                case WM.SIZING:
                    this.UpdateCore();
                    break;
            }
            return IntPtr.Zero;
        }

        private void UpdateCore()
        {
            RECT rect;
            if (UnsafeNativeMethods.GetWindowRect(ownerHandle, out rect))
            {
                NativeMethods.SetWindowPos(handle, ownerHandle,
                                           (int)(getLeft(DpiFactor, rect)),
                                           (int)(getTop(DpiFactor, rect)),
                                           (int)(getWidth(DpiFactor, rect)),
                                           (int)(getHeight(DpiFactor, rect)),
                                           SWP.NOACTIVATE);
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == (int)WM.SHOWWINDOW)
            {
                if((int)lParam == 3 && this.Visibility != Visibility.Visible) // 3 == SW_PARENTOPENING
                {
                    handled = true; //handle this message so window isn't shown until we want it to                   
                }
            }            
            if (msg == (int)WM.MOUSEACTIVATE)
            {
                handled = true;
                return new IntPtr(3);
            }

            if (msg == (int)WM.LBUTTONDOWN)
            {
                var pt = new Point((int)lParam & 0xFFFF, ((int)lParam >> 16) & 0xFFFF);

                NativeMethods.PostMessage(ownerHandle, (uint)WM.NCLBUTTONDOWN, (IntPtr)getHitTestValue(pt),
                                          IntPtr.Zero);
            }
            if (msg == (int)WM.NCHITTEST)
            {
                var ptScreen = new Point((int)lParam & 0xFFFF, ((int)lParam >> 16) & 0xFFFF);
                Point ptClient = PointFromScreen(ptScreen);
                Cursor cursor = getCursor(ptClient);
                if (cursor != Cursor) Cursor = cursor;
            }

            return IntPtr.Zero;
        }
    }
}
