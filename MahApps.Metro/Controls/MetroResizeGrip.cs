using MahApps.Metro.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public class MetroResizeGrip : System.Windows.Controls.Primitives.ResizeGrip
    {
        public static readonly DependencyProperty DeferResizeProperty =
            DependencyProperty.RegisterAttached("DeferResize", typeof(bool), typeof(MetroResizeGrip), new PropertyMetadata(false));

        private bool _IsDragging;
        private MetroWindow _ParentWindow;
        private IntPtr _ParentWindowHandle;
        private ResizerWindow _ResizerWindow;
        private IntPtr _ResizerWindowHandle;
        private int _XOffset;
        private int _YOffset;
        private bool _DeferResize;
        private Point _MousePosition;

        public MetroResizeGrip()
        {
            this.Loaded += (sender, e) =>
            {
                _FindParentWindow();
                InvalidateDeferResizeProperty();
                _ResizerWindow = new ResizerWindow(_ParentWindow);
                _ResizerWindow.SourceInitialized += (sender2, e2) =>
                {
                    _ResizerWindowHandle = ((HwndSource)HwndSource.FromVisual(_ResizerWindow)).Handle;
                };
                _ResizerWindow.Show();
                _ResizerWindow.Visibility = System.Windows.Visibility.Collapsed;
                _ResizerWindow.HideStoryboard.Completed += (sender2, e2) =>
                {
                    if (_ResizerWindow.Opacity == 0)
                        _ResizerWindow.Visibility = System.Windows.Visibility.Collapsed;
                };
            };
            this.MouseLeftButtonDown += ResizeGrip_MouseLeftButtonDown;
            this.MouseLeftButtonUp += ResizeGrip_MouseLeftButtonUp;
            this.MouseMove += ResizeGrip_MouseMove;
        }

        public static bool GetDeferResize(DependencyObject obj)
        {
            return (bool)obj.GetValue(DeferResizeProperty);
        }

        public static void SetDeferResize(DependencyObject obj, bool value)
        {
            obj.SetValue(DeferResizeProperty, value);
        }

        public void InvalidateDeferResizeProperty()
        {
            _DeferResize = MetroResizeGrip.GetDeferResize(_ParentWindow);
        }

        private void _DetermineNewSize(out int cx, out int cy)
        {
            Point pt = _GetRelativePoint();

            cx = (int)pt.X + _XOffset;
            cy = (int)pt.Y + _YOffset;

            if (pt.X <= _ParentWindow.MinWidth)
                cx = (int)_ParentWindow.MinWidth;
            if (pt.Y <= _ParentWindow.MinHeight)
                cy = (int)_ParentWindow.MinHeight;

            // This is to guard against the user dragging the resize grip under the taskbar, since he won't be able to get a
            // grip on it again.
            Int32Rect r = _GetScreenWorkingAreaBoundsFromPoint(_MousePosition);
            if (_MousePosition.Y + _YOffset >= r.Height)
            {
                cy = r.Height - (int)_ParentWindow.Top;
            }
        }

        private void _FindParentWindow()
        {
            DependencyObject DO = this;
            while (true)
            {
                DO = VisualTreeHelper.GetParent(DO);
                if (DO is Window)
                {
                    _ParentWindow = (MetroWindow)DO;
                    _ParentWindowHandle = ((HwndSource)(HwndSource.FromVisual(_ParentWindow))).Handle;
                    _ParentWindow.Closed += (sender, e) => _ResizerWindow.Close();
                    break;
                }
            }
        }

        private Point _GetRelativePoint()
        {
            POINT p; UnsafeNativeMethods.GetCursorPos(out p);
            Point point = _MousePosition = new Point(p.X, p.Y);
            Point point2 = _ParentWindow.PointFromScreen(point);
            return point2;
        }

        private void _Process()
        {
            int cx, cy;
            _DetermineNewSize(out cx, out cy);

            UnsafeNativeMethods.SetWindowPos(_ParentWindowHandle, IntPtr.Zero, 0, 0, cx, cy, (uint)0x2 /* SWP_NOMOVE */);
        }

        private void _UpdateResizer()
        {
            int cx, cy;
            _DetermineNewSize(out cx, out cy);

            UnsafeNativeMethods.SetWindowPos(_ResizerWindowHandle, new IntPtr((int)-1 /* HWND_TOPMOST */), (int)_ParentWindow.Left - 10, (int)_ParentWindow.Top - 10, cx + 20, cy + 20, (uint)0x10 /* SWP_NOACTIVE */);
        }

        private void ResizeGrip_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.Assert(_ParentWindow != null);

            e.Handled = true;
            _IsDragging = true;
            Point basePoint = _GetRelativePoint();
            _XOffset = (int)_ParentWindow.ActualWidth - (int)basePoint.X;
            _YOffset = (int)_ParentWindow.ActualHeight - (int)basePoint.Y;

            if (MetroResizeGrip.GetDeferResize(_ParentWindow))
            {
                _ResizerWindow.Visibility = System.Windows.Visibility.Visible;
                _ResizerWindow.ShowStoryboard.Begin();
            }
            Mouse.Capture(this);
        }

        private void ResizeGrip_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.Assert(_ParentWindow != null);

            e.Handled = true;
            if (MetroResizeGrip.GetDeferResize(_ParentWindow))
            {
                _Process();
                _ResizerWindow.HideStoryboard.Begin();
            }
            _IsDragging = false;
            this.ReleaseMouseCapture();
        }

        private void ResizeGrip_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Debug.Assert(_ParentWindow != null);
            Debug.Assert(_ParentWindowHandle != null);

            e.Handled = true;
            if (!_IsDragging)
                return;

            if (_DeferResize)
            {
                _UpdateResizer();
            }
            else
            {
                _Process();
            }
        }

        private Int32Rect _GetScreenWorkingAreaBoundsFromPoint(System.Windows.Point point)
        {
            Screen screen = Screen.FromPoint(new System.Drawing.Point((int)point.X, (int)point.Y));
            System.Drawing.Rectangle r = screen.WorkingArea;
            return new Int32Rect(r.X, r.Y, r.Width, r.Height);
        }
    }
}
