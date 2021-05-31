// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
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
    /// An extended Window class.
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
    [TemplatePart(Name = PART_Content, Type = typeof(MetroContentControl))]
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

        FrameworkElement? icon;
        UIElement? titleBar;
        UIElement? titleBarBackground;
        Thumb? windowTitleThumb;
        Thumb? flyoutModalDragMoveThumb;
        private IInputElement? restoreFocus;
        internal ContentPresenter? LeftWindowCommandsPresenter;
        internal ContentPresenter? RightWindowCommandsPresenter;
        internal ContentPresenter? WindowButtonCommandsPresenter;

        internal Grid? overlayBox;
        internal Grid? metroActiveDialogContainer;
        internal Grid? metroInactiveDialogContainer;
        private Storyboard? overlayStoryboard;
        Rectangle? flyoutModal;

        private EventHandler? onOverlayFadeInStoryboardCompleted = null;
        private EventHandler? onOverlayFadeOutStoryboardCompleted = null;

        /// <summary>Identifies the <see cref="ShowIconOnTitleBar"/> dependency property.</summary>
        public static readonly DependencyProperty ShowIconOnTitleBarProperty
            = DependencyProperty.Register(nameof(ShowIconOnTitleBar),
                                          typeof(bool),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(BooleanBoxes.TrueBox, OnShowIconOnTitleBarPropertyChangedCallback));

        private static void OnShowIconOnTitleBarPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (MetroWindow)d;
            if (e.NewValue != e.OldValue)
            {
                window.UpdateIconVisibility();
            }
        }

        /// <summary>
        /// Get or sets whether the TitleBar icon is visible or not.
        /// </summary>
        public bool ShowIconOnTitleBar
        {
            get => (bool)this.GetValue(ShowIconOnTitleBarProperty);
            set => this.SetValue(ShowIconOnTitleBarProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="IconEdgeMode"/> dependency property.</summary>
        public static readonly DependencyProperty IconEdgeModeProperty
            = DependencyProperty.Register(nameof(IconEdgeMode),
                                          typeof(EdgeMode),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(EdgeMode.Aliased));

        /// <summary>
        /// Gets or sets the edge mode for the TitleBar icon.
        /// </summary>
        public EdgeMode IconEdgeMode
        {
            get => (EdgeMode)this.GetValue(IconEdgeModeProperty);
            set => this.SetValue(IconEdgeModeProperty, value);
        }

        /// <summary>Identifies the <see cref="IconBitmapScalingMode"/> dependency property.</summary>
        public static readonly DependencyProperty IconBitmapScalingModeProperty
            = DependencyProperty.Register(nameof(IconBitmapScalingMode),
                                          typeof(BitmapScalingMode),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(BitmapScalingMode.HighQuality));

        /// <summary>
        /// Gets or sets the bitmap scaling mode for the TitleBar icon.
        /// </summary>
        public BitmapScalingMode IconBitmapScalingMode
        {
            get => (BitmapScalingMode)this.GetValue(IconBitmapScalingModeProperty);
            set => this.SetValue(IconBitmapScalingModeProperty, value);
        }

        /// <summary>Identifies the <see cref="IconScalingMode"/> dependency property.</summary>
        public static readonly DependencyProperty IconScalingModeProperty
            = DependencyProperty.Register(nameof(IconScalingMode),
                                          typeof(MultiFrameImageMode),
                                          typeof(MetroWindow),
                                          new FrameworkPropertyMetadata(MultiFrameImageMode.ScaleDownLargerFrame, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the scaling mode for the TitleBar icon.
        /// </summary>
        public MultiFrameImageMode IconScalingMode
        {
            get => (MultiFrameImageMode)this.GetValue(IconScalingModeProperty);
            set => this.SetValue(IconScalingModeProperty, value);
        }

        /// <summary>Identifies the <see cref="ShowTitleBar"/> dependency property.</summary>
        public static readonly DependencyProperty ShowTitleBarProperty
            = DependencyProperty.Register(nameof(ShowTitleBar),
                                          typeof(bool),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(BooleanBoxes.TrueBox, OnShowTitleBarPropertyChangedCallback, OnShowTitleBarCoerceValueCallback));

        [MustUseReturnValue]
        private static object? OnShowTitleBarCoerceValueCallback(DependencyObject d, object? value)
        {
            // if UseNoneWindowStyle = true no title bar should be shown
            return ((MetroWindow)d).UseNoneWindowStyle ? false : value;
        }

        /// <summary>
        /// Gets or sets whether the TitleBar is visible or not.
        /// </summary>
        public bool ShowTitleBar
        {
            get => (bool)this.GetValue(ShowTitleBarProperty);
            set => this.SetValue(ShowTitleBarProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ShowDialogsOverTitleBar"/> dependency property.</summary>
        public static readonly DependencyProperty ShowDialogsOverTitleBarProperty
            = DependencyProperty.Register(nameof(ShowDialogsOverTitleBar),
                                          typeof(bool),
                                          typeof(MetroWindow),
                                          new FrameworkPropertyMetadata(BooleanBoxes.TrueBox, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Get or sets whether a dialog will be shown over the TitleBar.
        /// </summary>
        public bool ShowDialogsOverTitleBar
        {
            get => (bool)this.GetValue(ShowDialogsOverTitleBarProperty);
            set => this.SetValue(ShowDialogsOverTitleBarProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="IsAnyDialogOpen"/> dependency property.</summary>
        internal static readonly DependencyPropertyKey IsAnyDialogOpenPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsAnyDialogOpen),
                                                  typeof(bool),
                                                  typeof(MetroWindow),
                                                  new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>Identifies the <see cref="IsAnyDialogOpen"/> dependency property.</summary>
        public static readonly DependencyProperty IsAnyDialogOpenProperty = IsAnyDialogOpenPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets whether that there are one or more dialogs open.
        /// </summary>
        public bool IsAnyDialogOpen
        {
            get => (bool)this.GetValue(IsAnyDialogOpenProperty);
            protected set => this.SetValue(IsAnyDialogOpenPropertyKey, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ShowMinButton"/> dependency property.</summary>
        public static readonly DependencyProperty ShowMinButtonProperty
            = DependencyProperty.Register(nameof(ShowMinButton),
                                          typeof(bool),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether if the minimize button is visible and the minimize system menu is enabled.
        /// </summary>
        public bool ShowMinButton
        {
            get => (bool)this.GetValue(ShowMinButtonProperty);
            set => this.SetValue(ShowMinButtonProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ShowMaxRestoreButton"/> dependency property.</summary>
        public static readonly DependencyProperty ShowMaxRestoreButtonProperty
            = DependencyProperty.Register(nameof(ShowMaxRestoreButton),
                                          typeof(bool),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether if the maximize/restore button is visible and the maximize/restore system menu is enabled.
        /// </summary>
        public bool ShowMaxRestoreButton
        {
            get => (bool)this.GetValue(ShowMaxRestoreButtonProperty);
            set => this.SetValue(ShowMaxRestoreButtonProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ShowCloseButton"/> dependency property.</summary>
        public static readonly DependencyProperty ShowCloseButtonProperty
            = DependencyProperty.Register(nameof(ShowCloseButton),
                                          typeof(bool),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether if the close button is visible.
        /// </summary>
        public bool ShowCloseButton
        {
            get => (bool)this.GetValue(ShowCloseButtonProperty);
            set => this.SetValue(ShowCloseButtonProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="IsMinButtonEnabled"/> dependency property.</summary>
        public static readonly DependencyProperty IsMinButtonEnabledProperty
            = DependencyProperty.Register(nameof(IsMinButtonEnabled),
                                          typeof(bool),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets if the minimize button is enabled.
        /// </summary>
        public bool IsMinButtonEnabled
        {
            get => (bool)this.GetValue(IsMinButtonEnabledProperty);
            set => this.SetValue(IsMinButtonEnabledProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="IsMaxRestoreButtonEnabled"/> dependency property.</summary>
        public static readonly DependencyProperty IsMaxRestoreButtonEnabledProperty
            = DependencyProperty.Register(nameof(IsMaxRestoreButtonEnabled),
                                          typeof(bool),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets if the maximize/restore button is enabled.
        /// </summary>
        public bool IsMaxRestoreButtonEnabled
        {
            get => (bool)this.GetValue(IsMaxRestoreButtonEnabledProperty);
            set => this.SetValue(IsMaxRestoreButtonEnabledProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="IsCloseButtonEnabled"/> dependency property.</summary>
        public static readonly DependencyProperty IsCloseButtonEnabledProperty
            = DependencyProperty.Register(nameof(IsCloseButtonEnabled),
                                          typeof(bool),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets if the close button is enabled.
        /// </summary>
        public bool IsCloseButtonEnabled
        {
            get => (bool)this.GetValue(IsCloseButtonEnabledProperty);
            set => this.SetValue(IsCloseButtonEnabledProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="IsCloseButtonEnabledWithDialog"/> dependency property.</summary>
        internal static readonly DependencyPropertyKey IsCloseButtonEnabledWithDialogPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsCloseButtonEnabledWithDialog),
                                                  typeof(bool),
                                                  typeof(MetroWindow),
                                                  new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>Identifies the <see cref="IsCloseButtonEnabledWithDialog"/> dependency property.</summary>
        public static readonly DependencyProperty IsCloseButtonEnabledWithDialogProperty = IsCloseButtonEnabledWithDialogPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets whether if the close button is enabled if a dialog is open.
        /// It's true if <see cref="ShowDialogsOverTitleBar"/> or the <see cref="MetroDialogSettings.OwnerCanCloseWithDialog"/> is set to true
        /// otherwise false.
        /// </summary>
        public bool IsCloseButtonEnabledWithDialog
        {
            get => (bool)this.GetValue(IsCloseButtonEnabledWithDialogProperty);
            protected set => this.SetValue(IsCloseButtonEnabledWithDialogPropertyKey, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ShowSystemMenu"/> dependency property.</summary>
        public static readonly DependencyProperty ShowSystemMenuProperty
            = DependencyProperty.Register(nameof(ShowSystemMenu),
                                          typeof(bool),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets a value that indicates whether the system menu should popup with left mouse click on the window icon.
        /// </summary>
        public bool ShowSystemMenu
        {
            get => (bool)this.GetValue(ShowSystemMenuProperty);
            set => this.SetValue(ShowSystemMenuProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ShowSystemMenuOnRightClick"/> dependency property.</summary>
        public static readonly DependencyProperty ShowSystemMenuOnRightClickProperty
            = DependencyProperty.Register(nameof(ShowSystemMenuOnRightClick),
                                          typeof(bool),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets a value that indicates whether the system menu should popup with right mouse click if the mouse position is on title bar or on the entire window if it has no TitleBar (and no TitleBar height).
        /// </summary>
        public bool ShowSystemMenuOnRightClick
        {
            get => (bool)this.GetValue(ShowSystemMenuOnRightClickProperty);
            set => this.SetValue(ShowSystemMenuOnRightClickProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="TitleBarHeight"/> dependency property.</summary>
        public static readonly DependencyProperty TitleBarHeightProperty
            = DependencyProperty.Register(nameof(TitleBarHeight),
                                          typeof(int),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(30, TitleBarHeightPropertyChangedCallback));

        private static void TitleBarHeightPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((MetroWindow)d).UpdateTitleBarElementsVisibility();
            }
        }

        /// <summary>
        /// Gets or sets the TitleBar's height.
        /// </summary>
        public int TitleBarHeight
        {
            get => (int)this.GetValue(TitleBarHeightProperty);
            set => this.SetValue(TitleBarHeightProperty, value);
        }

        /// <summary>Identifies the <see cref="TitleCharacterCasing"/> dependency property.</summary>
        public static readonly DependencyProperty TitleCharacterCasingProperty
            = DependencyProperty.Register(nameof(TitleCharacterCasing),
                                          typeof(CharacterCasing),
                                          typeof(MetroWindow),
                                          new FrameworkPropertyMetadata(CharacterCasing.Upper, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure),
                                          value => CharacterCasing.Normal <= (CharacterCasing)value && (CharacterCasing)value <= CharacterCasing.Upper);

        /// <summary>
        /// Gets or sets the Character casing of the title.
        /// </summary>
        public CharacterCasing TitleCharacterCasing
        {
            get => (CharacterCasing)this.GetValue(TitleCharacterCasingProperty);
            set => this.SetValue(TitleCharacterCasingProperty, value);
        }

        /// <summary>Identifies the <see cref="TitleAlignment"/> dependency property.</summary>
        public static readonly DependencyProperty TitleAlignmentProperty
            = DependencyProperty.Register(nameof(TitleAlignment),
                                          typeof(HorizontalAlignment),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(HorizontalAlignment.Stretch, OnTitleAlignmentChanged));

        private static void OnTitleAlignmentChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                var window = (MetroWindow)dependencyObject;

                window.SizeChanged -= window.MetroWindow_SizeChanged;
                if (e.NewValue is HorizontalAlignment horizontalAlignment && horizontalAlignment == HorizontalAlignment.Center && window.titleBar != null)
                {
                    window.SizeChanged += window.MetroWindow_SizeChanged;
                }
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the title.
        /// </summary>
        public HorizontalAlignment TitleAlignment
        {
            get => (HorizontalAlignment)this.GetValue(TitleAlignmentProperty);
            set => this.SetValue(TitleAlignmentProperty, value);
        }

        /// <summary>Identifies the <see cref="SaveWindowPosition"/> dependency property.</summary>
        public static readonly DependencyProperty SaveWindowPositionProperty
            = DependencyProperty.Register(nameof(SaveWindowPosition),
                                          typeof(bool),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets whether the window will save it's position and size.
        /// </summary>
        public bool SaveWindowPosition
        {
            get => (bool)this.GetValue(SaveWindowPositionProperty);
            set => this.SetValue(SaveWindowPositionProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="WindowPlacementSettings"/> dependency property.</summary>
        public static readonly DependencyProperty WindowPlacementSettingsProperty
            = DependencyProperty.Register(nameof(WindowPlacementSettings),
                                          typeof(IWindowPlacementSettings),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(null));

        /// <summary>
        ///  Gets or sets the settings to save and load the position and size of the window.
        /// </summary>
        public IWindowPlacementSettings? WindowPlacementSettings
        {
            get => (IWindowPlacementSettings?)this.GetValue(WindowPlacementSettingsProperty);
            set => this.SetValue(WindowPlacementSettingsProperty, value);
        }

        /// <summary>Identifies the <see cref="TitleForeground"/> dependency property.</summary>
        public static readonly DependencyProperty TitleForegroundProperty
            = DependencyProperty.Register(nameof(TitleForeground),
                                          typeof(Brush),
                                          typeof(MetroWindow));

        /// <summary>
        /// Gets or sets the brush used for the TitleBar's foreground.
        /// </summary>
        public Brush? TitleForeground
        {
            get => (Brush?)this.GetValue(TitleForegroundProperty);
            set => this.SetValue(TitleForegroundProperty, value);
        }

        /// <summary>Identifies the <see cref="TitleTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty TitleTemplateProperty
            = DependencyProperty.Register(nameof(TitleTemplate),
                                          typeof(DataTemplate),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the <see cref="Window.Title"/>.
        /// </summary>
        public DataTemplate? TitleTemplate
        {
            get => (DataTemplate?)this.GetValue(TitleTemplateProperty);
            set => this.SetValue(TitleTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="WindowTitleBrush"/> dependency property.</summary>
        public static readonly DependencyProperty WindowTitleBrushProperty
            = DependencyProperty.Register(nameof(WindowTitleBrush),
                                          typeof(Brush),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Gets or sets the brush used for the background of the TitleBar.
        /// </summary>
        public Brush WindowTitleBrush
        {
            get => (Brush)this.GetValue(WindowTitleBrushProperty);
            set => this.SetValue(WindowTitleBrushProperty, value);
        }

        /// <summary>Identifies the <see cref="NonActiveWindowTitleBrush"/> dependency property.</summary>
        public static readonly DependencyProperty NonActiveWindowTitleBrushProperty
            = DependencyProperty.Register(nameof(NonActiveWindowTitleBrush),
                                          typeof(Brush),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(Brushes.Gray));

        /// <summary>
        /// Gets or sets the non-active brush used for the background of the TitleBar.
        /// </summary>
        public Brush NonActiveWindowTitleBrush
        {
            get => (Brush)this.GetValue(NonActiveWindowTitleBrushProperty);
            set => this.SetValue(NonActiveWindowTitleBrushProperty, value);
        }

        /// <summary>Identifies the <see cref="NonActiveBorderBrush"/> dependency property.</summary>
        public static readonly DependencyProperty NonActiveBorderBrushProperty
            = DependencyProperty.Register(nameof(NonActiveBorderBrush),
                                          typeof(Brush),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(Brushes.Gray));

        /// <summary>
        /// Gets or sets the non-active brush used for the border of the window.
        /// </summary>
        public Brush NonActiveBorderBrush
        {
            get => (Brush)this.GetValue(NonActiveBorderBrushProperty);
            set => this.SetValue(NonActiveBorderBrushProperty, value);
        }

        /// <summary>Identifies the <see cref="GlowBrush"/> dependency property.</summary>
        public static readonly DependencyProperty GlowBrushProperty
            = DependencyProperty.Register(nameof(GlowBrush),
                                          typeof(Brush),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the brush used for glow border of the window.
        /// </summary>
        public Brush? GlowBrush
        {
            get => (Brush?)this.GetValue(GlowBrushProperty);
            set => this.SetValue(GlowBrushProperty, value);
        }

        /// <summary>Identifies the <see cref="NonActiveGlowBrush"/> dependency property.</summary>
        public static readonly DependencyProperty NonActiveGlowBrushProperty
            = DependencyProperty.Register(nameof(NonActiveGlowBrush),
                                          typeof(Brush),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the non-active brush used for glow border of the window.
        /// </summary>
        public Brush? NonActiveGlowBrush
        {
            get => (Brush?)this.GetValue(NonActiveGlowBrushProperty);
            set => this.SetValue(NonActiveGlowBrushProperty, value);
        }

        /// <summary>Identifies the <see cref="OverlayBrush"/> dependency property.</summary>
        public static readonly DependencyProperty OverlayBrushProperty
            = DependencyProperty.Register(nameof(OverlayBrush),
                                          typeof(Brush),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the brush used for the overlay when a dialog is open.
        /// </summary>
        public Brush? OverlayBrush
        {
            get => (Brush?)this.GetValue(OverlayBrushProperty);
            set => this.SetValue(OverlayBrushProperty, value);
        }

        /// <summary>Identifies the <see cref="OverlayOpacity"/> dependency property.</summary>
        public static readonly DependencyProperty OverlayOpacityProperty
            = DependencyProperty.Register(nameof(OverlayOpacity),
                                          typeof(double),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(0.7d));

        /// <summary>
        /// Gets or sets the opacity used for the overlay when a dialog is open.
        /// </summary>
        public double OverlayOpacity
        {
            get => (double)this.GetValue(OverlayOpacityProperty);
            set => this.SetValue(OverlayOpacityProperty, value);
        }

        /// <summary>Identifies the <see cref="FlyoutOverlayBrush"/> dependency property.</summary>
        public static readonly DependencyProperty FlyoutOverlayBrushProperty
            = DependencyProperty.Register(nameof(FlyoutOverlayBrush),
                                          typeof(Brush),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the brush used for the overlay when a modal Flyout is open.
        /// </summary>
        public Brush? FlyoutOverlayBrush
        {
            get => (Brush?)this.GetValue(FlyoutOverlayBrushProperty);
            set => this.SetValue(FlyoutOverlayBrushProperty, value);
        }

        /// <summary>Identifies the <see cref="OverlayFadeIn"/> dependency property.</summary>
        public static readonly DependencyProperty OverlayFadeInProperty
            = DependencyProperty.Register(nameof(OverlayFadeIn),
                                          typeof(Storyboard),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(default(Storyboard)));

        /// <summary>
        /// Gets or sets the storyboard for the overlay fade in effect.
        /// </summary>
        public Storyboard? OverlayFadeIn
        {
            get => (Storyboard?)this.GetValue(OverlayFadeInProperty);
            set => this.SetValue(OverlayFadeInProperty, value);
        }

        /// <summary>Identifies the <see cref="OverlayFadeOut"/> dependency property.</summary>
        public static readonly DependencyProperty OverlayFadeOutProperty
            = DependencyProperty.Register(nameof(OverlayFadeOut),
                                          typeof(Storyboard),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(default(Storyboard)));

        /// <summary>
        /// Gets or sets the storyboard for the overlay fade out effect.
        /// </summary>
        public Storyboard? OverlayFadeOut
        {
            get => (Storyboard?)this.GetValue(OverlayFadeOutProperty);
            set => this.SetValue(OverlayFadeOutProperty, value);
        }

        /// <summary>Identifies the <see cref="Flyouts"/> dependency property.</summary>
        public static readonly DependencyProperty FlyoutsProperty
            = DependencyProperty.Register(nameof(Flyouts),
                                          typeof(FlyoutsControl),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(null, UpdateLogicalChildren));

        /// <summary>
        /// Gets or sets a <see cref="FlyoutsControl"/> host for the <see cref="Flyout"/> controls.
        /// </summary>
        public FlyoutsControl? Flyouts
        {
            get => (FlyoutsControl?)this.GetValue(FlyoutsProperty);
            set => this.SetValue(FlyoutsProperty, value);
        }

        /// <summary>Identifies the <see cref="WindowTransitionsEnabled"/> dependency property.</summary>
        public static readonly DependencyProperty WindowTransitionsEnabledProperty
            = DependencyProperty.Register(nameof(WindowTransitionsEnabled),
                                          typeof(bool),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether the start animation of the window content is available.
        /// </summary>
        public bool WindowTransitionsEnabled
        {
            get => (bool)this.GetValue(WindowTransitionsEnabledProperty);
            set => this.SetValue(WindowTransitionsEnabledProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="MetroDialogOptions"/> dependency property.</summary>
        public static readonly DependencyProperty MetroDialogOptionsProperty
            = DependencyProperty.Register(nameof(MetroDialogOptions),
                                          typeof(MetroDialogSettings),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(default(MetroDialogSettings)));

        /// <summary>
        /// Gets or sets the default settings for the dialogs.
        /// </summary>
        public MetroDialogSettings? MetroDialogOptions
        {
            get => (MetroDialogSettings?)this.GetValue(MetroDialogOptionsProperty);
            set => this.SetValue(MetroDialogOptionsProperty, value);
        }

        /// <summary>Identifies the <see cref="IconTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty IconTemplateProperty
            = DependencyProperty.Register(nameof(IconTemplate),
                                          typeof(DataTemplate),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the icon on the TitleBar.
        /// </summary>
        public DataTemplate? IconTemplate
        {
            get => (DataTemplate?)this.GetValue(IconTemplateProperty);
            set => this.SetValue(IconTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="LeftWindowCommands"/> dependency property.</summary>
        public static readonly DependencyProperty LeftWindowCommandsProperty
            = DependencyProperty.Register(nameof(LeftWindowCommands),
                                          typeof(WindowCommands),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(null, OnLeftWindowCommandsPropertyChanged));

        private static void OnLeftWindowCommandsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is WindowCommands windowCommands)
            {
                AutomationProperties.SetName(windowCommands, nameof(LeftWindowCommands));
            }

            UpdateLogicalChildren(d, e);
        }

        /// <summary>
        /// Gets or sets the <see cref="WindowCommands"/> host on the left side of the TitleBar.
        /// </summary>
        public WindowCommands? LeftWindowCommands
        {
            get => (WindowCommands?)this.GetValue(LeftWindowCommandsProperty);
            set => this.SetValue(LeftWindowCommandsProperty, value);
        }

        /// <summary>Identifies the <see cref="RightWindowCommands"/> dependency property.</summary>
        public static readonly DependencyProperty RightWindowCommandsProperty
            = DependencyProperty.Register(nameof(RightWindowCommands),
                                          typeof(WindowCommands),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(null, OnRightWindowCommandsPropertyChanged));

        private static void OnRightWindowCommandsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is WindowCommands windowCommands)
            {
                AutomationProperties.SetName(windowCommands, nameof(RightWindowCommands));
            }

            UpdateLogicalChildren(d, e);
        }

        /// <summary>
        /// Gets or sets the <see cref="WindowCommands"/> host on the right side of the TitleBar.
        /// </summary>
        public WindowCommands? RightWindowCommands
        {
            get => (WindowCommands?)this.GetValue(RightWindowCommandsProperty);
            set => this.SetValue(RightWindowCommandsProperty, value);
        }

        /// <summary>Identifies the <see cref="WindowButtonCommands"/> dependency property.</summary>
        public static readonly DependencyProperty WindowButtonCommandsProperty
            = DependencyProperty.Register(nameof(WindowButtonCommands),
                                          typeof(WindowButtonCommands),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(null, UpdateLogicalChildren));

        /// <summary>
        /// Gets or sets the <see cref="WindowButtonCommands"/> host that shows the minimize/maximize/restore/close buttons.
        /// </summary>
        public WindowButtonCommands? WindowButtonCommands
        {
            get => (WindowButtonCommands?)this.GetValue(WindowButtonCommandsProperty);
            set => this.SetValue(WindowButtonCommandsProperty, value);
        }

        /// <summary>Identifies the <see cref="LeftWindowCommandsOverlayBehavior"/> dependency property.</summary>
        public static readonly DependencyProperty LeftWindowCommandsOverlayBehaviorProperty
            = DependencyProperty.Register(nameof(LeftWindowCommandsOverlayBehavior),
                                          typeof(WindowCommandsOverlayBehavior),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(WindowCommandsOverlayBehavior.Never, OnShowTitleBarPropertyChangedCallback));

        private static void OnShowTitleBarPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((MetroWindow)d).UpdateTitleBarElementsVisibility();
            }
        }

        /// <summary>
        /// Gets or sets the overlay behavior for the <see cref="WindowCommands"/> host on the left side.
        /// </summary>
        public WindowCommandsOverlayBehavior LeftWindowCommandsOverlayBehavior
        {
            get => (WindowCommandsOverlayBehavior)this.GetValue(LeftWindowCommandsOverlayBehaviorProperty);
            set => this.SetValue(LeftWindowCommandsOverlayBehaviorProperty, value);
        }

        /// <summary>Identifies the <see cref="RightWindowCommandsOverlayBehavior"/> dependency property.</summary>
        public static readonly DependencyProperty RightWindowCommandsOverlayBehaviorProperty
            = DependencyProperty.Register(nameof(RightWindowCommandsOverlayBehavior),
                                          typeof(WindowCommandsOverlayBehavior),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(WindowCommandsOverlayBehavior.Never, OnShowTitleBarPropertyChangedCallback));

        /// <summary>
        /// Gets or sets the overlay behavior for the <see cref="WindowCommands"/> host on the right side.
        /// </summary>
        public WindowCommandsOverlayBehavior RightWindowCommandsOverlayBehavior
        {
            get => (WindowCommandsOverlayBehavior)this.GetValue(RightWindowCommandsOverlayBehaviorProperty);
            set => this.SetValue(RightWindowCommandsOverlayBehaviorProperty, value);
        }

        /// <summary>Identifies the <see cref="WindowButtonCommandsOverlayBehavior"/> dependency property.</summary>
        public static readonly DependencyProperty WindowButtonCommandsOverlayBehaviorProperty
            = DependencyProperty.Register(nameof(WindowButtonCommandsOverlayBehavior),
                                          typeof(OverlayBehavior),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(OverlayBehavior.Always, OnShowTitleBarPropertyChangedCallback));

        /// <summary>
        /// Gets or sets the overlay behavior for the <see cref="WindowButtonCommands"/> host.
        /// </summary>
        public OverlayBehavior WindowButtonCommandsOverlayBehavior
        {
            get => (OverlayBehavior)this.GetValue(WindowButtonCommandsOverlayBehaviorProperty);
            set => this.SetValue(WindowButtonCommandsOverlayBehaviorProperty, value);
        }

        /// <summary>Identifies the <see cref="IconOverlayBehavior"/> dependency property.</summary>
        public static readonly DependencyProperty IconOverlayBehaviorProperty
            = DependencyProperty.Register(nameof(IconOverlayBehavior),
                                          typeof(OverlayBehavior),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(OverlayBehavior.Never, OnShowTitleBarPropertyChangedCallback));

        /// <summary>
        /// Gets or sets the overlay behavior for the <see cref="Window.Icon"/>.
        /// </summary>
        public OverlayBehavior IconOverlayBehavior
        {
            get => (OverlayBehavior)this.GetValue(IconOverlayBehaviorProperty);
            set => this.SetValue(IconOverlayBehaviorProperty, value);
        }

        /// <summary>Identifies the <see cref="UseNoneWindowStyle"/> dependency property.</summary>
        public static readonly DependencyProperty UseNoneWindowStyleProperty
            = DependencyProperty.Register(nameof(UseNoneWindowStyle),
                                          typeof(bool),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(BooleanBoxes.FalseBox, OnUseNoneWindowStylePropertyChangedCallback));

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
        /// Gets or sets whether the window will force WindowStyle to None.
        /// </summary>
        public bool UseNoneWindowStyle
        {
            get => (bool)this.GetValue(UseNoneWindowStyleProperty);
            set => this.SetValue(UseNoneWindowStyleProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="OverrideDefaultWindowCommandsBrush"/> dependency property.</summary>
        public static readonly DependencyProperty OverrideDefaultWindowCommandsBrushProperty
            = DependencyProperty.Register(nameof(OverrideDefaultWindowCommandsBrush),
                                          typeof(Brush),
                                          typeof(MetroWindow));

        /// <summary>
        /// Allows easy handling of <see cref="WindowCommands"/> brush. Theme is also applied based on this brush.
        /// </summary>
        public Brush? OverrideDefaultWindowCommandsBrush
        {
            get => (Brush?)this.GetValue(OverrideDefaultWindowCommandsBrushProperty);
            set => this.SetValue(OverrideDefaultWindowCommandsBrushProperty, value);
        }

        /// <summary>Identifies the <see cref="IsWindowDraggable"/> dependency property.</summary>
        public static readonly DependencyProperty IsWindowDraggableProperty
            = DependencyProperty.Register(nameof(IsWindowDraggable),
                                          typeof(bool),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether the whole window is draggable.
        /// </summary>
        public bool IsWindowDraggable
        {
            get => (bool)this.GetValue(IsWindowDraggableProperty);
            set => this.SetValue(IsWindowDraggableProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="IgnoreTaskbarOnMaximize"/> dependency property.</summary>
        public static readonly DependencyProperty IgnoreTaskbarOnMaximizeProperty
            = DependencyProperty.Register(nameof(IgnoreTaskbarOnMaximize),
                                          typeof(bool),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets whether if the Taskbar should be ignored when maximizing the window.
        /// </summary>
        public bool IgnoreTaskbarOnMaximize
        {
            get => (bool)this.GetValue(IgnoreTaskbarOnMaximizeProperty);
            set => this.SetValue(IgnoreTaskbarOnMaximizeProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ResizeBorderThickness"/> dependency property.</summary>
        public static readonly DependencyProperty ResizeBorderThicknessProperty
            = DependencyProperty.Register(nameof(ResizeBorderThickness),
                                          typeof(Thickness),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(new Thickness(6D)));

        /// <summary>
        /// Gets or sets resize border thickness of the window.
        /// </summary>
        public Thickness ResizeBorderThickness
        {
            get => (Thickness)this.GetValue(ResizeBorderThicknessProperty);
            set => this.SetValue(ResizeBorderThicknessProperty, value);
        }

        /// <summary>Identifies the <see cref="KeepBorderOnMaximize"/> dependency property.</summary>
        public static readonly DependencyProperty KeepBorderOnMaximizeProperty
            = DependencyProperty.Register(nameof(KeepBorderOnMaximize),
                                          typeof(bool),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether if the border thickness should be kept on maximize
        /// if the MaxHeight/MaxWidth of the window is less than the monitor resolution.
        /// </summary>
        public bool KeepBorderOnMaximize
        {
            get => (bool)this.GetValue(KeepBorderOnMaximizeProperty);
            set => this.SetValue(KeepBorderOnMaximizeProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="TryToBeFlickerFree"/> dependency property.</summary>
        public static readonly DependencyProperty TryToBeFlickerFreeProperty
            = DependencyProperty.Register(nameof(TryToBeFlickerFree),
                                          typeof(bool),
                                          typeof(MetroWindow),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets whether the resizing of the window should be tried in a way that does not cause flicker/jitter, especially when resizing from the left side.
        /// </summary>
        /// <remarks>
        /// Please note that setting this to <c>true</c> may cause resize lag and black areas appearing on some systems.
        /// </remarks>
        public bool TryToBeFlickerFree
        {
            get => (bool)this.GetValue(TryToBeFlickerFreeProperty);
            set => this.SetValue(TryToBeFlickerFreeProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="FlyoutsStatusChanged"/> routed event.</summary>
        public static readonly RoutedEvent FlyoutsStatusChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(FlyoutsStatusChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(MetroWindow));

        public event RoutedEventHandler FlyoutsStatusChanged
        {
            add => this.AddHandler(FlyoutsStatusChangedEvent, value);
            remove => this.RemoveHandler(FlyoutsStatusChangedEvent, value);
        }

        /// <summary>Identifies the <see cref="WindowTransitionCompleted"/> routed event.</summary>
        public static readonly RoutedEvent WindowTransitionCompletedEvent
            = EventManager.RegisterRoutedEvent(nameof(WindowTransitionCompleted),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(MetroWindow));

        public event RoutedEventHandler WindowTransitionCompleted
        {
            add => this.AddHandler(WindowTransitionCompletedEvent, value);
            remove => this.RemoveHandler(WindowTransitionCompletedEvent, value);
        }

        /// <summary>
        /// Gets the window placement settings (can be overwritten).
        /// </summary>
        public virtual IWindowPlacementSettings? GetWindowPlacementSettings()
        {
            return this.WindowPlacementSettings ?? new WindowApplicationSettings(this);
        }

        private void UpdateIconVisibility()
        {
            var isVisible = (this.IconOverlayBehavior.HasFlag(OverlayBehavior.HiddenTitleBar) && !this.ShowTitleBar)
                            || (this.ShowIconOnTitleBar && this.ShowTitleBar);
            this.icon?.SetCurrentValue(VisibilityProperty, isVisible ? Visibility.Visible : Visibility.Collapsed);
        }

        private void UpdateTitleBarElementsVisibility()
        {
            this.UpdateIconVisibility();

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

        private bool CanUseOverlayFadingStoryboard([NotNullWhen(true)] Storyboard? sb, [NotNullWhen(true)] out DoubleAnimation? animation)
        {
            animation = null;

            if (sb is null)
            {
                return false;
            }

            sb.Dispatcher.VerifyAccess();

            animation = sb.Children.OfType<DoubleAnimation>().FirstOrDefault();

            if (animation is null)
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
        /// Starts the overlay fade in effect.
        /// </summary>
        /// <returns>A task representing the process.</returns>
        public System.Threading.Tasks.Task ShowOverlayAsync()
        {
            if (this.overlayBox is null)
            {
                throw new InvalidOperationException("OverlayBox can not be founded in this MetroWindow's template. Are you calling this before the window has loaded?");
            }

            var tcs = new System.Threading.Tasks.TaskCompletionSource<object>();

            if (this.IsOverlayVisible() && this.overlayStoryboard is null)
            {
                //No Task.FromResult in .NET 4.
                tcs.SetResult(null!);
                return tcs.Task;
            }

            this.Dispatcher.VerifyAccess();

            var sb = this.OverlayFadeIn?.Clone();
            this.overlayStoryboard = sb;
            if (this.CanUseOverlayFadingStoryboard(sb, out var animation))
            {
                this.overlayBox.SetCurrentValue(VisibilityProperty, Visibility.Visible);

                animation.To = this.OverlayOpacity;

                this.onOverlayFadeInStoryboardCompleted = (_, _) =>
                    {
                        sb.Completed -= this.onOverlayFadeInStoryboardCompleted;
                        if (this.overlayStoryboard == sb)
                        {
                            this.overlayStoryboard = null;
                        }

                        tcs.TrySetResult(null!);
                    };

                sb.Completed += this.onOverlayFadeInStoryboardCompleted;
                this.overlayBox.BeginStoryboard(sb);
            }
            else
            {
                this.ShowOverlay();
                tcs.TrySetResult(null!);
            }

            return tcs.Task;
        }

        /// <summary>
        /// Starts the overlay fade out effect.
        /// </summary>
        /// <returns>A task representing the process.</returns>
        public System.Threading.Tasks.Task HideOverlayAsync()
        {
            if (this.overlayBox is null)
            {
                throw new InvalidOperationException("OverlayBox can not be founded in this MetroWindow's template. Are you calling this before the window has loaded?");
            }

            var tcs = new System.Threading.Tasks.TaskCompletionSource<object>();

            if (this.overlayBox.Visibility == Visibility.Visible && this.overlayBox.Opacity <= 0.0)
            {
                //No Task.FromResult in .NET 4.
                this.overlayBox.SetCurrentValue(VisibilityProperty, Visibility.Hidden);
                tcs.SetResult(null!);
                return tcs.Task;
            }

            this.Dispatcher.VerifyAccess();

            var sb = this.OverlayFadeOut?.Clone();
            this.overlayStoryboard = sb;
            if (this.CanUseOverlayFadingStoryboard(sb, out var animation))
            {
                animation.To = 0d;

                this.onOverlayFadeOutStoryboardCompleted = (_, _) =>
                    {
                        sb.Completed -= this.onOverlayFadeOutStoryboardCompleted;
                        if (this.overlayStoryboard == sb)
                        {
                            this.overlayBox.SetCurrentValue(VisibilityProperty, Visibility.Hidden);
                            this.overlayStoryboard = null;
                        }

                        tcs.TrySetResult(null!);
                    };

                sb.Completed += this.onOverlayFadeOutStoryboardCompleted;
                this.overlayBox.BeginStoryboard(sb);
            }
            else
            {
                this.HideOverlay();
                tcs.TrySetResult(null!);
            }

            return tcs.Task;
        }

        public bool IsOverlayVisible()
        {
            if (this.overlayBox is null)
            {
                throw new InvalidOperationException("OverlayBox can not be founded in this MetroWindow's template. Are you calling this before the window has loaded?");
            }

            return this.overlayBox.Visibility == Visibility.Visible && this.overlayBox.Opacity >= this.OverlayOpacity;
        }

        public void ShowOverlay()
        {
            this.overlayBox?.SetCurrentValue(VisibilityProperty, Visibility.Visible);
            this.overlayBox?.SetCurrentValue(OpacityProperty, this.OverlayOpacity);
        }

        public void HideOverlay()
        {
            this.overlayBox?.SetCurrentValue(OpacityProperty, 0d);
            this.overlayBox?.SetCurrentValue(VisibilityProperty, Visibility.Hidden);
        }

        /// <summary>
        /// Stores the given element, or the last focused element via FocusManager, for restoring the focus after closing a dialog.
        /// </summary>
        /// <param name="thisElement">The element which will be focused again.</param>
        public void StoreFocus(IInputElement? thisElement = null)
        {
            this.Dispatcher.BeginInvoke(new Action(() => { this.restoreFocus = thisElement ?? (this.restoreFocus ?? FocusManager.GetFocusedElement(this)); }));
        }

        internal void RestoreFocus()
        {
            if (this.restoreFocus != null)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Keyboard.Focus(this.restoreFocus);
                        this.restoreFocus = null;
                    }));
            }
        }

        /// <summary>
        /// Clears the stored element which would get the focus after closing a dialog.
        /// </summary>
        public void ResetStoredFocus()
        {
            this.restoreFocus = null;
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

            this.DataContextChanged += this.MetroWindow_DataContextChanged;
            this.Loaded += this.MetroWindow_Loaded;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Flyouts ??= new FlyoutsControl();

            this.ResetAllWindowCommandsBrush();

            ThemeManager.Current.ThemeChanged += this.HandleThemeManagerThemeChanged;
            this.Unloaded += (_, _) => ThemeManager.Current.ThemeChanged -= this.HandleThemeManagerThemeChanged;
        }

        private void InitializeWindowChromeBehavior()
        {
            Interaction.GetBehaviors(this).Add(new BorderlessWindowBehavior());
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
            Interaction.GetBehaviors(this).Add(new WindowsSettingBehavior());
        }

        protected override async void OnClosing(CancelEventArgs e)
        {
            // Don't overwrite cancellation for close
            if (e.Cancel == false)
            {
                // #2409: don't close window if there is a dialog still open
                var dialog = await this.GetCurrentDialogAsync<BaseMetroDialog>();
                e.Cancel = dialog != null && (this.ShowDialogsOverTitleBar || !dialog.DialogSettings.OwnerCanCloseWithDialog);
            }

            base.OnClosing(e);
        }

        private void MetroWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // MahApps add these controls to the window with AddLogicalChild method.
            // This has the side effect that the DataContext doesn't update, so do this now here.
            if (this.LeftWindowCommands != null)
            {
                this.LeftWindowCommands.DataContext = this.DataContext;
            }

            if (this.RightWindowCommands != null)
            {
                this.RightWindowCommands.DataContext = this.DataContext;
            }

            if (this.WindowButtonCommands != null)
            {
                this.WindowButtonCommands.DataContext = this.DataContext;
            }

            if (this.Flyouts != null)
            {
                this.Flyouts.DataContext = this.DataContext;
            }
        }

        private void MetroWindow_SizeChanged(object sender, RoutedEventArgs e)
        {
            // this all works only for centered title
            if (this.TitleAlignment != HorizontalAlignment.Center
                || this.titleBar is null)
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

        private void HandleThemeManagerThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            this.Invoke(() =>
                {
                    var flyouts = (this.Flyouts?.GetFlyouts().ToList() ?? new List<Flyout>());

                    // since we disabled the ThemeManager OnThemeChanged part, we must change all children flyouts too
                    // e.g if the FlyoutsControl is hosted in a UserControl
                    var allChildFlyouts = (this.Content as DependencyObject)
                                          .FindChildren<FlyoutsControl>(true)
                                          .SelectMany(flyoutsControl => flyoutsControl.GetFlyouts());
                    flyouts.AddRange(allChildFlyouts);

                    if (!flyouts.Any())
                    {
                        // we must update the window command brushes!!!
                        this.ResetAllWindowCommandsBrush();
                        return;
                    }

                    var newTheme = ReferenceEquals(e.Target, this)
                        ? e.NewTheme
                        : ThemeManager.Current.DetectTheme(this);

                    if (newTheme is null)
                    {
                        return;
                    }

                    foreach (var flyout in flyouts)
                    {
                        flyout.ChangeFlyoutTheme(newTheme);
                    }

                    this.HandleWindowCommandsForFlyouts(flyouts);
                });
        }

        private void FlyoutsPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is DependencyObject element)
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

            if (this.Flyouts!.OverrideExternalCloseButton is null)
            {
                foreach (var flyout in this.Flyouts.GetFlyouts().Where(x => x.IsOpen && x.ExternalCloseButton == e.ChangedButton && (!x.IsPinned || this.Flyouts.OverrideIsPinned)))
                {
                    flyout.SetCurrentValue(Flyout.IsOpenProperty, BooleanBoxes.FalseBox);
                }
            }
            else if (this.Flyouts.OverrideExternalCloseButton == e.ChangedButton)
            {
                foreach (var flyout in this.Flyouts.GetFlyouts().Where(x => x.IsOpen && (!x.IsPinned || this.Flyouts.OverrideIsPinned)))
                {
                    flyout.SetCurrentValue(Flyout.IsOpenProperty, BooleanBoxes.FalseBox);
                }
            }
        }

        private static void UpdateLogicalChildren(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is not MetroWindow window)
            {
                return;
            }

            if (e.OldValue is FrameworkElement oldChild)
            {
                window.RemoveLogicalChild(oldChild);
            }

            if (e.NewValue is FrameworkElement newChild)
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

            this.LeftWindowCommandsPresenter = this.GetTemplateChild(PART_LeftWindowCommands) as ContentPresenter;
            this.RightWindowCommandsPresenter = this.GetTemplateChild(PART_RightWindowCommands) as ContentPresenter;
            this.WindowButtonCommandsPresenter = this.GetTemplateChild(PART_WindowButtonCommands) as ContentPresenter;

            this.LeftWindowCommands ??= new WindowCommands();
            this.RightWindowCommands ??= new WindowCommands();
            this.WindowButtonCommands ??= new WindowButtonCommands();

            this.LeftWindowCommands.SetValue(WindowCommands.ParentWindowPropertyKey, this);
            this.RightWindowCommands.SetValue(WindowCommands.ParentWindowPropertyKey, this);
            this.WindowButtonCommands.SetValue(WindowButtonCommands.ParentWindowPropertyKey, this);

            this.overlayBox = this.GetTemplateChild(PART_OverlayBox) as Grid;
            this.metroActiveDialogContainer = this.GetTemplateChild(PART_MetroActiveDialogContainer) as Grid;
            this.metroInactiveDialogContainer = this.GetTemplateChild(PART_MetroInactiveDialogsContainer) as Grid;
            this.flyoutModal = this.GetTemplateChild(PART_FlyoutModal) as Rectangle;

            if (this.flyoutModal is not null)
            {
                this.flyoutModal.PreviewMouseDown += this.FlyoutsPreviewMouseDown;
            }

            this.PreviewMouseDown += this.FlyoutsPreviewMouseDown;

            this.icon = this.GetTemplateChild(PART_Icon) as FrameworkElement;
            this.titleBar = this.GetTemplateChild(PART_TitleBar) as UIElement;
            this.titleBarBackground = this.GetTemplateChild(PART_WindowTitleBackground) as UIElement;
            this.windowTitleThumb = this.GetTemplateChild(PART_WindowTitleThumb) as Thumb;
            this.flyoutModalDragMoveThumb = this.GetTemplateChild(PART_FlyoutModalDragMoveThumb) as Thumb;

            this.UpdateTitleBarElementsVisibility();

            if (this.GetTemplateChild(PART_Content) is MetroContentControl metroContentControl)
            {
                metroContentControl.TransitionCompleted += (_, _) => this.RaiseEvent(new RoutedEventArgs(WindowTransitionCompletedEvent));
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
            if (this.windowTitleThumb != null)
            {
                this.windowTitleThumb.PreviewMouseLeftButtonUp -= this.WindowTitleThumbOnPreviewMouseLeftButtonUp;
                this.windowTitleThumb.DragDelta -= this.WindowTitleThumbMoveOnDragDelta;
                this.windowTitleThumb.MouseDoubleClick -= this.WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
                this.windowTitleThumb.MouseRightButtonUp -= this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;
            }

            if (this.titleBar is IMetroThumb thumbContentControl)
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

            if (this.icon != null)
            {
                this.icon.MouseDown -= this.IconMouseDown;
            }

            this.SizeChanged -= this.MetroWindow_SizeChanged;
        }

        private void SetWindowEvents()
        {
            // clear all event handlers first
            this.ClearWindowEvents();

            // set mouse down/up for icon
            if (this.icon != null && this.icon.Visibility == Visibility.Visible)
            {
                this.icon.MouseDown += this.IconMouseDown;
            }

            if (this.windowTitleThumb != null)
            {
                this.windowTitleThumb.PreviewMouseLeftButtonUp += this.WindowTitleThumbOnPreviewMouseLeftButtonUp;
                this.windowTitleThumb.DragDelta += this.WindowTitleThumbMoveOnDragDelta;
                this.windowTitleThumb.MouseDoubleClick += this.WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
                this.windowTitleThumb.MouseRightButtonUp += this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;
            }

            if (this.titleBar is IMetroThumb thumbContentControl)
            {
                thumbContentControl.PreviewMouseLeftButtonUp += this.WindowTitleThumbOnPreviewMouseLeftButtonUp;
                thumbContentControl.DragDelta += this.WindowTitleThumbMoveOnDragDelta;
                thumbContentControl.MouseDoubleClick += this.WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
                thumbContentControl.MouseRightButtonUp += this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;
            }

            if (this.flyoutModalDragMoveThumb != null)
            {
                this.flyoutModalDragMoveThumb.PreviewMouseLeftButtonUp += this.WindowTitleThumbOnPreviewMouseLeftButtonUp;
                this.flyoutModalDragMoveThumb.DragDelta += this.WindowTitleThumbMoveOnDragDelta;
                this.flyoutModalDragMoveThumb.MouseDoubleClick += this.WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
                this.flyoutModalDragMoveThumb.MouseRightButtonUp += this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;
            }

            // handle size if we have a Grid for the title (e.g. clean window have a centered title)
            if (this.titleBar != null && this.TitleAlignment == HorizontalAlignment.Center)
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
                    this.Close();
                }
                else if (this.ShowSystemMenu)
                {
#pragma warning disable 618
                    ControlzEx.Windows.Shell.SystemCommands.ShowSystemMenuPhysicalCoordinates(this, this.PointToScreen(new Point(this.BorderThickness.Left, this.TitleBarHeight + this.BorderThickness.Top)));
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

        internal static void DoWindowTitleThumbMoveOnDragDelta(IMetroThumb? thumb, MetroWindow? window, DragDeltaEventArgs dragDeltaEventArgs)
        {
            if (thumb is null)
            {
                throw new ArgumentNullException(nameof(thumb));
            }

            if (window is null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            // drag only if IsWindowDraggable is set to true
            if (!window.IsWindowDraggable ||
                (!(Math.Abs(dragDeltaEventArgs.HorizontalChange) > 2) && !(Math.Abs(dragDeltaEventArgs.VerticalChange) > 2)))
            {
                return;
            }

            // This was taken from DragMove internal code
            window.VerifyAccess();

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
                EventHandler? onWindowStateChanged = null;
                onWindowStateChanged = (sender, args) =>
                    {
                        window.StateChanged -= onWindowStateChanged;

                        if (window.WindowState == WindowState.Normal)
                        {
                            Mouse.Capture(thumb, CaptureMode.Element);
                        }
                    };

                window.StateChanged -= onWindowStateChanged;
                window.StateChanged += onWindowStateChanged;
            }

#pragma warning disable 618
            // these lines are from DragMove
            // NativeMethods.SendMessage(criticalHandle, WM.SYSCOMMAND, (IntPtr)SC.MOUSEMOVE, IntPtr.Zero);
            // NativeMethods.SendMessage(criticalHandle, WM.LBUTTONUP, IntPtr.Zero, IntPtr.Zero);

            var wpfPoint = window.PointToScreen(Mouse.GetPosition(window));
            var x = (int)wpfPoint.X;
            var y = (int)wpfPoint.Y;
            NativeMethods.SendMessage(window.CriticalHandle, WM.NCLBUTTONDOWN, (IntPtr)HT.CAPTION, new IntPtr(x | (y << 16)));
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
        /// <typeparam name="T">The interface type inherited from DependencyObject.</typeparam>
        /// <param name="name">The name of the template child.</param>
        internal T? GetPart<T>(string name)
            where T : class
        {
            return this.GetTemplateChild(name) as T;
        }

        internal void HandleFlyoutStatusChange(Flyout flyout, IList<Flyout> visibleFlyouts)
        {
            // Checks a recently opened Flyout's position.
            var zIndex = flyout.IsOpen ? Panel.GetZIndex(flyout) + 3 : visibleFlyouts.Count() + 2;

            //if the the corresponding behavior has the right flag, set the window commands' and icon zIndex to a number that is higher than the Flyout's.
            this.icon?.SetValue(Panel.ZIndexProperty, flyout.IsModal && flyout.IsOpen ? 0 : (this.IconOverlayBehavior.HasFlag(OverlayBehavior.Flyouts) ? zIndex : 1));
            this.LeftWindowCommandsPresenter?.SetValue(Panel.ZIndexProperty, flyout.IsModal && flyout.IsOpen ? 0 : 1);
            this.RightWindowCommandsPresenter?.SetValue(Panel.ZIndexProperty, flyout.IsModal && flyout.IsOpen ? 0 : 1);
            this.WindowButtonCommandsPresenter?.SetValue(Panel.ZIndexProperty, flyout.IsModal && flyout.IsOpen ? 0 : (this.WindowButtonCommandsOverlayBehavior.HasFlag(OverlayBehavior.Flyouts) ? zIndex : 1));

            this.HandleWindowCommandsForFlyouts(visibleFlyouts);

            if (this.flyoutModal != null)
            {
                this.flyoutModal.Visibility = visibleFlyouts.Any(x => x.IsModal) ? Visibility.Visible : Visibility.Hidden;
            }

            this.RaiseEvent(new FlyoutStatusChangedRoutedEventArgs(FlyoutsStatusChangedEvent, this) { ChangedFlyout = flyout });
        }

        public class FlyoutStatusChangedRoutedEventArgs : RoutedEventArgs
        {
            internal FlyoutStatusChangedRoutedEventArgs(RoutedEvent rEvent, object source)
                : base(rEvent, source)
            {
            }

            public Flyout? ChangedFlyout { get; internal set; }
        }
    }
}