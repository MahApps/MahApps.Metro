using System;
using System.ComponentModel;
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
    TemplatePart(Name = "PART_RangeSliderContainer", Type = typeof(StackPanel)),
    TemplatePart(Name = "PART_LeftEdge", Type = typeof(RepeatButton)),
    TemplatePart(Name = "PART_RightEdge", Type = typeof(RepeatButton)),
    TemplatePart(Name = "PART_LeftThumb", Type = typeof(Thumb)),
    TemplatePart(Name = "PART_MiddleThumb", Type = typeof(Thumb)),
    TemplatePart(Name = "PART_RightThumb", Type = typeof(Thumb))]
    public sealed class RangeSlider : Slider
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
                new UIPropertyMetadata((Double) 0, RangesChanged, CoerceLowerValue));

        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register("UpperValue", typeof (Double), typeof (RangeSlider),
                new UIPropertyMetadata((Double) 1, RangesChanged, CoerceUpperValue));

        public static readonly DependencyProperty MinRangeProperty =
            DependencyProperty.Register("MinRange", typeof (Double), typeof (RangeSlider),
                new UIPropertyMetadata((Double) 0, MinRangeChanged));

        public static readonly DependencyProperty MinBridgeWidthProperty =
            DependencyProperty.Register("MinBridgeWidth", typeof(Double), typeof(RangeSlider),
                new UIPropertyMetadata((Double)15, MinBridgeWidthChanged, CoerceMinBridgeWidth));

        public static readonly DependencyProperty MoveWholeSelectionProperty =
            DependencyProperty.Register("MoveWholeSelection", typeof(Boolean), typeof(RangeSlider),
                new UIPropertyMetadata((Boolean)false));

        public static readonly DependencyProperty ExtendedModeProperty =
            DependencyProperty.Register("ExtendedMode", typeof(Boolean), typeof(RangeSlider),
                new UIPropertyMetadata((Boolean)false));

        public Boolean ExtendedMode
        {
            get { return (Boolean)GetValue(ExtendedModeProperty); }
            set { SetValue(ExtendedModeProperty, value); }
        }

        //Property means Will whole selection be moved when pressing on left or right repeat button
        public Boolean MoveWholeSelection
        {
            get { return (Boolean)GetValue(MoveWholeSelectionProperty); }
            set { SetValue(MoveWholeSelectionProperty, value); }
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

        private const Double RepeatButtonMoveRatio = 0.1;
        private const Double DefaultSplittersThumbWidth = 10;
        private bool _internalUpdate;
        private Thumb _centerThumb;
        private Thumb _leftThumb;
        private Thumb _rightThumb;
        private RepeatButton _leftButton;
        private RepeatButton _rightButton;
        private StackPanel _visualElementsContainer;
        //private double _movableRange;
        private Double _movableWidth;
        private DispatcherTimer timer = new DispatcherTimer();
        private uint tickCount = 0;

        #endregion


        public RangeSlider()
        {
            CommandBindings.Add(new CommandBinding(MoveBack, MoveBackHandler));
            CommandBindings.Add(new CommandBinding(MoveForward, MoveForwardHandler));
            CommandBindings.Add(new CommandBinding(MoveAllForward, MoveAllForwardHandler));
            CommandBindings.Add(new CommandBinding(MoveAllBack, MoveAllBackHandler));

            DependencyPropertyDescriptor.FromProperty(ActualWidthProperty, typeof(RangeSlider)).AddValueChanged(this, delegate { ReCalculateWidths(); });

            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0,0,0,0,Interval);
        }

        

        static RangeSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(typeof(RangeSlider)));
            MaximumProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(MaximumProperty.DefaultMetadata.DefaultValue, null, CoerceMaximum));
            MinimumProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(MinimumProperty.DefaultMetadata.DefaultValue, null, CoerceMinimum));
            
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

        private static void RangeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var slider = (RangeSlider)dependencyObject;
            if (slider._internalUpdate)
                return;

            slider.ReCalculateRanges();
            slider.ReCalculateWidths();
        }

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
            if ((double)e.NewValue < 0)
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

        

        private void LeftButtonClick(object sender, RoutedEventArgs e)
        {
            
            
        }

        private void RightButtonClick(object sender, RoutedEventArgs e)
        {
            if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                Point p = Mouse.GetPosition(_rightButton);
                if (IsMoveToPointEnabled && !MoveWholeSelection)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        MoveThumb(_centerThumb, _rightButton, (p.X + (_rightThumb.ActualWidth/2)), Orientation);
                    }
                    else
                    {
                        MoveThumb(_centerThumb, _rightButton, (p.Y + (_rightThumb.ActualHeight/2)), Orientation);
                    }
                    ReCalculateRangeSelected(false, true);
                }
                else if (IsMoveToPointEnabled && MoveWholeSelection)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        MoveThumb(_leftButton, _rightButton, (p.X + (_rightThumb.ActualWidth/2)),
                            Orientation);
                    }
                    else
                    {
                        MoveThumb(_leftButton, _rightButton, (p.Y + (_rightThumb.ActualHeight/2)),
                            Orientation);
                    }
                    ReCalculateRangeSelected(true, true);
                }
                else if (!IsMoveToPointEnabled && !MoveWholeSelection)
                {

                }
                else if (!IsMoveToPointEnabled && MoveWholeSelection)
                {

                }
                //else
                //{
                //    MoveSelection(false);
                //}
            }
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
            }
        }

        private void ReCalculateRangeSelected(bool reCalculateStart, bool reCalculateStop)
        {
            Double oldStart = 0, oldStop = 0;

            _internalUpdate = true;//set flag to signal that the properties are being set by the object itself
            if (Orientation == Orientation.Horizontal)
            {
                if (reCalculateStart)
                {
                    oldStart = LowerValue;

                    // Make sure to get exactly rangestart if thumb is at the start
                    LowerValue = _leftButton.Width == 0.0
                        ? Minimum
                        : Math.Max(Minimum, (Minimum + MovableRange*_leftButton.Width/_movableWidth));
                }

                if (reCalculateStop)
                {
                    oldStop = UpperValue;

                    // Make sure to get exactly rangestop if thumb is at the end
                    UpperValue = _rightButton.Width == 0.0
                        ? Maximum
                        : Math.Min(Maximum, (Maximum - MovableRange*_rightButton.Width/_movableWidth));

                }

                _internalUpdate = false; //set flag to signal that the properties are being set by the object itself

                if (reCalculateStart || reCalculateStop)
                {
                    //raise the RangeSelectionChanged event
                    OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
                }
            }
            else if (Orientation == Orientation.Vertical)
            {
                if (reCalculateStart)
                {
                    oldStart = LowerValue;

                    // Make sure to get exactly rangestart if thumb is at the start
                    LowerValue = _leftButton.Height == 0.0
                        ? Minimum
                        : Math.Max(Minimum, (Minimum + MovableRange * _leftButton.Height / _movableWidth));
                }

                if (reCalculateStop)
                {
                    oldStop = UpperValue;

                    // Make sure to get exactly rangestop if thumb is at the end
                    UpperValue = _rightButton.Height == 0.0
                        ? Maximum
                        : Math.Min(Maximum, (Maximum - MovableRange * _rightButton.Height / _movableWidth));

                }

                _internalUpdate = false; //set flag to signal that the properties are being set by the object itself

                if (reCalculateStart || reCalculateStop)
                {
                    //raise the RangeSelectionChanged event
                    OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
                }
            }

            if (reCalculateStart && oldStart != LowerValue)
                OnRangeParameterChanged(new RangeParameterChangedEventArgs(RangeParameterChangeType.Lower, oldStart, LowerValue), LowerValueChangedEvent);
            else if (reCalculateStop && oldStop != UpperValue)
                OnRangeParameterChanged(new RangeParameterChangedEventArgs(RangeParameterChangeType.Upper, oldStop, UpperValue), UpperValueChangedEvent);
        }



        private void OnRangeParameterChanged(RangeParameterChangedEventArgs e, RoutedEvent Event)
        {
            e.RoutedEvent = Event;
            RaiseEvent(e);
        }


        public void MoveSelection(bool isLeft)
        {
            //_leftButton.
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
            _visualElementsContainer = EnforceInstance<StackPanel>("PART_RangeSliderContainer");
            _visualElementsContainer.HorizontalAlignment = HorizontalAlignment.Stretch;
            _centerThumb = EnforceInstance<Thumb>("PART_MiddleThumb");
            _leftButton = EnforceInstance<RepeatButton>("PART_LeftEdge");
            _rightButton = EnforceInstance<RepeatButton>("PART_RightEdge");
            _leftThumb = EnforceInstance<Thumb>("PART_LeftThumb");
            _rightThumb = EnforceInstance<Thumb>("PART_RightThumb");
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
            _visualElementsContainer.Orientation = Orientation;
            if (Orientation == Orientation.Horizontal)
            {
                _leftThumb.Width = DefaultSplittersThumbWidth;
                _rightThumb.Width = DefaultSplittersThumbWidth;
                _centerThumb.MinWidth = 10;
            }
            else if (Orientation == Orientation.Vertical)
            {
                _leftThumb.Height = DefaultSplittersThumbWidth;
                _rightThumb.Height = DefaultSplittersThumbWidth;
                _centerThumb.MinHeight = 10;
            }

            _leftThumb.DragCompleted += LeftThumbDragComplete;
            _rightThumb.DragCompleted += RightThumbDragComplete;
            _leftThumb.DragStarted += LeftThumbDragStart;
            _rightThumb.DragStarted += RightThumbDragStart;
            _centerThumb.DragStarted += CenterThumb_DragStarted;
            _centerThumb.DragCompleted += CenterThumb_DragCompleted;

            
            //handle the drag delta events
            _centerThumb.DragDelta += CenterThumbDragDelta;
            _leftThumb.DragDelta += LeftThumbDragDelta;
            _rightThumb.DragDelta += RightThumbDragDelta;


            _leftButton.PreviewMouseLeftButtonDown += _leftButton_PreviewMouseLeftButtonDown;
            _rightButton.PreviewMouseLeftButtonDown += _rightButton_PreviewMouseLeftButtonDown;

            _visualElementsContainer.PreviewMouseUp += _visualElementsContainer_PreviewMouseUp;
            _visualElementsContainer.MouseLeave += _visualElementsContainer_MouseLeave;

            _centerThumb.PreviewMouseDown += _centerThumb_PreviewMouseDown;

            _visualElementsContainer.MouseWheel += _visualElementsContainer_MouseWheel;
        }

        void _visualElementsContainer_MouseLeave(object sender, MouseEventArgs e)
        {
            timer.Stop();
        }

        void _visualElementsContainer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            timer.Stop();
        }

        void _rightButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Point p = Mouse.GetPosition(_rightButton);
                if (IsMoveToPointEnabled && !MoveWholeSelection)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        MoveThumb(_centerThumb, _rightButton, (p.X + (_rightThumb.ActualWidth / 2)), Orientation);
                    }
                    else
                    {
                        MoveThumb(_centerThumb, _rightButton, (p.Y + (_rightThumb.ActualHeight / 2)), Orientation);
                    }
                    ReCalculateRangeSelected(false, true);
                }
                else if (IsMoveToPointEnabled && MoveWholeSelection)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        MoveThumb(_leftButton, _rightButton, (p.X + (_rightThumb.ActualWidth / 2)),
                            Orientation);
                    }
                    else
                    {
                        MoveThumb(_leftButton, _rightButton, (p.Y + (_rightThumb.ActualHeight / 2)),
                            Orientation);
                    }
                    ReCalculateRangeSelected(true, true);
                }
                else if (!IsMoveToPointEnabled)
                {
                    position = Mouse.GetPosition(_visualElementsContainer);
                    bType = ButtonType.Right;
                    direction = Direction.Increase;
                    timer.Start();
                }
            }
        }

        void _leftButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (IsMoveToPointEnabled && !MoveWholeSelection)
                {
                    Point p = Mouse.GetPosition(_leftButton);
                    if (Orientation == Orientation.Horizontal)
                    {
                        MoveThumb(_leftButton, _centerThumb,
                            -(_leftButton.ActualWidth - p.X + (_leftThumb.ActualWidth / 2)),
                            Orientation);
                    }
                    else
                    {
                        MoveThumb(_leftButton, _centerThumb,
                            -(_leftButton.ActualHeight - p.Y + (_leftThumb.ActualHeight / 2)),
                            Orientation);
                    }
                    ReCalculateRangeSelected(true, false);
                }
                else if (IsMoveToPointEnabled && MoveWholeSelection)
                {
                    Point p = Mouse.GetPosition(_leftButton);
                    if (Orientation == Orientation.Horizontal)
                    {
                        MoveThumb(_leftButton, _rightButton,
                            -(_leftButton.ActualWidth - p.X + (_leftThumb.ActualWidth / 2)),
                            Orientation);
                    }
                    else
                    {
                        MoveThumb(_leftButton, _rightButton,
                            -(_leftButton.ActualHeight - p.Y + (_leftThumb.ActualHeight / 2)),
                            Orientation);
                    }
                    ReCalculateRangeSelected(true, true);
                }
                else if (!IsMoveToPointEnabled)
                {
                    position = Mouse.GetPosition(_visualElementsContainer);
                    bType = ButtonType.Left;
                    direction = Direction.Decrease;
                    timer.Start();
                }
            }
        }

        void _visualElementsContainer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double change = 0;
            if (e.Delta > 0)
            {
                change = LargeChange;
            }
            else
            {
                change = -LargeChange;
            }
            MoveThumb(_leftButton, _rightButton, change, Orientation);
            ReCalculateRangeSelected(true, true);
        }

        void _centerThumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ExtendedMode)
            {
                if (e.LeftButton == MouseButtonState.Pressed && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    Point p = Mouse.GetPosition(_centerThumb);
                    if (IsMoveToPointEnabled && !MoveWholeSelection)
                    {
                        if (Orientation == Orientation.Horizontal)
                        {
                            MoveThumb(_leftButton, _centerThumb,
                                (p.X + (_leftThumb.ActualWidth / 2)),
                                Orientation);
                        }
                        else
                        {
                            MoveThumb(_leftButton, _centerThumb,
                                (p.Y + (_leftThumb.ActualHeight / 2)),
                                Orientation);
                        }
                        ReCalculateRangeSelected(true, false);
                    }
                    else if (IsMoveToPointEnabled && MoveWholeSelection)
                    {
                        if (Orientation == Orientation.Horizontal)
                        {
                            MoveThumb(_leftButton, _rightButton,
                                (p.X + (_leftThumb.ActualWidth / 2)),
                                Orientation);
                        }
                        else
                        {
                            MoveThumb(_leftButton, _rightButton,
                                (p.Y + (_leftThumb.ActualHeight / 2)),
                                Orientation);
                        }
                        ReCalculateRangeSelected(true, true);
                    }
                }
                else if (e.RightButton == MouseButtonState.Pressed && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    Point p = Mouse.GetPosition(_centerThumb);
                    if (IsMoveToPointEnabled && !MoveWholeSelection)
                    {
                        if (Orientation == Orientation.Horizontal)
                        {
                            MoveThumb(_centerThumb, _rightButton,
                                -(_centerThumb.ActualWidth - p.X + (_rightThumb.ActualWidth/2)),
                                Orientation);
                        }
                        else
                        {
                            MoveThumb(_centerThumb, _rightButton,
                                -(_centerThumb.ActualHeight - p.Y + (_rightThumb.ActualHeight/2)),
                                Orientation);
                        }
                        ReCalculateRangeSelected(false, true);
                    }
                    else if (IsMoveToPointEnabled && MoveWholeSelection)
                    {
                        if (Orientation == Orientation.Horizontal)
                        {
                            MoveThumb(_leftButton, _rightButton,
                                -(_centerThumb.ActualWidth - p.X + (_rightThumb.ActualWidth / 2)),
                                Orientation);
                        }
                        else
                        {
                            MoveThumb(_leftButton, _rightButton,
                                -(_centerThumb.ActualHeight - p.Y + (_rightThumb.ActualHeight / 2)),
                                Orientation);
                        }
                        ReCalculateRangeSelected(true, true);
                    }
                }
            }
        }

        
       
        void timer_Tick(object sender, EventArgs e)
        {
            if (!MoveWholeSelection)
            {
                if (bType == ButtonType.Right && direction == Direction.Increase)
                {
                    double widthChange = SmallChange;
                    if (tickCount > 10)
                    {
                        widthChange = LargeChange;
                    }
                    if (Orientation == Orientation.Horizontal)
                    {
                        
                        if (position.X > ActualWidth - _rightButton.ActualWidth)
                        {
                            MoveThumb(_centerThumb, _rightButton, widthChange, Orientation);
                        }
                        else
                        {
                            timer.Stop();
                            tickCount = 0;
                        }
                    }
                    else
                    {
                        if (position.Y > ActualHeight - _rightButton.ActualHeight)
                        {
                            MoveThumb(_centerThumb, _rightButton, widthChange, Orientation);
                        }
                        else
                        {
                            timer.Stop();
                            tickCount = 0;
                        }
                    }
                    ReCalculateRangeSelected(false, true);
                }
                else if (bType == ButtonType.Left && direction == Direction.Decrease)
                {
                    double widthChange = SmallChange;
                    if (tickCount > 10)
                    {
                        widthChange = LargeChange;
                    }
                    if (Orientation == Orientation.Horizontal)
                    {

                        if (position.X < _leftButton.ActualWidth)
                        {
                            MoveThumb(_leftButton, _centerThumb, -widthChange, Orientation);
                        }
                        else
                        {
                            timer.Stop();
                            tickCount = 0;
                        }
                    }
                    else
                    {
                        if (position.Y < _leftButton.ActualWidth)
                        {
                            MoveThumb(_leftButton, _centerThumb, -widthChange, Orientation);
                        }
                        else
                        {
                            timer.Stop();
                            tickCount = 0;
                        }
                    }
                    ReCalculateRangeSelected(false, true);
                }
                tickCount++;
            }
            else
            {
                if (bType == ButtonType.Right && direction == Direction.Increase)
                {
                    double widthChange = SmallChange;
                    if (tickCount > 10)
                    {
                        widthChange = LargeChange;
                    }
                    if (Orientation == Orientation.Horizontal)
                    {

                        if (position.X > ActualWidth - _rightButton.ActualWidth)
                        {
                            MoveThumb(_leftButton, _rightButton, widthChange, Orientation);
                        }
                        else
                        {
                            timer.Stop();
                            tickCount = 0;
                        }
                    }
                    else
                    {
                        if (position.Y > ActualHeight - _rightButton.ActualHeight)
                        {
                            MoveThumb(_leftButton, _rightButton, widthChange, Orientation);
                        }
                        else
                        {
                            timer.Stop();
                            tickCount = 0;
                        }
                    }
                    ReCalculateRangeSelected(true, true);
                }
                else if (bType == ButtonType.Left && direction == Direction.Decrease)
                {
                    double widthChange = SmallChange;
                    if (tickCount > 10)
                    {
                        widthChange = LargeChange;
                    }
                    if (Orientation == Orientation.Horizontal)
                    {

                        if (position.X < _leftButton.ActualWidth)
                        {
                            MoveThumb(_leftButton, _rightButton, -widthChange, Orientation);
                        }
                        else
                        {
                            timer.Stop();
                            tickCount = 0;
                        }
                    }
                    else
                    {
                        if (position.Y < _leftButton.ActualWidth)
                        {
                            MoveThumb(_leftButton, _rightButton, -widthChange, Orientation);
                        }
                        else
                        {
                            timer.Stop();
                            tickCount = 0;
                        }
                    }
                    ReCalculateRangeSelected(true, true);
                }
                tickCount++;
            }
            
        }

        enum ButtonType
        {
            Left,
            Right
        }

        enum Direction
        {
            Increase,
            Decrease
        }

        private Direction direction;
        private ButtonType bType;
        private Point position;


        

        void CenterThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            e.RoutedEvent = CentralThumbDragCompletedEvent;
            RaiseEvent(e);
        }

        void CenterThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            e.RoutedEvent = CentralThumbDragStartedEvent;
            RaiseEvent(e);
        }

        private void CenterThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            double change = Orientation == Orientation.Horizontal
                ? e.HorizontalChange
                : e.VerticalChange;
            MoveThumb(_leftButton, _rightButton, change, Orientation);
            ReCalculateRangeSelected(true, true);
            e.RoutedEvent = CentralThumbDragDeltaEvent;
            RaiseEvent(e);
        }

        private void RightThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            double change = Orientation == Orientation.Horizontal ? e.HorizontalChange :
            e.VerticalChange;
            MoveThumb(_centerThumb, _rightButton, change, Orientation);
            ReCalculateRangeSelected(false, true);
            e.RoutedEvent = UpperThumbDragDeltaEvent;
            RaiseEvent(e);
        }

        private void LeftThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            double change = Orientation == Orientation.Horizontal
                ? e.HorizontalChange
                : e.VerticalChange;
            MoveThumb(_leftButton, _centerThumb, change, Orientation);
            ReCalculateRangeSelected(true, false);
            e.RoutedEvent = LowerThumbDragDeltaEvent;
            RaiseEvent(e);
        }

        private void RightThumbDragStart(object sender, DragStartedEventArgs e)
        {
            e.RoutedEvent = UpperThumbDragStartedEvent;
            RaiseEvent(e);
        }

        private void LeftThumbDragComplete(object sender, DragCompletedEventArgs e)
        {
            e.RoutedEvent = LowerThumbDragCompletedEvent;
            RaiseEvent(e);
        }

        private void LeftThumbDragStart(object sender, DragStartedEventArgs e)
        {
            e.RoutedEvent = LowerThumbDragStartedEvent;
            RaiseEvent(e);
        }

        private void RightThumbDragComplete(object sender, DragCompletedEventArgs e)
        {
            e.RoutedEvent = UpperThumbDragCompletedEvent;
            RaiseEvent(e);
        }

        
        private static object CoerceLowerValue(DependencyObject d, object basevalue)
        {
            RangeSlider rs = (RangeSlider)d;

            //double value = (double)basevalue;

            //if (value <= rs.Minimum)
            //    return rs.Minimum;
            //return Math.Min(value, rs.UpperValue);
            return (Double) basevalue;
        }

        private static object CoerceUpperValue(DependencyObject d, object basevalue)
        {
            RangeSlider rs = (RangeSlider)d;

            //double value = (double)basevalue;

            //if (value >= rs.Maximum)
            //    return rs.Maximum;
            //return Math.Max(value, rs.LowerValue);
            return (Double) basevalue;
        }

        private static object CoerceMaximum(DependencyObject d, object basevalue)
        {
            RangeSlider rs = (RangeSlider)d;
            return (Double)basevalue;
            //return (Double)basevalue < rs.Minimum ? rs.Minimum : (Double)basevalue;
        }

        private static object CoerceMinimum(DependencyObject d, object basevalue)
        {
            RangeSlider rs = (RangeSlider)d;
            return (Double) basevalue;
            //return (Double)basevalue > rs.Maximum ? rs.Maximum : (Double)basevalue;
        }

        private static object CoerceMinBridgeWidth(DependencyObject d, object basevalue)
        {
            RangeSlider rs = (RangeSlider)d;
            double width = 0;
            if (rs.Orientation == Orientation.Horizontal)
            {
                width = rs.ActualWidth - rs._leftThumb.ActualWidth - rs._rightThumb.ActualWidth;
            }
            else
            {
                width = rs.ActualHeight - rs._leftThumb.ActualHeight - rs._rightThumb.ActualHeight;
            }
            return (Double)basevalue > width ? width : (Double)basevalue;
        }




        #region Unused methods. Candidates for deletion

        public void MoveSelection(double span)
        {
            if (span > 0)
            {
                if (UpperValue + span > Maximum)
                    span = Maximum - UpperValue;
            }
            else
            {
                if (LowerValue + span < Minimum)
                    span = Minimum - LowerValue;
            }

            if (span == 0)
                return;

            _internalUpdate = true; //set flag to signal that the properties are being set by the object itself
            LowerValue += span;
            UpperValue += span;
            ReCalculateWidths();
            _internalUpdate = false; //set flag to signal that the properties are being set by the object itself

            OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
        }

        public void SetSelectedRange(double selectionStart, double selectionStop)
        {
            double start = Math.Max(Minimum, selectionStart);
            double stop = Math.Min(selectionStop, Maximum);
            start = Math.Min(start, Maximum - MinRange);
            stop = Math.Max(Minimum + MinRange, stop);
            if (stop < start + MinRange)
                return;

            _internalUpdate = true; //set flag to signal that the properties are being set by the object itself
            LowerValue = start;
            UpperValue = stop;
            ReCalculateWidths();
            _internalUpdate = false; //set flag to signal that the properties are being set by the object itself
            OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
        }

        public void ZoomToSpan(double span)
        {
            _internalUpdate = true; //set flag to signal that the properties are being set by the object itself
            // Ensure new span is within the valid range
            span = Math.Min(span, Maximum - Minimum);
            span = Math.Max(span, MinRange);
            if (span == UpperValue - LowerValue)
                return; // No change

            // First zoom half of it to the right
            double rightChange = (span - (UpperValue - LowerValue))/2;
            double leftChange = rightChange;

            // If we will hit the right edge, spill over the leftover change to the other side
            if (rightChange > 0 && UpperValue + rightChange > Maximum)
                leftChange += rightChange - (Maximum - UpperValue);
            UpperValue = Math.Min(UpperValue + rightChange, Maximum);
            rightChange = 0;

            // If we will hit the left edge and there is space on the right, add the leftover change to the other side
            if (leftChange > 0 && LowerValue - leftChange < Minimum)
                rightChange = Minimum - (LowerValue - leftChange);
            LowerValue = Math.Max(LowerValue - leftChange, Minimum);
            if (rightChange > 0) // leftovers to the right
                UpperValue = Math.Min(UpperValue + rightChange, Maximum);

            ReCalculateWidths();
            _internalUpdate = false; //set flag to signal that the properties are being set by the object itself
            OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
        }

        #endregion
    }
}