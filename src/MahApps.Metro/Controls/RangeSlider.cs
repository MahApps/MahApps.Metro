// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ControlzEx;
using JetBrains.Annotations;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A slider control with the ability to select a range between two values.
    /// </summary>
    [DefaultEvent("RangeSelectionChanged")]
    [TemplatePart(Name = "PART_Container", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_RangeSliderContainer", Type = typeof(StackPanel))]
    [TemplatePart(Name = "PART_LeftEdge", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_LeftThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_MiddleThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_RightThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_RightEdge", Type = typeof(RepeatButton))]
    public class RangeSlider : RangeBase
    {
        #region Routed UI commands

        public static readonly RoutedUICommand MoveBack
            = new RoutedUICommand(nameof(MoveBack),
                                  nameof(MoveBack),
                                  typeof(RangeSlider),
                                  new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.B, ModifierKeys.Control) }));

        public static readonly RoutedUICommand MoveForward
            = new RoutedUICommand(nameof(MoveForward),
                                  nameof(MoveForward),
                                  typeof(RangeSlider),
                                  new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F, ModifierKeys.Control) }));

        public static readonly RoutedUICommand MoveAllForward
            = new RoutedUICommand(nameof(MoveAllForward),
                                  nameof(MoveAllForward),
                                  typeof(RangeSlider),
                                  new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F, ModifierKeys.Alt) }));

        public static readonly RoutedUICommand MoveAllBack
            = new RoutedUICommand(nameof(MoveAllBack),
                                  nameof(MoveAllBack),
                                  typeof(RangeSlider),
                                  new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.B, ModifierKeys.Alt) }));

        #endregion

        #region Routed events

        /// <summary>Identifies the <see cref="RangeSelectionChanged"/> routed event.</summary>
        public static readonly RoutedEvent RangeSelectionChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(RangeSelectionChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RangeSelectionChangedEventHandler<double>),
                                               typeof(RangeSlider));

        public event RangeSelectionChangedEventHandler<double> RangeSelectionChanged
        {
            add => this.AddHandler(RangeSelectionChangedEvent, value);
            remove => this.RemoveHandler(RangeSelectionChangedEvent, value);
        }

        /// <summary>Identifies the <see cref="LowerValueChanged"/> routed event.</summary>
        public static readonly RoutedEvent LowerValueChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(LowerValueChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedPropertyChangedEventHandler<double>),
                                               typeof(RangeSlider));

        public event RoutedPropertyChangedEventHandler<double> LowerValueChanged
        {
            add => this.AddHandler(LowerValueChangedEvent, value);
            remove => this.RemoveHandler(LowerValueChangedEvent, value);
        }

        /// <summary>Identifies the <see cref="UpperValueChanged"/> routed event.</summary>
        public static readonly RoutedEvent UpperValueChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(UpperValueChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedPropertyChangedEventHandler<double>),
                                               typeof(RangeSlider));

        public event RoutedPropertyChangedEventHandler<double> UpperValueChanged
        {
            add => this.AddHandler(UpperValueChangedEvent, value);
            remove => this.RemoveHandler(UpperValueChangedEvent, value);
        }

        /// <summary>Identifies the <see cref="LowerThumbDragStarted"/> routed event.</summary>
        public static readonly RoutedEvent LowerThumbDragStartedEvent
            = EventManager.RegisterRoutedEvent(nameof(LowerThumbDragStarted),
                                               RoutingStrategy.Bubble,
                                               typeof(DragStartedEventHandler),
                                               typeof(RangeSlider));

        public event DragStartedEventHandler LowerThumbDragStarted
        {
            add => this.AddHandler(LowerThumbDragStartedEvent, value);
            remove => this.RemoveHandler(LowerThumbDragStartedEvent, value);
        }

        /// <summary>Identifies the <see cref="LowerThumbDragCompleted"/> routed event.</summary>
        public static readonly RoutedEvent LowerThumbDragCompletedEvent
            = EventManager.RegisterRoutedEvent(nameof(LowerThumbDragCompleted),
                                               RoutingStrategy.Bubble,
                                               typeof(DragCompletedEventHandler),
                                               typeof(RangeSlider));

        public event DragCompletedEventHandler LowerThumbDragCompleted
        {
            add => this.AddHandler(LowerThumbDragCompletedEvent, value);
            remove => this.RemoveHandler(LowerThumbDragCompletedEvent, value);
        }

        /// <summary>Identifies the <see cref="UpperThumbDragStarted"/> routed event.</summary>
        public static readonly RoutedEvent UpperThumbDragStartedEvent
            = EventManager.RegisterRoutedEvent(nameof(UpperThumbDragStarted),
                                               RoutingStrategy.Bubble,
                                               typeof(DragStartedEventHandler),
                                               typeof(RangeSlider));

        public event DragStartedEventHandler UpperThumbDragStarted
        {
            add => this.AddHandler(UpperThumbDragStartedEvent, value);
            remove => this.RemoveHandler(UpperThumbDragStartedEvent, value);
        }

        /// <summary>Identifies the <see cref="UpperThumbDragCompleted"/> routed event.</summary>
        public static readonly RoutedEvent UpperThumbDragCompletedEvent
            = EventManager.RegisterRoutedEvent(nameof(UpperThumbDragCompleted),
                                               RoutingStrategy.Bubble,
                                               typeof(DragCompletedEventHandler),
                                               typeof(RangeSlider));

        public event DragCompletedEventHandler UpperThumbDragCompleted
        {
            add => this.AddHandler(UpperThumbDragCompletedEvent, value);
            remove => this.RemoveHandler(UpperThumbDragCompletedEvent, value);
        }

        /// <summary>Identifies the <see cref="CentralThumbDragStarted"/> routed event.</summary>
        public static readonly RoutedEvent CentralThumbDragStartedEvent
            = EventManager.RegisterRoutedEvent(nameof(CentralThumbDragStarted),
                                               RoutingStrategy.Bubble,
                                               typeof(DragStartedEventHandler),
                                               typeof(RangeSlider));

        public event DragStartedEventHandler CentralThumbDragStarted
        {
            add => this.AddHandler(CentralThumbDragStartedEvent, value);
            remove => this.RemoveHandler(CentralThumbDragStartedEvent, value);
        }

        /// <summary>Identifies the <see cref="CentralThumbDragCompleted"/> routed event.</summary>
        public static readonly RoutedEvent CentralThumbDragCompletedEvent
            = EventManager.RegisterRoutedEvent(nameof(CentralThumbDragCompleted),
                                               RoutingStrategy.Bubble,
                                               typeof(DragCompletedEventHandler),
                                               typeof(RangeSlider));

        public event DragCompletedEventHandler CentralThumbDragCompleted
        {
            add => this.AddHandler(CentralThumbDragCompletedEvent, value);
            remove => this.RemoveHandler(CentralThumbDragCompletedEvent, value);
        }

        /// <summary>Identifies the <see cref="LowerThumbDragDelta"/> routed event.</summary>
        public static readonly RoutedEvent LowerThumbDragDeltaEvent
            = EventManager.RegisterRoutedEvent(nameof(LowerThumbDragDelta),
                                               RoutingStrategy.Bubble,
                                               typeof(DragDeltaEventHandler),
                                               typeof(RangeSlider));

        public event DragDeltaEventHandler LowerThumbDragDelta
        {
            add => this.AddHandler(LowerThumbDragDeltaEvent, value);
            remove => this.RemoveHandler(LowerThumbDragDeltaEvent, value);
        }

        /// <summary>Identifies the <see cref="UpperThumbDragDelta"/> routed event.</summary>
        public static readonly RoutedEvent UpperThumbDragDeltaEvent
            = EventManager.RegisterRoutedEvent(nameof(UpperThumbDragDelta),
                                               RoutingStrategy.Bubble,
                                               typeof(DragDeltaEventHandler),
                                               typeof(RangeSlider));

        public event DragDeltaEventHandler UpperThumbDragDelta
        {
            add => this.AddHandler(UpperThumbDragDeltaEvent, value);
            remove => this.RemoveHandler(UpperThumbDragDeltaEvent, value);
        }

        /// <summary>Identifies the <see cref="CentralThumbDragDelta"/> routed event.</summary>
        public static readonly RoutedEvent CentralThumbDragDeltaEvent
            = EventManager.RegisterRoutedEvent(nameof(CentralThumbDragDelta),
                                               RoutingStrategy.Bubble,
                                               typeof(DragDeltaEventHandler),
                                               typeof(RangeSlider));

        public event DragDeltaEventHandler CentralThumbDragDelta
        {
            add => this.AddHandler(CentralThumbDragDeltaEvent, value);
            remove => this.RemoveHandler(CentralThumbDragDeltaEvent, value);
        }

        #endregion

        #region Dependency properties

        /// <summary>Identifies the <see cref="UpperValue"/> dependency property.</summary>
        public static readonly DependencyProperty UpperValueProperty
            = DependencyProperty.Register(nameof(UpperValue),
                                          typeof(double),
                                          typeof(RangeSlider),
                                          new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, RangesChanged, CoerceUpperValue));

        /// <summary>
        /// Get/sets the end of the range selection.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        public double UpperValue
        {
            get => (double)this.GetValue(UpperValueProperty);
            set => this.SetValue(UpperValueProperty, value);
        }

        internal static object CoerceUpperValue(DependencyObject d, object basevalue)
        {
            if (d is RangeSlider rangeSlider && basevalue is double value)
            {
                if (value > rangeSlider.Maximum || rangeSlider.LowerValue + rangeSlider.MinRange > rangeSlider.Maximum)
                {
                    return rangeSlider.Maximum;
                }

                if (value < rangeSlider.LowerValue + rangeSlider.MinRange)
                {
                    return rangeSlider.LowerValue + rangeSlider.MinRange;
                }
            }

            return basevalue;
        }

        /// <summary>Identifies the <see cref="LowerValue"/> dependency property.</summary>
        public static readonly DependencyProperty LowerValueProperty
            = DependencyProperty.Register(nameof(LowerValue),
                                          typeof(double),
                                          typeof(RangeSlider),
                                          new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, RangesChanged, CoerceLowerValue));

        /// <summary>
        /// Get/sets the beginning of the range selection.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        public double LowerValue
        {
            get => (double)this.GetValue(LowerValueProperty);
            set => this.SetValue(LowerValueProperty, value);
        }

        //Lower/Upper values property changed callback
        private static void RangesChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is RangeSlider rangeSlider)
            {
                if (rangeSlider._internalUpdate)
                {
                    return;
                }

                rangeSlider.CoerceLowerUpperValues();
            }
        }

        private void CoerceLowerUpperValues()
        {
            this.CoerceValue(LowerValueProperty);
            this.CoerceValue(UpperValueProperty);
            RaiseValueChangedEvents(this);
            this._oldLower = this.LowerValue;
            this._oldUpper = this.UpperValue;
            this.ReCalculateSize();
        }

        internal static object CoerceLowerValue(DependencyObject d, object basevalue)
        {
            if (d is RangeSlider rangeSlider && basevalue is double value)
            {
                if (value < rangeSlider.Minimum || rangeSlider.UpperValue - rangeSlider.MinRange < rangeSlider.Minimum)
                {
                    return rangeSlider.Minimum;
                }

                if (value > rangeSlider.UpperValue - rangeSlider.MinRange)
                {
                    return rangeSlider.UpperValue - rangeSlider.MinRange;
                }
            }

            return basevalue;
        }

        /// <summary>Identifies the <see cref="MinRange"/> dependency property.</summary>
        public static readonly DependencyProperty MinRangeProperty
            = DependencyProperty.Register(nameof(MinRange),
                                          typeof(double),
                                          typeof(RangeSlider),
                                          new FrameworkPropertyMetadata(0d, MinRangeChanged, CoerceMinRange), IsValidMinRange);

        /// <summary>
        /// Get/sets the minimum range that can be selected.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        public double MinRange
        {
            get => (double)this.GetValue(MinRangeProperty);
            set => this.SetValue(MinRangeProperty, value);
        }

        private static void MinRangeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var value = (double)e.NewValue;
            if (value < 0)
            {
                value = 0;
            }

            var slider = (RangeSlider)dependencyObject;
            dependencyObject.CoerceValue(MinRangeProperty);
            slider._internalUpdate = true;
            slider.UpperValue = Math.Max(slider.UpperValue, slider.LowerValue + value);
            slider.UpperValue = Math.Min(slider.UpperValue, slider.Maximum);
            slider._internalUpdate = false;

            slider.CoerceValue(UpperValueProperty);

            RaiseValueChangedEvents(dependencyObject);

            slider._oldLower = slider.LowerValue;
            slider._oldUpper = slider.UpperValue;

            slider.ReCalculateSize();
        }

        private static object CoerceMinRange(DependencyObject d, object basevalue)
        {
            var rangeSlider = (RangeSlider)d;
            var value = (double)basevalue;

            if (rangeSlider.LowerValue + value > rangeSlider.Maximum)
            {
                return rangeSlider.Maximum - rangeSlider.LowerValue;
            }

            return basevalue;
        }

        /// <summary>Identifies the <see cref="MinRangeWidth"/> dependency property.</summary>
        public static readonly DependencyProperty MinRangeWidthProperty
            = DependencyProperty.Register(nameof(MinRangeWidth),
                                          typeof(double),
                                          typeof(RangeSlider),
                                          new FrameworkPropertyMetadata(30d, MinRangeWidthChanged, CoerceMinRangeWidth), IsValidMinRange);

        /// <summary>
        /// Get/sets the minimal distance between two thumbs.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        public double MinRangeWidth
        {
            get => (double)this.GetValue(MinRangeWidthProperty);
            set => this.SetValue(MinRangeWidthProperty, value);
        }

        private static void MinRangeWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as RangeSlider)?.ReCalculateSize();
        }

        private static object CoerceMinRangeWidth(DependencyObject d, object basevalue)
        {
            var rangeSlider = (RangeSlider)d;

            if (rangeSlider._leftThumb != null && rangeSlider._rightThumb != null)
            {
                double width;
                if (rangeSlider.Orientation == Orientation.Horizontal)
                {
                    width = rangeSlider.ActualWidth - rangeSlider._leftThumb.ActualWidth - rangeSlider._rightThumb.ActualWidth;
                }
                else
                {
                    width = rangeSlider.ActualHeight - rangeSlider._leftThumb.ActualHeight - rangeSlider._rightThumb.ActualHeight;
                }

                return (double)basevalue > width / 2 ? width / 2 : (double)basevalue;
            }

            return basevalue;
        }

        private static bool IsValidMinRange(object value)
        {
            return value is double doubleValue && IsValidDouble(doubleValue) && doubleValue >= 0d;
        }

        /// <summary>Identifies the <see cref="MoveWholeRange"/> dependency property.</summary>
        public static readonly DependencyProperty MoveWholeRangeProperty
            = DependencyProperty.Register(nameof(MoveWholeRange),
                                          typeof(bool),
                                          typeof(RangeSlider),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Get/sets whether whole range will be moved when press on right/left/central part of control
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        public bool MoveWholeRange
        {
            get => (bool)this.GetValue(MoveWholeRangeProperty);
            set => this.SetValue(MoveWholeRangeProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ExtendedMode"/> dependency property.</summary>
        public static readonly DependencyProperty ExtendedModeProperty
            = DependencyProperty.Register(nameof(ExtendedMode),
                                          typeof(bool),
                                          typeof(RangeSlider),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Get/sets whether possibility to make manipulations inside range with left/right mouse buttons + cotrol button
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        public bool ExtendedMode
        {
            get => (bool)this.GetValue(ExtendedModeProperty);
            set => this.SetValue(ExtendedModeProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="IsSnapToTickEnabled"/> dependency property.</summary>
        public static readonly DependencyProperty IsSnapToTickEnabledProperty
            = DependencyProperty.Register(nameof(IsSnapToTickEnabled),
                                          typeof(bool),
                                          typeof(RangeSlider),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Get/sets whether possibility to make manipulations inside range with left/right mouse buttons + cotrol button
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        public bool IsSnapToTickEnabled
        {
            get => (bool)this.GetValue(IsSnapToTickEnabledProperty);
            set => this.SetValue(IsSnapToTickEnabledProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="Orientation"/> dependency property.</summary>
        public static readonly DependencyProperty OrientationProperty
            = DependencyProperty.Register(nameof(Orientation),
                                          typeof(Orientation),
                                          typeof(RangeSlider),
                                          new FrameworkPropertyMetadata(Orientation.Horizontal));

        /// <summary>
        /// Gets or sets the orientation of the <see cref="T:MahApps.Metro.Controls.RangeSlider" />.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        public Orientation Orientation
        {
            get => (Orientation)this.GetValue(OrientationProperty);
            set => this.SetValue(OrientationProperty, value);
        }

        /// <summary>Identifies the <see cref="TickPlacement"/> dependency property.</summary>
        public static readonly DependencyProperty TickPlacementProperty
            = DependencyProperty.Register(nameof(TickPlacement),
                                          typeof(TickPlacement),
                                          typeof(RangeSlider),
                                          new FrameworkPropertyMetadata(TickPlacement.None));

        /// <summary>
        /// Gets or sets the position of tick marks with respect to the <see cref="T:System.Windows.Controls.Primitives.Track" /> of the <see cref="T:MahApps.Metro.Controls.RangeSlider" />.
        /// </summary>
        /// <returns>
        /// A <see cref="P:MahApps.Metro.Controls.RangeSlider.TickPlacement" /> value that defines how to position the tick marks in a <see cref="T:MahApps.Metro.Controls.RangeSlider" /> with respect to the slider bar. The default is <see cref="F:System.Windows.Controls.Primitives.TickPlacement.None" />.
        /// </returns>
        [Bindable(true)]
        [Category("Appearance")]
        public TickPlacement TickPlacement
        {
            get => (TickPlacement)this.GetValue(TickPlacementProperty);
            set => this.SetValue(TickPlacementProperty, value);
        }

        /// <summary>Identifies the <see cref="TickFrequency"/> dependency property.</summary>
        public static readonly DependencyProperty TickFrequencyProperty
            = DependencyProperty.Register(nameof(TickFrequency),
                                          typeof(double),
                                          typeof(RangeSlider),
                                          new FrameworkPropertyMetadata(1d), IsValidDoubleValue);

        /// <summary>
        /// Gets or sets the interval between tick marks.
        /// </summary>
        /// <returns>
        /// The distance between tick marks. The default is (1.0).
        /// </returns>
        [Bindable(true)]
        [Category("Appearance")]
        public double TickFrequency
        {
            get => (double)this.GetValue(TickFrequencyProperty);
            set => this.SetValue(TickFrequencyProperty, value);
        }

        /// <summary>Identifies the <see cref="Ticks"/> dependency property.</summary>
        public static readonly DependencyProperty TicksProperty
            = DependencyProperty.Register(nameof(Ticks),
                                          typeof(DoubleCollection),
                                          typeof(RangeSlider),
                                          new FrameworkPropertyMetadata(default(DoubleCollection)));

        /// <summary>
        /// Gets or sets the positions of the tick marks to display for a <see cref="T:MahApps.Metro.Controls.RangeSlider" />. </summary>
        /// <returns>
        /// A set of tick marks to display for a <see cref="T:MahApps.Metro.Controls.RangeSlider" />. The default is <see langword="null" />.
        /// </returns>
        [Bindable(true)]
        [Category("Appearance")]
        public DoubleCollection Ticks
        {
            get => (DoubleCollection)this.GetValue(TicksProperty);
            set => this.SetValue(TicksProperty, value);
        }

        /// <summary>Identifies the <see cref="IsMoveToPointEnabled"/> dependency property.</summary>
        public static readonly DependencyProperty IsMoveToPointEnabledProperty
            = DependencyProperty.Register(nameof(IsMoveToPointEnabled),
                                          typeof(bool),
                                          typeof(RangeSlider),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Get or sets IsMoveToPoint feature which will enable/disable moving to exact point inside control when user clicked on it
        /// Gets or sets a value that indicates whether the two <see cref="P:System.Windows.Controls.Primitives.Track.Thumb" /> of a <see cref="T:MahApps.Metro.Controls.RangeSlider" /> moves immediately to the location of the mouse click that occurs while the mouse pointer pauses on the <see cref="T:MahApps.Metro.Controls.RangeSlider" /> tracks.
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        public bool IsMoveToPointEnabled
        {
            get => (bool)this.GetValue(IsMoveToPointEnabledProperty);
            set => this.SetValue(IsMoveToPointEnabledProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="AutoToolTipPlacement"/> dependency property.</summary>
        public static readonly DependencyProperty AutoToolTipPlacementProperty
            = DependencyProperty.Register(nameof(AutoToolTipPlacement),
                                          typeof(AutoToolTipPlacement),
                                          typeof(RangeSlider),
                                          new FrameworkPropertyMetadata(AutoToolTipPlacement.None));

        /// <summary>
        /// Gets or sets whether a tooltip that contains the current value of the <see cref="T:MahApps.Metro.Controls.RangeSlider" /> displays when the <see cref="P:System.Windows.Controls.Primitives.Track.Thumb" /> is pressed. If a tooltip is displayed, this property also specifies the placement of the tooltip.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Windows.Controls.Primitives.AutoToolTipPlacement" /> values that determines where to display the tooltip with respect to the <see cref="P:System.Windows.Controls.Primitives.Track.Thumb" /> of the <see cref="T:MahApps.Metro.Controls.RangeSlider" />, or that specifies to not show a tooltip. The default is <see cref="F:System.Windows.Controls.Primitives.AutoToolTipPlacement.None" />, which specifies that a tooltip is not displayed.
        /// </returns>
        [Bindable(true)]
        [Category("Behavior")]
        public AutoToolTipPlacement AutoToolTipPlacement
        {
            get => (AutoToolTipPlacement)this.GetValue(AutoToolTipPlacementProperty);
            set => this.SetValue(AutoToolTipPlacementProperty, (object)value);
        }

        /// <summary>Identifies the <see cref="AutoToolTipPrecision"/> dependency property.</summary>
        public static readonly DependencyProperty AutoToolTipPrecisionProperty
            = DependencyProperty.Register(nameof(AutoToolTipPrecision),
                                          typeof(int),
                                          typeof(RangeSlider),
                                          new FrameworkPropertyMetadata(0), IsValidPrecision);

        /// <summary>
        /// Gets or sets the number of digits that are displayed to the right side of the decimal point for the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Value" /> of the <see cref="T:MahApps.Metro.Controls.RangeSlider" /> in a tooltip.
        /// </summary>
        /// <returns>
        /// The precision of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Value" /> that displays in the tooltip, specified as the number of digits that appear to the right of the decimal point. The default is zero (0).
        /// </returns>
        [Bindable(true)]
        [Category("Appearance")]
        public int AutoToolTipPrecision
        {
            get => (int)this.GetValue(AutoToolTipPrecisionProperty);
            set => this.SetValue(AutoToolTipPrecisionProperty, (object)value);
        }

        /// <summary>Identifies the <see cref="AutoToolTipLowerValueTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty AutoToolTipLowerValueTemplateProperty
            = DependencyProperty.Register(nameof(AutoToolTipLowerValueTemplate),
                                          typeof(DataTemplate),
                                          typeof(RangeSlider),
                                          new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets a template for the auto tooltip to show the lower value.
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        public DataTemplate AutoToolTipLowerValueTemplate
        {
            get => (DataTemplate)this.GetValue(AutoToolTipLowerValueTemplateProperty);
            set => this.SetValue(AutoToolTipLowerValueTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="AutoToolTipUpperValueTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty AutoToolTipUpperValueTemplateProperty
            = DependencyProperty.Register(nameof(AutoToolTipUpperValueTemplate),
                                          typeof(DataTemplate),
                                          typeof(RangeSlider),
                                          new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets a template for the auto tooltip to show the upper value.
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        public DataTemplate AutoToolTipUpperValueTemplate
        {
            get => (DataTemplate)this.GetValue(AutoToolTipUpperValueTemplateProperty);
            set => this.SetValue(AutoToolTipUpperValueTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="AutoToolTipRangeValuesTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty AutoToolTipRangeValuesTemplateProperty
            = DependencyProperty.Register(nameof(AutoToolTipRangeValuesTemplate),
                                          typeof(DataTemplate),
                                          typeof(RangeSlider),
                                          new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets a template for the auto tooltip to show the center value.
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        public DataTemplate AutoToolTipRangeValuesTemplate
        {
            get => (DataTemplate)this.GetValue(AutoToolTipRangeValuesTemplateProperty);
            set => this.SetValue(AutoToolTipRangeValuesTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="Interval"/> dependency property.</summary>
        public static readonly DependencyProperty IntervalProperty
            = DependencyProperty.Register(nameof(Interval),
                                          typeof(int),
                                          typeof(RangeSlider),
                                          new FrameworkPropertyMetadata(100, IntervalChangedCallback), IsValidPrecision);

        /// <summary>
        /// Get/sets value how fast thumbs will move when user press on left/right/central with left mouse button (IsMoveToPoint must be set to FALSE)
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        public int Interval
        {
            get => (int)this.GetValue(IntervalProperty);
            set => this.SetValue(IntervalProperty, value);
        }

        /// <summary>Identifies the <see cref="IsSelectionRangeEnabled"/> dependency property.</summary>
        public static readonly DependencyProperty IsSelectionRangeEnabledProperty
            = DependencyProperty.Register(nameof(IsSelectionRangeEnabled),
                                          typeof(bool),
                                          typeof(RangeSlider),
                                          new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets a value that indicates whether the <see cref="T:MahApps.Metro.Controls.RangeSlider" /> displays a selection range along the <see cref="T:MahApps.Metro.Controls.RangeSlider" />.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> if a selection range is displayed; otherwise, <see langword="false" />. The default is <see langword="false" />.
        /// </returns>
        [Bindable(true)]
        [Category("Appearance")]
        public bool IsSelectionRangeEnabled
        {
            get => (bool)this.GetValue(IsSelectionRangeEnabledProperty);
            set => this.SetValue(IsSelectionRangeEnabledProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="SelectionStart"/> dependency property.</summary>
        public static readonly DependencyProperty SelectionStartProperty
            = DependencyProperty.Register(nameof(SelectionStart),
                                          typeof(double),
                                          typeof(RangeSlider),
                                          new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectionStartChanged, CoerceSelectionStart),
                                          IsValidDoubleValue);

        /// <summary>
        /// Gets or sets the smallest value of a specified selection for a <see cref="T:MahApps.Metro.Controls.RangeSlider" />.
        /// </summary>
        /// <returns>
        /// The largest value of a selected range of values of a <see cref="T:MahApps.Metro.Controls.RangeSlider" />. The default is zero (0.0).
        /// </returns>
        [Bindable(true)]
        [Category("Appearance")]
        public double SelectionStart
        {
            get => (double)this.GetValue(SelectionStartProperty);
            set => this.SetValue(SelectionStartProperty, (object)value);
        }

        private static void OnSelectionStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RangeSlider)?.CoerceValue(SelectionEndProperty);
        }

        private static object CoerceSelectionStart(DependencyObject d, object value)
        {
            RangeSlider rangeSlider = (RangeSlider)d;
            double num = (double)value;
            double minimum = rangeSlider.Minimum;
            double maximum = rangeSlider.Maximum;

            if (num < minimum)
            {
                return minimum;
            }

            if (num > maximum)
            {
                return maximum;
            }

            return value;
        }

        /// <summary>Identifies the <see cref="SelectionEnd"/> dependency property.</summary>
        public static readonly DependencyProperty SelectionEndProperty
            = DependencyProperty.Register(nameof(SelectionEnd),
                                          typeof(double),
                                          typeof(RangeSlider),
                                          new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectionEndChanged, CoerceSelectionEnd),
                                          IsValidDoubleValue);

        /// <summary>
        /// Gets or sets the largest value of a specified selection for a <see cref="T:MahApps.Metro.Controls.RangeSlider" />.
        /// </summary>
        /// <returns>
        /// The largest value of a selected range of values of a <see cref="T:MahApps.Metro.Controls.RangeSlider" />. The default is zero (0.0).
        /// </returns>
        [Bindable(true)]
        [Category("Appearance")]
        public double SelectionEnd
        {
            get => (double)this.GetValue(SelectionEndProperty);
            set => this.SetValue(SelectionEndProperty, (object)value);
        }

        private static void OnSelectionEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RangeSlider)?.CoerceValue(SelectionStartProperty);
        }

        private static object CoerceSelectionEnd(DependencyObject d, object value)
        {
            RangeSlider rangeSlider = (RangeSlider)d;
            double num = (double)value;
            double selectionStart = rangeSlider.SelectionStart;
            double maximum = rangeSlider.Maximum;

            if (num < selectionStart)
            {
                return selectionStart;
            }

            if (num > maximum)
            {
                return maximum;
            }

            return value;
        }

        protected double MovableRange => this.Maximum - this.Minimum - this.MinRange;

        #endregion

        #region Variables

        private const double Epsilon = 0.00000153;

        private bool _internalUpdate;
        private Thumb _centerThumb;
        private Thumb _leftThumb;
        private Thumb _rightThumb;
        private RepeatButton _leftButton;
        private RepeatButton _rightButton;
        private StackPanel _visualElementsContainer;
        private FrameworkElement _container;
        private double _movableWidth;
        private readonly DispatcherTimer _timer;
        private uint _tickCount;
        private double _currentpoint;
        private bool _isInsideRange;
        private bool _centerThumbBlocked;
        private Direction _direction;
        private ButtonType _bType;
        private Point _position;
        private Point _basePoint;
        private double _currenValue;
        private double _density;
        private ToolTip _autoToolTip;
        private double _oldLower;
        private double _oldUpper;
        private bool _isMoved;
        private bool _roundToPrecision;
        private int _precision;
        private readonly PropertyChangeNotifier actualWidthPropertyChangeNotifier;
        private readonly PropertyChangeNotifier actualHeightPropertyChangeNotifier;

        #endregion

        public RangeSlider()
        {
            this.CommandBindings.Add(new CommandBinding(MoveBack, (sender, e) => this.MoveSelection(true)));
            this.CommandBindings.Add(new CommandBinding(MoveForward, (sender, e) => this.MoveSelection(false)));
            this.CommandBindings.Add(new CommandBinding(MoveAllForward, (sender, e) => this.ResetSelection(false)));
            this.CommandBindings.Add(new CommandBinding(MoveAllBack, (sender, e) => this.ResetSelection(true)));

            this.actualWidthPropertyChangeNotifier = new PropertyChangeNotifier(this, ActualWidthProperty);
            this.actualWidthPropertyChangeNotifier.ValueChanged += (s, e) => this.ReCalculateSize();
            this.actualHeightPropertyChangeNotifier = new PropertyChangeNotifier(this, ActualHeightProperty);
            this.actualHeightPropertyChangeNotifier.ValueChanged += (s, e) => this.ReCalculateSize();

            this._timer = new DispatcherTimer();
            this._timer.Tick += this.MoveToNextValue;
            this._timer.Interval = TimeSpan.FromMilliseconds(this.Interval);
        }

        static RangeSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(typeof(RangeSlider)));
            MinimumProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure, MinPropertyChangedCallback, CoerceMinimum));
            MaximumProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsMeasure, MaxPropertyChangedCallback, CoerceMaximum));
        }

        /// <summary>
        /// Responds to a change in the value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum"/> property.
        /// </summary>
        /// <param name="oldMinimum">The old value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum"/> property.</param><param name="newMinimum">The new value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum"/> property.</param>
        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            this.CoerceValue(SelectionStartProperty);
            this.ReCalculateSize();
        }

        /// <summary>
        /// Responds to a change in the value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum"/> property.
        /// </summary>
        /// <param name="oldMaximum">The old value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum"/> property.</param><param name="newMaximum">The new value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum"/> property.</param>
        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            this.CoerceValue(SelectionStartProperty);
            this.CoerceValue(SelectionEndProperty);
            this.ReCalculateSize();
        }

        private static void MoveThumb(FrameworkElement x, FrameworkElement y, double change, Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
            {
                MoveThumbHorizontal(x, y, change);
            }
            else if (orientation == Orientation.Vertical)
            {
                MoveThumbVertical(x, y, change);
            }
        }

        private static void MoveThumb(FrameworkElement x, FrameworkElement y, double change, Orientation orientation, out Direction direction)
        {
            direction = Direction.Increase;
            if (orientation == Orientation.Horizontal)
            {
                direction = change < 0 ? Direction.Decrease : Direction.Increase;
                MoveThumbHorizontal(x, y, change);
            }
            else if (orientation == Orientation.Vertical)
            {
                direction = change < 0 ? Direction.Increase : Direction.Decrease;
                MoveThumbVertical(x, y, change);
            }
        }

        private static void MoveThumbHorizontal(FrameworkElement x, FrameworkElement y, double horizonalChange)
        {
            if (!double.IsNaN(x.Width) && !double.IsNaN(y.Width))
            {
                if (horizonalChange < 0) //slider went left
                {
                    var change = GetChangeKeepPositive(x.Width, horizonalChange);
                    if (x.Name == "PART_MiddleThumb")
                    {
                        if (x.Width > x.MinWidth)
                        {
                            if (x.Width + change < x.MinWidth)
                            {
                                var dif = x.Width - x.MinWidth;
                                x.Width = x.MinWidth;
                                y.Width += dif;
                            }
                            else
                            {
                                x.Width += change;
                                y.Width -= change;
                            }
                        }
                    }
                    else
                    {
                        x.Width += change;
                        y.Width -= change;
                    }
                }
                else if (horizonalChange > 0) //slider went right if(horizontal change == 0 do nothing)
                {
                    var change = -GetChangeKeepPositive(y.Width, -horizonalChange);
                    if (y.Name == "PART_MiddleThumb")
                    {
                        if (y.Width > y.MinWidth)
                        {
                            if (y.Width - change < y.MinWidth)
                            {
                                var dif = y.Width - y.MinWidth;
                                y.Width = y.MinWidth;
                                x.Width += dif;
                            }
                            else
                            {
                                x.Width += change;
                                y.Width -= change;
                            }
                        }
                    }
                    else
                    {
                        x.Width += change;
                        y.Width -= change;
                    }
                }
            }
        }

        private static void MoveThumbVertical(FrameworkElement x, FrameworkElement y, double verticalChange)
        {
            if (!double.IsNaN(x.Height) && !double.IsNaN(y.Height))
            {
                if (verticalChange < 0) //slider went up
                {
                    var change = -GetChangeKeepPositive(y.Height, verticalChange); //get positive number
                    if (y.Name == "PART_MiddleThumb")
                    {
                        if (y.Height > y.MinHeight)
                        {
                            if (y.Height - change < y.MinHeight)
                            {
                                var dif = y.Height - y.MinHeight;
                                y.Height = y.MinHeight;
                                x.Height += dif;
                            }
                            else
                            {
                                x.Height += change;
                                y.Height -= change;
                            }
                        }
                    }
                    else
                    {
                        x.Height += change;
                        y.Height -= change;
                    }
                }
                else if (verticalChange > 0) //slider went down if(horizontal change == 0 do nothing)
                {
                    var change = GetChangeKeepPositive(x.Height, -verticalChange); //get negative number
                    if (x.Name == "PART_MiddleThumb")
                    {
                        if (x.Height > y.MinHeight)
                        {
                            if (x.Height + change < x.MinHeight)
                            {
                                var dif = x.Height - x.MinHeight;
                                x.Height = x.MinHeight;
                                y.Height += dif;
                            }
                            else
                            {
                                x.Height += change;
                                y.Height -= change;
                            }
                        }
                    }
                    else
                    {
                        x.Height += change;
                        y.Height -= change;
                    }
                }
            }
        }

        //Recalculation of Control Height or Width
        private void ReCalculateSize()
        {
            if (this._leftButton != null && this._rightButton != null && this._centerThumb != null)
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    this._movableWidth = Math.Max(this.ActualWidth - this._rightThumb.ActualWidth - this._leftThumb.ActualWidth - this.MinRangeWidth, 1);
                    if (this.MovableRange <= 0)
                    {
                        this._leftButton.Width = double.NaN;
                        this._rightButton.Width = double.NaN;
                    }
                    else
                    {
                        this._leftButton.Width = Math.Max(this._movableWidth * (this.LowerValue - this.Minimum) / this.MovableRange, 0);
                        this._rightButton.Width = Math.Max(this._movableWidth * (this.Maximum - this.UpperValue) / this.MovableRange, 0);
                    }

                    if (IsValidDouble(this._rightButton.Width) && IsValidDouble(this._leftButton.Width))
                    {
                        this._centerThumb.Width = Math.Max(this.ActualWidth - (this._leftButton.Width + this._rightButton.Width + this._rightThumb.ActualWidth + this._leftThumb.ActualWidth), 0);
                    }
                    else
                    {
                        this._centerThumb.Width = Math.Max(this.ActualWidth - (this._rightThumb.ActualWidth + this._leftThumb.ActualWidth), 0);
                    }
                }
                else if (this.Orientation == Orientation.Vertical)
                {
                    this._movableWidth = Math.Max(this.ActualHeight - this._rightThumb.ActualHeight - this._leftThumb.ActualHeight - this.MinRangeWidth, 1);
                    if (this.MovableRange <= 0)
                    {
                        this._leftButton.Height = double.NaN;
                        this._rightButton.Height = double.NaN;
                    }
                    else
                    {
                        this._leftButton.Height = Math.Max(this._movableWidth * (this.LowerValue - this.Minimum) / this.MovableRange, 0);
                        this._rightButton.Height = Math.Max(this._movableWidth * (this.Maximum - this.UpperValue) / this.MovableRange, 0);
                    }

                    if (IsValidDouble(this._rightButton.Height) && IsValidDouble(this._leftButton.Height))
                    {
                        this._centerThumb.Height = Math.Max(this.ActualHeight - (this._leftButton.Height + this._rightButton.Height + this._rightThumb.ActualHeight + this._leftThumb.ActualHeight), 0);
                    }
                    else
                    {
                        this._centerThumb.Height = Math.Max(this.ActualHeight - (this._rightThumb.ActualHeight + this._leftThumb.ActualHeight), 0);
                    }
                }

                this._density = this._movableWidth / this.MovableRange;
            }
        }

        //Method calculates new values when IsSnapToTickEnabled = FALSE
        private void ReCalculateRangeSelected(bool reCalculateLowerValue, bool reCalculateUpperValue, Direction direction)
        {
            this._internalUpdate = true; //set flag to signal that the properties are being set by the object itself
            if (direction == Direction.Increase)
            {
                if (reCalculateUpperValue)
                {
                    this._oldUpper = this.UpperValue;
                    var width = this.Orientation == Orientation.Horizontal ? this._rightButton.Width : this._rightButton.Height;
                    //Check first if button width is not Double.NaN
                    if (IsValidDouble(width))
                    {
                        // Make sure to get exactly rangestop if thumb is at the end
                        var upper = Equals(width, 0.0) ? this.Maximum : Math.Min(this.Maximum, (this.Maximum - this.MovableRange * width / this._movableWidth));
                        this.UpperValue = this._isMoved ? upper : (this._roundToPrecision ? Math.Round(upper, this._precision) : upper);
                    }
                }

                if (reCalculateLowerValue)
                {
                    this._oldLower = this.LowerValue;
                    var width = this.Orientation == Orientation.Horizontal ? this._leftButton.Width : this._leftButton.Height;
                    //Check first if button width is not Double.NaN
                    if (IsValidDouble(width))
                    {
                        // Make sure to get exactly rangestart if thumb is at the start
                        var lower = Equals(width, 0.0) ? this.Minimum : Math.Max(this.Minimum, (this.Minimum + this.MovableRange * width / this._movableWidth));
                        this.LowerValue = this._isMoved ? lower : (this._roundToPrecision ? Math.Round(lower, this._precision) : lower);
                    }
                }
            }
            else
            {
                if (reCalculateLowerValue)
                {
                    this._oldLower = this.LowerValue;
                    var width = this.Orientation == Orientation.Horizontal ? this._leftButton.Width : this._leftButton.Height;
                    //Check first if button width is not Double.NaN
                    if (IsValidDouble(width))
                    {
                        // Make sure to get exactly rangestart if thumb is at the start
                        var lower = Equals(width, 0.0) ? this.Minimum : Math.Max(this.Minimum, (this.Minimum + this.MovableRange * width / this._movableWidth));
                        this.LowerValue = this._isMoved ? lower : (this._roundToPrecision ? Math.Round(lower, this._precision) : lower);
                    }
                }

                if (reCalculateUpperValue)
                {
                    this._oldUpper = this.UpperValue;
                    var width = this.Orientation == Orientation.Horizontal ? this._rightButton.Width : this._rightButton.Height;
                    //Check first if button width is not Double.NaN
                    if (IsValidDouble(width))
                    {
                        // Make sure to get exactly rangestop if thumb is at the end
                        var upper = Equals(width, 0.0) ? this.Maximum : Math.Min(this.Maximum, (this.Maximum - this.MovableRange * width / this._movableWidth));
                        this.UpperValue = this._isMoved ? upper : (this._roundToPrecision ? Math.Round(upper, this._precision) : upper);
                    }
                }
            }

            this._roundToPrecision = false;
            this._internalUpdate = false; //set flag to signal that the properties are being set by the object itself

            RaiseValueChangedEvents(this, reCalculateLowerValue, reCalculateUpperValue);
        }

        //Method used for cheking and setting correct values when IsSnapToTickEnable = TRUE (When thumb moving separately)
        private void ReCalculateRangeSelected(bool reCalculateLowerValue, bool reCalculateUpperValue, double value, Direction direction)
        {
            this._internalUpdate = true; //set flag to signal that the properties are being set by the object itself
            var tickFrequency = this.TickFrequency.ToString(CultureInfo.InvariantCulture);
            if (reCalculateLowerValue)
            {
                this._oldLower = this.LowerValue;
                double lower = 0;
                if (this.IsSnapToTickEnabled)
                {
                    lower = direction == Direction.Increase ? Math.Min(this.UpperValue - this.MinRange, value) : Math.Max(this.Minimum, value);
                }

                if (!tickFrequency.ToLower().Contains("e+") && tickFrequency.Contains("."))
                {
                    //decimal part is for cutting value exactly on that number of digits, which has TickFrequency to have correct values
                    var decimalPart = tickFrequency.Split('.');
                    this.LowerValue = Math.Round(lower, decimalPart[1].Length, MidpointRounding.AwayFromZero);
                }
                else
                {
                    this.LowerValue = lower;
                }
            }

            if (reCalculateUpperValue)
            {
                this._oldUpper = this.UpperValue;
                double upper = 0;
                if (this.IsSnapToTickEnabled)
                {
                    upper = direction == Direction.Increase ? Math.Min(value, this.Maximum) : Math.Max(this.LowerValue + this.MinRange, value);
                }

                if (!tickFrequency.ToLower().Contains("e+") && tickFrequency.Contains("."))
                {
                    var decimalPart = tickFrequency.Split('.');
                    this.UpperValue = Math.Round(upper, decimalPart[1].Length, MidpointRounding.AwayFromZero);
                }
                else
                {
                    this.UpperValue = upper;
                }
            }

            this._internalUpdate = false; //set flag to signal that the properties are being set by the object itself

            RaiseValueChangedEvents(this, reCalculateLowerValue, reCalculateUpperValue);
        }

        //Method used for cheking and setting correct values when IsSnapToTickEnable = TRUE (When thumb moving together)
        private void ReCalculateRangeSelected(double newLower, double newUpper, Direction direction)
        {
            double lower = 0,
                   upper = 0;
            this._internalUpdate = true; //set flag to signal that the properties are being set by the object itself
            this._oldLower = this.LowerValue;
            this._oldUpper = this.UpperValue;

            if (this.IsSnapToTickEnabled)
            {
                if (direction == Direction.Increase)
                {
                    lower = Math.Min(newLower, this.Maximum - (this.UpperValue - this.LowerValue));
                    upper = Math.Min(newUpper, this.Maximum);
                }
                else
                {
                    lower = Math.Max(newLower, this.Minimum);
                    upper = Math.Max(this.Minimum + (this.UpperValue - this.LowerValue), newUpper);
                }

                var tickFrequency = this.TickFrequency.ToString(CultureInfo.InvariantCulture);
                if (!tickFrequency.ToLower().Contains("e+") && tickFrequency.Contains("."))
                {
                    //decimal part is for cutting value exactly on that number of digits, which has TickFrequency to have correct values
                    var decimalPart = tickFrequency.Split('.');
                    //used when whole range decreasing to have correct updated values (lower first, upper - second)
                    if (direction == Direction.Decrease)
                    {
                        this.LowerValue = Math.Round(lower, decimalPart[1].Length, MidpointRounding.AwayFromZero);
                        this.UpperValue = Math.Round(upper, decimalPart[1].Length, MidpointRounding.AwayFromZero);
                    }
                    //used when whole range increasing to have correct updated values (upper first, lower - second)
                    else
                    {
                        this.UpperValue = Math.Round(upper, decimalPart[1].Length, MidpointRounding.AwayFromZero);
                        this.LowerValue = Math.Round(lower, decimalPart[1].Length, MidpointRounding.AwayFromZero);
                    }
                }
                else
                {
                    //used when whole range decreasing to have correct updated values (lower first, upper - second)
                    if (direction == Direction.Decrease)
                    {
                        this.LowerValue = lower;
                        this.UpperValue = upper;
                    }
                    //used when whole range increasing to have correct updated values (upper first, lower - second)
                    else
                    {
                        this.UpperValue = upper;
                        this.LowerValue = lower;
                    }
                }
            }

            this._internalUpdate = false; //set flag to signal that the properties are being set by the object itself

            RaiseValueChangedEvents(this);
        }

        public void MoveSelection(bool isLeft)
        {
            var widthChange = this.SmallChange * (this.UpperValue - this.LowerValue) * this._movableWidth / this.MovableRange;

            widthChange = isLeft ? -widthChange : widthChange;
            MoveThumb(this._leftButton, this._rightButton, widthChange, this.Orientation, out this._direction);
            this.ReCalculateRangeSelected(true, true, this._direction);
            this.CoerceLowerUpperValues();
        }

        public void ResetSelection(bool isStart)
        {
            var widthChange = this.Maximum - this.Minimum;
            widthChange = isStart ? -widthChange : widthChange;

            MoveThumb(this._leftButton, this._rightButton, widthChange, this.Orientation, out this._direction);
            this.ReCalculateRangeSelected(true, true, this._direction);
            this.CoerceLowerUpperValues();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._container = this.GetTemplateChild("PART_Container") as FrameworkElement;
            this._visualElementsContainer = this.GetTemplateChild("PART_RangeSliderContainer") as StackPanel;
            this._centerThumb = this.GetTemplateChild("PART_MiddleThumb") as Thumb;
            this._leftButton = this.GetTemplateChild("PART_LeftEdge") as RepeatButton;
            this._rightButton = this.GetTemplateChild("PART_RightEdge") as RepeatButton;
            this._leftThumb = this.GetTemplateChild("PART_LeftThumb") as Thumb;
            this._rightThumb = this.GetTemplateChild("PART_RightThumb") as Thumb;

            this.InitializeVisualElementsContainer();
            this.ReCalculateSize();
        }

        //adds visual element to the container
        private void InitializeVisualElementsContainer()
        {
            if (this._visualElementsContainer != null
                && this._leftThumb != null
                && this._rightThumb != null
                && this._centerThumb != null)
            {
                this._leftThumb.DragCompleted -= this.LeftThumbDragComplete;
                this._rightThumb.DragCompleted -= this.RightThumbDragComplete;
                this._leftThumb.DragStarted -= this.LeftThumbDragStart;
                this._rightThumb.DragStarted -= this.RightThumbDragStart;
                this._centerThumb.DragStarted -= this.CenterThumbDragStarted;
                this._centerThumb.DragCompleted -= this.CenterThumbDragCompleted;

                //handle the drag delta events
                this._centerThumb.DragDelta -= this.CenterThumbDragDelta;
                this._leftThumb.DragDelta -= this.LeftThumbDragDelta;
                this._rightThumb.DragDelta -= this.RightThumbDragDelta;

                this._visualElementsContainer.PreviewMouseDown -= this.VisualElementsContainerPreviewMouseDown;
                this._visualElementsContainer.PreviewMouseUp -= this.VisualElementsContainerPreviewMouseUp;
                this._visualElementsContainer.MouseLeave -= this.VisualElementsContainerMouseLeave;
                this._visualElementsContainer.MouseDown -= this.VisualElementsContainerMouseDown;

                this._leftThumb.DragCompleted += this.LeftThumbDragComplete;
                this._rightThumb.DragCompleted += this.RightThumbDragComplete;
                this._leftThumb.DragStarted += this.LeftThumbDragStart;
                this._rightThumb.DragStarted += this.RightThumbDragStart;
                this._centerThumb.DragStarted += this.CenterThumbDragStarted;
                this._centerThumb.DragCompleted += this.CenterThumbDragCompleted;

                //handle the drag delta events
                this._centerThumb.DragDelta += this.CenterThumbDragDelta;
                this._leftThumb.DragDelta += this.LeftThumbDragDelta;
                this._rightThumb.DragDelta += this.RightThumbDragDelta;

                this._visualElementsContainer.PreviewMouseDown += this.VisualElementsContainerPreviewMouseDown;
                this._visualElementsContainer.PreviewMouseUp += this.VisualElementsContainerPreviewMouseUp;
                this._visualElementsContainer.MouseLeave += this.VisualElementsContainerMouseLeave;
                this._visualElementsContainer.MouseDown += this.VisualElementsContainerMouseDown;
            }
        }

        //Handler for preview mouse button down for the whole StackPanel container
        private void VisualElementsContainerPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = Mouse.GetPosition(this._visualElementsContainer);
            if (this.Orientation == Orientation.Horizontal)
            {
                if (position.X < this._leftButton.ActualWidth)
                {
                    this.LeftButtonMouseDown();
                }
                else if (position.X > this.ActualWidth - this._rightButton.ActualWidth)
                {
                    this.RightButtonMouseDown();
                }
                else if (position.X > (this._leftButton.ActualWidth + this._leftThumb.ActualWidth) &&
                         position.X < (this.ActualWidth - (this._rightButton.ActualWidth + this._rightThumb.ActualWidth)))
                {
                    this.CentralThumbMouseDown();
                }
            }
            else
            {
                if (position.Y > this.ActualHeight - this._leftButton.ActualHeight)
                {
                    this.LeftButtonMouseDown();
                }
                else if (position.Y < this._rightButton.ActualHeight)
                {
                    this.RightButtonMouseDown();
                }
                else if (position.Y > (this._rightButton.ActualHeight + this._rightButton.ActualHeight) &&
                         position.Y < (this.ActualHeight - (this._leftButton.ActualHeight + this._leftThumb.ActualHeight)))
                {
                    this.CentralThumbMouseDown();
                }
            }
        }

        private void VisualElementsContainerMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                this.MoveWholeRange = this.MoveWholeRange != true;
            }
        }

        #region Mouse events

        private void VisualElementsContainerMouseLeave(object sender, MouseEventArgs e)
        {
            this._tickCount = 0;
            this._timer.Stop();
        }

        private void VisualElementsContainerPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            this._tickCount = 0;
            this._timer.Stop();
            this._centerThumbBlocked = false;
        }

        private void LeftButtonMouseDown()
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var p = Mouse.GetPosition(this._visualElementsContainer);
                var change = this.Orientation == Orientation.Horizontal
                    ? this._leftButton.ActualWidth - p.X + (this._leftThumb.ActualWidth / 2)
                    : -(this._leftButton.ActualHeight - (this.ActualHeight - (p.Y + (this._leftThumb.ActualHeight / 2))));
                if (!this.IsSnapToTickEnabled)
                {
                    if (this.IsMoveToPointEnabled && !this.MoveWholeRange)
                    {
                        MoveThumb(this._leftButton, this._centerThumb, -change, this.Orientation, out this._direction);
                        this.ReCalculateRangeSelected(true, false, this._direction);
                        this.CoerceLowerUpperValues();
                    }
                    else if (this.IsMoveToPointEnabled && this.MoveWholeRange)
                    {
                        MoveThumb(this._leftButton, this._rightButton, -change, this.Orientation, out this._direction);
                        this.ReCalculateRangeSelected(true, true, this._direction);
                        this.CoerceLowerUpperValues();
                    }
                }
                else
                {
                    if (this.IsMoveToPointEnabled && !this.MoveWholeRange)
                    {
                        this.JumpToNextTick(Direction.Decrease, ButtonType.BottomLeft, -change, this.LowerValue, true);
                    }
                    else if (this.IsMoveToPointEnabled && this.MoveWholeRange)
                    {
                        this.JumpToNextTick(Direction.Decrease, ButtonType.Both, -change, this.LowerValue, true);
                    }
                }

                if (!this.IsMoveToPointEnabled)
                {
                    this._position = Mouse.GetPosition(this._visualElementsContainer);
                    this._bType = this.MoveWholeRange ? ButtonType.Both : ButtonType.BottomLeft;
                    this._currentpoint = this.Orientation == Orientation.Horizontal ? this._position.X : this._position.Y;
                    this._currenValue = this.LowerValue;
                    this._isInsideRange = false;
                    this._direction = Direction.Decrease;
                    this._timer.Start();
                }
            }
        }

        private void RightButtonMouseDown()
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var p = Mouse.GetPosition(this._visualElementsContainer);
                var change = this.Orientation == Orientation.Horizontal
                    ? this._rightButton.ActualWidth - (this.ActualWidth - (p.X + (this._rightThumb.ActualWidth / 2)))
                    : -(this._rightButton.ActualHeight - (p.Y - (this._rightThumb.ActualHeight / 2)));
                if (!this.IsSnapToTickEnabled)
                {
                    if (this.IsMoveToPointEnabled && !this.MoveWholeRange)
                    {
                        MoveThumb(this._centerThumb, this._rightButton, change, this.Orientation, out this._direction);
                        this.ReCalculateRangeSelected(false, true, this._direction);
                        this.CoerceLowerUpperValues();
                    }
                    else if (this.IsMoveToPointEnabled && this.MoveWholeRange)
                    {
                        MoveThumb(this._leftButton, this._rightButton, change, this.Orientation, out this._direction);
                        this.ReCalculateRangeSelected(true, true, this._direction);
                        this.CoerceLowerUpperValues();
                    }
                }
                else
                {
                    if (this.IsMoveToPointEnabled && !this.MoveWholeRange)
                    {
                        this.JumpToNextTick(Direction.Increase, ButtonType.TopRight, change, this.UpperValue, true);
                    }
                    else if (this.IsMoveToPointEnabled && this.MoveWholeRange)
                    {
                        this.JumpToNextTick(Direction.Increase, ButtonType.Both, change, this.UpperValue, true);
                    }
                }

                if (!this.IsMoveToPointEnabled)
                {
                    this._position = Mouse.GetPosition(this._visualElementsContainer);
                    this._bType = this.MoveWholeRange ? ButtonType.Both : ButtonType.TopRight;
                    this._currentpoint = this.Orientation == Orientation.Horizontal ? this._position.X : this._position.Y;
                    this._currenValue = this.UpperValue;
                    this._direction = Direction.Increase;
                    this._isInsideRange = false;
                    this._timer.Start();
                }
            }
        }

        private void CentralThumbMouseDown()
        {
            if (this.ExtendedMode)
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    this._centerThumbBlocked = true;
                    var p = Mouse.GetPosition(this._visualElementsContainer);
                    var change = this.Orientation == Orientation.Horizontal
                        ? (p.X + (this._leftThumb.ActualWidth / 2) - (this._leftButton.ActualWidth + this._leftThumb.ActualWidth))
                        : -(this.ActualHeight - ((p.Y + (this._leftThumb.ActualHeight / 2)) + this._leftButton.ActualHeight));
                    if (!this.IsSnapToTickEnabled)
                    {
                        if (this.IsMoveToPointEnabled && !this.MoveWholeRange)
                        {
                            MoveThumb(this._leftButton, this._centerThumb, change, this.Orientation, out this._direction);
                            this.ReCalculateRangeSelected(true, false, this._direction);
                            this.CoerceLowerUpperValues();
                        }
                        else if (this.IsMoveToPointEnabled && this.MoveWholeRange)
                        {
                            MoveThumb(this._leftButton, this._rightButton, change, this.Orientation, out this._direction);
                            this.ReCalculateRangeSelected(true, true, this._direction);
                            this.CoerceLowerUpperValues();
                        }
                    }
                    else
                    {
                        if (this.IsMoveToPointEnabled && !this.MoveWholeRange)
                        {
                            this.JumpToNextTick(Direction.Increase, ButtonType.BottomLeft, change, this.LowerValue, true);
                        }
                        else if (this.IsMoveToPointEnabled && this.MoveWholeRange)
                        {
                            this.JumpToNextTick(Direction.Increase, ButtonType.Both, change, this.LowerValue, true);
                        }
                    }

                    if (!this.IsMoveToPointEnabled)
                    {
                        this._position = Mouse.GetPosition(this._visualElementsContainer);
                        this._bType = this.MoveWholeRange ? ButtonType.Both : ButtonType.BottomLeft;
                        this._currentpoint = this.Orientation == Orientation.Horizontal ? this._position.X : this._position.Y;
                        this._currenValue = this.LowerValue;
                        this._direction = Direction.Increase;
                        this._isInsideRange = true;
                        this._timer.Start();
                    }
                }
                else if (Mouse.RightButton == MouseButtonState.Pressed && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    this._centerThumbBlocked = true;
                    var p = Mouse.GetPosition(this._visualElementsContainer);
                    var change = this.Orientation == Orientation.Horizontal
                        ? this.ActualWidth - (p.X + (this._rightThumb.ActualWidth / 2) + this._rightButton.ActualWidth)
                        : -(p.Y + (this._rightThumb.ActualHeight / 2) - (this._rightButton.ActualHeight + this._rightThumb.ActualHeight));
                    if (!this.IsSnapToTickEnabled)
                    {
                        if (this.IsMoveToPointEnabled && !this.MoveWholeRange)
                        {
                            MoveThumb(this._centerThumb, this._rightButton, -change, this.Orientation, out this._direction);
                            this.ReCalculateRangeSelected(false, true, this._direction);
                            this.CoerceLowerUpperValues();
                        }
                        else if (this.IsMoveToPointEnabled && this.MoveWholeRange)
                        {
                            MoveThumb(this._leftButton, this._rightButton, -change, this.Orientation, out this._direction);
                            this.ReCalculateRangeSelected(true, true, this._direction);
                            this.CoerceLowerUpperValues();
                        }
                    }
                    else
                    {
                        if (this.IsMoveToPointEnabled && !this.MoveWholeRange)
                        {
                            this.JumpToNextTick(Direction.Decrease, ButtonType.TopRight, -change, this.UpperValue, true);
                        }
                        else if (this.IsMoveToPointEnabled && this.MoveWholeRange)
                        {
                            this.JumpToNextTick(Direction.Decrease, ButtonType.Both, -change, this.UpperValue, true);
                        }
                    }

                    if (!this.IsMoveToPointEnabled)
                    {
                        this._position = Mouse.GetPosition(this._visualElementsContainer);
                        this._bType = this.MoveWholeRange ? ButtonType.Both : ButtonType.TopRight;
                        this._currentpoint = this.Orientation == Orientation.Horizontal ? this._position.X : this._position.Y;
                        this._currenValue = this.UpperValue;
                        this._direction = Direction.Decrease;
                        this._isInsideRange = true;
                        this._timer.Start();
                    }
                }
            }
        }

        #endregion

        #region Thumb Drag event handlers

        private void LeftThumbDragStart(object sender, DragStartedEventArgs e)
        {
            this._isMoved = true;
            if (this.AutoToolTipPlacement != AutoToolTipPlacement.None)
            {
                if (this._autoToolTip == null)
                {
                    this._autoToolTip = new ToolTip();
                    this._autoToolTip.Placement = PlacementMode.Custom;
                    this._autoToolTip.CustomPopupPlacementCallback = this.PopupPlacementCallback;
                }

                this._autoToolTip.SetValue(ContentControl.ContentTemplateProperty, this.AutoToolTipLowerValueTemplate);
                this._autoToolTip.Content = this.GetToolTipNumber(this.LowerValue);
                this._autoToolTip.PlacementTarget = this._leftThumb;
                this._autoToolTip.IsOpen = true;
            }

            this._basePoint = Mouse.GetPosition(this._container);
            e.RoutedEvent = LowerThumbDragStartedEvent;
            this.RaiseEvent(e);
        }

        private void LeftThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var change = this.Orientation == Orientation.Horizontal ? e.HorizontalChange : e.VerticalChange;
            if (!this.IsSnapToTickEnabled)
            {
                MoveThumb(this._leftButton, this._centerThumb, change, this.Orientation, out this._direction);
                this.ReCalculateRangeSelected(true, false, this._direction);
                this.CoerceLowerUpperValues();
            }
            else
            {
                Direction localDirection;
                var currentPoint = Mouse.GetPosition(this._container);
                if (this.Orientation == Orientation.Horizontal)
                {
                    if (currentPoint.X >= 0 && currentPoint.X < this._container.ActualWidth - (this._rightButton.ActualWidth + this._rightThumb.ActualWidth + this._centerThumb.MinWidth))
                    {
                        localDirection = currentPoint.X > this._basePoint.X ? Direction.Increase : Direction.Decrease;
                        this.JumpToNextTick(localDirection, ButtonType.BottomLeft, change, this.LowerValue, false);
                    }
                }
                else
                {
                    if (currentPoint.Y <= this._container.ActualHeight && currentPoint.Y > this._rightButton.ActualHeight + this._rightThumb.ActualHeight + this._centerThumb.MinHeight)
                    {
                        localDirection = currentPoint.Y < this._basePoint.Y ? Direction.Increase : Direction.Decrease;
                        this.JumpToNextTick(localDirection, ButtonType.BottomLeft, -change, this.LowerValue, false);
                    }
                }
            }

            this._basePoint = Mouse.GetPosition(this._container);
            if (this.AutoToolTipPlacement != AutoToolTipPlacement.None)
            {
                this._autoToolTip.Content = this.GetToolTipNumber(this.LowerValue);
                this.RelocateAutoToolTip();
            }

            e.RoutedEvent = LowerThumbDragDeltaEvent;
            this.RaiseEvent(e);
        }

        private void LeftThumbDragComplete(object sender, DragCompletedEventArgs e)
        {
            if (this._autoToolTip != null)
            {
                this._autoToolTip.IsOpen = false;
                this._autoToolTip = null;
            }

            e.RoutedEvent = LowerThumbDragCompletedEvent;
            this.RaiseEvent(e);
        }

        private void RightThumbDragStart(object sender, DragStartedEventArgs e)
        {
            this._isMoved = true;
            if (this.AutoToolTipPlacement != AutoToolTipPlacement.None)
            {
                if (this._autoToolTip == null)
                {
                    this._autoToolTip = new ToolTip();
                    this._autoToolTip.Placement = PlacementMode.Custom;
                    this._autoToolTip.CustomPopupPlacementCallback = this.PopupPlacementCallback;
                }

                this._autoToolTip.SetValue(ContentControl.ContentTemplateProperty, this.AutoToolTipUpperValueTemplate);
                this._autoToolTip.Content = this.GetToolTipNumber(this.UpperValue);
                this._autoToolTip.PlacementTarget = this._rightThumb;
                this._autoToolTip.IsOpen = true;
            }

            this._basePoint = Mouse.GetPosition(this._container);
            e.RoutedEvent = UpperThumbDragStartedEvent;
            this.RaiseEvent(e);
        }

        private void RightThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var change = this.Orientation == Orientation.Horizontal ? e.HorizontalChange : e.VerticalChange;
            if (!this.IsSnapToTickEnabled)
            {
                MoveThumb(this._centerThumb, this._rightButton, change, this.Orientation, out this._direction);
                this.ReCalculateRangeSelected(false, true, this._direction);
                this.CoerceLowerUpperValues();
            }
            else
            {
                Direction localDirection;
                var currentPoint = Mouse.GetPosition(this._container);
                if (this.Orientation == Orientation.Horizontal)
                {
                    if (currentPoint.X < this._container.ActualWidth && currentPoint.X > this._leftButton.ActualWidth + this._leftThumb.ActualWidth + this._centerThumb.MinWidth)
                    {
                        localDirection = currentPoint.X > this._basePoint.X ? Direction.Increase : Direction.Decrease;
                        this.JumpToNextTick(localDirection, ButtonType.TopRight, change, this.UpperValue, false);
                    }
                }
                else
                {
                    if (currentPoint.Y >= 0 && currentPoint.Y < this._container.ActualHeight - (this._leftButton.ActualHeight + this._leftThumb.ActualHeight + this._centerThumb.MinHeight))
                    {
                        localDirection = currentPoint.Y < this._basePoint.Y ? Direction.Increase : Direction.Decrease;
                        this.JumpToNextTick(localDirection, ButtonType.TopRight, -change, this.UpperValue, false);
                    }
                }

                this._basePoint = Mouse.GetPosition(this._container);
            }

            if (this.AutoToolTipPlacement != AutoToolTipPlacement.None)
            {
                this._autoToolTip.Content = this.GetToolTipNumber(this.UpperValue);
                this.RelocateAutoToolTip();
            }

            e.RoutedEvent = UpperThumbDragDeltaEvent;
            this.RaiseEvent(e);
        }

        private void RightThumbDragComplete(object sender, DragCompletedEventArgs e)
        {
            if (this._autoToolTip != null)
            {
                this._autoToolTip.IsOpen = false;
                this._autoToolTip = null;
            }

            e.RoutedEvent = UpperThumbDragCompletedEvent;
            this.RaiseEvent(e);
        }

        private void CenterThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            this._isMoved = true;
            if (this.AutoToolTipPlacement != AutoToolTipPlacement.None)
            {
                if (this._autoToolTip == null)
                {
                    this._autoToolTip = new ToolTip();
                    this._autoToolTip.Placement = PlacementMode.Custom;
                    this._autoToolTip.CustomPopupPlacementCallback = this.PopupPlacementCallback;
                }

                var autoToolTipRangeValuesTemplate = this.AutoToolTipRangeValuesTemplate;
                this._autoToolTip.SetValue(ContentControl.ContentTemplateProperty, autoToolTipRangeValuesTemplate);
                if (autoToolTipRangeValuesTemplate != null)
                {
                    this._autoToolTip.Content = new RangeSliderAutoTooltipValues(this);
                }
                else
                {
                    this._autoToolTip.Content = this.GetToolTipNumber(this.LowerValue) + " - " + this.GetToolTipNumber(this.UpperValue);
                }

                this._autoToolTip.PlacementTarget = this._centerThumb;
                this._autoToolTip.IsOpen = true;
            }

            this._basePoint = Mouse.GetPosition(this._container);
            e.RoutedEvent = CentralThumbDragStartedEvent;
            this.RaiseEvent(e);
        }

        private void CenterThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!this._centerThumbBlocked)
            {
                var change = this.Orientation == Orientation.Horizontal ? e.HorizontalChange : e.VerticalChange;
                if (!this.IsSnapToTickEnabled)
                {
                    MoveThumb(this._leftButton, this._rightButton, change, this.Orientation, out this._direction);
                    this.ReCalculateRangeSelected(true, true, this._direction);
                    this.CoerceLowerUpperValues();
                }
                else
                {
                    Direction localDirection;
                    var currentPoint = Mouse.GetPosition(this._container);
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        if (currentPoint.X >= 0 && currentPoint.X < this._container.ActualWidth)
                        {
                            localDirection = currentPoint.X > this._basePoint.X ? Direction.Increase : Direction.Decrease;
                            this.JumpToNextTick(localDirection, ButtonType.Both, change, localDirection == Direction.Increase ? this.UpperValue : this.LowerValue, false);
                        }
                    }
                    else
                    {
                        if (currentPoint.Y >= 0 && currentPoint.Y < this._container.ActualHeight)
                        {
                            localDirection = currentPoint.Y < this._basePoint.Y ? Direction.Increase : Direction.Decrease;
                            this.JumpToNextTick(localDirection, ButtonType.Both, -change, localDirection == Direction.Increase ? this.UpperValue : this.LowerValue, false);
                        }
                    }
                }

                this._basePoint = Mouse.GetPosition(this._container);
                if (this.AutoToolTipPlacement != AutoToolTipPlacement.None)
                {
                    if (this._autoToolTip.ContentTemplate != null)
                    {
                        (this._autoToolTip.Content as RangeSliderAutoTooltipValues)?.UpdateValues(this);
                    }
                    else
                    {
                        this._autoToolTip.Content = this.GetToolTipNumber(this.LowerValue) + " - " + this.GetToolTipNumber(this.UpperValue);
                    }

                    this.RelocateAutoToolTip();
                }
            }

            e.RoutedEvent = CentralThumbDragDeltaEvent;
            this.RaiseEvent(e);
        }

        private void CenterThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (this._autoToolTip != null)
            {
                this._autoToolTip.IsOpen = false;
                this._autoToolTip = null;
            }

            e.RoutedEvent = CentralThumbDragCompletedEvent;
            this.RaiseEvent(e);
        }

        #endregion

        #region Helper methods

        private static double GetChangeKeepPositive(double width, double increment)
        {
            return Math.Max(width + increment, 0) - width;
        }

        //Method updates end point, which is needed to correctly compare current position on the thumb with
        //current width of button
        private double UpdateEndPoint(ButtonType type, Direction dir)
        {
            double d = 0;
            //if we increase value 
            if (dir == Direction.Increase)
            {
                if (type == ButtonType.BottomLeft || (type == ButtonType.Both && this._isInsideRange))
                {
                    d = this.Orientation == Orientation.Horizontal ? this._leftButton.ActualWidth + this._leftThumb.ActualWidth : this.ActualHeight - (this._leftButton.ActualHeight + this._leftThumb.ActualHeight);
                }
                else if (type == ButtonType.TopRight || (type == ButtonType.Both && !this._isInsideRange))
                {
                    d = this.Orientation == Orientation.Horizontal ? this.ActualWidth - this._rightButton.ActualWidth : this._rightButton.ActualHeight;
                }
            }
            else if (dir == Direction.Decrease)
            {
                if (type == ButtonType.BottomLeft || (type == ButtonType.Both && !this._isInsideRange))
                {
                    d = this.Orientation == Orientation.Horizontal ? this._leftButton.ActualWidth : this.ActualHeight - this._leftButton.ActualHeight;
                }
                else if (type == ButtonType.TopRight || (type == ButtonType.Both && this._isInsideRange))
                {
                    d = this.Orientation == Orientation.Horizontal ? this.ActualWidth - this._rightButton.ActualWidth - this._rightThumb.ActualWidth : this._rightButton.ActualHeight + this._rightThumb.ActualHeight;
                }
            }

            return d;
        }

        private bool GetResult(double currentPoint, double endPoint, Direction direction)
        {
            if (direction == Direction.Increase)
            {
                return this.Orientation == Orientation.Horizontal && currentPoint > endPoint || this.Orientation == Orientation.Vertical && currentPoint < endPoint;
            }

            return this.Orientation == Orientation.Horizontal && currentPoint < endPoint || this.Orientation == Orientation.Vertical && currentPoint > endPoint;
        }

        //This is timer event, which starts when IsMoveToPoint = false
        //Supports IsSnapToTick option
        private void MoveToNextValue(object sender, EventArgs e)
        {
            //Get updated position of cursor
            this._position = Mouse.GetPosition(this._visualElementsContainer);
            this._currentpoint = this.Orientation == Orientation.Horizontal ? this._position.X : this._position.Y;
            var endpoint = this.UpdateEndPoint(this._bType, this._direction);
            var result = this.GetResult(this._currentpoint, endpoint, this._direction);
            double widthChange;
            if (!this.IsSnapToTickEnabled)
            {
                widthChange = this.SmallChange;
                if (this._tickCount > 5)
                {
                    widthChange = this.LargeChange;
                }

                this._roundToPrecision = true;
                if (!widthChange.ToString(CultureInfo.InvariantCulture).ToLower().Contains("e") &&
                    widthChange.ToString(CultureInfo.InvariantCulture).Contains("."))
                {
                    var array = widthChange.ToString(CultureInfo.InvariantCulture).Split('.');
                    this._precision = array[1].Length;
                }
                else
                {
                    this._precision = 0;
                }

                //Change value sign according to Horizontal or Vertical orientation
                widthChange = this.Orientation == Orientation.Horizontal ? widthChange : -widthChange;
                //Change value sign one more time according to Increase or Decrease direction
                widthChange = this._direction == Direction.Increase ? widthChange : -widthChange;
                if (result)
                {
                    switch (this._bType)
                    {
                        case ButtonType.BottomLeft:
                            MoveThumb(this._leftButton, this._centerThumb, widthChange * this._density, this.Orientation, out this._direction);
                            this.ReCalculateRangeSelected(true, false, this._direction);
                            this.CoerceLowerUpperValues();
                            break;
                        case ButtonType.TopRight:
                            MoveThumb(this._centerThumb, this._rightButton, widthChange * this._density, this.Orientation, out this._direction);
                            this.ReCalculateRangeSelected(false, true, this._direction);
                            this.CoerceLowerUpperValues();
                            break;
                        case ButtonType.Both:
                            MoveThumb(this._leftButton, this._rightButton, widthChange * this._density, this.Orientation, out this._direction);
                            this.ReCalculateRangeSelected(true, true, this._direction);
                            this.CoerceLowerUpperValues();
                            break;
                    }
                }
            }
            else
            {
                //Get the difference between current and next value
                widthChange = this.CalculateNextTick(this._direction, this._currenValue, 0, true);
                var value = widthChange;
                //Change value sign according to Horizontal or Vertical orientation
                widthChange = this.Orientation == Orientation.Horizontal ? widthChange : -widthChange;
                if (this._direction == Direction.Increase)
                {
                    if (result)
                    {
                        switch (this._bType)
                        {
                            case ButtonType.BottomLeft:
                                MoveThumb(this._leftButton, this._centerThumb, widthChange * this._density, this.Orientation);
                                this.ReCalculateRangeSelected(true, false, this.LowerValue + value, this._direction);
                                this.CoerceLowerUpperValues();
                                break;
                            case ButtonType.TopRight:
                                MoveThumb(this._centerThumb, this._rightButton, widthChange * this._density, this.Orientation);
                                this.ReCalculateRangeSelected(false, true, this.UpperValue + value, this._direction);
                                this.CoerceLowerUpperValues();
                                break;
                            case ButtonType.Both:
                                MoveThumb(this._leftButton, this._rightButton, widthChange * this._density, this.Orientation);
                                this.ReCalculateRangeSelected(this.LowerValue + value, this.UpperValue + value, this._direction);
                                this.CoerceLowerUpperValues();
                                break;
                        }
                    }
                }
                else if (this._direction == Direction.Decrease)
                {
                    if (result)
                    {
                        switch (this._bType)
                        {
                            case ButtonType.BottomLeft:
                                MoveThumb(this._leftButton, this._centerThumb, -widthChange * this._density, this.Orientation);
                                this.ReCalculateRangeSelected(true, false, this.LowerValue - value, this._direction);
                                this.CoerceLowerUpperValues();
                                break;
                            case ButtonType.TopRight:
                                MoveThumb(this._centerThumb, this._rightButton, -widthChange * this._density, this.Orientation);
                                this.ReCalculateRangeSelected(false, true, this.UpperValue - value, this._direction);
                                this.CoerceLowerUpperValues();
                                break;
                            case ButtonType.Both:
                                MoveThumb(this._leftButton, this._rightButton, -widthChange * this._density, this.Orientation);
                                this.ReCalculateRangeSelected(this.LowerValue - value, this.UpperValue - value, this._direction);
                                this.CoerceLowerUpperValues();
                                break;
                        }
                    }
                }
            }

            this._tickCount++;
        }

        //Helper method to handle snapToTick scenario and decrease amount of code
        private void SnapToTickHandle(ButtonType type, Direction direction, double difference)
        {
            var value = difference;
            //change sign of "difference" variable because Horizontal and Vertical orientations has are different directions
            difference = this.Orientation == Orientation.Horizontal ? difference : -difference;
            if (direction == Direction.Increase)
            {
                switch (type)
                {
                    case ButtonType.TopRight:
                        if (this.UpperValue < this.Maximum)
                        {
                            MoveThumb(this._centerThumb, this._rightButton, difference * this._density, this.Orientation);
                            this.ReCalculateRangeSelected(false, true, this.UpperValue + value, direction);
                            this.CoerceLowerUpperValues();
                        }

                        break;
                    case ButtonType.BottomLeft:
                        if (this.LowerValue < this.UpperValue - this.MinRange)
                        {
                            MoveThumb(this._leftButton, this._centerThumb, difference * this._density, this.Orientation);
                            this.ReCalculateRangeSelected(true, false, this.LowerValue + value, direction);
                            this.CoerceLowerUpperValues();
                        }

                        break;
                    case ButtonType.Both:
                        if (this.UpperValue < this.Maximum)
                        {
                            MoveThumb(this._leftButton, this._rightButton, difference * this._density, this.Orientation);
                            this.ReCalculateRangeSelected(this.LowerValue + value, this.UpperValue + value, direction);
                            this.CoerceLowerUpperValues();
                        }

                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case ButtonType.TopRight:
                        if (this.UpperValue > this.LowerValue + this.MinRange)
                        {
                            MoveThumb(this._centerThumb, this._rightButton, -difference * this._density, this.Orientation);
                            this.ReCalculateRangeSelected(false, true, this.UpperValue - value, direction);
                            this.CoerceLowerUpperValues();
                        }

                        break;
                    case ButtonType.BottomLeft:
                        if (this.LowerValue > this.Minimum)
                        {
                            MoveThumb(this._leftButton, this._centerThumb, -difference * this._density, this.Orientation);
                            this.ReCalculateRangeSelected(true, false, this.LowerValue - value, direction);
                            this.CoerceLowerUpperValues();
                        }

                        break;
                    case ButtonType.Both:
                        if (this.LowerValue > this.Minimum)
                        {
                            MoveThumb(this._leftButton, this._rightButton, -difference * this._density, this.Orientation);
                            this.ReCalculateRangeSelected(this.LowerValue - value, this.UpperValue - value, direction);
                            this.CoerceLowerUpperValues();
                        }

                        break;
                }
            }
        }

        //Calculating next value for Tick
        private double CalculateNextTick(Direction direction, double checkingValue, double distance, bool moveDirectlyToNextTick)
        {
            var checkingValuePos = checkingValue - this.Minimum;
            if (!this.IsMoveToPointEnabled)
            {
                //Check if current value is exactly Tick value or it situated between Ticks
                var checkingValueChanged = checkingValuePos; // + distance; // <-- introduced by @drayde with #2006 but it breaks the left thumb movement #2880
                var x = checkingValueChanged / this.TickFrequency;
                if (!this.IsDoubleCloseToInt(x))
                {
                    distance = this.TickFrequency * (int)x;
                    if (direction == Direction.Increase)
                    {
                        distance += this.TickFrequency;
                    }

                    distance = (distance - Math.Abs(checkingValuePos));
                    this._currenValue = 0;
                    return Math.Abs(distance);
                }
            }

            //If we need move directly to next tick without calculating the difference between ticks
            //Use when MoveToPoint disabled
            if (moveDirectlyToNextTick)
            {
                distance = this.TickFrequency;
            }
            //If current value == tick (Value is divisible)
            else
            {
                //current value in units (exactly in the place under cursor)
                var currentValue = checkingValuePos + (distance / this._density);
                var x = currentValue / this.TickFrequency;
                if (direction == Direction.Increase)
                {
                    var nextvalue = x.ToString(CultureInfo.InvariantCulture).ToLower().Contains("e+")
                        ? (x * this.TickFrequency) + this.TickFrequency
                        : ((int)x * this.TickFrequency) + this.TickFrequency;

                    distance = (nextvalue - Math.Abs(checkingValuePos));
                }
                else
                {
                    var previousValue = x.ToString(CultureInfo.InvariantCulture).ToLower().Contains("e+")
                        ? x * this.TickFrequency
                        : (int)x * this.TickFrequency;
                    distance = (Math.Abs(checkingValuePos) - previousValue);
                }
            }

            //return absolute value without sign not to depend on it if value is negative 
            //(could cause bugs in calcutaions if return not absolute value)
            return Math.Abs(distance);
        }

        //Move thumb to next calculated Tick and update corresponding value
        private void JumpToNextTick(Direction direction, ButtonType type, double distance, double checkingValue, bool jumpDirectlyToTick)
        {
            //find the difference between current value and next value
            var difference = this.CalculateNextTick(direction, checkingValue, distance, false);
            var p = Mouse.GetPosition(this._visualElementsContainer);
            var pos = this.Orientation == Orientation.Horizontal ? p.X : p.Y;
            var widthHeight = this.Orientation == Orientation.Horizontal ? this.ActualWidth : this.ActualHeight;
            var tickIntervalInPixels = direction == Direction.Increase
                ? this.TickFrequency * this._density
                : -this.TickFrequency * this._density;

            if (jumpDirectlyToTick)
            {
                this.SnapToTickHandle(type, direction, difference);
            }
            else
            {
                if (direction == Direction.Increase)
                {
                    if (!this.IsDoubleCloseToInt(checkingValue / this.TickFrequency))
                    {
                        if (distance > (difference * this._density) / 2 || (distance >= (widthHeight - pos) || distance >= pos))
                        {
                            this.SnapToTickHandle(type, direction, difference);
                        }
                    }
                    else
                    {
                        if ((distance > tickIntervalInPixels / 2) || (distance >= (widthHeight - pos) || distance >= pos))
                        {
                            this.SnapToTickHandle(type, direction, difference);
                        }
                    }
                }
                else
                {
                    if (!this.IsDoubleCloseToInt(checkingValue / this.TickFrequency))
                    {
                        if ((distance <= -(difference * this._density) / 2) || (this.UpperValue - this.LowerValue) < difference)
                        {
                            this.SnapToTickHandle(type, direction, difference);
                        }
                    }
                    else
                    {
                        if (distance < tickIntervalInPixels / 2 || (this.UpperValue - this.LowerValue) < difference)
                        {
                            this.SnapToTickHandle(type, direction, difference);
                        }
                    }
                }
            }
        }

        //Change AutotoolTipPosition to move sync with Thumb
        private void RelocateAutoToolTip()
        {
            var offset = this._autoToolTip.HorizontalOffset;
            this._autoToolTip.HorizontalOffset = offset + 0.001;
            this._autoToolTip.HorizontalOffset = offset;
        }

        //CHeck if two doubles approximately equals
        private bool ApproximatelyEquals(double value1, double value2)
        {
            return Math.Abs(value1 - value2) <= Epsilon;
        }

        private bool IsDoubleCloseToInt(double val)
        {
            return this.ApproximatelyEquals(Math.Abs(val - Math.Round(val)), 0);
        }

        internal string GetToolTipNumber(double value)
        {
            var numberFormatInfo = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            numberFormatInfo.NumberDecimalDigits = this.AutoToolTipPrecision;
            return value.ToString("N", numberFormatInfo);
        }

        //CustomPopupPlacement callback for placing autotooltip int TopLeft or BottomRight position
        private CustomPopupPlacement[] PopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
        {
            switch (this.AutoToolTipPlacement)
            {
                case AutoToolTipPlacement.TopLeft:
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        // Place popup at top of thumb
                        return new CustomPopupPlacement[] { new CustomPopupPlacement(new Point((targetSize.Width - popupSize.Width) * 0.5, -popupSize.Height), PopupPrimaryAxis.Horizontal) };
                    }

                    // Place popup at left of thumb 
                    return new CustomPopupPlacement[] { new CustomPopupPlacement(new Point(-popupSize.Width, (targetSize.Height - popupSize.Height) * 0.5), PopupPrimaryAxis.Vertical) };

                case AutoToolTipPlacement.BottomRight:
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        // Place popup at bottom of thumb 
                        return new CustomPopupPlacement[] { new CustomPopupPlacement(new Point((targetSize.Width - popupSize.Width) * 0.5, targetSize.Height), PopupPrimaryAxis.Horizontal) };
                    }

                    // Place popup at right of thumb 
                    return new CustomPopupPlacement[] { new CustomPopupPlacement(new Point(targetSize.Width, (targetSize.Height - popupSize.Height) * 0.5), PopupPrimaryAxis.Vertical) };

                default:
                    return new CustomPopupPlacement[] { };
            }
        }

        #endregion

        #region Validation methods

        private static bool IsValidDoubleValue(object value)
        {
            return value is double doubleValue && IsValidDouble(doubleValue);
        }

        private static bool IsValidDouble(double d)
        {
            return !double.IsNaN(d) && !double.IsInfinity(d);
        }

        private static bool IsValidPrecision(object value)
        {
            return value is int intValue && intValue >= 0;
        }

        #endregion

        #region Coerce callbacks

        private static object CoerceMinimum(DependencyObject d, object basevalue)
        {
            var rangeSlider = (RangeSlider)d;
            var value = (double)basevalue;

            if (value > rangeSlider.Maximum)
            {
                return rangeSlider.Maximum;
            }

            return basevalue;
        }

        private static object CoerceMaximum(DependencyObject d, object basevalue)
        {
            var rangeSlider = (RangeSlider)d;
            var value = (double)basevalue;

            if (value < rangeSlider.Minimum)
            {
                return rangeSlider.Minimum;
            }

            return basevalue;
        }

        #endregion

        #region PropertyChanged CallBacks

        private static void MaxPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            dependencyObject.CoerceValue(MaximumProperty);
            dependencyObject.CoerceValue(MinimumProperty);
            dependencyObject.CoerceValue(UpperValueProperty);
        }

        private static void MinPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            dependencyObject.CoerceValue(MinimumProperty);
            dependencyObject.CoerceValue(MaximumProperty);
            dependencyObject.CoerceValue(LowerValueProperty);
        }

        private static void IntervalChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var rs = (RangeSlider)dependencyObject;
            rs._timer.Interval = TimeSpan.FromMilliseconds((int)e.NewValue);
        }

        //Raises all value changes events
        private static void RaiseValueChangedEvents(DependencyObject dependencyObject, bool lowerValueReCalculated = true, bool upperValueReCalculated = true)
        {
            var slider = (RangeSlider)dependencyObject;
            var lowerValueEquals = Equals(slider._oldLower, slider.LowerValue);
            var upperValueEquals = Equals(slider._oldUpper, slider.UpperValue);

            if ((lowerValueReCalculated || upperValueReCalculated) && (!lowerValueEquals || !upperValueEquals))
            {
                slider.RaiseEvent(new RangeSelectionChangedEventArgs<double>(slider._oldLower, slider.LowerValue, slider._oldUpper, slider.UpperValue, RangeSelectionChangedEvent));
            }

            if (lowerValueReCalculated && !lowerValueEquals)
            {
                slider.RaiseEvent(new RoutedPropertyChangedEventArgs<double>(slider._oldLower, slider.LowerValue, LowerValueChangedEvent));
            }

            if (upperValueReCalculated && !upperValueEquals)
            {
                slider.RaiseEvent(new RoutedPropertyChangedEventArgs<double>(slider._oldUpper, slider.UpperValue, UpperValueChangedEvent));
            }
        }

        #endregion

        //enum for understanding which repeat button (left, right or both) is changing its width or height
        private enum ButtonType
        {
            BottomLeft,
            TopRight,
            Both
        }

        //enum for understanding current thumb moving direction 
        private enum Direction
        {
            Increase,
            Decrease
        }
    }

    public class RangeSliderAutoTooltipValues : INotifyPropertyChanged
    {
        private string lowerValue;

        /// <summary>
        /// Gets the lower value of the range selection.
        /// </summary>
        public string LowerValue
        {
            get => this.lowerValue;
            set
            {
                if (value.Equals(this.lowerValue)) return;
                this.lowerValue = value;
                this.OnPropertyChanged();
            }
        }

        private string upperValue;

        /// <summary>
        /// Gets the upper value of the range selection.
        /// </summary>
        public string UpperValue
        {
            get => this.upperValue;
            set
            {
                if (value.Equals(this.upperValue)) return;
                this.upperValue = value;
                this.OnPropertyChanged();
            }
        }

        internal RangeSliderAutoTooltipValues(RangeSlider rangeSlider)
        {
            this.UpdateValues(rangeSlider);
        }

        internal void UpdateValues(RangeSlider rangeSlider)
        {
            this.LowerValue = rangeSlider.GetToolTipNumber(rangeSlider.LowerValue);
            this.UpperValue = rangeSlider.GetToolTipNumber(rangeSlider.UpperValue);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.LowerValue + " - " + this.UpperValue;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}