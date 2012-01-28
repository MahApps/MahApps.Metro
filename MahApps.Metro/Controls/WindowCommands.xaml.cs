using System;
using System.Windows;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public partial class WindowCommands
    {
        public WindowCommands()
        {
            InitializeComponent();
        }

        public event ClosingWindowEventHandler ClosingWindow;

        protected void OnClosingWindow(ClosingWindowEventHandlerArgs args)
        {
            var handler = ClosingWindow;
            if (handler != null) handler(this, args);
        }

        private void MinimiseClick(object sender, RoutedEventArgs e)
        {
            var parentWindow = GetParentWindow();
            if (parentWindow != null)
            {
                parentWindow.WindowState = WindowState.Minimized;
            }
        }

        private void MaximiseClick(object sender, RoutedEventArgs e)
        {
            var parentWindow = GetParentWindow();
            if (parentWindow != null)
            {
                if (parentWindow.WindowState == WindowState.Maximized)
                    parentWindow.WindowState = WindowState.Normal;
                else
                    parentWindow.WindowState = WindowState.Maximized;

                RefreshMaximiseIconState(parentWindow);
            }
        }

        public void RefreshMaximiseIconState()
        {
            RefreshMaximiseIconState(GetParentWindow());
        }

        private void RefreshMaximiseIconState(Window parentWindow)
        {
            if (parentWindow != null)
            {
                if (parentWindow.WindowState == WindowState.Normal)
                {
                    btnMax.Content = "1";
                    btnMax.SetResourceReference(System.Windows.Controls.Button.ToolTipProperty, "WindowCommandsMaximiseToolTip");
                }
                else
                {
                    btnMax.Content = "2";
                    btnMax.SetResourceReference(System.Windows.Controls.Button.ToolTipProperty, "WindowCommandsRestoreToolTip");
                }
            }
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            var closingWindowEventHandlerArgs = new ClosingWindowEventHandlerArgs();
            OnClosingWindow(closingWindowEventHandlerArgs);

            if (closingWindowEventHandlerArgs.Cancelled) return;

            var parentWindow = GetParentWindow();
            if (parentWindow != null)
            {
                parentWindow.Close();
            }
        }

        private Window GetParentWindow()
        {
            var parent = VisualTreeHelper.GetParent(this);

            while (parent != null && !(parent is Window))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            var parentWindow = parent as Window;
            return parentWindow;
        }

        public delegate void ClosingWindowEventHandler(object sender, ClosingWindowEventHandlerArgs args);

        public class ClosingWindowEventHandlerArgs : EventArgs
        {
            public bool Cancelled { get; set; }
        }
    }
}
