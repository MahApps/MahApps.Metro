// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    ///     Represents a container with two views; one view for the main content and another view that is typically used for
    ///     navigation commands.
    /// </summary>
    [TemplatePart(Name = "PaneClipRectangle", Type = typeof(RectangleGeometry))]
    [TemplatePart(Name = "LightDismissLayer", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_ResizingThumb", Type = typeof(MetroThumb))]
    [TemplateVisualState(Name = "Closed                 ", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "ClosedCompactLeft      ", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "ClosedCompactRight     ", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "OpenOverlayLeft        ", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "OpenOverlayRight       ", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "OpenInlineLeft         ", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "OpenInlineRight        ", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "OpenCompactOverlayLeft ", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "OpenCompactOverlayRight", GroupName = "DisplayModeStates")]
    [ContentProperty(nameof(Content))]
    [StyleTypedProperty(Property = nameof(ResizeThumbStyle), StyleTargetType = typeof(MetroThumb))]
    public class SplitView : Control
    {
        private Rectangle lightDismissLayer;
        private RectangleGeometry paneClipRectangle;
        private MetroThumb resizingThumb;

        /// <summary>Identifies the <see cref="CompactPaneLength"/> dependency property.</summary>
        public static readonly DependencyProperty CompactPaneLengthProperty
            = DependencyProperty.Register(nameof(CompactPaneLength),
                                          typeof(double),
                                          typeof(SplitView),
                                          new PropertyMetadata(0d, OnCompactPaneLengthPropertyChangedCallback));

        private static void OnCompactPaneLengthPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is double && dependencyObject is SplitView splitView)
            {
                splitView.CoerceValue(OpenPaneLengthProperty);
                splitView.TemplateSettings?.Update();
                splitView.ChangeVisualState(true, true);
            }
        }

        /// <summary>
        ///     Gets or sets the width of the <see cref="SplitView" /> pane in its compact display mode.
        /// </summary>
        /// <returns>
        ///     The width of the pane in it's compact display mode. The default is 48 device-independent pixel (DIP) (defined
        ///     by the SplitViewCompactPaneThemeLength resource).
        /// </returns>
        public double CompactPaneLength
        {
            get => (double)this.GetValue(CompactPaneLengthProperty);
            set => this.SetValue(CompactPaneLengthProperty, value);
        }

        /// <summary>Identifies the <see cref="Content"/> dependency property.</summary>
        public static readonly DependencyProperty ContentProperty
            = DependencyProperty.Register(nameof(Content),
                                          typeof(UIElement),
                                          typeof(SplitView),
                                          new PropertyMetadata(null));

        /// <summary>
        ///     Gets or sets the contents of the main panel of a <see cref="SplitView" />.
        /// </summary>
        /// <returns>The contents of the main panel of a <see cref="SplitView" />. The default is null.</returns>
        public UIElement Content
        {
            get => (UIElement)this.GetValue(ContentProperty);
            set => this.SetValue(ContentProperty, value);
        }

        /// <summary>Identifies the <see cref="DisplayMode"/> dependency property.</summary>
        public static readonly DependencyProperty DisplayModeProperty
            = DependencyProperty.Register(nameof(DisplayMode),
                                          typeof(SplitViewDisplayMode),
                                          typeof(SplitView),
                                          new PropertyMetadata(SplitViewDisplayMode.Overlay, OnDisplayModePropertyChangedCallback));

        private static void OnDisplayModePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is SplitViewDisplayMode && dependencyObject is SplitView splitView)
            {
                splitView.CoerceValue(OpenPaneLengthProperty);
                splitView.ChangeVisualState(true, false);
            }
        }

        /// <summary>
        ///     Gets of sets a value that specifies how the pane and content areas of a <see cref="SplitView" /> are shown.
        /// </summary>
        /// <returns>
        ///     A value of the enumeration that specifies how the pane and content areas of a <see cref="SplitView" /> are
        ///     shown. The default is <see cref="SplitViewDisplayMode.Overlay" />.
        /// </returns>
        public SplitViewDisplayMode DisplayMode
        {
            get => (SplitViewDisplayMode)this.GetValue(DisplayModeProperty);
            set => this.SetValue(DisplayModeProperty, value);
        }

        /// <summary>Identifies the <see cref="IsPaneOpen"/> dependency property.</summary>
        public static readonly DependencyProperty IsPaneOpenProperty
            = DependencyProperty.Register(nameof(IsPaneOpen),
                                          typeof(bool),
                                          typeof(SplitView),
                                          new PropertyMetadata(BooleanBoxes.FalseBox, OnIsPaneOpenChanged));

        private static void OnIsPaneOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as SplitView;
            var newValue = (bool)e.NewValue;
            var oldValue = (bool)e.OldValue;

            if (sender == null
                || newValue == oldValue)
            {
                return;
            }

            if (newValue)
            {
                sender.ChangeVisualState(true, false); // Open pane
            }
            else
            {
                sender.OnIsPaneOpenChanged(); // Close pane
            }
        }

        /// <summary>
        ///     Gets or sets a value that specifies whether the <see cref="SplitView" /> pane is expanded to its full width.
        /// </summary>
        /// <returns>true if the pane is expanded to its full width; otherwise, false. The default is true.</returns>
        public bool IsPaneOpen
        {
            get => (bool)this.GetValue(IsPaneOpenProperty);
            set => this.SetValue(IsPaneOpenProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="OverlayBrush"/> dependency property.</summary>
        public static readonly DependencyProperty OverlayBrushProperty
            = DependencyProperty.Register(nameof(OverlayBrush),
                                          typeof(Brush),
                                          typeof(SplitView),
                                          new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Gets or sets a value that specifies the OverlayBrush 
        /// </summary>
        /// <returns>The current OverlayBrush</returns>
        public Brush OverlayBrush
        {
            get => (Brush)this.GetValue(OverlayBrushProperty);
            set => this.SetValue(OverlayBrushProperty, value);
        }

        /// <summary>Identifies the <see cref="OpenPaneLength"/> dependency property.</summary>
        public static readonly DependencyProperty OpenPaneLengthProperty
            = DependencyProperty.Register(nameof(OpenPaneLength),
                                          typeof(double),
                                          typeof(SplitView),
                                          new PropertyMetadata(0d, OnOpenPaneLengthPropertyChangedCallback, OnOpenPaneLengthCoerceValueCallback));

        private static object OnOpenPaneLengthCoerceValueCallback(DependencyObject dependencyObject, object inputValue)
        {
            if (dependencyObject is SplitView splitView && splitView.ActualWidth > 0 && inputValue is double openPaneLength)
            {
                // Get the minimum needed width
                var minWidth = splitView.DisplayMode == SplitViewDisplayMode.CompactInline || splitView.DisplayMode == SplitViewDisplayMode.CompactOverlay
                    ? Math.Max(splitView.CompactPaneLength, splitView.MinimumOpenPaneLength)
                    : Math.Max(0, splitView.MinimumOpenPaneLength);

                if (minWidth < 0)
                {
                    minWidth = 0;
                }

                // Get the maximum allowed width
                var maxWidth = Math.Min(splitView.ActualWidth, splitView.MaximumOpenPaneLength);

                // Check if max < min
                if (maxWidth < minWidth)
                {
                    minWidth = maxWidth;
                }

                // Check is OpenPaneLength is valid
                if (openPaneLength < minWidth)
                {
                    return minWidth;
                }

                if (openPaneLength > maxWidth)
                {
                    return maxWidth;
                }

                return openPaneLength;
            }
            else
            {
                return inputValue;
            }
        }

        private static void OnOpenPaneLengthPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && dependencyObject is SplitView splitView)
            {
                splitView.TemplateSettings?.Update();
                splitView.ChangeVisualState(true, true);
            }
        }

        /// <summary>
        ///     Gets or sets the width of the <see cref="SplitView" /> pane when it's fully expanded.
        /// </summary>
        /// <returns>
        ///     The width of the <see cref="SplitView" /> pane when it's fully expanded. The default is 320 device-independent
        ///     pixel (DIP).
        /// </returns>
        public double OpenPaneLength
        {
            get => (double)this.GetValue(OpenPaneLengthProperty);
            set => this.SetValue(OpenPaneLengthProperty, value);
        }

        /// <summary>Identifies the <see cref="MinimumOpenPaneLength"/> dependency property.</summary>
        public static readonly DependencyProperty MinimumOpenPaneLengthProperty = DependencyProperty.Register(nameof(MinimumOpenPaneLength), typeof(double), typeof(SplitView), new PropertyMetadata(100d, MinimumOpenPaneLengthPropertyChangedCallback));

        private static void MinimumOpenPaneLengthPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is double && dependencyObject is SplitView splitView)
            {
                splitView.CoerceValue(OpenPaneLengthProperty);
                splitView.TemplateSettings?.Update();
                splitView.ChangeVisualState(true, true);
            }
        }

        /// <summary>
        ///     Gets or sets the minimum width of the <see cref="SplitView" /> pane when it's fully expanded.
        /// </summary>
        /// <returns>
        ///     The minimum width of the <see cref="SplitView" /> pane when it's fully expanded. The default is 320 device-independent
        ///     pixel (DIP).
        /// </returns>
        public double MinimumOpenPaneLength
        {
            get => (double)this.GetValue(MinimumOpenPaneLengthProperty);
            set => this.SetValue(MinimumOpenPaneLengthProperty, value);
        }

        /// <summary>Identifies the <see cref="MaximumOpenPaneLength"/> dependency property.</summary>
        public static readonly DependencyProperty MaximumOpenPaneLengthProperty = DependencyProperty.Register(nameof(MaximumOpenPaneLength), typeof(double), typeof(SplitView), new PropertyMetadata(500d, OnMaximumOpenPaneLengthPropertyChangedCallback));

        private static void OnMaximumOpenPaneLengthPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is double && dependencyObject is SplitView splitView)
            {
                splitView.CoerceValue(OpenPaneLengthProperty);
                splitView.TemplateSettings?.Update();
                splitView.ChangeVisualState(true, true);
            }
        }

        /// <summary>
        ///     Gets or sets the maximum width of the <see cref="SplitView" /> pane when it's fully expanded.
        /// </summary>
        /// <returns>
        ///     The maximum width of the <see cref="SplitView" /> pane when it's fully expanded. The default is 320 device-independent
        ///     pixel (DIP).
        /// </returns>
        public double MaximumOpenPaneLength
        {
            get => (double)this.GetValue(MaximumOpenPaneLengthProperty);
            set => this.SetValue(MaximumOpenPaneLengthProperty, value);
        }

        /// <summary>Identifies the <see cref="CanResizeOpenPane"/> dependency property.</summary>
        public static readonly DependencyProperty CanResizeOpenPaneProperty = DependencyProperty.Register(nameof(CanResizeOpenPane), typeof(bool), typeof(SplitView), new PropertyMetadata(BooleanBoxes.FalseBox, OnCanResizeOpenPanePropertyChangedCallback));

        private static void OnCanResizeOpenPanePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is bool && dependencyObject is SplitView splitView)
            {
                splitView.CoerceValue(OpenPaneLengthProperty);
            }
        }

        /// <summary>
        /// Gets or Sets if the open pane can be resized by the user. The default value is false.
        /// </summary>
        public bool CanResizeOpenPane
        {
            get => (bool)this.GetValue(CanResizeOpenPaneProperty);
            set => this.SetValue(CanResizeOpenPaneProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ResizeThumbStyle"/> dependency property.</summary>
        public static readonly DependencyProperty ResizeThumbStyleProperty = DependencyProperty.Register(nameof(ResizeThumbStyle), typeof(Style), typeof(SplitView), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the <see cref="Style"/> for the resizing Thumb (type of <see cref="MetroThumb"/>)
        /// </summary>
        public Style ResizeThumbStyle
        {
            get => (Style)this.GetValue(ResizeThumbStyleProperty);
            set => this.SetValue(ResizeThumbStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="Pane"/> dependency property.</summary>
        public static readonly DependencyProperty PaneProperty
            = DependencyProperty.Register(nameof(Pane),
                                          typeof(UIElement),
                                          typeof(SplitView),
                                          new PropertyMetadata(null, UpdateLogicalChild));

        /// <summary>
        ///     Gets or sets the contents of the pane of a <see cref="SplitView" />.
        /// </summary>
        /// <returns>The contents of the pane of a <see cref="SplitView" />. The default is null.</returns>
        public UIElement Pane
        {
            get => (UIElement)this.GetValue(PaneProperty);
            set => this.SetValue(PaneProperty, value);
        }

        /// <summary>Identifies the <see cref="PaneBackground"/> dependency property.</summary>
        public static readonly DependencyProperty PaneBackgroundProperty
            = DependencyProperty.Register(nameof(PaneBackground),
                                          typeof(Brush),
                                          typeof(SplitView),
                                          new PropertyMetadata(null));

        /// <summary>
        ///     Gets or sets the Brush to apply to the background of the <see cref="Pane" /> area of the control.
        /// </summary>
        /// <returns>The Brush to apply to the background of the <see cref="Pane" /> area of the control.</returns>
        public Brush PaneBackground
        {
            get => (Brush)this.GetValue(PaneBackgroundProperty);
            set => this.SetValue(PaneBackgroundProperty, value);
        }

        /// <summary>Identifies the <see cref="PaneForeground"/> dependency property.</summary>
        public static readonly DependencyProperty PaneForegroundProperty
            = DependencyProperty.Register(nameof(PaneForeground),
                                          typeof(Brush),
                                          typeof(SplitView),
                                          new PropertyMetadata(null));

        /// <summary>
        ///     Gets or sets the Brush to apply to the foreground of the <see cref="Pane" /> area of the control.
        /// </summary>
        /// <returns>The Brush to apply to the background of the <see cref="Pane" /> area of the control.</returns>
        public Brush PaneForeground
        {
            get => (Brush)this.GetValue(PaneForegroundProperty);
            set => this.SetValue(PaneForegroundProperty, value);
        }

        /// <summary>Identifies the <see cref="PanePlacement"/> dependency property.</summary>
        public static readonly DependencyProperty PanePlacementProperty
            = DependencyProperty.Register(nameof(PanePlacement),
                                          typeof(SplitViewPanePlacement),
                                          typeof(SplitView),
                                          new PropertyMetadata(SplitViewPanePlacement.Left, OnPanePlacementPropertyChangedCallback));

        private static void OnPanePlacementPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is SplitViewPanePlacement && dependencyObject is SplitView splitView)
            {
                splitView.CoerceValue(OpenPaneLengthProperty);
                splitView.ChangeVisualState(true, false);
            }
        }

        /// <summary>
        ///     Gets or sets a value that specifies whether the pane is shown on the right or left side of the
        ///     <see cref="SplitView" />.
        /// </summary>
        /// <returns>
        ///     A value of the enumeration that specifies whether the pane is shown on the right or left side of the
        ///     <see cref="SplitView" />. The default is <see cref="SplitViewPanePlacement.Left" />.
        /// </returns>
        public SplitViewPanePlacement PanePlacement
        {
            get => (SplitViewPanePlacement)this.GetValue(PanePlacementProperty);
            set => this.SetValue(PanePlacementProperty, value);
        }

        /// <summary>Identifies the <see cref="TemplateSettings"/> dependency property.</summary>
        public static readonly DependencyProperty TemplateSettingsProperty
            = DependencyProperty.Register(nameof(TemplateSettings),
                                          typeof(SplitViewTemplateSettings),
                                          typeof(SplitView),
                                          new PropertyMetadata(null));

        /// <summary>
        ///     Gets an object that provides calculated values that can be referenced as TemplateBinding sources when defining
        ///     templates for a <see cref="SplitView" /> control.
        /// </summary>
        /// <returns>An object that provides calculated values for templates.</returns>
        public SplitViewTemplateSettings TemplateSettings
        {
            get => (SplitViewTemplateSettings)this.GetValue(TemplateSettingsProperty);
            private set => this.SetValue(TemplateSettingsProperty, value);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SplitView" /> class.
        /// </summary>
        public SplitView()
        {
            this.DefaultStyleKey = typeof(SplitView);
            this.TemplateSettings = new SplitViewTemplateSettings(this);
            this.DataContextChanged += this.SplitViewDataContextChanged;
        }

        private void SplitViewDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // MahApps add this pane to the SplitView with AddLogicalChild method.
            // This has the side effect that the DataContext doesn't update, so do this now here.
            if (this.Pane is FrameworkElement elementPane)
            {
                elementPane.DataContext = this.DataContext;
            }
        }

        /// <summary>
        ///     Occurs when the <see cref="SplitView" /> pane is closed.
        /// </summary>
        public event EventHandler PaneClosed;

        /// <summary>
        ///     Occurs when the <see cref="SplitView" /> pane is closing.
        /// </summary>
        public event EventHandler<SplitViewPaneClosingEventArgs> PaneClosing;

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            if (this.lightDismissLayer != null)
            {
                this.lightDismissLayer.MouseDown -= this.OnLightDismiss;
            }

            if (this.resizingThumb != null)
            {
                this.resizingThumb.DragDelta -= this.ResizingThumb_DragDelta;
            }

            base.OnApplyTemplate();

            this.paneClipRectangle = this.GetTemplateChild("PaneClipRectangle") as RectangleGeometry;

            this.lightDismissLayer = this.GetTemplateChild("LightDismissLayer") as Rectangle;
            if (this.lightDismissLayer != null)
            {
                this.lightDismissLayer.MouseDown += this.OnLightDismiss;
            }

            this.resizingThumb = this.GetTemplateChild("PART_ResizingThumb") as MetroThumb;
            if (this.resizingThumb != null)
            {
                this.resizingThumb.DragDelta += this.ResizingThumb_DragDelta;
            }

            this.ExecuteWhenLoaded(() =>
                {
                    this.TemplateSettings.Update();
                    this.ChangeVisualState(false, false);
                });
        }

        private void ResizingThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            this.SetCurrentValue(OpenPaneLengthProperty, this.PanePlacement == SplitViewPanePlacement.Left ? this.OpenPaneLength + e.HorizontalChange : this.OpenPaneLength - e.HorizontalChange);
        }

        private static void UpdateLogicalChild(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (!(dependencyObject is SplitView splitView))
            {
                return;
            }

            if (e.OldValue is FrameworkElement oldChild)
            {
                splitView.RemoveLogicalChild(oldChild);
            }

            if (e.NewValue is FrameworkElement newChild)
            {
                splitView.AddLogicalChild(newChild);
                newChild.DataContext = splitView.DataContext;
            }
        }

        /// <inheritdoc />
        protected override IEnumerator LogicalChildren
        {
            get
            {
                // cheat, make a list with all logical content and return the enumerator
                ArrayList children = new ArrayList();
                if (this.Pane != null)
                {
                    children.Add(this.Pane);
                }

                if (this.Content != null)
                {
                    children.Add(this.Content);
                }

                return children.GetEnumerator();
            }
        }

        /// <inheritdoc />
        protected override void OnRenderSizeChanged(SizeChangedInfo info)
        {
            base.OnRenderSizeChanged(info);

            if (this.IsPaneOpen)
            {
                this.CoerceValue(OpenPaneLengthProperty);
            }

            if (this.paneClipRectangle != null)
            {
                this.paneClipRectangle.Rect = new Rect(0, 0, this.OpenPaneLength, this.ActualHeight);
            }
        }

        protected virtual void ChangeVisualState(bool animated, bool reset)
        {
            if (this.paneClipRectangle != null)
            {
                this.paneClipRectangle.Rect = new Rect(0, 0, this.OpenPaneLength, this.ActualHeight); // We could also use ActualHeight and subscribe to the SizeChanged property
            }

            var state = string.Empty;
            if (this.IsPaneOpen)
            {
                state += "Open";
                switch (this.DisplayMode)
                {
                    case SplitViewDisplayMode.CompactInline:
                        state += "Inline";
                        break;
                    default:
                        state += this.DisplayMode.ToString();
                        break;
                }

                state += this.PanePlacement.ToString();
            }
            else
            {
                state += "Closed";
                if (this.DisplayMode == SplitViewDisplayMode.CompactInline
                    || this.DisplayMode == SplitViewDisplayMode.CompactOverlay)
                {
                    state += "Compact";
                    state += this.PanePlacement.ToString();
                }
            }

            if (reset)
            {
                VisualStateManager.GoToState(this, "None", animated);
            }

            VisualStateManager.GoToState(this, state, animated);
        }

        protected void OnIsPaneOpenChanged()
        {
            var cancel = false;
            if (this.PaneClosing != null)
            {
                var args = new SplitViewPaneClosingEventArgs();
                foreach (var paneClosingDelegates in this.PaneClosing.GetInvocationList())
                {
                    var eventHandler = paneClosingDelegates as EventHandler<SplitViewPaneClosingEventArgs>;
                    if (eventHandler == null)
                    {
                        continue;
                    }

                    eventHandler(this, args);
                    if (args.Cancel)
                    {
                        cancel = true;
                        break;
                    }
                }
            }

            if (!cancel)
            {
                this.ChangeVisualState(true, false);
                this.PaneClosed?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                this.SetCurrentValue(IsPaneOpenProperty, BooleanBoxes.FalseBox);
            }
        }

        private void OnLightDismiss(object sender, MouseButtonEventArgs e)
        {
            this.SetCurrentValue(IsPaneOpenProperty, BooleanBoxes.FalseBox);
        }
    }
}