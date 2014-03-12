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
using MahApps.Metro.Converters;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A control that allows the user to toggle between two states: One represents true; The other represents false.
    /// </summary>
    [TemplateVisualState(Name = NormalState, GroupName = CommonStates)]
    [TemplateVisualState(Name = DisabledState, GroupName = CommonStates)]
    [TemplatePart(Name = SwitchPart, Type = typeof(ToggleButton))]
    public class ToggleSwitch : ContentControl
    {
        private const string CommonStates = "CommonStates";
        private const string NormalState = "Normal";
        private const string DisabledState = "Disabled";
        private const string SwitchPart = "Switch";

        private ToggleButton _toggleButton;
        private bool _wasContentSet;

        public static readonly DependencyProperty OnLabelProperty = DependencyProperty.Register("OnLabel", typeof(string), typeof(ToggleSwitch), new PropertyMetadata("On"));
        public static readonly DependencyProperty OffLabelProperty = DependencyProperty.Register("OffLabel", typeof(string), typeof(ToggleSwitch), new PropertyMetadata("Off"));
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(ToggleSwitch), new PropertyMetadata(null));
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ToggleSwitch), new PropertyMetadata(null));
        public static readonly DependencyProperty SwitchForegroundProperty = DependencyProperty.Register("SwitchForeground", typeof(Brush), typeof(ToggleSwitch), null);
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool?), typeof(ToggleSwitch), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsCheckedChanged));
        // LeftToRight means content left and button right and RightToLeft vise versa
        public static readonly DependencyProperty ContentDirectionProperty = DependencyProperty.Register("ContentDirection", typeof(FlowDirection), typeof(ToggleSwitch), new PropertyMetadata(FlowDirection.LeftToRight));

        public event EventHandler<RoutedEventArgs> Checked;
        public event EventHandler<RoutedEventArgs> Unchecked;
        public event EventHandler<RoutedEventArgs> Indeterminate;
        public event EventHandler<RoutedEventArgs> Click;

        /// <summary>
        /// Gets/sets the text to display when the control is in it's On state.
        /// </summary>
        public string OnLabel
        {
            get { return (string)GetValue(OnLabelProperty); }
            set { SetValue(OnLabelProperty, value); }
        }

        /// <summary>
        /// Gets/sets the text to display when the control is in it's Off state.
        /// </summary>
        public string OffLabel
        {
            get { return (string)GetValue(OffLabelProperty); }
            set { SetValue(OffLabelProperty, value); }
        }

        /// <summary>
        /// Gets/sets the header to display on top of the control.
        /// </summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Gets/sets the data template used to display the header on the top of the control.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Gets/sets the brush used for the switch's foreground.
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
        /// Gets/sets the control's content flow direction.
        /// </summary>
        public FlowDirection ContentDirection {
            get { return (FlowDirection)GetValue(ContentDirectionProperty); }
            set { SetValue(ContentDirectionProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether the control is Checked (On) or not (Off).
        /// </summary>
        [TypeConverter(typeof(NullableBoolConverter))]
        public bool? IsChecked
        {
            get { return (bool?)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        /// <summary>
        /// An event that is raised when the value of IsChecked changes.
        /// </summary>
        public event EventHandler IsCheckedChanged;

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toggleSwitch = (ToggleSwitch)d;
            if (toggleSwitch._toggleButton != null)
            {
                var oldValue = (bool?)e.OldValue;
                var newValue = (bool?)e.NewValue;

                if (oldValue != newValue && toggleSwitch.IsCheckedChanged != null)
                {
                    toggleSwitch.IsCheckedChanged(toggleSwitch, EventArgs.Empty);
                }
            }
        }

        public ToggleSwitch()
        {
            DefaultStyleKey = typeof(ToggleSwitch);

            PreviewKeyUp += ToggleSwitch_PreviewKeyUp;
        }

        void ToggleSwitch_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Space && e.OriginalSource == sender)
                IsChecked = !IsChecked;       
        }

        private void SetDefaultContent()
        {
            Binding binding = new Binding("IsChecked") { Source = this, Converter = new OffOnConverter(), ConverterParameter = this };
            SetBinding(ContentProperty, binding);
        }

        private void ChangeVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, IsEnabled ? NormalState : DisabledState, useTransitions);
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            _wasContentSet = true;
        }

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
                BindingOperations.ClearBinding(_toggleButton, ToggleButton.IsCheckedProperty);

                _toggleButton.IsEnabledChanged -= IsEnabledHandler;
            }
            _toggleButton = GetTemplateChild(SwitchPart) as ToggleButton;
            if (_toggleButton != null)
            {
                _toggleButton.Checked += CheckedHandler;
                _toggleButton.Unchecked += UncheckedHandler;
                _toggleButton.Indeterminate += IndeterminateHandler;
                _toggleButton.Click += ClickHandler;
                var binding = new Binding("IsChecked") { Source = this };
                _toggleButton.SetBinding(ToggleButton.IsCheckedProperty, binding);

                _toggleButton.IsEnabledChanged += IsEnabledHandler;
            }
            ChangeVisualState(false);
        }

        private void IsEnabledHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            ChangeVisualState(false);
        }

        private void CheckedHandler(object sender, RoutedEventArgs e)
        {
            SafeRaise.Raise(Checked, this, e);
        }

        private void UncheckedHandler(object sender, RoutedEventArgs e)
        {
            SafeRaise.Raise(Unchecked, this, e);
        }

        private void IndeterminateHandler(object sender, RoutedEventArgs e)
        {
            SafeRaise.Raise(Indeterminate, this, e);
        }

        private void ClickHandler(object sender, RoutedEventArgs e)
        {
            SafeRaise.Raise(Click, this, e);
        }

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
}


