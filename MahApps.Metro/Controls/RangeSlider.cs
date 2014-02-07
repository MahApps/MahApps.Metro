using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        public static RoutedUICommand MoveBack = new RoutedUICommand("MoveBack", "MoveBack", typeof(RangeSlider), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.B, ModifierKeys.Control) }));
        public static RoutedUICommand MoveForward = new RoutedUICommand("MoveForward", "MoveForward", typeof(RangeSlider), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F, ModifierKeys.Control) }));
        public static RoutedUICommand MoveAllForward = new RoutedUICommand("MoveAllForward", "MoveAllForward", typeof(RangeSlider), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F, ModifierKeys.Alt) }));
        public static RoutedUICommand MoveAllBack = new RoutedUICommand("MoveAllBack", "MoveAllBack", typeof(RangeSlider), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.B, ModifierKeys.Alt) }));

        #region Routed events

        public static readonly RoutedEvent RangeSelectionChangedEvent =
            EventManager.RegisterRoutedEvent("RangeSelectionChanged", RoutingStrategy.Bubble,
                typeof (RangeSelectionChangedEventHandler), typeof (RangeSlider));

        public static readonly RoutedEvent LowerValueChangedEvent =
            EventManager.RegisterRoutedEvent("LowerValueChanged", RoutingStrategy.Bubble,
                typeof (RangeParameterChangedEventHandler), typeof (RangeSlider));

        public static readonly RoutedEvent UpperValueChangedEvent =
            EventManager.RegisterRoutedEvent("UpperValueChanged", RoutingStrategy.Bubble,
                typeof (RangeParameterChangedEventHandler), typeof (RangeSlider));

        public static readonly RoutedEvent LowerThumbDragStartedEvent =
            EventManager.RegisterRoutedEvent("LowerThumbDragStarted", RoutingStrategy.Bubble,
                typeof (DragStartedEventHandler), typeof (RangeSlider));

        public static readonly RoutedEvent LowerThumbDragCompletedEvent =
            EventManager.RegisterRoutedEvent("LowerThumbDragCompleted", RoutingStrategy.Bubble,
                typeof (DragCompletedEventHandler), typeof (RangeSlider));

        public static readonly RoutedEvent UpperThumbDragStartedEvent =
            EventManager.RegisterRoutedEvent("UpperThumbDragStarted", RoutingStrategy.Bubble,
                typeof (DragStartedEventHandler), typeof (RangeSlider));

        public static readonly RoutedEvent UpperThumbDragCompletedEvent =
            EventManager.RegisterRoutedEvent("UpperThumbDragCompleted", RoutingStrategy.Bubble,
                typeof (DragCompletedEventHandler), typeof (RangeSlider));

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

        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register("LowerValue", typeof (Double), typeof (RangeSlider),
                new FrameworkPropertyMetadata((Double) 0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, RangesChanged, CoerceLowerValue));

        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register("UpperValue", typeof (Double), typeof (RangeSlider),
                new FrameworkPropertyMetadata((Double) 1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, RangesChanged, CoerceUpperValue));

        public static readonly DependencyProperty MinRangeProperty =
            DependencyProperty.Register("MinRange", typeof (Double), typeof (RangeSlider),
                new FrameworkPropertyMetadata((Double)0, MinRangeChanged));

        public static readonly DependencyProperty MinBridgeWidthProperty =
            DependencyProperty.Register("MinBridgeWidth", typeof(Double), typeof(RangeSlider),
                new UIPropertyMetadata((Double)15, MinBridgeWidthChanged, CoerceMinBridgeWidth));

        public static readonly DependencyProperty MoveWholeRangeProperty =
            DependencyProperty.Register("MoveWholeRange", typeof(Boolean), typeof(RangeSlider),
                new PropertyMetadata((Boolean)false));

        public static readonly DependencyProperty ExtendedModeProperty =
            DependencyProperty.Register("ExtendedMode", typeof(Boolean), typeof(RangeSlider),
                new PropertyMetadata((Boolean)false));


        public static readonly DependencyProperty IsSnapToTickEnabledProperty =
            DependencyProperty.Register("IsSnapToTickEnabled", typeof(Boolean), typeof(RangeSlider),
                new PropertyMetadata((Boolean)false));

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(RangeSlider),
                new FrameworkPropertyMetadata(Orientation.Horizontal));

        public static readonly DependencyProperty TickFrequencyProperty =
            DependencyProperty.Register("TickFrequency", typeof(Double), typeof(RangeSlider),
                new UIPropertyMetadata((Double)10, TickFrequencyChangedCallback, CoerceTickFrequencyCallback));

        

        public static readonly DependencyProperty IsMoveToPointEnabledProperty =
            DependencyProperty.Register("IsMoveToPointEnabled", typeof(Boolean), typeof(RangeSlider),
                new PropertyMetadata((Boolean)false));

        public static readonly DependencyProperty TickPlacementProperty =
            DependencyProperty.Register("TickPlacement", typeof(TickPlacement), typeof(RangeSlider),
                new FrameworkPropertyMetadata(TickPlacement.None));

        

        public static readonly DependencyProperty AutoToolTipPlacementProperty =
            DependencyProperty.Register("AutoToolTipPlacement", typeof(AutoToolTipPlacement), typeof(RangeSlider),
                new FrameworkPropertyMetadata(AutoToolTipPlacement.None));

        public static readonly DependencyProperty AutoToolTipPrecisionProperty =
            DependencyProperty.Register("AutoToolTipPrecision", typeof (Int32), typeof (RangeSlider),
                new FrameworkPropertyMetadata(0), ValidateValueCallback);

        private static bool ValidateValueCallback(object value)
        {
            return ((Int32) value >= 0);
        }


        public Int32 AutoToolTipPrecision
        {
            get { return (Int32)GetValue(AutoToolTipPrecisionProperty); }
            set { SetValue(AutoToolTipPrecisionProperty, value); }
        }

        public AutoToolTipPlacement AutoToolTipPlacement
        {
            get { return (AutoToolTipPlacement)GetValue(AutoToolTipPlacementProperty); }
            set { SetValue(AutoToolTipPlacementProperty, value); }
        }

        public TickPlacement TickPlacement
        {
            get { return (TickPlacement)GetValue(TickPlacementProperty); }
            set { SetValue(TickPlacementProperty, value); }
        }

        public Boolean IsMoveToPointEnabled
        {
            get { return (Boolean)GetValue(IsMoveToPointEnabledProperty); }
            set { SetValue(IsMoveToPointEnabledProperty, value); }
        }

        public Double TickFrequency
        {
            get { return (Double)GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }


        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public Boolean IsSnapToTickEnabled
        {
            get { return (Boolean)GetValue(IsSnapToTickEnabledProperty); }
            set { SetValue(IsSnapToTickEnabledProperty, value); }
        }


        public Boolean ExtendedMode
        {
            get { return (Boolean)GetValue(ExtendedModeProperty); }
            set { SetValue(ExtendedModeProperty, value); }
        }

        //Property means that whole range will be moved when pressing on left or right repeat button
        //or inside range when ExtendedMode property is set to true
        public Boolean MoveWholeRange
        {
            get { return (Boolean)GetValue(MoveWholeRangeProperty); }
            set { SetValue(MoveWholeRangeProperty, value); }
        }

        public Double MinBridgeWidth
        {
            get { return (Double)GetValue(MinBridgeWidthProperty); }
            set { SetValue(MinBridgeWidthProperty, value); }
        }


        /// <summary>
        /// Get/sets the beginning of the range selection.
        /// </summary>
        public Double LowerValue
        {
            get { return (Double) GetValue(LowerValueProperty); }
            set { SetValue(LowerValueProperty, value); }
        }

        /// <summary>
        /// Get/sets the end of the range selection.
        /// </summary>
        public Double UpperValue
        {
            get { return (Double) GetValue(UpperValueProperty); }
            set { SetValue(UpperValueProperty, value); }
        }

        /// <summary>
        /// Get/sets the minimum range that can be selected.
        /// </summary>
        public Double MinRange
        {
            get { return (Double) GetValue(MinRangeProperty); }
            set { SetValue(MinRangeProperty, value); }
        }

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
        private StackPanel _container;
        private TickBar _topTick;
        private TickBar _bottomTick;
        //private double _movableRange;
        private Double _movableWidth;
        private readonly DispatcherTimer _timer;

        private uint _tickCount = 0;
        private double _currentpoint;
        private bool _isInsideRange;
        private Direction _direction;
        private ButtonType _bType;
        private Point _position;
        private Point _basePoint;
        private double _currenValue;
        private double _density;
        private ToolTip _autoToolTip;

        #endregion


        public RangeSlider()
        {
            CommandBindings.Add(new CommandBinding(MoveBack, MoveBackHandler));
            CommandBindings.Add(new CommandBinding(MoveForward, MoveForwardHandler));
            CommandBindings.Add(new CommandBinding(MoveAllForward, MoveAllForwardHandler));
            CommandBindings.Add(new CommandBinding(MoveAllBack, MoveAllBackHandler));

            DependencyPropertyDescriptor.FromProperty(ActualWidthProperty, typeof(RangeSlider)).AddValueChanged(this, delegate { ReCalculateWidths(); });
            _timer = new DispatcherTimer();
            _timer.Tick += SerialMovement;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
        }

       

        static RangeSlider()
        {
            
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(typeof(RangeSlider)));
            MinimumProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata((Double)0.0, FrameworkPropertyMetadataOptions.AffectsMeasure, PropertyChangedCallback, CoerceMinimum));
            //MaximumProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata((Double)1.0, FrameworkPropertyMetadataOptions.AffectsMeasure, CoerceMaximum));
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            
        }


        private static void OnTickPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeSlider slider = (RangeSlider)d;
            if (slider._topTick != null && slider._bottomTick != null)
            {
                slider._topTick.Placement = (TickBarPlacement)e.NewValue;
                slider._bottomTick.Placement = (TickBarPlacement)e.NewValue;
            }
        }

        private static void OnAutoToolTipPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //RangeSlider slider = (RangeSlider)d;
            //if (slider._topTick != null && slider._bottomTick != null)
            //{
            //    slider._topTick.TickFrequency = (Double)e.NewValue;
            //    slider._bottomTick.TickFrequency = (Double)e.NewValue;
            //}
        }


        /// <summary>
        /// Responds to a change in the value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum"/> property.
        /// </summary>
        /// <param name="oldMinimum">The old value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum"/> property.</param><param name="newMinimum">The new value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum"/> property.</param>
        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            CheckLowerValue();
            ReCalculateRanges();
            ReCalculateWidths();
        }

        /// <summary>
        /// Responds to a change in the value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum"/> property.
        /// </summary>
        /// <param name="oldMaximum">The old value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum"/> property.</param><param name="newMaximum">The new value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum"/> property.</param>
        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            CheckUpperValue();
            ReCalculateRanges();
            ReCalculateWidths();
        }

        //if LowerValue or UpperValue will not set before start of App, then width of Control will be
        //calculated incorrect. Thats why we use CheckLowerValue and CheckUpperValue inside OnMaximum and OnMinimum
        private void CheckLowerValue()
        {
            if (LowerValue < Minimum)
            {
                LowerValue = Minimum;
            }
            else if (LowerValue > Maximum)
            {
                LowerValue = Maximum;
            }
        }

        private void CheckUpperValue()
        {
            if (UpperValue > Maximum)
            {
                UpperValue = Maximum;
            }
            else if (UpperValue < Minimum)
            {
                UpperValue = Minimum;
            }
        }

        /*
        private static void RangeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var slider = (RangeSlider)dependencyObject;
            if (slider._internalUpdate)
                return;

            slider.ReCalculateRanges();
            slider.ReCalculateWidths();
        }*/

        private static void RangesChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var slider = (RangeSlider)dependencyObject;
            if (slider._internalUpdate)
                return;

            slider.ReCalculateWidths();
            slider.OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(slider));
        }

        private static void MinBridgeWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var slider = (RangeSlider)sender;
            if (slider.Orientation == Orientation.Horizontal)
            {
                slider._centerThumb.MinWidth = slider.MinBridgeWidth;
            }
            else
            {
                slider._centerThumb.MinHeight = slider.MinBridgeWidth;
            }
            slider.ReCalculateWidths();
        }

        private static void MinRangeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((Double)e.NewValue < 0)
                throw new ArgumentOutOfRangeException("value", "value for MinRange cannot be less than 0");
            

            var slider = (RangeSlider)sender;
            if (slider._internalUpdate)
                return;


            slider._internalUpdate = true;
            slider.UpperValue = Math.Max(slider.UpperValue, slider.LowerValue + (double)e.NewValue);
            slider.Maximum = Math.Max(slider.Maximum, slider.UpperValue);
            slider._internalUpdate = false;

            slider.ReCalculateRanges();
            slider.ReCalculateWidths();
        }

        void MoveAllBackHandler(object sender, ExecutedRoutedEventArgs e)
        {
            ResetSelection(true);
        }

        void MoveAllForwardHandler(object sender, ExecutedRoutedEventArgs e)
        {
            ResetSelection(false);
        }

        void MoveBackHandler(object sender, ExecutedRoutedEventArgs e)
        {
            
            MoveSelection(true);
        }

        void MoveForwardHandler(object sender, ExecutedRoutedEventArgs e)
        {
            MoveSelection(false);
        }

        private static void MoveThumb(FrameworkElement x, FrameworkElement y, double horizonalChange, Orientation orientation)
        {
            double change = 0;
            if (orientation == Orientation.Horizontal)
            {
                if (horizonalChange < 0) //slider went left
                {
                    change = GetChangeKeepPositive(x.Width, horizonalChange);
                    if (x.Name == "PART_MiddleThumb")
                    {
                        if (x.Width > x.MinWidth)
                        {
                            if (x.Width + change < x.MinWidth)
                            {
                                double dif = x.Width - x.MinWidth;
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
                    change = -GetChangeKeepPositive(y.Width, -horizonalChange);
                    if (y.Name == "PART_MiddleThumb")
                    {
                        if (y.Width > y.MinWidth)
                        {
                            if (y.Width - change < y.MinWidth)
                            {
                                double dif = y.Width - y.MinWidth;
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
            else if (orientation == Orientation.Vertical)
            {
                if (horizonalChange < 0) //slider went left
                {
                    change = GetChangeKeepPositive(x.Height, horizonalChange);
                    if (x.Name == "PART_MiddleThumb")
                    {
                        if (x.Height > x.MinHeight)
                        {
                            if (x.Height + change < x.MinHeight)
                            {
                                double dif = x.Height - x.MinHeight;
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
                else if (horizonalChange > 0) //slider went right if(horizontal change == 0 do nothing)
                {
                    change = -GetChangeKeepPositive(y.Height, -horizonalChange);
                    if (y.Name == "PART_MiddleThumb")
                    {
                        if (y.Height > y.MinHeight)
                        {
                            if (y.Height - change < y.MinHeight)
                            {
                                double dif = y.Height - y.MinHeight;
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
            }
        }

        private static double GetChangeKeepPositive(double width, double increment)
        {
            return Math.Max(width + increment, 0) - width;
        }

        private void ReCalculateRanges()
        {
            //_movableRange = Maximum - Minimum - MinRange;
        }

        public double MovableRange
        {
            get
            {
                return Maximum - Minimum - MinRange;
            }
        }

        
        private void ReCalculateWidths()
        {
            if (_leftButton != null && _rightButton != null && _centerThumb != null)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    _movableWidth =
                        Math.Max(
                            ActualWidth - _rightThumb.ActualWidth - _leftThumb.ActualWidth - _centerThumb.MinWidth, 1);
                    _leftButton.Width = Math.Max(_movableWidth * (LowerValue - Minimum) / MovableRange, 0);
                    _rightButton.Width = Math.Max(_movableWidth * (Maximum - UpperValue) / MovableRange, 0);
                    _centerThumb.Width =
                        Math.Max(
                            ActualWidth - _leftButton.Width - _rightButton.Width - _rightThumb.ActualWidth -
                            _leftThumb.ActualWidth, 0);
                }
                else if (Orientation == Orientation.Vertical)
                {
                    _movableWidth =
                        Math.Max(
                            ActualHeight - _rightThumb.ActualHeight - _leftThumb.ActualHeight - _centerThumb.MinHeight, 1);
                    _leftButton.Height = Math.Max(_movableWidth * (LowerValue - Minimum) / MovableRange, 0);
                    _rightButton.Height = Math.Max(_movableWidth * (Maximum - UpperValue) / MovableRange, 0);
                    _centerThumb.Height =
                        Math.Max(
                            ActualHeight - _leftButton.Height - _rightButton.Height - _rightThumb.ActualHeight -
                            _leftThumb.ActualHeight, 0);
                }
                _density = _movableWidth / (Maximum - Minimum);
            }
        }

        private void ReCalculateRangeSelected(bool reCalculateLowerValue, bool reCalculateUpperValue)
        {
            Double oldLower = 0, oldUpper = 0;

            _internalUpdate = true; //set flag to signal that the properties are being set by the object itself
            if (reCalculateLowerValue)
            {
                oldLower = LowerValue;
                if (Orientation == Orientation.Horizontal)
                {
                    // Make sure to get exactly rangestart if thumb is at the start
                    LowerValue = Equals(_leftButton.Width, 0.0)
                        ? Minimum
                        : Math.Max(Minimum, (Minimum + MovableRange*_leftButton.Width/_movableWidth));
                }
                else
                {
                    LowerValue = Equals(_leftButton.Height, 0.0)
                        ? Minimum
                        : Math.Max(Minimum, (Minimum + MovableRange*_leftButton.Height/_movableWidth));
                }
                
            }

            if (reCalculateUpperValue)
            {
                oldUpper = UpperValue;
                if (Orientation == Orientation.Horizontal)
                {
                    // Make sure to get exactly rangestop if thumb is at the end
                    UpperValue = Equals(_rightButton.Width, 0.0)
                        ? Maximum
                        : Math.Min(Maximum, (Maximum - MovableRange*_rightButton.Width/_movableWidth));
                }
                else
                {
                    UpperValue = Equals(_rightButton.Height, 0.0)
                        ? Maximum
                        : Math.Min(Maximum, (Maximum - MovableRange*_rightButton.Height/_movableWidth));
                }
            }

            _internalUpdate = false; //set flag to signal that the properties are being set by the object itself

            if (reCalculateLowerValue || reCalculateUpperValue)
            {
                //raise the RangeSelectionChanged event
                OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
            }

            if (reCalculateLowerValue && !Equals(oldLower, LowerValue))
                OnRangeParameterChanged(
                    new RangeParameterChangedEventArgs(RangeParameterChangeType.Lower, oldLower, LowerValue),
                    LowerValueChangedEvent);
            else if (reCalculateUpperValue && !Equals(oldUpper, UpperValue))
                OnRangeParameterChanged(
                    new RangeParameterChangedEventArgs(RangeParameterChangeType.Upper, oldUpper, UpperValue),
                    UpperValueChangedEvent);
        }

        private bool ApproximatelyEquals(double value1, double value2)
        {
            return Math.Abs(value1 - value2) <= Epsilon; 
        }


        private void ReCalculateRangeSelected(bool reCalculateLowerValue, bool reCalculateUpperValue, double value, Direction direction)
        {
            Double oldLower = 0, oldUpper = 0;

            _internalUpdate = true; //set flag to signal that the properties are being set by the object itself
            if (reCalculateLowerValue)
            {
                oldLower = LowerValue;
                double lower;
                if (Orientation == Orientation.Horizontal)
                {
                    // Make sure to get exactly rangestart if thumb is at the start
                    lower = Equals(_leftButton.Width, 0.0)
                        ? Minimum
                        : Math.Max(Minimum, (Minimum + MovableRange * _leftButton.Width / _movableWidth));
                }
                else
                {
                    lower = Equals(_leftButton.Height, 0.0)
                        ? Minimum
                        : Math.Max(Minimum, (Minimum + MovableRange * _leftButton.Height / _movableWidth));
                }
                if (IsSnapToTickEnabled)
                {
                    Debug.WriteLine("LowerValue = " + lower);
                    Debug.WriteLine("Value = " + value+"\n\n\n\n");
                    if (ApproximatelyEquals(lower, value))
                    {
                        if (direction == Direction.Increase)
                        {
                            LowerValue = Math.Min(value, UpperValue - MinRange);
                        }
                        else
                        {
                            Debug.WriteLine("LowerValue = " + LowerValue);
                            Debug.WriteLine("UpperValue = " + UpperValue);
                            Debug.WriteLine("value = " + value);
                            Debug.WriteLine("upper = " + lower);
                            LowerValue = Math.Max(Minimum, value);
                            Debug.WriteLine("UpperValue = " + UpperValue);
                        }
                    }
                    else
                    {
                        if (direction == Direction.Increase)
                        {
                            LowerValue = Math.Min(UpperValue-MinRange, value);
                        }
                        else
                        {
                            LowerValue = Math.Max(Minimum, value);
                        }
                    }
                }
            }

            if (reCalculateUpperValue)
            {
                oldUpper = UpperValue;
                double upper;
                if (Orientation == Orientation.Horizontal)
                {
                    // Make sure to get exactly rangestop if thumb is at the end
                    upper = Equals(_rightButton.Width, 0.0)
                        ? Maximum
                        : Math.Min(Maximum, (Maximum - MovableRange * _rightButton.Width / _movableWidth));
                }
                else
                {
                    upper = Equals(_rightButton.Height, 0.0)
                        ? Maximum
                        : Math.Min(Maximum, (Maximum - MovableRange * _rightButton.Height / _movableWidth));
                }
                if (IsSnapToTickEnabled)
                {
                    Debug.WriteLine("UpperValue = " + upper);
                    Debug.WriteLine("Value = " + value + "\n\n\n\n");
                    if (ApproximatelyEquals(upper, value))
                    {
                        if (direction == Direction.Increase)
                        {
                            UpperValue = Math.Min(value, Maximum);
                        }
                        else
                        {
                            Debug.WriteLine("LowerValue = " + LowerValue);
                            Debug.WriteLine("UpperValue = " + UpperValue);
                            Debug.WriteLine("value = " + value);
                            Debug.WriteLine("upper = " + upper);
                            UpperValue = Math.Max(LowerValue + MinRange, value);
                            Debug.WriteLine("UpperValue = " + UpperValue);
                        }
                    }
                    else
                    {
                        {
                            if (direction == Direction.Increase)
                            {
                                UpperValue = Math.Min(value, Maximum);
                            }
                            else
                            {
                                Debug.WriteLine("LowerValue = "+LowerValue);
                                Debug.WriteLine("UpperValue = " + UpperValue);
                                Debug.WriteLine("value = " + value);
                                Debug.WriteLine("upper = " + upper);
                                UpperValue = Math.Max(LowerValue+MinRange, value);
                                Debug.WriteLine("UpperValue = " + UpperValue);
                            }
                        }
                    }
                }
            }

            _internalUpdate = false; //set flag to signal that the properties are being set by the object itself

            if (reCalculateLowerValue || reCalculateUpperValue)
            {
                //raise the RangeSelectionChanged event
                OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
            }

            if (reCalculateLowerValue && !Equals(oldLower, LowerValue))
                OnRangeParameterChanged(
                    new RangeParameterChangedEventArgs(RangeParameterChangeType.Lower, oldLower, LowerValue),
                    LowerValueChangedEvent);
            else if (reCalculateUpperValue && !Equals(oldUpper, UpperValue))
                OnRangeParameterChanged(
                    new RangeParameterChangedEventArgs(RangeParameterChangeType.Upper, oldUpper, UpperValue),
                    UpperValueChangedEvent);
        }

        private void ReCalculateRangeSelected(double newLower, double newUpper, Direction direction)
        {
            Double oldLower = 0, oldUpper = 0;
            Debug.WriteLine("\n\n\n\n\n\nUpperValue = " + UpperValue);
            Debug.WriteLine("LowerValue = " + LowerValue);
            Debug.WriteLine("newLower = " + newLower);
            Debug.WriteLine("newUpper = " + newUpper);
            _internalUpdate = true; //set flag to signal that the properties are being set by the object itself
                oldLower = LowerValue;
                double lower;
                oldUpper = UpperValue;
                double upper;
                
            if (IsSnapToTickEnabled)
            {
                if (direction == Direction.Increase)
                {
                    Debug.WriteLine("зашёл внутрь encreasa");
                    lower = Math.Min(newLower, Maximum-(UpperValue - LowerValue));
                    upper = Math.Min(newUpper, Maximum);
                    Debug.WriteLine("lower = " + lower);
                    Debug.WriteLine("upper = " + upper);
                }
                else
                {
                    Debug.WriteLine("зашёл внутрь decreasa");
                    lower = Math.Max(newLower, Minimum);
                    upper = Math.Max(Minimum+(UpperValue - LowerValue), newUpper);
                    Debug.WriteLine("lower = " + lower);
                    Debug.WriteLine("upper = " + upper);
                }
                LowerValue = lower;
                UpperValue = upper;
                Debug.WriteLine("LowerValue = " + LowerValue);
                Debug.WriteLine("UpperValue = " + UpperValue);
                Debug.WriteLine("\n\n\n\n\n\n");
            }
            _internalUpdate = false; //set flag to signal that the properties are being set by the object itself

            {
                //raise the RangeSelectionChanged event
                OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
            }

            if (!Equals(oldLower, LowerValue))
                OnRangeParameterChanged(
                    new RangeParameterChangedEventArgs(RangeParameterChangeType.Lower, oldLower, LowerValue),
                    LowerValueChangedEvent);
            else if ( !Equals(oldUpper, UpperValue))OnRangeParameterChanged(
                    new RangeParameterChangedEventArgs(RangeParameterChangeType.Upper, oldUpper, UpperValue),
                    UpperValueChangedEvent);
        }


        private void OnRangeParameterChanged(RangeParameterChangedEventArgs e, RoutedEvent Event)
        {
            e.RoutedEvent = Event;
            RaiseEvent(e);
        }


        public void MoveSelection(bool isLeft)
        {
            double widthChange = SmallChange * (UpperValue - LowerValue)
                * _movableWidth / MovableRange;

            widthChange = isLeft ? -widthChange : widthChange;
            MoveThumb(_leftButton, _rightButton, widthChange, Orientation);
            ReCalculateRangeSelected(true, true);
        }

        public void ResetSelection(bool isStart)
        {
            double widthChange = Maximum - Minimum;
            widthChange = isStart ? -widthChange : widthChange;

            MoveThumb(_leftButton, _rightButton, widthChange, Orientation);
            ReCalculateRangeSelected(true, true);
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
            _topTick = EnforceInstance<TickBar>("PART_TopTick");
            _bottomTick = EnforceInstance<TickBar>("PART_BottomTick");
            InitializeVisualElementsContainer();
            ReCalculateWidths();
        }

        T EnforceInstance<T>(string partName) where T : FrameworkElement, new()
        {
            T element = GetTemplateChild(partName) as T ?? new T();
            return element;
        }

        //adds all visual element to the conatiner
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

            _leftButton.PreviewMouseLeftButtonDown += LeftButtonPreviewMouseLeftButtonDown;
            _rightButton.PreviewMouseLeftButtonDown += RightButtonPreviewMouseLeftButtonDown;

            _visualElementsContainer.PreviewMouseUp += VisualElementsContainerPreviewMouseUp;
            _visualElementsContainer.MouseLeave += VisualElementsContainerMouseLeave;

            _centerThumb.PreviewMouseDown += CenterThumbPreviewMouseDown;

        }


        void VisualElementsContainerMouseLeave(object sender, MouseEventArgs e)
        {
            _tickCount = 0;
            _timer.Stop();
        }

        void VisualElementsContainerPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _tickCount = 0;
            _timer.Stop();
        }

        void RightButtonPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Point p = Mouse.GetPosition(_rightButton);
                double change = Orientation == Orientation.Horizontal
                    ? p.X + (_rightThumb.ActualWidth/2)
                    : p.Y + (_rightThumb.ActualHeight/2);
                if (!IsSnapToTickEnabled)
                {
                    if (IsMoveToPointEnabled && !MoveWholeRange)
                    {
                        MoveThumb(_centerThumb, _rightButton, change, Orientation);
                        ReCalculateRangeSelected(false, true);
                    }
                    else if (IsMoveToPointEnabled && MoveWholeRange)
                    {
                        MoveThumb(_leftButton, _rightButton, change, Orientation);
                        ReCalculateRangeSelected(true, true);
                    }
                }
                else
                {
                    if (IsMoveToPointEnabled && !MoveWholeRange)
                    {
                        JumpToNextTick(Direction.Increase, ButtonType.Right, change, UpperValue);
                    }
                    else if (IsMoveToPointEnabled && MoveWholeRange)
                    {
                        JumpToNextTick(Direction.Increase, ButtonType.Both, change, UpperValue);
                    }
                }
                if (!IsMoveToPointEnabled)
                {
                    _position = Mouse.GetPosition(_visualElementsContainer);
                    _bType = MoveWholeRange ? ButtonType.Both : ButtonType.Right;
                    _currentpoint = Orientation == Orientation.Horizontal ? _position.X : _position.Y;
                    _currenValue = UpperValue;
                    _direction = Direction.Increase;
                    _isInsideRange = false;
                    _timer.Start();
                }
            }
        }

        void LeftButtonPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = Mouse.GetPosition(_leftButton);
                double change = Orientation == Orientation.Horizontal
                    ? _leftButton.ActualWidth - p.X + (_leftThumb.ActualWidth/2)
                    : _leftButton.ActualHeight - p.Y + (_leftThumb.ActualHeight/2);
                if (!IsSnapToTickEnabled)
                {
                    if (IsMoveToPointEnabled && !MoveWholeRange)
                    {
                        MoveThumb(_leftButton, _centerThumb, -change, Orientation);
                        ReCalculateRangeSelected(true, false);
                    }
                    else if (IsMoveToPointEnabled && MoveWholeRange)
                    {
                        MoveThumb(_leftButton, _rightButton, -change, Orientation);
                        ReCalculateRangeSelected(true, true);
                    }
                }
                else
                {
                    if (IsMoveToPointEnabled && !MoveWholeRange)
                    {
                        JumpToNextTick(Direction.Decrease, ButtonType.Left, -change, LowerValue);
                    }
                    else if (IsMoveToPointEnabled && MoveWholeRange)
                    {
                        JumpToNextTick(Direction.Decrease, ButtonType.Both, -change, LowerValue);
                    }
                }
                if (!IsMoveToPointEnabled)
                {
                    _position = Mouse.GetPosition(_visualElementsContainer);
                    _bType = MoveWholeRange ? ButtonType.Both : ButtonType.Left;
                    _currentpoint = Orientation == Orientation.Horizontal ? _position.X : _position.Y;
                    _currenValue = LowerValue;
                    _isInsideRange = false;
                    _direction = Direction.Decrease;
                    _timer.Start();
                }
            }
        }

        void CenterThumbPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ExtendedMode)
            {
                if (e.LeftButton == MouseButtonState.Pressed && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    Point p = Mouse.GetPosition(_centerThumb);
                    double change = Orientation == Orientation.Horizontal
                    ? p.X + (_leftThumb.ActualWidth / 2)
                    : p.Y + (_leftThumb.ActualHeight / 2);
                    if (!IsSnapToTickEnabled)
                    {
                        if (IsMoveToPointEnabled && !MoveWholeRange)
                        {
                            MoveThumb(_leftButton, _centerThumb, change, Orientation);
                            ReCalculateRangeSelected(true, false);
                        }
                        else if (IsMoveToPointEnabled && MoveWholeRange)
                        {
                            MoveThumb(_leftButton, _rightButton, change, Orientation);
                            ReCalculateRangeSelected(true, true);
                        }
                    }
                    else
                    {
                        if (IsMoveToPointEnabled && !MoveWholeRange)
                        {
                            JumpToNextTick(Direction.Increase, ButtonType.Left, change, LowerValue);
                        }
                        else if (IsMoveToPointEnabled && MoveWholeRange)
                        {
                            JumpToNextTick(Direction.Increase, ButtonType.Both, change, LowerValue);
                        }
                    }
                    if (!IsMoveToPointEnabled)
                    {
                        _position = Mouse.GetPosition(_visualElementsContainer);
                        _bType = MoveWholeRange ? ButtonType.Both : ButtonType.Left;
                        _currentpoint = Orientation == Orientation.Horizontal ? _position.X : _position.Y;
                        _currenValue = LowerValue;
                        _direction = Direction.Increase;
                        _isInsideRange = true;
                        _timer.Start();
                    }
                }
                else if (e.RightButton == MouseButtonState.Pressed && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    Point p = Mouse.GetPosition(_centerThumb);
                    double change = Orientation == Orientation.Horizontal
                    ? _centerThumb.ActualWidth - p.X + (_rightThumb.ActualWidth / 2)
                    : _centerThumb.ActualHeight - p.Y + (_rightThumb.ActualHeight / 2);
                    if (!IsSnapToTickEnabled)
                    {
                        if (IsMoveToPointEnabled && !MoveWholeRange)
                        {
                            MoveThumb(_centerThumb, _rightButton, -change, Orientation);
                            ReCalculateRangeSelected(false, true);
                        }
                        else if (IsMoveToPointEnabled && MoveWholeRange)
                        {
                            MoveThumb(_leftButton, _rightButton, -change, Orientation);
                            ReCalculateRangeSelected(true, true);
                        }
                    }
                    else
                    {
                        if (IsMoveToPointEnabled && !MoveWholeRange)
                        {
                            JumpToNextTick(Direction.Decrease, ButtonType.Right, -change, UpperValue);
                        }
                        else if (IsMoveToPointEnabled && MoveWholeRange)
                        {
                            JumpToNextTick(Direction.Decrease, ButtonType.Both, -change, UpperValue);
                        }
                    }
                    if (!IsMoveToPointEnabled)
                    {
                        _position = Mouse.GetPosition(_visualElementsContainer);
                        _bType = MoveWholeRange ? ButtonType.Both : ButtonType.Right;
                        _currentpoint = Orientation == Orientation.Horizontal ? _position.X : _position.Y;
                        _currenValue = UpperValue;
                        _direction = Direction.Decrease;
                        _isInsideRange = true;
                        _timer.Start();
                    }
                }
            }
        }


        //Method updates end point, which is needed to correctly compare current position on the thumb with
        //current width of button
        private double UpdateEndPoint(ButtonType type, Direction dir)
        {
            double d = 0;
            if (dir == Direction.Increase)
            {
                if (type == ButtonType.Left)
                {
                    d = Orientation == Orientation.Horizontal
                ? _leftButton.ActualWidth + _leftThumb.ActualWidth
                : _leftButton.ActualHeight + _leftThumb.ActualHeight;
                }
                else if (type == ButtonType.Right)
                {
                    d = Orientation == Orientation.Horizontal
                ? ActualWidth - _rightButton.ActualWidth
                : ActualHeight - _rightButton.ActualHeight;
                }
                else if (type == ButtonType.Both)
                {
                    if (!_isInsideRange)
                    {
                        d = Orientation == Orientation.Horizontal
                            ? ActualWidth - _rightButton.ActualWidth
                            : ActualHeight - _rightButton.ActualHeight;
                    }
                    else
                    {
                        d = Orientation == Orientation.Horizontal
                            ? _leftButton.ActualWidth + _leftThumb.ActualWidth
                            : _leftButton.ActualHeight + _leftThumb.ActualHeight;
                    }
                }
            }
            else if (dir == Direction.Decrease)
            {
                if (type == ButtonType.Left)
                {
                    d = Orientation == Orientation.Horizontal
                ? _leftButton.ActualWidth
                : _leftButton.ActualHeight;
                }
                if (type == ButtonType.Right)
                {
                    d = Orientation == Orientation.Horizontal
                ? ActualWidth - _rightButton.ActualWidth - _rightThumb.ActualWidth
                : ActualHeight - _rightButton.ActualHeight - _rightThumb.ActualHeight;
                }
                else if (type == ButtonType.Both)
                {
                    if (!_isInsideRange)
                    {
                        d = Orientation == Orientation.Horizontal
                            ? _leftButton.ActualWidth
                            : _leftButton.ActualHeight;
                    }
                    else
                    {
                        d = Orientation == Orientation.Horizontal
                            ? ActualWidth - _rightButton.ActualWidth - _rightThumb.ActualWidth
                            : ActualHeight - _rightButton.ActualHeight - _rightThumb.ActualHeight;
                    }
                }
            }
            return d;
        }


        //This is timer event, which starts when IsMoveToPoint = false
        //Supports IsSnapToTick option
        void SerialMovement(object sender, EventArgs e)
        {
            double _endpoint = UpdateEndPoint(_bType, _direction);
            double widthChange = 0;
            if (!IsSnapToTickEnabled)
            {
                widthChange = SmallChange;
                if (_tickCount > 10)
                {
                    widthChange = LargeChange;
                }
                if (_direction == Direction.Increase)
                {
                    if (_currentpoint > _endpoint)
                    {
                        if (_bType == ButtonType.Left)
                        {
                            MoveThumb(_leftButton, _centerThumb, widthChange, Orientation);
                            ReCalculateRangeSelected(true, false);
                        }
                        else if (_bType == ButtonType.Right)
                        {
                            MoveThumb(_centerThumb, _rightButton, widthChange, Orientation);
                            ReCalculateRangeSelected(false, true);
                        }
                        else if (_bType == ButtonType.Both)
                        {
                            MoveThumb(_leftButton, _rightButton, widthChange, Orientation);
                            ReCalculateRangeSelected(true, true);
                        }
                    }
                    else
                    {
                        _tickCount = 0;
                        _timer.Stop();
                    }
                }
                else if (_direction == Direction.Decrease)
                {
                    if (_currentpoint < _endpoint)
                    {
                        if (_bType == ButtonType.Left)
                        {
                            MoveThumb(_leftButton, _centerThumb, -widthChange, Orientation);
                            ReCalculateRangeSelected(true, false);
                        }
                        else if (_bType == ButtonType.Right)
                        {
                            MoveThumb(_centerThumb, _rightButton, -widthChange, Orientation);
                            ReCalculateRangeSelected(false, true);
                        }
                        else if (_bType == ButtonType.Both)
                        {
                            MoveThumb(_leftButton, _rightButton, -widthChange, Orientation);
                            ReCalculateRangeSelected(true, true);
                        }
                    }
                    else
                    {
                        _tickCount = 0;
                        _timer.Stop();
                    }
                }
            }
            else
            {
                widthChange = CalculateNextTick(_direction, _currenValue, 0, true);
                if (_tickCount%2 == 0)
                {
                    if (_direction == Direction.Increase)
                    {
                        if (_currentpoint > _endpoint)
                        {
                            if (_bType == ButtonType.Left)
                            {
                                MoveThumb(_leftButton, _centerThumb, widthChange*_density, Orientation);
                                ReCalculateRangeSelected(true, false, LowerValue + widthChange, _direction);
                            }
                            else if (_bType == ButtonType.Right)
                            {
                                MoveThumb(_centerThumb, _rightButton, widthChange * _density, Orientation);
                                ReCalculateRangeSelected(false, true, UpperValue + widthChange, _direction);
                            }
                            else if (_bType == ButtonType.Both)
                            {
                                MoveThumb(_leftButton, _rightButton, widthChange * _density, Orientation);
                                ReCalculateRangeSelected(LowerValue + widthChange, UpperValue+widthChange, _direction);
                            }
                        }
                        else
                        {
                            _tickCount = 0;
                            _timer.Stop();
                        }
                    }
                    else if (_direction == Direction.Decrease)
                    {
                        if (_currentpoint < _endpoint)
                        {
                            if (_bType == ButtonType.Left)
                            {
                                MoveThumb(_leftButton, _centerThumb, -widthChange*_density, Orientation);
                                ReCalculateRangeSelected(true, false, LowerValue - widthChange, _direction);
                            }
                            else if (_bType == ButtonType.Right)
                            {
                                MoveThumb(_centerThumb, _rightButton, -widthChange * _density, Orientation);
                                ReCalculateRangeSelected(false, true, UpperValue - widthChange, _direction);
                            }
                            else if (_bType == ButtonType.Both)
                            {
                                MoveThumb(_leftButton, _rightButton, -widthChange * _density, Orientation);
                                ReCalculateRangeSelected(LowerValue - widthChange, UpperValue - widthChange, _direction);
                            }
                        }
                        else
                        {
                            _tickCount = 0;
                            _timer.Stop();
                        }
                    }
                }
            }
            _tickCount++;
        }


        


        private Double CalculateNextTick(Direction dir, double chekingValue, double distance, bool moveDirectlyToNextTick)
        {
            Debug.WriteLine("\n\n\n\ndistance = " + distance);
            Debug.WriteLine("TickFrequency = " + TickFrequency);
            Debug.WriteLine("chekingValue = " + chekingValue);
            Debug.WriteLine("chekingValue / TickFrequency = " + chekingValue / TickFrequency);
            Debug.WriteLine("ToInt = " + IsDoubleCloseToInt(chekingValue / TickFrequency));
            if (!IsMoveToPointEnabled)
            {
                if (!IsDoubleCloseToInt((chekingValue - Minimum)/TickFrequency))
                {
                    double x = (chekingValue - Minimum)/TickFrequency;
                    distance = TickFrequency*(int) x;
                    Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!x = " + x.ToString());
                    if (dir == Direction.Increase)
                    {
                        distance += TickFrequency;
                    }
                    distance = (distance - Math.Abs(chekingValue - Minimum));
                    Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!Distance---> = " + distance);
                    _currenValue = 0;
                    return Math.Abs(distance);
                }
            }
            if (moveDirectlyToNextTick)
            {
                distance = TickFrequency;
            }
            else
            {
                Debug.WriteLine("distance = " + distance);
                Debug.WriteLine("chekingValue = " + chekingValue);
                double currentValue = chekingValue - Minimum + (distance/_density); //в единицах
                Debug.WriteLine("currentValue = " + currentValue);
                double x = currentValue/TickFrequency;
                Debug.WriteLine("x = " + x);
                if (dir == Direction.Increase)
                {
                    double nextvalue = x.ToString().ToLower().Contains("e+")
                        ? (x*TickFrequency) + TickFrequency
                        : ((int) x*TickFrequency) + TickFrequency;

                    Debug.WriteLine("nextvalue = " + nextvalue);
                    distance = (nextvalue - Math.Abs(chekingValue - Minimum));
                    Debug.WriteLine("distance = " + distance);
                }
                else
                {
                    double previousValue = x.ToString().ToLower().Contains("e+")
                        ? x*TickFrequency
                        : (int) x*TickFrequency;
                    Debug.WriteLine("previousValue = " + previousValue);
                    distance = (Math.Abs(chekingValue - Minimum) - previousValue);
                    Debug.WriteLine("distance = " + distance);
                }

            }
            return Math.Abs(distance);
        }

        private void JumpToNextTick(Direction mDirection, ButtonType type, double distance, double chekingValue)
        {
            double difference = CalculateNextTick(mDirection, chekingValue, distance, false);

            if (mDirection == Direction.Increase)
            {
                if (!IsDoubleCloseToInt(chekingValue / TickFrequency))
                {
                    if (distance > (difference* _density) / 2)
                    {
                        if (type == ButtonType.Right)
                        {
                            MoveThumb(_centerThumb, _rightButton, difference*_density, Orientation);
                            ReCalculateRangeSelected(false, true, UpperValue + difference, mDirection);
                        }
                        else if (type == ButtonType.Left)
                        {
                            MoveThumb(_leftButton, _centerThumb, difference * _density, Orientation);
                            ReCalculateRangeSelected(true, false, LowerValue + difference, mDirection);
                        }
                        else if (type == ButtonType.Both)
                        {
                            MoveThumb(_leftButton, _rightButton, difference * _density, Orientation);
                            ReCalculateRangeSelected(LowerValue + difference, UpperValue + difference, mDirection);
                        }
                    }
                }
                else
                {
                    Point p = Mouse.GetPosition(_container);
                    double pos = Orientation == Orientation.Horizontal ? p.X : p.Y;
                    double widthHeight = Orientation == Orientation.Horizontal ? ActualWidth : ActualHeight;
                    double tickIntervalInPixels = TickFrequency * _density;
                    if ((distance > tickIntervalInPixels / 2) || widthHeight - pos < (tickIntervalInPixels / 2))
                    {
                        if (type == ButtonType.Right)
                        {
                            MoveThumb(_centerThumb, _rightButton, difference*_density, Orientation);
                            ReCalculateRangeSelected(false, true, UpperValue + difference, mDirection);
                        }
                        else if (type == ButtonType.Left)
                        {
                            MoveThumb(_leftButton, _centerThumb, difference * _density, Orientation);
                            ReCalculateRangeSelected(true, false, LowerValue + difference, mDirection);
                        }
                        else if (type == ButtonType.Both)
                        {
                            MoveThumb(_leftButton, _rightButton, difference * _density, Orientation);
                            ReCalculateRangeSelected(LowerValue + difference, UpperValue + difference, mDirection);
                        }
                    }
                }
            }
            else
            {
                if (!IsDoubleCloseToInt(chekingValue / TickFrequency))
                {
                    if ((distance <= -(difference*_density) / 2))
                    {
                        if (type == ButtonType.Right)
                        {
                            MoveThumb(_centerThumb, _rightButton, -difference * _density, Orientation);
                            ReCalculateRangeSelected(false, true, UpperValue - difference, mDirection);
                        }
                        else if (type == ButtonType.Left)
                        {
                            MoveThumb(_leftButton, _centerThumb, -difference * _density, Orientation);
                            ReCalculateRangeSelected(true, false, LowerValue - difference, mDirection);
                        }
                        else if (type == ButtonType.Both)
                        {
                            MoveThumb(_leftButton, _rightButton, -difference * _density, Orientation);
                            ReCalculateRangeSelected(LowerValue - difference, UpperValue - difference, mDirection);
                        }
                    }
                }
                else
                {
                    Point p = Mouse.GetPosition(_container);
                    double pos = Orientation == Orientation.Horizontal ? p.X : p.Y;
                    double tickIntervalInPixels = -TickFrequency * _density;
                    if (distance < tickIntervalInPixels / 2 || pos < (-tickIntervalInPixels / 2))
                    {
                        if (type == ButtonType.Right)
                        {
                            MoveThumb(_centerThumb, _rightButton, -difference * _density, Orientation);
                            ReCalculateRangeSelected(false, true, UpperValue - difference, mDirection);
                        }
                        else if (type == ButtonType.Left)
                        {
                            MoveThumb(_leftButton, _centerThumb, -difference * _density, Orientation);
                            ReCalculateRangeSelected(true, false, LowerValue - difference, mDirection);
                        }
                        else if (type == ButtonType.Both)
                        {
                            MoveThumb(_leftButton, _rightButton, -difference * _density, Orientation);
                            ReCalculateRangeSelected(LowerValue - difference, UpperValue - difference, mDirection);
                        }
                    }
                }
            }
        }



        #region Event Handlers

        private void LeftThumbDragStart(object sender, DragStartedEventArgs e)
        {
            if (AutoToolTipPlacement != AutoToolTipPlacement.None)
            {
                if (_autoToolTip == null)
                {
                    _autoToolTip = new ToolTip();
                    _autoToolTip.Placement = PlacementMode.Custom;
                    _autoToolTip.CustomPopupPlacementCallback = PopupPlacementCallback;
                }
                _autoToolTip.Content = GetLowerToolTipNumber();
                _leftThumb.ToolTip = _autoToolTip.Content;
                _autoToolTip.PlacementTarget = _leftThumb;
                _autoToolTip.IsOpen = true;
            }
            _basePoint = Mouse.GetPosition(_container);
            e.RoutedEvent = LowerThumbDragStartedEvent;
            RaiseEvent(e);
        }


        private void LeftThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            double change = Orientation == Orientation.Horizontal
                ? e.HorizontalChange
                : e.VerticalChange;
            if (!IsSnapToTickEnabled)
            {
                MoveThumb(_leftButton, _centerThumb, change, Orientation);
                ReCalculateRangeSelected(true, false);
            }
            else
            {
                Direction localDirection;
                Point currentPoint = Mouse.GetPosition(_container);
                if (Orientation == Orientation.Horizontal)
                {
                    if (currentPoint.X >= 0 &&
                        currentPoint.X <
                        _container.ActualWidth -
                        (_rightButton.ActualWidth + _rightThumb.ActualWidth + _centerThumb.MinWidth))
                    {
                        localDirection = currentPoint.X > _basePoint.X ? Direction.Increase : Direction.Decrease;
                        JumpToNextTick(localDirection, ButtonType.Left, change, LowerValue);
                    }
                }
                else
                {
                    if (currentPoint.Y >= 0 &&
                        currentPoint.Y <
                        _container.ActualHeight -
                        (_rightButton.ActualHeight + _rightThumb.ActualHeight + _centerThumb.MinHeight))
                    {
                        localDirection = currentPoint.Y > _basePoint.Y ? Direction.Increase : Direction.Decrease;
                        JumpToNextTick(localDirection, ButtonType.Left, change, LowerValue);
                    }
                }
            }
            _basePoint = Mouse.GetPosition(_container);
            if (AutoToolTipPlacement != AutoToolTipPlacement.None)
            {
                _leftThumb.ToolTip = _autoToolTip;
                _autoToolTip.Content = GetLowerToolTipNumber();
                RelocateAutoToolTip();
            }
            
            e.RoutedEvent = LowerThumbDragDeltaEvent;
            RaiseEvent(e);
        }

        private void RelocateAutoToolTip()
        {
            var offset = _autoToolTip.HorizontalOffset;
            _autoToolTip.HorizontalOffset = offset + 1;
            _autoToolTip.HorizontalOffset = offset;
        }

        private void LeftThumbDragComplete(object sender, DragCompletedEventArgs e)
        {
            if (_autoToolTip != null)
            {
                _autoToolTip.IsOpen = false;
                _leftThumb.ToolTip = String.Empty;
                _autoToolTip = null;
            }
            e.RoutedEvent = LowerThumbDragCompletedEvent;
            RaiseEvent(e);
        }

        private void RightThumbDragStart(object sender, DragStartedEventArgs e)
        {
            if (AutoToolTipPlacement != AutoToolTipPlacement.None)
            {
                if (_autoToolTip == null)
                {
                    _autoToolTip = new ToolTip();
                    _autoToolTip.Placement = PlacementMode.Custom;
                    _autoToolTip.CustomPopupPlacementCallback = PopupPlacementCallback;
                }
                _autoToolTip.Content = GetUpperToolTipNumber();
                _rightThumb.ToolTip = _autoToolTip;
                _autoToolTip.PlacementTarget = _rightThumb;
                _autoToolTip.IsOpen = true;
            }
            _basePoint = Mouse.GetPosition(_container);
            e.RoutedEvent = UpperThumbDragStartedEvent;
            RaiseEvent(e);
        }


        private void RightThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            double change = Orientation == Orientation.Horizontal
                ? e.HorizontalChange
                : e.VerticalChange;
            if (!IsSnapToTickEnabled)
            {
                MoveThumb(_centerThumb, _rightButton, change, Orientation);
                ReCalculateRangeSelected(false, true);
            }
            else
            {
                Direction localDirection;
                Point currentPoint = Mouse.GetPosition(_container);
                if (Orientation == Orientation.Horizontal)
                {
                    if (currentPoint.X < _container.ActualWidth &&
                        currentPoint.X > _leftButton.ActualWidth + _leftThumb.ActualWidth + _centerThumb.MinWidth)
                    {
                        localDirection = currentPoint.X > _basePoint.X ? Direction.Increase : Direction.Decrease;
                        JumpToNextTick(localDirection, ButtonType.Right, change, UpperValue);
                    }
                }
                else
                {
                    if (currentPoint.Y < _container.ActualHeight &&
                        currentPoint.Y > _leftButton.ActualHeight + _leftThumb.ActualHeight + _centerThumb.MinHeight)
                    {
                        localDirection = currentPoint.Y > _basePoint.Y ? Direction.Increase : Direction.Decrease;
                        JumpToNextTick(localDirection, ButtonType.Right, change, UpperValue);
                    }
                }
                
                _basePoint = Mouse.GetPosition(_container);
            }
            if (AutoToolTipPlacement != AutoToolTipPlacement.None)
            {
                _autoToolTip.Content = GetUpperToolTipNumber();
                _leftThumb.ToolTip = _autoToolTip;
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
                _rightThumb.ToolTip = String.Empty;
                _autoToolTip = null;
            }
            e.RoutedEvent = UpperThumbDragCompletedEvent;
            RaiseEvent(e);
        }

        private void CenterThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            if (AutoToolTipPlacement != AutoToolTipPlacement.None)
            {
                if (_autoToolTip == null)
                {
                    _autoToolTip = new ToolTip
                    {
                        Placement = PlacementMode.Custom,
                        CustomPopupPlacementCallback = PopupPlacementCallback
                    };
                }
                _autoToolTip.Content = GetLowerToolTipNumber() + " ; " + GetUpperToolTipNumber();
                _centerThumb.ToolTip = _autoToolTip.Content;
                _autoToolTip.PlacementTarget = _centerThumb;
                _autoToolTip.IsOpen = true;
            }
            _basePoint = Mouse.GetPosition(_container);
            e.RoutedEvent = CentralThumbDragStartedEvent;
            RaiseEvent(e);
        }

        private void CenterThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            double change = Orientation == Orientation.Horizontal
                ? e.HorizontalChange
                : e.VerticalChange;
            if (!IsSnapToTickEnabled)
            {
                MoveThumb(_leftButton, _rightButton, change, Orientation);
                ReCalculateRangeSelected(true, true);
            }
            else
            {
                Direction localDirection;
                Point currentPoint = Mouse.GetPosition(_container);
                if (Orientation == Orientation.Horizontal)
                {
                    if (currentPoint.X >= 0 &&
                        currentPoint.X < _container.ActualWidth)
                    {
                        localDirection = currentPoint.X > _basePoint.X ? Direction.Increase : Direction.Decrease;
                        JumpToNextTick(localDirection, ButtonType.Both, change,
                            localDirection == Direction.Increase ? UpperValue : LowerValue);
                    }
                }
                else
                {
                    if (currentPoint.Y >= 0 &&
                        currentPoint.Y < _container.ActualHeight)
                    {
                        localDirection = currentPoint.Y > _basePoint.Y ? Direction.Increase : Direction.Decrease;
                        JumpToNextTick(localDirection, ButtonType.Both, change,
                            localDirection == Direction.Increase ? UpperValue : LowerValue);
                    }
                }
            }
            _basePoint = Mouse.GetPosition(_container);
            if (AutoToolTipPlacement != AutoToolTipPlacement.None)
            {
                _autoToolTip.Content = GetLowerToolTipNumber() +" ; "+ GetUpperToolTipNumber();
                _centerThumb.ToolTip = _autoToolTip;
                RelocateAutoToolTip();
            }
            e.RoutedEvent = CentralThumbDragDeltaEvent;
            RaiseEvent(e);
        }

        private void CenterThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (_autoToolTip != null)
            {
                _autoToolTip.IsOpen = false;
                _centerThumb.ToolTip = String.Empty;
                _autoToolTip = null;
            }
            e.RoutedEvent = CentralThumbDragCompletedEvent;
            RaiseEvent(e);
        }

        #endregion



        #region Helper methods

        private Boolean IsDoubleCloseToInt(double val)
        {
            return ApproximatelyEquals(Math.Abs(val - Math.Round(val)), 0);
        }

        private String GetLowerToolTipNumber()
        {
            NumberFormatInfo format = (NumberFormatInfo) (NumberFormatInfo.CurrentInfo.Clone());
            format.NumberDecimalDigits = AutoToolTipPrecision;
            return LowerValue.ToString("N", format);
        }

        private String GetUpperToolTipNumber()
        {
            NumberFormatInfo format = (NumberFormatInfo) (NumberFormatInfo.CurrentInfo.Clone());
            format.NumberDecimalDigits = AutoToolTipPrecision;
            return UpperValue.ToString("N", format);
        }

        private CustomPopupPlacement[] PopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
        {
            switch (AutoToolTipPlacement)
            {
                case AutoToolTipPlacement.TopLeft:
                    if (Orientation == Orientation.Horizontal)
                    {
                        // Place popup at top of thumb
                        return new CustomPopupPlacement[]
                        {
                            new CustomPopupPlacement(
                                new Point((targetSize.Width - popupSize.Width)*0.5, -popupSize.Height),
                                PopupPrimaryAxis.Horizontal)
                        };
                    }
                    else
                    {
                        // Place popup at left of thumb 
                        return new CustomPopupPlacement[]
                        {
                            new CustomPopupPlacement(
                                new Point(-popupSize.Width, (targetSize.Height - popupSize.Height)*0.5),
                                PopupPrimaryAxis.Vertical)
                        };
                    }

                case AutoToolTipPlacement.BottomRight:
                    if (Orientation == Orientation.Horizontal)
                    {
                        // Place popup at bottom of thumb 
                        return new CustomPopupPlacement[]
                        {
                            new CustomPopupPlacement(
                                new Point((targetSize.Width - popupSize.Width)*0.5, targetSize.Height),
                                PopupPrimaryAxis.Horizontal)
                        };

                    }
                    else
                    {
                        // Place popup at right of thumb 
                        return new CustomPopupPlacement[]
                        {
                            new CustomPopupPlacement(
                                new Point(targetSize.Width, (targetSize.Height - popupSize.Height)*0.5),
                                PopupPrimaryAxis.Vertical)
                        };
                    }

                default:
                    return new CustomPopupPlacement[] {};
            }
        }

        #endregion



        #region Coerce callbacks

        private static object CoerceLowerValue(DependencyObject d, object basevalue)
        {
            RangeSlider rs = (RangeSlider) d;

            //double value = (double)basevalue;

            //if (value <= rs.Minimum)
            //    return rs.Minimum;
            //return Math.Min(value, rs.UpperValue);
            return (Double) basevalue;
            //return Math.Min(rs.Minimum, (Double)basevalue);
        }

        private static object CoerceUpperValue(DependencyObject d, object basevalue)
        {
            RangeSlider rs = (RangeSlider) d;

            //double value = (double)basevalue;

            //if (value >= rs.Maximum)
            //    return rs.Maximum;
            //return Math.Max(value, rs.LowerValue);
            return (Double) basevalue;
            //return Math.Min(rs.LowerValue, (Double)basevalue);
        }

        private static object CoerceMaximum(DependencyObject d, object basevalue)
        {
            RangeSlider rs = (RangeSlider) d;
            return (Double)basevalue;
            //return Math.Max(rs.UpperValue, (Double) basevalue);
        }

        

        private static object CoerceMinimum(DependencyObject d, object basevalue)
        {
            RangeSlider rs = (RangeSlider) d;
            return (Double)basevalue;
            //return Math.Min(rs.LowerValue, (Double) basevalue);
        }

        private static object CoerceMinBridgeWidth(DependencyObject d, object basevalue)
        {
            RangeSlider rs = (RangeSlider) d;
            double width = 0;
            if (rs.Orientation == Orientation.Horizontal)
            {
                width = rs.ActualWidth - rs._leftThumb.ActualWidth - rs._rightThumb.ActualWidth;
            }
            else
            {
                width = rs.ActualHeight - rs._leftThumb.ActualHeight - rs._rightThumb.ActualHeight;
            }
            return (Double) basevalue > width/2 ? width/2 : (Double) basevalue;
        }

        private static object CoerceTickFrequencyCallback(DependencyObject dependencyObject, object baseValue)
        {
            if ((Double) baseValue <= 0)
                return 0.1;
            //double value = 0;
            //RangeSlider rs = (RangeSlider)dependencyObject;
            //if ((Double)baseValue > rs.Maximum - rs.Minimum)
            //{
            //    value = rs.Maximum - rs.Minimum;
            //}
            //return value;
            return baseValue;
        }

        #endregion



        #region PropertyChanged CallBacks

        private static void TickFrequencyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            RangeSlider rs = (RangeSlider)dependencyObject;
            if (rs._topTick != null && rs._bottomTick != null)
            {
                if ((Double)e.NewValue > 0 && (Double)e.NewValue <= rs.Maximum - rs.Minimum)
                {
                    rs._topTick.TickFrequency = (Double)e.NewValue;
                    rs._bottomTick.TickFrequency = (Double)e.NewValue;
                }
            }
        }

        
        #endregion


        //enum for understanding which repeat button (left, right or both) is changing its width 
        enum ButtonType
        {
            Left,
            Right,
            Both
        }

        //enum for understanding current thumb moving direction 
        enum Direction
        {
            Increase,
            Decrease
        }
    }
}