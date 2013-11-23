using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Native;
using System.ComponentModel;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// An extended, metrofied Window class.
    /// </summary>
    [TemplatePart(Name = PART_TitleBar, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_WindowCommands, Type = typeof(WindowCommands))]
    [TemplatePart(Name = PART_WindowButtonCommands, Type = typeof(WindowButtonCommands))]
    [TemplatePart(Name = PART_OverlayBox, Type = typeof(Grid))]
    [TemplatePart(Name = PART_MessageDialogContainer, Type = typeof(Grid))]
    public class MetroWindow : Window
    {
        private const string PART_TitleBar = "PART_TitleBar";
        private const string PART_WindowCommands = "PART_WindowCommands";
        private const string PART_WindowButtonCommands = "PART_WindowButtonCommands";
        private const string PART_OverlayBox = "PART_OverlayBox";
        private const string PART_MessageDialogContainer = "PART_MessageDialogContainer";

        public static readonly DependencyProperty ShowIconOnTitleBarProperty = DependencyProperty.Register("ShowIconOnTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowTitleBarProperty = DependencyProperty.Register("ShowTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true, null, OnShowTitleBarCoerceValueCallback));
        public static readonly DependencyProperty ShowMinButtonProperty = DependencyProperty.Register("ShowMinButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowMaxRestoreButtonProperty = DependencyProperty.Register("ShowMaxRestoreButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty TitlebarHeightProperty = DependencyProperty.Register("TitlebarHeight", typeof(int), typeof(MetroWindow), new PropertyMetadata(30));
        public static readonly DependencyProperty TitleCapsProperty = DependencyProperty.Register("TitleCaps", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty SaveWindowPositionProperty = DependencyProperty.Register("SaveWindowPosition", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false));
        public static readonly DependencyProperty WindowPlacementSettingsProperty = DependencyProperty.Register("WindowPlacementSettings", typeof(IWindowPlacementSettings), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(MetroWindow));
        public static readonly DependencyProperty IgnoreTaskbarOnMaximizeProperty = DependencyProperty.Register("IgnoreTaskbarOnMaximize", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false));
        public static readonly DependencyProperty GlowBrushProperty = DependencyProperty.Register("GlowBrush", typeof(SolidColorBrush), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty FlyoutsProperty = DependencyProperty.Register("Flyouts", typeof(FlyoutsControl), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty WindowTransitionsEnabledProperty = DependencyProperty.Register("WindowTransitionsEnabled", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowWindowCommandsOnTopProperty = DependencyProperty.Register("ShowWindowCommandsOnTop", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty TextBlockStyleProperty = DependencyProperty.Register("TextBlockStyle", typeof(Style), typeof(MetroWindow), new PropertyMetadata(default(Style)));
        public static readonly DependencyProperty UseNoneWindowStyleProperty = DependencyProperty.Register("UseNoneWindowStyle", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false, OnUseNoneWindowStylePropertyChangedCallback));

        bool isDragging;
        ContentPresenter WindowCommandsPresenter;
        WindowButtonCommands WindowButtonCommands;
        UIElement titleBar;
        internal Grid overlayBox;
        internal Grid messageDialogContainer;
        private Storyboard overlayStoryboard;

        public MetroDialogSettings MetroDialogOptions { get; private set; }

        public Style TextBlockStyle
        {
            get { return (Style)this.GetValue(TextBlockStyleProperty); }
            set { SetValue(TextBlockStyleProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether the Window Commands will show on top of a Flyout with it's position set to Top or Right.
        /// </summary>
        public bool ShowWindowCommandsOnTop
        {
            get { return (bool)this.GetValue(ShowWindowCommandsOnTopProperty); }
            set { SetValue(ShowWindowCommandsOnTopProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether the window's entrance transition animation is enabled.
        /// </summary>
        public bool WindowTransitionsEnabled
        {
            get { return (bool)this.GetValue(WindowTransitionsEnabledProperty); }
            set { SetValue(WindowTransitionsEnabledProperty, value); }
        }

        /// <summary>
        /// Gets/sets the FlyoutsControl that hosts the window's flyouts.
        /// </summary>
        public FlyoutsControl Flyouts
        {
            get { return (FlyoutsControl)GetValue(FlyoutsProperty); }
            set { SetValue(FlyoutsProperty, value); }
        }

        public WindowCommands WindowCommands { get; set; }

        /// <summary>
        /// Gets/sets whether the window will ignore (and overlap) the taskbar when maximized.
        /// </summary>
        public bool IgnoreTaskbarOnMaximize
        {
            get { return (bool)this.GetValue(IgnoreTaskbarOnMaximizeProperty); }
            set { SetValue(IgnoreTaskbarOnMaximizeProperty, value); }
        }

        /// <summary>
        /// Gets/sets the brush used for the titlebar's foreground.
        /// </summary>
        public Brush TitleForeground
        {
            get { return (Brush)GetValue(TitleForegroundProperty); }
            set { SetValue(TitleForegroundProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether the window will save it's position between loads.
        /// </summary>
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

        /// <summary>
        /// Get/sets whether the titlebar icon is visible or not.
        /// </summary>
        public bool ShowIconOnTitleBar
        {
            get { return (bool)GetValue(ShowIconOnTitleBarProperty); }
            set { SetValue(ShowIconOnTitleBarProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether the TitleBar is visible or not.
        /// </summary>
        public bool ShowTitleBar
        {
            get { return (bool)GetValue(ShowTitleBarProperty); }
            set { SetValue(ShowTitleBarProperty, value); }
        }

        private static object OnShowTitleBarCoerceValueCallback(DependencyObject d, object value)
        {
            // if UseNoneWindowStyle = true no title bar should be shown
            if (((MetroWindow)d).UseNoneWindowStyle)
            {
                return false;
            }
            return value;
        }

        /// <summary>
        /// Gets/sets whether the WindowStyle is None or not.
        /// </summary>
        public bool UseNoneWindowStyle
        {
            get { return (bool)GetValue(UseNoneWindowStyleProperty); }
            set { SetValue(UseNoneWindowStyleProperty, value); }
        }

        private static void OnUseNoneWindowStylePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                // if UseNoneWindowStyle = true no title bar should be shown
                if ((bool)e.NewValue)
                {
                    ((MetroWindow)d).ShowTitleBar = false;
                }
            }
        }

        /// <summary>
        /// Gets/sets if the minimize button is visible.
        /// </summary>
        public bool ShowMinButton
        {
            get { return (bool)GetValue(ShowMinButtonProperty); }
            set { SetValue(ShowMinButtonProperty, value); }
        }

        /// <summary>
        /// Gets/sets if the close button is visible.
        /// </summary>
        public bool ShowCloseButton
        {
            get { return (bool)GetValue(ShowCloseButtonProperty); }
            set { SetValue(ShowCloseButtonProperty, value); }
        }

        /// <summary>
        /// Gets/sets the TitleBar's height.
        /// </summary>
        public int TitlebarHeight
        {
            get { return (int)GetValue(TitlebarHeightProperty); }
            set { SetValue(TitlebarHeightProperty, value); }
        }

        /// <summary>
        /// Gets/sets if the Maximize/Restore button is visible.
        /// </summary>
        public bool ShowMaxRestoreButton
        {
            get { return (bool)GetValue(ShowMaxRestoreButtonProperty); }
            set { SetValue(ShowMaxRestoreButtonProperty, value); }
        }

        /// <summary>
        /// Gets/sets if the TitleBar's text is automatically capitalized.
        /// </summary>
        public bool TitleCaps
        {
            get { return (bool)GetValue(TitleCapsProperty); }
            set { SetValue(TitleCapsProperty, value); }
        }

        /// <summary>
        /// Gets/sets the brush used for the Window's glow.
        /// </summary>
        public SolidColorBrush GlowBrush
        {
            get { return (SolidColorBrush)GetValue(GlowBrushProperty); }
            set { SetValue(GlowBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the TitleBar/Window's Text.
        /// </summary>
        public string WindowTitle
        {
            get { return TitleCaps ? Title.ToUpper() : Title; }
        }

        /// <summary>
        /// Begins to show the MetroWindow's overlay effect.
        /// </summary>
        /// <returns>A task representing the process.</returns>
        public System.Threading.Tasks.Task ShowOverlayAsync()
        {
            if (IsOverlayVisible() && overlayStoryboard == null)
                return new System.Threading.Tasks.Task(() => { }); //No Task.FromResult in .NET 4.

            Dispatcher.VerifyAccess();

            overlayBox.Visibility = System.Windows.Visibility.Visible;

            System.Threading.Tasks.TaskCompletionSource<object> tcs = new System.Threading.Tasks.TaskCompletionSource<object>();

            Storyboard sb = this.Template.Resources["OverlayFastSemiFadeIn"] as Storyboard;

            sb = sb.Clone();

            EventHandler completionHandler = null;
            completionHandler = new EventHandler((sender, args) =>
                {
                    sb.Completed -= completionHandler;

                    if (overlayStoryboard == sb)
                    {
                        overlayStoryboard = null;
                    }

                    tcs.TrySetResult(null);
                });

            sb.Completed += completionHandler;

            overlayBox.BeginStoryboard(sb);

            overlayStoryboard = sb;

            return tcs.Task;
        }
        /// <summary>
        /// Begins to hide the MetroWindow's overlay effect.
        /// </summary>
        /// <returns>A task representing the process.</returns>
        public System.Threading.Tasks.Task HideOverlayAsync()
        {
            if (overlayBox.Visibility == System.Windows.Visibility.Visible && overlayBox.Opacity == 0.0)
                return new System.Threading.Tasks.Task(() => { }); //No Task.FromResult in .NET 4.

            Dispatcher.VerifyAccess();

            System.Threading.Tasks.TaskCompletionSource<object> tcs = new System.Threading.Tasks.TaskCompletionSource<object>();

            Storyboard sb = this.Template.Resources["OverlayFastSemiFadeOut"] as Storyboard;

            sb = sb.Clone();

            EventHandler completionHandler = null;
            completionHandler = new EventHandler((sender, args) =>
            {
                sb.Completed -= completionHandler;

                if (overlayStoryboard == sb)
                {
                    overlayBox.Visibility = System.Windows.Visibility.Hidden;
                    overlayStoryboard = null;
                }

                tcs.TrySetResult(null);
            });

            sb.Completed += completionHandler;

            overlayBox.BeginStoryboard(sb);

            overlayStoryboard = sb;

            return tcs.Task;
        }
        public bool IsOverlayVisible() { return overlayBox.Visibility == System.Windows.Visibility.Visible && overlayBox.Opacity >= 0.7; }

        /// <summary>
        /// Initializes a new instance of the MahApps.Metro.Controls.MetroWindow class.
        /// </summary>
        public MetroWindow()
        {
            Loaded += this.MetroWindow_Loaded;

            if (MetroDialogOptions == null)
                MetroDialogOptions = new MetroDialogSettings();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.WindowTransitionsEnabled)
            {
                VisualStateManager.GoToState(this, "AfterLoaded", true);
            }

            if (!ShowTitleBar)
            {
                //Disables the system menu for reasons other than clicking an invisible titlebar.
                IntPtr handle = new WindowInteropHelper(this).Handle;
                UnsafeNativeMethods.SetWindowLong(handle, UnsafeNativeMethods.GWL_STYLE, UnsafeNativeMethods.GetWindowLong(handle, UnsafeNativeMethods.GWL_STYLE) & ~UnsafeNativeMethods.WS_SYSMENU);
            }

            // if UseNoneWindowStyle = true no title bar, window commands or min, max, close buttons should be shown
            if (UseNoneWindowStyle)
            {
                WindowCommandsPresenter.Visibility = Visibility.Collapsed;
                ShowMinButton = false;
                ShowMaxRestoreButton = false;
                ShowCloseButton = false;
            }

            if (this.Flyouts == null)
            {
                this.Flyouts = new FlyoutsControl();
            }
        }

        static MetroWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroWindow), new FrameworkPropertyMetadata(typeof(MetroWindow)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (TextBlockStyle != null && !this.Resources.Contains(typeof(TextBlock)))
            {
                this.Resources.Add(typeof(TextBlock), TextBlockStyle);
            }

            if (WindowCommands == null)
                WindowCommands = new WindowCommands();
            WindowCommandsPresenter = GetTemplateChild("PART_WindowCommands") as ContentPresenter;
            WindowButtonCommands = GetTemplateChild(PART_WindowButtonCommands) as WindowButtonCommands;

            overlayBox = GetTemplateChild(PART_OverlayBox) as Grid;
            messageDialogContainer = GetTemplateChild(PART_MessageDialogContainer) as Grid;

            titleBar = GetTemplateChild(PART_TitleBar) as UIElement;

            if (titleBar != null && titleBar.Visibility == System.Windows.Visibility.Visible)
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
            if (WindowButtonCommands != null)
            {
                WindowButtonCommands.RefreshMaximiseIconState();
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
                else if (!UseNoneWindowStyle)
                {
                    // if UseNoneWindowStyle = true no movement, no maximize please
                    IntPtr windowHandle = new WindowInteropHelper(this).Handle;
                    UnsafeNativeMethods.ReleaseCapture();

                    var mPoint = Mouse.GetPosition(this);

                    var wpfPoint = PointToScreen(mPoint);
                    var x = Convert.ToInt16(wpfPoint.X);
                    var y = Convert.ToInt16(wpfPoint.Y);
                    var lParam = x | (y << 16);
                    UnsafeNativeMethods.SendMessage(windowHandle, Constants.WM_NCLBUTTONDOWN, Constants.HT_CAPTION, lParam);

                    if (e.ClickCount == 2 && (ResizeMode == ResizeMode.CanResizeWithGrip || ResizeMode == ResizeMode.CanResize) && mPoint.Y <= TitlebarHeight)
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
                if (!(titleBar.Visibility == System.Windows.Visibility.Visible) && mouseAbsolute.Y > TitlebarHeight)
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

        internal void HandleFlyoutStatusChange(Flyout flyout, int visibleFlyouts)
        {
            //checks a recently opened flyout's position.
            if (flyout.Position == Position.Right || flyout.Position == Position.Top)
            {
                //get it's zindex
                var zIndex = flyout.IsOpen ? Panel.GetZIndex(flyout) + 3 : visibleFlyouts + 2;
                if (this.ShowWindowCommandsOnTop) //if ShowWindowCommandsOnTop is true, set the window commands' zindex to a number that is higher than the flyout's. 
                {
                    WindowCommandsPresenter.SetValue(Panel.ZIndexProperty, zIndex);
                }
                WindowButtonCommands.SetValue(Panel.ZIndexProperty, zIndex);
            }
        }
    }
}