using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using MahApps.Metro.Models.Win32;

namespace MahApps.Metro.Controls
{
    partial class GlowWindow : Window
    {
        private readonly Func<Point, Cursor> getCursor;
        private readonly Func<double> getHeight;
        private readonly Func<Point, HitTestValues> getHitTestValue;
        private readonly Func<double> getLeft;
        private readonly Func<double> getTop;
        private readonly Func<double> getWidth;
        private const double edgeSize = 20.0;
        private const double glowSize = 9.0;
        private IntPtr handle;
        private IntPtr ownerHandle;
        private static double? _dpiFactor = null;

        public GlowWindow(Window owner, GlowDirection direction)
        {
            InitializeComponent();

            this.Owner = owner;
            glow.Visibility = Visibility.Collapsed;

            var b = new Binding("GlowBrush.Color");
            b.Source = owner;
            glow.SetBinding(Glow.GlowColorProperty, b);

            switch (direction)
            {
                case GlowDirection.Left:
                    glow.Orientation = Orientation.Vertical;
                    glow.HorizontalAlignment = HorizontalAlignment.Right;
                    getLeft = () => Math.Ceiling(owner.Left - glowSize);
                    getTop = () => owner.Top - glowSize;
                    getWidth = () => glowSize;
                    getHeight = () => owner.ActualHeight + glowSize*2;
                    getHitTestValue = p => new Rect(0, 0, ActualWidth, edgeSize).Contains(p)
                                               ? HitTestValues.HTTOPLEFT
                                               : new Rect(0, ActualHeight - edgeSize, ActualWidth, edgeSize).Contains(p)
                                                     ? HitTestValues.HTBOTTOMLEFT
                                                     : HitTestValues.HTLEFT;
                    getCursor = p => new Rect(0, 0, ActualWidth, edgeSize).Contains(p)
                                         ? Cursors.SizeNWSE
                                         : new Rect(0, ActualHeight - edgeSize, ActualWidth, edgeSize).Contains(p)
                                               ? Cursors.SizeNESW
                                               : Cursors.SizeWE;
                    break;
                case GlowDirection.Right:
                    glow.Orientation = Orientation.Vertical;
                    glow.HorizontalAlignment = HorizontalAlignment.Left;
                    getLeft = () => owner.Left + owner.ActualWidth;
                    getTop = () => owner.Top - glowSize;
                    getWidth = () => glowSize;
                    getHeight = () => owner.ActualHeight + glowSize*2;
                    getHitTestValue = p => new Rect(0, 0, ActualWidth, edgeSize).Contains(p)
                                               ? HitTestValues.HTTOPRIGHT
                                               : new Rect(0, ActualHeight - edgeSize, ActualWidth, edgeSize).Contains(p)
                                                     ? HitTestValues.HTBOTTOMRIGHT
                                                     : HitTestValues.HTRIGHT;
                    getCursor = p => new Rect(0, 0, ActualWidth, edgeSize).Contains(p)
                                         ? Cursors.SizeNESW
                                         : new Rect(0, ActualHeight - edgeSize, ActualWidth, edgeSize).Contains(p)
                                               ? Cursors.SizeNWSE
                                               : Cursors.SizeWE;
                    break;
                case GlowDirection.Top:
                    glow.Orientation = Orientation.Horizontal;
                    glow.VerticalAlignment = VerticalAlignment.Bottom;
                    getLeft = () => owner.Left;
                    getTop = () => Math.Ceiling(owner.Top - glowSize);
                    getWidth = () => owner.ActualWidth;
                    getHeight = () => glowSize;
                    getHitTestValue = p => new Rect(0, 0, edgeSize - glowSize, ActualHeight).Contains(p)
                                               ? HitTestValues.HTTOPLEFT
                                               : new Rect(Width - edgeSize + glowSize, 0, edgeSize - glowSize,
                                                          ActualHeight).Contains(p)
                                                     ? HitTestValues.HTTOPRIGHT
                                                     : HitTestValues.HTTOP;
                    getCursor = p => new Rect(0, 0, edgeSize - glowSize, ActualHeight).Contains(p)
                                         ? Cursors.SizeNWSE
                                         : new Rect(Width - edgeSize + glowSize, 0, edgeSize - glowSize, ActualHeight).
                                               Contains(p)
                                               ? Cursors.SizeNESW
                                               : Cursors.SizeNS;
                    break;
                case GlowDirection.Bottom:
                    glow.Orientation = Orientation.Horizontal;
                    glow.VerticalAlignment = VerticalAlignment.Top;
                    getLeft = () => owner.Left;
                    getTop = () => owner.Top + owner.ActualHeight;
                    getWidth = () => owner.ActualWidth;
                    getHeight = () => glowSize;
                    getHitTestValue = p => new Rect(0, 0, edgeSize - glowSize, ActualHeight).Contains(p)
                                               ? HitTestValues.HTBOTTOMLEFT
                                               : new Rect(Width - edgeSize + glowSize, 0, edgeSize - glowSize,
                                                          ActualHeight).Contains(p)
                                                     ? HitTestValues.HTBOTTOMRIGHT
                                                     : HitTestValues.HTBOTTOM;
                    getCursor = p => new Rect(0, 0, edgeSize - glowSize, ActualHeight).Contains(p)
                                         ? Cursors.SizeNESW
                                         : new Rect(Width - edgeSize + glowSize, 0, edgeSize - glowSize, ActualHeight).
                                               Contains(p)
                                               ? Cursors.SizeNWSE
                                               : Cursors.SizeNS;
                    break;
            }

            owner.ContentRendered += (sender, e) => glow.Visibility = Visibility.Visible;
            owner.Activated += (sender, e) => Update();
            owner.Activated += (sender, e) => glow.IsGlow = true;
            owner.Deactivated += (sender, e) => glow.IsGlow = false;
            owner.LocationChanged += (sender, e) => Update();
            owner.SizeChanged += (sender, e) => Update();
            owner.StateChanged += (sender, e) => Update();
            owner.Closed += (sender, e) => Close();
        }

