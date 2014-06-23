using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Native;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// An extended, metrofied Window class.
    /// </summary>
    [TemplatePart(Name = PART_Icon, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_TitleBar, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_WindowTitleBackground, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_LeftWindowCommands, Type = typeof(WindowCommands))]
    [TemplatePart(Name = PART_RightWindowCommands, Type = typeof(WindowCommands))]
    [TemplatePart(Name = PART_WindowButtonCommands, Type = typeof(WindowButtonCommands))]
    [TemplatePart(Name = PART_OverlayBox, Type = typeof(Grid))]
    [TemplatePart(Name = PART_MetroDialogContainer, Type = typeof(Grid))]
    [TemplatePart(Name = PART_FlyoutModal, Type = typeof(Rectangle))]
    public class MetroWindow : Window
    {
        private const string PART_Icon = "PART_Icon";
        private const string PART_TitleBar = "PART_TitleBar";
        private const string PART_WindowTitleBackground = "PART_WindowTitleBackground";
        private const string PART_LeftWindowCommands = "PART_LeftWindowCommands";
        private const string PART_RightWindowCommands = "PART_RightWindowCommands";
        private const string PART_WindowButtonCommands = "PART_WindowButtonCommands";
        private const string PART_OverlayBox = "PART_OverlayBox";
        private const string PART_MetroDialogContainer = "PART_MetroDialogContainer";
        private const string PART_FlyoutModal = "PART_FlyoutModal";

        public static readonly DependencyProperty ShowIconOnTitleBarProperty = DependencyProperty.Register("ShowIconOnTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty IconEdgeModeProperty = DependencyProperty.Register("IconEdgeMode", typeof(EdgeMode), typeof(MetroWindow), new PropertyMetadata(EdgeMode.Aliased));
        public static readonly DependencyProperty IconBitmapScalingModeProperty = DependencyProperty.Register("IconBitmapScalingMode", typeof(BitmapScalingMode), typeof(MetroWindow), new PropertyMetadata(BitmapScalingMode.HighQuality));
        public static readonly DependencyProperty ShowTitleBarProperty = DependencyProperty.Register("ShowTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true, OnShowTitleBarPropertyChangedCallback, OnShowTitleBarCoerceValueCallback));

        public static readonly DependencyProperty ShowMinButtonProperty = DependencyProperty.Register("ShowMinButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowMaxRestoreButtonProperty = DependencyProperty.Register("ShowMaxRestoreButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty ShowWindowButtonCommandsOnHiddenTitleBarProperty = DependencyProperty.Register("ShowWindowButtonCommandsOnHiddenTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty ShowSystemMenuOnRightClickProperty = DependencyProperty.Register("ShowSystemMenuOnRightClick", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty TitlebarHeightProperty = DependencyProperty.Register("TitlebarHeight", typeof(int), typeof(MetroWindow), new PropertyMetadata(30, TitlebarHeightPropertyChangedCallback));
        public static readonly DependencyProperty TitleCapsProperty = DependencyProperty.Register("TitleCaps", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty SaveWindowPositionProperty = DependencyProperty.Register("SaveWindowPosition", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false));
        public static readonly DependencyProperty WindowPlacementSettingsProperty = DependencyProperty.Register("WindowPlacementSettings", typeof(IWindowPlacementSettings), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(MetroWindow));
        public static readonly DependencyProperty IgnoreTaskbarOnMaximizeProperty = DependencyProperty.Register("IgnoreTaskbarOnMaximize", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false));
        public static readonly DependencyProperty FlyoutsProperty = DependencyProperty.Register("Flyouts", typeof(FlyoutsControl), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty WindowTransitionsEnabledProperty = DependencyProperty.Register("WindowTransitionsEnabled", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty MetroDialogOptionsProperty = DependencyProperty.Register("MetroDialogOptions", typeof(MetroDialogSettings), typeof(MetroWindow), new PropertyMetadata(new MetroDialogSettings()));

        public static readonly DependencyProperty WindowTitleBrushProperty = DependencyProperty.Register("WindowTitleBrush", typeof(Brush), typeof(MetroWindow), new PropertyMetadata(Brushes.Transparent));
        public static readonly DependencyProperty GlowBrushProperty = DependencyProperty.Register("GlowBrush", typeof(SolidColorBrush), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty NonActiveGlowBrushProperty = DependencyProperty.Register("NonActiveGlowBrush", typeof(SolidColorBrush), typeof(MetroWindow), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(153, 153, 153)))); // #999999
        public static readonly DependencyProperty NonActiveBorderBrushProperty = DependencyProperty.Register("NonActiveBorderBrush", typeof(Brush), typeof(MetroWindow), new PropertyMetadata(Brushes.Gray));
        public static readonly DependencyProperty NonActiveWindowTitleBrushProperty = DependencyProperty.Register("NonActiveWindowTitleBrush", typeof(Brush), typeof(MetroWindow), new PropertyMetadata(Brushes.Gray));

        public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(MetroWindow), new PropertyMetadata(null));
        
        public static readonly DependencyProperty LeftWindowCommandsProperty = DependencyProperty.Register("LeftWindowCommands", typeof(WindowCommands), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty RightWindowCommandsProperty = DependencyProperty.Register("RightWindowCommands", typeof(WindowCommands), typeof(MetroWindow), new PropertyMetadata(null));
        [Obsolete("This property is obsolete and will be delete in next release, use RightWindowCommands instead.")]
        public static readonly DependencyProperty WindowCommandsProperty = DependencyProperty.Register("WindowCommands", typeof(WindowCommands), typeof(MetroWindow), new PropertyMetadata(null, WindowCommandsPropertyChangedCallback));
        public static readonly DependencyProperty ShowWindowCommandsOnTopProperty = DependencyProperty.Register("ShowWindowCommandsOnTop", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty WindowMinButtonStyleProperty = DependencyProperty.Register("WindowMinButtonStyle", typeof(Style), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty WindowMaxButtonStyleProperty = DependencyProperty.Register("WindowMaxButtonStyle", typeof(Style), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty WindowCloseButtonStyleProperty = DependencyProperty.Register("WindowCloseButtonStyle", typeof(Style), typeof(MetroWindow), new PropertyMetadata(null));
        
        [Obsolete("This propery isn't needed anymore, it will be deleted in next release...")]
        public static readonly DependencyProperty TextBlockStyleProperty = DependencyProperty.Register("TextBlockStyle", typeof(Style), typeof(MetroWindow), new PropertyMetadata(default(Style)));
        public static readonly DependencyProperty UseNoneWindowStyleProperty = DependencyProperty.Register("UseNoneWindowStyle", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false, OnUseNoneWindowStylePropertyChangedCallback));
        public static readonly DependencyProperty OverrideDefaultWindowCommandsBrushProperty = DependencyProperty.Register("OverrideDefaultWindowCommandsBrush", typeof(SolidColorBrush), typeof(MetroWindow));

        public static readonly DependencyProperty EnableDWMDropShadowProperty = DependencyProperty.Register("EnableDWMDropShadow", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false));

        UIElement icon;
        UIElement titleBar;
        UIElement titleBarBackground;
        internal ContentPresenter LeftWindowCommandsPresenter;
        internal ContentPresenter RightWindowCommandsPresenter;
        internal WindowButtonCommands WindowButtonCommands;
        
        internal Grid overlayBox;
        internal Grid metroDialogContainer;
        private Storyboard overlayStoryboard;
        Rectangle flyoutModal;

        public static readonly RoutedEvent FlyoutsStatusChangedEvent = EventManager.RegisterRoutedEvent(
            "FlyoutsStatusChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MetroWindow));

        // Provide CLR accessors for the event 
        public event RoutedEventHandler FlyoutsStatusChanged
        {
            add { AddHandler(FlyoutsStatusChangedEvent, value); }
            remove { RemoveHandler(FlyoutsStatusChangedEvent, value); }
        }

        /// <summary>
        /// CleanWindow sets this so it has the correct default window commands brush
        /// </summary>
        public SolidColorBrush OverrideDefaultWindowCommandsBrush
        {
            get { return (SolidColorBrush)this.GetValue(OverrideDefaultWindowCommandsBrushProperty); }
            set { this.SetValue(OverrideDefaultWindowCommandsBrushProperty, value); }
        }

        public MetroDialogSettings MetroDialogOptions
        {
            get { return (MetroDialogSettings)GetValue(MetroDialogOptionsProperty); }
            set { SetValue(MetroDialogOptionsProperty, value); }
        }


        [Obsolete("This propery isn't needed anymore, it will be deleted in next release...")]
        public Style TextBlockStyle
        {
            get { return (Style)this.GetValue(TextBlockStyleProperty); }
            set { SetValue(TextBlockStyleProperty, value); }
        }

        public bool EnableDWMDropShadow
        {
            get { return (bool)GetValue(EnableDWMDropShadowProperty); }
            set { SetValue(EnableDWMDropShadowProperty, value); }
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
        /// Gets/sets the style for the MIN button style.
        /// </summary>
        public Style WindowMinButtonStyle
        {
            get { return (Style)this.GetValue(WindowMinButtonStyleProperty); }
            set { SetValue(WindowMinButtonStyleProperty, value); }
        }

        /// <summary>
        /// Gets/sets the style for the MAX button style.
        /// </summary>
        public Style WindowMaxButtonStyle
        {
            get { return (Style)this.GetValue(WindowMaxButtonStyleProperty); }
            set { SetValue(WindowMaxButtonStyleProperty, value); }
        }

        /// <summary>
        /// Gets/sets the style for the CLOSE button style.
        /// </summary>
        public Style WindowCloseButtonStyle
        {
            get { return (Style)this.GetValue(WindowCloseButtonStyleProperty); }
            set { SetValue(WindowCloseButtonStyleProperty, value); }
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

        /// <summary>
        /// Gets/sets the icon content template to show a custom icon.
        /// </summary>
        public DataTemplate IconTemplate
        {
            get { return (DataTemplate)GetValue(IconTemplateProperty); }
            set { SetValue(IconTemplateProperty, value); }
        }

        /// <summary>
        /// Gets/sets the title content template to show a custom title.
        /// </summary>
        public DataTemplate TitleTemplate
        {
            get { return (DataTemplate)GetValue(TitleTemplateProperty); }
            set { SetValue(TitleTemplateProperty, value); }
        }

        /// <summary>
        /// Gets/sets the left window commands that hosts the user commands.
        /// </summary>
        public WindowCommands LeftWindowCommands
        {
            get { return (WindowCommands)GetValue(LeftWindowCommandsProperty); }
            set { SetValue(LeftWindowCommandsProperty, value); }
        }

        /// <summary>
        /// Gets/sets the right window commands that hosts the user commands.
        /// </summary>
        public WindowCommands RightWindowCommands
        {
            get { return (WindowCommands)GetValue(RightWindowCommandsProperty); }
            set { SetValue(RightWindowCommandsProperty, value); }
        }

        /// <summary>
        /// Gets/sets the right window commands that hosts the user commands.
        /// </summary>
        [Obsolete("This property is obsolete and will be delete in next release, use RightWindowCommands instead.")]
        public WindowCommands WindowCommands
        {
            get { return (WindowCommands)GetValue(WindowCommandsProperty); }
            set { SetValue(WindowCommandsProperty, value); }
        }

        private static void WindowCommandsPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue != null)
            {
                ((MetroWindow)dependencyObject).RightWindowCommands = (WindowCommands)e.NewValue;
            }
        }

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
        /// Gets/sets edge mode of the titlebar icon.
        /// </summary>
        public EdgeMode IconEdgeMode
        {
            get { return (EdgeMode)this.GetValue(IconEdgeModeProperty); }
            set { SetValue(IconEdgeModeProperty, value); }
        }

        /// <summary>
        /// Gets/sets bitmap scaling mode of the titlebar icon.
        /// </summary>
        public BitmapScalingMode IconBitmapScalingMode
        {
            get { return (BitmapScalingMode)this.GetValue(IconBitmapScalingModeProperty); }
            set { SetValue(IconBitmapScalingModeProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether the TitleBar is visible or not.
        /// </summary>
        public bool ShowTitleBar
        {
            get { return (bool)GetValue(ShowTitleBarProperty); }
            set { SetValue(ShowTitleBarProperty, value); }
        }

        private static void OnShowTitleBarPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (MetroWindow)d;
            if (e.NewValue != e.OldValue)
            {
                window.ToggleVisibiltyForAllTitleElements((bool)e.NewValue);
            }
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
        /// Gets/sets if the Maximize/Restore button is visible.
        /// </summary>
        public bool ShowMaxRestoreButton
        {
            get { return (bool)GetValue(ShowMaxRestoreButtonProperty); }
            set { SetValue(ShowMaxRestoreButtonProperty, value); }
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
        /// Gets/sets whether the Window Button Commands will show if the TitleBar is hidden.
        /// </summary>
        public bool ShowWindowButtonCommandsOnHiddenTitleBar
        {
            get { return (bool)GetValue(ShowWindowButtonCommandsOnHiddenTitleBarProperty); }
            set { SetValue(ShowWindowButtonCommandsOnHiddenTitleBarProperty, value); }
        }

        /// <summary>
        /// Gets/sets if the the system menu should popup on right click.
        /// </summary>
        public bool ShowSystemMenuOnRightClick
        {
            get { return (bool)GetValue(ShowSystemMenuOnRightClickProperty); }
            set { SetValue(ShowSystemMenuOnRightClickProperty, value); }
        }

        /// <summary>
        /// Gets/sets the TitleBar's height.
        /// </summary>
        public int TitlebarHeight
        {
            get { return (int)GetValue(TitlebarHeightProperty); }
            set { SetValue(TitlebarHeightProperty, value); }
        }

        private static void TitlebarHeightPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var window = (MetroWindow)dependencyObject;
            if (e.NewValue != e.OldValue)
            {
                window.ToggleVisibiltyForAllTitleElements((int)e.NewValue > 0);
            }
        }

        private void ToggleVisibiltyForAllTitleElements(bool visible)
        {
            var newVisibility = visible && this.ShowTitleBar ? Visibility.Visible : Visibility.Collapsed;
            if (this.icon != null)
            {
                var iconVisibility = visible && this.ShowTitleBar && this.ShowIconOnTitleBar ? Visibility.Visible : Visibility.Collapsed;
                this.icon.Visibility = iconVisibility;
            }
            if (this.titleBar != null)
            {
                this.titleBar.Visibility = newVisibility;
            }
            if (this.titleBarBackground != null)
            {
                this.titleBarBackground.Visibility = newVisibility;
            }
            if (this.LeftWindowCommandsPresenter != null)
            {
                this.LeftWindowCommandsPresenter.Visibility = newVisibility;
            }
            if (this.RightWindowCommandsPresenter != null)
            {
                this.RightWindowCommandsPresenter.Visibility = newVisibility;
            }
            if (this.WindowButtonCommands != null)
            {
                var windowCommandsVisibility = ShowWindowButtonCommandsOnHiddenTitleBar ? Visibility.Visible : newVisibility;
                this.WindowButtonCommands.Visibility = windowCommandsVisibility;
            }

            SetWindowEvents();
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
        /// Gets/sets the brush used for the Window's title bar.
        /// </summary>
        public Brush WindowTitleBrush
        {
            get { return (Brush)GetValue(WindowTitleBrushProperty); }
            set { SetValue(WindowTitleBrushProperty, value); }
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
        /// Gets/sets the brush used for the Window's non-active glow.
        /// </summary>
        public SolidColorBrush NonActiveGlowBrush
        {
            get { return (SolidColorBrush)GetValue(NonActiveGlowBrushProperty); }
            set { SetValue(NonActiveGlowBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the brush used for the Window's non-active border.
        /// </summary>
        public Brush NonActiveBorderBrush
        {
            get { return (Brush)GetValue(NonActiveBorderBrushProperty); }
            set { SetValue(NonActiveBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the brush used for the Window's non-active title bar.
        /// </summary>
        public Brush NonActiveWindowTitleBrush
        {
            get { return (Brush)GetValue(NonActiveWindowTitleBrushProperty); }
            set { SetValue(NonActiveWindowTitleBrushProperty, value); }
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

            overlayBox.Visibility = Visibility.Visible;

            var tcs = new System.Threading.Tasks.TaskCompletionSource<object>();

            var sb = (Storyboard) this.Template.Resources["OverlayFastSemiFadeIn"];

            sb = sb.Clone();

            EventHandler completionHandler = null;
            completionHandler = (sender, args) =>
            {
                sb.Completed -= completionHandler;

                if (overlayStoryboard == sb)
                {
                    overlayStoryboard = null;
                }

                tcs.TrySetResult(null);
            };

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
            if (overlayBox.Visibility == Visibility.Visible && overlayBox.Opacity == 0.0)
                return new System.Threading.Tasks.Task(() => { }); //No Task.FromResult in .NET 4.

            Dispatcher.VerifyAccess();

            var tcs = new System.Threading.Tasks.TaskCompletionSource<object>();

            var sb = (Storyboard) this.Template.Resources["OverlayFastSemiFadeOut"];

            sb = sb.Clone();

            EventHandler completionHandler = null;
            completionHandler = (sender, args) =>
            {
                sb.Completed -= completionHandler;

                if (overlayStoryboard == sb)
                {
                    overlayBox.Visibility = Visibility.Hidden;
                    overlayStoryboard = null;
                }

                tcs.TrySetResult(null);
            };

            sb.Completed += completionHandler;

            overlayBox.BeginStoryboard(sb);

            overlayStoryboard = sb;

            return tcs.Task;
        }
        public bool IsOverlayVisible()
        {
            return overlayBox.Visibility == Visibility.Visible && overlayBox.Opacity >= 0.7;
        }
        public void ShowOverlay()
        {
            overlayBox.Visibility = Visibility.Visible;
            //overlayBox.Opacity = 0.7;
            overlayBox.SetCurrentValue(Grid.OpacityProperty, 0.7);
        }
        public void HideOverlay()
        {
            //overlayBox.Opacity = 0.0;
            overlayBox.SetCurrentValue(Grid.OpacityProperty, 0.0);
            overlayBox.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Initializes a new instance of the MahApps.Metro.Controls.MetroWindow class.
        /// </summary>
        public MetroWindow()
        {
            Loaded += this.MetroWindow_Loaded;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.WindowTransitionsEnabled)
            {
                VisualStateManager.GoToState(this, "AfterLoaded", true);
            }

            // if UseNoneWindowStyle = true no title bar, window commands or min, max, close buttons should be shown
            if (UseNoneWindowStyle)
            {
                if (LeftWindowCommandsPresenter != null)
                {
                    LeftWindowCommandsPresenter.Visibility = Visibility.Collapsed;
                }
                if (RightWindowCommandsPresenter != null)
                {
                    RightWindowCommandsPresenter.Visibility = Visibility.Collapsed;
                }
                ShowMinButton = false;
                ShowMaxRestoreButton = false;
                ShowCloseButton = false;
            }

            if (this.Flyouts == null)
            {
                this.Flyouts = new FlyoutsControl();
            }

            this.ResetAllWindowCommandsBrush();

            ThemeManager.IsThemeChanged += ThemeManagerOnIsThemeChanged;
            this.Unloaded += (o, args) => ThemeManager.IsThemeChanged -= ThemeManagerOnIsThemeChanged;
        }

        private void MetroWindow_SizeChanged(object sender, RoutedEventArgs e)
        {
            // this all works only for CleanWindow style
            
            var titleBarGrid = titleBar as Grid;
            var titleBarLabel = titleBarGrid.Children[0] as Label;
            var titleControl = titleBarLabel.Content as ContentControl;
            var iconContentControl = icon as ContentControl;

            // Half of this MetroWindow
            var halfDistance = this.Width / 2;
            // Distance between center and left/right
            var distanceToCenter = titleControl.ActualWidth / 2;
            // Distance between right edge from LeftWindowCommands to left window side
            var distanceFromLeft = iconContentControl.ActualWidth + LeftWindowCommands.ActualWidth;
            // Distance between left edge from RightWindowCommands to right window side
            var distanceFromRight = WindowButtonCommands.ActualWidth + RightWindowCommands.ActualWidth;
            // Margin
            const double horizontalMargin = 5.0;

            if ((distanceFromLeft + distanceToCenter + horizontalMargin < halfDistance) && (distanceFromRight + distanceToCenter + horizontalMargin < halfDistance))
            {
                Grid.SetColumn(titleBarGrid, 0);
                Grid.SetColumnSpan(titleBarGrid, 5);
            }
            else
            {
                Grid.SetColumn(titleBarGrid, 2);
                Grid.SetColumnSpan(titleBarGrid, 1);
            }
        }

        private void ThemeManagerOnIsThemeChanged(object sender, OnThemeChangedEventArgs e)
        {
            if (e.Accent != null)
            {
                var flyouts = this.Flyouts.GetFlyouts().ToList();
                // since we disabled the ThemeManager OnThemeChanged part, we must change all children flyouts too
                // e.g if the FlyoutsControl is hosted in a UserControl
                var allChildFlyouts = (this.Content as DependencyObject).FindChildren<FlyoutsControl>(true).ToList();
                if (allChildFlyouts.Any())
                {
                    flyouts.AddRange(allChildFlyouts.SelectMany(flyoutsControl => flyoutsControl.GetFlyouts()));
                }

                if (!flyouts.Any())
                {
                    // we must update the window command brushes!!!
                    this.ResetAllWindowCommandsBrush();
                    return;
                }

                foreach (var flyout in flyouts)
                {
                    flyout.ChangeFlyoutTheme(e.Accent, e.AppTheme);
                }
                this.HandleWindowCommandsForFlyouts(flyouts);
            }
        }
        
        private void FlyoutsPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (e.OriginalSource as FrameworkElement);
            if (element != null && element.TryFindParent<Flyout>() != null)
            {
                return;
            }
            
            if (Flyouts.OverrideExternalCloseButton == null)
            {
                foreach (Flyout flyout in Flyouts.GetFlyouts().Where(x => x.IsOpen && x.ExternalCloseButton == e.ChangedButton && (!x.IsPinned || Flyouts.OverrideIsPinned)))
                {
                    flyout.IsOpen = false;
                    e.Handled = true;
                }
            }

            else if (Flyouts.OverrideExternalCloseButton == e.ChangedButton)
            {
                foreach (Flyout flyout in Flyouts.GetFlyouts().Where(x => x.IsOpen && (!x.IsPinned || Flyouts.OverrideIsPinned)))
                {
                    flyout.IsOpen = false;
                    e.Handled = true;
                }
            }
        }

        static MetroWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroWindow), new FrameworkPropertyMetadata(typeof(MetroWindow)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (LeftWindowCommands == null)
                LeftWindowCommands = new WindowCommands();
            if (RightWindowCommands == null)
                RightWindowCommands = new WindowCommands();

            LeftWindowCommandsPresenter = GetTemplateChild(PART_LeftWindowCommands) as ContentPresenter;
            RightWindowCommandsPresenter = GetTemplateChild(PART_RightWindowCommands) as ContentPresenter;
            WindowButtonCommands = GetTemplateChild(PART_WindowButtonCommands) as WindowButtonCommands;

            overlayBox = GetTemplateChild(PART_OverlayBox) as Grid;
            metroDialogContainer = GetTemplateChild(PART_MetroDialogContainer) as Grid;
            flyoutModal = GetTemplateChild(PART_FlyoutModal) as Rectangle;
            flyoutModal.PreviewMouseDown += FlyoutsPreviewMouseDown;
            this.PreviewMouseDown += FlyoutsPreviewMouseDown;

            icon = GetTemplateChild(PART_Icon) as UIElement;
            titleBar = GetTemplateChild(PART_TitleBar) as UIElement;
            titleBarBackground = GetTemplateChild(PART_WindowTitleBackground) as UIElement;

            this.ToggleVisibiltyForAllTitleElements(this.TitlebarHeight > 0);
        }

        private void ClearWindowEvents()
        {
            // clear all event handlers first:
            if (titleBarBackground != null)
            {
                titleBarBackground.MouseDown -= TitleBarMouseDown;
                titleBarBackground.MouseUp -= TitleBarMouseUp;
            }
            if (titleBar != null)
            {
                titleBar.MouseDown -= TitleBarMouseDown;
                titleBar.MouseUp -= TitleBarMouseUp;
            }
            if (icon != null)
            {
                icon.MouseDown -= IconMouseDown;
            }
            MouseDown -= TitleBarMouseDown;
            MouseUp -= TitleBarMouseUp;
            SizeChanged -= MetroWindow_SizeChanged;
        }

        private void SetWindowEvents()
        {
            // clear all event handlers first
            this.ClearWindowEvents();

            // set mouse down/up for icon
            if (icon != null && icon.Visibility == Visibility.Visible)
            {
                icon.MouseDown += IconMouseDown;
            }

            // handle mouse events for PART_WindowTitleBackground -> MetroWindow
            if (titleBarBackground != null && titleBarBackground.Visibility == Visibility.Visible)
            {
                titleBarBackground.MouseDown += TitleBarMouseDown;
                titleBarBackground.MouseUp += TitleBarMouseUp;
            }

            // handle mouse events for PART_TitleBar -> MetroWindow and CleanWindow
            if (titleBar != null && titleBar.Visibility == Visibility.Visible)
            {
                titleBar.MouseDown += TitleBarMouseDown;
                titleBar.MouseUp += TitleBarMouseUp;

                // special title resizing for CleanWindow title
                if (titleBar.GetType() == typeof(Grid))
                {
                    SizeChanged += MetroWindow_SizeChanged;
                }
            }
            else
            {
                // handle mouse events for windows without PART_WindowTitleBackground or PART_TitleBar
                MouseDown += TitleBarMouseDown;
                MouseUp += TitleBarMouseUp;
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowButtonCommands != null && !this.UseNoneWindowStyle)
            {
                WindowButtonCommands.RefreshMaximiseIconState();
            }

            base.OnStateChanged(e);
        }

        private void IconMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
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
        }

        protected void TitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && !this.UseNoneWindowStyle)
            {
                // if UseNoneWindowStyle = true no movement, no maximize please
                IntPtr windowHandle = new WindowInteropHelper(this).Handle;
                UnsafeNativeMethods.ReleaseCapture();

                var mPoint = Mouse.GetPosition(this);

                var wpfPoint = this.PointToScreen(mPoint);
                var x = Convert.ToInt16(wpfPoint.X);
                var y = Convert.ToInt16(wpfPoint.Y);
                var lParam = x | (y << 16);
                UnsafeNativeMethods.SendMessage(windowHandle, Constants.WM_NCLBUTTONDOWN, Constants.HT_CAPTION, lParam);

                if (e.ClickCount == 2 && (this.ResizeMode == ResizeMode.CanResizeWithGrip || this.ResizeMode == ResizeMode.CanResize) && mPoint.Y <= this.TitlebarHeight && this.TitlebarHeight > 0)
                {
                    if (this.WindowState == WindowState.Maximized)
                    {
                        Microsoft.Windows.Shell.SystemCommands.RestoreWindow(this);
                    }
                    else
                    {
                        Microsoft.Windows.Shell.SystemCommands.MaximizeWindow(this);
                    }
                }
            }
        }

        protected void TitleBarMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ShowSystemMenuOnRightClick)
            {
                var mousePosition = e.GetPosition(this);
                if (e.ChangedButton == MouseButton.Right && (UseNoneWindowStyle || mousePosition.Y <= TitlebarHeight))
                {
                    ShowSystemMenuPhysicalCoordinates(this, PointToScreen(mousePosition));
                }
            }
        }

        internal T GetPart<T>(string name) where T : DependencyObject
        {
            return GetTemplateChild(name) as T;
        }

        private static void ShowSystemMenuPhysicalCoordinates(Window window, Point physicalScreenLocation)
        {
            if (window == null) return;

            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero || !UnsafeNativeMethods.IsWindow(hwnd))
                return;

            var hmenu = UnsafeNativeMethods.GetSystemMenu(hwnd, false);

            var cmd = UnsafeNativeMethods.TrackPopupMenuEx(hmenu, Constants.TPM_LEFTBUTTON | Constants.TPM_RETURNCMD, 
                (int)physicalScreenLocation.X, (int)physicalScreenLocation.Y, hwnd, IntPtr.Zero);
            if (0 != cmd)
                UnsafeNativeMethods.PostMessage(hwnd, Constants.SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
        }

        internal void HandleFlyoutStatusChange(Flyout flyout, IEnumerable<Flyout> visibleFlyouts)
        {
            //checks a recently opened flyout's position.
            if (flyout.Position == Position.Left || flyout.Position == Position.Right || flyout.Position == Position.Top)
            {
                //get it's zindex
                var zIndex = flyout.IsOpen ? Panel.GetZIndex(flyout) + 3 : visibleFlyouts.Count() + 2;

                //if ShowWindowCommandsOnTop is true, set the window commands' and icon zindex to a number that is higher than the flyout's. 
                if (icon != null)
                {
                    icon.SetValue(Panel.ZIndexProperty, this.ShowWindowCommandsOnTop ? zIndex : 1);
                }
                if (LeftWindowCommandsPresenter != null)
                {
                    LeftWindowCommandsPresenter.SetValue(Panel.ZIndexProperty, this.ShowWindowCommandsOnTop ? zIndex : 1);
                }
                if (RightWindowCommandsPresenter != null)
                {
                    RightWindowCommandsPresenter.SetValue(Panel.ZIndexProperty, this.ShowWindowCommandsOnTop ? zIndex : 1);
                }
                if (WindowButtonCommands != null)
                {
                    WindowButtonCommands.SetValue(Panel.ZIndexProperty, this.ShowWindowCommandsOnTop ? zIndex : 1);
                }

                this.HandleWindowCommandsForFlyouts(visibleFlyouts);
            }

            flyoutModal.Visibility = visibleFlyouts.Any(x => x.IsModal) ? Visibility.Visible : Visibility.Hidden;

            RaiseEvent(new FlyoutStatusChangedRoutedEventArgs(FlyoutsStatusChangedEvent, this)
            {
                ChangedFlyout = flyout
            });
        }

        public class FlyoutStatusChangedRoutedEventArgs : RoutedEventArgs
        {
            internal FlyoutStatusChangedRoutedEventArgs(RoutedEvent rEvent, object source): base(rEvent, source)
            { }

            public Flyout ChangedFlyout { get; internal set; }
        }
    }
}
