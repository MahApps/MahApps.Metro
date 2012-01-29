namespace MahApps.Metro.Behaviours
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;

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
                AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
                AssociatedObject.MouseMove += OnMouseMove;
            }

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            _window = null;

            if (AssociatedObject != null)
            {
                AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
                AssociatedObject.MouseMove -= OnMouseMove;
            }

            base.OnDetaching();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed
                && e.LeftButton == MouseButtonState.Pressed && _window.WindowState == WindowState.Maximized)
            {
                // Calcualting correct left coordinate for multi-screen system.
                double virtualScreenWidth = SystemParameters.VirtualScreenWidth;
                double mouseX = _window.PointToScreen(Mouse.GetPosition(_window)).X;
                double left = mouseX - _window.Width / 2;
                // Aligning window's position to fit the screen.
                left = left < 0 ? 0 : left;
                left = left + _window.Width > virtualScreenWidth ? virtualScreenWidth - _window.Width : left;
                
                _window.Top = 0;
                _window.Left = left;

                // Restore window to normal state.
                _window.WindowState = WindowState.Normal;

                _window.DragMove();
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _window.DragMove();

            if (e.ClickCount == 2)
                _window.WindowState = _window.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
        }
    }
}