        public static double DpiFactor
        {
            get
            {
                if (_dpiFactor == null)
                {

                    PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
                    double dpiX = 96.0, dpiY = 96.0;
                    if (source != null)
                    {
                        dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                        dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
                    }
                    if (dpiX == dpiY)
                    {
                        _dpiFactor = dpiX / 96.0;
                    }
                }
                return _dpiFactor.Value;
            }
            
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var source = (HwndSource) PresentationSource.FromVisual(this);
            WS ws = source.Handle.GetWindowLong();
            WSEX wsex = source.Handle.GetWindowLongEx();

            //ws |= WS.POPUP;
            wsex ^= WSEX.APPWINDOW;
            wsex |= WSEX.NOACTIVATE;

            source.Handle.SetWindowLong(ws);
            source.Handle.SetWindowLongEx(wsex);
            source.AddHook(WndProc);

            handle = source.Handle;
        }

        public void Update()
        {
            if (Owner.WindowState == WindowState.Normal)
            {
                Visibility = Visibility.Visible;

                UpdateCore();
            }
            else if (Owner.Visibility == Visibility.Hidden)
            {
                Visibility = Visibility.Hidden;

                UpdateCore();
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateCore()
        {
            if (ownerHandle == IntPtr.Zero)
            {
                ownerHandle = new WindowInteropHelper(Owner).Handle;
            }

            NativeMethods.SetWindowPos(
                handle,
                ownerHandle,
                (int) (getLeft() * DpiFactor),
                (int) (getTop() * DpiFactor),
                (int) (getWidth() * DpiFactor),
                (int) (getHeight() * DpiFactor),
                SWP.NOACTIVATE);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == (int) WM.MOUSEACTIVATE)
            {
                handled = true;
                return new IntPtr(3);
            }

            if (msg == (int) WM.LBUTTONDOWN)
            {
                var pt = new Point((int) lParam & 0xFFFF, ((int) lParam >> 16) & 0xFFFF);

                NativeMethods.PostMessage(ownerHandle, (uint) WM.NCLBUTTONDOWN, (IntPtr) getHitTestValue(pt),
                                          IntPtr.Zero);
            }
            if (msg == (int) WM.NCHITTEST)
            {
                var ptScreen = new Point((int) lParam & 0xFFFF, ((int) lParam >> 16) & 0xFFFF);
                Point ptClient = PointFromScreen(ptScreen);
                Cursor cursor = getCursor(ptClient);
                if (cursor != Cursor) Cursor = cursor;
            }

            return IntPtr.Zero;
        }
    }
}