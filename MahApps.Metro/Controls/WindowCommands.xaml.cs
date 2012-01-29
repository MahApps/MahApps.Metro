namespace MahApps.Metro.Controls
{
    using System;
    using System.Windows;

    public partial class WindowCommands
    {
        private Window _parentWindow;

        public WindowCommands()
        {
            Loaded += (sender, args) => _parentWindow = Window.GetWindow(this);
            InitializeComponent();
        }

        public event EventHandler<ClosingWindowEventHandlerArgs> ClosingWindow;

        protected void OnClosingWindow(ClosingWindowEventHandlerArgs args)
        {
            EventHandler<ClosingWindowEventHandlerArgs> handler = ClosingWindow;
            if (handler != null)
                handler(this, args);
        }

        private void MinimiseClick(object sender, RoutedEventArgs e)
        {
            if (_parentWindow != null)
                _parentWindow.WindowState = WindowState.Minimized;
        }

        private void MaximiseClick(object sender, RoutedEventArgs e)
        {
            if (_parentWindow == null)
                return;

            _parentWindow.WindowState = _parentWindow.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;

            RefreshMaximiseIconState(_parentWindow);
        }

        public void RefreshMaximiseIconState()
        {
            RefreshMaximiseIconState(_parentWindow);
        }

        private void RefreshMaximiseIconState(Window parentWindow)
        {
            if (parentWindow == null)
                return;

            if (parentWindow.WindowState == WindowState.Normal)
            {
                btnMax.Content = "1";
                btnMax.SetResourceReference(ToolTipProperty, "WindowCommandsMaximiseToolTip");
            }
            else
            {
                btnMax.Content = "2";
                btnMax.SetResourceReference(ToolTipProperty, "WindowCommandsRestoreToolTip");
            }
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            var closingWindowEventHandlerArgs = new ClosingWindowEventHandlerArgs();
            OnClosingWindow(closingWindowEventHandlerArgs);

            if (closingWindowEventHandlerArgs.Cancelled)
                return;

            if (_parentWindow != null)
                _parentWindow.Close();
        }
    }

    public class ClosingWindowEventHandlerArgs : EventArgs
    {
        public bool Cancelled { get; set; }
    }
}