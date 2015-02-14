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

        private bool isDragging;
        private MetroWindow parentWindow;
        private IntPtr parentWindowHandle;
        private ResizerWindow resizerWindow;
        private IntPtr resizerWindowHandle;
        private int xOffset;
        private int yOffset;
        private bool deferResize;
        private Point mousePosition;

        public MetroResizeGrip()
        {
            this.Loaded += (sender, e) =>
            {
                _FindParentWindow();
                InvalidateDeferResizeProperty();
                resizerWindow = new ResizerWindow(parentWindow);
                resizerWindow.SourceInitialized += (sender2, e2) =>
                {
                    resizerWindowHandle = ((HwndSource)HwndSource.FromVisual(resizerWindow)).Handle;
                };
                resizerWindow.Show();
                resizerWindow.Visibility = System.Windows.Visibility.Collapsed;
                resizerWindow.HideStoryboard.Completed += (sender2, e2) =>
                {
                    if (resizerWindow.Opacity == 0)
                        resizerWindow.Visibility = System.Windows.Visibility.Collapsed;
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
            deferResize = MetroResizeGrip.GetDeferResize(parentWindow);
        }

        private void _DetermineNewSize(out int cx, out int cy)
        {
            Point pt = _GetRelativePoint();

            cx = (int)pt.X + xOffset;
            cy = (int)pt.Y + yOffset;

            if (pt.X <= parentWindow.MinWidth)
                cx = (int)parentWindow.MinWidth;
            if (pt.Y <= parentWindow.MinHeight)
                cy = (int)parentWindow.MinHeight;

            // This is to guard against the user dragging the resize grip under the taskbar, since he won't be able to get a
            // grip on it again.
            Int32Rect r = _GetScreenWorkingAreaBoundsFromPoint(mousePosition);
            if (mousePosition.Y + yOffset >= r.Height)
            {
                cy = r.Height - (int)parentWindow.Top;
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
                    parentWindow = (MetroWindow)DO;
                    parentWindowHandle = ((HwndSource)(HwndSource.FromVisual(parentWindow))).Handle;
                    parentWindow.Closed += (sender, e) => resizerWindow.Close();
                    break;
                }
            }
        }

        private Point _GetRelativePoint()
        {
            POINT p; UnsafeNativeMethods.GetCursorPos(out p);
            Point point = mousePosition = new Point(p.X, p.Y);
            Point point2 = parentWindow.PointFromScreen(point);
            return point2;
        }

        private void _Process()
        {
            int cx, cy;
            _DetermineNewSize(out cx, out cy);

            UnsafeNativeMethods.SetWindowPos(parentWindowHandle, IntPtr.Zero, 0, 0, cx, cy, (uint)0x2 /* SWP_NOMOVE */);
        }

        private void _UpdateResizer()
        {
            int cx, cy;
            _DetermineNewSize(out cx, out cy);

            UnsafeNativeMethods.SetWindowPos(resizerWindowHandle, new IntPtr((int)-1 /* HWND_TOPMOST */), (int)parentWindow.Left - 10, (int)parentWindow.Top - 10, cx + 20, cy + 20, (uint)0x10 /* SWP_NOACTIVE */);
        }

        private void ResizeGrip_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.Assert(parentWindow != null);

            e.Handled = true;
            isDragging = true;
            Point basePoint = _GetRelativePoint();
            xOffset = (int)parentWindow.ActualWidth - (int)basePoint.X;
            yOffset = (int)parentWindow.ActualHeight - (int)basePoint.Y;

            if (MetroResizeGrip.GetDeferResize(parentWindow))
            {
                resizerWindow.Visibility = System.Windows.Visibility.Visible;
                resizerWindow.ShowStoryboard.Begin();
            }
            Mouse.Capture(this);
        }

        private void ResizeGrip_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.Assert(parentWindow != null);

            e.Handled = true;
            if (MetroResizeGrip.GetDeferResize(parentWindow))
            {
                _Process();
                resizerWindow.HideStoryboard.Begin();
            }
            isDragging = false;
            this.ReleaseMouseCapture();
        }

        private void ResizeGrip_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Debug.Assert(parentWindow != null);
            Debug.Assert(parentWindowHandle != null);

            e.Handled = true;
            if (!isDragging)
                return;

            if (deferResize)
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
