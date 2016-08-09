namespace MahApps.Metro.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using MahApps.Metro.Behaviours;

    /// <summary>
    ///     Represents a container with two views; one view for the main content and another view that is typically used for
    ///     navigation commands.
    /// </summary>
    [TemplatePart(Name = "PART_PaneClipRectangle", Type = typeof(RectangleGeometry))]
    [TemplatePart(Name = "PART_LightDismissLayer", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_ClosedToOpenOverlayLeftPaneTransformXAnimation                                 ", Type = typeof(DiscreteDoubleKeyFrame))] // HACK
    [TemplatePart(Name = "PART_ClosedToOpenOverlayLeftPaneClipRectangleTransformXAnimation                    ", Type = typeof(DiscreteDoubleKeyFrame))] // HACK
    [TemplatePart(Name = "PART_ClosedToOpenOverlayRightPaneTransformXAnimation                                ", Type = typeof(DiscreteDoubleKeyFrame))] // HACK
    [TemplatePart(Name = "PART_ClosedToOpenOverlayRightPaneClipRectangleTransformXAnimation                   ", Type = typeof(DiscreteDoubleKeyFrame))] // HACK
    [TemplatePart(Name = "PART_ClosedCompactLeftToOpenCompactOverlayLeftColumnDefinition1WidthAnimation       ", Type = typeof(GridLengthAnimation))] // HACK
    [TemplatePart(Name = "PART_ClosedCompactLeftToOpenCompactOverlayLeftPaneClipRectangleTransformXAnimation  ", Type = typeof(DiscreteDoubleKeyFrame))] // HACK
    [TemplatePart(Name = "PART_ClosedCompactRightToOpenCompactOverlayRightColumnDefinition2WidthAnimation     ", Type = typeof(GridLengthAnimation))] // HACK
    [TemplatePart(Name = "PART_ClosedCompactRightToOpenCompactOverlayRightPaneClipRectangleTransformXAnimation", Type = typeof(DiscreteDoubleKeyFrame))] // HACK
    [TemplatePart(Name = "PART_OpenOverlayLeftToClosedPanePaneTransformXAnimation                             ", Type = typeof(SplineDoubleKeyFrame))] // HACK
    [TemplatePart(Name = "PART_OpenOverlayLeftToClosedPaneClipRectangleTransformXAnimation                    ", Type = typeof(SplineDoubleKeyFrame))] // HACK
    [TemplatePart(Name = "PART_OpenOverlayRightToClosedPanePaneTransformXAnimation                            ", Type = typeof(SplineDoubleKeyFrame))] // HACK
    [TemplatePart(Name = "PART_OpenOverlayRightToClosedPaneClipRectangleTransformXAnimation                   ", Type = typeof(SplineDoubleKeyFrame))] // HACK
    [TemplatePart(Name = "PART_OpenCompactOverlayRightToClosedCompactRightColumnDefinition2WidthAnimation     ", Type = typeof(GridLengthAnimation))] // HACK
    [TemplatePart(Name = "PART_OpenCompactOverlayLeftToClosedCompactLeftColumnDefinition1Width                ", Type = typeof(GridLengthAnimation))] // HACK
    [TemplatePart(Name = "PART_OpenCompactOverlayLeftToClosedCompactLeftPaneClipRectangleTransformXAnimation  ", Type = typeof(SplineDoubleKeyFrame))] // HACK
    [TemplatePart(Name = "PART_OpenCompactOverlayRightToClosedCompactRightPaneClipRectangleTransformXAnimation", Type = typeof(SplineDoubleKeyFrame))] // HACK
    [TemplatePart(Name = "PART_ClosedCompactLeftColumnDefinition1Width                                        ", Type = typeof(GridLengthAnimation))] // HACK
    [TemplatePart(Name = "PART_ClosedCompactLeftPaneClipRectangleTransformXAnimation                          ", Type = typeof(DoubleAnimation))] // HACK
    [TemplatePart(Name = "PART_ClosedCompactRightColumnDefinition2Width                                       ", Type = typeof(GridLengthAnimation))] // HACK
    [TemplatePart(Name = "PART_ClosedCompactRightPaneClipRectangleTransformXAnimation                         ", Type = typeof(DoubleAnimation))] // HACK
    [TemplatePart(Name = "PART_OpenInlineRightColumnDefinition2Width                                          ", Type = typeof(GridLengthAnimation))] // HACK
    [TemplatePart(Name = "PART_OpenCompactOverlayLeftColumnDefinition1Width                                   ", Type = typeof(GridLengthAnimation))] // HACK
    [TemplatePart(Name = "PART_OpenCompactOverlayRightColumnDefinition2Width                                  ", Type = typeof(GridLengthAnimation))] // HACK
    [TemplateVisualState(Name = "Closed                 ", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "ClosedCompactLeft      ", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "ClosedCompactRight     ", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "OpenOverlayLeft        ", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "OpenOverlayRight       ", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "OpenInlineLeft         ", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "OpenInlineRight        ", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "OpenCompactOverlayLeft ", GroupName = "DisplayModeStates")]
    [TemplateVisualState(Name = "OpenCompactOverlayRight", GroupName = "DisplayModeStates")]
    [ContentProperty("Content")]
    public class SplitView : Control
    {
        /// <summary>
        ///     Identifies the <see cref="CompactPaneLength" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="CompactPaneLength" /> property.</returns>
        public static readonly DependencyProperty CompactPaneLengthProperty =
            DependencyProperty.Register("CompactPaneLength", typeof(double), typeof(SplitView), new PropertyMetadata(0d, OnMetricsChanged));

        private static void OnMetricsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as SplitView;
            sender?.TemplateSettings?.Update();
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
            get { return (double)this.GetValue(CompactPaneLengthProperty); }
            set { this.SetValue(CompactPaneLengthProperty, value); }
        }

        /// <summary>
        ///     Identifies the <see cref="Content" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="Content" /> dependency property.</returns>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(UIElement), typeof(SplitView), new PropertyMetadata(null));

        /// <summary>
        ///     Gets or sets the contents of the main panel of a <see cref="SplitView" />.
        /// </summary>
        /// <returns>The contents of the main panel of a <see cref="SplitView" />. The default is null.</returns>
        public UIElement Content
        {
            get { return (UIElement)this.GetValue(ContentProperty); }
            set { this.SetValue(ContentProperty, value); }
        }

        /// <summary>
        ///     Identifies the <see cref="DisplayMode" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="DisplayMode" /> dependency property.</returns>
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(SplitViewDisplayMode), typeof(SplitView), new PropertyMetadata(SplitViewDisplayMode.Overlay, OnStateChanged));

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as SplitView;
            sender?.ChangeVisualState();
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
            get { return (SplitViewDisplayMode)this.GetValue(DisplayModeProperty); }
            set { this.SetValue(DisplayModeProperty, value); }
        }

        /// <summary>
        ///     Identifies the <see cref="IsPaneOpen" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="IsPaneOpen" /> dependency property.</returns>
        public static readonly DependencyProperty IsPaneOpenProperty =
            DependencyProperty.Register("IsPaneOpen", typeof(bool), typeof(SplitView), new PropertyMetadata(false, OnIsPaneOpenChanged));

        private static void OnIsPaneOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as SplitView;
            var newValue = (bool)e.NewValue;
            var oldValue = (bool)e.OldValue;

            if (sender == null
                || newValue == oldValue)
                return;

            if (newValue)
                sender.ChangeVisualState(); // Open pane
            else
                sender.OnIsPaneOpenChanged(); // Close pane
        }

        /// <summary>
        ///     Gets or sets a value that specifies whether the <see cref="SplitView" /> pane is expanded to its full width.
        /// </summary>
        /// <returns>true if the pane is expanded to its full width; otherwise, false. The default is true.</returns>
        public bool IsPaneOpen
        {
            get { return (bool)this.GetValue(IsPaneOpenProperty); }
            set { this.SetValue(IsPaneOpenProperty, value); }
        }

        /// <summary>
        ///     Identifies the <see cref="OpenPaneLength" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="OpenPaneLength" /> dependency property.</returns>
        public static readonly DependencyProperty OpenPaneLengthProperty =
            DependencyProperty.Register("OpenPaneLength", typeof(double), typeof(SplitView), new PropertyMetadata(0d, OnMetricsChanged));

        /// <summary>
        ///     Gets or sets the width of the <see cref="SplitView" /> pane when it's fully expanded.
        /// </summary>
        /// <returns>
        ///     The width of the <see cref="SplitView" /> pane when it's fully expanded. The default is 320 device-independent
        ///     pixel (DIP).
        /// </returns>
        public double OpenPaneLength
        {
            get { return (double)this.GetValue(OpenPaneLengthProperty); }
            set { this.SetValue(OpenPaneLengthProperty, value); }
        }

        /// <summary>
        ///     Identifies the <see cref="Pane" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="Pane" /> dependency property.</returns>
        public static readonly DependencyProperty PaneProperty =
            DependencyProperty.Register("Pane", typeof(UIElement), typeof(SplitView), new PropertyMetadata(null));

        /// <summary>
        ///     Gets or sets the contents of the pane of a <see cref="SplitView" />.
        /// </summary>
        /// <returns>The contents of the pane of a <see cref="SplitView" />. The default is null.</returns>
        public UIElement Pane
        {
            get { return (UIElement)this.GetValue(PaneProperty); }
            set { this.SetValue(PaneProperty, value); }
        }

        /// <summary>
        ///     Identifies the <see cref="PaneBackground" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="PaneBackground" /> dependency property.</returns>
        public static readonly DependencyProperty PaneBackgroundProperty =
            DependencyProperty.Register("PaneBackground", typeof(Brush), typeof(SplitView), new PropertyMetadata(null));

        /// <summary>
        ///     Gets or sets the Brush to apply to the background of the <see cref="Pane" /> area of the control.
        /// </summary>
        /// <returns>The Brush to apply to the background of the <see cref="Pane" /> area of the control.</returns>
        public Brush PaneBackground
        {
            get { return (Brush)this.GetValue(PaneBackgroundProperty); }
            set { this.SetValue(PaneBackgroundProperty, value); }
        }

        /// <summary>
        ///     Identifies the PanePlacement dependency property.
        /// </summary>
        /// <returns>The identifier for the PanePlacement dependency property.</returns>
        public static readonly DependencyProperty PanePlacementProperty =
            DependencyProperty.Register("PanePlacement", typeof(SplitViewPanePlacement), typeof(SplitView), new PropertyMetadata(SplitViewPanePlacement.Left, OnStateChanged));

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
            get { return (SplitViewPanePlacement)this.GetValue(PanePlacementProperty); }
            set { this.SetValue(PanePlacementProperty, value); }
        }

        /// <summary>
        ///     Identifies the <see cref="TemplateSettings" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="TemplateSettings" /> dependency property.</returns>
        public static readonly DependencyProperty TemplateSettingsProperty =
            DependencyProperty.Register("TemplateSettings", typeof(SplitViewTemplateSettings), typeof(SplitView), new PropertyMetadata(null));

        /// <summary>
        ///     Gets an object that provides calculated values that can be referenced as TemplateBinding sources when defining
        ///     templates for a <see cref="SplitView" /> control.
        /// </summary>
        /// <returns>An object that provides calculated values for templates.</returns>
        public SplitViewTemplateSettings TemplateSettings
        {
            get { return (SplitViewTemplateSettings)this.GetValue(TemplateSettingsProperty); }
            private set { this.SetValue(TemplateSettingsProperty, value); }
        }

        private Rectangle lightDismissLayer;
        private RectangleGeometry paneClipRectangle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SplitView" /> class.
        /// </summary>
        public SplitView()
        {
            this.DefaultStyleKey = typeof(SplitView);
            this.TemplateSettings = new SplitViewTemplateSettings(this);

            this.Loaded += (s, args) =>
                {
                    this.TemplateSettings.Update();
                    this.ChangeVisualState(false);
                };
        }

        /// <summary>
        ///     Occurs when the <see cref="SplitView" /> pane is closed.
        /// </summary>
        public event EventHandler PaneClosed;

        /// <summary>
        ///     Occurs when the <see cref="SplitView" /> pane is closing.
        /// </summary>
        public event EventHandler<SplitViewPaneClosingEventArgs> PaneClosing;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.HackBindingAnimations(); // FIX ME: Remove this hack when it is possible to bind value of animations.

            this.paneClipRectangle = this.GetTemplateChild("PART_PaneClipRectangle") as RectangleGeometry;

            this.lightDismissLayer = this.GetTemplateChild("PART_LightDismissLayer") as Rectangle;
            if (this.lightDismissLayer != null)
            {
                this.lightDismissLayer.MouseDown += this.OnLightDismiss;
            }
        }

        protected virtual void ChangeVisualState(bool animated = true)
        {
            if (this.paneClipRectangle != null)
                this.paneClipRectangle.Rect = new Rect(0, 0, this.OpenPaneLength, short.MaxValue); // We could also use ActualHeight and subscribe to the SizeChanged property

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

            VisualStateManager.GoToState(this, state, animated);
        }

        protected virtual void OnIsPaneOpenChanged()
        {
            var cancel = false;
            if (this.PaneClosing != null)
            {
                var args = new SplitViewPaneClosingEventArgs();
                foreach (var paneClosingDelegates in this.PaneClosing.GetInvocationList())
                {
                    var eventHandler = paneClosingDelegates as EventHandler<SplitViewPaneClosingEventArgs>;
                    if (eventHandler == null)
                        continue;

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
                this.ChangeVisualState();
                this.PaneClosed?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                this.IsPaneOpen = false;
            }
        }

        /// <summary>
        ///     It is currently impossible to bind the value of an animation but one workaround is to retrieve the element using
        ///     its name and then applying the binding after the creation of the animation...
        ///     This is the extract from the MSDN:
        ///     You can't use dynamic resource references or data binding expressions to set Storyboard or animation property
        ///     values.
        ///     That's because everything inside a ControlTemplate must be thread-safe, and the timing system must Freeze
        ///     Storyboard objects to make them thread-safe.
        ///     A Storyboard cannot be frozen if it or its child timelines contain dynamic resource references or data binding
        ///     expressions
        ///     For more info:
        ///     https://social.msdn.microsoft.com/Forums/vstudio/en-US/1b2bb84d-58c6-4dde-9c6c-00ed466e2382/using-binding-in-visualstate-storyboard?forum=wpf
        /// </summary>
        private void HackBindingAnimations()
        {
            var closedToOpenOverlayLeftPaneTransformXAnimation = this.GetTemplateChild("PART_ClosedToOpenOverlayLeftPaneTransformXAnimation") as DiscreteDoubleKeyFrame;
            if (closedToOpenOverlayLeftPaneTransformXAnimation != null)
            {
                var binding = new Binding("TemplateSettings.NegativeOpenPaneLength") { Source = this };
                BindingOperations.SetBinding(closedToOpenOverlayLeftPaneTransformXAnimation, DoubleKeyFrame.ValueProperty, binding);
            }

            var closedToOpenOverlayLeftPaneClipRectangleTransformXAnimation = this.GetTemplateChild("PART_ClosedToOpenOverlayLeftPaneClipRectangleTransformXAnimation") as DiscreteDoubleKeyFrame;
            if (closedToOpenOverlayLeftPaneClipRectangleTransformXAnimation != null)
            {
                var binding = new Binding("TemplateSettings.OpenPaneLength") { Source = this };
                BindingOperations.SetBinding(closedToOpenOverlayLeftPaneClipRectangleTransformXAnimation, DoubleKeyFrame.ValueProperty, binding);
            }

            var closedToOpenOverlayRightPaneTransformXAnimation = this.GetTemplateChild("PART_ClosedToOpenOverlayRightPaneTransformXAnimation") as DiscreteDoubleKeyFrame;
            if (closedToOpenOverlayRightPaneTransformXAnimation != null)
            {
                var binding = new Binding("TemplateSettings.OpenPaneLength") { Source = this };
                BindingOperations.SetBinding(closedToOpenOverlayRightPaneTransformXAnimation, DoubleKeyFrame.ValueProperty, binding);
            }

            var closedToOpenOverlayRightPaneClipRectangleTransformXAnimation = this.GetTemplateChild("PART_ClosedToOpenOverlayRightPaneClipRectangleTransformXAnimation") as DiscreteDoubleKeyFrame;
            if (closedToOpenOverlayRightPaneClipRectangleTransformXAnimation != null)
            {
                var binding = new Binding("TemplateSettings.NegativeOpenPaneLength") { Source = this };
                BindingOperations.SetBinding(closedToOpenOverlayRightPaneClipRectangleTransformXAnimation, DoubleKeyFrame.ValueProperty, binding);
            }

            var closedCompactLeftToOpenCompactOverlayLeftColumnDefinition1WidthAnimation = this.GetTemplateChild("PART_ClosedCompactLeftToOpenCompactOverlayLeftColumnDefinition1WidthAnimation") as GridLengthAnimation;
            if (closedCompactLeftToOpenCompactOverlayLeftColumnDefinition1WidthAnimation != null)
            {
                var binding = new Binding("TemplateSettings.CompactPaneGridLength") { Source = this };
                BindingOperations.SetBinding(closedCompactLeftToOpenCompactOverlayLeftColumnDefinition1WidthAnimation, GridLengthAnimation.ToProperty, binding);
            }

            var closedCompactLeftToOpenCompactOverlayLeftPaneClipRectangleTransformXAnimation = this.GetTemplateChild("PART_ClosedCompactLeftToOpenCompactOverlayLeftPaneClipRectangleTransformXAnimation") as DiscreteDoubleKeyFrame;
            if (closedCompactLeftToOpenCompactOverlayLeftPaneClipRectangleTransformXAnimation != null)
            {
                var binding = new Binding("TemplateSettings.NegativeOpenPaneLengthMinusCompactLength") { Source = this };
                BindingOperations.SetBinding(closedCompactLeftToOpenCompactOverlayLeftPaneClipRectangleTransformXAnimation, DoubleKeyFrame.ValueProperty, binding);
            }

            var closedCompactRightToOpenCompactOverlayRightColumnDefinition2WidthAnimation = this.GetTemplateChild("PART_ClosedCompactRightToOpenCompactOverlayRightColumnDefinition2WidthAnimation") as GridLengthAnimation;
            if (closedCompactRightToOpenCompactOverlayRightColumnDefinition2WidthAnimation != null)
            {
                var binding = new Binding("TemplateSettings.CompactPaneGridLength") { Source = this };
                BindingOperations.SetBinding(closedCompactRightToOpenCompactOverlayRightColumnDefinition2WidthAnimation, GridLengthAnimation.ToProperty, binding);
            }

            var closedCompactRightToOpenCompactOverlayRightPaneClipRectangleTransformXAnimation = this.GetTemplateChild("PART_ClosedCompactRightToOpenCompactOverlayRightPaneClipRectangleTransformXAnimation") as DiscreteDoubleKeyFrame;
            if (closedCompactRightToOpenCompactOverlayRightPaneClipRectangleTransformXAnimation != null)
            {
                var binding = new Binding("TemplateSettings.OpenPaneLengthMinusCompactLength") { Source = this };
                BindingOperations.SetBinding(closedCompactRightToOpenCompactOverlayRightPaneClipRectangleTransformXAnimation, DoubleKeyFrame.ValueProperty, binding);
            }

            var openOverlayLeftToClosedPanePaneTransformXAnimation = this.GetTemplateChild("PART_OpenOverlayLeftToClosedPanePaneTransformXAnimation") as SplineDoubleKeyFrame;
            if (openOverlayLeftToClosedPanePaneTransformXAnimation != null)
            {
                var binding = new Binding("TemplateSettings.NegativeOpenPaneLength") { Source = this };
                BindingOperations.SetBinding(openOverlayLeftToClosedPanePaneTransformXAnimation, DoubleKeyFrame.ValueProperty, binding);
            }

            var openOverlayLeftToClosedPaneClipRectangleTransformXAnimation = this.GetTemplateChild("PART_OpenOverlayLeftToClosedPaneClipRectangleTransformXAnimation") as SplineDoubleKeyFrame;
            if (openOverlayLeftToClosedPaneClipRectangleTransformXAnimation != null)
            {
                var binding = new Binding("TemplateSettings.OpenPaneLength") { Source = this };
                BindingOperations.SetBinding(openOverlayLeftToClosedPaneClipRectangleTransformXAnimation, DoubleKeyFrame.ValueProperty, binding);
            }

            var openOverlayRightToClosedPanePaneTransformXAnimation = this.GetTemplateChild("PART_OpenOverlayRightToClosedPanePaneTransformXAnimation") as SplineDoubleKeyFrame;
            if (openOverlayRightToClosedPanePaneTransformXAnimation != null)
            {
                var binding = new Binding("TemplateSettings.OpenPaneLength") { Source = this };
                BindingOperations.SetBinding(openOverlayRightToClosedPanePaneTransformXAnimation, DoubleKeyFrame.ValueProperty, binding);
            }

            var openOverlayRightToClosedPaneClipRectangleTransformXAnimation = this.GetTemplateChild("PART_OpenOverlayRightToClosedPaneClipRectangleTransformXAnimation") as SplineDoubleKeyFrame;
            if (openOverlayRightToClosedPaneClipRectangleTransformXAnimation != null)
            {
                var binding = new Binding("TemplateSettings.NegativeOpenPaneLength") { Source = this };
                BindingOperations.SetBinding(openOverlayRightToClosedPaneClipRectangleTransformXAnimation, DoubleKeyFrame.ValueProperty, binding);
            }

            var openCompactOverlayRightToClosedCompactRightColumnDefinition2WidthAnimation = this.GetTemplateChild("PART_OpenCompactOverlayRightToClosedCompactRightColumnDefinition2WidthAnimation") as GridLengthAnimation;
            if (openCompactOverlayRightToClosedCompactRightColumnDefinition2WidthAnimation != null)
            {
                var binding = new Binding("TemplateSettings.CompactPaneGridLength") { Source = this };
                BindingOperations.SetBinding(openCompactOverlayRightToClosedCompactRightColumnDefinition2WidthAnimation, GridLengthAnimation.ToProperty, binding);
            }

            var openCompactOverlayLeftToClosedCompactLeftColumnDefinition1Width = this.GetTemplateChild("PART_OpenCompactOverlayLeftToClosedCompactLeftColumnDefinition1Width") as GridLengthAnimation;
            if (openCompactOverlayLeftToClosedCompactLeftColumnDefinition1Width != null)
            {
                var binding = new Binding("TemplateSettings.CompactPaneGridLength") { Source = this };
                BindingOperations.SetBinding(openCompactOverlayLeftToClosedCompactLeftColumnDefinition1Width, GridLengthAnimation.ToProperty, binding);
            }

            var openCompactOverlayLeftToClosedCompactLeftPaneClipRectangleTransformXAnimation = this.GetTemplateChild("PART_OpenCompactOverlayLeftToClosedCompactLeftPaneClipRectangleTransformXAnimation") as SplineDoubleKeyFrame;
            if (openCompactOverlayLeftToClosedCompactLeftPaneClipRectangleTransformXAnimation != null)
            {
                var binding = new Binding("TemplateSettings.NegativeOpenPaneLengthMinusCompactLength") { Source = this };
                BindingOperations.SetBinding(openCompactOverlayLeftToClosedCompactLeftPaneClipRectangleTransformXAnimation, DoubleKeyFrame.ValueProperty, binding);
            }

            var openCompactOverlayRightToClosedCompactRightPaneClipRectangleTransformXAnimation = this.GetTemplateChild("PART_OpenCompactOverlayRightToClosedCompactRightPaneClipRectangleTransformXAnimation") as SplineDoubleKeyFrame;
            if (openCompactOverlayRightToClosedCompactRightPaneClipRectangleTransformXAnimation != null)
            {
                var binding = new Binding("TemplateSettings.OpenPaneLengthMinusCompactLength") { Source = this };
                BindingOperations.SetBinding(openCompactOverlayRightToClosedCompactRightPaneClipRectangleTransformXAnimation, DoubleKeyFrame.ValueProperty, binding);
            }

            var closedCompactLeftColumnDefinition1Width = this.GetTemplateChild("PART_ClosedCompactLeftColumnDefinition1Width") as GridLengthAnimation;
            if (closedCompactLeftColumnDefinition1Width != null)
            {
                var binding = new Binding("TemplateSettings.CompactPaneGridLength") { Source = this };
                BindingOperations.SetBinding(closedCompactLeftColumnDefinition1Width, GridLengthAnimation.ToProperty, binding);
            }

            var closedCompactLeftPaneClipRectangleTransformXAnimation = this.GetTemplateChild("PART_ClosedCompactLeftPaneClipRectangleTransformXAnimation") as DoubleAnimation;
            if (closedCompactLeftPaneClipRectangleTransformXAnimation != null)
            {
                var binding = new Binding("TemplateSettings.NegativeOpenPaneLengthMinusCompactLength") { Source = this };
                BindingOperations.SetBinding(closedCompactLeftPaneClipRectangleTransformXAnimation, DoubleAnimation.ToProperty, binding);
            }

            var closedCompactRightColumnDefinition2Width = this.GetTemplateChild("PART_ClosedCompactRightColumnDefinition2Width") as GridLengthAnimation;
            if (closedCompactRightColumnDefinition2Width != null)
            {
                var binding = new Binding("TemplateSettings.CompactPaneGridLength") { Source = this };
                BindingOperations.SetBinding(closedCompactRightColumnDefinition2Width, GridLengthAnimation.ToProperty, binding);
            }

            var closedCompactRightPaneClipRectangleTransformXAnimation = this.GetTemplateChild("PART_ClosedCompactRightPaneClipRectangleTransformXAnimation") as DoubleAnimation;
            if (closedCompactRightPaneClipRectangleTransformXAnimation != null)
            {
                var binding = new Binding("TemplateSettings.OpenPaneLengthMinusCompactLength") { Source = this };
                BindingOperations.SetBinding(closedCompactRightPaneClipRectangleTransformXAnimation, DoubleAnimation.ToProperty, binding);
            }

            var openInlineRightColumnDefinition2Width = this.GetTemplateChild("PART_OpenInlineRightColumnDefinition2Width") as GridLengthAnimation;
            if (openInlineRightColumnDefinition2Width != null)
            {
                var binding = new Binding("TemplateSettings.OpenPaneGridLength") { Source = this };
                BindingOperations.SetBinding(openInlineRightColumnDefinition2Width, GridLengthAnimation.ToProperty, binding);
            }

            var openCompactOverlayLeftColumnDefinition1Width = this.GetTemplateChild("PART_OpenCompactOverlayLeftColumnDefinition1Width") as GridLengthAnimation;
            if (openCompactOverlayLeftColumnDefinition1Width != null)
            {
                var binding = new Binding("TemplateSettings.CompactPaneGridLength") { Source = this };
                BindingOperations.SetBinding(openCompactOverlayLeftColumnDefinition1Width, GridLengthAnimation.ToProperty, binding);
            }

            var openCompactOverlayRightColumnDefinition2Width = this.GetTemplateChild("PART_OpenCompactOverlayRightColumnDefinition2Width") as GridLengthAnimation;
            if (openCompactOverlayRightColumnDefinition2Width != null)
            {
                var binding = new Binding("TemplateSettings.CompactPaneGridLength") { Source = this };
                BindingOperations.SetBinding(openCompactOverlayRightColumnDefinition2Width, GridLengthAnimation.ToProperty, binding);
            }
        }

        private void OnLightDismiss(object sender, MouseButtonEventArgs e)
        {
            this.IsPaneOpen = false;
        }
    }
}