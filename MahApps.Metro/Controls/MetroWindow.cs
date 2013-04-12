using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using MahApps.Metro.Native;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = PART_TitleBar, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_WindowCommands, Type = typeof(WindowCommands))]
    public class MetroWindow : Window
    {
        private const string PART_TitleBar = "PART_TitleBar";
        private const string PART_WindowCommands = "PART_WindowCommands";

        public static readonly DependencyProperty ShowIconOnTitleBarProperty = DependencyProperty.Register("ShowIconOnTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowTitleBarProperty = DependencyProperty.Register("ShowTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowMinButtonProperty = DependencyProperty.Register("ShowMinButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowMaxRestoreButtonProperty = DependencyProperty.Register("ShowMaxRestoreButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty TitlebarHeightProperty = DependencyProperty.Register("TitlebarHeight", typeof(int), typeof(MetroWindow), new PropertyMetadata(30));
        public static readonly DependencyProperty TitleCapsProperty = DependencyProperty.Register("TitleCaps", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty SaveWindowPositionProperty = DependencyProperty.Register("SaveWindowPosition", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false));
        public static readonly DependencyProperty WindowPlacementSettingsProperty = DependencyProperty.Register("WindowPlacementSettings", typeof(IWindowPlacementSettings), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(MetroWindow));
        public static readonly DependencyProperty IgnoreTaskbarOnMaximizeProperty = DependencyProperty.Register("IgnoreTaskbar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false));
        public static readonly DependencyProperty GlowBrushProperty = DependencyProperty.Register("GlowBrush", typeof(SolidColorBrush), typeof(MetroWindow), new PropertyMetadata(null));

        bool isDragging;

        public ObservableCollection<Flyout> Flyouts { get; set; }

        public WindowCommands WindowCommands { get; set; }

        public bool IgnoreTaskbarOnMaximize
        {
            get { return (bool)this.GetValue(IgnoreTaskbarOnMaximizeProperty); }
            set { SetValue(IgnoreTaskbarOnMaximizeProperty, value); }
        }

        public Brush TitleForeground
        {
            get { return (Brush)GetValue(TitleForegroundProperty); }
            set { SetValue(TitleForegroundProperty, value); }
        }

        public bool SaveWindowPosition
        {
            get { return (bool)GetValue(SaveWindowPositionProperty); }
            set { SetValue(SaveWindowPositionProperty, value); }
        }

        public IWindowPlacementSettings WindowPlacementSettings
        {
            get { return (IWindowPlacementSettings)GetValue(WindowPlacementSettingsProperty); }
            set { SetValue(WindowPlacementSettingsProperty, value); }
        }

        public bool ShowIconOnTitleBar
        {
            get { return (bool)GetValue(ShowIconOnTitleBarProperty); }
            set { SetValue(ShowIconOnTitleBarProperty, value); }
        }

        public bool ShowTitleBar
        {
            get { return (bool)GetValue(ShowTitleBarProperty); }
            set { SetValue(ShowTitleBarProperty, value); }
        }

        public bool ShowMinButton
        {
            get { return (bool)GetValue(ShowMinButtonProperty); }
            set { SetValue(ShowMinButtonProperty, value); }
        }

        public bool ShowCloseButton
        {
            get { return (bool)GetValue(ShowCloseButtonProperty); }
            set { SetValue(ShowCloseButtonProperty, value); }
        }

        public int TitlebarHeight
        {
            get { return (int)GetValue(TitlebarHeightProperty); }
            set { SetValue(TitlebarHeightProperty, value); }
        }

        public bool ShowMaxRestoreButton
        {
            get { return (bool)GetValue(ShowMaxRestoreButtonProperty); }
            set { SetValue(ShowMaxRestoreButtonProperty, value); }
        }

        public bool TitleCaps
        {
            get { return (bool)GetValue(TitleCapsProperty); }
            set { SetValue(TitleCapsProperty, value); }
        }

        public SolidColorBrush GlowBrush
        {
            get { return (SolidColorBrush)GetValue(GlowBrushProperty); }
            set { SetValue(GlowBrushProperty, value); }
        }

        public string WindowTitle
        {
            get { return TitleCaps ? Title.ToUpper() : Title; }
        }

        public MetroWindow()
        {
            Flyouts = new ObservableCollection<Flyout>();
        }

        static MetroWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroWindow), new FrameworkPropertyMetadata(typeof(MetroWindow)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (WindowCommands == null)
                WindowCommands = new WindowCommands();

            var titleBar = GetTemplateChild(PART_TitleBar) as UIElement;

            if (ShowTitleBar && titleBar != null)
            {
                titleBar.MouseDown += TitleBarMouseDown;
                titleBar.MouseUp += TitleBarMouseUp;
                titleBar.MouseMove += TitleBarMouseMove;
            }
            else
            {
                MouseDown += TitleBarMouseDown;
                MouseUp += TitleBarMouseUp;
                MouseMove += TitleBarMouseMove;
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowCommands != null)
            {
                WindowCommands.RefreshMaximiseIconState();
            }

            base.OnStateChanged(e);
        }

        protected void TitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(this);
            bool isIconClick = ShowIconOnTitleBar && mousePosition.X <= TitlebarHeight && mousePosition.Y <= TitlebarHeight;

            if (e.ChangedButton == MouseButton.Left)
            {
                if (isIconClick)
                {
                    if (e.ClickCount == 2)
                    {
                        Close();
                    }
                    else
                    {
                        ShowSystemMenuPhysicalCoordinates(this, PointToScreen(new Point(0, TitlebarHeight)));
                    }
                }
                else
                {
                    if (e.ClickCount == 1)
                    {
                        isDragging = true;
                        DragMove();
                    }
                    else if (e.ClickCount == 2 && (ResizeMode == ResizeMode.CanResizeWithGrip || ResizeMode == ResizeMode.CanResize))
                    {
                        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                    }
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                ShowSystemMenuPhysicalCoordinates(this, PointToScreen(mousePosition));
            }
        }

        protected void TitleBarMouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
        }

        private void TitleBarMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                isDragging = false;
            }

            if (isDragging
                && WindowState == WindowState.Maximized
                && ResizeMode != ResizeMode.NoResize)
            {
                // Calculating correct left coordinate for multi-screen system.
                Point mouseAbsolute = PointToScreen(Mouse.GetPosition(this));
                double width = RestoreBounds.Width;
                double left = mouseAbsolute.X - width / 2;

                // Check if the mouse is at the top of the screen if TitleBar is not visible
                if(!ShowTitleBar && mouseAbsolute.Y > TitlebarHeight)
                    return;

                // Aligning window's position to fit the screen.
                double virtualScreenWidth = SystemParameters.VirtualScreenWidth;
                left = left + width > virtualScreenWidth ? virtualScreenWidth - width : left;

                var mousePosition = e.MouseDevice.GetPosition(this);

                // When dragging the window down at the very top of the border,
                // move the window a bit upwards to avoid showing the resize handle as soon as the mouse button is released
                Top = mousePosition.Y < 5 ? -5 : mouseAbsolute.Y - mousePosition.Y;
                Left = left;

                // Restore window to normal state.
                WindowState = WindowState.Normal;

                DragMove();
            }
        }

        internal T GetPart<T>(string name) where T : DependencyObject
        {
            return (T)GetTemplateChild(name);
        }

        private static void ShowSystemMenuPhysicalCoordinates(Window window, Point physicalScreenLocation)
        {
            if (window == null) return;

            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero || !UnsafeNativeMethods.IsWindow(hwnd))
                return;

            var hmenu = UnsafeNativeMethods.GetSystemMenu(hwnd, false);

            var cmd = UnsafeNativeMethods.TrackPopupMenuEx(hmenu, Constants.TPM_LEFTBUTTON | Constants.TPM_RETURNCMD, (int)physicalScreenLocation.X, (int)physicalScreenLocation.Y, hwnd, IntPtr.Zero);
            if (0 != cmd)
                UnsafeNativeMethods.PostMessage(hwnd, Constants.SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
        }
    }
}