using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    [TemplateVisualState(Name = NormalState, GroupName = CommonStates)]
    [TemplateVisualState(Name = DisabledState, GroupName = CommonStates)]
    [TemplateVisualState(Name = CheckedState, GroupName = CheckStates)]
    [TemplateVisualState(Name = DraggingState, GroupName = CheckStates)]
    [TemplateVisualState(Name = UncheckedState, GroupName = CheckStates)]
    [TemplatePart(Name = SwitchRootPart, Type = typeof(Grid))]
    [TemplatePart(Name = SwitchBackgroundPart, Type = typeof(UIElement))]
    [TemplatePart(Name = SwitchTrackPart, Type = typeof(Grid))]
    [TemplatePart(Name = SwitchThumbPart, Type = typeof(FrameworkElement))]
    public class ToggleSwitchButton : ToggleButton
    {
        private const string CommonStates = "CommonStates";
        private const string NormalState = "Normal";
        private const string DisabledState = "Disabled";
        private const string CheckStates = "CheckStates";
        private const string CheckedState = "Checked";
        private const string DraggingState = "Dragging";
        private const string UncheckedState = "Unchecked";
        private const string SwitchRootPart = "SwitchRoot";
        private const string SwitchBackgroundPart = "SwitchBackground";
        private const string SwitchTrackPart = "SwitchTrack";
        private const string SwitchThumbPart = "SwitchThumb";
        
        private TranslateTransform _backgroundTranslation;
        private TranslateTransform _thumbTranslation;
        private Grid _root;
        private Grid _track;
        private FrameworkElement _thumb;
        private bool _isDragging = false;

        public static readonly DependencyProperty SwitchForegroundProperty = DependencyProperty.Register("SwitchForeground", typeof(Brush), typeof(ToggleSwitchButton), new PropertyMetadata(null));

        public Brush SwitchForeground
        {
            get
            {
                return (Brush)GetValue(SwitchForegroundProperty);
            }
            set
            {
                SetValue(SwitchForegroundProperty, value);
            }
        }

        public ToggleSwitchButton()
        {
            DefaultStyleKey = typeof(ToggleSwitchButton);
        }

        private void ChangeVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, IsEnabled ? NormalState : DisabledState, useTransitions);

            if (_isDragging)
            {
                // TODO: _isDragging is never set to true, so we never enter this state
                VisualStateManager.GoToState(this, DraggingState, useTransitions);
            }
            else if (IsChecked == true)
            {
                VisualStateManager.GoToState(this, CheckedState, useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, UncheckedState, useTransitions);
            }
        }

        protected override void OnToggle()
        {
            IsChecked = IsChecked != true;
            ChangeVisualState(true);
        }

        public override void OnApplyTemplate()
        {
            if (_track != null)
            {
                _track.SizeChanged -= SizeChangedHandler;
            }
            if (_thumb != null)
            {
                _thumb.SizeChanged -= SizeChangedHandler;
            }
            base.OnApplyTemplate();
            _root = GetTemplateChild(SwitchRootPart) as Grid;
            var background = GetTemplateChild(SwitchBackgroundPart) as UIElement;
            _backgroundTranslation = background == null ? null : background.RenderTransform as TranslateTransform;
            _track = GetTemplateChild(SwitchTrackPart) as Grid;
            _thumb = GetTemplateChild(SwitchThumbPart) as Border;
            _thumbTranslation = _thumb == null ? null : _thumb.RenderTransform as TranslateTransform;
            if (_root != null && _track != null && _thumb != null && (_backgroundTranslation != null || _thumbTranslation != null))
            {
                _track.SizeChanged += SizeChangedHandler;
                _thumb.SizeChanged += SizeChangedHandler;
            }
            ChangeVisualState(false);
        }

        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            _track.Clip = new RectangleGeometry { Rect = new Rect(0, 0, _track.ActualWidth, _track.ActualHeight) };
            // TODO: this value is being assigned on each callback but not used anywhere
            var checkedTranslation = _track.ActualWidth - _thumb.ActualWidth - _thumb.Margin.Left - _thumb.Margin.Right;
        }
    }
}