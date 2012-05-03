﻿using System;
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
        private readonly int doubleclick = UnsafeNativeMethods.GetDoubleClickTime();
        private DateTime lastMouseClick;

        public static readonly DependencyProperty ShowIconOnTitleBarProperty = DependencyProperty.Register("ShowIconOnTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowTitleBarProperty = DependencyProperty.Register("ShowTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowMinButtonProperty = DependencyProperty.Register("ShowMinButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowMaxRestoreButtonProperty = DependencyProperty.Register("ShowMaxRestoreButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty TitlebarHeightProperty = DependencyProperty.Register("TitlebarHeight", typeof(int), typeof(MetroWindow), new PropertyMetadata(30));
        public static readonly DependencyProperty SaveWindowPositionProperty = DependencyProperty.Register("SaveWindowPosition", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false));

        public bool SaveWindowPosition
        {
            get { return (bool)GetValue(SaveWindowPositionProperty); }
            set { SetValue(SaveWindowPositionProperty, value); }
        }

        static MetroWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroWindow), new FrameworkPropertyMetadata(typeof(MetroWindow)));
        }

        public WindowCommands WindowCommands { get; set; }

        public bool ShowIconOnTitleBar
        {
            get { return (bool) GetValue(ShowIconOnTitleBarProperty); }
            set { SetValue(ShowIconOnTitleBarProperty, value); }
        }

        public bool ShowTitleBar
        {
            get { return (bool) GetValue(ShowTitleBarProperty); }
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (WindowCommands == null)
                WindowCommands = new WindowCommands();

            var titleBar = GetTemplateChild(PART_TitleBar) as UIElement;

            if (ShowTitleBar && titleBar != null)
            {
                titleBar.MouseDown += TitleBarMouseDown;
                titleBar.MouseMove += TitleBarMouseMove;
            }
            else
            {
                MouseDown += TitleBarMouseDown;
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
            // Mouse single-click + hold is dragging window
            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Pressed)
                DragMove();

            var mousePosition = GetCorrectPosition(this);

            // Window icon is visible and mouse is left-clicked on icon area
            if (mousePosition.X <= TitlebarHeight && mousePosition.Y <= TitlebarHeight && ShowIconOnTitleBar)
            {
                // mouse is double-clicked
                if ((DateTime.Now - lastMouseClick).TotalMilliseconds <= doubleclick)
                {
                    Close();
                    return;
                }

                // Mouse is single-clicked
                lastMouseClick = DateTime.Now;
                ShowSystemMenuPhysicalCoordinates(this, PointToScreen(new Point(0, TitlebarHeight)));
            }

            // Mouse is double-clicked on titlebar
            else if (e.ClickCount == 2 && (ResizeMode == ResizeMode.CanResizeWithGrip || ResizeMode == ResizeMode.CanResize))
            {
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }

            // Mouse is right-clicked on titlebar
            else if (e.ChangedButton == MouseButton.Right)
            {
                ShowSystemMenuPhysicalCoordinates(this, PointToScreen(GetCorrectPosition(this)));
            }
        }

        private static Point GetCorrectPosition(Visual relativeTo)
        {
            UnsafeNativeMethods.Win32Point w32Mouse;
            UnsafeNativeMethods.GetCursorPos(out w32Mouse);
            return relativeTo.PointFromScreen(new Point(w32Mouse.X, w32Mouse.Y));
        }

        private void TitleBarMouseMove(object sender, MouseEventArgs e)
        {
            // Check if any buttons pressed
            if (e.RightButton == MouseButtonState.Pressed || e.MiddleButton == MouseButtonState.Pressed ||
                WindowState != WindowState.Maximized || e.LeftButton != MouseButtonState.Pressed) return;

            // Check if mouse not in icon area
            var mousePos = GetCorrectPosition(this);
            if (mousePos.X <= TitlebarHeight && mousePos.Y <= TitlebarHeight && ShowIconOnTitleBar)
                return;

            // Calculating correct left coordinate for multi-screen system.
            Point mouseAbsolute = PointToScreen(Mouse.GetPosition(this));
            double width = RestoreBounds.Width;
            double left = mouseAbsolute.X - width / 2;

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
