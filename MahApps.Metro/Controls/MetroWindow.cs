using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using MahApps.Metro.Native;
using MahApps.Metro.Behaviours;

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
        public static readonly DependencyProperty ShowTitleBarProperty = DependencyProperty.Register("ShowTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true, (o, args) => ((MetroWindow)o).UpdateDragWindowBehavior()));
        public static readonly DependencyProperty ShowMinButtonProperty = DependencyProperty.Register("ShowMinButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowMaxRestoreButtonProperty = DependencyProperty.Register("ShowMaxRestoreButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty TitlebarHeightProperty = DependencyProperty.Register("TitlebarHeight", typeof(int), typeof(MetroWindow), new PropertyMetadata(30));
        public static readonly DependencyProperty TitleCapsProperty = DependencyProperty.Register("TitleCaps", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty SavePositionProperty = DependencyProperty.Register("SaveWindowPosition", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false));
        private UIElement _titleBar;

        private readonly DragWindowBehavior _dragWindowBehavior = new DragWindowBehavior();

        public bool SaveWindowPosition
        {
            get { return (bool)GetValue(SavePositionProperty); }
            set { SetValue(SavePositionProperty, value); }
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

        public bool TitleCaps
        {
            get { return (bool)GetValue(TitleCapsProperty); }
            set { SetValue(TitleCapsProperty, value); }
        }

        public string WindowTitle
        {
            get { return TitleCaps ? Title.ToUpper() : Title; }
        }

        private UIElement TitleBar
        {
            get { return _titleBar; }
            set
            {
                if (ReferenceEquals(value, _titleBar))
                    return;

                if (_titleBar != null)
                {
                    _titleBar.RemoveBehavior(_dragWindowBehavior);
                    _titleBar.MouseUp -= TitleBarMouseUp;
                }
                _titleBar = value;
                _titleBar.MouseUp += TitleBarMouseUp;
                UpdateDragWindowBehavior();
            }
        }

        private void UpdateDragWindowBehavior()
        {
            var titleBar = TitleBar;

            titleBar.RemoveBehavior(_dragWindowBehavior);
            this.RemoveBehavior(_dragWindowBehavior);

            // Add dragging behavior to title bar if it is no null, otherwise to current window.
            if (ShowTitleBar && titleBar != null)
                titleBar.AddBehavior(_dragWindowBehavior);
            else
                this.AddBehavior(_dragWindowBehavior);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (WindowCommands == null)
                WindowCommands = new WindowCommands();

            TitleBar = GetPart<UIElement>(PART_TitleBar);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowCommands != null)
            {
                WindowCommands.RefreshMaximiseIconState();
            }

            base.OnStateChanged(e);
        }

        protected void TitleBarMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!ShowIconOnTitleBar || !ShowTitleBar)
                return;

            var mousePosition = GetCorrectPosition(this);
            
            if (mousePosition.X <= TitlebarHeight && mousePosition.Y <= TitlebarHeight)
            {
                if ((DateTime.Now - lastMouseClick).TotalMilliseconds <= doubleclick)
                {
                    Close();
                    return;
                }
                lastMouseClick = DateTime.Now;

                ShowSystemMenuPhysicalCoordinates(this, PointToScreen(new Point(0, TitlebarHeight)));
            }
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
