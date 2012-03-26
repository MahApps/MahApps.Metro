using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_Max", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Close", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Min", Type = typeof(Button))]
    public class WindowCommands : ItemsControl
    {
        [DllImport("user32", CharSet = CharSet.Auto)]
        static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);

        [DllImport("kernel32")]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll")]
        static extern bool FreeLibrary(IntPtr hModule);

        public event ClosingWindowEventHandler ClosingWindow;

        static WindowCommands()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowCommands), new FrameworkPropertyMetadata(typeof(WindowCommands)));
        }

        private Button min;
        private Button max;
        private Button close;

        private IntPtr user32 = IntPtr.Zero;

        ~WindowCommands()
        {
            if (user32 != IntPtr.Zero)
                FreeLibrary(user32);
        }

        private string GetCaption(int id)
        {
            if (user32 == IntPtr.Zero)
                user32 = LoadLibrary(Environment.SystemDirectory + "\\User32.dll");

            var sb = new StringBuilder(256);
            LoadString(user32, (uint)id, sb, sb.Capacity);
            return sb.ToString().Replace("&", "");
        }

        public string strMinimize
        {
            get { return GetCaption(900); }
        }

        public string strMaximize
        {
            get { return GetCaption(901); }
        }

        public string strClose
        {
            get { return GetCaption(905); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            close = Template.FindName("PART_Close", this) as Button;
            if (close != null)
                close.Click += CloseClick;

            max = Template.FindName("PART_Max", this) as Button;
            if (max != null)
                max.Click += MaximiseClick;

            min = Template.FindName("PART_Min", this) as Button;
            if (min != null)
                min.Click += MinimiseClick;
        }

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
            if (parentWindow == null)
                return;

            parentWindow.WindowState = parentWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            RefreshMaximiseIconState(parentWindow);
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
                    max.Content = "1";
                    max.ToolTip = GetCaption(901);
                }
                else
                {
                    max.Content = "2";
                    max.ToolTip = GetCaption(903);
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
