// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using ControlzEx.Behaviors;
using ControlzEx.Native;
using ControlzEx.Standard;
using ControlzEx.Theming;
using JetBrains.Annotations;
using MahApps.Metro.Automation.Peers;
using MahApps.Metro.Behaviors;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.ValueBoxes;
using Microsoft.Xaml.Behaviors;

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
    [TemplatePart(Name = PART_LeftWindowCommands, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_RightWindowCommands, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_WindowButtonCommands, Type = typeof(ContentPresenter))]
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

        public static readonly DependencyProperty ShowIconOnTitleBarProperty = DependencyProperty.Register(nameof(ShowIconOnTitleBar), typeof(bool), typeof(MetroWindow), new PropertyMetadata(BooleanBoxes.TrueBox, OnShowIconOnTitleBarPropertyChangedCallback));
        public static readonly DependencyProperty IconEdgeModeProperty = DependencyProperty.Register(nameof(IconEdgeMode), typeof(EdgeMode), typeof(MetroWindow), new PropertyMetadata(EdgeMode.Aliased));
        public static readonly DependencyProperty IconBitmapScalingModeProperty = DependencyProperty.Register(nameof(IconBitmapScalingMode), typeof(BitmapScalingMode), typeof(MetroWindow), new PropertyMetadata(BitmapScalingMode.HighQuality));
        public static readonly DependencyProperty IconScalingModeProperty = DependencyProperty.Register(nameof(IconScalingMode), typeof(MultiFrameImageMode), typeof(MetroWindow), new FrameworkPropertyMetadata(MultiFrameImageMode.ScaleDownLargerFrame, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty ShowTitleBarProperty = DependencyProperty.Register(nameof(ShowTitleBar), typeof(bool), typeof(MetroWindow), new PropertyMetadata(BooleanBoxes.TrueBox, OnShowTitleBarPropertyChangedCallback, OnShowTitleBarCoerceValueCallback));

        public static readonly DependencyProperty ShowDialogsOverTitleBarProperty = DependencyProperty.Register(nameof(ShowDialogsOverTitleBar), typeof(bool), typeof(MetroWindow), new FrameworkPropertyMetadata(BooleanBoxes.TrueBox, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        internal static readonly DependencyPropertyKey IsAnyDialogOpenPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsAnyDialogOpen), typeof(bool), typeof(MetroWindow), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Identifies the <see cref="IsAnyDialogOpen"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAnyDialogOpenProperty = IsAnyDialogOpenPropertyKey.DependencyProperty;

        public static readonly DependencyProperty ShowMinButtonProperty = DependencyProperty.Register(nameof(ShowMinButton), typeof(bool), typeof(MetroWindow), new PropertyMetadata(BooleanBoxes.TrueBox));
        public static readonly DependencyProperty ShowMaxRestoreButtonProperty = DependencyProperty.Register(nameof(ShowMaxRestoreButton), typeof(bool), typeof(MetroWindow), new PropertyMetadata(BooleanBoxes.TrueBox));
        public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register(nameof(ShowCloseButton), typeof(bool), typeof(MetroWindow), new PropertyMetadata(BooleanBoxes.TrueBox));

        public static readonly DependencyProperty IsMinButtonEnabledProperty = DependencyProperty.Register(nameof(IsMinButtonEnabled), typeof(bool), typeof(MetroWindow), new PropertyMetadata(BooleanBoxes.TrueBox));
        public static readonly DependencyProperty IsMaxRestoreButtonEnabledProperty = DependencyProperty.Register(nameof(IsMaxRestoreButtonEnabled), typeof(bool), typeof(MetroWindow), new PropertyMetadata(BooleanBoxes.TrueBox));
        public static readonly DependencyProperty IsCloseButtonEnabledProperty = DependencyProperty.Register(nameof(IsCloseButtonEnabled), typeof(bool), typeof(MetroWindow), new PropertyMetadata(BooleanBoxes.TrueBox));

        internal static readonly DependencyPropertyKey IsCloseButtonEnabledWithDialogPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsCloseButtonEnabledWithDialog), typeof(bool), typeof(MetroWindow), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Identifies the <see cref="IsCloseButtonEnabledWithDialog"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCloseButtonEnabledWithDialogProperty = IsCloseButtonEnabledWithDialogPropertyKey.DependencyProperty;

        public static readonly DependencyProperty ShowSystemMenuProperty = DependencyProperty.Register(nameof(ShowSystemMenu), typeof(bool), typeof(MetroWindow), new PropertyMetadata(BooleanBoxes.TrueBox));
        public static readonly DependencyProperty ShowSystemMenuOnRightClickProperty = DependencyProperty.Register(nameof(ShowSystemMenuOnRightClick), typeof(bool), typeof(MetroWindow), new PropertyMetadata(BooleanBoxes.TrueBox));

        public static readonly DependencyProperty TitleBarHeightProperty = DependencyProperty.Register(nameof(TitleBarHeight), typeof(int), typeof(MetroWindow), new PropertyMetadata(30, TitleBarHeightPropertyChangedCallback));
        public static readonly DependencyProperty TitleCharacterCasingProperty = DependencyProperty.Register(nameof(TitleCharacterCasing), typeof(CharacterCasing), typeof(MetroWindow), new FrameworkPropertyMetadata(CharacterCasing.Upper, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure), value => CharacterCasing.Normal <= (CharacterCasing)value && (CharacterCasing)value <= CharacterCasing.Upper);
        public static readonly DependencyProperty TitleAlignmentProperty = DependencyProperty.Register(nameof(TitleAlignment), typeof(HorizontalAlignment), typeof(MetroWindow), new PropertyMetadata(HorizontalAlignment.Stretch, OnTitleAlignmentChanged));

        public static readonly DependencyProperty SaveWindowPositionProperty = DependencyProperty.Register(nameof(SaveWindowPosition), typeof(bool), typeof(MetroWindow), new PropertyMetadata(BooleanBoxes.FalseBox));
        public static readonly DependencyProperty WindowPlacementSettingsProperty = DependencyProperty.Register(nameof(WindowPlacementSettings), typeof(IWindowPlacementSettings), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register(nameof(TitleForeground), typeof(Brush), typeof(MetroWindow));
        public static readonly DependencyProperty FlyoutsProperty = DependencyProperty.Register(nameof(Flyouts), typeof(FlyoutsControl), typeof(MetroWindow), new PropertyMetadata(null, UpdateLogicalChilds));
        public static readonly DependencyProperty WindowTransitionsEnabledProperty = DependencyProperty.Register(nameof(WindowTransitionsEnabled), typeof(bool), typeof(MetroWindow), new PropertyMetadata(BooleanBoxes.TrueBox));
        public static readonly DependencyProperty MetroDialogOptionsProperty = DependencyProperty.Register(nameof(MetroDialogOptions), typeof(MetroDialogSettings), typeof(MetroWindow), new PropertyMetadata(default(MetroDialogSettings)));

        public static readonly DependencyProperty WindowTitleBrushProperty = DependencyProperty.Register(nameof(WindowTitleBrush), typeof(Brush), typeof(MetroWindow), new PropertyMetadata(Brushes.Transparent));
        public static readonly DependencyProperty NonActiveWindowTitleBrushProperty = DependencyProperty.Register(nameof(NonActiveWindowTitleBrush), typeof(Brush), typeof(MetroWindow), new PropertyMetadata(Brushes.Gray));
        public static readonly DependencyProperty NonActiveBorderBrushProperty = DependencyProperty.Register(nameof(NonActiveBorderBrush), typeof(Brush), typeof(MetroWindow), new PropertyMetadata(Brushes.Gray));

        public static readonly DependencyProperty GlowBrushProperty = DependencyProperty.Register(nameof(GlowBrush), typeof(Brush), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty NonActiveGlowBrushProperty = DependencyProperty.Register(nameof(NonActiveGlowBrush), typeof(Brush), typeof(MetroWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty OverlayBrushProperty = DependencyProperty.Register(nameof(OverlayBrush), typeof(Brush), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty OverlayOpacityProperty = DependencyProperty.Register(nameof(OverlayOpacity), typeof(double), typeof(MetroWindow), new PropertyMetadata(0.7d));

        public static readonly DependencyProperty FlyoutOverlayBrushProperty = DependencyProperty.Register(nameof(FlyoutOverlayBrush), typeof(Brush), typeof(MetroWindow), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="OverlayFadeIn"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OverlayFadeInProperty = DependencyProperty.Register(nameof(OverlayFadeIn), typeof(Storyboard), typeof(MetroWindow), new PropertyMetadata(default(Storyboard)));
        /// <summary>
        /// Identifies the <see cref="OverlayFadeOut"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OverlayFadeOutProperty = DependencyProperty.Register(nameof(OverlayFadeOut), typeof(Storyboard), typeof(MetroWindow), new PropertyMetadata(default(Storyboard)));

        public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.Register(nameof(IconTemplate), typeof(DataTemplate), typeof(MetroWindow), new PropertyMetadata(null));
        public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register(nameof(TitleTemplate), typeof(DataTemplate), typeof(MetroWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty LeftWindowCommandsProperty = DependencyProperty.Register(nameof(LeftWindowCommands), typeof(WindowCommands), typeof(MetroWindow), new PropertyMetadata(null, OnLeftWindowCommandsPropertyChanged));

        private static void OnLeftWindowCommandsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is WindowCommands windowCommands)
            {
                AutomationProperties.SetName(windowCommands, nameof(LeftWindowCommands));
            }

            UpdateLogicalChilds(d, e);
        }

        public static readonly DependencyProperty RightWindowCommandsProperty = DependencyProperty.Register(nameof(RightWindowCommands), typeof(WindowCommands), typeof(MetroWindow), new PropertyMetadata(null, OnRightWindowCommandsPropertyChanged));

        private static void OnRightWindowCommandsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is WindowCommands windowCommands)
            {
                AutomationProperties.SetName(windowCommands, nameof(RightWindowCommands));
            }

            UpdateLogicalChilds(d, e);
        }

        public static readonly DependencyProperty WindowButtonCommandsProperty = DependencyProperty.Register(nameof(WindowButtonCommands), typeof(WindowButtonCommands), typeof(MetroWindow), new PropertyMetadata(null, UpdateLogicalChilds));

        public static readonly DependencyProperty LeftWindowCommandsOverlayBehaviorProperty = DependencyProperty.Register(nameof(LeftWindowCommandsOverlayBehavior), typeof(WindowCommandsOverlayBehavior), typeof(MetroWindow), new PropertyMetadata(WindowCommandsOverlayBehavior.Never, OnShowTitleBarPropertyChangedCallback));
        public static readonly DependencyProperty RightWindowCommandsOverlayBehaviorProperty = DependencyProperty.Register(nameof(RightWindowCommandsOverlayBehavior), typeof(WindowCommandsOverlayBehavior), typeof(MetroWindow), new PropertyMetadata(WindowCommandsOverlayBehavior.Never, OnShowTitleBarPropertyChangedCallback));
        public static readonly DependencyProperty WindowButtonCommandsOverlayBehaviorProperty = DependencyProperty.Register(nameof(WindowButtonCommandsOverlayBehavior), typeof(OverlayBehavior), typeof(MetroWindow), new PropertyMetadata(OverlayBehavior.Always, OnShowTitleBarPropertyChangedCallback));
        public static readonly DependencyProperty IconOverlayBehaviorProperty = DependencyProperty.Register(nameof(IconOverlayBehavior), typeof(OverlayBehavior), typeof(MetroWindow), new PropertyMetadata(OverlayBehavior.Never, OnShowTitleBarPropertyChangedCallback));

        public static readonly DependencyProperty UseNoneWindowStyleProperty = DependencyProperty.Register(nameof(UseNoneWindowStyle), typeof(bool), typeof(MetroWindow), new PropertyMetadata(BooleanBoxes.FalseBox, OnUseNoneWindowStylePropertyChangedCallback));
        public static readonly DependencyProperty OverrideDefaultWindowCommandsBrushProperty = DependencyProperty.Register(nameof(OverrideDefaultWindowCommandsBrush), typeof(Brush), typeof(MetroWindow));

        public static readonly DependencyProperty IsWindowDraggableProperty = DependencyProperty.Register(nameof(IsWindowDraggable), typeof(bool), typeof(MetroWindow), new PropertyMetadata(BooleanBoxes.TrueBox));

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
            nameof(FlyoutsStatusChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MetroWindow));

        // Provide CLR accessors for the event
        public event RoutedEventHandler FlyoutsStatusChanged
        {
            add { AddHandler(FlyoutsStatusChangedEvent, value); }
            remove { RemoveHandler(FlyoutsStatusChangedEvent, value); }
        }

        public static readonly RoutedEvent WindowTransitionCompletedEvent = EventManager.RegisterRoutedEvent(nameof(WindowTransitionCompleted), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MetroWindow));

        public event RoutedEventHandler WindowTransitionCompleted
        {
            add { this.AddHandler(WindowTransitionCompletedEvent, value); }
            remove { this.RemoveHandler(WindowTransitionCompletedEvent, value); }
        }

        /// <summary>
        /// Allows easy handling of window commands brush. Theme is also applied based on this brush.
        /// </summary>
        public Brush OverrideDefaultWindowCommandsBrush
        {
            get { return (Brush)this.GetValue(OverrideDefaultWindowCommandsBrushProperty); }
            set { this.SetValue(OverrideDefaultWindowCommandsBrushProperty, value); }
        }

        public MetroDialogSettings MetroDialogOptions
        {
            get { return (MetroDialogSettings)GetValue(MetroDialogOptionsProperty); }
            set { SetValue(MetroDialogOptionsProperty, value); }
        }

        public bool IsWindowDraggable
        {
            get { return (bool)GetValue(IsWindowDraggableProperty); }
            set { SetValue(IsWindowDraggableProperty, BooleanBoxes.Box(value)); }
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

        public OverlayBehavior WindowButtonCommandsOverlayBehavior
        {
            get { return (OverlayBehavior)this.GetValue(WindowButtonCommandsOverlayBehaviorProperty); }
            set { SetValue(WindowButtonCommandsOverlayBehaviorProperty, value); }
        }

        public OverlayBehavior IconOverlayBehavior
        {
            get { return (OverlayBehavior)this.GetValue(IconOverlayBehaviorProperty); }
            set { SetValue(IconOverlayBehaviorProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether the window's entrance transition animation is enabled.
        /// </summary>
        public bool WindowTransitionsEnabled
        {
            get { return (bool)this.GetValue(WindowTransitionsEnabledProperty); }
            set { SetValue(WindowTransitionsEnabledProperty, BooleanBoxes.Box(value)); }
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
        /// Defines if the Taskbar should be ignored when maximizing a Window.
        /// This only works with WindowStyle = None.
        /// </summary>
        public bool IgnoreTaskbarOnMaximize
        {
            get { return (bool)this.GetValue(IgnoreTaskbarOnMaximizeProperty); }
            set { SetValue(IgnoreTaskbarOnMaximizeProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        /// Identifies the <see cref="IgnoreTaskbarOnMaximize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IgnoreTaskbarOnMaximizeProperty = DependencyProperty.Register(nameof(IgnoreTaskbarOnMaximize), typeof(bool), typeof(MetroWindow), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets resize border thickness. This enables animation, styling, binding, etc...
        /// </summary>
        public Thickness ResizeBorderThickness
        {
            get { return (Thickness)this.GetValue(ResizeBorderThicknessProperty); }
            set { this.SetValue(ResizeBorderThicknessProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="ResizeBorderThickness"/>.
        /// </summary>
        public static readonly DependencyProperty ResizeBorderThicknessProperty = DependencyProperty.Register(nameof(ResizeBorderThickness), typeof(Thickness), typeof(MetroWindow), new PropertyMetadata(new Thickness(6D)));

        /// <summary>
        /// Gets/sets if the border thickness value should be kept on maximize
        /// if the MaxHeight/MaxWidth of the window is less than the monitor resolution.
        /// </summary>
        public bool KeepBorderOnMaximize
        {
            get { return (bool)this.GetValue(KeepBorderOnMaximizeProperty); }
            set { this.SetValue(KeepBorderOnMaximizeProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="KeepBorderOnMaximize"/>.
        /// </summary>
        public static readonly DependencyProperty KeepBorderOnMaximizeProperty = DependencyProperty.Register(nameof(KeepBorderOnMaximize), typeof(bool), typeof(MetroWindow), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets wether the resizing of the window should be tried in a way that does not cause flicker/jitter, especially when resizing from the left side.
        /// </summary>
        /// <remarks>
        /// Please note that setting this to <c>true</c> may cause resize lag and black areas appearing on some systems.
        /// </remarks>
        public bool TryToBeFlickerFree
        {
            get { return (bool)this.GetValue(TryToBeFlickerFreeProperty); }
            set { this.SetValue(TryToBeFlickerFreeProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="TryToBeFlickerFree"/>.
        /// </summary>
        public static readonly DependencyProperty TryToBeFlickerFreeProperty = DependencyProperty.Register(nameof(TryToBeFlickerFree), typeof(bool), typeof(MetroWindow), new PropertyMetadata(BooleanBoxes.FalseBox));

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
            set { SetValue(SaveWindowPositionProperty, BooleanBoxes.Box(value)); }
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
            set { SetValue(ShowIconOnTitleBarProperty, BooleanBoxes.Box(value)); }
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
            set { SetValue(ShowDialogsOverTitleBarProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        /// Gets whether one or more dialogs are shown.
        /// </summary>
        public bool IsAnyDialogOpen
        {
            get { return (bool)GetValue(IsAnyDialogOpenProperty); }
            protected set { SetValue(IsAnyDialogOpenPropertyKey, BooleanBoxes.Box(value)); }
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
            set { SetValue(ShowTitleBarProperty, BooleanBoxes.Box(value)); }
        }

        private static void OnShowTitleBarPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (MetroWindow)d;
            if (e.NewValue != e.OldValue)
            {
                window.SetVisibiltyForAllTitleElements();
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
            set { SetValue(UseNoneWindowStyleProperty, BooleanBoxes.Box(value)); }
        }

        private static void OnUseNoneWindowStylePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                // if UseNoneWindowStyle = true no title bar should be shown
                var useNoneWindowStyle = (bool)e.NewValue;

                // UseNoneWindowStyle means no title bar, window commands or min, max, close buttons
                if (useNoneWindowStyle)
                {
                    ((MetroWindow)d).SetCurrentValue(ShowTitleBarProperty, BooleanBoxes.FalseBox);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether if the minimize button is visible and the minimize system menu is enabled.
        /// </summary>
        public bool ShowMinButton
        {
            get { return (bool)GetValue(ShowMinButtonProperty); }
            set { SetValue(ShowMinButtonProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        /// Gets or sets whether if the maximize/restore button is visible and the maximize/restore system menu is enabled.
        /// </summary>
        public bool ShowMaxRestoreButton
        {
            get { return (bool)GetValue(ShowMaxRestoreButtonProperty); }
            set { SetValue(ShowMaxRestoreButtonProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        /// Gets or sets whether if the close button is visible.
        /// </summary>
        public bool ShowCloseButton
        {
            get { return (bool)GetValue(ShowCloseButtonProperty); }
            set { SetValue(ShowCloseButtonProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        /// Gets/sets if the min button is enabled.
        /// </summary>
        public bool IsMinButtonEnabled
        {
            get { return (bool)GetValue(IsMinButtonEnabledProperty); }
            set { SetValue(IsMinButtonEnabledProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        /// Gets/sets if the max/restore button is enabled.
        /// </summary>
        public bool IsMaxRestoreButtonEnabled
        {
            get { return (bool)GetValue(IsMaxRestoreButtonEnabledProperty); }
            set { SetValue(IsMaxRestoreButtonEnabledProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        /// Gets/sets if the close button is enabled.
        /// </summary>
        public bool IsCloseButtonEnabled
        {
            get { return (bool)GetValue(IsCloseButtonEnabledProperty); }
            set { SetValue(IsCloseButtonEnabledProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        /// Gets or sets whether if the close button should be enabled or not if a dialog is shown.
        /// </summary>
        public bool IsCloseButtonEnabledWithDialog
        {
            get { return (bool)GetValue(IsCloseButtonEnabledWithDialogProperty); }
            protected set { SetValue(IsCloseButtonEnabledWithDialogPropertyKey, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the system menu should popup with left mouse click on the window icon.
        /// </summary>
        public bool ShowSystemMenu
        {
            get { return (bool)GetValue(ShowSystemMenuProperty); }
            set { SetValue(ShowSystemMenuProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the system menu should popup with right mouse click if the mouse position is on title bar or on the entire window if it has no title bar (and no title bar height).
        /// </summary>
        public bool ShowSystemMenuOnRightClick
        {
            get { return (bool)GetValue(ShowSystemMenuOnRightClickProperty); }
            set { SetValue(ShowSystemMenuOnRightClickProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        /// Gets/sets the TitleBar's height.
        /// </summary>
        public int TitleBarHeight
        {
            get { return (int)GetValue(TitleBarHeightProperty); }
            set { SetValue(TitleBarHeightProperty, value); }
        }

        private static void TitleBarHeightPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var window = (MetroWindow)dependencyObject;
            if (e.NewValue != e.OldValue)
            {
                window.SetVisibiltyForAllTitleElements();
            }
        }

        private void SetVisibiltyForIcon()
        {
            if (this.icon != null)
            {
                var isVisible = (this.IconOverlayBehavior.HasFlag(OverlayBehavior.HiddenTitleBar) && !this.ShowTitleBar)
                                || (this.ShowIconOnTitleBar && this.ShowTitleBar);
                var iconVisibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                this.icon.Visibility = iconVisibility;
            }
        }

        private void SetVisibiltyForAllTitleElements()
        {
            this.SetVisibiltyForIcon();
            var newVisibility = this.TitleBarHeight > 0 && this.ShowTitleBar && !this.UseNoneWindowStyle ? Visibility.Visible : Visibility.Collapsed;

            this.titleBar?.SetCurrentValue(VisibilityProperty, newVisibility);
            this.titleBarBackground?.SetCurrentValue(VisibilityProperty, newVisibility);

            var leftWindowCommandsVisibility = this.LeftWindowCommandsOverlayBehavior.HasFlag(WindowCommandsOverlayBehavior.HiddenTitleBar) && !this.UseNoneWindowStyle ? Visibility.Visible : newVisibility;
            this.LeftWindowCommandsPresenter?.SetCurrentValue(VisibilityProperty, leftWindowCommandsVisibility);

            var rightWindowCommandsVisibility = this.RightWindowCommandsOverlayBehavior.HasFlag(WindowCommandsOverlayBehavior.HiddenTitleBar) && !this.UseNoneWindowStyle ? Visibility.Visible : newVisibility;
            this.RightWindowCommandsPresenter?.SetCurrentValue(VisibilityProperty, rightWindowCommandsVisibility);

            var windowButtonCommandsVisibility = this.WindowButtonCommandsOverlayBehavior.HasFlag(OverlayBehavior.HiddenTitleBar) ? Visibility.Visible : newVisibility;
            this.WindowButtonCommandsPresenter?.SetCurrentValue(VisibilityProperty, windowButtonCommandsVisibility);

            this.SetWindowEvents();
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
                if (e.NewValue is HorizontalAlignment && (HorizontalAlignment)e.NewValue == HorizontalAlignment.Center && window.titleBar != null)
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
        /// Gets or sets the brush used for the Flyouts overlay.
        /// </summary>
        public Brush FlyoutOverlayBrush
        {
            get { return (Brush)GetValue(FlyoutOverlayBrushProperty); }
            set { SetValue(FlyoutOverlayBrushProperty, value); }
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
            Dispatcher.BeginInvoke(new Action(() => { restoreFocus = thisElement ?? (this.restoreFocus ?? FocusManager.GetFocusedElement(this)); }));
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
            this.SetCurrentValue(MetroDialogOptionsProperty, new MetroDialogSettings());

            // BorderlessWindowBehavior initialization has to occur in constructor. Otherwise the load event is fired early and performance of the window is degraded.
            this.InitializeWindowChromeBehavior();
            this.InitializeSettingsBehavior();
            this.InitializeGlowWindowBehavior();

            DataContextChanged += MetroWindow_DataContextChanged;
            Loaded += this.MetroWindow_Loaded;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Flyouts == null)
            {
                this.Flyouts = new FlyoutsControl();
            }

            this.ResetAllWindowCommandsBrush();

            ThemeManager.Current.ThemeChanged += ThemeManagerOnIsThemeChanged;
            this.Unloaded += (o, args) => ThemeManager.Current.ThemeChanged -= ThemeManagerOnIsThemeChanged;
        }

        private void InitializeWindowChromeBehavior()
        {
            var behavior = new BorderlessWindowBehavior();
            Interaction.GetBehaviors(this).Add(behavior);
        }

        private void InitializeGlowWindowBehavior()
        {
            var glowWindowBehavior = new GlowWindowBehavior();
            BindingOperations.SetBinding(glowWindowBehavior, GlowWindowBehavior.ResizeBorderThicknessProperty, new Binding { Path = new PropertyPath(ResizeBorderThicknessProperty), Source = this });
            BindingOperations.SetBinding(glowWindowBehavior, GlowWindowBehavior.GlowBrushProperty, new Binding { Path = new PropertyPath(GlowBrushProperty), Source = this });
            BindingOperations.SetBinding(glowWindowBehavior, GlowWindowBehavior.NonActiveGlowBrushProperty, new Binding { Path = new PropertyPath(NonActiveGlowBrushProperty), Source = this });
            Interaction.GetBehaviors(this).Add(glowWindowBehavior);
        }

        private void InitializeSettingsBehavior()
        {
            var behavior = new WindowsSettingBehavior();
            Interaction.GetBehaviors(this).Add(behavior);
        }

        /// <summary>
        /// Initializes various behaviors for the window.
        /// For example <see cref="BorderlessWindowBehavior"/>, <see cref="WindowsSettingBehavior"/> and <see cref="GlowWindowBehavior"/>.
        /// </summary>
        private void InitializeBehaviors()
        {
            // var borderlessWindowBehavior = new BorderlessWindowBehavior();
            //
            // var windowsSettingBehavior = new WindowsSettingBehavior();
            //
            // var glowWindowBehavior = new GlowWindowBehavior();
            // BindingOperations.SetBinding(glowWindowBehavior, GlowWindowBehavior.ResizeBorderThicknessProperty, new Binding { Path = new PropertyPath(ResizeBorderThicknessProperty), Source = this });
            // BindingOperations.SetBinding(glowWindowBehavior, GlowWindowBehavior.GlowBrushProperty, new Binding { Path = new PropertyPath(GlowBrushProperty), Source = this });
            // BindingOperations.SetBinding(glowWindowBehavior, GlowWindowBehavior.NonActiveGlowBrushProperty, new Binding { Path = new PropertyPath(NonActiveGlowBrushProperty), Source = this });
            //
            // var collection = new StylizedBehaviorCollection
            // {
            //     borderlessWindowBehavior,
            //     windowsSettingBehavior,
            //     glowWindowBehavior
            // };
            //
            // StylizedBehaviors.SetBehaviors(this, collection);
        }

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

        private void MetroWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // MahApps add these controls to the window with AddLogicalChild method.
            // This has the side effect that the DataContext doesn't update, so do this now here.
            if (this.LeftWindowCommands != null) this.LeftWindowCommands.DataContext = this.DataContext;
            if (this.RightWindowCommands != null) this.RightWindowCommands.DataContext = this.DataContext;
            if (this.WindowButtonCommands != null) this.WindowButtonCommands.DataContext = this.DataContext;
            if (this.Flyouts != null) this.Flyouts.DataContext = this.DataContext;
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
            var margin = (Thickness)this.titleBar.GetValue(MarginProperty);
            var distanceToCenter = (this.titleBar.DesiredSize.Width - margin.Left - margin.Right) / 2;

            var iconWidth = this.icon?.ActualWidth ?? 0;
            var leftWindowCommandsWidth = this.LeftWindowCommands?.ActualWidth ?? 0;
            var rightWindowCommandsWidth = this.RightWindowCommands?.ActualWidth ?? 0;
            var windowButtonCommandsWith = this.WindowButtonCommands?.ActualWidth ?? 0;

            // Distance between right edge from LeftWindowCommands to left window side
            var distanceFromLeft = iconWidth + leftWindowCommandsWidth;
            // Distance between left edge from RightWindowCommands to right window side
            var distanceFromRight = rightWindowCommandsWidth + windowButtonCommandsWith;
            // Margin
            const double horizontalMargin = 5.0;

            var dLeft = distanceFromLeft + distanceToCenter + horizontalMargin;
            var dRight = distanceFromRight + distanceToCenter + horizontalMargin;
            if ((dLeft < halfDistance) && (dRight < halfDistance))
            {
                this.titleBar.SetCurrentValue(MarginProperty, default(Thickness));
                Grid.SetColumn(this.titleBar, 0);
                Grid.SetColumnSpan(this.titleBar, 5);
            }
            else
            {
                this.titleBar.SetCurrentValue(MarginProperty, new Thickness(leftWindowCommandsWidth, 0, rightWindowCommandsWidth, 0));
                Grid.SetColumn(this.titleBar, 2);
                Grid.SetColumnSpan(this.titleBar, 1);
            }
        }

        private void ThemeManagerOnIsThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            this.Invoke(() =>
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
                        flyout.ChangeFlyoutTheme(e.NewTheme);
                    }

                    this.HandleWindowCommandsForFlyouts(flyouts);
                });
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

        /// <inheritdoc />
        protected override IEnumerator LogicalChildren
        {
            get
            {
                // cheat, make a list with all logical content and return the enumerator
                ArrayList children = new ArrayList();
                if (this.Content != null)
                {
                    children.Add(this.Content);
                }

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
            {
                LeftWindowCommands = new WindowCommands();
            }

            if (RightWindowCommands == null)
            {
                RightWindowCommands = new WindowCommands();
            }

            if (WindowButtonCommands == null)
            {
                WindowButtonCommands = new WindowButtonCommands();
            }

            LeftWindowCommands.SetValue(WindowCommands.ParentWindowPropertyKey, this);
            RightWindowCommands.SetValue(WindowCommands.ParentWindowPropertyKey, this);
            WindowButtonCommands.SetValue(WindowButtonCommands.ParentWindowPropertyKey, this);

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

            this.SetVisibiltyForAllTitleElements();

            var metroContentControl = GetTemplateChild(PART_Content) as MetroContentControl;
            if (metroContentControl != null)
            {
                metroContentControl.TransitionCompleted += (sender, args) => this.RaiseEvent(new RoutedEventArgs(WindowTransitionCompletedEvent));
            }
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new MetroWindowAutomationPeer(this);
        }

        protected internal IntPtr CriticalHandle
        {
            get
            {
                this.VerifyAccess();
                var value = typeof(Window).GetProperty("CriticalHandle", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(this, new object[0]) ?? IntPtr.Zero;
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
                else if (this.ShowSystemMenu)
                {
#pragma warning disable 618
                    ControlzEx.Windows.Shell.SystemCommands.ShowSystemMenuPhysicalCoordinates(this, PointToScreen(new Point(BorderThickness.Left, TitleBarHeight + BorderThickness.Top)));
#pragma warning restore 618
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
            var isMouseOnTitlebar = Mouse.GetPosition(thumb).Y <= window.TitleBarHeight && window.TitleBarHeight > 0;
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
                window.StateChanged -= windowOnStateChanged;
                window.StateChanged += windowOnStateChanged;
            }

            var criticalHandle = window.CriticalHandle;
#pragma warning disable 618
            // these lines are from DragMove
            // NativeMethods.SendMessage(criticalHandle, WM.SYSCOMMAND, (IntPtr)SC.MOUSEMOVE, IntPtr.Zero);
            // NativeMethods.SendMessage(criticalHandle, WM.LBUTTONUP, IntPtr.Zero, IntPtr.Zero);

            var wpfPoint = window.PointToScreen(Mouse.GetPosition(window));
            var x = (int)wpfPoint.X;
            var y = (int)wpfPoint.Y;
            NativeMethods.SendMessage(criticalHandle, WM.NCLBUTTONDOWN, (IntPtr)HT.CAPTION, new IntPtr(x | (y << 16)));
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
                var isMouseOnTitlebar = mousePos.Y <= window.TitleBarHeight && window.TitleBarHeight > 0;
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
                if ((mousePos.Y <= window.TitleBarHeight && window.TitleBarHeight > 0) || (window.UseNoneWindowStyle && window.TitleBarHeight <= 0))
                {
#pragma warning disable 618
                    ControlzEx.Windows.Shell.SystemCommands.ShowSystemMenuPhysicalCoordinates(window, window.PointToScreen(mousePos));
#pragma warning restore 618
                }
            }
        }

        /// <summary>
        /// Gets the template child with the given name.
        /// </summary>
        /// <typeparam name="T">The interface type inheirted from DependencyObject.</typeparam>
        /// <param name="name">The name of the template child.</param>
        internal T GetPart<T>(string name)
            where T : class
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

        internal void HandleFlyoutStatusChange(Flyout flyout, IList<Flyout> visibleFlyouts)
        {
            //checks a recently opened flyout's position.
            //if (flyout.Position == Position.Left || flyout.Position == Position.Right || flyout.Position == Position.Top)
            {
                //get it's zindex
                var zIndex = flyout.IsOpen ? Panel.GetZIndex(flyout) + 3 : visibleFlyouts.Count() + 2;

                //if the the corresponding behavior has the right flag, set the window commands' and icon zIndex to a number that is higher than the flyout's.
                this.icon?.SetValue(Panel.ZIndexProperty, flyout.IsModal && flyout.IsOpen ? 0 : (this.IconOverlayBehavior.HasFlag(OverlayBehavior.Flyouts) ? zIndex : 1));
                this.LeftWindowCommandsPresenter?.SetValue(Panel.ZIndexProperty, flyout.IsModal && flyout.IsOpen ? 0 : 1);
                this.RightWindowCommandsPresenter?.SetValue(Panel.ZIndexProperty, flyout.IsModal && flyout.IsOpen ? 0 : 1);
                this.WindowButtonCommandsPresenter?.SetValue(Panel.ZIndexProperty, flyout.IsModal && flyout.IsOpen ? 0 : (this.WindowButtonCommandsOverlayBehavior.HasFlag(OverlayBehavior.Flyouts) ? zIndex : 1));
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
            internal FlyoutStatusChangedRoutedEventArgs(RoutedEvent rEvent, object source)
                : base(rEvent, source)
            {
            }

            public Flyout ChangedFlyout { get; internal set; }
        }
    }
}