using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Data;
using ControlzEx;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A Button that allows the user to toggle between two or three states.
    /// </summary>
    [TemplatePart(Name = PART_BackgroundTranslate, Type = typeof(TranslateTransform))]
    [TemplatePart(Name = PART_DraggingThumb, Type = typeof(Thumb))]
    [TemplatePart(Name = PART_SwitchTrack, Type = typeof(Grid))]
    [TemplatePart(Name = PART_ThumbIndicator, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_ThumbTranslate, Type = typeof(TranslateTransform))]
    public class ToggleSwitchButton : ToggleButton
    {
        private const string PART_BackgroundTranslate = "PART_BackgroundTranslate";
        private const string PART_DraggingThumb = "PART_DraggingThumb";
        private const string PART_SwitchTrack = "PART_SwitchTrack";
        private const string PART_ThumbIndicator = "PART_ThumbIndicator";
        private const string PART_ThumbTranslate = "PART_ThumbTranslate";

        private TranslateTransform _BackgroundTranslate;
        private Thumb _DraggingThumb;
        private Grid _SwitchTrack;
        private FrameworkElement _ThumbIndicator;
        private TranslateTransform _ThumbTranslate;
        private readonly PropertyChangeNotifier isCheckedPropertyChangeNotifier;

        public static readonly DependencyProperty OnSwitchBrushProperty = DependencyProperty.Register("OnSwitchBrush", typeof(Brush), typeof(ToggleSwitchButton), null);
        public static readonly DependencyProperty OffSwitchBrushProperty = DependencyProperty.Register("OffSwitchBrush", typeof(Brush), typeof(ToggleSwitchButton), null);
        public static readonly DependencyProperty IntermediateSwitchBrushProperty = DependencyProperty.Register("IntermediateSwitchBrush", typeof(Brush), typeof(ToggleSwitchButton), null);

        public static readonly DependencyProperty OnBorderBrushProperty = DependencyProperty.Register("OnBorderBrush", typeof(Brush), typeof(ToggleSwitchButton), null);
        public static readonly DependencyProperty OffBorderBrushProperty = DependencyProperty.Register("OffBorderBrush", typeof(Brush), typeof(ToggleSwitchButton), null);
        public static readonly DependencyProperty IntermediateBorderBrushProperty = DependencyProperty.Register("IntermediateBorderBrush", typeof(Brush), typeof(ToggleSwitchButton), null);

        public static readonly DependencyProperty ThumbIndicatorOnBrushProperty = DependencyProperty.Register("ThumbIndicatorOnBrush", typeof(Brush), typeof(ToggleSwitchButton), null);
        public static readonly DependencyProperty ThumbIndicatorOffBrushProperty = DependencyProperty.Register("ThumbIndicatorOffBrush", typeof(Brush), typeof(ToggleSwitchButton), null);
        public static readonly DependencyProperty ThumbIndicatorIntermediateBrushProperty = DependencyProperty.Register("ThumbIndicatorIntermediateBrush", typeof(Brush), typeof(ToggleSwitchButton), null);

        public static readonly DependencyProperty ThumbIndicatorDisabledBrushProperty = DependencyProperty.Register("ThumbIndicatorDisabledBrush", typeof(Brush), typeof(ToggleSwitchButton), null);
        public static readonly DependencyProperty ThumbIndicatorWidthProperty = DependencyProperty.Register("ThumbIndicatorWidth", typeof(double), typeof(ToggleSwitchButton), new PropertyMetadata(13d));

        public static readonly DependencyProperty SwitchWidthProperty = DependencyProperty.Register("SwitchWidth", typeof(double), typeof(ToggleSwitchButton), new PropertyMetadata(44d));
        public static readonly DependencyProperty SwitchHeightProperty = DependencyProperty.Register("SwitchHeight", typeof(double), typeof(ToggleSwitchButton), new PropertyMetadata(20d));

        public static readonly DependencyProperty ContentPaddingProperty = DependencyProperty.Register("ContentPadding", typeof(Thickness), typeof(ToggleSwitchButton), new FrameworkPropertyMetadata(new Thickness(4,0,0,0)));

        /// <summary>
        /// Gets/sets the brush used for the on-switch's foreground.
        /// </summary>
        public Brush OnSwitchBrush
        {
            get { return (Brush)GetValue(OnSwitchBrushProperty); }
            set { SetValue(OnSwitchBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the brush used for the off-switch's foreground.
        /// </summary>
        public Brush OffSwitchBrush
        {
            get { return (Brush)GetValue(OffSwitchBrushProperty); }
            set { SetValue(OffSwitchBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the brush used for the intermediate-switch's foreground.
        /// </summary>
        public Brush IntermediateSwitchBrush
        {
            get { return (Brush)GetValue(IntermediateSwitchBrushProperty); }
            set { SetValue(IntermediateSwitchBrushProperty, value); }
        }


        /// <summary>
        /// Gets/sets the brush used for the on-switch's foreground.
        /// </summary>
        public Brush OnBorderBrush
        {
            get { return (Brush)GetValue(OnBorderBrushProperty); }
            set { SetValue(OnBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the brush used for the off-switch's foreground.
        /// </summary>
        public Brush OffBorderBrush
        {
            get { return (Brush)GetValue(OffBorderBrushProperty); }
            set { SetValue(OffBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the brush used for the intermediate-switch's foreground.
        /// </summary>
        public Brush IntermediateBorderBrush
        {
            get { return (Brush)GetValue(IntermediateBorderBrushProperty); }
            set { SetValue(IntermediateBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the brush used for the thumb indicator (on).
        /// </summary>
        public Brush ThumbIndicatorOnBrush
        {
            get { return (Brush)GetValue(ThumbIndicatorOnBrushProperty); }
            set { SetValue(ThumbIndicatorOnBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the brush used for the thumb indicator (off).
        /// </summary>
        public Brush ThumbIndicatorOffBrush
        {
            get { return (Brush)GetValue(ThumbIndicatorOffBrushProperty); }
            set { SetValue(ThumbIndicatorOffBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the brush used for the thumb indicator (intermeidate).
        /// </summary>
        public Brush ThumbIndicatorIntermediateBrush
        {
            get { return (Brush)GetValue(ThumbIndicatorIntermediateBrushProperty); }
            set { SetValue(ThumbIndicatorIntermediateBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the brush used for the thumb indicator.
        /// </summary>
        public Brush ThumbIndicatorDisabledBrush
        {
            get { return (Brush)GetValue(ThumbIndicatorDisabledBrushProperty); }
            set { SetValue(ThumbIndicatorDisabledBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the width of the thumb indicator.
        /// </summary>
        public double ThumbIndicatorWidth
        {
            get { return (double)GetValue(ThumbIndicatorWidthProperty); }
            set { SetValue(ThumbIndicatorWidthProperty, value); }
        }

        /// <summary>
        /// Gets/sets the width of the swtich
        /// </summary>
        public double SwitchWidth
        {
            get { return (double)GetValue(SwitchWidthProperty); }
            set { SetValue(SwitchWidthProperty, value); }
        }

        /// <summary>
        /// Gets/sets the width of the swtich
        /// </summary>
        public double SwitchHeight
        {
            get { return (double)GetValue(SwitchHeightProperty); }
            set { SetValue(SwitchHeightProperty, value); }
        }

        /// <summary>
        /// Gets/sets the content padding
        /// </summary>
        public Thickness ContentPadding
        {
            get { return (Thickness)GetValue(ContentPaddingProperty); }
            set { SetValue(ContentPaddingProperty, value); }
        }
        static ToggleSwitchButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleSwitchButton), new FrameworkPropertyMetadata(typeof(ToggleSwitchButton)));
        }

        public ToggleSwitchButton()
        {
            isCheckedPropertyChangeNotifier = new PropertyChangeNotifier(this, ToggleSwitchButton.IsCheckedProperty);
            isCheckedPropertyChangeNotifier.ValueChanged += IsCheckedPropertyChangeNotifierValueChanged;
        }

        private void IsCheckedPropertyChangeNotifierValueChanged(object sender, EventArgs e)
        {
            UpdateThumb();
        }

        DoubleAnimation _thumbAnimation;
        private void UpdateThumb()
        {
            if (_ThumbTranslate != null && _SwitchTrack != null && _ThumbIndicator != null)
            {
                double destination = GetThumbDestination(); 

                _thumbAnimation = new DoubleAnimation();
                _thumbAnimation.To = destination;
                _thumbAnimation.Duration = TimeSpan.FromMilliseconds(500);
                _thumbAnimation.EasingFunction = new ExponentialEase() { Exponent = 9 };
                _thumbAnimation.FillBehavior = FillBehavior.Stop;

                AnimationTimeline currentAnimation = _thumbAnimation;
                _thumbAnimation.Completed += (sender, e) => {
                    if (_thumbAnimation != null && currentAnimation == _thumbAnimation)
                    {
                        _ThumbTranslate.X = destination;
                        _thumbAnimation = null;
                    }
                };
                _ThumbTranslate.BeginAnimation(TranslateTransform.XProperty, _thumbAnimation);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _BackgroundTranslate = GetTemplateChild(PART_BackgroundTranslate) as TranslateTransform;
            _DraggingThumb = GetTemplateChild(PART_DraggingThumb) as Thumb;
            _SwitchTrack = GetTemplateChild(PART_SwitchTrack) as Grid;
            _ThumbIndicator = GetTemplateChild(PART_ThumbIndicator) as FrameworkElement;
            _ThumbTranslate = GetTemplateChild(PART_ThumbTranslate) as TranslateTransform;

            if (_ThumbIndicator != null && _ThumbTranslate != null && _BackgroundTranslate != null)
            {
                Binding translationBinding;
                translationBinding = new System.Windows.Data.Binding("X");
                translationBinding.Source = _ThumbTranslate;
                BindingOperations.SetBinding(_BackgroundTranslate, TranslateTransform.XProperty, translationBinding);
            }

            if (_DraggingThumb != null && _ThumbIndicator != null && _ThumbTranslate != null)
            {
                _DraggingThumb.DragStarted -= _DraggingThumb_DragStarted;
                _DraggingThumb.DragDelta -= _DraggingThumb_DragDelta;
                _DraggingThumb.DragCompleted -= _DraggingThumb_DragCompleted;
                _DraggingThumb.DragStarted += _DraggingThumb_DragStarted;
                _DraggingThumb.DragDelta += _DraggingThumb_DragDelta;
                _DraggingThumb.DragCompleted += _DraggingThumb_DragCompleted;
                if (_SwitchTrack != null)
                {
                    _SwitchTrack.SizeChanged -= _SwitchTrack_SizeChanged;
                    _SwitchTrack.SizeChanged += _SwitchTrack_SizeChanged;
                }
            }
        }

        private void SetIsPressed(bool pressed)
        {
            // we can't use readonly IsPressedProperty
            typeof(ToggleButton).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic)
                                .Invoke(this, new object[] { pressed });
        }

        // This function gets the new position for the following parameter: _ThumbTranslate.X 
        private double GetThumbDestination()
        {
            switch (IsChecked)
            {
                case true:
                    return _SwitchTrack.ActualWidth - _ThumbIndicator.ActualWidth;
                case null:
                    return _SwitchTrack.ActualWidth / 2 - _ThumbIndicator.ActualWidth / 2;
                default:
                    return 0;
            }
        }

        private double? _lastDragPosition;
        private bool _isDragging;
        void _DraggingThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (!IsPressed)
                {
                    SetIsPressed(true);
                }
            }
            if (_ThumbTranslate != null)
            {
                _ThumbTranslate.BeginAnimation(TranslateTransform.XProperty, null);
                _ThumbTranslate.X = GetThumbDestination();
                _thumbAnimation = null;
            }
            _lastDragPosition = _ThumbTranslate.X;
            _isDragging = false;
        }

        void _DraggingThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_lastDragPosition.HasValue)
            {
                if (Math.Abs(e.HorizontalChange) > 3)
                    _isDragging = true;
                if (_SwitchTrack != null && _ThumbIndicator != null)
                {
                    double lastDragPosition = _lastDragPosition.Value;
                    _ThumbTranslate.X = Math.Min(_SwitchTrack.ActualWidth - (_SwitchTrack.Margin.Left + _SwitchTrack.Margin.Right + _ThumbIndicator.ActualWidth + _ThumbIndicator.Margin.Left + _ThumbIndicator.Margin.Right), Math.Max(0, lastDragPosition + e.HorizontalChange));
                }
            }
        }

        void _DraggingThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            SetIsPressed(false);
            _lastDragPosition = null;
            double _currentXPos = 0;
            double _switchTrackWidth = _SwitchTrack.ActualWidth;

            if (!_isDragging)
            {
                // Get the currentMousePosition 
                _currentXPos = Mouse.GetPosition(_SwitchTrack).X;
            }
            else if (_ThumbTranslate != null && _SwitchTrack != null)
            {
                // Get the currentThumbPosition 
                _currentXPos = _ThumbTranslate.X + ThumbIndicatorWidth / 2;
                _switchTrackWidth = _SwitchTrack.ActualWidth - ThumbIndicatorWidth / 2;
            }

            // Is ThreeStateAllowed or not?
            if (IsThreeState)
            {
                if (_currentXPos < _switchTrackWidth / 3)
                {
                    IsChecked = false;
                }
                else if (_currentXPos > _switchTrackWidth / 3 * 2)
                {
                    IsChecked = true;
                }
                else
                {
                    IsChecked = null;
                } 
            }
            else
            {
                if (_currentXPos < _switchTrackWidth / 2)
                {
                    IsChecked = false;
                }
                else
                {
                    IsChecked = true;
                }
            }

            UpdateThumb();
        }


        void _SwitchTrack_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_ThumbTranslate != null && _SwitchTrack != null && _ThumbIndicator != null)
            {
                _ThumbTranslate.X = GetThumbDestination();
            }
        }
    }
}