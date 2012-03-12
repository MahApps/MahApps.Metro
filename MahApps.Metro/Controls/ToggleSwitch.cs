// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace MahApps.Metro.Controls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// Represents a switch that can be toggled between two states.
    /// </summary>
    [TemplateVisualState(Name = NormalState, GroupName = CommonStates)]
    [TemplateVisualState(Name = DisabledState, GroupName = CommonStates)]
    [TemplatePart(Name = SwitchPart, Type = typeof(ToggleButton))]
    public class ToggleSwitch : ContentControl
    {
        #region Constants and Fields

        /// <summary>
        /// Identifies the Header DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header", typeof(object), typeof(ToggleSwitch), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the HeaderTemplate DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            "HeaderTemplate", typeof(DataTemplate), typeof(ToggleSwitch), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the IsChecked DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
            "IsChecked", typeof(bool?), typeof(ToggleSwitch), new PropertyMetadata(false, OnIsCheckedChanged));

        /// <summary>
        /// Identifies the SwitchForeground DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty SwitchForegroundProperty =
            DependencyProperty.Register("SwitchForeground", typeof(Brush), typeof(ToggleSwitch), null);

        /// <summary>
        /// Common visual states.
        /// </summary>
        private const string CommonStates = "CommonStates";

        /// <summary>
        /// Disabled visual state.
        /// </summary>
        private const string DisabledState = "Disabled";

        /// <summary>
        /// Normal visual state.
        /// </summary>
        private const string NormalState = "Normal";

        /// <summary>
        /// The ToggleButton that drives this.
        /// </summary>
        private const string SwitchPart = "Switch";

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

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the ToggleSwitch class.
        /// </summary>
        public ToggleSwitch()
        {
            this.DefaultStyleKey = typeof(ToggleSwitch);
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleSwitch),
            //                                       new FrameworkPropertyMetadata(typeof(ToggleSwitch)));
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Occurs when the
        /// <see cref="T:MahApps.Metro.Controls.ToggleSwitch"/>
        /// is checked.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Checked;

        /// <summary>
        /// Occurs when the
        /// <see cref="System.Windows.Controls.Primitives.ToggleButton"/>
        /// is clicked.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Click;

        /// <summary>
        /// Occurs when the
        /// <see cref="T:MahApps.Metro.Controls.ToggleSwitch"/>
        /// is indeterminate.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Indeterminate;

        /// <summary>
        /// Occurs when the
        /// <see cref="T:MahApps.Metro.Controls.ToggleSwitch"/>
        /// is unchecked.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Unchecked;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public object Header
        {
            get
            {
                return this.GetValue(HeaderProperty);
            }
            set
            {
                this.SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the template used to display the control's header.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(HeaderTemplateProperty);
            }
            set
            {
                this.SetValue(HeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets whether the ToggleSwitch is checked.
        /// </summary>
        [TypeConverter(typeof(NullableBoolConverter))]
        public bool? IsChecked
        {
            get
            {
                return (bool?)this.GetValue(IsCheckedProperty);
            }
            set
            {
                this.SetValue(IsCheckedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the switch foreground.
        /// </summary>
        public Brush SwitchForeground
        {
            get
            {
                return (Brush)this.GetValue(SwitchForegroundProperty);
            }
            set
            {
                this.SetValue(SwitchForegroundProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets all the template parts and initializes the corresponding state.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!this._wasContentSet && this.GetBindingExpression(ContentProperty) == null)
            {
                this.SetDefaultContent();
            }

            if (this._toggleButton != null)
            {
                this._toggleButton.Checked -= this.CheckedHandler;
                this._toggleButton.Unchecked -= this.UncheckedHandler;
                this._toggleButton.Indeterminate -= this.IndeterminateHandler;
                this._toggleButton.Click -= this.ClickHandler;
            }
            this._toggleButton = this.GetTemplateChild(SwitchPart) as ToggleButton;
            if (this._toggleButton != null)
            {
                this._toggleButton.Checked += this.CheckedHandler;
                this._toggleButton.Unchecked += this.UncheckedHandler;
                this._toggleButton.Indeterminate += this.IndeterminateHandler;
                this._toggleButton.Click += this.ClickHandler;
                this._toggleButton.IsChecked = this.IsChecked;
            }
            this.ChangeVisualState(false);
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
                this.IsChecked,
                this.Content);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Makes the content an "Off" or "On" string to match the state if the content is set to null in the design tool.
        /// </summary>
        /// <param name="oldContent">The old content.</param>
        /// <param name="newContent">The new content.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            this._wasContentSet = true;
            /*if (DesignerProperties.IsInDesignTool && newContent == null && GetBindingExpression(ContentProperty) == null)
            {
                SetDefaultContent();
            }*/
        }

        /// <summary>
        /// Invoked when the IsChecked DependencyProperty is changed.
        /// </summary>
        /// <param name="d">The event sender.</param>
        /// <param name="e">The event information.</param>
        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toggleSwitch = (ToggleSwitch)d;
            if (toggleSwitch._toggleButton != null)
            {
                toggleSwitch._toggleButton.IsChecked = (bool?)e.NewValue;
            }
        }

        /// <summary>
        /// Change the visual state.
        /// </summary>
        /// <param name="useTransitions">Indicates whether to use animation transitions.</param>
        private void ChangeVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, this.IsEnabled ? NormalState : DisabledState, useTransitions);
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
            this.IsChecked = true;
            SafeRaise.Raise(this.Checked, this, e);
        }

        /// <summary>
        /// Mirrors the 
        /// <see cref="System.Windows.Controls.Primitives.ToggleButton.Click"/>
        /// event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private void ClickHandler(object sender, RoutedEventArgs e)
        {
            SafeRaise.Raise(this.Click, this, e);
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
            this.IsChecked = null;
            SafeRaise.Raise(this.Indeterminate, this, e);
        }

        /// <summary>
        /// Makes the content an "Off" or "On" string to match the state.
        /// </summary>
        private void SetDefaultContent()
        {
            var binding = new Binding("IsChecked") { Source = this, Converter = new OffOnConverter() };
            SetBinding(ContentProperty, binding);
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
            this.IsChecked = false;
            SafeRaise.Raise(this.Unchecked, this, e);
        }

        #endregion
    }

    /// <summary>
    /// A helper class for raising events safely.
    /// </summary>
    internal static class SafeRaise
    {
        #region Delegates

        /// <summary>
        /// This is a method that returns event args, used for lazy creation.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns></returns>
        public delegate T GetEventArgs<out T>() where T : EventArgs;

        #endregion

        #region Public Methods and Operators

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
        /// Raise an event in a thread-safe manner, with the required null check. Lazily creates event args.
        /// </summary>
        /// <typeparam name="T">The event args type.</typeparam>
        /// <param name="eventToRaise">The event to raise.</param>
        /// <param name="sender">The event sender.</param>
        /// <param name="getEventArgs">The delegate to return the event args if needed.</param>
        public static void Raise<T>(EventHandler<T> eventToRaise, object sender, GetEventArgs<T> getEventArgs)
            where T : EventArgs
        {
            if (eventToRaise != null)
            {
                eventToRaise(sender, getEventArgs());
            }
        }

        #endregion
    }

    public class OffOnConverter : IValueConverter
    {
        #region Public Methods and Operators

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

        #endregion
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
        #region Constants and Fields

        /// <summary>
        /// Identifies the SwitchForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty SwitchForegroundProperty =
            DependencyProperty.Register(
                "SwitchForeground", typeof(Brush), typeof(ToggleSwitchButton), new PropertyMetadata(null));

        /// <summary>
        /// Check visual states.
        /// </summary>
        private const string CheckStates = "CheckStates";

        /// <summary>
        /// Checked visual state.
        /// </summary>
        private const string CheckedState = "Checked";

        /// <summary>
        /// Common visual states.
        /// </summary>
        private const string CommonStates = "CommonStates";

        /// <summary>
        /// Disabled visual state.
        /// </summary>
        private const string DisabledState = "Disabled";

        /// <summary>
        /// Dragging visual state.
        /// </summary>
        private const string DraggingState = "Dragging";

        /// <summary>
        /// Normal visual state.
        /// </summary>
        private const string NormalState = "Normal";

        /// <summary>
        /// Switch background template part name.
        /// </summary>
        private const string SwitchBackgroundPart = "SwitchBackground";

        /// <summary>
        /// Switch root template part name.
        /// </summary>
        private const string SwitchRootPart = "SwitchRoot";

        /// <summary>
        /// Switch thumb template part name.
        /// </summary>
        private const string SwitchThumbPart = "SwitchThumb";

        /// <summary>
        /// Switch track template part name.
        /// </summary>
        private const string SwitchTrackPart = "SwitchTrack";

        /// <summary>
        /// Unchecked visual state.
        /// </summary>
        private const string UncheckedState = "Unchecked";

        /// <summary>
        /// The minimum translation.
        /// </summary>
        private const double _uncheckedTranslation = 0;

        /// <summary>
        /// The background TranslateTransform.
        /// </summary>
        private TranslateTransform _backgroundTranslation;

        /// <summary>
        /// The maximum translation.
        /// </summary>
        private double _checkedTranslation;

        /// <summary>
        /// The drag translation.
        /// </summary>
        private double _dragTranslation;

        /// <summary>
        /// Whether the dragging state is current.
        /// </summary>
        private bool _isDragging;

        /// <summary>
        /// The root template part.
        /// </summary>
        private Grid _root;

        /// <summary>
        /// The thumb template part.
        /// </summary>
        private FrameworkElement _thumb;

        /// <summary>
        /// The thumb TranslateTransform.
        /// </summary>
        private TranslateTransform _thumbTranslation;

        /// <summary>
        /// The track template part.
        /// </summary>
        private Grid _track;

        /// <summary>
        /// Whether the translation ever changed during the drag.
        /// </summary>
        private bool _wasDragged;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the ToggleSwitch class.
        /// </summary>
        public ToggleSwitchButton()
        {
            this.DefaultStyleKey = typeof(ToggleSwitchButton);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the switch foreground.
        /// </summary>
        public Brush SwitchForeground
        {
            get
            {
                return (Brush)this.GetValue(SwitchForegroundProperty);
            }
            set
            {
                this.SetValue(SwitchForegroundProperty, value);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the thumb and background translation.
        /// </summary>
        /// <returns>The translation.</returns>
        private double Translation
        {
            get
            {
                return this._backgroundTranslation == null ? this._thumbTranslation.X : this._backgroundTranslation.X;
            }
            set
            {
                if (this._backgroundTranslation != null)
                {
                    this._backgroundTranslation.X = value;
                }

                if (this._thumbTranslation != null)
                {
                    this._thumbTranslation.X = value;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets all the template parts and initializes the corresponding state.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (this._track != null)
            {
                this._track.SizeChanged -= this.SizeChangedHandler;
            }
            if (this._thumb != null)
            {
                this._thumb.SizeChanged -= this.SizeChangedHandler;
            }
            base.OnApplyTemplate();
            this._root = this.GetTemplateChild(SwitchRootPart) as Grid;
            var background = this.GetTemplateChild(SwitchBackgroundPart) as UIElement;
            this._backgroundTranslation = background == null ? null : background.RenderTransform as TranslateTransform;
            this._track = this.GetTemplateChild(SwitchTrackPart) as Grid;
            this._thumb = this.GetTemplateChild(SwitchThumbPart) as Border;
            this._thumbTranslation = this._thumb == null ? null : this._thumb.RenderTransform as TranslateTransform;
            if (this._root != null && this._track != null && this._thumb != null
                && (this._backgroundTranslation != null || this._thumbTranslation != null))
            {
                /*GestureListener gestureListener = GestureService.GetGestureListener(_root);
                gestureListener.DragStarted += DragStartedHandler;
                gestureListener.DragDelta += DragDeltaHandler;
                gestureListener.DragCompleted += DragCompletedHandler;*/
                this._track.SizeChanged += this.SizeChangedHandler;
                this._thumb.SizeChanged += this.SizeChangedHandler;
            }
            this.ChangeVisualState(false);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called by the OnClick method to implement toggle behavior.
        /// </summary>
        protected override void OnToggle()
        {
            this.IsChecked = this.IsChecked != true;
            this.ChangeVisualState(true);
        }

        /// <summary>
        /// Change the visual state.
        /// </summary>
        /// <param name="useTransitions">Indicates whether to use animation transitions.</param>
        private void ChangeVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, this.IsEnabled ? NormalState : DisabledState, useTransitions);

            if (this._isDragging)
            {
                VisualStateManager.GoToState(this, DraggingState, useTransitions);
            }
            else if (this.IsChecked == true)
            {
                VisualStateManager.GoToState(this, CheckedState, useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, UncheckedState, useTransitions);
            }
        }

        /// <summary>
        /// Handles changed sizes for the track and the thumb.
        /// Sets the clip of the track and computes the indeterminate and checked translations.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            this._track.Clip = new RectangleGeometry
                { Rect = new Rect(0, 0, this._track.ActualWidth, this._track.ActualHeight) };
            this._checkedTranslation = this._track.ActualWidth - this._thumb.ActualWidth - this._thumb.Margin.Left
                                       - this._thumb.Margin.Right;
        }

        #endregion
    }
}