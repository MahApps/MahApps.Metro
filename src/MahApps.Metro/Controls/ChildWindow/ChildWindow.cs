// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using MahApps.Metro.Automation.Peers;
using MahApps.Metro.Controls.Helper;
using MahApps.Metro.Native;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A window inside a window: it sits in the dialog container of its host, over an overlay, and can be moved and closed there.
    /// </summary>
    [TemplatePart(Name = PART_Overlay, Type = typeof(Grid))]
    [TemplatePart(Name = PART_Window, Type = typeof(Grid))]
    [TemplatePart(Name = PART_TitleBar, Type = typeof(Grid))]
    [TemplatePart(Name = PART_TitleBarThumb, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_TitleBarIcon, Type = typeof(ContentControl))]
    [TemplatePart(Name = PART_TitleBarCloseButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_Border, Type = typeof(Border))]
    [TemplatePart(Name = PART_Content, Type = typeof(ContentPresenter))]
    [StyleTypedProperty(Property = nameof(TitleBarCloseButtonStyle), StyleTargetType = typeof(Button))]
    public class ChildWindow : ContentControl
    {
        private const string PART_Overlay = "PART_Overlay";
        private const string HideStoryboard = "HideStoryboard";
        private const string PART_Window = "PART_Window";
        private const string PART_TitleBar = "PART_TitleBar";
        private const string PART_TitleBarThumb = "PART_TitleBarThumb";
        private const string PART_TitleBarIcon = "PART_TitleBarIcon";
        private const string PART_TitleBarCloseButton = "PART_TitleBarCloseButton";
        private const string PART_Border = "PART_Border";
        private const string PART_Content = "PART_Content";

        private DispatcherTimer? autoCloseTimer;

        /// <summary>Identifies the <see cref="AllowMove"/> dependency property.</summary>
        public static readonly DependencyProperty AllowMoveProperty
            = DependencyProperty.Register(nameof(AllowMove),
                                          typeof(bool),
                                          typeof(ChildWindow),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets a value indicating whether the child window can be moved inside the overlay container.
        /// </summary>
        public bool AllowMove
        {
            get => (bool)this.GetValue(AllowMoveProperty);
            set => this.SetValue(AllowMoveProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="OffsetX"/> dependency property.</summary>
        public static readonly DependencyProperty OffsetXProperty
            = DependencyProperty.Register(nameof(OffsetX),
                                          typeof(double),
                                          typeof(ChildWindow),
                                          new UIPropertyMetadata(0d, OnOffsetXChanged));

        /// <summary>
        /// Gets or Sets the current X-Position of the ChildWindow.
        /// </summary>
        public double OffsetX
        {
            get => (double)this.GetValue(OffsetXProperty);
            set => this.SetValue(OffsetXProperty, value);
        }

        private static void OnOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChildWindow childWindow)
            {
                childWindow.moveTransform.SetCurrentValue(TranslateTransform.XProperty, e.NewValue);
            }
        }

        /// <summary>Identifies the <see cref="OffsetY"/> dependency property.</summary>
        public static readonly DependencyProperty OffsetYProperty
            = DependencyProperty.Register(nameof(OffsetY),
                                          typeof(double),
                                          typeof(ChildWindow),
                                          new UIPropertyMetadata(0d, OnOffsetYChanged));

        /// <summary>
        /// Gets or Sets the current Y-Position of the ChildWindow.
        /// </summary>
        public double OffsetY
        {
            get => (double)this.GetValue(OffsetYProperty);
            set => this.SetValue(OffsetYProperty, value);
        }

        private static void OnOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChildWindow childWindow)
            {
                childWindow.moveTransform.SetCurrentValue(TranslateTransform.YProperty, e.NewValue);
            }
        }

        /// <summary>Identifies the <see cref="IsModal"/> dependency property.</summary>
        public static readonly DependencyProperty IsModalProperty
            = DependencyProperty.Register(nameof(IsModal),
                                          typeof(bool),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(BooleanBoxes.TrueBox, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets a value indicating whether the child window is modal.
        /// </summary>
        public bool IsModal
        {
            get => (bool)this.GetValue(IsModalProperty);
            set => this.SetValue(IsModalProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="OverlayBrush"/> dependency property.</summary>
        public static readonly DependencyProperty OverlayBrushProperty
            = DependencyProperty.Register(nameof(OverlayBrush),
                                          typeof(Brush),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the overlay brush.
        /// </summary>
        public Brush OverlayBrush
        {
            get => (Brush)this.GetValue(OverlayBrushProperty);
            set => this.SetValue(OverlayBrushProperty, value);
        }

        /// <summary>Identifies the <see cref="CloseOnOverlay"/> dependency property.</summary>
        public static readonly DependencyProperty CloseOnOverlayProperty
            = DependencyProperty.Register(nameof(CloseOnOverlay),
                                          typeof(bool),
                                          typeof(ChildWindow),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets a value indicating whether the child window can be closed by clicking the overlay container.
        /// </summary>
        public bool CloseOnOverlay
        {
            get => (bool)this.GetValue(CloseOnOverlayProperty);
            set => this.SetValue(CloseOnOverlayProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="CloseByEscape"/> dependency property.</summary>
        public static readonly DependencyProperty CloseByEscapeProperty
            = DependencyProperty.Register(nameof(CloseByEscape),
                                          typeof(bool),
                                          typeof(ChildWindow),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets a value indicating whether the child window can be closed by the Escape key.
        /// </summary>
        public bool CloseByEscape
        {
            get => (bool)this.GetValue(CloseByEscapeProperty);
            set => this.SetValue(CloseByEscapeProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ShowTitleBar"/> dependency property.</summary>
        public static readonly DependencyProperty ShowTitleBarProperty
            = DependencyProperty.Register(nameof(ShowTitleBar),
                                          typeof(bool),
                                          typeof(ChildWindow),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether the title bar is visible or not.
        /// </summary>
        public bool ShowTitleBar
        {
            get => (bool)this.GetValue(ShowTitleBarProperty);
            set => this.SetValue(ShowTitleBarProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="TitleBarHeight"/> dependency property.</summary>
        public static readonly DependencyProperty TitleBarHeightProperty
            = DependencyProperty.Register(nameof(TitleBarHeight),
                                          typeof(double),
                                          typeof(ChildWindow),
                                          new PropertyMetadata(30d));

        /// <summary>
        /// Gets or sets the height of the title bar.
        /// </summary>
        public double TitleBarHeight
        {
            get => (double)this.GetValue(TitleBarHeightProperty);
            set => this.SetValue(TitleBarHeightProperty, value);
        }

        /// <summary>Identifies the <see cref="TitleBarBackground"/> dependency property.</summary>
        public static readonly DependencyProperty TitleBarBackgroundProperty
            = DependencyProperty.Register(nameof(TitleBarBackground),
                                          typeof(Brush),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the title bar background.
        /// </summary>
        public Brush TitleBarBackground
        {
            get => (Brush)this.GetValue(TitleBarBackgroundProperty);
            set => this.SetValue(TitleBarBackgroundProperty, value);
        }

        /// <summary>Identifies the <see cref="TitleBarNonActiveBackground"/> dependency property.</summary>
        public static readonly DependencyProperty TitleBarNonActiveBackgroundProperty
            = DependencyProperty.Register(nameof(TitleBarNonActiveBackground),
                                          typeof(Brush),
                                          typeof(ChildWindow),
                                          new PropertyMetadata(Brushes.Gray));

        /// <summary>
        /// Gets or sets the title bar background for non-active status.
        /// </summary>
        public Brush TitleBarNonActiveBackground
        {
            get => (Brush)this.GetValue(TitleBarNonActiveBackgroundProperty);
            set => this.SetValue(TitleBarNonActiveBackgroundProperty, value);
        }

        /// <summary>Identifies the <see cref="NonActiveBorderBrush"/> dependency property.</summary>
        public static readonly DependencyProperty NonActiveBorderBrushProperty
            = DependencyProperty.Register(nameof(NonActiveBorderBrush),
                                          typeof(Brush),
                                          typeof(ChildWindow),
                                          new PropertyMetadata(Brushes.Gray));

        /// <summary>
        /// Gets or sets the border brush for non-active status.
        /// </summary>
        public Brush NonActiveBorderBrush
        {
            get => (Brush)this.GetValue(NonActiveBorderBrushProperty);
            set => this.SetValue(NonActiveBorderBrushProperty, value);
        }

        /// <summary>Identifies the <see cref="TitleBarForeground"/> dependency property.</summary>
        public static readonly DependencyProperty TitleBarForegroundProperty
            = DependencyProperty.Register(nameof(TitleBarForeground),
                                          typeof(Brush),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the title foreground.
        /// </summary>
        public Brush TitleBarForeground
        {
            get => (Brush)this.GetValue(TitleBarForegroundProperty);
            set => this.SetValue(TitleBarForegroundProperty, value);
        }

        /// <summary>Identifies the <see cref="Title"/> dependency property.</summary>
        public static readonly DependencyProperty TitleProperty
            = DependencyProperty.Register(nameof(Title),
                                          typeof(string),
                                          typeof(ChildWindow),
                                          new PropertyMetadata(default(string)));

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string? Title
        {
            get => (string?)this.GetValue(TitleProperty);
            set => this.SetValue(TitleProperty, value);
        }

        /// <summary>Identifies the <see cref="TitleCharacterCasing"/> dependency property.</summary>
        public static readonly DependencyProperty TitleCharacterCasingProperty
            = DependencyProperty.Register(nameof(TitleCharacterCasing),
                                          typeof(CharacterCasing),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(CharacterCasing.Normal, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure),
                                          value => CharacterCasing.Normal <= (CharacterCasing)value && (CharacterCasing)value <= CharacterCasing.Upper);

        /// <summary>
        /// Gets or sets the character casing of the title.
        /// </summary>
        public CharacterCasing TitleCharacterCasing
        {
            get => (CharacterCasing)this.GetValue(TitleCharacterCasingProperty);
            set => this.SetValue(TitleCharacterCasingProperty, value);
        }

        /// <summary>Identifies the <see cref="TitleHorizontalAlignment"/> dependency property.</summary>
        public static readonly DependencyProperty TitleHorizontalAlignmentProperty
            = DependencyProperty.Register(nameof(TitleHorizontalAlignment),
                                          typeof(HorizontalAlignment),
                                          typeof(ChildWindow),
                                          new PropertyMetadata(HorizontalAlignment.Stretch));

        /// <summary>
        /// Gets or sets the title horizontal alignment.
        /// </summary>
        public HorizontalAlignment TitleHorizontalAlignment
        {
            get => (HorizontalAlignment)this.GetValue(TitleHorizontalAlignmentProperty);
            set => this.SetValue(TitleHorizontalAlignmentProperty, value);
        }

        /// <summary>Identifies the <see cref="TitleVerticalAlignment"/> dependency property.</summary>
        public static readonly DependencyProperty TitleVerticalAlignmentProperty
            = DependencyProperty.Register(nameof(TitleVerticalAlignment),
                                          typeof(VerticalAlignment),
                                          typeof(ChildWindow),
                                          new PropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// Gets or sets the title vertical alignment.
        /// </summary>
        public VerticalAlignment TitleVerticalAlignment
        {
            get => (VerticalAlignment)this.GetValue(TitleVerticalAlignmentProperty);
            set => this.SetValue(TitleVerticalAlignmentProperty, value);
        }

        /// <summary>Identifies the <see cref="TitleTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty TitleTemplateProperty
            = DependencyProperty.Register(nameof(TitleTemplate),
                                          typeof(DataTemplate),
                                          typeof(ChildWindow),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the title content template to show a custom title.
        /// </summary>
        public DataTemplate? TitleTemplate
        {
            get => (DataTemplate?)this.GetValue(TitleTemplateProperty);
            set => this.SetValue(TitleTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="TitleFontSize"/> dependency property.</summary>
        public static readonly DependencyProperty TitleFontSizeProperty
            = DependencyProperty.Register(nameof(TitleFontSize),
                                          typeof(double),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(SystemFonts.CaptionFontSize, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// The FontSize property specifies the size of the title.
        /// </summary>
        [TypeConverter(typeof(FontSizeConverter))]
        public double TitleFontSize
        {
            get => (double)this.GetValue(TitleFontSizeProperty);
            set => this.SetValue(TitleFontSizeProperty, value);
        }

        /// <summary>Identifies the <see cref="TitleFontFamily"/> dependency property.</summary>
        public static readonly DependencyProperty TitleFontFamilyProperty
            = DependencyProperty.Register(nameof(TitleFontFamily),
                                          typeof(FontFamily),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(SystemFonts.CaptionFontFamily, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// The FontFamily property specifies the font family of the title.
        /// </summary>
        [Bindable(true)]
        public FontFamily TitleFontFamily
        {
            get => (FontFamily)this.GetValue(TitleFontFamilyProperty);
            set => this.SetValue(TitleFontFamilyProperty, value);
        }

        /// <summary>Identifies the <see cref="TitleBarIcon"/> dependency property.</summary>
        public static readonly DependencyProperty TitleBarIconProperty
            = DependencyProperty.Register(nameof(TitleBarIcon),
                                          typeof(object),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Gets or sets a icon for the title bar.
        /// </summary>
        [Bindable(true)]
        public object? TitleBarIcon
        {
            get => this.GetValue(TitleBarIconProperty);
            set => this.SetValue(TitleBarIconProperty, value);
        }

        /// <summary>Identifies the <see cref="TitleBarIconTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty TitleBarIconTemplateProperty
            = DependencyProperty.Register(nameof(TitleBarIconTemplate),
                                          typeof(DataTemplate),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Gets or sets a icon content template for the title bar.
        /// </summary>
        [Bindable(true)]
        public DataTemplate? TitleBarIconTemplate
        {
            get => (DataTemplate?)this.GetValue(TitleBarIconTemplateProperty);
            set => this.SetValue(TitleBarIconTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="ShowTitleBarCloseButton"/> dependency property.</summary>
        public static readonly DependencyProperty ShowTitleBarCloseButtonProperty
            = DependencyProperty.Register(nameof(ShowTitleBarCloseButton),
                                          typeof(bool),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Gets or sets if the close button is visible.
        /// </summary>
        public bool ShowTitleBarCloseButton
        {
            get => (bool)this.GetValue(ShowTitleBarCloseButtonProperty);
            set => this.SetValue(ShowTitleBarCloseButtonProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="TitleBarCloseButtonStyle"/> dependency property.</summary>
        public static readonly DependencyProperty TitleBarCloseButtonStyleProperty
            = DependencyProperty.Register(nameof(TitleBarCloseButtonStyle),
                                          typeof(Style),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Gets or sets the close button style.
        /// </summary>
        [Bindable(true)]
        public Style? TitleBarCloseButtonStyle
        {
            get => (Style?)this.GetValue(TitleBarCloseButtonStyleProperty);
            set => this.SetValue(TitleBarCloseButtonStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="TitleBarCloseButtonCommand"/> dependency property.</summary>
        public static readonly DependencyProperty TitleBarCloseButtonCommandProperty
            = DependencyProperty.Register(nameof(TitleBarCloseButtonCommand),
                                          typeof(ICommand),
                                          typeof(ChildWindow),
                                          new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// Gets or sets the command that runs when the close button in the title bar is clicked. Its CanExecute decides whether that button closes the window.
        /// </summary>
        public ICommand? TitleBarCloseButtonCommand
        {
            get => (ICommand?)this.GetValue(TitleBarCloseButtonCommandProperty);
            set => this.SetValue(TitleBarCloseButtonCommandProperty, value);
        }

        /// <summary>Identifies the <see cref="TitleBarCloseButtonCommandParameter"/> dependency property.</summary>
        public static readonly DependencyProperty TitleBarCloseButtonCommandParameterProperty
            = DependencyProperty.Register(nameof(TitleBarCloseButtonCommandParameter),
                                          typeof(object),
                                          typeof(ChildWindow),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command parameter that is used by the TitleBarCloseButtonCommand when the Close Button is clicked.
        /// </summary>
        public object? TitleBarCloseButtonCommandParameter
        {
            get => this.GetValue(TitleBarCloseButtonCommandParameterProperty);
            set => this.SetValue(TitleBarCloseButtonCommandParameterProperty, value);
        }

        /// <summary>Identifies the <see cref="IsOpen"/> dependency property.</summary>
        public static readonly DependencyProperty IsOpenProperty
            = DependencyProperty.Register(nameof(IsOpen),
                                          typeof(bool),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsOpenChanged));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open or closed.
        /// </summary>
        public bool IsOpen
        {
            get => (bool)this.GetValue(IsOpenProperty);
            set => this.SetValue(IsOpenProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ChildWindowWidth"/> dependency property.</summary>
        public static readonly DependencyProperty ChildWindowWidthProperty
            = DependencyProperty.Register(nameof(ChildWindowWidth),
                                          typeof(double),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure), IsWidthHeightValid);

        /// <summary>
        /// Gets or sets the width of the child window.
        /// </summary>
        public double ChildWindowWidth
        {
            get => (double)this.GetValue(ChildWindowWidthProperty);
            set => this.SetValue(ChildWindowWidthProperty, value);
        }

        private static bool IsWidthHeightValid(object? value)
        {
            var v = (double)value!;
            return (double.IsNaN(v)) || (v >= 0.0d && !double.IsPositiveInfinity(v));
        }

        /// <summary>Identifies the <see cref="ChildWindowHeight"/> dependency property.</summary>
        public static readonly DependencyProperty ChildWindowHeightProperty
            = DependencyProperty.Register(nameof(ChildWindowHeight),
                                          typeof(double),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure), IsWidthHeightValid);

        /// <summary>
        /// Gets or sets the height of the child window.
        /// </summary>
        public double ChildWindowHeight
        {
            get => (double)this.GetValue(ChildWindowHeightProperty);
            set => this.SetValue(ChildWindowHeightProperty, value);
        }

        /// <summary>Identifies the <see cref="ChildWindowMinWidth"/> dependency property.</summary>
        public static readonly DependencyProperty ChildWindowMinWidthProperty
            = DependencyProperty.Register(nameof(ChildWindowMinWidth),
                                          typeof(double),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure), IsWidthHeightValid);

        /// <summary>
        /// Gets or sets the min width of the child window.
        /// </summary>
        public double ChildWindowMinWidth
        {
            get => (double)this.GetValue(ChildWindowMinWidthProperty);
            set => this.SetValue(ChildWindowMinWidthProperty, value);
        }

        /// <summary>Identifies the <see cref="ChildWindowMinHeight"/> dependency property.</summary>
        public static readonly DependencyProperty ChildWindowMinHeightProperty
            = DependencyProperty.Register(nameof(ChildWindowMinHeight),
                                          typeof(double),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure), IsWidthHeightValid);

        /// <summary>
        /// Gets or sets the min height of the child window.
        /// </summary>
        public double ChildWindowMinHeight
        {
            get => (double)this.GetValue(ChildWindowMinHeightProperty);
            set => this.SetValue(ChildWindowMinHeightProperty, value);
        }


        /// <summary>Identifies the <see cref="ChildWindowMaxWidth"/> dependency property.</summary>
        public static readonly DependencyProperty ChildWindowMaxWidthProperty
            = DependencyProperty.Register(nameof(ChildWindowMaxWidth),
                                          typeof(double),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure), IsWidthHeightValid);

        /// <summary>
        /// Gets or sets the max width of the child window.
        /// </summary>
        public double ChildWindowMaxWidth
        {
            get => (double)this.GetValue(ChildWindowMaxWidthProperty);
            set => this.SetValue(ChildWindowMaxWidthProperty, value);
        }

        /// <summary>Identifies the <see cref="ChildWindowMaxHeight"/> dependency property.</summary>
        public static readonly DependencyProperty ChildWindowMaxHeightProperty
            = DependencyProperty.Register(nameof(ChildWindowMaxHeight),
                                          typeof(double),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure), IsWidthHeightValid);

        /// <summary>
        /// Gets or sets the max height of the child window.
        /// </summary>
        public double ChildWindowMaxHeight
        {
            get => (double)this.GetValue(ChildWindowMaxHeightProperty);
            set => this.SetValue(ChildWindowMaxHeightProperty, value);
        }

        /// <summary>Identifies the <see cref="ChildWindowImage"/> dependency property.</summary>
        public static readonly DependencyProperty ChildWindowImageProperty
            = DependencyProperty.Register(nameof(ChildWindowImage),
                                          typeof(MessageBoxImage),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(MessageBoxImage.None, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets which image is shown on the left side of the window content.
        /// </summary>
        public MessageBoxImage ChildWindowImage
        {
            get => (MessageBoxImage)this.GetValue(ChildWindowImageProperty);
            set => this.SetValue(ChildWindowImageProperty, value);
        }

        /// <summary>Identifies the <see cref="EnableDropShadow"/> dependency property.</summary>
        public static readonly DependencyProperty EnableDropShadowProperty
            = DependencyProperty.Register(nameof(EnableDropShadow),
                                          typeof(bool),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(BooleanBoxes.TrueBox, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets a value indicating whether the window has a drop shadow (glow brush).
        /// </summary>
        public bool EnableDropShadow
        {
            get => (bool)this.GetValue(EnableDropShadowProperty);
            set => this.SetValue(EnableDropShadowProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="AllowFocusElement"/> dependency property.</summary>
        public static readonly DependencyProperty AllowFocusElementProperty
            = DependencyProperty.Register(nameof(AllowFocusElement),
                                          typeof(bool),
                                          typeof(ChildWindow),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets a value indicating whether the child window should try focus an element.
        /// </summary>
        public bool AllowFocusElement
        {
            get => (bool)this.GetValue(AllowFocusElementProperty);
            set => this.SetValue(AllowFocusElementProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="FocusedElement"/> dependency property.</summary>
        public static readonly DependencyProperty FocusedElementProperty
            = DependencyProperty.Register(nameof(FocusedElement),
                                          typeof(FrameworkElement),
                                          typeof(ChildWindow),
                                          new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the focused element.
        /// </summary>
        public FrameworkElement? FocusedElement
        {
            get => (FrameworkElement?)this.GetValue(FocusedElementProperty);
            set => this.SetValue(FocusedElementProperty, value);
        }

        /// <summary>Identifies the <see cref="GlowBrush"/> dependency property.</summary>
        public static readonly DependencyProperty GlowBrushProperty
            = DependencyProperty.Register(nameof(GlowBrush),
                                          typeof(SolidColorBrush),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the glow brush (drop shadow).
        /// </summary>
        public SolidColorBrush GlowBrush
        {
            get => (SolidColorBrush)this.GetValue(GlowBrushProperty);
            set => this.SetValue(GlowBrushProperty, value);
        }

        /// <summary>Identifies the <see cref="NonActiveGlowBrush"/> dependency property.</summary>
        public static readonly DependencyProperty NonActiveGlowBrushProperty
            = DependencyProperty.Register(nameof(NonActiveGlowBrush),
                                          typeof(SolidColorBrush),
                                          typeof(ChildWindow),
                                          new PropertyMetadata(Brushes.Gray));

        /// <summary>
        /// Gets or sets the glow brush (drop shadow) for non-active status.
        /// </summary>
        public SolidColorBrush NonActiveGlowBrush
        {
            get => (SolidColorBrush)this.GetValue(NonActiveGlowBrushProperty);
            set => this.SetValue(NonActiveGlowBrushProperty, value);
        }

        /// <summary>Identifies the <see cref="IsAutoCloseEnabled"/> dependency property.</summary>
        public static readonly DependencyProperty IsAutoCloseEnabledProperty
            = DependencyProperty.Register(nameof(IsAutoCloseEnabled),
                                          typeof(bool),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, OnIsAutoCloseEnabledChanged));

        /// <summary>
        /// Gets or sets a value indicating whether the ChildWindow should auto close after AutoCloseInterval has passed.
        /// </summary>
        public bool IsAutoCloseEnabled
        {
            get => (bool)this.GetValue(IsAutoCloseEnabledProperty);
            set => this.SetValue(IsAutoCloseEnabledProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="AutoCloseInterval"/> dependency property.</summary>
        public static readonly DependencyProperty AutoCloseIntervalProperty
            = DependencyProperty.Register(nameof(AutoCloseInterval),
                                          typeof(long),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(5000L, OnAutoCloseIntervalChanged));

        /// <summary>
        /// Gets or sets the time in milliseconds when the ChildWindow should auto close.
        /// </summary>
        public long AutoCloseInterval
        {
            get => (long)this.GetValue(AutoCloseIntervalProperty);
            set => this.SetValue(AutoCloseIntervalProperty, value);
        }

        /// <summary>Identifies the <see cref="IsWindowHostActive"/> dependency property.</summary>
        public static readonly DependencyProperty IsWindowHostActiveProperty
            = DependencyProperty.Register(nameof(IsWindowHostActive),
                                          typeof(bool),
                                          typeof(ChildWindow),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets a value indicating whether the host Window is active or not.
        /// </summary>
        public bool IsWindowHostActive
        {
            get => (bool)this.GetValue(IsWindowHostActiveProperty);
            set => this.SetValue(IsWindowHostActiveProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="CornerRadius"/> dependency property.</summary>
        public static readonly DependencyProperty CornerRadiusProperty
            = DependencyProperty.Register(nameof(CornerRadius),
                                          typeof(CornerRadius),
                                          typeof(ChildWindow),
                                          new FrameworkPropertyMetadata(new CornerRadius(), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets a value that represents the degree to which the corners of the <see cref="ChildWindow"/> are rounded.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)this.GetValue(CornerRadiusProperty);
            set => this.SetValue(CornerRadiusProperty, value);
        }

        /// <summary>
        /// An event that will be raised when <see cref="IsOpen"/> dependency property changes.
        /// </summary>
        public static readonly RoutedEvent IsOpenChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(IsOpenChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(ChildWindow));

        /// <summary>
        /// An event that will be raised when <see cref="IsOpen"/> dependency property changes.
        /// </summary>
        public event RoutedEventHandler IsOpenChanged
        {
            add => this.AddHandler(IsOpenChangedEvent, value);
            remove => this.RemoveHandler(IsOpenChangedEvent, value);
        }

        /// <summary>
        /// An event that will be raised when the ChildWindow is closing.
        /// </summary>
        public event EventHandler<CancelEventArgs>? Closing;

        /// <summary>
        /// An event that will be raised when the closing animation has finished.
        /// </summary>
        public static readonly RoutedEvent ClosingFinishedEvent
            = EventManager.RegisterRoutedEvent(nameof(ClosingFinished),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(ChildWindow));

        /// <summary>
        /// An event that will be raised when the closing animation has finished.
        /// </summary>
        public event RoutedEventHandler ClosingFinished
        {
            add => this.AddHandler(ClosingFinishedEvent, value);
            remove => this.RemoveHandler(ClosingFinishedEvent, value);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            if (!e.Handled)
            {
                var container = this.Parent as Panel;
                var elementOnTop = container?.Children.OfType<UIElement>().OrderBy(c => c.GetValue(Panel.ZIndexProperty)).LastOrDefault();

                // only another child window gives way, a dialog over this one stays where it is
                if (elementOnTop is ChildWindow && !Equals(elementOnTop, this))
                {
                    var zIndexOnTop = (int)elementOnTop.GetValue(Panel.ZIndexProperty);
                    elementOnTop.SetCurrentValue(Panel.ZIndexProperty, zIndexOnTop - 1);
                    this.SetCurrentValue(Panel.ZIndexProperty, zIndexOnTop);
                }
            }
        }

        private static void OnIsOpenChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (Equals(e.OldValue, e.NewValue))
            {
                return;
            }

            var childWindow = (ChildWindow)dependencyObject;

            if ((bool)e.NewValue)
            {
                // a window shown again starts without what closed it the time before
                childWindow.ChildWindowResult = null;
                childWindow.ClosedBy = CloseReason.None;
            }

            void OpenChangedAction()
            {
                if ((bool)e.NewValue)
                {
                    if (childWindow.hideStoryboard != null)
                    {
                        // don't let the storyboard end it's completed event
                        // otherwise it could be hidden on start
                        childWindow.hideStoryboard.Completed -= childWindow.HideStoryboard_Completed;
                    }

                    var parent = childWindow.Parent as Panel;
                    Panel.SetZIndex(childWindow, parent?.ZIndexOnTop(childWindow) ?? 99);

                    childWindow.TryToSetFocusedElement();

                    if (childWindow.IsAutoCloseEnabled)
                    {
                        childWindow.StartAutoCloseTimer();
                    }
                }
                else
                {
                    childWindow.StopAutoCloseTimer();

                    if (childWindow.hideStoryboard != null)
                    {
                        childWindow.hideStoryboard.Completed += childWindow.HideStoryboard_Completed;
                    }
                    else
                    {
                        childWindow.OnClosingFinished();
                    }
                }

                VisualStateManager.GoToState(childWindow, (bool)e.NewValue == false ? "Hide" : "Show", true);

                childWindow.RaiseEvent(new RoutedEventArgs(IsOpenChangedEvent, childWindow));
            }

            childWindow.Dispatcher?.BeginInvoke(DispatcherPriority.Background, (Action)OpenChangedAction);
        }

        private void TryToSetFocusedElement()
        {
            if (this.AllowFocusElement && !DesignerProperties.GetIsInDesignMode(this))
            {
                // first focus itself
                this.Focus();

                var elementToFocus = this.GetElementToFocus();

                if (elementToFocus != null)
                {
                    void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
                    {
                        elementToFocus.IsVisibleChanged -= OnIsVisibleChanged;

                        // visible is not laid out yet: a text box of the Windows 10 or the WinUI set that gets the
                        // focus before it has its template makes WPF throw while the template loads, so the focus
                        // waits until the layout has run
                        elementToFocus.Dispatcher.BeginInvoke(DispatcherPriority.Input, (Action)(() =>
                            {
                                if (elementToFocus.Focusable && elementToFocus.IsVisible && elementToFocus is HwndHost == false)
                                {
                                    elementToFocus.Focus();
                                }
                            }));
                    }

                    elementToFocus.IsVisibleChanged += OnIsVisibleChanged;
                }
            }
        }

        /// <summary>
        /// The element that gets the focus once the window is open.
        /// </summary>
        protected virtual UIElement? GetElementToFocus()
        {
            var elementToFocus = this.FocusedElement ?? this.FindChildren<UIElement>().FirstOrDefault(c => c.Focusable);
            if (this.ShowTitleBarCloseButton && this.closeButton != null && elementToFocus == null)
            {
                this.closeButton.SetCurrentValue(FocusableProperty, true);
                elementToFocus = this.closeButton;
            }

            return elementToFocus;
        }

        private void HideStoryboard_Completed(object? sender, EventArgs e)
        {
            this.hideStoryboard!.Completed -= this.HideStoryboard_Completed;
            this.OnClosingFinished();
        }

        private void OnClosingFinished()
        {
            this.RaiseEvent(new RoutedEventArgs(ClosingFinishedEvent, this));
        }

        /// <summary>
        /// Gets the child window result when the dialog will be closed.
        /// </summary>
        public object? ChildWindowResult { get; protected set; }

        /// <summary>
        /// Gets the dialog close reason.
        /// </summary>
        public CloseReason ClosedBy { get; private set; } = CloseReason.None;

        private static void OnIsAutoCloseEnabledChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var childWindow = (ChildWindow)dependencyObject;

            void AutoCloseEnabledChangedAction()
            {
                if (e.NewValue != e.OldValue)
                {
                    if ((bool)e.NewValue)
                    {
                        if (childWindow.IsOpen)
                        {
                            childWindow.StartAutoCloseTimer();
                        }
                    }
                    else
                    {
                        childWindow.StopAutoCloseTimer();
                    }
                }
            }

            childWindow.Dispatcher?.BeginInvoke(DispatcherPriority.Background, (Action)AutoCloseEnabledChangedAction);
        }

        private static void OnAutoCloseIntervalChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var childWindow = (ChildWindow)dependencyObject;

            void AutoCloseIntervalChangedAction()
            {
                if (e.NewValue != e.OldValue)
                {
                    childWindow.InitializeAutoCloseTimer();
                    if (childWindow.IsAutoCloseEnabled && childWindow.IsOpen)
                    {
                        childWindow.StartAutoCloseTimer();
                    }
                }
            }

            childWindow.Dispatcher?.BeginInvoke(DispatcherPriority.Background, (Action)AutoCloseIntervalChangedAction);
        }

        private void InitializeAutoCloseTimer()
        {
            this.StopAutoCloseTimer();

            this.autoCloseTimer = new DispatcherTimer();
            this.autoCloseTimer.Tick += this.AutoCloseTimerCallback;
            this.autoCloseTimer.Interval = TimeSpan.FromMilliseconds(this.AutoCloseInterval);
        }

        private void AutoCloseTimerCallback(object? sender, EventArgs e)
        {
            this.StopAutoCloseTimer();

            // if the ChildWindow is open and autoclose is still enabled then close the ChildWindow
            if (this.IsOpen && this.IsAutoCloseEnabled)
            {
                this.Close(CloseReason.AutoClose);
            }
        }

        private void StartAutoCloseTimer()
        {
            // in case it is already running
            this.StopAutoCloseTimer();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.autoCloseTimer?.Start();
            }
        }

        private void StopAutoCloseTimer()
        {
            if (this.autoCloseTimer is { IsEnabled: true })
            {
                this.autoCloseTimer.Stop();
            }
        }

        private string? closeText;

        /// <summary>
        /// Gets the close button tool tip.
        /// </summary>
        public string TitleBarCloseButtonToolTip => this.closeText ??= WinApiHelper.GetCaption(905);

        private Storyboard? hideStoryboard;
        private IMetroThumb? headerThumb;
        private Button? closeButton;
        private readonly TranslateTransform moveTransform = new TranslateTransform();
        private Grid? partWindow;
        private Grid? partOverlay;
        private ContentControl? icon;

        static ChildWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChildWindow), new FrameworkPropertyMetadata(typeof(ChildWindow)));
        }

        public ChildWindow()
        {
            // its own access keys work while the window swallows those of everything under a dialog, as for BaseMetroDialog
            AccessKeyHelper.SetIsAccessKeyScope(this, true);

            this.InitializeAutoCloseTimer();
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.Template == null)
            {
                return;
            }

            var isActiveBindingAction = new Action(() =>
                {
                    var window = Window.GetWindow(this);
                    if (window != null)
                    {
                        this.SetBinding(IsWindowHostActiveProperty, new Binding(nameof(Window.IsActive)) { Source = window, Mode = BindingMode.OneWay });
                    }
                });
            if (!this.IsLoaded)
            {
                this.BeginInvoke(isActiveBindingAction, DispatcherPriority.Loaded);
            }
            else
            {
                isActiveBindingAction();
            }

            this.hideStoryboard = this.Template.FindName(HideStoryboard, this) as Storyboard;

            if (this.partOverlay != null)
            {
                this.partOverlay.MouseLeftButtonDown -= this.PartOverlayOnClose;
                this.partOverlay.SizeChanged -= this.PartOverlay_SizeChanged;
            }

            this.partOverlay = this.Template.FindName(PART_Overlay, this) as Grid;
            if (this.partOverlay != null)
            {
                this.partOverlay.MouseLeftButtonDown += this.PartOverlayOnClose;
                this.partOverlay.SizeChanged += this.PartOverlay_SizeChanged;
            }

            this.partWindow = this.Template.FindName(PART_Window, this) as Grid;
            this.partWindow?.SetCurrentValue(RenderTransformProperty, this.moveTransform);

            this.icon = this.Template.FindName(PART_TitleBarIcon, this) as ContentControl;

            if (this.headerThumb != null)
            {
                this.headerThumb.DragDelta -= this.OnTitleBarThumbDragDelta;
            }

            this.headerThumb = this.Template.FindName(PART_TitleBarThumb, this) as IMetroThumb;
            if (this.headerThumb != null && this.partWindow != null)
            {
                this.headerThumb.DragDelta += this.OnTitleBarThumbDragDelta;
            }

            if (this.closeButton != null)
            {
                this.closeButton.Click -= this.OnCloseButtonClick;
            }

            this.closeButton = this.Template.FindName(PART_TitleBarCloseButton, this) as Button;
            if (this.closeButton != null)
            {
                this.closeButton.Click += this.OnCloseButtonClick;
            }
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ChildWindowAutomationPeer(this);
        }

        private void PartOverlayOnClose(object sender, MouseButtonEventArgs e)
        {
            if (Equals(e.OriginalSource, this.partOverlay) && this.CloseOnOverlay)
            {
                this.Close(CloseReason.Overlay);
            }
        }

        private void PartOverlay_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ProcessMove(0, 0);
        }

        /// <summary>
        /// Moves the window by a drag on its title, for a template that offers more than one place to drag it by.
        /// </summary>
        protected void OnTitleBarThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.partWindow is null)
            {
                return;
            }

            var allowDragging = this.AllowMove && this.partWindow.HorizontalAlignment != HorizontalAlignment.Stretch && this.partWindow.VerticalAlignment != VerticalAlignment.Stretch;
            // drag only if IsWindowDraggable is set to true
            if (allowDragging && (Math.Abs(e.HorizontalChange) > 2 || Math.Abs(e.VerticalChange) > 2))
            {
                this.ProcessMove(e.HorizontalChange, e.VerticalChange);
            }
        }

        private void ProcessMove(double x, double y)
        {
            if (this.partOverlay is null || this.partWindow is null)
            {
                return;
            }

            var width = this.partOverlay.RenderSize.Width;
            var height = this.partOverlay.RenderSize.Height;

            var offset = VisualTreeHelper.GetOffset(this.partWindow);
            var widthOffset = offset.X;
            var heightOffset = offset.Y;

            var realX = this.moveTransform.X + x + widthOffset;
            var realY = this.moveTransform.Y + y + heightOffset;

            const int extraGap = 5;
            var widthGap = Math.Max(this.icon?.ActualWidth + 5 ?? 30, 30);
            var heightGap = Math.Max(this.TitleBarHeight, 30d);
            var changeX = this.moveTransform.X;
            var changeY = this.moveTransform.Y;

            if (realX < (0 + extraGap))
            {
                changeX = -widthOffset + extraGap;
            }
            else if (realX > (width - widthGap - extraGap))
            {
                changeX = width - widthOffset - widthGap - extraGap;
            }
            else
            {
                changeX += x;
            }

            if (realY < (0 + extraGap))
            {
                changeY = -heightOffset + extraGap;
            }
            else if (realY > (height - heightGap - extraGap))
            {
                changeY = height - heightOffset - heightGap - extraGap;
            }
            else
            {
                changeY += y;
            }

            if (!Equals(changeX, this.moveTransform.X) || !Equals(changeY, this.moveTransform.Y))
            {
                this.SetCurrentValue(OffsetXProperty, changeX);
                this.SetCurrentValue(OffsetYProperty, changeY);

                this.InvalidateArrange();
            }
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.OnTitleBarCloseButtonClick();
        }

        /// <summary>
        /// Called when the close button in the title bar is clicked. Runs <see cref="TitleBarCloseButtonCommand"/>,
        /// whose CanExecute can keep the window open, and closes the window.
        /// </summary>
        protected virtual void OnTitleBarCloseButtonClick()
        {
            var command = this.TitleBarCloseButtonCommand;
            var parameter = this.TitleBarCloseButtonCommandParameter ?? this;
            if (command is not null && !command.CanExecute(parameter))
            {
                return;
            }

            if (this.Close(CloseReason.Close))
            {
                command?.Execute(parameter);
            }
        }

        /// <summary>
        /// This method fires the <see cref="Closing"/> event.
        /// </summary>
        /// <param name="e">The EventArgs for the closing step.</param>
        protected virtual void OnClosing(CancelEventArgs e)
        {
            this.Closing?.Invoke(this, e);
        }

        /// <summary>
        /// Creates the arguments of the <see cref="Closing"/> event, so that a derived window can hand out its own.
        /// </summary>
        /// <param name="closedBy">Why the window closes.</param>
        /// <param name="childWindowResult">The result it closes with.</param>
        protected virtual CancelEventArgs CreateClosingEventArgs(CloseReason closedBy, object? childWindowResult)
        {
            return new CancelEventArgs();
        }

        /// <summary>
        /// Closes this dialog.
        /// </summary>
        /// <param name="childWindowResult">A dialog result (optional).</param>
        public bool Close(object? childWindowResult = null)
        {
            return this.Close(CloseReason.Close, childWindowResult);
        }

        /// <summary>
        /// Closes this dialog.
        /// </summary>
        /// <param name="closedBy">The dialog close reason.</param>
        /// <param name="childWindowResult">A dialog result (optional).</param>
        public bool Close(CloseReason closedBy, object? childWindowResult = null)
        {
            this.ClosedBy = closedBy;

            // check if we really want close the dialog
            var e = this.CreateClosingEventArgs(closedBy, childWindowResult);
            this.OnClosing(e);

            if (!e.Cancel)
            {
                this.ChildWindowResult = childWindowResult;
                this.SetCurrentValue(IsOpenProperty, BooleanBoxes.FalseBox);
                return true;
            }
            else
            {
                this.ClosedBy = CloseReason.None;
                return false;
            }
        }

        /// <inheritdoc />
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (this.CloseByEscape && e.Key == Key.Escape)
            {
                e.Handled = this.Close(CloseReason.Escape);
            }

            base.OnKeyDown(e);
        }
    }
}