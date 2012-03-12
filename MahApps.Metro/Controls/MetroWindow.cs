namespace MahApps.Metro.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    [TemplatePart(Name = PART_TitleBar, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_WindowCommands, Type = typeof(WindowCommands))]
    public class MetroWindow : Window
    {
        #region Constants and Fields

        public static readonly DependencyProperty ShowIconOnTitleBarProperty =
            DependencyProperty.Register(
                "ShowIconOnTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty ShowMaxRestoreButtonProperty =
            DependencyProperty.Register(
                "ShowMaxRestoreButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty ShowMinButtonProperty = DependencyProperty.Register(
            "ShowMinButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty ShowTitleBarProperty = DependencyProperty.Register(
            "ShowTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        private const string PART_TitleBar = "PART_TitleBar";

        private const string PART_WindowCommands = "PART_WindowCommands";

        #endregion

        #region Constructors and Destructors

        static MetroWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(MetroWindow), new FrameworkPropertyMetadata(typeof(MetroWindow)));
        }

        #endregion

        #region Public Properties

        public bool ShowIconOnTitleBar
        {
            get
            {
                return (bool)this.GetValue(ShowIconOnTitleBarProperty);
            }
            set
            {
                this.SetValue(ShowIconOnTitleBarProperty, value);
            }
        }

        public bool ShowMaxRestoreButton
        {
            get
            {
                return (bool)this.GetValue(ShowMaxRestoreButtonProperty);
            }
            set
            {
                this.SetValue(ShowMaxRestoreButtonProperty, value);
            }
        }

        public bool ShowMinButton
        {
            get
            {
                return (bool)this.GetValue(ShowMinButtonProperty);
            }
            set
            {
                this.SetValue(ShowMinButtonProperty, value);
            }
        }

        public bool ShowTitleBar
        {
            get
            {
                return (bool)this.GetValue(ShowTitleBarProperty);
            }
            set
            {
                this.SetValue(ShowTitleBarProperty, value);
            }
        }

        public WindowCommands WindowCommands { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.WindowCommands == null)
            {
                this.WindowCommands = new WindowCommands();
            }

            var titleBar = this.GetTemplateChild(PART_TitleBar) as UIElement;

            if (this.ShowTitleBar && titleBar != null)
            {
                titleBar.MouseDown += this.TitleBarMouseDown;
                titleBar.MouseMove += this.TitleBarMouseMove;
            }
            else
            {
                this.MouseDown += this.TitleBarMouseDown;
                this.MouseMove += this.TitleBarMouseMove;
            }
        }

        #endregion

        #region Methods

        internal T GetPart<T>(string name) where T : DependencyObject
        {
            return (T)this.GetTemplateChild(name);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (this.WindowCommands != null)
            {
                this.WindowCommands.RefreshMaximiseIconState();
            }

            base.OnStateChanged(e);
        }

        protected void TitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed
                && e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }

            if (e.ClickCount == 2)
            {
                this.WindowState = this.WindowState == WindowState.Maximized
                                       ? WindowState.Normal
                                       : WindowState.Maximized;
            }
        }

        protected void TitleBarMouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed || e.MiddleButton == MouseButtonState.Pressed
                || e.LeftButton != MouseButtonState.Pressed || this.WindowState != WindowState.Maximized)
            {
                return;
            }

            // Calcualting correct left coordinate for multi-screen system.
            var mouseAbsolute = this.PointToScreen(Mouse.GetPosition(this));
            var width = this.RestoreBounds.Width;
            var left = mouseAbsolute.X - width / 2;

            // Aligning window's position to fit the screen.
            var virtualScreenWidth = SystemParameters.VirtualScreenWidth;
            left = left + width > virtualScreenWidth ? virtualScreenWidth - width : left;

            this.Top = mouseAbsolute.Y - e.MouseDevice.GetPosition(this).Y;
            this.Left = left;

            // Restore window to normal state.
            this.WindowState = WindowState.Normal;

            this.DragMove();
        }

        #endregion
    }
}