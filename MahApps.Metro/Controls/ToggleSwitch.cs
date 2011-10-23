// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Represents a switch that can be toggled between two states.
    /// </summary>
    [TemplateVisualState(Name = NormalState, GroupName = CommonStates)]
    [TemplateVisualState(Name = DisabledState, GroupName = CommonStates)]
    [TemplatePart(Name = SwitchPart, Type = typeof(ToggleButton))]
    public class ToggleSwitch : ContentControl
    {
        /// <summary>
        /// Common visual states.
        /// </summary>
        private const string CommonStates = "CommonStates";

        /// <summary>
        /// Normal visual state.
        /// </summary>
        private const string NormalState = "Normal";

        /// <summary>
        /// Disabled visual state.
        /// </summary>
        private const string DisabledState = "Disabled";

        /// <summary>
        /// The ToggleButton that drives this.
        /// </summary>
        private const string SwitchPart = "Switch";

        /// <summary>
        /// Identifies the Header DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(ToggleSwitch), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the HeaderTemplate DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ToggleSwitch), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the template used to display the control's header.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the SwitchForeground DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty SwitchForegroundProperty = DependencyProperty.Register("SwitchForeground", typeof(Brush), typeof(ToggleSwitch), null);
        
        /// <summary>
        /// Gets or sets the switch foreground.
        /// </summary>
        public Brush SwitchForeground
        {
            get { return (Brush)GetValue(SwitchForegroundProperty); }
            set
            {
                SetValue(SwitchForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets whether the ToggleSwitch is checked.
        /// </summary>
        [TypeConverter(typeof(NullableBoolConverter))]
        public bool? IsChecked
        {
            get { return (bool?)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        /// <summary>
        /// Identifies the IsChecked DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool?), typeof(ToggleSwitch), new PropertyMetadata(false, OnIsCheckedChanged));

        /// <summary>
        /// Invoked when the IsChecked DependencyProperty is changed.
        /// </summary>
        /// <param name="d">The event sender.</param>
        /// <param name="e">The event information.</param>
        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)d;
            if (toggleSwitch._toggleButton != null)
            {
                toggleSwitch._toggleButton.IsChecked = (bool?)e.NewValue;
            }
        }

        /// <summary>
        /// Occurs when the
        /// <see cref="T:MahApps.Metro.Controls.ToggleSwitch"/>
        /// is checked.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Checked;

        /// <summary>
        /// Occurs when the
        /// <see cref="T:MahApps.Metro.Controls.ToggleSwitch"/>
        /// is unchecked.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Unchecked;

        /// <summary>
        /// Occurs when the
        /// <see cref="T:MahApps.Metro.Controls.ToggleSwitch"/>
        /// is indeterminate.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Indeterminate;

        /// <summary>
        /// Occurs when the
        /// <see cref="System.Windows.Controls.Primitives.ToggleButton"/>
        /// is clicked.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Click;

        /// <summary>
        /// The
        /// <see cref="System.Windows.Controls.Primitives.ToggleButton"/>
        /// template part.
        /// </summary>
        private ToggleButton _toggleButton;

        /// <summary>
        /// Whether the content was set.
        /// </summary>
        private bool _wasContentSet;

        /// <summary>
        /// Initializes a new instance of the ToggleSwitch class.
        /// </summary>
        public ToggleSwitch()
        {
            DefaultStyleKey = typeof(ToggleSwitch);
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleSwitch),
            //                                       new FrameworkPropertyMetadata(typeof(ToggleSwitch)));
        }

        /// <summary>
        /// Makes the content an "Off" or "On" string to match the state.
        /// </summary>
        private void SetDefaultContent()
        {
            Binding binding = new Binding("IsChecked") { Source = this, Converter = new OffOnConverter() };
            SetBinding(ContentProperty, binding);
        }

        /// <summary>
        /// Change the visual state.
        /// </summary>
        /// <param name="useTransitions">Indicates whether to use animation transitions.</param>
        private void ChangeVisualState(bool useTransitions)
        {
            if (IsEnabled)
            {
                VisualStateManager.GoToState(this, NormalState, useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, DisabledState, useTransitions);
            }
        }

        /// <summary>
        /// Makes the content an "Off" or "On" string to match the state if the content is set to null in the design tool.
        /// </summary>
        /// <param name="oldContent">The old content.</param>
        /// <param name="newContent">The new content.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            _wasContentSet = true;
            /*if (DesignerProperties.IsInDesignTool && newContent == null && GetBindingExpression(ContentProperty) == null)
            {
                SetDefaultContent();
            }*/
        }

        /// <summary>
        /// Gets all the template parts and initializes the corresponding state.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!_wasContentSet && GetBindingExpression(ContentProperty) == null)
            {
                SetDefaultContent();
            }

            if (_toggleButton != null)
            {
                _toggleButton.Checked -= CheckedHandler;
                _toggleButton.Unchecked -= UncheckedHandler;
                _toggleButton.Indeterminate -= IndeterminateHandler;
                _toggleButton.Click -= ClickHandler;
            }
            _toggleButton = GetTemplateChild(SwitchPart) as ToggleButton;
            if (_toggleButton != null)
            {
                _toggleButton.Checked += CheckedHandler;
                _toggleButton.Unchecked += UncheckedHandler;
                _toggleButton.Indeterminate += IndeterminateHandler;
                _toggleButton.Click += ClickHandler;
                _toggleButton.IsChecked = IsChecked;
            }
            ChangeVisualState(false);
        }

        /// <summary>
        /// Mirrors the
        /// <see cref="E:System.Windows.Controls.Primitives.ToggleButton.Checked"/>
        /// event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private void CheckedHandler(object sender, RoutedEventArgs e)
        {
            IsChecked = true;
            SafeRaise.Raise(Checked, this, e);
        }

        /// <summary>
        /// Mirrors the
        /// <see cref="E:System.Windows.Controls.Primitives.ToggleButton.Unchecked"/>
        /// event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private void UncheckedHandler(object sender, RoutedEventArgs e)
        {
            IsChecked = false;
            SafeRaise.Raise(Unchecked, this, e);
        }

        /// <summary>
        /// Mirrors the
        /// <see cref="E:System.Windows.Controls.Primitives.ToggleButton.Indeterminate"/>
        /// event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private void IndeterminateHandler(object sender, RoutedEventArgs e)
        {
            IsChecked = null;
            SafeRaise.Raise(Indeterminate, this, e);
        }

        /// <summary>
        /// Mirrors the 
        /// <see cref="E:System.Windows.Controls.Primitives.ToggleButton.Click"/>
        /// event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private void ClickHandler(object sender, RoutedEventArgs e)
        {
            SafeRaise.Raise(Click, this, e);
        }

        /// <summary>
        /// Returns a
        /// <see cref="T:System.String"/>
        /// that represents the current
        /// <see cref="T:System.Object"/>
        /// .
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{{ToggleSwitch IsChecked={0}, Content={1}}}",
                IsChecked,
                Content
            );
        }
    }

    /// <summary>
    /// A helper class for raising events safely.
    /// </summary>
    internal static class SafeRaise
    {
        /// <summary>
        /// Raises an event in a thread-safe manner, also does the null check.
        /// </summary>
        /// <param name="eventToRaise">The event to raise.</param>
        /// <param name="sender">The event sender.</param>
        public static void Raise(EventHandler eventToRaise, object sender)
        {
            if (eventToRaise != null)
            {
                eventToRaise(sender, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises an event in a thread-safe manner, also does the null check.
        /// </summary>
        /// <param name="eventToRaise">The event to raise.</param>
        /// <param name="sender">The event sender.</param>
        public static void Raise(EventHandler<EventArgs> eventToRaise, object sender)
        {
            Raise(eventToRaise, sender, EventArgs.Empty);
        }

        /// <summary>
        /// Raises an event in a thread-safe manner, also does the null check.
        /// </summary>
        /// <typeparam name="T">The event args type.</typeparam>
        /// <param name="eventToRaise">The event to raise.</param>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event args.</param>
        public static void Raise<T>(EventHandler<T> eventToRaise, object sender, T args) where T : EventArgs
        {
            if (eventToRaise != null)
            {
                eventToRaise(sender, args);
            }
        }

        // Lazy event args creation example:
        //
        // public class MyEventArgs : EventArgs
        // {
        //     public MyEventArgs(int x) { X = x; }
        //     public int X { get; set; }
        // }
        //
        // event EventHandler<MyEventArgs> Foo;
        //
        // public void Bar()
        // {
        //     int y = 2;
        //     Raise(Foo, null, () => { return new MyEventArgs(y); });
        // }

        /// <summary>
        /// This is a method that returns event args, used for lazy creation.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns></returns>
        public delegate T GetEventArgs<T>() where T : EventArgs;

        /// <summary>
        /// Raise an event in a thread-safe manner, with the required null check. Lazily creates event args.
        /// </summary>
        /// <typeparam name="T">The event args type.</typeparam>
        /// <param name="eventToRaise">The event to raise.</param>
        /// <param name="sender">The event sender.</param>
        /// <param name="getEventArgs">The delegate to return the event args if needed.</param>
        public static void Raise<T>(EventHandler<T> eventToRaise, object sender, GetEventArgs<T> getEventArgs) where T : EventArgs
        {
            if (eventToRaise != null)
            {
                eventToRaise(sender, getEventArgs());
            }
        }
    }

    public class OffOnConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool? || value == null)
            {
                return (bool?)value == true ? "On" : "Off";
            }
            return "Off";
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Represents a switch that can be toggled between two states.
    /// </summary>
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
        /// <summary>
        /// Common visual states.
        /// </summary>
        private const string CommonStates = "CommonStates";

        /// <summary>
        /// Normal visual state.
        /// </summary>
        private const string NormalState = "Normal";

        /// <summary>
        /// Disabled visual state.
        /// </summary>
        private const string DisabledState = "Disabled";

        /// <summary>
        /// Check visual states.
        /// </summary>
        private const string CheckStates = "CheckStates";

        /// <summary>
        /// Checked visual state.
        /// </summary>
        private const string CheckedState = "Checked";

        /// <summary>
        /// Dragging visual state.
        /// </summary>
        private const string DraggingState = "Dragging";

        /// <summary>
        /// Unchecked visual state.
        /// </summary>
        private const string UncheckedState = "Unchecked";

        /// <summary>
        /// Switch root template part name.
        /// </summary>
        private const string SwitchRootPart = "SwitchRoot";

        /// <summary>
        /// Switch background template part name.
        /// </summary>
        private const string SwitchBackgroundPart = "SwitchBackground";

        /// <summary>
        /// Switch track template part name.
        /// </summary>
        private const string SwitchTrackPart = "SwitchTrack";

        /// <summary>
        /// Switch thumb template part name.
        /// </summary>
        private const string SwitchThumbPart = "SwitchThumb";

        /// <summary>
        /// Identifies the SwitchForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty SwitchForegroundProperty =
            DependencyProperty.Register("SwitchForeground", typeof(Brush), typeof(ToggleSwitchButton), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the switch foreground.
        /// </summary>
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

        /// <summary>
        /// The background TranslateTransform.
        /// </summary>
        private TranslateTransform _backgroundTranslation;

        /// <summary>
        /// The thumb TranslateTransform.
        /// </summary>
        private TranslateTransform _thumbTranslation;

        /// <summary>
        /// The root template part.
        /// </summary>
        private Grid _root;

        /// <summary>
        /// The track template part.
        /// </summary>
        private Grid _track;

        /// <summary>
        /// The thumb template part.
        /// </summary>
        private FrameworkElement _thumb;

        /// <summary>
        /// The minimum translation.
        /// </summary>
        private const double _uncheckedTranslation = 0;

        /// <summary>
        /// The maximum translation.
        /// </summary>
        private double _checkedTranslation;

        /// <summary>
        /// The drag translation.
        /// </summary>
        private double _dragTranslation;

        /// <summary>
        /// Whether the translation ever changed during the drag.
        /// </summary>
        private bool _wasDragged;

        /// <summary>
        /// Whether the dragging state is current.
        /// </summary>
        private bool _isDragging;

        /// <summary>
        /// Initializes a new instance of the ToggleSwitch class.
        /// </summary>
        public ToggleSwitchButton()
        {
            DefaultStyleKey = typeof(ToggleSwitchButton);
        }

        /// <summary>
        /// Gets and sets the thumb and background translation.
        /// </summary>
        /// <returns>The translation.</returns>
        private double Translation
        {
            get
            {
                return _backgroundTranslation == null ? _thumbTranslation.X : _backgroundTranslation.X;
            }
            set
            {
                if (_backgroundTranslation != null)
                {
                    _backgroundTranslation.X = value;
                }

                if (_thumbTranslation != null)
                {
                    _thumbTranslation.X = value;
                }
            }
        }

        /// <summary>
        /// Change the visual state.
        /// </summary>
        /// <param name="useTransitions">Indicates whether to use animation transitions.</param>
        private void ChangeVisualState(bool useTransitions)
        {
            if (IsEnabled)
            {
                VisualStateManager.GoToState(this, NormalState, useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, DisabledState, useTransitions);
            }

            if (_isDragging)
            {
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

        /// <summary>
        /// Called by the OnClick method to implement toggle behavior.
        /// </summary>
        protected override void OnToggle()
        {
            IsChecked = IsChecked == true ? false : true;
            ChangeVisualState(true);
        }

        /// <summary>
        /// Gets all the template parts and initializes the corresponding state.
        /// </summary>
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
            UIElement background = GetTemplateChild(SwitchBackgroundPart) as UIElement;
            _backgroundTranslation = background == null ? null : background.RenderTransform as TranslateTransform;
            _track = GetTemplateChild(SwitchTrackPart) as Grid;
            _thumb = GetTemplateChild(SwitchThumbPart) as Border;
            _thumbTranslation = _thumb == null ? null : _thumb.RenderTransform as TranslateTransform;
            if (_root != null && _track != null && _thumb != null && (_backgroundTranslation != null || _thumbTranslation != null))
            {
                /*GestureListener gestureListener = GestureService.GetGestureListener(_root);
                gestureListener.DragStarted += DragStartedHandler;
                gestureListener.DragDelta += DragDeltaHandler;
                gestureListener.DragCompleted += DragCompletedHandler;*/
                _track.SizeChanged += SizeChangedHandler;
                _thumb.SizeChanged += SizeChangedHandler;
            }
            ChangeVisualState(false);
        }

        /// <summary>
        /// Handles changed sizes for the track and the thumb.
        /// Sets the clip of the track and computes the indeterminate and checked translations.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            _track.Clip = new RectangleGeometry { Rect = new Rect(0, 0, _track.ActualWidth, _track.ActualHeight) };
            _checkedTranslation = _track.ActualWidth - _thumb.ActualWidth - _thumb.Margin.Left - _thumb.Margin.Right;
        }
    }
}


