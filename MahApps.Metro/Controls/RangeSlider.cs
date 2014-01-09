using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    public delegate void RangeSelectionChangedEventHandler(object sender, RangeSelectionChangedEventArgs e);

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
        public static readonly RoutedEvent RangeSelectionChangedEvent = EventManager.RegisterRoutedEvent("RangeSelectionChanged", RoutingStrategy.Bubble, typeof(RangeSelectionChangedEventHandler), typeof(RangeSlider));
        public static RoutedUICommand MoveBack = new RoutedUICommand("MoveBack", "MoveBack", typeof(RangeSlider), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.B, ModifierKeys.Control) }));
        public static RoutedUICommand MoveForward = new RoutedUICommand("MoveForward", "MoveForward", typeof(RangeSlider), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F, ModifierKeys.Control) }));
        public static RoutedUICommand MoveAllForward = new RoutedUICommand("MoveAllForward", "MoveAllForward", typeof(RangeSlider), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F, ModifierKeys.Alt) }));
        public static RoutedUICommand MoveAllBack = new RoutedUICommand("MoveAllBack", "MoveAllBack", typeof(RangeSlider), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.B, ModifierKeys.Alt) }));

        public static readonly DependencyProperty RangeStartSelectedProperty = DependencyProperty.Register("RangeStartSelected", typeof(double), typeof(RangeSlider), new UIPropertyMetadata((double)0, RangesChanged,CoearceRangeStart));

   

        public static readonly DependencyProperty RangeStopSelectedProperty = DependencyProperty.Register("RangeStopSelected", typeof(double), typeof(RangeSlider), new UIPropertyMetadata((double)1, RangesChanged,CoerceRangeStop));
   

        public static readonly DependencyProperty MinRangeProperty = DependencyProperty.Register("MinRange", typeof(double), typeof(RangeSlider), new UIPropertyMetadata((double)0, MinRangeChanged));


        /// <summary>
        /// Get/sets the beginning of the range selection.
        /// </summary>
        public double RangeStartSelected
        {
            get { return (double)GetValue(RangeStartSelectedProperty); }
            set { SetValue(RangeStartSelectedProperty, value); }
        }

        /// <summary>
        /// Get/sets the end of the range selection.
        /// </summary>
        public double RangeStopSelected
        {
            get { return (double)GetValue(RangeStopSelectedProperty); }
            set { SetValue(RangeStopSelectedProperty, value); }
        }

        /// <summary>
        /// Get/sets the minimum range that can be selected.
        /// </summary>
        public double MinRange
        {
            get { return (double)GetValue(MinRangeProperty); }
            set { SetValue(MinRangeProperty, value); }
        }


        public event RangeSelectionChangedEventHandler RangeSelectionChanged
        {
            add { AddHandler(RangeSelectionChangedEvent, value); }
            remove { RemoveHandler(RangeSelectionChangedEvent, value); }
        }

        private const double RepeatButtonMoveRatio = 0.1;
        private const double DefaultSplittersThumbWidth = 10;
        private bool _internalUpdate;
        private Thumb _centerThumb;
        private Thumb _leftThumb;
        private Thumb _rightThumb;
        private RepeatButton _leftButton;
        private RepeatButton _rightButton;
        private StackPanel _visualElementsContainer;
        //private double _movableRange;
        private double _movableWidth;

        public RangeSlider()
        {
            CommandBindings.Add(new CommandBinding(MoveBack, MoveBackHandler));
            CommandBindings.Add(new CommandBinding(MoveForward, MoveForwardHandler));
            CommandBindings.Add(new CommandBinding(MoveAllForward, MoveAllForwardHandler));
            CommandBindings.Add(new CommandBinding(MoveAllBack, MoveAllBackHandler));

            DependencyPropertyDescriptor.FromProperty(ActualWidthProperty, typeof(RangeSlider)).AddValueChanged(this, delegate { ReCalculateWidths(); });
        }

        static RangeSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(typeof(RangeSlider)));

            MinimumProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(MinimumProperty.DefaultMetadata.DefaultValue, null, CoerceMinimum));
            MaximumProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(MaximumProperty.DefaultMetadata.DefaultValue, null, CoerceMaximum));
        }

        /// <summary>
        /// Responds to a change in the value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum"/> property.
        /// </summary>
        /// <param name="oldMinimum">The old value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum"/> property.</param><param name="newMinimum">The new value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Minimum"/> property.</param>
        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            if (RangeStartSelected < newMinimum)
                RangeStartSelected = newMinimum;
        }

        /// <summary>
        /// Responds to a change in the value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum"/> property.
        /// </summary>
        /// <param name="oldMaximum">The old value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum"/> property.</param><param name="newMaximum">The new value of the <see cref="P:System.Windows.Controls.Primitives.RangeBase.Maximum"/> property.</param>
        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            if (RangeStopSelected > newMaximum)
                RangeStopSelected = newMaximum;
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

        private static void MinRangeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((double)e.NewValue < 0)
                throw new ArgumentOutOfRangeException("value", "value for MinRange cannot be less than 0");

            var slider = (RangeSlider)sender;
            if (slider._internalUpdate)
                return;

            slider._internalUpdate = true;
            slider.RangeStopSelected = Math.Max(slider.RangeStopSelected, slider.RangeStartSelected + (double)e.NewValue);
            slider.Maximum = Math.Max(slider.Maximum, slider.RangeStopSelected);
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

        private void RightThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            MoveThumb(_centerThumb, _rightButton, e.HorizontalChange);
            ReCalculateRangeSelected(false, true);
        }

        private void LeftThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            MoveThumb(_leftButton, _centerThumb, e.HorizontalChange);
            ReCalculateRangeSelected(true, false);
        }

        private void LeftButtonClick(object sender, RoutedEventArgs e)
        {
            MoveSelection(true);
        }

        private void RightButtonClick(object sender, RoutedEventArgs e)
        {
            MoveSelection(false);
        }

        private void CenterThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            MoveThumb(_leftButton, _rightButton, e.HorizontalChange);
            ReCalculateRangeSelected(true, true);
        }

        private static void MoveThumb(FrameworkElement x, FrameworkElement y, double horizonalChange)
        {
            double change = 0;
            if (horizonalChange < 0) //slider went left
                change = GetChangeKeepPositive(x.Width, horizonalChange);
            else if (horizonalChange > 0) //slider went right if(horizontal change == 0 do nothing)
                change = -GetChangeKeepPositive(y.Width, -horizonalChange);

            x.Width += change;
            y.Width -= change;
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
                _movableWidth = Math.Max(ActualWidth - _rightThumb.ActualWidth - _leftThumb.ActualWidth - _centerThumb.MinWidth, 1);
                _leftButton.Width = Math.Max(_movableWidth * (RangeStartSelected - Minimum) / MovableRange, 0);
                _rightButton.Width = Math.Max(_movableWidth * (Maximum - RangeStopSelected) / MovableRange, 0);
                _centerThumb.Width = Math.Max(ActualWidth - _leftButton.Width - _rightButton.Width - _rightThumb.ActualWidth - _leftThumb.ActualWidth, 0);
            }
        }

        private void ReCalculateRangeSelected(bool reCalculateStart, bool reCalculateStop)
        {
            _internalUpdate = true;//set flag to signal that the properties are being set by the object itself
            if (reCalculateStart)
            {
                // Make sure to get exactly rangestart if thumb is at the start
                RangeStartSelected = _leftButton.Width == 0.0 ? Minimum : Math.Max(Minimum, (Minimum + MovableRange * _leftButton.Width / _movableWidth));
            }

            if (reCalculateStop)
            {
                // Make sure to get exactly rangestop if thumb is at the end
                RangeStopSelected = _rightButton.Width == 0.0 ? Maximum : Math.Min(Maximum, (Maximum - MovableRange * _rightButton.Width / _movableWidth));
            }

            _internalUpdate = false;//set flag to signal that the properties are being set by the object itself

            if (reCalculateStart || reCalculateStop)
                //raise the RangeSelectionChanged event
                OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
        }

        public void MoveSelection(bool isLeft)
        {
            double widthChange = RepeatButtonMoveRatio * (RangeStopSelected - RangeStartSelected)
                * _movableWidth / MovableRange;

            widthChange = isLeft ? -widthChange : widthChange;
            MoveThumb(_leftButton, _rightButton, widthChange);
            ReCalculateRangeSelected(true, true);
        }

        public void ResetSelection(bool isStart)
        {
            double widthChange = Maximum - Minimum;
            widthChange = isStart ? -widthChange : widthChange;

            MoveThumb(_leftButton, _rightButton, widthChange);
            ReCalculateRangeSelected(true, true);
        }

        public void MoveSelection(double span)
        {
            if (span > 0)
            {
                if (RangeStopSelected + span > Maximum)
                    span = Maximum - RangeStopSelected;
            }
            else
            {
                if (RangeStartSelected + span < Minimum)
                    span = Minimum - RangeStartSelected;
            }

            if (span == 0)
                return;

            _internalUpdate = true;//set flag to signal that the properties are being set by the object itself
            RangeStartSelected += span;
            RangeStopSelected += span;
            ReCalculateWidths();
            _internalUpdate = false;//set flag to signal that the properties are being set by the object itself

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

            _internalUpdate = true;//set flag to signal that the properties are being set by the object itself
            RangeStartSelected = start;
            RangeStopSelected = stop;
            ReCalculateWidths();
            _internalUpdate = false;//set flag to signal that the properties are being set by the object itself
            OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
        }

        public void ZoomToSpan(double span)
        {
            _internalUpdate = true;//set flag to signal that the properties are being set by the object itself
            // Ensure new span is within the valid range
            span = Math.Min(span, Maximum - Minimum);
            span = Math.Max(span, MinRange);
            if (span == RangeStopSelected - RangeStartSelected)
                return; // No change

            // First zoom half of it to the right
            double rightChange = (span - (RangeStopSelected - RangeStartSelected)) / 2;
            double leftChange = rightChange;

            // If we will hit the right edge, spill over the leftover change to the other side
            if (rightChange > 0 && RangeStopSelected + rightChange > Maximum)
                leftChange += rightChange - (Maximum - RangeStopSelected);
            RangeStopSelected = Math.Min(RangeStopSelected + rightChange, Maximum);
            rightChange = 0;

            // If we will hit the left edge and there is space on the right, add the leftover change to the other side
            if (leftChange > 0 && RangeStartSelected - leftChange < Minimum)
                rightChange = Minimum - (RangeStartSelected - leftChange);
            RangeStartSelected = Math.Max(RangeStartSelected - leftChange, Minimum);
            if (rightChange > 0) // leftovers to the right
                RangeStopSelected = Math.Min(RangeStopSelected + rightChange, Maximum);

            ReCalculateWidths();
            _internalUpdate = false;//set flag to signal that the properties are being set by the object itself
            OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
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
            _visualElementsContainer.Orientation = Orientation.Horizontal;
            _leftThumb.Width = DefaultSplittersThumbWidth;
            _leftThumb.Tag = "left";
            _rightThumb.Width = DefaultSplittersThumbWidth;
            _rightThumb.Tag = "right";

            //handle the drag delta
            _centerThumb.DragDelta += CenterThumbDragDelta;
            _leftThumb.DragDelta += LeftThumbDragDelta;
            _rightThumb.DragDelta += RightThumbDragDelta;
            _leftButton.Click += LeftButtonClick;
            _rightButton.Click += RightButtonClick;

        }

        private static object CoearceRangeStart(DependencyObject d, object basevalue)
        {
            RangeSlider rs = (RangeSlider)d;

            double value = (double)basevalue;

            if (value < rs.Minimum)
                return rs.Minimum;
            return Math.Min(value, rs.RangeStopSelected);
        }

        private static object CoerceRangeStop(DependencyObject d, object basevalue)
        {
            RangeSlider rs = (RangeSlider)d;

            double value = (double)basevalue;

            if (value > rs.Maximum)
                return rs.Maximum;
            return Math.Max(value, rs.RangeStartSelected);
        }

        private static object CoerceMaximum(DependencyObject d, object basevalue)
        {
            RangeSlider rs = (RangeSlider)d;
            return (double)basevalue < rs.RangeStopSelected ? rs.RangeStopSelected : (double)basevalue;
        }

        private static object CoerceMinimum(DependencyObject d, object basevalue)
        {
            RangeSlider rs = (RangeSlider)d;

            return (double)basevalue > rs.RangeStartSelected ? rs.RangeStartSelected : (double)basevalue;
        }
    }
}