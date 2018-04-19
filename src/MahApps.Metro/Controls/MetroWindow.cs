using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using ControlzEx.Behaviors;
using ControlzEx.Native;
using ControlzEx.Standard;
using JetBrains.Annotations;
using MahApps.Metro.Behaviours;
using MahApps.Metro.Controls.Dialogs;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// An extended, metrofied Window class.
    /// </summary>
    [TemplatePart(Name = PART_Icon, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_TitleBar, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_WindowTitleBackground, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_WindowTitleThumb, Type = typeof(Thumb))]
    [TemplatePart(Name = PART_FlyoutModalDragMoveThumb, Type = typeof(Thumb))]
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
        private const string PART_WindowTitleThumb = "PART_WindowTitleThumb";
        private const string PART_FlyoutModalDragMoveThumb = "PART_FlyoutModalDragMoveThumb";
        private const string PART_LeftWindowCommands = "PART_LeftWindowCommands";
        private const string PART_RightWindowCommands = "PART_RightWindowCommands";
        private const string PART_WindowButtonCommands = "PART_WindowButtonCommands";
        private const string PART_OverlayBox = "PART_OverlayBox";
        private const string PART_MetroActiveDialogContainer = "PART_MetroActiveDialogContainer";
        private const string PART_MetroInactiveDialogsContainer = "PART_MetroInactiveDialogsContainer";
        private const string PART_FlyoutModal = "PART_FlyoutModal";
        private const string PART_Content = "PART_Content";

        public static readonly DependencyProperty ShowIconOnTitleBarProperty = DependencyProperty.Register("ShowIconOnTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true, OnShowIconOnTitleBarPropertyChangedCallback));
        public static readonly DependencyProperty IconEdgeModeProperty = DependencyProperty.Register("IconEdgeMode", typeof(EdgeMode), typeof(MetroWindow), new PropertyMetadata(EdgeMode.Aliased));
        public static readonly DependencyProperty IconBitmapScalingModeProperty = DependencyProperty.Register("IconBitmapScalingMode", typeof(BitmapScalingMode), typeof(MetroWindow), new PropertyMetadata(BitmapScalingMode.HighQuality));
        public static readonly DependencyProperty IconScalingModeProperty = DependencyProperty.Register("IconScalingMode", typeof(MultiFrameImageMode), typeof(MetroWindow), new FrameworkPropertyMetadata(MultiFrameImageMode.ScaleDownLargerFrame, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty ShowTitleBarProperty = DependencyProperty.Register("ShowTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true, OnShowTitleBarPropertyChangedCallback, OnShowTitleBarCoerceValueCallback));

        public static readonly DependencyProperty ShowDialogsOverTitleBarProperty = DependencyProperty.Register("ShowDialogsOverTitleBar", typeof(bool), typeof(MetroWindow), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyPropertyKey IsAnyDialogOpenPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsAnyDialogOpen), typeof(bool), typeof(MetroWindow), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IsAnyDialogOpen"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAnyDialogOpenProperty = IsAnyDialogOpenPropertyKey.DependencyProperty;

        public static readonly DependencyProperty ShowMinButtonProperty = DependencyProperty.Register("ShowMinButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowMaxRestoreButtonProperty = DependencyProperty.Register("ShowMaxRestoreButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty IsMinButtonEnabledProperty = DependencyProperty.Register("IsMinButtonEnabled", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty IsMaxRestoreButtonEnabledProperty = DependencyProperty.Register("IsMaxRestoreButtonEnabled", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty IsCloseButtonEnabledProperty = DependencyProperty.Register("IsCloseButtonEnabled", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        public static readonly DependencyPropertyKey IsCloseButtonEnabledWithDialogPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsCloseButtonEnabledWithDialog), typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsCloseButtonEnabledWithDialog"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCloseButtonEnabledWithDialogProperty = IsCloseButtonEnabledWithDialogPropertyKey.DependencyProperty;

        public static readonly DependencyProperty ShowSystemMenuOnRightClickProperty = DependencyProperty.Register("ShowSystemMenuOnRightClick", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty TitlebarHeightProperty = DependencyProperty.Register("TitlebarHeight", typeof(int), typeof(MetroWindow), new PropertyMetadata(30, TitlebarHeightPropertyChangedCallback));
        [Obsolete(@"This property will be deleted in the next release. You should use the new TitleCharacterCasing dependency property.")]
        public static readonly DependencyProperty TitleCapsProperty = DependencyProperty.Register("TitleCaps", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true, (o, e) => ((MetroWindow)o).SetCurrentValue(TitleCharacterCasingProperty, (bool)e.NewValue ? CharacterCasing.Upper : CharacterCasing.Normal)));
        public static readonly DependencyProperty TitleCharacterCasingProperty = DependencyProperty.Register("TitleCharacterCasing", typeof(CharacterCasing), typeof(MetroWindow), new FrameworkPropertyMetadata(CharacterCasing.Upper, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure), value => CharacterCasing.Normal <= (CharacterCasing)value && (CharacterCasing)value <= CharacterCasing.Upper);
        public static readonly DependencyProperty TitleAlignmentProperty = DependencyProperty.Register("TitleAlignment", typeof(HorizontalAlignment), typeof(MetroWindow), new PropertyMetadata(HorizontalAlignment.Stretch, OnTitleAlignmentChanged));

        public static readonly DependencyProperty SaveWindowPositionProperty = DependencyProperty.Register("SaveWindowPosition", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false));
        public static readonly DependencyProperty WindowPlacementSettingsProperty = DependencyProperty.Register("WindowPlacementSettings", typeof(IWindowPlacementSettings), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(MetroWindow));
        public static readonly DependencyProperty IgnoreTaskbarOnMaximizeProperty = DependencyProperty.Register("IgnoreTaskbarOnMaximize", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false));
        public static readonly DependencyProperty FlyoutsProperty = DependencyProperty.Register("Flyouts", typeof(FlyoutsControl), typeof(MetroWindow), new PropertyMetadata(null, UpdateLogicalChilds));
        public static readonly DependencyProperty WindowTransitionsEnabledProperty = DependencyProperty.Register("WindowTransitionsEnabled", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));
        public static readonly DependencyProperty MetroDialogOptionsProperty = DependencyProperty.Register("MetroDialogOptions", typeof(MetroDialogSettings), typeof(MetroWindow), new PropertyMetadata(default(MetroDialogSettings)));

        public static readonly DependencyProperty WindowTitleBrushProperty = DependencyProperty.Register("WindowTitleBrush", typeof(Brush), typeof(MetroWindow), new PropertyMetadata(Brushes.Transparent));
        public static readonly DependencyProperty NonActiveWindowTitleBrushProperty = DependencyProperty.Register("NonActiveWindowTitleBrush", typeof(Brush), typeof(MetroWindow), new PropertyMetadata(Brushes.Gray));
        public static readonly DependencyProperty NonActiveBorderBrushProperty = DependencyProperty.Register("NonActiveBorderBrush", typeof(Brush), typeof(MetroWindow), new PropertyMetadata(Brushes.Gray));

        public static readonly DependencyProperty GlowBrushProperty = DependencyProperty.Register("GlowBrush", typeof(Brush), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty NonActiveGlowBrushProperty = DependencyProperty.Register("NonActiveGlowBrush", typeof(Brush), typeof(MetroWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty OverlayBrushProperty = DependencyProperty.Register("OverlayBrush", typeof(Brush), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty OverlayOpacityProperty = DependencyProperty.Register("OverlayOpacity", typeof(double), typeof(MetroWindow), new PropertyMetadata(0.7d));

        /// <summary>
        /// Identifies the <see cref="OverlayFadeIn"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OverlayFadeInProperty = DependencyProperty.Register("OverlayFadeIn", typeof(Storyboard), typeof(MetroWindow), new PropertyMetadata(default(Storyboard)));
        /// <summary>
        /// Identifies the <see cref="OverlayFadeOut"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OverlayFadeOutProperty = DependencyProperty.Register("OverlayFadeOut", typeof(Storyboard), typeof(MetroWindow), new PropertyMetadata(default(Storyboard)));

        public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(MetroWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty LeftWindowCommandsProperty = DependencyProperty.Register("LeftWindowCommands", typeof(WindowCommands), typeof(MetroWindow), new PropertyMetadata(null, UpdateLogicalChilds));
        public static readonly DependencyProperty RightWindowCommandsProperty = DependencyProperty.Register("RightWindowCommands", typeof(WindowCommands), typeof(MetroWindow), new PropertyMetadata(null, UpdateLogicalChilds));
        public static readonly DependencyProperty WindowButtonCommandsProperty = DependencyProperty.Register("WindowButtonCommands", typeof(WindowButtonCommands), typeof(MetroWindow), new PropertyMetadata(null, UpdateLogicalChilds));

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

        FrameworkElement icon;
        UIElement titleBar;
        UIElement titleBarBackground;
        Thumb windowTitleThumb;
        Thumb flyoutModalDragMoveThumb;
        private IInputElement restoreFocus;
        internal ContentPresenter LeftWindowCommandsPresenter;
        internal ContentPresenter RightWindowCommandsPresenter;
        internal ContentPresenter WindowButtonCommandsPresenter;

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

        public static readonly RoutedEvent WindowTransitionCompletedEvent = EventManager.RegisterRoutedEvent("WindowTransitionCompleted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MetroWindow));

        public event RoutedEventHandler WindowTransitionCompleted
        {
            add { this.AddHandler(WindowTransitionCompletedEvent, value); }
            remove { this.RemoveHandler(WindowTransitionCompletedEvent, value); }
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
            this.SetCurrentValue(BorderThicknessProperty, new Thickness(0));
            this.SetCurrentValue(BorderBrushProperty, null);
            this.SetCurrentValue(GlowBrushProperty, Brushes.Black);
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

        /// <summary>
        /// Gets/sets the window button commands that hosts the min/max/close commands.
        /// </summary>
        public WindowButtonCommands WindowButtonCommands
        {
            get { return (WindowButtonCommands)GetValue(WindowButtonCommandsProperty); }
            set { SetValue(WindowButtonCommandsProperty, value); }
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
        /// Gets or sets resize border thickness
        /// </summary>
        public Thickness ResizeBorderThickness
        {
            get { return (Thickness)this.GetValue(ResizeBorderThicknessProperty); }
            set { this.SetValue(ResizeBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ResizeBorderTickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResizeBorderThicknessProperty =
            DependencyProperty.Register(nameof(ResizeBorderThickness), typeof(Thickness), typeof(MetroWindow), new PropertyMetadata(WindowChromeBehavior.GetDefaultResizeBorderThickness()));

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
        /// Get/sets whether dialogs show over the title bar.
        /// </summary>
        public bool ShowDialogsOverTitleBar
        {
            get { return (bool)GetValue(ShowDialogsOverTitleBarProperty); }
            set { SetValue(ShowDialogsOverTitleBarProperty, value); }
        }

        /// <summary>
        /// Gets whether one or more dialogs are shown.
        /// </summary>
        public bool IsAnyDialogOpen
        {
            get { return (bool)GetValue(IsAnyDialogOpenProperty); }
            private set { SetValue(IsAnyDialogOpenPropertyKey, value); }
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
        /// Gets/sets icon scaling mode of the titlebar.
        /// </summary>
        public MultiFrameImageMode IconScalingMode
        {
            get { return (MultiFrameImageMode)this.GetValue(IconScalingModeProperty); }
            set { SetValue(IconScalingModeProperty, value); }
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
                window.ToggleNoneWindowStyle(useNoneWindowStyle, window.ShowTitleBar);
            }
        }

        private void ToggleNoneWindowStyle(bool useNoneWindowStyle, bool isTitleBarVisible)
        {
            // UseNoneWindowStyle means no title bar, window commands or min, max, close buttons
            if (useNoneWindowStyle)
            {
                this.SetCurrentValue(ShowTitleBarProperty, false);
            }
            else
            {
                this.SetCurrentValue(ShowTitleBarProperty, isTitleBarVisible);
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
        /// Gets or sets whether if the close button should be enabled or not if a dialog is shown.
        /// </summary>
        public bool IsCloseButtonEnabledWithDialog
        {
            get { return (bool)GetValue(IsCloseButtonEnabledWithDialogProperty); }
            private set { SetValue(IsCloseButtonEnabledWithDialogPropertyKey, value); }
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

            this.titleBar?.SetCurrentValue(VisibilityProperty, newVisibility);
            this.titleBarBackground?.SetCurrentValue(VisibilityProperty, newVisibility);

            newVisibility = this.LeftWindowCommandsOverlayBehavior.HasFlag(WindowCommandsOverlayBehavior.HiddenTitleBar) ? Visibility.Visible : newVisibility;
            this.LeftWindowCommandsPresenter?.SetCurrentValue(VisibilityProperty, newVisibility);

            newVisibility = this.RightWindowCommandsOverlayBehavior.HasFlag(WindowCommandsOverlayBehavior.HiddenTitleBar) ? Visibility.Visible : newVisibility;
            this.RightWindowCommandsPresenter?.SetCurrentValue(VisibilityProperty, newVisibility);

            newVisibility = this.WindowButtonCommandsOverlayBehavior.HasFlag(WindowCommandsOverlayBehavior.HiddenTitleBar) ? Visibility.Visible : newVisibility;
            this.WindowButtonCommandsPresenter?.SetCurrentValue(VisibilityProperty, newVisibility);

            SetWindowEvents();
        }

        /// <summary>
        /// Gets/sets if the TitleBar's text is automatically capitalized.
        /// </summary>
        [Obsolete(@"This property will be deleted in the next release. You should use the new TitleCharacterCasing dependency property.")]
        public bool TitleCaps
        {
            get { return (bool)GetValue(TitleCapsProperty); }
            set { SetValue(TitleCapsProperty, value); }
        }

        /// <summary>
        /// Character casing of the title
        /// </summary>
        public CharacterCasing TitleCharacterCasing
        {
            get { return (CharacterCasing)GetValue(TitleCharacterCasingProperty); }
            set { SetValue(TitleCharacterCasingProperty, value); }
        }

        /// <summary>
        /// Gets/sets the title horizontal alignment.
        /// </summary>
        public HorizontalAlignment TitleAlignment
        {
            get { return (HorizontalAlignment)GetValue(TitleAlignmentProperty); }
            set { SetValue(TitleAlignmentProperty, value); }
        }

        private static void OnTitleAlignmentChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var window = dependencyObject as MetroWindow;
            if (window != null)
            {
                window.SizeChanged -= window.MetroWindow_SizeChanged;
                if (e.NewValue is HorizontalAlignment && (HorizontalAlignment)e.NewValue == HorizontalAlignment.Center)
                {
                    window.SizeChanged += window.MetroWindow_SizeChanged;
                }
            }
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
        /// Gets/sets the brush used for the dialog overlay.
        /// </summary>
        public Brush OverlayBrush
        {
            get { return (Brush)GetValue(OverlayBrushProperty); }
            set { SetValue(OverlayBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the opacity used for the dialog overlay.
        /// </summary>
        public double OverlayOpacity
        {
            get { return (double)GetValue(OverlayOpacityProperty); }
            set { SetValue(OverlayOpacityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the overlay fade in storyboard.
        /// </summary>
        public Storyboard OverlayFadeIn
        {
            get { return (Storyboard)GetValue(OverlayFadeInProperty); }
            set { SetValue(OverlayFadeInProperty, value); }
        }

        /// <summary>
        /// Gets or sets the overlay fade out storyboard.
        /// </summary>
        public Storyboard OverlayFadeOut
        {
            get { return (Storyboard)GetValue(OverlayFadeOutProperty); }
            set { SetValue(OverlayFadeOutProperty, value); }
        }

        [Obsolete("This property will be deleted in the next release.")]
        public string WindowTitle
        {
            get { return TitleCaps ? Title.ToUpper() : Title; }
        }

        private bool CanUseOverlayFadingStoryboard(Storyboard sb, out DoubleAnimation animation)
        {
            animation = null;
            if (null == sb)
            {
                return false;
            }

            sb.Dispatcher.VerifyAccess();

            animation = sb.Children.OfType<DoubleAnimation>().FirstOrDefault();
            if (null == animation)
            {
                return false;
            }

            return (sb.Duration.HasTimeSpan && sb.Duration.TimeSpan.Ticks > 0)
                   || (sb.AccelerationRatio > 0)
                   || (sb.DecelerationRatio > 0)
                   || (animation.Duration.HasTimeSpan && animation.Duration.TimeSpan.Ticks > 0)
                   || animation.AccelerationRatio > 0
                   || animation.DecelerationRatio > 0;
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

            var sb = OverlayFadeIn?.Clone();
            overlayStoryboard = sb;
            DoubleAnimation animation;
            if (CanUseOverlayFadingStoryboard(sb, out animation))
            {
                this.overlayBox.SetCurrentValue(VisibilityProperty, Visibility.Visible);

                animation.To = this.OverlayOpacity;

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
            }
            else
            {
                ShowOverlay();
                tcs.TrySetResult(null);
            }

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

            if (overlayBox.Visibility == Visibility.Visible && overlayBox.Opacity <= 0.0)
            {
                //No Task.FromResult in .NET 4.
                this.overlayBox.SetCurrentValue(VisibilityProperty, Visibility.Hidden);
                tcs.SetResult(null);
                return tcs.Task;
            }

            Dispatcher.VerifyAccess();

            var sb = OverlayFadeOut?.Clone();
            overlayStoryboard = sb;
            DoubleAnimation animation;
            if (CanUseOverlayFadingStoryboard(sb, out animation))
            {
                animation.To = 0d;

                EventHandler completionHandler = null;
                completionHandler = (sender, args) =>
                    {
                        sb.Completed -= completionHandler;
                        if (overlayStoryboard == sb)
                        {
                            this.overlayBox.SetCurrentValue(VisibilityProperty, Visibility.Hidden);
                            overlayStoryboard = null;
                        }

                        tcs.TrySetResult(null);
                    };

                sb.Completed += completionHandler;
                overlayBox.BeginStoryboard(sb);
            }
            else
            {
                HideOverlay();
                tcs.TrySetResult(null);
            }

            return tcs.Task;
        }

        public bool IsOverlayVisible()
        {
            if (overlayBox == null) throw new InvalidOperationException("OverlayBox can not be founded in this MetroWindow's template. Are you calling this before the window has loaded?");

            return overlayBox.Visibility == Visibility.Visible && overlayBox.Opacity >= this.OverlayOpacity;
        }

        public void ShowOverlay()
        {
            this.overlayBox.SetCurrentValue(VisibilityProperty, Visibility.Visible);
            overlayBox.SetCurrentValue(Grid.OpacityProperty, this.OverlayOpacity);
        }

        public void HideOverlay()
        {
            overlayBox.SetCurrentValue(Grid.OpacityProperty, 0d);
            this.overlayBox.SetCurrentValue(VisibilityProperty, Visibility.Hidden);
        }

        /// <summary>
        /// Stores the given element, or the last focused element via FocusManager, for restoring the focus after closing a dialog.
        /// </summary>
        /// <param name="thisElement">The element which will be focused again.</param>
        public void StoreFocus([CanBeNull] IInputElement thisElement = null)
        {
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    restoreFocus = thisElement ?? (this.restoreFocus ?? FocusManager.GetFocusedElement(this));
                }));
        }

        internal void RestoreFocus()
        {
            if (restoreFocus != null)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Keyboard.Focus(restoreFocus);
                        restoreFocus = null;
                    }));
            }
        }

        /// <summary>
        /// Clears the stored element which would get the focus after closing a dialog.
        /// </summary>
        public void ResetStoredFocus()
        {
            restoreFocus = null;
        }

        static MetroWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroWindow), new FrameworkPropertyMetadata(typeof(MetroWindow)));
        }

        /// <summary>
        /// Initializes a new instance of the MahApps.Metro.Controls.MetroWindow class.
        /// </summary>
        public MetroWindow()
        {
            this.MetroDialogOptions = new MetroDialogSettings();

            DataContextChanged += MetroWindow_DataContextChanged;
            Loaded += this.MetroWindow_Loaded;

            // BorderlessWindowBehavior initialization has to occur in constructor. Otherwise the load event is fired early and performance of the window is degraded.
            this.InitializeStylizedBehaviors();
        }

        /// <summary>
        /// Initializes various behaviors for the window.
        /// For example <see cref="BorderlessWindowBehavior"/>, <see cref="WindowsSettingBehaviour"/> and <see cref="GlowWindowBehavior"/>.
        /// </summary>
        private void InitializeStylizedBehaviors()
        {
            var collection = new StylizedBehaviorCollection
            {
                new BorderlessWindowBehavior(),
                new WindowsSettingBehaviour(),
                new GlowWindowBehavior()
            };

            StylizedBehaviors.SetBehaviors(this, collection);
        }

#if NET4_5
        protected override async void OnClosing(CancelEventArgs e)
        {
            // Don't overwrite cancellation for close
            if (e.Cancel == false)
            {
                // #2409: don't close window if there is a dialog still open
                var dialog = await this.GetCurrentDialogAsync<BaseMetroDialog>();
                e.Cancel = dialog != null && (this.ShowDialogsOverTitleBar || dialog.DialogSettings == null || !dialog.DialogSettings.OwnerCanCloseWithDialog);
            }

            base.OnClosing(e);
        }
#else
        protected override void OnClosing(CancelEventArgs e)
        {
            // Don't overwrite cancellation for close
            if (e.Cancel == false)
            {
                // #2409: don't close window if there is a dialog still open
                var dialog = this.Invoke(() => this.metroActiveDialogContainer?.Children.OfType<BaseMetroDialog>().LastOrDefault());
                e.Cancel = dialog != null && (this.ShowDialogsOverTitleBar || dialog.DialogSettings == null || !dialog.DialogSettings.OwnerCanCloseWithDialog);
            }

            base.OnClosing(e);
        }
#endif

        private void MetroWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // MahApps add these controls to the window with AddLogicalChild method.
            // This has the side effect that the DataContext doesn't update, so do this now here.
            if (this.LeftWindowCommands != null) this.LeftWindowCommands.DataContext = this.DataContext;
            if (this.RightWindowCommands != null) this.RightWindowCommands.DataContext = this.DataContext;
            if (this.WindowButtonCommands != null) this.WindowButtonCommands.DataContext = this.DataContext;
            if (this.Flyouts != null) this.Flyouts.DataContext = this.DataContext;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
#pragma warning disable 618
            if (EnableDWMDropShadow)
            {
                this.UseDropShadow();
            }
#pragma warning restore 618

            if (this.WindowTransitionsEnabled)
            {
                VisualStateManager.GoToState(this, "AfterLoaded", true);
            }

            this.ToggleNoneWindowStyle(this.UseNoneWindowStyle, this.ShowTitleBar);

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
            if (TitleAlignment != HorizontalAlignment.Center)
            {
                return;
            }

            // Half of this MetroWindow
            var halfDistance = this.ActualWidth / 2;
            // Distance between center and left/right
            var distanceToCenter = this.titleBar.DesiredSize.Width / 2;
            // Distance between right edge from LeftWindowCommands to left window side
            var distanceFromLeft = this.icon.ActualWidth + this.LeftWindowCommands.ActualWidth;
            // Distance between left edge from RightWindowCommands to right window side
            var distanceFromRight = this.WindowButtonCommands.ActualWidth + this.RightWindowCommands.ActualWidth;
            // Margin
            const double horizontalMargin = 5.0;

            var dLeft = distanceFromLeft + distanceToCenter + horizontalMargin;
            var dRight = distanceFromRight + distanceToCenter + horizontalMargin;
            if ((dLeft < halfDistance) && (dRight < halfDistance))
            {
                Grid.SetColumn(this.titleBar, 0);
                Grid.SetColumnSpan(this.titleBar, 7);
            }
            else
            {
                Grid.SetColumn(this.titleBar, 3);
                Grid.SetColumnSpan(this.titleBar, 1);
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
                    || Equals(element, this.overlayBox)
                    || element.TryFindParent<BaseMetroDialog>() != null
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
                }
            }
            else if (Flyouts.OverrideExternalCloseButton == e.ChangedButton)
            {
                foreach (var flyout in Flyouts.GetFlyouts().Where(x => x.IsOpen && (!x.IsPinned || Flyouts.OverrideIsPinned)))
                {
                    flyout.IsOpen = false;
                }
            }
        }


        private static void UpdateLogicalChilds(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var window = dependencyObject as MetroWindow;
            if (window == null)
            {
                return;
            }
            var oldChild = e.OldValue as FrameworkElement;
            if (oldChild != null)
            {
                window.RemoveLogicalChild(oldChild);
            }
            var newChild = e.NewValue as FrameworkElement;
            if (newChild != null)
            {
                window.AddLogicalChild(newChild);
                // Yes, that's crazy. But we must do this to enable all possible scenarios for setting DataContext
                // in a Window. Without set the DataContext at this point it can happen that e.g. a Flyout
                // doesn't get the same DataContext.
                // So now we can type
                //
                // this.InitializeComponent();
                // this.DataContext = new MainViewModel();
                //
                // or
                //
                // this.DataContext = new MainViewModel();
                // this.InitializeComponent();
                //
                newChild.DataContext = window.DataContext;
            }
        }

        protected override IEnumerator LogicalChildren
        {
            get
            {
                // cheat, make a list with all logical content and return the enumerator
                ArrayList children = new ArrayList { this.Content };
                if (this.LeftWindowCommands != null)
                {
                    children.Add(this.LeftWindowCommands);
                }
                if (this.RightWindowCommands != null)
                {
                    children.Add(this.RightWindowCommands);
                }
                if (this.WindowButtonCommands != null)
                {
                    children.Add(this.WindowButtonCommands);
                }
                if (this.Flyouts != null)
                {
                    children.Add(this.Flyouts);
                }
                return children.GetEnumerator();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            LeftWindowCommandsPresenter = GetTemplateChild(PART_LeftWindowCommands) as ContentPresenter;
            RightWindowCommandsPresenter = GetTemplateChild(PART_RightWindowCommands) as ContentPresenter;
            WindowButtonCommandsPresenter = GetTemplateChild(PART_WindowButtonCommands) as ContentPresenter;

            if (LeftWindowCommands == null)
                LeftWindowCommands = new WindowCommands();
            if (RightWindowCommands == null)
                RightWindowCommands = new WindowCommands();
            if (WindowButtonCommands == null)
                WindowButtonCommands = new WindowButtonCommands();

            LeftWindowCommands.ParentWindow = this;
            RightWindowCommands.ParentWindow = this;
            WindowButtonCommands.ParentWindow = this;

            overlayBox = GetTemplateChild(PART_OverlayBox) as Grid;
            metroActiveDialogContainer = GetTemplateChild(PART_MetroActiveDialogContainer) as Grid;
            metroInactiveDialogContainer = GetTemplateChild(PART_MetroInactiveDialogsContainer) as Grid;
            flyoutModal = (Rectangle)GetTemplateChild(PART_FlyoutModal);
            flyoutModal.PreviewMouseDown += FlyoutsPreviewMouseDown;
            this.PreviewMouseDown += FlyoutsPreviewMouseDown;

            icon = GetTemplateChild(PART_Icon) as FrameworkElement;
            titleBar = GetTemplateChild(PART_TitleBar) as UIElement;
            titleBarBackground = GetTemplateChild(PART_WindowTitleBackground) as UIElement;
            this.windowTitleThumb = GetTemplateChild(PART_WindowTitleThumb) as Thumb;
            this.flyoutModalDragMoveThumb = GetTemplateChild(PART_FlyoutModalDragMoveThumb) as Thumb;

            this.SetVisibiltyForAllTitleElements(this.TitlebarHeight > 0);

            var metroContentControl = GetTemplateChild(PART_Content) as MetroContentControl;
            if (metroContentControl != null)
            {
                metroContentControl.TransitionCompleted += (sender, args) => this.RaiseEvent(new RoutedEventArgs(WindowTransitionCompletedEvent));
            }
        }

        protected IntPtr CriticalHandle
        {
            get
            {
                var value = typeof(Window)
                    .GetProperty("CriticalHandle", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(this, new object[0]);
                return (IntPtr)value;
            }
        }

        private void ClearWindowEvents()
        {
            // clear all event handlers first:
            if (this.windowTitleThumb != null)
            {
                this.windowTitleThumb.PreviewMouseLeftButtonUp -= this.WindowTitleThumbOnPreviewMouseLeftButtonUp;
                this.windowTitleThumb.DragDelta -= this.WindowTitleThumbMoveOnDragDelta;
                this.windowTitleThumb.MouseDoubleClick -= this.WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
                this.windowTitleThumb.MouseRightButtonUp -= this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;
            }
            var thumbContentControl = this.titleBar as IMetroThumb;
            if (thumbContentControl != null)
            {
                thumbContentControl.PreviewMouseLeftButtonUp -= this.WindowTitleThumbOnPreviewMouseLeftButtonUp;
                thumbContentControl.DragDelta -= this.WindowTitleThumbMoveOnDragDelta;
                thumbContentControl.MouseDoubleClick -= this.WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
                thumbContentControl.MouseRightButtonUp -= this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;
            }
            if (this.flyoutModalDragMoveThumb != null)
            {
                this.flyoutModalDragMoveThumb.PreviewMouseLeftButtonUp -= this.WindowTitleThumbOnPreviewMouseLeftButtonUp;
                this.flyoutModalDragMoveThumb.DragDelta -= this.WindowTitleThumbMoveOnDragDelta;
                this.flyoutModalDragMoveThumb.MouseDoubleClick -= this.WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
                this.flyoutModalDragMoveThumb.MouseRightButtonUp -= this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;
            }
            if (icon != null)
            {
                icon.MouseDown -= IconMouseDown;
            }
            this.SizeChanged -= this.MetroWindow_SizeChanged;
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

            if (this.windowTitleThumb != null)
            {
                this.windowTitleThumb.PreviewMouseLeftButtonUp += WindowTitleThumbOnPreviewMouseLeftButtonUp;
                this.windowTitleThumb.DragDelta += this.WindowTitleThumbMoveOnDragDelta;
                this.windowTitleThumb.MouseDoubleClick += this.WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
                this.windowTitleThumb.MouseRightButtonUp += this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;
            }
            var thumbContentControl = this.titleBar as IMetroThumb;
            if (thumbContentControl != null)
            {
                thumbContentControl.PreviewMouseLeftButtonUp += WindowTitleThumbOnPreviewMouseLeftButtonUp;
                thumbContentControl.DragDelta += this.WindowTitleThumbMoveOnDragDelta;
                thumbContentControl.MouseDoubleClick += this.WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
                thumbContentControl.MouseRightButtonUp += this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;
            }
            if (this.flyoutModalDragMoveThumb != null)
            {
                this.flyoutModalDragMoveThumb.PreviewMouseLeftButtonUp += WindowTitleThumbOnPreviewMouseLeftButtonUp;
                this.flyoutModalDragMoveThumb.DragDelta += this.WindowTitleThumbMoveOnDragDelta;
                this.flyoutModalDragMoveThumb.MouseDoubleClick += this.WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
                this.flyoutModalDragMoveThumb.MouseRightButtonUp += this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;
            }

            // handle size if we have a Grid for the title (e.g. clean window have a centered title)
            //if (titleBar != null && titleBar.GetType() == typeof(Grid))
            if (titleBar != null && TitleAlignment == HorizontalAlignment.Center)
            {
                this.SizeChanged += this.MetroWindow_SizeChanged;
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

        private void WindowTitleThumbOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DoWindowTitleThumbOnPreviewMouseLeftButtonUp(this, e);
        }

        private void WindowTitleThumbMoveOnDragDelta(object sender, DragDeltaEventArgs dragDeltaEventArgs)
        {
            DoWindowTitleThumbMoveOnDragDelta(sender as IMetroThumb, this, dragDeltaEventArgs);
        }

        private void WindowTitleThumbChangeWindowStateOnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            DoWindowTitleThumbChangeWindowStateOnMouseDoubleClick(this, mouseButtonEventArgs);
        }

        private void WindowTitleThumbSystemMenuOnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DoWindowTitleThumbSystemMenuOnMouseRightButtonUp(this, e);
        }

        internal static void DoWindowTitleThumbOnPreviewMouseLeftButtonUp(MetroWindow window, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (mouseButtonEventArgs.Source == mouseButtonEventArgs.OriginalSource)
            {
                Mouse.Capture(null);
            }
        }

        internal static void DoWindowTitleThumbMoveOnDragDelta(IMetroThumb thumb, [NotNull] MetroWindow window, DragDeltaEventArgs dragDeltaEventArgs)
        {
            if (thumb == null)
            {
                throw new ArgumentNullException(nameof(thumb));
            }
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            // drag only if IsWindowDraggable is set to true
            if (!window.IsWindowDraggable ||
                (!(Math.Abs(dragDeltaEventArgs.HorizontalChange) > 2) && !(Math.Abs(dragDeltaEventArgs.VerticalChange) > 2)))
            {
                return;
            }

            // tage from DragMove internal code
            window.VerifyAccess();

            //var cursorPos = WinApiHelper.GetPhysicalCursorPos();

            // if the window is maximized dragging is only allowed on title bar (also if not visible)
            var windowIsMaximized = window.WindowState == WindowState.Maximized;
            var isMouseOnTitlebar = Mouse.GetPosition(thumb).Y <= window.TitlebarHeight && window.TitlebarHeight > 0;
            if (!isMouseOnTitlebar && windowIsMaximized)
            {
                return;
            }

#pragma warning disable 618
            // for the touch usage
            UnsafeNativeMethods.ReleaseCapture();
#pragma warning restore 618

            if (windowIsMaximized)
            {
                //var cursorXPos = cursorPos.x;
                EventHandler windowOnStateChanged = null;
                windowOnStateChanged = (sender, args) =>
                    {
                        //window.Top = 2;
                        //window.Left = Math.Max(cursorXPos - window.RestoreBounds.Width / 2, 0);

                        window.StateChanged -= windowOnStateChanged;
                        if (window.WindowState == WindowState.Normal)
                        {
                            Mouse.Capture(thumb, CaptureMode.Element);
                        }
                    };
                window.StateChanged += windowOnStateChanged;
            }

            var criticalHandle = window.CriticalHandle;
            // DragMove works too
            // window.DragMove();
            // instead this 2 lines
#pragma warning disable 618
            NativeMethods.SendMessage(criticalHandle, WM.SYSCOMMAND, (IntPtr)SC.MOUSEMOVE, IntPtr.Zero);
            NativeMethods.SendMessage(criticalHandle, WM.LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
#pragma warning restore 618
        }

        internal static void DoWindowTitleThumbChangeWindowStateOnMouseDoubleClick(MetroWindow window, MouseButtonEventArgs mouseButtonEventArgs)
        {
            // restore/maximize only with left button
            if (mouseButtonEventArgs.ChangedButton == MouseButton.Left)
            {
                // we can maximize or restore the window if the title bar height is set (also if title bar is hidden)
                var canResize = window.ResizeMode == ResizeMode.CanResizeWithGrip || window.ResizeMode == ResizeMode.CanResize;
                var mousePos = Mouse.GetPosition(window);
                var isMouseOnTitlebar = mousePos.Y <= window.TitlebarHeight && window.TitlebarHeight > 0;
                if (canResize && isMouseOnTitlebar)
                {
#pragma warning disable 618
                    if (window.WindowState == WindowState.Normal)
                    {
                        ControlzEx.Windows.Shell.SystemCommands.MaximizeWindow(window);
                    }
                    else
                    {
                        ControlzEx.Windows.Shell.SystemCommands.RestoreWindow(window);
                    }
#pragma warning restore 618
                    mouseButtonEventArgs.Handled = true;
                }
            }
        }

        internal static void DoWindowTitleThumbSystemMenuOnMouseRightButtonUp(MetroWindow window, MouseButtonEventArgs e)
        {
            if (window.ShowSystemMenuOnRightClick)
            {
                // show menu only if mouse pos is on title bar or if we have a window with none style and no title bar
                var mousePos = e.GetPosition(window);
                if ((mousePos.Y <= window.TitlebarHeight && window.TitlebarHeight > 0) || (window.UseNoneWindowStyle && window.TitlebarHeight <= 0))
                {
                    ShowSystemMenuPhysicalCoordinates(window, window.PointToScreen(mousePos));
                }
            }
        }

        /// <summary>
        /// Gets the template child with the given name.
        /// </summary>
        /// <typeparam name="T">The interface type inheirted from DependencyObject.</typeparam>
        /// <param name="name">The name of the template child.</param>
        internal T GetPart<T>(string name) where T : class
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

#pragma warning disable 618
        private static void ShowSystemMenuPhysicalCoordinates(Window window, Point physicalScreenLocation)
        {
            if (window == null) return;

            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero || !NativeMethods.IsWindow(hwnd))
                return;

            var hmenu = NativeMethods.GetSystemMenu(hwnd, false);

            var cmd = NativeMethods.TrackPopupMenuEx(hmenu, Constants.TPM_LEFTBUTTON | Constants.TPM_RETURNCMD,
                (int)physicalScreenLocation.X, (int)physicalScreenLocation.Y, hwnd, IntPtr.Zero);
            if (0 != cmd)
                NativeMethods.PostMessage(hwnd, WM.SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
        }
#pragma warning restore 618

        internal void HandleFlyoutStatusChange(Flyout flyout, IList<Flyout> visibleFlyouts)
        {
            //checks a recently opened flyout's position.
            //if (flyout.Position == Position.Left || flyout.Position == Position.Right || flyout.Position == Position.Top)
            {
                //get it's zindex
                var zIndex = flyout.IsOpen ? Panel.GetZIndex(flyout) + 3 : visibleFlyouts.Count() + 2;

                // Note: ShowWindowCommandsOnTop is here for backwards compatibility reasons
                //if the the corresponding behavior has the right flag, set the window commands' and icon zIndex to a number that is higher than the flyout's.
                this.icon?.SetValue(Panel.ZIndexProperty, flyout.IsModal && flyout.IsOpen ? 0 : (this.IconOverlayBehavior.HasFlag(WindowCommandsOverlayBehavior.Flyouts) ? zIndex : 1));
                this.LeftWindowCommandsPresenter?.SetValue(Panel.ZIndexProperty, flyout.IsModal && flyout.IsOpen ? 0 : (this.LeftWindowCommandsOverlayBehavior.HasFlag(WindowCommandsOverlayBehavior.Flyouts) ? zIndex : 1));
                this.RightWindowCommandsPresenter?.SetValue(Panel.ZIndexProperty, flyout.IsModal && flyout.IsOpen ? 0 : (this.RightWindowCommandsOverlayBehavior.HasFlag(WindowCommandsOverlayBehavior.Flyouts) ? zIndex : 1));
                this.WindowButtonCommandsPresenter?.SetValue(Panel.ZIndexProperty, flyout.IsModal && flyout.IsOpen ? 0 : (this.WindowButtonCommandsOverlayBehavior.HasFlag(WindowCommandsOverlayBehavior.Flyouts) ? zIndex : 1));
                this.HandleWindowCommandsForFlyouts(visibleFlyouts);
            }

            if (this.flyoutModal != null)
            {
                this.flyoutModal.Visibility = visibleFlyouts.Any(x => x.IsModal) ? Visibility.Visible : Visibility.Hidden;
            }

            RaiseEvent(new FlyoutStatusChangedRoutedEventArgs(FlyoutsStatusChangedEvent, this) { ChangedFlyout = flyout });
        }

        public class FlyoutStatusChangedRoutedEventArgs : RoutedEventArgs
        {
            internal FlyoutStatusChangedRoutedEventArgs(RoutedEvent rEvent, object source): base(rEvent, source)
            { }

            public Flyout ChangedFlyout { get; internal set; }
        }
    }
}
