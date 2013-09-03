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

        public static readonly DependencyProperty OnProperty = DependencyProperty.Register("OnLabel", typeof(string), typeof(ToggleSwitch), new PropertyMetadata("On"));
        public static readonly DependencyProperty OffProperty = DependencyProperty.Register("OffLabel", typeof(string), typeof(ToggleSwitch), new PropertyMetadata("Off"));
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(ToggleSwitch), new PropertyMetadata(null));
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ToggleSwitch), new PropertyMetadata(null));
        public static readonly DependencyProperty SwitchForegroundProperty = DependencyProperty.Register("SwitchForeground", typeof(Brush), typeof(ToggleSwitch), null);
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool?), typeof(ToggleSwitch), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsCheckedChanged));

        public event EventHandler<RoutedEventArgs> Checked;
        public event EventHandler<RoutedEventArgs> Unchecked;
        public event EventHandler<RoutedEventArgs> Indeterminate;
        public event EventHandler<RoutedEventArgs> Click;

        public string OnLabel
        {
            get { return (string)GetValue(OnProperty); }
            set { SetValue(OnProperty, value); }
        }

        public string OffLabel
        {
            get { return (string)GetValue(OffProperty); }
            set { SetValue(OffProperty, value); }
        }

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public Brush SwitchForeground
        {
            get { return (Brush)GetValue(SwitchForegroundProperty); }
            set
            {
                SetValue(SwitchForegroundProperty, value);
            }
        }

        [TypeConverter(typeof(NullableBoolConverter))]
        public bool? IsChecked
        {
            get { return (bool?)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public event EventHandler IsCheckedChanged;

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toggleSwitch = (ToggleSwitch)d;
            if (toggleSwitch._toggleButton != null)
            {
                var oldValue = (bool?)e.OldValue;
                var newValue = (bool?)e.NewValue;

                toggleSwitch._toggleButton.IsChecked = newValue;

                if (oldValue != newValue
                    && toggleSwitch.IsCheckedChanged != null)
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
            }
            ChangeVisualState(false);
        }

        private void CheckedHandler(object sender, RoutedEventArgs e)
        {
            IsChecked = true;
            SafeRaise.Raise(Checked, this, e);
        }

        private void UncheckedHandler(object sender, RoutedEventArgs e)
        {
            IsChecked = false;
            SafeRaise.Raise(Unchecked, this, e);
        }

        private void IndeterminateHandler(object sender, RoutedEventArgs e)
        {
            IsChecked = null;
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


