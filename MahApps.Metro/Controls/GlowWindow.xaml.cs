using System;
using System.Globalization;
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
using WM = MahApps.Metro.Models.Win32.WM;
using WS = MahApps.Metro.Models.Win32.WS;

namespace MahApps.Metro.Controls
{
    partial class GlowWindow : Window
    {
        private readonly Func<Point, Cursor> getCursor;
        private readonly Func<Point, HitTestValues> getHitTestValue;
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

            glow.Direction = direction;

            switch (direction)
            {
                case GlowDirection.Left:
                    glow.Orientation = Orientation.Vertical;
                    glow.HorizontalAlignment = HorizontalAlignment.Right;
                    getLeft = (rect) => rect.left - glowSize + 1;
                    getTop = (rect) => rect.top - 2;
                    getWidth = (rect) => glowSize;
                    getHeight = (rect) => rect.Height + 4;
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
                    getLeft = (rect) => rect.right - 1;
                    getTop = (rect) => rect.top - 2;
                    getWidth = (rect) => glowSize;
                    getHeight = (rect) => rect.Height + 4;
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
                    getLeft = (rect) => rect.left - 2;
                    getTop = (rect) => rect.top - glowSize + 1;
                    getWidth = (rect) => rect.Width + 4;
                    getHeight = (rect) => glowSize;
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
                    getLeft = (rect) => rect.left - 2;
                    getTop = (rect) => rect.bottom - 1;
                    getWidth = (rect) => rect.Width + 4;
                    getHeight = (rect) => glowSize;
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
            owner.StateChanged += (sender, e) => Update();
            owner.IsVisibleChanged += (sender, e) => Update();
            owner.Closed += (sender, e) =>
            {
                closing = true;
                Close();
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
            if (hwndSource == null) return;
            
            var ws = hwndSource.Handle.GetWindowLong();
            var wsex = hwndSource.Handle.GetWindowLongEx();

            //ws |= WS.POPUP;
            wsex ^= WSEX.APPWINDOW;
            wsex |= WSEX.NOACTIVATE;
            if (this.Owner.ResizeMode == ResizeMode.NoResize || this.Owner.ResizeMode == ResizeMode.CanMinimize)
            {
                wsex |= WSEX.TRANSPARENT;
            }

            hwndSource.Handle.SetWindowLong(ws);
            hwndSource.Handle.SetWindowLongEx(wsex);
            hwndSource.AddHook(WndProc);

            handle = hwndSource.Handle;
            ownerHandle = new WindowInteropHelper(Owner).Handle;

            this.resizeModeChangeNotifier = new PropertyChangeNotifier(this.Owner, Window.ResizeModeProperty);
            this.resizeModeChangeNotifier.ValueChanged += ResizeModeChanged;
        }

        private void ResizeModeChanged(object sender, EventArgs e)
        {
            var wsex = hwndSource.Handle.GetWindowLongEx();
            if (this.Owner.ResizeMode == ResizeMode.NoResize || this.Owner.ResizeMode == ResizeMode.CanMinimize)
            {
                wsex |= WSEX.TRANSPARENT;
            }
            else
            {
                wsex ^= WSEX.TRANSPARENT;
            }
            hwndSource.Handle.SetWindowLongEx(wsex);
        }

        public void Update()
        {
            RECT rect;
            if (Owner.Visibility == Visibility.Hidden)
            {
                Visibility = Visibility.Hidden;

                if (ownerHandle != IntPtr.Zero && UnsafeNativeMethods.GetWindowRect(ownerHandle, out rect))
                {
                    UpdateCore(rect);
                }
            }
            else if (Owner.WindowState == WindowState.Normal)
            {
                if (this.closing) return;

                Visibility = IsGlowing ? Visibility.Visible : Visibility.Collapsed;
                glow.Visibility = IsGlowing ? Visibility.Visible : Visibility.Collapsed;

                if (ownerHandle != IntPtr.Zero && UnsafeNativeMethods.GetWindowRect(ownerHandle, out rect))
                {
                    UpdateCore(rect);
                }
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

        internal void UpdateCore(RECT rect)
        {
            NativeMethods.SetWindowPos(handle, ownerHandle,
                                       (int)(getLeft(rect)),
                                       (int)(getTop(rect)),
                                       (int)(getWidth(rect)),
                                       (int)(getHeight(rect)),
                                       SWP.NOACTIVATE | SWP.NOZORDER);
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
                NativeMethods.PostMessage(ownerHandle, (uint)WM.NCLBUTTONDOWN, (IntPtr)getHitTestValue(pt), IntPtr.Zero);
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
