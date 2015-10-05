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
    [TemplatePart(Name = PART_MetroActiveDialogContainer, Type = typeof(Grid))]
    [TemplatePart(Name = PART_MetroInactiveDialogsContainer, Type = typeof(Grid))]
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
        private const string PART_MetroActiveDialogContainer = "PART_MetroActiveDialogContainer";
        private const string PART_MetroInactiveDialogsContainer = "PART_MetroInactiveDialogsContainer";
        private const string PART_FlyoutModal = "PART_FlyoutModal";

        public static readonly DependencyProperty ShowIconOnTitleBarProperty = DependencyProperty.Register("ShowIconOnTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true, OnShowIconOnTitleBarPropertyChangedCallback));
        public static readonly DependencyProperty IconEdgeModeProperty = DependencyProperty.Register("IconEdgeMode", typeof(EdgeMode), typeof(MetroWindow), new PropertyMetadata(EdgeMode.Aliased));
        public static readonly DependencyProperty IconBitmapScalingModeProperty = DependencyProperty.Register("IconBitmapScalingMode", typeof(BitmapScalingMode), typeof(MetroWindow), new PropertyMetadata(BitmapScalingMode.HighQuality));
        public static readonly DependencyProperty ShowTitleBarProperty = DependencyProperty.Register("ShowTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true, OnShowTitleBarPropertyChangedCallback, OnShowTitleBarCoerceValueCallback));

        public static readonly DependencyProperty ShowMinButtonProperty = DependencyProperty.Register("ShowMinButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowMaxRestoreButtonProperty = DependencyProperty.Register("ShowMaxRestoreButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty IsMinButtonEnabledProperty = DependencyProperty.Register("IsMinButtonEnabled", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty IsMaxRestoreButtonEnabledProperty = DependencyProperty.Register("IsMaxRestoreButtonEnabled", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty IsCloseButtonEnabledProperty = DependencyProperty.Register("IsCloseButtonEnabled", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty ShowSystemMenuOnRightClickProperty = DependencyProperty.Register("ShowSystemMenuOnRightClick", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty TitlebarHeightProperty = DependencyProperty.Register("TitlebarHeight", typeof(int), typeof(MetroWindow), new PropertyMetadata(30, TitlebarHeightPropertyChangedCallback));
        public static readonly DependencyProperty TitleCapsProperty = DependencyProperty.Register("TitleCaps", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty TitleAlignmentProperty = DependencyProperty.Register("TitleAlignment", typeof(HorizontalAlignment), typeof(MetroWindow), new PropertyMetadata(HorizontalAlignment.Stretch));
        public static readonly DependencyProperty SaveWindowPositionProperty = DependencyProperty.Register("SaveWindowPosition", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false));
        public static readonly DependencyProperty WindowPlacementSettingsProperty = DependencyProperty.Register("WindowPlacementSettings", typeof(IWindowPlacementSettings), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(MetroWindow));
        public static readonly DependencyProperty IgnoreTaskbarOnMaximizeProperty = DependencyProperty.Register("IgnoreTaskbarOnMaximize", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false));
        public static readonly DependencyProperty FlyoutsProperty = DependencyProperty.Register("Flyouts", typeof(FlyoutsControl), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty WindowTransitionsEnabledProperty = DependencyProperty.Register("WindowTransitionsEnabled", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty MetroDialogOptionsProperty = DependencyProperty.Register("MetroDialogOptions", typeof(MetroDialogSettings), typeof(MetroWindow), new PropertyMetadata(new MetroDialogSettings()));

        public static readonly DependencyProperty WindowTitleBrushProperty = DependencyProperty.Register("WindowTitleBrush", typeof(Brush), typeof(MetroWindow), new PropertyMetadata(Brushes.Transparent));
        public static readonly DependencyProperty GlowBrushProperty = DependencyProperty.Register("GlowBrush", typeof(Brush), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty NonActiveGlowBrushProperty = DependencyProperty.Register("NonActiveGlowBrush", typeof(Brush), typeof(MetroWindow), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(153, 153, 153)))); // #999999
        public static readonly DependencyProperty NonActiveBorderBrushProperty = DependencyProperty.Register("NonActiveBorderBrush", typeof(Brush), typeof(MetroWindow), new PropertyMetadata(Brushes.Gray));
        public static readonly DependencyProperty NonActiveWindowTitleBrushProperty = DependencyProperty.Register("NonActiveWindowTitleBrush", typeof(Brush), typeof(MetroWindow), new PropertyMetadata(Brushes.Gray));

        public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(MetroWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty LeftWindowCommandsProperty = DependencyProperty.Register("LeftWindowCommands", typeof(WindowCommands), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty RightWindowCommandsProperty = DependencyProperty.Register("RightWindowCommands", typeof(WindowCommands), typeof(MetroWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty LeftWindowCommandsOverlayBehaviorProperty = DependencyProperty.Register("LeftWindowCommandsOverlayBehavior", typeof(WindowCommandsOverlayBehavior), typeof(MetroWindow), new PropertyMetadata(WindowCommandsOverlayBehavior.Always));
        public static readonly DependencyProperty RightWindowCommandsOverlayBehaviorProperty = DependencyProperty.Register("RightWindowCommandsOverlayBehavior", typeof(WindowCommandsOverlayBehavior), typeof(MetroWindow), new PropertyMetadata(WindowCommandsOverlayBehavior.Always));
        public static readonly DependencyProperty WindowButtonCommandsOverlayBehaviorProperty = DependencyProperty.Register("WindowButtonCommandsOverlayBehavior", typeof(WindowCommandsOverlayBehavior), typeof(MetroWindow), new PropertyMetadata(WindowCommandsOverlayBehavior.Always));
        public static readonly DependencyProperty IconOverlayBehaviorProperty = DependencyProperty.Register("IconOverlayBehavior", typeof(WindowCommandsOverlayBehavior), typeof(MetroWindow), new PropertyMetadata(WindowCommandsOverlayBehavior.Never));

        [Obsolete(@"This property will be deleted in the next release. You should use LightMinButtonStyle or DarkMinButtonStyle in WindowButtonCommands to override the style.")]
        public static readonly DependencyProperty WindowMinButtonStyleProperty = DependencyProperty.Register("WindowMinButtonStyle", typeof(Style), typeof(MetroWindow), new PropertyMetadata(null, OnWindowButtonStyleChanged));
        [Obsolete(@"This property will be deleted in the next release. You should use LightMaxButtonStyle or DarkMaxButtonStyle in WindowButtonCommands to override the style.")]
        public static readonly DependencyProperty WindowMaxButtonStyleProperty = DependencyProperty.Register("WindowMaxButtonStyle", typeof(Style), typeof(MetroWindow), new PropertyMetadata(null, OnWindowButtonStyleChanged));
        [Obsolete(@"This property will be deleted in the next release. You should use LightCloseButtonStyle or DarkCloseButtonStyle in WindowButtonCommands to override the style.")]
        public static readonly DependencyProperty WindowCloseButtonStyleProperty = DependencyProperty.Register("WindowCloseButtonStyle", typeof(Style), typeof(MetroWindow), new PropertyMetadata(null, OnWindowButtonStyleChanged));

        public static readonly DependencyProperty UseNoneWindowStyleProperty = DependencyProperty.Register("UseNoneWindowStyle", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false, OnUseNoneWindowStylePropertyChangedCallback));
        public static readonly DependencyProperty OverrideDefaultWindowCommandsBrushProperty = DependencyProperty.Register("OverrideDefaultWindowCommandsBrush", typeof(SolidColorBrush), typeof(MetroWindow));

        [Obsolete(@"This property will be deleted in the next release. You should use BorderThickness=""0"" and a GlowBrush=""Black"" to get a drop shadow around the Window.")]
        public static readonly DependencyProperty EnableDWMDropShadowProperty = DependencyProperty.Register("EnableDWMDropShadow", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false, OnEnableDWMDropShadowPropertyChangedCallback));
        public static readonly DependencyProperty IsWindowDraggableProperty = DependencyProperty.Register("IsWindowDraggable", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        UIElement icon;
        UIElement titleBar;
        UIElement titleBarBackground;
        internal ContentPresenter LeftWindowCommandsPresenter;
        internal ContentPresenter RightWindowCommandsPresenter;
        internal WindowButtonCommands WindowButtonCommands;

        internal Grid overlayBox;
        internal Grid metroActiveDialogContainer;
        internal Grid metroInactiveDialogContainer;
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
        /// Allows easy handling of window commands brush. Theme is also applied based on this brush.
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

        [Obsolete(@"This property will be deleted in the next release. You should use BorderThickness=""0"" and a GlowBrush=""Black"" to get a drop shadow around the Window.")]
        public bool EnableDWMDropShadow
        {
            get { return (bool)GetValue(EnableDWMDropShadowProperty); }
            set { SetValue(EnableDWMDropShadowProperty, value); }
        }

        private static void OnEnableDWMDropShadowPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && (bool)e.NewValue)
            {
                var window = (MetroWindow)d;
                window.UseDropShadow();
            }
        }

        private void UseDropShadow()
        {
            this.BorderThickness = new Thickness(0);
            this.BorderBrush = null;
            this.GlowBrush = Brushes.Black;
        }

        public bool IsWindowDraggable
        {
            get { return (bool)GetValue(IsWindowDraggableProperty); }
            set { SetValue(IsWindowDraggableProperty, value); }
        }

        public WindowCommandsOverlayBehavior LeftWindowCommandsOverlayBehavior
        {
            get { return (WindowCommandsOverlayBehavior)this.GetValue(LeftWindowCommandsOverlayBehaviorProperty); }
            set { SetValue(LeftWindowCommandsOverlayBehaviorProperty, value); }
        }

        public WindowCommandsOverlayBehavior RightWindowCommandsOverlayBehavior
        {
            get { return (WindowCommandsOverlayBehavior)this.GetValue(RightWindowCommandsOverlayBehaviorProperty); }
            set { SetValue(RightWindowCommandsOverlayBehaviorProperty, value); }
        }

        public WindowCommandsOverlayBehavior WindowButtonCommandsOverlayBehavior
        {
            get { return (WindowCommandsOverlayBehavior)this.GetValue(WindowButtonCommandsOverlayBehaviorProperty); }
            set { SetValue(WindowButtonCommandsOverlayBehaviorProperty, value); }
        }

        public WindowCommandsOverlayBehavior IconOverlayBehavior
        {
            get { return (WindowCommandsOverlayBehavior)this.GetValue(IconOverlayBehaviorProperty); }
            set { SetValue(IconOverlayBehaviorProperty, value); }
        }

        [Obsolete(@"This property will be deleted in the next release. You should use LightMinButtonStyle or DarkMinButtonStyle in WindowButtonCommands to override the style.")]
        public Style WindowMinButtonStyle
        {
            get { return (Style)this.GetValue(WindowMinButtonStyleProperty); }
            set { SetValue(WindowMinButtonStyleProperty, value); }
        }

        [Obsolete(@"This property will be deleted in the next release. You should use LightMaxButtonStyle or DarkMaxButtonStyle in WindowButtonCommands to override the style.")]
        public Style WindowMaxButtonStyle
        {
            get { return (Style)this.GetValue(WindowMaxButtonStyleProperty); }
            set { SetValue(WindowMaxButtonStyleProperty, value); }
        }

        [Obsolete(@"This property will be deleted in the next release. You should use LightCloseButtonStyle or DarkCloseButtonStyle in WindowButtonCommands to override the style.")]
        public Style WindowCloseButtonStyle
        {
            get { return (Style)this.GetValue(WindowCloseButtonStyleProperty); }
            set { SetValue(WindowCloseButtonStyleProperty, value); }
        }

        public static void OnWindowButtonStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
            {
                return;
            }

            var window = (MetroWindow) d;
            if (window.WindowButtonCommands != null)
                window.WindowButtonCommands.ApplyTheme();
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
        /// Gets the window placement settings (can be overwritten).
        /// </summary>
        public virtual IWindowPlacementSettings GetWindowPlacementSettings()
        {
            return this.WindowPlacementSettings ?? new WindowApplicationSettings(this);
        }

        /// <summary>
        /// Get/sets whether the titlebar icon is visible or not.
        /// </summary>
        public bool ShowIconOnTitleBar
        {
            get { return (bool)GetValue(ShowIconOnTitleBarProperty); }
            set { SetValue(ShowIconOnTitleBarProperty, value); }
        }

        private static void OnShowIconOnTitleBarPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (MetroWindow)d;
            if (e.NewValue != e.OldValue)
            {
                window.SetVisibiltyForIcon();
            }
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
                window.SetVisibiltyForAllTitleElements((bool)e.NewValue);
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
                var useNoneWindowStyle = (bool)e.NewValue;
                var window = (MetroWindow)d;
                window.ToggleNoneWindowStyle(useNoneWindowStyle);
            }
        }

        private void ToggleNoneWindowStyle(bool useNoneWindowStyle)
        {
            // UseNoneWindowStyle means no title bar, window commands or min, max, close buttons
            if (useNoneWindowStyle)
            {
                ShowTitleBar = false;
            }
            if (LeftWindowCommandsPresenter != null)
            {
                LeftWindowCommandsPresenter.Visibility = useNoneWindowStyle ? Visibility.Collapsed : Visibility.Visible;
            }
            if (RightWindowCommandsPresenter != null)
            {
                RightWindowCommandsPresenter.Visibility = useNoneWindowStyle ? Visibility.Collapsed : Visibility.Visible;
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
        /// Gets/sets if the min button is enabled.
        /// </summary>
        public bool IsMinButtonEnabled
        {
            get { return (bool)GetValue(IsMinButtonEnabledProperty); }
            set { SetValue(IsMinButtonEnabledProperty, value); }
        }

        /// <summary>
        /// Gets/sets if the max/restore button is enabled.
        /// </summary>
        public bool IsMaxRestoreButtonEnabled
        {
            get { return (bool)GetValue(IsMaxRestoreButtonEnabledProperty); }
            set { SetValue(IsMaxRestoreButtonEnabledProperty, value); }
        }

        /// <summary>
        /// Gets/sets if the close button is enabled.
        /// </summary>
        public bool IsCloseButtonEnabled
        {
            get { return (bool)GetValue(IsCloseButtonEnabledProperty); }
            set { SetValue(IsCloseButtonEnabledProperty, value); }
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
                window.SetVisibiltyForAllTitleElements((int)e.NewValue > 0);
            }
        }

        private void SetVisibiltyForIcon()
        {
            if (this.icon != null)
            {
                var isVisible = (this.IconOverlayBehavior.HasFlag(WindowCommandsOverlayBehavior.HiddenTitleBar) && !this.ShowTitleBar)
                                || (this.ShowIconOnTitleBar && this.ShowTitleBar);
                var iconVisibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                this.icon.Visibility = iconVisibility;
            }
        }

        private void SetVisibiltyForAllTitleElements(bool visible)
        {
            this.SetVisibiltyForIcon();
            var newVisibility = visible && this.ShowTitleBar ? Visibility.Visible : Visibility.Collapsed;
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
                this.LeftWindowCommandsPresenter.Visibility = this.LeftWindowCommandsOverlayBehavior.HasFlag(WindowCommandsOverlayBehavior.HiddenTitleBar) ?
                    Visibility.Visible : newVisibility;
            }
            if (this.RightWindowCommandsPresenter != null)
            {
                this.RightWindowCommandsPresenter.Visibility = this.RightWindowCommandsOverlayBehavior.HasFlag(WindowCommandsOverlayBehavior.HiddenTitleBar) ?
                    Visibility.Visible : newVisibility;
            }
            if (this.WindowButtonCommands != null)
            {
                this.WindowButtonCommands.Visibility = this.WindowButtonCommandsOverlayBehavior.HasFlag(WindowCommandsOverlayBehavior.HiddenTitleBar) ?
                    Visibility.Visible : newVisibility;
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
        /// Gets/sets the title horizontal alignment.
        /// </summary>
        public HorizontalAlignment TitleAlignment
        {
            get { return (HorizontalAlignment)GetValue(TitleAlignmentProperty); }
            set { SetValue(TitleAlignmentProperty, value); }
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
        public Brush GlowBrush
        {
            get { return (Brush)GetValue(GlowBrushProperty); }
            set { SetValue(GlowBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the brush used for the Window's non-active glow.
        /// </summary>
        public Brush NonActiveGlowBrush
        {
            get { return (Brush)GetValue(NonActiveGlowBrushProperty); }
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
            if (overlayBox == null) throw new InvalidOperationException("OverlayBox can not be founded in this MetroWindow's template. Are you calling this before the window has loaded?");

            var tcs = new System.Threading.Tasks.TaskCompletionSource<object>();

            if (IsOverlayVisible() && overlayStoryboard == null)
            {
                //No Task.FromResult in .NET 4.
                tcs.SetResult(null);
                return tcs.Task;
            }

            Dispatcher.VerifyAccess();

            overlayBox.Visibility = Visibility.Visible;

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
            if (overlayBox == null) throw new InvalidOperationException("OverlayBox can not be founded in this MetroWindow's template. Are you calling this before the window has loaded?");

            var tcs = new System.Threading.Tasks.TaskCompletionSource<object>();

            if (overlayBox.Visibility == Visibility.Visible && overlayBox.Opacity == 0.0)
            {
                //No Task.FromResult in .NET 4.
                tcs.SetResult(null);
                return tcs.Task;
            }

            Dispatcher.VerifyAccess();

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
            if (overlayBox == null) throw new InvalidOperationException("OverlayBox can not be founded in this MetroWindow's template. Are you calling this before the window has loaded?");

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
            if (EnableDWMDropShadow)
            {
                this.UseDropShadow();
            }

            if (this.WindowTransitionsEnabled)
            {
                VisualStateManager.GoToState(this, "AfterLoaded", true);
            }

            this.ToggleNoneWindowStyle(this.UseNoneWindowStyle);

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
            // this all works only for centered title

            var titleBarGrid = (Grid)titleBar;
            var titleBarLabel = (Label)titleBarGrid.Children[0];
            var titleControl = (ContentControl)titleBarLabel.Content;
            var iconContentControl = (ContentControl)icon;

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
            var element = (e.OriginalSource as DependencyObject);
            if (element != null)
            {
                // no preview if we just clicked these elements
                if (element.TryFindParent<Flyout>() != null
                    || Equals(element.TryFindParent<ContentControl>(), this.icon)
                    || element.TryFindParent<WindowCommands>() != null
                    || element.TryFindParent<WindowButtonCommands>() != null)
                {
                    return;
                }
            }

            if (Flyouts.OverrideExternalCloseButton == null)
            {
                foreach (var flyout in Flyouts.GetFlyouts().Where(x => x.IsOpen && x.ExternalCloseButton == e.ChangedButton && (!x.IsPinned || Flyouts.OverrideIsPinned)))
                {
                    flyout.IsOpen = false;
                    e.Handled = true;
                }
            }
            else if (Flyouts.OverrideExternalCloseButton == e.ChangedButton)
            {
                foreach (var flyout in Flyouts.GetFlyouts().Where(x => x.IsOpen && (!x.IsPinned || Flyouts.OverrideIsPinned)))
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

            LeftWindowCommands.ParentWindow = this;
            RightWindowCommands.ParentWindow = this;

            LeftWindowCommandsPresenter = GetTemplateChild(PART_LeftWindowCommands) as ContentPresenter;
            RightWindowCommandsPresenter = GetTemplateChild(PART_RightWindowCommands) as ContentPresenter;
            WindowButtonCommands = GetTemplateChild(PART_WindowButtonCommands) as WindowButtonCommands;

            if (WindowButtonCommands != null)
            {
                WindowButtonCommands.ParentWindow = this;
            }

            overlayBox = GetTemplateChild(PART_OverlayBox) as Grid;
            metroActiveDialogContainer = GetTemplateChild(PART_MetroActiveDialogContainer) as Grid;
            metroInactiveDialogContainer = GetTemplateChild(PART_MetroInactiveDialogsContainer) as Grid;
            flyoutModal = (Rectangle)GetTemplateChild(PART_FlyoutModal);
            flyoutModal.PreviewMouseDown += FlyoutsPreviewMouseDown;
            this.PreviewMouseDown += FlyoutsPreviewMouseDown;

            icon = GetTemplateChild(PART_Icon) as UIElement;
            titleBar = GetTemplateChild(PART_TitleBar) as UIElement;
            titleBarBackground = GetTemplateChild(PART_WindowTitleBackground) as UIElement;

            this.SetVisibiltyForAllTitleElements(this.TitlebarHeight > 0);
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

            // handle mouse events for PART_TitleBar -> MetroWindow
            if (titleBar != null && titleBar.Visibility == Visibility.Visible)
            {
                titleBar.MouseDown += TitleBarMouseDown;
                titleBar.MouseUp += TitleBarMouseUp;

                // special title resizing for centered title
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
            // if UseNoneWindowStyle = true no movement, no maximize please
            if (e.ChangedButton == MouseButton.Left && !this.UseNoneWindowStyle)
            {
                var mPoint = Mouse.GetPosition(this);

                if (IsWindowDraggable)
                {
                    IntPtr windowHandle = new WindowInteropHelper(this).Handle;
                    UnsafeNativeMethods.ReleaseCapture();
                    var wpfPoint = this.PointToScreen(mPoint);
                    var x = Convert.ToInt16(wpfPoint.X);
                    var y = Convert.ToInt16(wpfPoint.Y);
                    var lParam = (int) (uint) x | (y << 16);
                    UnsafeNativeMethods.SendMessage(windowHandle, Constants.WM_NCLBUTTONDOWN, Constants.HT_CAPTION, lParam);
                }

                var canResize = this.ResizeMode == ResizeMode.CanResizeWithGrip || this.ResizeMode == ResizeMode.CanResize;
                // we can maximize or restore the window if the title bar height is set (also if title bar is hidden)
                var isMouseOnTitlebar = mPoint.Y <= this.TitlebarHeight && this.TitlebarHeight > 0;
                if (e.ClickCount == 2 && canResize && isMouseOnTitlebar)
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

        /// <summary>
        /// Gets the template child with the given name.
        /// </summary>
        /// <typeparam name="T">The interface type inheirted from DependencyObject.</typeparam>
        /// <param name="name">The name of the template child.</param>
        internal T GetPart<T>(string name) where T : DependencyObject
        {
            return GetTemplateChild(name) as T;
        }

        /// <summary>
        /// Gets the template child with the given name.
        /// </summary>
        /// <param name="name">The name of the template child.</param>
        internal DependencyObject GetPart(string name)
        {
            return GetTemplateChild(name);
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

                // Note: ShowWindowCommandsOnTop is here for backwards compatibility reasons
                //if the the corresponding behavior has the right flag, set the window commands' and icon zIndex to a number that is higher than the flyout's.
                if (icon != null)
                {
                    icon.SetValue(Panel.ZIndexProperty,
                        this.IconOverlayBehavior.HasFlag(WindowCommandsOverlayBehavior.Flyouts) ? zIndex : 1);
                }

                if (LeftWindowCommandsPresenter != null)
                {
                    LeftWindowCommandsPresenter.SetValue(Panel.ZIndexProperty,
                        this.LeftWindowCommandsOverlayBehavior.HasFlag(WindowCommandsOverlayBehavior.Flyouts) ? zIndex : 1);
                }

                if (RightWindowCommandsPresenter != null)
                {
                    RightWindowCommandsPresenter.SetValue(Panel.ZIndexProperty,
                        this.RightWindowCommandsOverlayBehavior.HasFlag(WindowCommandsOverlayBehavior.Flyouts) ? zIndex : 1);
                }

                if (WindowButtonCommands != null)
                {
                    WindowButtonCommands.SetValue(Panel.ZIndexProperty,
                        this.WindowButtonCommandsOverlayBehavior.HasFlag(WindowCommandsOverlayBehavior.Flyouts) ? zIndex : 1);
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
