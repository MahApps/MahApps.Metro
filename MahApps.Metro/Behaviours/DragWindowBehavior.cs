namespace MahApps.Metro.Behaviours
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;

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
                // restore window to normal state
                _window.WindowState = WindowState.Normal;
                _window.Top = 0;
                _window.Left = Mouse.GetPosition(_window).X - _window.Width / 2;
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