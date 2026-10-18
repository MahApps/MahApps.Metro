// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// Ported from RatingControl.cpp of microsoft-ui-xaml, Copyright (c) Microsoft Corporation, MIT License.

using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Automation.Peers;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A control to show and set a rating, drawn as a row of glyphs or images.
    /// </summary>
    [TemplatePart(Name = PART_CaptionStackPanel, Type = typeof(StackPanel))]
    [TemplatePart(Name = PART_Caption, Type = typeof(TextBlock))]
    [TemplatePart(Name = PART_RatingBackgroundStackPanel, Type = typeof(StackPanel))]
    [TemplatePart(Name = PART_RatingForegroundStackPanel, Type = typeof(StackPanel))]
    public class RatingControl : Control
    {
        private const string PART_CaptionStackPanel = "PART_CaptionStackPanel";
        private const string PART_Caption = "PART_Caption";
        private const string PART_RatingBackgroundStackPanel = "PART_RatingBackgroundStackPanel";
        private const string PART_RatingForegroundStackPanel = "PART_RatingForegroundStackPanel";

        private const double NoValueSetSentinel = -1.0;

        private const string FontSizeForRenderingKey = "MahApps.Sizes.RatingControl.FontSizeForRendering";
        private const string ItemSpacingKey = "MahApps.Sizes.RatingControl.ItemSpacing";
        private const string ScaleOnPointerOverKey = "MahApps.Flags.RatingControl.ScaleOnPointerOver";
        private const double DefaultFontSizeForRendering = 32.0;
        private const double DefaultItemSpacing = 8.0;

        // the items are drawn at twice their size and scaled down by half
        private const double ItemScale = 0.5;

        // the stars under the pointer grow up to these, for a mouse or a pen and for a finger
        private const double MouseOverScale = 0.8;
        private const double TouchOverScale = 1.0;

        private static readonly RatingItemFontInfo DefaultFontInfo = CreateDefaultFontInfo();

        private StackPanel? backgroundStackPanel;
        private StackPanel? foregroundStackPanel;
        private double fontSizeForRendering = DefaultFontSizeForRendering;
        private double itemSpacing = DefaultItemSpacing;
        private bool isImageInfo;
        private bool isPointerOver;
        private bool isPointerDown;
        private double mousePercentage;
        private DependencyProperty? stateBrushProperty;

        // the tests replace this, since the position of a raised mouse event can't be set
        private Func<MouseEventArgs, IInputElement, Point> pointerPosition = (e, relativeTo) => e.GetPosition(relativeTo);

        private enum RatingControlStates
        {
            Unset,
            Set,
            PointerOverSet,
            PointerOverPlaceholder,
            Placeholder,
            Disabled
        }

        static RatingControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RatingControl), new FrameworkPropertyMetadata(typeof(RatingControl)));
        }

        /// <summary>Identifies the <see cref="Value"/> dependency property.</summary>
        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.Register(nameof(Value),
                                          typeof(double),
                                          typeof(RatingControl),
                                          new FrameworkPropertyMetadata(NoValueSetSentinel, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, CoerceValue));

        /// <summary>
        /// Gets or sets the rating. A negative value means no rating is set, anything else is held between 1 and <see cref="MaxRating"/>.
        /// </summary>
        public double Value
        {
            get => (double)this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        private static object CoerceValue(DependencyObject d, object? baseValue)
        {
            var value = (double)baseValue!;
            var maxRating = ((RatingControl)d).MaxRating;

            if (value < 0.0)
            {
                // all negative values are the sentinel for "unset"
                return NoValueSetSentinel;
            }

            if (value <= 1.0)
            {
                // the minimum valid, non-sentinel value
                return 1.0;
            }

            return value > maxRating ? (double)maxRating : value;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RatingControl)d;
            control.UpdateRatingItemsAppearance();

            if (UIElementAutomationPeer.FromElement(control) is RatingControlAutomationPeer peer)
            {
                peer.RaiseValuePropertyChangedEvent((double)e.OldValue, (double)e.NewValue);
            }
        }

        /// <summary>Identifies the <see cref="MaxRating"/> dependency property.</summary>
        public static readonly DependencyProperty MaxRatingProperty
            = DependencyProperty.Register(nameof(MaxRating),
                                          typeof(int),
                                          typeof(RatingControl),
                                          new FrameworkPropertyMetadata(5, OnMaxRatingChanged, CoerceMaxRating));

        /// <summary>
        /// Gets or sets the number of items, at least 1.
        /// </summary>
        public int MaxRating
        {
            get => (int)this.GetValue(MaxRatingProperty);
            set => this.SetValue(MaxRatingProperty, value);
        }

        private static object CoerceMaxRating(DependencyObject d, object? baseValue)
        {
            return Math.Max(1, (int)baseValue!);
        }

        private static void OnMaxRatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(ValueProperty);
            d.CoerceValue(PlaceholderValueProperty);
            ((RatingControl)d).StampOutRatingItems();
        }

        /// <summary>Identifies the <see cref="PlaceholderValue"/> dependency property.</summary>
        public static readonly DependencyProperty PlaceholderValueProperty
            = DependencyProperty.Register(nameof(PlaceholderValue),
                                          typeof(double),
                                          typeof(RatingControl),
                                          new FrameworkPropertyMetadata(NoValueSetSentinel, OnPlaceholderValueChanged, CoercePlaceholderValue));

        /// <summary>
        /// Gets or sets a value to show while no <see cref="Value"/> is set, for example an average rating. A fraction is kept.
        /// </summary>
        public double PlaceholderValue
        {
            get => (double)this.GetValue(PlaceholderValueProperty);
            set => this.SetValue(PlaceholderValueProperty, value);
        }

        private static object CoercePlaceholderValue(DependencyObject d, object? baseValue)
        {
            var value = (double)baseValue!;
            var maxRating = Math.Max(1, ((RatingControl)d).MaxRating);

            if (value < 0.0)
            {
                return NoValueSetSentinel;
            }

            return value > maxRating ? (double)maxRating : value;
        }

        private static void OnPlaceholderValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RatingControl)d).UpdateRatingItemsAppearance();
        }

        /// <summary>Identifies the <see cref="InitialSetValue"/> dependency property.</summary>
        public static readonly DependencyProperty InitialSetValueProperty
            = DependencyProperty.Register(nameof(InitialSetValue),
                                          typeof(int),
                                          typeof(RatingControl),
                                          new PropertyMetadata(1));

        /// <summary>
        /// Gets or sets the value the first input sets when there is no <see cref="Value"/> yet.
        /// </summary>
        public int InitialSetValue
        {
            get => (int)this.GetValue(InitialSetValueProperty);
            set => this.SetValue(InitialSetValueProperty, value);
        }

        /// <summary>Identifies the <see cref="IsClearEnabled"/> dependency property.</summary>
        public static readonly DependencyProperty IsClearEnabledProperty
            = DependencyProperty.Register(nameof(IsClearEnabled),
                                          typeof(bool),
                                          typeof(RatingControl),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether the user can clear the rating.
        /// </summary>
        public bool IsClearEnabled
        {
            get => (bool)this.GetValue(IsClearEnabledProperty);
            set => this.SetValue(IsClearEnabledProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="IsReadOnly"/> dependency property.</summary>
        public static readonly DependencyProperty IsReadOnlyProperty
            = DependencyProperty.Register(nameof(IsReadOnly),
                                          typeof(bool),
                                          typeof(RatingControl),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets whether the user can change the rating.
        /// </summary>
        public bool IsReadOnly
        {
            get => (bool)this.GetValue(IsReadOnlyProperty);
            set => this.SetValue(IsReadOnlyProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="Caption"/> dependency property.</summary>
        public static readonly DependencyProperty CaptionProperty
            = DependencyProperty.Register(nameof(Caption),
                                          typeof(string),
                                          typeof(RatingControl),
                                          new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the text shown beside the items.
        /// </summary>
        public string? Caption
        {
            get => (string?)this.GetValue(CaptionProperty);
            set => this.SetValue(CaptionProperty, value);
        }

        /// <summary>Identifies the <see cref="ItemInfo"/> dependency property.</summary>
        public static readonly DependencyProperty ItemInfoProperty
            = DependencyProperty.Register(nameof(ItemInfo),
                                          typeof(RatingItemInfo),
                                          typeof(RatingControl),
                                          new PropertyMetadata(null, OnItemInfoChanged));

        /// <summary>
        /// Gets or sets the glyphs or images the items are drawn with. Without one the default glyphs are used.
        /// </summary>
        public RatingItemInfo? ItemInfo
        {
            get => (RatingItemInfo?)this.GetValue(ItemInfoProperty);
            set => this.SetValue(ItemInfoProperty, value);
        }

        // a change inside an unfrozen info comes in here as well, as a sub-property change
        private static void OnItemInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RatingControl)d).OnItemInfoChanged();
        }

        /// <summary>Identifies the <c>UnselectedBrush</c> attached property.</summary>
        internal static readonly DependencyProperty UnselectedBrushProperty
            = DependencyProperty.RegisterAttached("UnselectedBrush",
                                                  typeof(Brush),
                                                  typeof(RatingControl),
                                                  new PropertyMetadata(null, OnStateBrushChanged));

        /// <summary>Gets the brush of the items that are not set.</summary>
        [AttachedPropertyBrowsableForType(typeof(RatingControl))]
        internal static Brush? GetUnselectedBrush(DependencyObject element) => (Brush?)element.GetValue(UnselectedBrushProperty);

        /// <summary>Sets the brush of the items that are not set.</summary>
        [AttachedPropertyBrowsableForType(typeof(RatingControl))]
        internal static void SetUnselectedBrush(DependencyObject element, Brush? value) => element.SetValue(UnselectedBrushProperty, value);

        /// <summary>Identifies the <c>SelectedBrush</c> attached property.</summary>
        internal static readonly DependencyProperty SelectedBrushProperty
            = DependencyProperty.RegisterAttached("SelectedBrush",
                                                  typeof(Brush),
                                                  typeof(RatingControl),
                                                  new PropertyMetadata(null, OnStateBrushChanged));

        /// <summary>Gets the brush of the set items.</summary>
        [AttachedPropertyBrowsableForType(typeof(RatingControl))]
        internal static Brush? GetSelectedBrush(DependencyObject element) => (Brush?)element.GetValue(SelectedBrushProperty);

        /// <summary>Sets the brush of the set items.</summary>
        [AttachedPropertyBrowsableForType(typeof(RatingControl))]
        internal static void SetSelectedBrush(DependencyObject element, Brush? value) => element.SetValue(SelectedBrushProperty, value);

        /// <summary>Identifies the <c>PlaceholderBrush</c> attached property.</summary>
        internal static readonly DependencyProperty PlaceholderBrushProperty
            = DependencyProperty.RegisterAttached("PlaceholderBrush",
                                                  typeof(Brush),
                                                  typeof(RatingControl),
                                                  new PropertyMetadata(null, OnStateBrushChanged));

        /// <summary>Gets the brush of the items of the placeholder value.</summary>
        [AttachedPropertyBrowsableForType(typeof(RatingControl))]
        internal static Brush? GetPlaceholderBrush(DependencyObject element) => (Brush?)element.GetValue(PlaceholderBrushProperty);

        /// <summary>Sets the brush of the items of the placeholder value.</summary>
        [AttachedPropertyBrowsableForType(typeof(RatingControl))]
        internal static void SetPlaceholderBrush(DependencyObject element, Brush? value) => element.SetValue(PlaceholderBrushProperty, value);

        /// <summary>Identifies the <c>PointerOverPlaceholderBrush</c> attached property.</summary>
        internal static readonly DependencyProperty PointerOverPlaceholderBrushProperty
            = DependencyProperty.RegisterAttached("PointerOverPlaceholderBrush",
                                                  typeof(Brush),
                                                  typeof(RatingControl),
                                                  new PropertyMetadata(null, OnStateBrushChanged));

        /// <summary>Gets the brush of the items under the pointer while neither a value nor a placeholder is set.</summary>
        [AttachedPropertyBrowsableForType(typeof(RatingControl))]
        internal static Brush? GetPointerOverPlaceholderBrush(DependencyObject element) => (Brush?)element.GetValue(PointerOverPlaceholderBrushProperty);

        /// <summary>Sets the brush of the items under the pointer while neither a value nor a placeholder is set.</summary>
        [AttachedPropertyBrowsableForType(typeof(RatingControl))]
        internal static void SetPointerOverPlaceholderBrush(DependencyObject element, Brush? value) => element.SetValue(PointerOverPlaceholderBrushProperty, value);

        /// <summary>Identifies the <c>PointerOverUnselectedBrush</c> attached property.</summary>
        internal static readonly DependencyProperty PointerOverUnselectedBrushProperty
            = DependencyProperty.RegisterAttached("PointerOverUnselectedBrush",
                                                  typeof(Brush),
                                                  typeof(RatingControl),
                                                  new PropertyMetadata(null, OnStateBrushChanged));

        /// <summary>Gets the brush of the items under the pointer while only a placeholder is set.</summary>
        [AttachedPropertyBrowsableForType(typeof(RatingControl))]
        internal static Brush? GetPointerOverUnselectedBrush(DependencyObject element) => (Brush?)element.GetValue(PointerOverUnselectedBrushProperty);

        /// <summary>Sets the brush of the items under the pointer while only a placeholder is set.</summary>
        [AttachedPropertyBrowsableForType(typeof(RatingControl))]
        internal static void SetPointerOverUnselectedBrush(DependencyObject element, Brush? value) => element.SetValue(PointerOverUnselectedBrushProperty, value);

        /// <summary>Identifies the <c>PointerOverSelectedBrush</c> attached property.</summary>
        internal static readonly DependencyProperty PointerOverSelectedBrushProperty
            = DependencyProperty.RegisterAttached("PointerOverSelectedBrush",
                                                  typeof(Brush),
                                                  typeof(RatingControl),
                                                  new PropertyMetadata(null, OnStateBrushChanged));

        /// <summary>Gets the brush of the items under the pointer while a value is set.</summary>
        [AttachedPropertyBrowsableForType(typeof(RatingControl))]
        internal static Brush? GetPointerOverSelectedBrush(DependencyObject element) => (Brush?)element.GetValue(PointerOverSelectedBrushProperty);

        /// <summary>Sets the brush of the items under the pointer while a value is set.</summary>
        [AttachedPropertyBrowsableForType(typeof(RatingControl))]
        internal static void SetPointerOverSelectedBrush(DependencyObject element, Brush? value) => element.SetValue(PointerOverSelectedBrushProperty, value);

        /// <summary>Identifies the <c>DisabledSelectedBrush</c> attached property.</summary>
        internal static readonly DependencyProperty DisabledSelectedBrushProperty
            = DependencyProperty.RegisterAttached("DisabledSelectedBrush",
                                                  typeof(Brush),
                                                  typeof(RatingControl),
                                                  new PropertyMetadata(null, OnStateBrushChanged));

        /// <summary>Gets the brush of the set items of a disabled control.</summary>
        [AttachedPropertyBrowsableForType(typeof(RatingControl))]
        internal static Brush? GetDisabledSelectedBrush(DependencyObject element) => (Brush?)element.GetValue(DisabledSelectedBrushProperty);

        /// <summary>Sets the brush of the set items of a disabled control.</summary>
        [AttachedPropertyBrowsableForType(typeof(RatingControl))]
        internal static void SetDisabledSelectedBrush(DependencyObject element, Brush? value) => element.SetValue(DisabledSelectedBrushProperty, value);

        /// <summary>Identifies the <c>CaptionBrush</c> attached property.</summary>
        internal static readonly DependencyProperty CaptionBrushProperty
            = DependencyProperty.RegisterAttached("CaptionBrush",
                                                  typeof(Brush),
                                                  typeof(RatingControl),
                                                  new PropertyMetadata(null, OnStateBrushChanged));

        /// <summary>Gets the brush of the caption.</summary>
        [AttachedPropertyBrowsableForType(typeof(RatingControl))]
        internal static Brush? GetCaptionBrush(DependencyObject element) => (Brush?)element.GetValue(CaptionBrushProperty);

        /// <summary>Sets the brush of the caption.</summary>
        [AttachedPropertyBrowsableForType(typeof(RatingControl))]
        internal static void SetCaptionBrush(DependencyObject element, Brush? value) => element.SetValue(CaptionBrushProperty, value);

        /// <summary>Identifies the <c>CaptionFontFamily</c> attached property.</summary>
        internal static readonly DependencyProperty CaptionFontFamilyProperty
            = DependencyProperty.RegisterAttached("CaptionFontFamily",
                                                  typeof(FontFamily),
                                                  typeof(RatingControl),
                                                  new PropertyMetadata(SystemFonts.MessageFontFamily));

        /// <summary>Gets the font of the caption, which must not be the symbol font of the items.</summary>
        [AttachedPropertyBrowsableForType(typeof(RatingControl))]
        internal static FontFamily GetCaptionFontFamily(DependencyObject element) => (FontFamily)element.GetValue(CaptionFontFamilyProperty);

        /// <summary>Sets the font of the caption, which must not be the symbol font of the items.</summary>
        [AttachedPropertyBrowsableForType(typeof(RatingControl))]
        internal static void SetCaptionFontFamily(DependencyObject element, FontFamily value) => element.SetValue(CaptionFontFamilyProperty, value);

        private static void OnStateBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RatingControl ratingControl && e.Property == ratingControl.stateBrushProperty)
            {
                ratingControl.ApplyStateBrush();
            }
        }

        /// <summary>Identifies the <see cref="ValueChanged"/> routed event.</summary>
        public static readonly RoutedEvent ValueChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(ValueChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedPropertyChangedEventHandler<double>),
                                               typeof(RatingControl));

        /// <summary>
        /// Occurs when an input changed the rating. Setting <see cref="Value"/> from code does not raise it.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> ValueChanged
        {
            add => this.AddHandler(ValueChangedEvent, value);
            remove => this.RemoveHandler(ValueChangedEvent, value);
        }

        private static RatingItemFontInfo CreateDefaultFontInfo()
        {
            var info = new RatingItemFontInfo { Glyph = "\uE735", UnsetGlyph = "\uE734" };
            info.Freeze();
            return info;
        }

        private RatingItemInfo EffectiveItemInfo => this.ItemInfo ?? DefaultFontInfo;

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RatingControlAutomationPeer(this);
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            if (this.backgroundStackPanel is not null)
            {
                this.backgroundStackPanel.MouseEnter -= this.OnBackgroundStackPanelMouseEnter;
                this.backgroundStackPanel.MouseMove -= this.OnBackgroundStackPanelMouseMove;
                this.backgroundStackPanel.MouseLeave -= this.OnBackgroundStackPanelMouseLeave;
                this.backgroundStackPanel.MouseLeftButtonDown -= this.OnBackgroundStackPanelMouseLeftButtonDown;
                this.backgroundStackPanel.MouseLeftButtonUp -= this.OnBackgroundStackPanelMouseLeftButtonUp;
                this.backgroundStackPanel.LostMouseCapture -= this.OnBackgroundStackPanelLostMouseCapture;
            }

            base.OnApplyTemplate();

            this.backgroundStackPanel = this.GetTemplateChild(PART_RatingBackgroundStackPanel) as StackPanel;
            this.foregroundStackPanel = this.GetTemplateChild(PART_RatingForegroundStackPanel) as StackPanel;

            if (this.backgroundStackPanel is not null)
            {
                this.backgroundStackPanel.MouseEnter += this.OnBackgroundStackPanelMouseEnter;
                this.backgroundStackPanel.MouseMove += this.OnBackgroundStackPanelMouseMove;
                this.backgroundStackPanel.MouseLeave += this.OnBackgroundStackPanelMouseLeave;
                this.backgroundStackPanel.MouseLeftButtonDown += this.OnBackgroundStackPanelMouseLeftButtonDown;
                this.backgroundStackPanel.MouseLeftButtonUp += this.OnBackgroundStackPanelMouseLeftButtonUp;
                this.backgroundStackPanel.LostMouseCapture += this.OnBackgroundStackPanelLostMouseCapture;
            }

            this.StampOutRatingItems();
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == IsEnabledProperty)
            {
                this.UpdateRatingItemsAppearance();
            }
        }

        private void LoadResources()
        {
            this.fontSizeForRendering = this.TryFindResource(FontSizeForRenderingKey) is double fontSize ? fontSize : DefaultFontSizeForRendering;
            this.itemSpacing = this.TryFindResource(ItemSpacingKey) is double spacing ? spacing : DefaultItemSpacing;
        }

        private void StampOutRatingItems()
        {
            if (this.backgroundStackPanel is null || this.foregroundStackPanel is null)
            {
                // the template is not applied yet, OnApplyTemplate comes back here
                return;
            }

            this.LoadResources();
            this.isImageInfo = this.EffectiveItemInfo is RatingItemImageInfo;

            this.backgroundStackPanel.Children.Clear();
            this.foregroundStackPanel.Children.Clear();

            var maxRating = this.MaxRating;
            for (var i = 0; i < maxRating; i++)
            {
                var isLast = i == maxRating - 1;
                this.backgroundStackPanel.Children.Add(this.CreateRatingItem(isLast, RatingControlStates.Unset));
                this.foregroundStackPanel.Children.Add(this.CreateRatingItem(isLast, RatingControlStates.Set));
            }

            this.UpdateRatingItemsAppearance();
        }

        private FrameworkElement CreateRatingItem(bool isLast, RatingControlStates state)
        {
            var size = this.fontSizeForRendering;

            // the negative margin takes the room the scale leaves empty off again, so the layout sees the scaled
            // item only, and a StackPanel of WPF has no spacing, so the room between two items is margin as well
            var inset = size * (1.0 - ItemScale) / 2.0;

            FrameworkElement item = this.isImageInfo
                ? new Image()
                : new TextBlock { FontSize = size, TextAlignment = TextAlignment.Center };
            item.Width = size;
            item.Height = size;
            item.Margin = new Thickness(-inset, -inset, isLast ? -inset : this.itemSpacing - inset, -inset);
            item.RenderTransform = new ScaleTransform(ItemScale, ItemScale, size / 2.0, size / 2.0);

            this.CustomizeRatingItem(item, state);
            return item;
        }

        private void OnItemInfoChanged()
        {
            if (this.backgroundStackPanel is null || this.foregroundStackPanel is null)
            {
                return;
            }

            if ((this.EffectiveItemInfo is RatingItemImageInfo) != this.isImageInfo)
            {
                // from glyphs to images or back, which needs other elements
                this.StampOutRatingItems();
                return;
            }

            this.CustomizeStackPanel(this.backgroundStackPanel, RatingControlStates.Unset);
            this.CustomizeStackPanel(this.foregroundStackPanel, RatingControlStates.Set);
            this.UpdateRatingItemsAppearance();
        }

        private void UpdateRatingItemsAppearance()
        {
            var foreground = this.foregroundStackPanel;
            if (foreground is null)
            {
                return;
            }

            var placeholderValue = this.PlaceholderValue;
            var ratingValue = this.Value;
            var value = 0.0;

            if (this.isPointerOver)
            {
                value = Math.Ceiling(this.mousePercentage * this.MaxRating);
                if (ratingValue == NoValueSetSentinel)
                {
                    if (placeholderValue == NoValueSetSentinel)
                    {
                        this.GoToState("PointerOverPlaceholder", PointerOverPlaceholderBrushProperty, RatingControlStates.PointerOverPlaceholder);
                    }
                    else
                    {
                        // WinUI can't change the glyph of this one any more, its API is locked
                        this.GoToState("PointerOverUnselected", PointerOverUnselectedBrushProperty, RatingControlStates.PointerOverPlaceholder);
                    }
                }
                else
                {
                    this.GoToState("PointerOverSet", PointerOverSelectedBrushProperty, RatingControlStates.PointerOverSet);
                }
            }
            else if (ratingValue > NoValueSetSentinel)
            {
                value = ratingValue;
                this.GoToState("Set", SelectedBrushProperty, RatingControlStates.Set);
            }
            else if (placeholderValue > NoValueSetSentinel)
            {
                value = placeholderValue;
                this.GoToState("Placeholder", PlaceholderBrushProperty, RatingControlStates.Placeholder);
            }

            // there is no unset state, the foreground items are simply clipped away

            if (!this.IsEnabled)
            {
                this.GoToState("Disabled", DisabledSelectedBrushProperty, RatingControlStates.Disabled);
            }

            var size = this.fontSizeForRendering;
            var i = 0;
            foreach (UIElement item in foreground.Children)
            {
                var width = size;
                if (i + 1 > value)
                {
                    // a partial item, or an empty one
                    width = i < value ? width * (value - Math.Floor(value)) : 0.0;
                }

                item.Clip = new RectangleGeometry(new Rect(0, 0, width, size));
                i++;
            }
        }

        private void GoToState(string stateName, DependencyProperty brushProperty, RatingControlStates itemState)
        {
            // a visual state of WPF can't carry the brush, a resource reference in its storyboard does not resolve
            VisualStateManager.GoToState(this, stateName, false);
            this.stateBrushProperty = brushProperty;
            this.ApplyStateBrush();

            if (this.foregroundStackPanel is not null)
            {
                this.CustomizeStackPanel(this.foregroundStackPanel, itemState);
            }
        }

        private void ApplyStateBrush()
        {
            if (this.foregroundStackPanel is null || this.stateBrushProperty is null)
            {
                return;
            }

            // without a brush for the state the items keep the Foreground of the control
            if (this.GetValue(this.stateBrushProperty) is Brush brush)
            {
                this.foregroundStackPanel.SetValue(TextElement.ForegroundProperty, brush);
            }
            else
            {
                this.foregroundStackPanel.ClearValue(TextElement.ForegroundProperty);
            }
        }

        private void CustomizeStackPanel(StackPanel stackPanel, RatingControlStates state)
        {
            foreach (UIElement item in stackPanel.Children)
            {
                this.CustomizeRatingItem(item, state);
            }
        }

        private void CustomizeRatingItem(UIElement item, RatingControlStates state)
        {
            // the glyphs take the FontFamily of the control through inheritance
            if (item is TextBlock textBlock)
            {
                textBlock.Text = this.GetAppropriateGlyph(state);
            }
            else if (item is Image image)
            {
                image.Source = this.GetAppropriateImageSource(state);
            }
        }

        private string? GetAppropriateGlyph(RatingControlStates state)
        {
            if (this.EffectiveItemInfo is not RatingItemFontInfo info)
            {
                return null;
            }

            return state switch
            {
                RatingControlStates.Disabled => this.GetNextGlyphIfEmpty(info.DisabledGlyph, RatingControlStates.Set),
                RatingControlStates.PointerOverSet => this.GetNextGlyphIfEmpty(info.PointerOverGlyph, RatingControlStates.Set),
                RatingControlStates.PointerOverPlaceholder => this.GetNextGlyphIfEmpty(info.PointerOverPlaceholderGlyph, RatingControlStates.Placeholder),
                RatingControlStates.Placeholder => this.GetNextGlyphIfEmpty(info.PlaceholderGlyph, RatingControlStates.Set),
                RatingControlStates.Unset => this.GetNextGlyphIfEmpty(info.UnsetGlyph, RatingControlStates.Set),
                _ => info.Glyph
            };
        }

        private string? GetNextGlyphIfEmpty(string? glyph, RatingControlStates fallbackState)
        {
            return string.IsNullOrEmpty(glyph) ? this.GetAppropriateGlyph(fallbackState) : glyph;
        }

        private ImageSource? GetAppropriateImageSource(RatingControlStates state)
        {
            if (this.EffectiveItemInfo is not RatingItemImageInfo info)
            {
                return null;
            }

            return state switch
            {
                RatingControlStates.Disabled => info.DisabledImage ?? this.GetAppropriateImageSource(RatingControlStates.Set),
                RatingControlStates.PointerOverSet => info.PointerOverImage ?? this.GetAppropriateImageSource(RatingControlStates.Set),
                RatingControlStates.PointerOverPlaceholder => info.PointerOverPlaceholderImage ?? this.GetAppropriateImageSource(RatingControlStates.Placeholder),
                RatingControlStates.Placeholder => info.PlaceholderImage ?? this.GetAppropriateImageSource(RatingControlStates.Set),
                RatingControlStates.Unset => info.UnsetImage ?? this.GetAppropriateImageSource(RatingControlStates.Set),
                _ => info.Image
            };
        }

        private double CalculateActualRatingWidth()
        {
            // max rating * item size + (max rating - 1) * item spacing
            var maxRating = (double)this.MaxRating;
            return (maxRating * this.fontSizeForRendering * ItemScale) + ((maxRating - 1.0) * this.itemSpacing);
        }

        private double CalculateStarCenter(int starIndex)
        {
            // item size * (star index + 0.5) + star index * item spacing
            return (this.fontSizeForRendering * ItemScale * (starIndex + 0.5)) + (starIndex * this.itemSpacing);
        }

        private double GetPointerX(MouseEventArgs e, StackPanel stackPanel)
        {
            // the position is taken in the panel, which WPF mirrors for right to left, and measured from the left
            // edge of the first item as it is drawn
            var firstItemOffset = 0.0;
            if (stackPanel.Children.Count > 0 && stackPanel.Children[0] is FrameworkElement firstItem && stackPanel.IsAncestorOf(firstItem))
            {
                firstItemOffset = firstItem.TransformToAncestor(stackPanel).TransformBounds(new Rect(firstItem.RenderSize)).Left;
            }

            return this.pointerPosition(e, stackPanel).X - firstItemOffset;
        }

        private double GetMousePercentage(MouseEventArgs e, StackPanel stackPanel)
        {
            return this.GetPointerX(e, stackPanel) / this.CalculateActualRatingWidth();
        }

        private void ScaleRatingItems(double? pointerX, double pointerScale)
        {
            // only a look that asks for it lets the stars grow, the way the stars of Windows 10 do, but shrinking them
            // back always happens, since the look may have changed while they were large
            if (pointerX is not null && this.TryFindResource(ScaleOnPointerOverKey) is not true)
            {
                return;
            }

            foreach (var stackPanel in new[] { this.backgroundStackPanel, this.foregroundStackPanel })
            {
                if (stackPanel is null)
                {
                    continue;
                }

                for (var i = 0; i < stackPanel.Children.Count; i++)
                {
                    if (stackPanel.Children[i].RenderTransform is not ScaleTransform scaleTransform)
                    {
                        continue;
                    }

                    var scale = ItemScale;
                    if (pointerX is { } x)
                    {
                        var distance = this.CalculateStarCenter(i) - x;
                        scale = Math.Max((-0.0005 * pointerScale * distance * distance) + pointerScale, ItemScale);
                    }

                    scaleTransform.ScaleX = scale;
                    scaleTransform.ScaleY = scale;
                }
            }
        }

        private void OnBackgroundStackPanelMouseEnter(object sender, MouseEventArgs e)
        {
            if (!this.IsReadOnly)
            {
                this.isPointerOver = true;
                e.Handled = true;
            }
        }

        private void OnBackgroundStackPanelMouseMove(object sender, MouseEventArgs e)
        {
            if (!this.IsReadOnly && this.backgroundStackPanel is not null)
            {
                var x = this.GetPointerX(e, this.backgroundStackPanel);
                this.ScaleRatingItems(x, e.StylusDevice is not null ? TouchOverScale : MouseOverScale);

                this.mousePercentage = x / this.CalculateActualRatingWidth();
                this.UpdateRatingItemsAppearance();
                e.Handled = true;
            }
        }

        private void OnBackgroundStackPanelMouseLeave(object sender, MouseEventArgs e)
        {
            this.PointerExited(e, true);
        }

        private void OnBackgroundStackPanelLostMouseCapture(object sender, MouseEventArgs e)
        {
            // the capture is only there for dragging off the left side to clear, so losing it after a click
            // leaves the items under the pointer as they are
            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                // someone else took the capture, so no button up comes here to end the press
                this.isPointerDown = false;
            }

            this.PointerExited(e, false);
        }

        private void PointerExited(MouseEventArgs e, bool resetPointerOver)
        {
            if (resetPointerOver)
            {
                this.isPointerOver = false;
            }

            if (!this.isPointerDown)
            {
                if (this.backgroundStackPanel is { IsMouseCaptured: true })
                {
                    this.backgroundStackPanel.ReleaseMouseCapture();
                }

                // losing the capture after a click leaves the stars under the pointer as large as they are
                if (resetPointerOver)
                {
                    this.ScaleRatingItems(null, MouseOverScale);
                }

                this.UpdateRatingItemsAppearance();
            }

            e.Handled = true;
        }

        private void OnBackgroundStackPanelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsReadOnly && this.backgroundStackPanel is not null)
            {
                this.isPointerDown = true;
                this.Focus();

                // the capture keeps the moves coming when the pointer is dragged off the left side to clear
                this.backgroundStackPanel.CaptureMouse();
            }
        }

        private void OnBackgroundStackPanelMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsReadOnly && this.backgroundStackPanel is not null)
            {
                var mousePercentage = this.GetMousePercentage(e, this.backgroundStackPanel);
                this.SetRatingTo(Math.Ceiling(mousePercentage * this.MaxRating), true);
            }

            if (this.isPointerDown)
            {
                this.isPointerDown = false;

                if (this.backgroundStackPanel is { IsMouseCaptured: true })
                {
                    this.backgroundStackPanel.ReleaseMouseCapture();
                }

                this.UpdateRatingItemsAppearance();
            }
        }

        private void ChangeRatingBy(double change, bool originatedFromMouse)
        {
            if (change == 0.0)
            {
                return;
            }

            double ratingValue;
            var current = this.Value;
            if (current != NoValueSetSentinel)
            {
                // a fraction set from code is dropped before the change is applied
                if (Math.Truncate(current) != current)
                {
                    ratingValue = change == -1 ? Math.Truncate(current) : Math.Truncate(current) + change;
                }
                else
                {
                    ratingValue = current + change;
                }
            }
            else
            {
                ratingValue = this.InitialSetValue;
            }

            this.SetRatingTo(ratingValue, originatedFromMouse);
        }

        private void SetRatingTo(double newRating, bool originatedFromMouse)
        {
            var oldRatingValue = this.Value;
            var maxRating = this.MaxRating;

            var ratingValue = Math.Min(newRating, maxRating);
            ratingValue = Math.Max(ratingValue, 0.0);

            // nothing happens when there is no rating and the input asks for none
            if (oldRatingValue <= NoValueSetSentinel && ratingValue == 0.0)
            {
                return;
            }

            if (!this.IsClearEnabled && ratingValue <= 0.0)
            {
                this.SetCurrentValue(ValueProperty, 1.0);
            }
            else if (ratingValue == oldRatingValue && this.IsClearEnabled && (ratingValue != maxRating || originatedFromMouse))
            {
                // raising the rating by key at the maximum keeps it, but a click on the star of the current rating clears it
                this.SetCurrentValue(ValueProperty, NoValueSetSentinel);
            }
            else if (ratingValue > 0.0)
            {
                this.SetCurrentValue(ValueProperty, ratingValue);
            }
            else
            {
                this.SetCurrentValue(ValueProperty, NoValueSetSentinel);
            }

            this.RaiseEvent(new RoutedPropertyChangedEventArgs<double>(oldRatingValue, this.Value, ValueChangedEvent));
        }

        /// <inheritdoc />
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!e.Handled && !this.IsReadOnly)
            {
                var key = e.Key;
                var flowDirectionReverser = this.FlowDirection == FlowDirection.RightToLeft ? -1.0 : 1.0;

                // up and down are right and left, and not mirrored
                if (key == Key.Up)
                {
                    key = Key.Right;
                    flowDirectionReverser = 1.0;
                }
                else if (key == Key.Down)
                {
                    key = Key.Left;
                    flowDirectionReverser = 1.0;
                }

                var handled = true;
                switch (key)
                {
                    case Key.Left:
                        this.ChangeRatingBy(-1.0 * flowDirectionReverser, false);
                        break;
                    case Key.Right:
                        this.ChangeRatingBy(1.0 * flowDirectionReverser, false);
                        break;
                    case Key.Home:
                        this.SetRatingTo(0.0, false);
                        break;
                    case Key.End:
                        this.SetRatingTo(this.MaxRating, false);
                        break;
                    default:
                        handled = false;
                        break;
                }

                e.Handled = handled;
            }

            base.OnKeyDown(e);
        }
    }
}
