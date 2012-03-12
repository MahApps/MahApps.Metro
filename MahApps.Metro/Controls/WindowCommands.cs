namespace MahApps.Metro.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    [TemplatePart(Name = "PART_Max", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Close", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Min", Type = typeof(Button))]
    public class WindowCommands : ItemsControl
    {
        #region Constants and Fields

        private Button close;

        private Button max;

        private Button min;

        #endregion

        #region Constructors and Destructors

        static WindowCommands()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(WindowCommands), new FrameworkPropertyMetadata(typeof(WindowCommands)));
        }

        #endregion

        #region Delegates

        public delegate void ClosingWindowEventHandler(object sender, ClosingWindowEventHandlerArgs args);

        #endregion

        #region Public Events

        public event ClosingWindowEventHandler ClosingWindow;

        #endregion

        #region Public Methods and Operators

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.close = this.Template.FindName("PART_Close", this) as Button;
            if (this.close != null)
            {
                this.close.Click += this.CloseClick;
            }

            this.max = this.Template.FindName("PART_Max", this) as Button;
            if (this.max != null)
            {
                this.max.Click += this.MaximiseClick;
            }

            this.min = this.Template.FindName("PART_Min", this) as Button;
            if (this.min != null)
            {
                this.min.Click += this.MinimiseClick;
            }
        }

        public void RefreshMaximiseIconState()
        {
            this.RefreshMaximiseIconState(this.GetParentWindow());
        }

        #endregion

        #region Methods

        protected void OnClosingWindow(ClosingWindowEventHandlerArgs args)
        {
            var handler = this.ClosingWindow;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            var closingWindowEventHandlerArgs = new ClosingWindowEventHandlerArgs();
            this.OnClosingWindow(closingWindowEventHandlerArgs);

            if (closingWindowEventHandlerArgs.Cancelled)
            {
                return;
            }

            var parentWindow = this.GetParentWindow();
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

        private void MaximiseClick(object sender, RoutedEventArgs e)
        {
            var parentWindow = this.GetParentWindow();
            if (parentWindow == null)
            {
                return;
            }

            parentWindow.WindowState = parentWindow.WindowState == WindowState.Maximized
                                           ? WindowState.Normal
                                           : WindowState.Maximized;
            this.RefreshMaximiseIconState(parentWindow);
        }

        private void MinimiseClick(object sender, RoutedEventArgs e)
        {
            var parentWindow = this.GetParentWindow();
            if (parentWindow != null)
            {
                parentWindow.WindowState = WindowState.Minimized;
            }
        }

        private void RefreshMaximiseIconState(Window parentWindow)
        {
            if (parentWindow != null)
            {
                if (parentWindow.WindowState == WindowState.Normal)
                {
                    this.max.Content = "1";
                    this.max.SetResourceReference(ToolTipProperty, "WindowCommandsMaximiseToolTip");
                }
                else
                {
                    this.max.Content = "2";
                    this.max.SetResourceReference(ToolTipProperty, "WindowCommandsRestoreToolTip");
                }
            }
        }

        #endregion

        public class ClosingWindowEventHandlerArgs : EventArgs
        {
            #region Public Properties

            public bool Cancelled { get; set; }

            #endregion
        }
    }
}