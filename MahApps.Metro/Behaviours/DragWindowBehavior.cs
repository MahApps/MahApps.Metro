using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace MahApps.Metro.Behaviours
{
    /// <summary>
    /// Drag window behavior adds dragging parent window behavior to any UIElement within window's visual tree.
    /// </summary>
    public class DragWindowBehavior : Behavior<UIElement>
    {
        private Window _window;

        protected override void OnAttached()
        {
            if (AssociatedObject != null)
            {
                _window = Window.GetWindow(AssociatedObject);
                Debug.Assert(_window != null, "Window must not be null.");
                AssociatedObject.MouseDown += OnMouseDown;
                AssociatedObject.MouseMove += OnMouseMove;
            }

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            _window = null;

            if (AssociatedObject != null)
            {
                AssociatedObject.MouseDown -= OnMouseDown;
                AssociatedObject.MouseMove -= OnMouseMove;
            }

            base.OnDetaching();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed
                && e.LeftButton == MouseButtonState.Pressed && _window.WindowState == WindowState.Maximized
                && _window.ResizeMode != ResizeMode.NoResize)
            {
                // Calculating correct left coordinate for multi-screen system.
                Point mouseAbsolute = _window.PointToScreen(Mouse.GetPosition(_window));
                double width = _window.RestoreBounds.Width;
                double left = mouseAbsolute.X - width/2;

                // Aligning window's position to fit the screen.
                double virtualScreenWidth = SystemParameters.VirtualScreenWidth;
                left = left < 0 ? 0 : left;
                left = left + width > virtualScreenWidth ? virtualScreenWidth - width : left;

                Point mousePosition = e.MouseDevice.GetPosition(_window);

                // When dragging the window down at the very top of the border,
                // move the window a bit upwards to avoid showing the resize handle as soon as the mouse button is released.
                _window.Top = mousePosition.Y < 5 ? -5 : mouseAbsolute.Y - mousePosition.Y;
                _window.Left = left;

                // Restore window to normal state.
                _window.WindowState = WindowState.Normal;

                _window.DragMove();
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed &&
                e.LeftButton == MouseButtonState.Pressed)
                _window.DragMove();

            if (e.ClickCount == 2 &&
                (_window.ResizeMode == ResizeMode.CanResizeWithGrip || _window.ResizeMode == ResizeMode.CanResize))
            {
                _window.WindowState = _window.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            }
        }
    }
}
