using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace MahApps.Metro.Controls
{
    public delegate void RangeSelectionChangedEventHandler(object sender, RangeSelectionChangedEventArgs e);

    public delegate void RangeParameterChangedEventHandler(object sender, RangeParameterChangedEventArgs e);

    /// <summary>
    /// A slider control with the ability to select a range between two values.
    /// </summary>
    [DefaultEvent("RangeSelectionChanged"),
     TemplatePart(Name = "PART_Container", Type = typeof(StackPanel)),
     TemplatePart(Name = "PART_RangeSliderContainer", Type = typeof(StackPanel)),
     TemplatePart(Name = "PART_LeftEdge", Type = typeof(RepeatButton)),
     TemplatePart(Name = "PART_RightEdge", Type = typeof(RepeatButton)),
     TemplatePart(Name = "PART_LeftThumb", Type = typeof(Thumb)),
     TemplatePart(Name = "PART_MiddleThumb", Type = typeof(Thumb)),
     TemplatePart(Name = "PART_PART_TopTick", Type = typeof(TickBar)),
     TemplatePart(Name = "PART_PART_BottomTick", Type = typeof(TickBar)),
     TemplatePart(Name = "PART_RightThumb", Type = typeof(Thumb))]
    public class RangeSlider : RangeBase
    {
        #region Routed UI commands

        public static RoutedUICommand MoveBack = new RoutedUICommand("MoveBack", "MoveBack", typeof(RangeSlider), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.B, ModifierKeys.Control) }));
        public static RoutedUICommand MoveForward = new RoutedUICommand("MoveForward", "MoveForward", typeof(RangeSlider), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F, ModifierKeys.Control) }));
        public static RoutedUICommand MoveAllForward = new RoutedUICommand("MoveAllForward", "MoveAllForward", typeof(RangeSlider), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F, ModifierKeys.Alt) }));
        public static RoutedUICommand MoveAllBack = new RoutedUICommand("MoveAllBack", "MoveAllBack", typeof(RangeSlider), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.B, ModifierKeys.Alt) }));

        #endregion

        #region Routed events

        public static readonly RoutedEvent RangeSelectionChangedEvent =
            EventManager.RegisterRoutedEvent("RangeSelectionChanged", RoutingStrategy.Bubble,
                                             typeof(RangeSelectionChangedEventHandler), typeof(RangeSlider));

        public static readonly RoutedEvent LowerValueChangedEvent =
            EventManager.RegisterRoutedEvent("LowerValueChanged", RoutingStrategy.Bubble,
                                             typeof(RangeParameterChangedEventHandler), typeof(RangeSlider));

        public static readonly RoutedEvent UpperValueChangedEvent =
            EventManager.RegisterRoutedEvent("UpperValueChanged", RoutingStrategy.Bubble,
                                             typeof(RangeParameterChangedEventHandler), typeof(RangeSlider));

        public static readonly RoutedEvent LowerThumbDragStartedEvent =
            EventManager.RegisterRoutedEvent("LowerThumbDragStarted", RoutingStrategy.Bubble,
                                             typeof(DragStartedEventHandler), typeof(RangeSlider));

        public static readonly RoutedEvent LowerThumbDragCompletedEvent =
            EventManager.RegisterRoutedEvent("LowerThumbDragCompleted", RoutingStrategy.Bubble,
                                             typeof(DragCompletedEventHandler), typeof(RangeSlider));

        public static readonly RoutedEvent UpperThumbDragStartedEvent =
            EventManager.RegisterRoutedEvent("UpperThumbDragStarted", RoutingStrategy.Bubble,
                                             typeof(DragStartedEventHandler), typeof(RangeSlider));

        public static readonly RoutedEvent UpperThumbDragCompletedEvent =
            EventManager.RegisterRoutedEvent("UpperThumbDragCompleted", RoutingStrategy.Bubble,
                                             typeof(DragCompletedEventHandler), typeof(RangeSlider));

        public static readonly RoutedEvent CentralThumbDragStartedEvent =
            EventManager.RegisterRoutedEvent("CentralThumbDragStarted", RoutingStrategy.Bubble,
                                             typeof(DragStartedEventHandler), typeof(RangeSlider));

        public static readonly RoutedEvent CentralThumbDragCompletedEvent =
            EventManager.RegisterRoutedEvent("CentralThumbDragCompleted", RoutingStrategy.Bubble,
                                             typeof(DragCompletedEventHandler), typeof(RangeSlider));

        public static readonly RoutedEvent LowerThumbDragDeltaEvent =
            EventManager.RegisterRoutedEvent("LowerThumbDragDelta", RoutingStrategy.Bubble,
                                             typeof(DragDeltaEventHandler), typeof(RangeSlider));

        public static readonly RoutedEvent UpperThumbDragDeltaEvent =
            EventManager.RegisterRoutedEvent("UpperThumbDragDelta", RoutingStrategy.Bubble,
                                             typeof(DragDeltaEventHandler), typeof(RangeSlider));

        public static readonly RoutedEvent CentralThumbDragDeltaEvent =
            EventManager.RegisterRoutedEvent("CentralThumbDragDelta", RoutingStrategy.Bubble,
                                             typeof(DragDeltaEventHandler), typeof(RangeSlider));

        #endregion

        #region Event handlers

        public event RangeSelectionChangedEventHandler RangeSelectionChanged
        {
            add { AddHandler(RangeSelectionChangedEvent, value); }
            remove { RemoveHandler(RangeSelectionChangedEvent, value); }
        }

        public event RangeParameterChangedEventHandler LowerValueChanged
        {
            add { AddHandler(LowerValueChangedEvent, value); }
            remove { RemoveHandler(LowerValueChangedEvent, value); }
        }

        public event RangeParameterChangedEventHandler UpperValueChanged
        {
            add { AddHandler(UpperValueChangedEvent, value); }
            remove { RemoveHandler(UpperValueChangedEvent, value); }
        }

        public event DragStartedEventHandler LowerThumbDragStarted
        {
            add { AddHandler(LowerThumbDragStartedEvent, value); }
            remove { RemoveHandler(LowerThumbDragStartedEvent, value); }
        }

        public event DragCompletedEventHandler LowerThumbDragCompleted
        {
            add { AddHandler(LowerThumbDragCompletedEvent, value); }
            remove { RemoveHandler(LowerThumbDragCompletedEvent, value); }
        }

        public event DragStartedEventHandler UpperThumbDragStarted
        {
            add { AddHandler(UpperThumbDragStartedEvent, value); }
            remove { RemoveHandler(UpperThumbDragStartedEvent, value); }
        }

        public event DragCompletedEventHandler UpperThumbDragCompleted
        {
            add { AddHandler(UpperThumbDragCompletedEvent, value); }
            remove { RemoveHandler(UpperThumbDragCompletedEvent, value); }
        }

        public event DragStartedEventHandler CentralThumbDragStarted
        {
            add { AddHandler(CentralThumbDragStartedEvent, value); }
            remove { RemoveHandler(CentralThumbDragStartedEvent, value); }
        }

        public event DragCompletedEventHandler CentralThumbDragCompleted
        {
            add { AddHandler(CentralThumbDragCompletedEvent, value); }
            remove { RemoveHandler(CentralThumbDragCompletedEvent, value); }
        }

        public event DragDeltaEventHandler LowerThumbDragDelta
        {
            add { AddHandler(LowerThumbDragDeltaEvent, value); }
            remove { RemoveHandler(LowerThumbDragDeltaEvent, value); }
        }

        public event DragDeltaEventHandler UpperThumbDragDelta
        {
            add { AddHandler(UpperThumbDragDeltaEvent, value); }
            remove { RemoveHandler(UpperThumbDragDeltaEvent, value); }
        }

        public event DragDeltaEventHandler CentralThumbDragDelta
        {
            add { AddHandler(CentralThumbDragDeltaEvent, value); }
            remove { RemoveHandler(CentralThumbDragDeltaEvent, value); }
        }

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register("UpperValue", typeof(Double), typeof(RangeSlider),
                                        new FrameworkPropertyMetadata((Double)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, RangesChanged, CoerceUpperValue));

        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register("LowerValue", typeof(Double), typeof(RangeSlider),
                                        new FrameworkPropertyMetadata((Double)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, RangesChanged, CoerceLowerValue));

        public static readonly DependencyProperty MinRangeProperty =
            DependencyProperty.Register("MinRange", typeof(Double), typeof(RangeSlider),
                                        new FrameworkPropertyMetadata((Double)0, MinRangeChanged, CoerceMinRange), IsValidMinRange);

        public static readonly DependencyProperty MinRangeWidthProperty =
            DependencyProperty.Register("MinRangeWidth", typeof(Double), typeof(RangeSlider),
                                        new FrameworkPropertyMetadata(30.0, MinRangeWidthChanged, CoerceMinRangeWidth), IsValidMinRange);

        public static readonly DependencyProperty MoveWholeRangeProperty =
            DependencyProperty.Register("MoveWholeRange", typeof(Boolean), typeof(RangeSlider),
                                        new PropertyMetadata(false));

        public static readonly DependencyProperty ExtendedModeProperty =
            DependencyProperty.Register("ExtendedMode", typeof(Boolean), typeof(RangeSlider),
                                        new PropertyMetadata(false));

        public static readonly DependencyProperty IsSnapToTickEnabledProperty =
            DependencyProperty.Register("IsSnapToTickEnabled", typeof(Boolean), typeof(RangeSlider),
                                        new PropertyMetadata(false));

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(RangeSlider),
                                        new FrameworkPropertyMetadata(Orientation.Horizontal));

        public static readonly DependencyProperty TickFrequencyProperty =
            DependencyProperty.Register("TickFrequency", typeof(Double), typeof(RangeSlider),
                                        new FrameworkPropertyMetadata(1.0), IsValidTickFrequency);

        public static readonly DependencyProperty IsMoveToPointEnabledProperty =
            DependencyProperty.Register("IsMoveToPointEnabled", typeof(Boolean), typeof(RangeSlider),
                                        new PropertyMetadata(false));

        public static readonly DependencyProperty TickPlacementProperty =
            DependencyProperty.Register("TickPlacement", typeof(TickPlacement), typeof(RangeSlider),
                                        new FrameworkPropertyMetadata(TickPlacement.None));

        public static readonly DependencyProperty AutoToolTipPlacementProperty =
            DependencyProperty.Register("AutoToolTipPlacement", typeof(AutoToolTipPlacement), typeof(RangeSlider),
                                        new FrameworkPropertyMetadata(AutoToolTipPlacement.None));

        public static readonly DependencyProperty AutoToolTipPrecisionProperty =
            DependencyProperty.Register("AutoToolTipPrecision", typeof(Int32), typeof(RangeSlider),
                                        new FrameworkPropertyMetadata(0), IsValidPrecision);

        public static readonly DependencyProperty AutoToolTipTextConverterProperty =
            DependencyProperty.Register("AutoToolTipTextConverter", typeof(IValueConverter), typeof(RangeSlider),
                                        new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(Int32), typeof(RangeSlider),
                                        new FrameworkPropertyMetadata(100, IntervalChangedCallback), IsValidPrecision);

        /// <summary>
        /// Get/sets value how fast thumbs will move when user press on left/right/central with left mouse button (IsMoveToPoint must be set to FALSE)
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public Int32 Interval
        {
            get { return (Int32)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        /// <summary>
        /// Get/sets precision of the value, which displaying inside AutotToolTip
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public Int32 AutoToolTipPrecision
        {
            get { return (Int32)GetValue(AutoToolTipPrecisionProperty); }
            set { SetValue(AutoToolTipPrecisionProperty, value); }
        }

        /// <summary>
        /// Get/sets the converter for the tooltip text
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public IValueConverter AutoToolTipTextConverter
        {
            get { return (IValueConverter)GetValue(AutoToolTipTextConverterProperty); }
            set { SetValue(AutoToolTipTextConverterProperty, value); }
        }

        /// <summary>
        /// Get/sets tooltip, which will show while dragging thumbs and display currect value
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public AutoToolTipPlacement AutoToolTipPlacement
        {
            get { return (AutoToolTipPlacement)GetValue(AutoToolTipPlacementProperty); }
            set { SetValue(AutoToolTipPlacementProperty, value); }
        }

        /// <summary>
        /// Get/sets tick placement position
        /// </summary>
        [Bindable(true), Category("Common")]
        public TickPlacement TickPlacement
        {
            get { return (TickPlacement)GetValue(TickPlacementProperty); }
            set { SetValue(TickPlacementProperty, value); }
        }

        /// <summary>
        /// Get/sets IsMoveToPoint feature which will enable/disable moving to exact point inside control when user clicked on it
        /// </summary>
        [Bindable(true), Category("Common")]
        public Boolean IsMoveToPointEnabled
        {
            get { return (Boolean)GetValue(IsMoveToPointEnabledProperty); }
            set { SetValue(IsMoveToPointEnabledProperty, value); }
        }

        /// <summary>
        /// Get/sets tickFrequency
        /// </summary>
        [Bindable(true), Category("Common")]
        public Double TickFrequency
        {
            get { return (Double)GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }

        /// <summary>
        /// Get/sets orientation of range slider
        /// </summary>
        [Bindable(true), Category("Common")]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Get/sets whether possibility to make manipulations inside range with left/right mouse buttons + cotrol button
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public Boolean IsSnapToTickEnabled
        {
            get { return (Boolean)GetValue(IsSnapToTickEnabledProperty); }
            set { SetValue(IsSnapToTickEnabledProperty, value); }
        }

        /// <summary>
        /// Get/sets whether possibility to make manipulations inside range with left/right mouse buttons + cotrol button
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public Boolean ExtendedMode
        {
            get { return (Boolean)GetValue(ExtendedModeProperty); }
            set { SetValue(ExtendedModeProperty, value); }
        }

        /// <summary>
        /// Get/sets whether whole range will be moved when press on right/left/central part of control
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public Boolean MoveWholeRange
        {
            get { return (Boolean)GetValue(MoveWholeRangeProperty); }
            set { SetValue(MoveWholeRangeProperty, value); }
        }

        /// <summary>
        /// Get/sets the minimal distance between two thumbs.
        /// </summary>
        [Bindable(true), Category("Common")]
        public Double MinRangeWidth
        {
            get { return (Double)GetValue(MinRangeWidthProperty); }
            set { SetValue(MinRangeWidthProperty, value); }
        }

        /// <summary>
        /// Get/sets the beginning of the range selection.
        /// </summary>
        [Bindable(true), Category("Common")]
        public Double LowerValue
        {
            get { return (Double)GetValue(LowerValueProperty); }
            set { SetValue(LowerValueProperty, value); }
        }

        /// <summary>
        /// Get/sets the end of the range selection.
        /// </summary>
        [Bindable(true), Category("Common")]
        public Double UpperValue
        {
            get { return (Double)GetValue(UpperValueProperty); }
            set { SetValue(UpperValueProperty, value); }
        }

        /// <summary>
        /// Get/sets the minimum range that can be selected.
        /// </summary>
        [Bindable(true), Category("Common")]
        public Double MinRange
        {
            get { return (Double)GetValue(MinRangeProperty); }
            set { SetValue(MinRangeProperty, value); }
        }

        #endregion

        #region Variables

        private const double Epsilon = 0.00000153;

        private Boolean _internalUpdate;
        private Thumb _centerThumb;
        private Thumb _leftThumb;
        private Thumb _rightThumb;
        private RepeatButton _leftButton;
        private RepeatButton _rightButton;
        private StackPanel _visualElementsContainer;
        private StackPanel _container;
        private Double _movableWidth;
        private readonly DispatcherTimer _timer;

        private uint _tickCount;
        private Double _currentpoint;
        private Boolean _isInsideRange;
        private Boolean _centerThumbBlocked;
        private Direction _direction;
        private ButtonType _bType;
        private Point _position;
        private Point _basePoint;
        private Double _currenValue;
        private Double _density;
        private ToolTip _autoToolTip;
        private Double _oldLower;
        private Double _oldUpper;
        private Boolean _isMoved;
        private Boolean _roundToPrecision;
        private Int32 _precision;
        private readonly PropertyChangeNotifier actualWidthPropertyChangeNotifier;
        private readonly PropertyChangeNotifier actualHeightPropertyChangeNotifier;

        #endregion

        public double MovableRange
        {
            get { return Maximum - Minimum - MinRange; }
        }

        public RangeSlider()
        {
            CommandBindings.Add(new CommandBinding(MoveBack, MoveBackHandler));
            CommandBindings.Add(new CommandBinding(MoveForward, MoveForwardHandler));
            CommandBindings.Add(new CommandBinding(MoveAllForward, MoveAllForwardHandler));
            CommandBindings.Add(new CommandBinding(MoveAllBack, MoveAllBackHandler));

            this.actualWidthPropertyChangeNotifier = new PropertyChangeNotifier(this, RangeSlider.ActualWidthProperty);
            this.actualWidthPropertyChangeNotifier.ValueChanged += (s, e) => ReCalculateSize();
            this.actualHeightPropertyChangeNotifier = new PropertyChangeNotifier(this, RangeSlider.ActualHeightProperty);
            this.actualHeightPropertyChangeNotifier.ValueChanged += (s, e) => ReCalculateSize();

            _timer = new DispatcherTimer();
            _timer.Tick += MoveToNextValue;
            _timer.Interval = TimeSpan.FromMilliseconds(Interval);
        }

        static RangeSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(typeof(RangeSlider)));
            MinimumProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure, MinPropertyChangedCallback, CoerceMinimum));
            MaximumProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsMeasure, MaxPropertyChangedCallback, CoerceMaximum));
        }

        /// <summary>
        /// Responds to a change in the value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum"/> property.
        /// </summary>
        /// <param name="oldMinimum">The old value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum"/> property.</param><param name="newMinimum">The new value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum"/> property.</param>
        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            ReCalculateSize();
        }

        /// <summary>
        /// Responds to a change in the value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum"/> property.
        /// </summary>
        /// <param name="oldMaximum">The old value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum"/> property.</param><param name="newMaximum">The new value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum"/> property.</param>
        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            ReCalculateSize();
        }

        private void MoveAllBackHandler(object sender, ExecutedRoutedEventArgs e)
        {
            ResetSelection(true);
        }

        private void MoveAllForwardHandler(object sender, ExecutedRoutedEventArgs e)
        {
            ResetSelection(false);
        }

        private void MoveBackHandler(object sender, ExecutedRoutedEventArgs e)
        {
            MoveSelection(true);
        }

        private void MoveForwardHandler(object sender, ExecutedRoutedEventArgs e)
        {
            MoveSelection(false);
        }

        private static void MoveThumb(FrameworkElement x, FrameworkElement y, double change, Orientation orientation)
        {
            var direction = Direction.Increase;
            MoveThumb(x, y, change, orientation, out direction);
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
            if (!Double.IsNaN(x.Width) && !Double.IsNaN(y.Width))
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
            if (!Double.IsNaN(x.Height) && !Double.IsNaN(y.Height))
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
            if (_leftButton != null && _rightButton != null && _centerThumb != null)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    _movableWidth = Math.Max(ActualWidth - _rightThumb.ActualWidth - _leftThumb.ActualWidth - MinRangeWidth, 1);
                    if (MovableRange <= 0)
                    {
                        _leftButton.Width = Double.NaN;
                        _rightButton.Width = Double.NaN;
                    }
                    else
                    {
                        _leftButton.Width = Math.Max(_movableWidth * (LowerValue - Minimum) / MovableRange, 0);
                        _rightButton.Width = Math.Max(_movableWidth * (Maximum - UpperValue) / MovableRange, 0);
                    }

                    if (IsValidDouble(_rightButton.Width) && IsValidDouble(_leftButton.Width))
                    {
                        _centerThumb.Width = Math.Max(ActualWidth - (_leftButton.Width + _rightButton.Width + _rightThumb.ActualWidth + _leftThumb.ActualWidth), 0);
                    }
                    else
                    {
                        _centerThumb.Width = Math.Max(ActualWidth - (_rightThumb.ActualWidth + _leftThumb.ActualWidth), 0);
                    }
                }
                else if (Orientation == Orientation.Vertical)
                {
                    _movableWidth = Math.Max(ActualHeight - _rightThumb.ActualHeight - _leftThumb.ActualHeight - MinRangeWidth, 1);
                    if (MovableRange <= 0)
                    {
                        _leftButton.Height = Double.NaN;
                        _rightButton.Height = Double.NaN;
                    }
                    else
                    {
                        _leftButton.Height = Math.Max(_movableWidth * (LowerValue - Minimum) / MovableRange, 0);
                        _rightButton.Height = Math.Max(_movableWidth * (Maximum - UpperValue) / MovableRange, 0);
                    }

                    if (IsValidDouble(_rightButton.Height) && IsValidDouble(_leftButton.Height))
                    {
                        _centerThumb.Height = Math.Max(ActualHeight - (_leftButton.Height + _rightButton.Height + _rightThumb.ActualHeight + _leftThumb.ActualHeight), 0);
                    }
                    else
                    {
                        _centerThumb.Height = Math.Max(ActualHeight - (_rightThumb.ActualHeight + _leftThumb.ActualHeight), 0);
                    }
                }
                _density = _movableWidth / MovableRange;
            }
        }

        //Method calculates new values when IsSnapToTickEnabled = FALSE
        private void ReCalculateRangeSelected(bool reCalculateLowerValue, bool reCalculateUpperValue, Direction direction)
        {
            _internalUpdate = true; //set flag to signal that the properties are being set by the object itself
            if (direction == Direction.Increase)
            {
                if (reCalculateUpperValue)
                {
                    _oldUpper = UpperValue;
                    var width = Orientation == Orientation.Horizontal ? _rightButton.Width : _rightButton.Height;
                    //Check first if button width is not Double.NaN
                    if (IsValidDouble(width))
                    {
                        // Make sure to get exactly rangestop if thumb is at the end
                        var upper = Equals(width, 0.0) ? Maximum : Math.Min(Maximum, (Maximum - MovableRange * width / _movableWidth));
                        this.UpperValue = this._isMoved ? upper : (this._roundToPrecision ? Math.Round(upper, this._precision) : upper);
                    }
                }

                if (reCalculateLowerValue)
                {
                    _oldLower = LowerValue;
                    var width = Orientation == Orientation.Horizontal ? _leftButton.Width : _leftButton.Height;
                    //Check first if button width is not Double.NaN
                    if (IsValidDouble(width))
                    {
                        // Make sure to get exactly rangestart if thumb is at the start
                        var lower = Equals(width, 0.0) ? Minimum : Math.Max(Minimum, (Minimum + MovableRange * width / _movableWidth));
                        this.LowerValue = this._isMoved ? lower : (this._roundToPrecision ? Math.Round(lower, this._precision) : lower);
                    }
                }
            }
            else
            {
                if (reCalculateLowerValue)
                {
                    _oldLower = LowerValue;
                    var width = Orientation == Orientation.Horizontal ? _leftButton.Width : _leftButton.Height;
                    //Check first if button width is not Double.NaN
                    if (IsValidDouble(width))
                    {
                        // Make sure to get exactly rangestart if thumb is at the start
                        var lower = Equals(width, 0.0) ? Minimum : Math.Max(Minimum, (Minimum + MovableRange * width / _movableWidth));
                        this.LowerValue = this._isMoved ? lower : (this._roundToPrecision ? Math.Round(lower, this._precision) : lower);
                    }
                }

                if (reCalculateUpperValue)
                {
                    _oldUpper = UpperValue;
                    var width = Orientation == Orientation.Horizontal ? _rightButton.Width : _rightButton.Height;
                    //Check first if button width is not Double.NaN
                    if (IsValidDouble(width))
                    {
                        // Make sure to get exactly rangestop if thumb is at the end
                        var upper = Equals(width, 0.0) ? Maximum : Math.Min(Maximum, (Maximum - MovableRange * width / _movableWidth));
                        this.UpperValue = this._isMoved ? upper : (this._roundToPrecision ? Math.Round(upper, this._precision) : upper);
                    }
                }
            }
            _roundToPrecision = false;
            _internalUpdate = false; //set flag to signal that the properties are being set by the object itself

            RaiseValueChangedEvents(this, reCalculateLowerValue, reCalculateUpperValue);
        }

        //Method used for cheking and setting correct values when IsSnapToTickEnable = TRUE (When thumb moving separately)
        private void ReCalculateRangeSelected(bool reCalculateLowerValue, bool reCalculateUpperValue, double value, Direction direction)
        {
            _internalUpdate = true; //set flag to signal that the properties are being set by the object itself
            var tickFrequency = this.TickFrequency.ToString(CultureInfo.InvariantCulture);
            if (reCalculateLowerValue)
            {
                _oldLower = LowerValue;
                double lower = 0;
                if (IsSnapToTickEnabled)
                {
                    lower = direction == Direction.Increase ? Math.Min(this.UpperValue - this.MinRange, value) : Math.Max(this.Minimum, value);
                }
                if (!tickFrequency.ToLower().Contains("e+") && tickFrequency.Contains("."))
                {
                    //decimal part is for cutting value exactly on that number of digits, which has TickFrequency to have correct values
                    var decimalPart = tickFrequency.Split('.');
                    LowerValue = Math.Round(lower, decimalPart[1].Length, MidpointRounding.AwayFromZero);
                }
                else
                {
                    LowerValue = lower;
                }
            }

            if (reCalculateUpperValue)
            {
                _oldUpper = UpperValue;
                double upper = 0;
                if (IsSnapToTickEnabled)
                {
                    upper = direction == Direction.Increase ? Math.Min(value, this.Maximum) : Math.Max(this.LowerValue + this.MinRange, value);
                }
                if (!tickFrequency.ToLower().Contains("e+") && tickFrequency.Contains("."))
                {
                    var decimalPart = tickFrequency.Split('.');
                    UpperValue = Math.Round(upper, decimalPart[1].Length, MidpointRounding.AwayFromZero);
                }
                else
                {
                    UpperValue = upper;
                }
            }

            _internalUpdate = false; //set flag to signal that the properties are being set by the object itself

            RaiseValueChangedEvents(this, reCalculateLowerValue, reCalculateUpperValue);
        }

        //Method used for cheking and setting correct values when IsSnapToTickEnable = TRUE (When thumb moving together)
        private void ReCalculateRangeSelected(double newLower, double newUpper, Direction direction)
        {
            double lower = 0, upper = 0;
            _internalUpdate = true; //set flag to signal that the properties are being set by the object itself
            _oldLower = LowerValue;
            _oldUpper = UpperValue;

            if (IsSnapToTickEnabled)
            {
                if (direction == Direction.Increase)
                {
                    lower = Math.Min(newLower, Maximum - (UpperValue - LowerValue));
                    upper = Math.Min(newUpper, Maximum);
                }
                else
                {
                    lower = Math.Max(newLower, Minimum);
                    upper = Math.Max(Minimum + (UpperValue - LowerValue), newUpper);
                }
                var tickFrequency = this.TickFrequency.ToString(CultureInfo.InvariantCulture);
                if (!tickFrequency.ToLower().Contains("e+") && tickFrequency.Contains("."))
                {
                    //decimal part is for cutting value exactly on that number of digits, which has TickFrequency to have correct values
                    var decimalPart = tickFrequency.Split('.');
                    //used when whole range decreasing to have correct updated values (lower first, upper - second)
                    if (direction == Direction.Decrease)
                    {
                        LowerValue = Math.Round(lower, decimalPart[1].Length, MidpointRounding.AwayFromZero);
                        UpperValue = Math.Round(upper, decimalPart[1].Length, MidpointRounding.AwayFromZero);
                    }
                    //used when whole range increasing to have correct updated values (upper first, lower - second)
                    else
                    {
                        UpperValue = Math.Round(upper, decimalPart[1].Length, MidpointRounding.AwayFromZero);
                        LowerValue = Math.Round(lower, decimalPart[1].Length, MidpointRounding.AwayFromZero);
                    }
                }
                else
                {
                    //used when whole range decreasing to have correct updated values (lower first, upper - second)
                    if (direction == Direction.Decrease)
                    {
                        LowerValue = lower;
                        UpperValue = upper;
                    }
                    //used when whole range increasing to have correct updated values (upper first, lower - second)
                    else
                    {
                        UpperValue = upper;
                        LowerValue = lower;
                    }
                }
            }
            _internalUpdate = false; //set flag to signal that the properties are being set by the object itself

            RaiseValueChangedEvents(this);
        }

        private void OnRangeParameterChanged(RangeParameterChangedEventArgs e, RoutedEvent Event)
        {
            e.RoutedEvent = Event;
            RaiseEvent(e);
        }

        public void MoveSelection(bool isLeft)
        {
            var widthChange = SmallChange * (UpperValue - LowerValue) * _movableWidth / MovableRange;

            widthChange = isLeft ? -widthChange : widthChange;
            MoveThumb(_leftButton, _rightButton, widthChange, Orientation, out _direction);
            ReCalculateRangeSelected(true, true, _direction);
        }

        public void ResetSelection(bool isStart)
        {
            var widthChange = Maximum - Minimum;
            widthChange = isStart ? -widthChange : widthChange;

            MoveThumb(_leftButton, _rightButton, widthChange, Orientation, out _direction);
            ReCalculateRangeSelected(true, true, _direction);
        }

        private void OnRangeSelectionChanged(RangeSelectionChangedEventArgs e)
        {
            e.RoutedEvent = RangeSelectionChangedEvent;
            RaiseEvent(e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _container = EnforceInstance<StackPanel>("PART_Container");
            _visualElementsContainer = EnforceInstance<StackPanel>("PART_RangeSliderContainer");
            _centerThumb = EnforceInstance<Thumb>("PART_MiddleThumb");
            _leftButton = EnforceInstance<RepeatButton>("PART_LeftEdge");
            _rightButton = EnforceInstance<RepeatButton>("PART_RightEdge");
            _leftThumb = EnforceInstance<Thumb>("PART_LeftThumb");
            _rightThumb = EnforceInstance<Thumb>("PART_RightThumb");
            InitializeVisualElementsContainer();
            ReCalculateSize();
        }

        //Get element from name. If it exist then element instance return, if not, new will be created
        private T EnforceInstance<T>(string partName) where T : FrameworkElement, new()
        {
            var element = GetTemplateChild(partName) as T ?? new T();
            return element;
        }

        //adds visual element to the container
        private void InitializeVisualElementsContainer()
        {
            _leftThumb.DragCompleted += LeftThumbDragComplete;
            _rightThumb.DragCompleted += RightThumbDragComplete;
            _leftThumb.DragStarted += LeftThumbDragStart;
            _rightThumb.DragStarted += RightThumbDragStart;
            _centerThumb.DragStarted += CenterThumbDragStarted;
            _centerThumb.DragCompleted += CenterThumbDragCompleted;

            //handle the drag delta events
            _centerThumb.DragDelta += CenterThumbDragDelta;
            _leftThumb.DragDelta += LeftThumbDragDelta;
            _rightThumb.DragDelta += RightThumbDragDelta;

            _visualElementsContainer.PreviewMouseDown += VisualElementsContainerPreviewMouseDown;
            _visualElementsContainer.PreviewMouseUp += VisualElementsContainerPreviewMouseUp;
            _visualElementsContainer.MouseLeave += VisualElementsContainerMouseLeave;

            _visualElementsContainer.MouseDown += VisualElementsContainerMouseDown;
        }

        //Handler for preview mouse button down for the whole StackPanel container
        private void VisualElementsContainerPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = Mouse.GetPosition(_visualElementsContainer);
            if (Orientation == Orientation.Horizontal)
            {
                if (position.X < _leftButton.ActualWidth)
                {
                    LeftButtonMouseDown();
                }
                else if (position.X > ActualWidth - _rightButton.ActualWidth)
                {
                    RightButtonMouseDown();
                }
                else if (position.X > (_leftButton.ActualWidth + _leftThumb.ActualWidth) &&
                         position.X < (ActualWidth - (_rightButton.ActualWidth + _rightThumb.ActualWidth)))
                {
                    CentralThumbMouseDown();
                }
            }
            else
            {
                if (position.Y > ActualHeight - _leftButton.ActualHeight)
                {
                    LeftButtonMouseDown();
                }
                else if (position.Y < _rightButton.ActualHeight)
                {
                    RightButtonMouseDown();
                }
                else if (position.Y > (_rightButton.ActualHeight + _rightButton.ActualHeight) &&
                         position.Y < (ActualHeight - (_leftButton.ActualHeight + _leftThumb.ActualHeight)))
                {
                    CentralThumbMouseDown();
                }
            }
        }

        private void VisualElementsContainerMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                MoveWholeRange = MoveWholeRange != true;
            }
        }

        #region Mouse events

        private void VisualElementsContainerMouseLeave(object sender, MouseEventArgs e)
        {
            _tickCount = 0;
            _timer.Stop();
        }

        private void VisualElementsContainerPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _tickCount = 0;
            _timer.Stop();
            _centerThumbBlocked = false;
        }

        private void LeftButtonMouseDown()
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var p = Mouse.GetPosition(_visualElementsContainer);
                var change = Orientation == Orientation.Horizontal
                    ? _leftButton.ActualWidth - p.X + (_leftThumb.ActualWidth / 2)
                    : -(_leftButton.ActualHeight - (ActualHeight - (p.Y + (_leftThumb.ActualHeight / 2))));
                if (!IsSnapToTickEnabled)
                {
                    if (IsMoveToPointEnabled && !MoveWholeRange)
                    {
                        MoveThumb(_leftButton, _centerThumb, -change, Orientation, out _direction);
                        ReCalculateRangeSelected(true, false, _direction);
                    }
                    else if (IsMoveToPointEnabled && MoveWholeRange)
                    {
                        MoveThumb(_leftButton, _rightButton, -change, Orientation, out _direction);
                        ReCalculateRangeSelected(true, true, _direction);
                    }
                }
                else
                {
                    if (IsMoveToPointEnabled && !MoveWholeRange)
                    {
                        this.JumpToNextTick(Direction.Decrease, ButtonType.BottomLeft, -change, this.LowerValue, true);
                    }
                    else if (IsMoveToPointEnabled && MoveWholeRange)
                    {
                        this.JumpToNextTick(Direction.Decrease, ButtonType.Both, -change, this.LowerValue, true);
                    }
                }
                if (!IsMoveToPointEnabled)
                {
                    _position = Mouse.GetPosition(_visualElementsContainer);
                    _bType = MoveWholeRange ? ButtonType.Both : ButtonType.BottomLeft;
                    _currentpoint = Orientation == Orientation.Horizontal ? _position.X : _position.Y;
                    _currenValue = LowerValue;
                    _isInsideRange = false;
                    _direction = Direction.Decrease;
                    _timer.Start();
                }
            }
        }

        private void RightButtonMouseDown()
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var p = Mouse.GetPosition(_visualElementsContainer);
                var change = Orientation == Orientation.Horizontal
                    ? _rightButton.ActualWidth - (ActualWidth - (p.X + (_rightThumb.ActualWidth / 2)))
                    : -(_rightButton.ActualHeight - (p.Y - (_rightThumb.ActualHeight / 2)));
                if (!IsSnapToTickEnabled)
                {
                    if (IsMoveToPointEnabled && !MoveWholeRange)
                    {
                        MoveThumb(_centerThumb, _rightButton, change, Orientation, out _direction);
                        ReCalculateRangeSelected(false, true, _direction);
                    }
                    else if (IsMoveToPointEnabled && MoveWholeRange)
                    {
                        MoveThumb(_leftButton, _rightButton, change, Orientation, out _direction);
                        ReCalculateRangeSelected(true, true, _direction);
                    }
                }
                else
                {
                    if (IsMoveToPointEnabled && !MoveWholeRange)
                    {
                        this.JumpToNextTick(Direction.Increase, ButtonType.TopRight, change, this.UpperValue, true);
                    }
                    else if (IsMoveToPointEnabled && MoveWholeRange)
                    {
                        this.JumpToNextTick(Direction.Increase, ButtonType.Both, change, this.UpperValue, true);
                    }
                }
                if (!IsMoveToPointEnabled)
                {
                    _position = Mouse.GetPosition(_visualElementsContainer);
                    _bType = MoveWholeRange ? ButtonType.Both : ButtonType.TopRight;
                    _currentpoint = Orientation == Orientation.Horizontal ? _position.X : _position.Y;
                    _currenValue = UpperValue;
                    _direction = Direction.Increase;
                    _isInsideRange = false;
                    _timer.Start();
                }
            }
        }

        private void CentralThumbMouseDown()
        {
            if (ExtendedMode)
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    _centerThumbBlocked = true;
                    var p = Mouse.GetPosition(_visualElementsContainer);
                    var change = Orientation == Orientation.Horizontal
                        ? (p.X + (_leftThumb.ActualWidth / 2) - (_leftButton.ActualWidth + _leftThumb.ActualWidth))
                        : -(ActualHeight - ((p.Y + (_leftThumb.ActualHeight / 2)) + _leftButton.ActualHeight));
                    if (!IsSnapToTickEnabled)
                    {
                        if (IsMoveToPointEnabled && !MoveWholeRange)
                        {
                            MoveThumb(_leftButton, _centerThumb, change, Orientation, out _direction);
                            ReCalculateRangeSelected(true, false, _direction);
                        }
                        else if (IsMoveToPointEnabled && MoveWholeRange)
                        {
                            MoveThumb(_leftButton, _rightButton, change, Orientation, out _direction);
                            ReCalculateRangeSelected(true, true, _direction);
                        }
                    }
                    else
                    {
                        if (IsMoveToPointEnabled && !MoveWholeRange)
                        {
                            this.JumpToNextTick(Direction.Increase, ButtonType.BottomLeft, change, this.LowerValue, true);
                        }
                        else if (IsMoveToPointEnabled && MoveWholeRange)
                        {
                            this.JumpToNextTick(Direction.Increase, ButtonType.Both, change, this.LowerValue, true);
                        }
                    }
                    if (!IsMoveToPointEnabled)
                    {
                        _position = Mouse.GetPosition(_visualElementsContainer);
                        _bType = MoveWholeRange ? ButtonType.Both : ButtonType.BottomLeft;
                        _currentpoint = Orientation == Orientation.Horizontal ? _position.X : _position.Y;
                        _currenValue = LowerValue;
                        _direction = Direction.Increase;
                        _isInsideRange = true;
                        _timer.Start();
                    }
                }
                else if (Mouse.RightButton == MouseButtonState.Pressed && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    _centerThumbBlocked = true;
                    var p = Mouse.GetPosition(_visualElementsContainer);
                    var change = Orientation == Orientation.Horizontal
                        ? ActualWidth - (p.X + (_rightThumb.ActualWidth / 2) + _rightButton.ActualWidth)
                        : -(p.Y + (_rightThumb.ActualHeight / 2) - (_rightButton.ActualHeight + _rightThumb.ActualHeight));
                    if (!IsSnapToTickEnabled)
                    {
                        if (IsMoveToPointEnabled && !MoveWholeRange)
                        {
                            MoveThumb(_centerThumb, _rightButton, -change, Orientation, out _direction);
                            ReCalculateRangeSelected(false, true, _direction);
                        }
                        else if (IsMoveToPointEnabled && MoveWholeRange)
                        {
                            MoveThumb(_leftButton, _rightButton, -change, Orientation, out _direction);
                            ReCalculateRangeSelected(true, true, _direction);
                        }
                    }
                    else
                    {
                        if (IsMoveToPointEnabled && !MoveWholeRange)
                        {
                            this.JumpToNextTick(Direction.Decrease, ButtonType.TopRight, -change, this.UpperValue, true);
                        }
                        else if (IsMoveToPointEnabled && MoveWholeRange)
                        {
                            this.JumpToNextTick(Direction.Decrease, ButtonType.Both, -change, this.UpperValue, true);
                        }
                    }
                    if (!IsMoveToPointEnabled)
                    {
                        _position = Mouse.GetPosition(_visualElementsContainer);
                        _bType = MoveWholeRange ? ButtonType.Both : ButtonType.TopRight;
                        _currentpoint = Orientation == Orientation.Horizontal ? _position.X : _position.Y;
                        _currenValue = UpperValue;
                        _direction = Direction.Decrease;
                        _isInsideRange = true;
                        _timer.Start();
                    }
                }
            }
        }

        #endregion

        #region Thumb Drag event handlers

        private void LeftThumbDragStart(object sender, DragStartedEventArgs e)
        {
            _isMoved = true;
            if (AutoToolTipPlacement != AutoToolTipPlacement.None)
            {
                if (_autoToolTip == null)
                {
                    _autoToolTip = new ToolTip();
                    _autoToolTip.Placement = PlacementMode.Custom;
                    _autoToolTip.CustomPopupPlacementCallback = PopupPlacementCallback;
                }
                _autoToolTip.Content = GetLowerToolTipNumber();
                _autoToolTip.PlacementTarget = _leftThumb;
                _autoToolTip.IsOpen = true;
            }
            _basePoint = Mouse.GetPosition(_container);
            e.RoutedEvent = LowerThumbDragStartedEvent;
            RaiseEvent(e);
        }

        private void LeftThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var change = Orientation == Orientation.Horizontal ? e.HorizontalChange : e.VerticalChange;
            if (!IsSnapToTickEnabled)
            {
                MoveThumb(_leftButton, _centerThumb, change, Orientation, out _direction);
                ReCalculateRangeSelected(true, false, _direction);
            }
            else
            {
                Direction localDirection;
                var currentPoint = Mouse.GetPosition(_container);
                if (Orientation == Orientation.Horizontal)
                {
                    if (currentPoint.X >= 0 && currentPoint.X < _container.ActualWidth - (_rightButton.ActualWidth + _rightThumb.ActualWidth + _centerThumb.MinWidth))
                    {
                        localDirection = currentPoint.X > _basePoint.X ? Direction.Increase : Direction.Decrease;
                        this.JumpToNextTick(localDirection, ButtonType.BottomLeft, change, this.LowerValue, false);
                    }
                }
                else
                {
                    if (currentPoint.Y <= _container.ActualHeight && currentPoint.Y > _rightButton.ActualHeight + _rightThumb.ActualHeight + _centerThumb.MinHeight)
                    {
                        localDirection = currentPoint.Y < _basePoint.Y ? Direction.Increase : Direction.Decrease;
                        this.JumpToNextTick(localDirection, ButtonType.BottomLeft, -change, this.LowerValue, false);
                    }
                }
            }
            _basePoint = Mouse.GetPosition(_container);
            if (AutoToolTipPlacement != AutoToolTipPlacement.None)
            {
                _autoToolTip.Content = GetLowerToolTipNumber();
                RelocateAutoToolTip();
            }

            e.RoutedEvent = LowerThumbDragDeltaEvent;
            RaiseEvent(e);
        }

        private void LeftThumbDragComplete(object sender, DragCompletedEventArgs e)
        {
            if (_autoToolTip != null)
            {
                _autoToolTip.IsOpen = false;
                _autoToolTip = null;
            }
            e.RoutedEvent = LowerThumbDragCompletedEvent;
            RaiseEvent(e);
        }

        private void RightThumbDragStart(object sender, DragStartedEventArgs e)
        {
            _isMoved = true;
            if (AutoToolTipPlacement != AutoToolTipPlacement.None)
            {
                if (_autoToolTip == null)
                {
                    _autoToolTip = new ToolTip();
                    _autoToolTip.Placement = PlacementMode.Custom;
                    _autoToolTip.CustomPopupPlacementCallback = PopupPlacementCallback;
                }
                _autoToolTip.Content = GetUpperToolTipNumber();
                _autoToolTip.PlacementTarget = _rightThumb;
                _autoToolTip.IsOpen = true;
            }
            _basePoint = Mouse.GetPosition(_container);
            e.RoutedEvent = UpperThumbDragStartedEvent;
            RaiseEvent(e);
        }

        private void RightThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var change = Orientation == Orientation.Horizontal ? e.HorizontalChange : e.VerticalChange;
            if (!IsSnapToTickEnabled)
            {
                MoveThumb(_centerThumb, _rightButton, change, Orientation, out _direction);
                ReCalculateRangeSelected(false, true, _direction);
            }
            else
            {
                Direction localDirection;
                var currentPoint = Mouse.GetPosition(_container);
                if (Orientation == Orientation.Horizontal)
                {
                    if (currentPoint.X < _container.ActualWidth && currentPoint.X > _leftButton.ActualWidth + _leftThumb.ActualWidth + _centerThumb.MinWidth)
                    {
                        localDirection = currentPoint.X > _basePoint.X ? Direction.Increase : Direction.Decrease;
                        this.JumpToNextTick(localDirection, ButtonType.TopRight, change, this.UpperValue, false);
                    }
                }
                else
                {
                    if (currentPoint.Y >= 0 && currentPoint.Y < _container.ActualHeight - (_leftButton.ActualHeight + _leftThumb.ActualHeight + _centerThumb.MinHeight))
                    {
                        localDirection = currentPoint.Y < _basePoint.Y ? Direction.Increase : Direction.Decrease;
                        this.JumpToNextTick(localDirection, ButtonType.TopRight, -change, this.UpperValue, false);
                    }
                }

                _basePoint = Mouse.GetPosition(_container);
            }
            if (AutoToolTipPlacement != AutoToolTipPlacement.None)
            {
                _autoToolTip.Content = GetUpperToolTipNumber();
                RelocateAutoToolTip();
            }
            e.RoutedEvent = UpperThumbDragDeltaEvent;
            RaiseEvent(e);
        }

        private void RightThumbDragComplete(object sender, DragCompletedEventArgs e)
        {
            if (_autoToolTip != null)
            {
                _autoToolTip.IsOpen = false;
                _autoToolTip = null;
            }
            e.RoutedEvent = UpperThumbDragCompletedEvent;
            RaiseEvent(e);
        }

        private void CenterThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            _isMoved = true;
            if (AutoToolTipPlacement != AutoToolTipPlacement.None)
            {
                if (_autoToolTip == null)
                {
                    _autoToolTip = new ToolTip {
                        Placement = PlacementMode.Custom,
                        CustomPopupPlacementCallback = PopupPlacementCallback
                    };
                }
                _autoToolTip.Content = GetLowerToolTipNumber() + " ; " + GetUpperToolTipNumber();
                _autoToolTip.PlacementTarget = _centerThumb;
                _autoToolTip.IsOpen = true;
            }
            _basePoint = Mouse.GetPosition(_container);
            e.RoutedEvent = CentralThumbDragStartedEvent;
            RaiseEvent(e);
        }

        private void CenterThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!_centerThumbBlocked)
            {
                var change = Orientation == Orientation.Horizontal ? e.HorizontalChange : e.VerticalChange;
                if (!IsSnapToTickEnabled)
                {
                    MoveThumb(_leftButton, _rightButton, change, Orientation, out _direction);
                    ReCalculateRangeSelected(true, true, _direction);
                }
                else
                {
                    Direction localDirection;
                    var currentPoint = Mouse.GetPosition(_container);
                    if (Orientation == Orientation.Horizontal)
                    {
                        if (currentPoint.X >= 0 && currentPoint.X < _container.ActualWidth)
                        {
                            localDirection = currentPoint.X > _basePoint.X ? Direction.Increase : Direction.Decrease;
                            this.JumpToNextTick(localDirection, ButtonType.Both, change, localDirection == Direction.Increase ? this.UpperValue : this.LowerValue, false);
                        }
                    }
                    else
                    {
                        if (currentPoint.Y >= 0 && currentPoint.Y < _container.ActualHeight)
                        {
                            localDirection = currentPoint.Y < _basePoint.Y ? Direction.Increase : Direction.Decrease;
                            this.JumpToNextTick(localDirection, ButtonType.Both, -change, localDirection == Direction.Increase ? this.UpperValue : this.LowerValue, false);
                        }
                    }
                }
                _basePoint = Mouse.GetPosition(_container);
                if (AutoToolTipPlacement != AutoToolTipPlacement.None)
                {
                    _autoToolTip.Content = GetLowerToolTipNumber() + " ; " + GetUpperToolTipNumber();
                    RelocateAutoToolTip();
                }
            }

            e.RoutedEvent = CentralThumbDragDeltaEvent;
            RaiseEvent(e);
        }

        private void CenterThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (_autoToolTip != null)
            {
                _autoToolTip.IsOpen = false;
                _autoToolTip = null;
            }
            e.RoutedEvent = CentralThumbDragCompletedEvent;
            RaiseEvent(e);
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

        private Boolean GetResult(Double currentPoint, Double endPoint, Direction direction)
        {
            if (direction == Direction.Increase)
            {
                return Orientation == Orientation.Horizontal && currentPoint > endPoint ||
                       Orientation == Orientation.Vertical && currentPoint < endPoint;
            }
            return Orientation == Orientation.Horizontal && currentPoint < endPoint ||
                   Orientation == Orientation.Vertical && currentPoint > endPoint;
        }

        //This is timer event, which starts when IsMoveToPoint = false
        //Supports IsSnapToTick option
        private void MoveToNextValue(object sender, EventArgs e)
        {
            //Get updated position of cursor
            _position = Mouse.GetPosition(_visualElementsContainer);
            _currentpoint = Orientation == Orientation.Horizontal ? _position.X : _position.Y;
            var endpoint = UpdateEndPoint(_bType, _direction);
            var result = GetResult(_currentpoint, endpoint, _direction);
            double widthChange;
            if (!IsSnapToTickEnabled)
            {
                widthChange = SmallChange;
                if (_tickCount > 5)
                {
                    widthChange = LargeChange;
                }
                _roundToPrecision = true;
                if (!widthChange.ToString(CultureInfo.InvariantCulture).ToLower().Contains("e") &&
                    widthChange.ToString(CultureInfo.InvariantCulture).Contains("."))
                {
                    var array = widthChange.ToString(CultureInfo.InvariantCulture).Split('.');
                    _precision = array[1].Length;
                }
                else
                {
                    _precision = 0;
                }
                //Change value sign according to Horizontal or Vertical orientation
                widthChange = Orientation == Orientation.Horizontal ? widthChange : -widthChange;
                //Change value sign one more time according to Increase or Decrease direction
                widthChange = _direction == Direction.Increase ? widthChange : -widthChange;
                if (result)
                {
                    switch (_bType)
                    {
                        case ButtonType.BottomLeft:
                            MoveThumb(_leftButton, _centerThumb, widthChange * _density, Orientation, out _direction);
                            ReCalculateRangeSelected(true, false, _direction);
                            break;
                        case ButtonType.TopRight:
                            MoveThumb(_centerThumb, _rightButton, widthChange * _density, Orientation, out _direction);
                            ReCalculateRangeSelected(false, true, _direction);
                            break;
                        case ButtonType.Both:
                            MoveThumb(_leftButton, _rightButton, widthChange * _density, Orientation, out _direction);
                            ReCalculateRangeSelected(true, true, _direction);
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
                widthChange = Orientation == Orientation.Horizontal ? widthChange : -widthChange;
                if (_direction == Direction.Increase)
                {
                    if (result)
                    {
                        switch (_bType)
                        {
                            case ButtonType.BottomLeft:
                                MoveThumb(this._leftButton, this._centerThumb, widthChange * this._density, this.Orientation);
                                ReCalculateRangeSelected(true, false, LowerValue + value, _direction);
                                break;
                            case ButtonType.TopRight:
                                MoveThumb(this._centerThumb, this._rightButton, widthChange * this._density, this.Orientation);
                                ReCalculateRangeSelected(false, true, UpperValue + value, _direction);
                                break;
                            case ButtonType.Both:
                                MoveThumb(this._leftButton, this._rightButton, widthChange * this._density, this.Orientation);
                                ReCalculateRangeSelected(LowerValue + value, UpperValue + value, _direction);
                                break;
                        }
                    }
                }
                else if (_direction == Direction.Decrease)
                {
                    if (result)
                    {
                        switch (_bType)
                        {
                            case ButtonType.BottomLeft:
                                MoveThumb(this._leftButton, this._centerThumb, -widthChange * this._density, this.Orientation);
                                ReCalculateRangeSelected(true, false, LowerValue - value, _direction);
                                break;
                            case ButtonType.TopRight:
                                MoveThumb(this._centerThumb, this._rightButton, -widthChange * this._density, this.Orientation);
                                ReCalculateRangeSelected(false, true, UpperValue - value, _direction);
                                break;
                            case ButtonType.Both:
                                MoveThumb(this._leftButton, this._rightButton, -widthChange * this._density, this.Orientation);
                                ReCalculateRangeSelected(LowerValue - value, UpperValue - value, _direction);
                                break;
                        }
                    }
                }
            }
            _tickCount++;
        }

        //Helper method to handle snapToTick scenario and decrease amount of code
        private void SnapToTickHandle(ButtonType type, Direction direction, double difference)
        {
            var value = difference;
            //change sign of "difference" variable because Horizontal and Vertical orientations has are different directions
            difference = Orientation == Orientation.Horizontal ? difference : -difference;
            if (direction == Direction.Increase)
            {
                switch (type)
                {
                    case ButtonType.TopRight:
                        if (UpperValue < Maximum)
                        {
                            MoveThumb(this._centerThumb, this._rightButton, difference * this._density, this.Orientation);
                            ReCalculateRangeSelected(false, true, UpperValue + value, direction);
                        }
                        break;
                    case ButtonType.BottomLeft:
                        if (LowerValue < UpperValue - MinRange)
                        {
                            MoveThumb(this._leftButton, this._centerThumb, difference * this._density, this.Orientation);
                            ReCalculateRangeSelected(true, false, LowerValue + value, direction);
                        }
                        break;
                    case ButtonType.Both:
                        if (UpperValue < Maximum)
                        {
                            MoveThumb(this._leftButton, this._rightButton, difference * this._density, this.Orientation);
                            ReCalculateRangeSelected(LowerValue + value, UpperValue + value, direction);
                        }
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case ButtonType.TopRight:
                        if (UpperValue > LowerValue + MinRange)
                        {
                            MoveThumb(this._centerThumb, this._rightButton, -difference * this._density, this.Orientation);
                            ReCalculateRangeSelected(false, true, UpperValue - value, direction);
                        }
                        break;
                    case ButtonType.BottomLeft:
                        if (LowerValue > Minimum)
                        {
                            MoveThumb(this._leftButton, this._centerThumb, -difference * this._density, this.Orientation);
                            ReCalculateRangeSelected(true, false, LowerValue - value, direction);
                        }
                        break;
                    case ButtonType.Both:
                        if (LowerValue > Minimum)
                        {
                            MoveThumb(this._leftButton, this._rightButton, -difference * this._density, this.Orientation);
                            ReCalculateRangeSelected(LowerValue - value, UpperValue - value, direction);
                        }
                        break;
                }
            }
        }

        //Calculating next value for Tick
        private double CalculateNextTick(Direction direction, double checkingValue, double distance, bool moveDirectlyToNextTick)
        {
            var checkingValuePos = checkingValue - Minimum;
            if (!IsMoveToPointEnabled)
            {
                //Check if current value is exactly Tick value or it situated between Ticks
                var checkingValueChanged = checkingValuePos; // + distance; // <-- introduced by @drayde with #2006 but it breaks the left thumb movement #2880
                var x = checkingValueChanged / TickFrequency;
                if (!IsDoubleCloseToInt(x))
                {
                    distance = TickFrequency * (int)x;
                    if (direction == Direction.Increase)
                    {
                        distance += TickFrequency;
                    }
                    distance = (distance - Math.Abs(checkingValuePos));
                    _currenValue = 0;
                    return Math.Abs(distance);
                }
            }
            //If we need move directly to next tick without calculating the difference between ticks
            //Use when MoveToPoint disabled
            if (moveDirectlyToNextTick)
            {
                distance = TickFrequency;
            }
            //If current value == tick (Value is divisible)
            else
            {
                //current value in units (exactly in the place under cursor)
                var currentValue = checkingValuePos + (distance / _density);
                var x = currentValue / TickFrequency;
                if (direction == Direction.Increase)
                {
                    var nextvalue = x.ToString(CultureInfo.InvariantCulture).ToLower().Contains("e+")
                        ? (x * TickFrequency) + TickFrequency
                        : ((int)x * TickFrequency) + TickFrequency;

                    distance = (nextvalue - Math.Abs(checkingValuePos));
                }
                else
                {
                    var previousValue = x.ToString(CultureInfo.InvariantCulture).ToLower().Contains("e+")
                        ? x * TickFrequency
                        : (int)x * TickFrequency;
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
            var p = Mouse.GetPosition(_visualElementsContainer);
            var pos = Orientation == Orientation.Horizontal ? p.X : p.Y;
            var widthHeight = Orientation == Orientation.Horizontal ? ActualWidth : ActualHeight;
            var tickIntervalInPixels = direction == Direction.Increase
                ? TickFrequency * _density
                : -TickFrequency * _density;

            if (jumpDirectlyToTick)
            {
                this.SnapToTickHandle(type, direction, difference);
            }
            else
            {
                if (direction == Direction.Increase)
                {
                    if (!IsDoubleCloseToInt(checkingValue / TickFrequency))
                    {
                        if (distance > (difference * _density) / 2 || (distance >= (widthHeight - pos) || distance >= pos))
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
                    if (!IsDoubleCloseToInt(checkingValue / TickFrequency))
                    {
                        if ((distance <= -(difference * _density) / 2) || (UpperValue - LowerValue) < difference)
                        {
                            this.SnapToTickHandle(type, direction, difference);
                        }
                    }
                    else
                    {
                        if (distance < tickIntervalInPixels / 2 || (UpperValue - LowerValue) < difference)
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
            var offset = _autoToolTip.HorizontalOffset;
            _autoToolTip.HorizontalOffset = offset + 0.001;
            _autoToolTip.HorizontalOffset = offset;
        }

        private Boolean IsValidDouble(Double d)
        {
            if (!Double.IsNaN(d) && !Double.IsInfinity(d))
            {
                return true;
            }
            return false;
        }

        //CHeck if two doubles approximately equals
        private bool ApproximatelyEquals(double value1, double value2)
        {
            return Math.Abs(value1 - value2) <= Epsilon;
        }

        private Boolean IsDoubleCloseToInt(double val)
        {
            return ApproximatelyEquals(Math.Abs(val - Math.Round(val)), 0);
        }

        //Get lower value for autotooltip
        private String GetLowerToolTipNumber()
        {
            var lowerValue = this.LowerValue;
            return this.GetToolTipNumber(lowerValue);
        }

        //Get upper value for autotooltip
        private String GetUpperToolTipNumber()
        {
            var upperValue = this.UpperValue;
            return this.GetToolTipNumber(upperValue);
        }

        private string GetToolTipNumber(double value)
        {
            var converter = this.AutoToolTipTextConverter;
            if (converter != null)
            {
                var convertedValue = converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);
                if (convertedValue != null)
                {
                    return convertedValue.ToString();
                }
            }

            var format = (NumberFormatInfo)(NumberFormatInfo.CurrentInfo.Clone());
            format.NumberDecimalDigits = this.AutoToolTipPrecision;
            return value.ToString("N", format);
        }

        //CustomPopupPlacement callback for placing autotooltip int TopLeft or BottomRight position
        private CustomPopupPlacement[] PopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
        {
            switch (AutoToolTipPlacement)
            {
                case AutoToolTipPlacement.TopLeft:
                    if (Orientation == Orientation.Horizontal)
                    {
                        // Place popup at top of thumb
                        return new CustomPopupPlacement[] { new CustomPopupPlacement(new Point((targetSize.Width - popupSize.Width) * 0.5, -popupSize.Height), PopupPrimaryAxis.Horizontal) };
                    }
                    // Place popup at left of thumb 
                    return new CustomPopupPlacement[] { new CustomPopupPlacement(new Point(-popupSize.Width, (targetSize.Height - popupSize.Height) * 0.5), PopupPrimaryAxis.Vertical) };

                case AutoToolTipPlacement.BottomRight:
                    if (Orientation == Orientation.Horizontal)
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

        private static bool IsValidPrecision(object value)
        {
            return ((Int32)value >= 0);
        }

        private static bool IsValidMinRange(object value)
        {
            var d = (double)value;
            if (d < 0.0 || Double.IsInfinity(d) || Double.IsNaN(d))
            {
                return false;
            }
            return true;
        }

        private static bool IsValidTickFrequency(object value)
        {
            var d = (double)value;
            if (d <= 0.0 || Double.IsInfinity(d) || Double.IsNaN(d))
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Coerce callbacks

        private static object CoerceMinimum(DependencyObject d, object basevalue)
        {
            var rs = (RangeSlider)d;
            var value = (double)basevalue;
            if (value > rs.Maximum)
            {
                return rs.Maximum;
            }
            return basevalue;
        }

        private static object CoerceMaximum(DependencyObject d, object basevalue)
        {
            var rs = (RangeSlider)d;
            var value = (double)basevalue;
            if (value < rs.Minimum)
            {
                return rs.Minimum;
            }
            return basevalue;
        }

        private static object CoerceLowerValue(DependencyObject d, object basevalue)
        {
            var rs = (RangeSlider)d;
            var value = (double)basevalue;
            if (value < rs.Minimum || rs.UpperValue - rs.MinRange < rs.Minimum)
            {
                return rs.Minimum;
            }
            if (value > rs.UpperValue - rs.MinRange)
            {
                return rs.UpperValue - rs.MinRange;
            }
            return basevalue;
        }

        private static object CoerceUpperValue(DependencyObject d, object basevalue)
        {
            var rs = (RangeSlider)d;
            var value = (double)basevalue;
            if (value > rs.Maximum || rs.LowerValue + rs.MinRange > rs.Maximum)
            {
                return rs.Maximum;
            }
            if (value < rs.LowerValue + rs.MinRange)
            {
                return rs.LowerValue + rs.MinRange;
            }
            return basevalue;
        }

        private static object CoerceMinRange(DependencyObject d, object basevalue)
        {
            var rs = (RangeSlider)d;
            var value = (double)basevalue;
            if (rs.LowerValue + value > rs.Maximum)
            {
                return rs.Maximum - rs.LowerValue;
            }
            return basevalue;
        }

        private static object CoerceMinRangeWidth(DependencyObject d, object basevalue)
        {
            var rs = (RangeSlider)d;
            if (rs._leftThumb != null && rs._rightThumb != null)
            {
                double width;
                if (rs.Orientation == Orientation.Horizontal)
                {
                    width = rs.ActualWidth - rs._leftThumb.ActualWidth - rs._rightThumb.ActualWidth;
                }
                else
                {
                    width = rs.ActualHeight - rs._leftThumb.ActualHeight - rs._rightThumb.ActualHeight;
                }
                return (Double)basevalue > width / 2 ? width / 2 : (Double)basevalue;
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

        //Lower/Upper values property changed callback
        private static void RangesChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var slider = (RangeSlider)dependencyObject;
            if (slider._internalUpdate)
            {
                return;
            }

            dependencyObject.CoerceValue(UpperValueProperty);
            dependencyObject.CoerceValue(LowerValueProperty);

            RaiseValueChangedEvents(dependencyObject);

            slider._oldLower = slider.LowerValue;
            slider._oldUpper = slider.UpperValue;
            slider.ReCalculateSize();
        }

        private static void MinRangeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var value = (Double)e.NewValue;
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

        private static void MinRangeWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var slider = (RangeSlider)sender;
            slider.ReCalculateSize();
        }

        private static void IntervalChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var rs = (RangeSlider)dependencyObject;
            rs._timer.Interval = TimeSpan.FromMilliseconds((Int32)e.NewValue);
        }

        //Raises all value changes events
        private static void RaiseValueChangedEvents(DependencyObject dependencyObject, bool lowerValueReCalculated = true, bool upperValueReCalculated = true)
        {
            var slider = (RangeSlider)dependencyObject;
            var lowerValueEquals = Equals(slider._oldLower, slider.LowerValue);
            var upperValueEquals = Equals(slider._oldUpper, slider.UpperValue);
            if ((lowerValueReCalculated || upperValueReCalculated) && (!lowerValueEquals || !upperValueEquals))
            {
                slider.OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(slider.LowerValue, slider.UpperValue, slider._oldLower, slider._oldUpper));
            }
            if (lowerValueReCalculated && !lowerValueEquals)
            {
                slider.OnRangeParameterChanged(new RangeParameterChangedEventArgs(RangeParameterChangeType.Lower, slider._oldLower, slider.LowerValue), LowerValueChangedEvent);
            }
            if (upperValueReCalculated && !upperValueEquals)
            {
                slider.OnRangeParameterChanged(new RangeParameterChangedEventArgs(RangeParameterChangeType.Upper, slider._oldUpper, slider.UpperValue), UpperValueChangedEvent);
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
}