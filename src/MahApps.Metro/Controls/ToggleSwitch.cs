﻿// (c) Copyright Microsoft Corporation.
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
using System.Windows.Input;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A control that allows the user to toggle between two states: One represents true; The other represents false.
    /// </summary>
    [TemplateVisualState(Name = NormalState, GroupName = CommonStates)]
    [TemplateVisualState(Name = DisabledState, GroupName = CommonStates)]
    [TemplatePart(Name = SwitchPart, Type = typeof(ToggleButton))]
    public class ToggleSwitch : HeaderedContentControl
    {
        private const string CommonStates = "CommonStates";
        private const string NormalState = "Normal";
        private const string DisabledState = "Disabled";
        private const string SwitchPart = "Switch";

        private ToggleButton _toggleButton;

        public static readonly DependencyProperty HeaderFontFamilyProperty = DependencyProperty.Register("HeaderFontFamily", typeof(FontFamily), typeof(ToggleSwitch), new PropertyMetadata(SystemFonts.MessageFontFamily));

        public static readonly DependencyProperty OnLabelProperty = DependencyProperty.Register("OnLabel", typeof(string), typeof(ToggleSwitch), new PropertyMetadata("On"));
        public static readonly DependencyProperty OffLabelProperty = DependencyProperty.Register("OffLabel", typeof(string), typeof(ToggleSwitch), new PropertyMetadata("Off"));

        public static readonly DependencyProperty OnSwitchBrushProperty = DependencyProperty.Register("OnSwitchBrush", typeof(Brush), typeof(ToggleSwitch), null);
        public static readonly DependencyProperty OffSwitchBrushProperty = DependencyProperty.Register("OffSwitchBrush", typeof(Brush), typeof(ToggleSwitch), null);

        public static readonly DependencyProperty ThumbIndicatorBrushProperty = DependencyProperty.Register("ThumbIndicatorBrush", typeof(Brush), typeof(ToggleSwitch), null);
        public static readonly DependencyProperty ThumbIndicatorDisabledBrushProperty = DependencyProperty.Register("ThumbIndicatorDisabledBrush", typeof(Brush), typeof(ToggleSwitch), null);
        public static readonly DependencyProperty ThumbIndicatorWidthProperty = DependencyProperty.Register("ThumbIndicatorWidth", typeof(double), typeof(ToggleSwitch), new PropertyMetadata(13d));

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool?), typeof(ToggleSwitch), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, OnIsCheckedChanged));

        public static readonly DependencyProperty CheckChangedCommandProperty = DependencyProperty.Register("CheckChangedCommand", typeof(ICommand), typeof(ToggleSwitch), new PropertyMetadata(null));
        public static readonly DependencyProperty CheckedCommandProperty = DependencyProperty.Register("CheckedCommand", typeof(ICommand), typeof(ToggleSwitch), new PropertyMetadata(null));
        public static readonly DependencyProperty UnCheckedCommandProperty = DependencyProperty.Register("UnCheckedCommand", typeof(ICommand), typeof(ToggleSwitch), new PropertyMetadata(null));

        public static readonly DependencyProperty CheckChangedCommandParameterProperty = DependencyProperty.Register("CheckChangedCommandParameter", typeof(object), typeof(ToggleSwitch), new PropertyMetadata(null));
        public static readonly DependencyProperty CheckedCommandParameterProperty = DependencyProperty.Register("CheckedCommandParameter", typeof(object), typeof(ToggleSwitch), new PropertyMetadata(null));
        public static readonly DependencyProperty UnCheckedCommandParameterProperty = DependencyProperty.Register("UnCheckedCommandParameter", typeof(object), typeof(ToggleSwitch), new PropertyMetadata(null));

        // LeftToRight means content left and button right and RightToLeft vise versa
        public static readonly DependencyProperty ContentDirectionProperty = DependencyProperty.Register("ContentDirection", typeof(FlowDirection), typeof(ToggleSwitch), new PropertyMetadata(FlowDirection.LeftToRight));
        /// <summary>
        /// Identifies the <see cref="P:MahApps.Metro.Controls.ToggleSwitch.ContentPadding" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentPaddingProperty = DependencyProperty.Register(nameof(ContentPadding), typeof(Thickness), typeof(ToggleSwitch), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsParentMeasure));
        public static readonly DependencyProperty ToggleSwitchButtonStyleProperty = DependencyProperty.Register("ToggleSwitchButtonStyle", typeof(Style), typeof(ToggleSwitch), new FrameworkPropertyMetadata(default(Style), FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public event EventHandler<RoutedEventArgs> Checked;
        public event EventHandler<RoutedEventArgs> Unchecked;
        public event EventHandler<RoutedEventArgs> Indeterminate;
        public event EventHandler<RoutedEventArgs> Click;

        /// <summary>
        /// Gets/sets the font family of the header.
        /// </summary> 
        [Bindable(true), Localizability(LocalizationCategory.Font)]
        public FontFamily HeaderFontFamily
        {
            get { return (FontFamily)GetValue(HeaderFontFamilyProperty); }
            set { SetValue(HeaderFontFamilyProperty, value); }
        }

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
        /// Gets/sets the brush used for the thumb indicator.
        /// </summary>
        public Brush ThumbIndicatorBrush
        {
            get { return (Brush)GetValue(ThumbIndicatorBrushProperty); }
            set { SetValue(ThumbIndicatorBrushProperty, value); }
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
        /// Gets/sets the control's content flow direction.
        /// </summary>
        public FlowDirection ContentDirection
        {
            get { return (FlowDirection)GetValue(ContentDirectionProperty); }
            set { SetValue(ContentDirectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the padding of the inner content.
        /// </summary>
        [Bindable(true)]
        [Category(AppName.MahApps)]
        public Thickness ContentPadding
        {
            get { return (Thickness)this.GetValue(ContentPaddingProperty); }
            set { this.SetValue(ContentPaddingProperty, value); }
        }

        /// <summary>
        /// Gets/sets the control's toggle switch button style.
        /// </summary>
        public Style ToggleSwitchButtonStyle
        {
            get { return (Style)GetValue(ToggleSwitchButtonStyleProperty); }
            set { SetValue(ToggleSwitchButtonStyleProperty, value); }
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
        /// Gets/sets the command which will be executed if the IsChecked property was changed.
        /// </summary>
        public ICommand CheckChangedCommand
        {
            get { return (ICommand)GetValue(CheckChangedCommandProperty); }
            set { SetValue(CheckChangedCommandProperty, value); }
        }

        /// <summary>
        /// Gets/sets the command which will be executed if the checked event of the control is fired.
        /// </summary>
        public ICommand CheckedCommand
        {
            get { return (ICommand)GetValue(CheckedCommandProperty); }
            set { SetValue(CheckedCommandProperty, value); }
        }

        /// <summary>
        /// Gets/sets the command which will be executed if the checked event of the control is fired.
        /// </summary>
        public ICommand UnCheckedCommand
        {
            get { return (ICommand)GetValue(UnCheckedCommandProperty); }
            set { SetValue(UnCheckedCommandProperty, value); }
        }

        /// <summary>
        /// Gets/sets the command parameter which will be passed by the CheckChangedCommand.
        /// </summary>
        public object CheckChangedCommandParameter
        {
            get { return (object)GetValue(CheckChangedCommandParameterProperty); }
            set { SetValue(CheckChangedCommandParameterProperty, value); }
        }

        /// <summary>
        /// Gets/sets the command parameter which will be passed by the CheckedCommand.
        /// </summary>
        public object CheckedCommandParameter
        {
            get { return (object)GetValue(CheckedCommandParameterProperty); }
            set { SetValue(CheckedCommandParameterProperty, value); }
        }

        /// <summary>
        /// Gets/sets the command parameter which will be passed by the UnCheckedCommand.
        /// </summary>
        public object UnCheckedCommandParameter
        {
            get { return (object)GetValue(UnCheckedCommandParameterProperty); }
            set { SetValue(UnCheckedCommandParameterProperty, value); }
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

                if (oldValue != newValue)
                {
                    var command = toggleSwitch.CheckChangedCommand;
                    var commandParameter = toggleSwitch.CheckChangedCommandParameter ?? toggleSwitch;
                    if (command != null && command.CanExecute(commandParameter))
                    {
                        command.Execute(commandParameter);
                    }

                    var eh = toggleSwitch.IsCheckedChanged;
                    if (eh != null)
                    {
                        eh(toggleSwitch, EventArgs.Empty);
                    }
                }
            }
        }

        static ToggleSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleSwitch), new FrameworkPropertyMetadata(typeof(ToggleSwitch)));
        }

        public ToggleSwitch()
        {
            PreviewKeyUp += ToggleSwitch_PreviewKeyUp;
            MouseUp += (sender, args) => Keyboard.Focus(this);
        }

        void ToggleSwitch_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Space && e.OriginalSource == sender)
            {
                this.SetCurrentValue(ToggleSwitch.IsCheckedProperty, !IsChecked);
            }
        }

        private void ChangeVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, IsEnabled ? NormalState : DisabledState, useTransitions);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_toggleButton != null)
            {
                _toggleButton.Checked -= CheckedHandler;
                _toggleButton.Unchecked -= UncheckedHandler;
                _toggleButton.Indeterminate -= IndeterminateHandler;
                _toggleButton.Click -= ClickHandler;
                BindingOperations.ClearBinding(_toggleButton, ToggleButton.IsCheckedProperty);

                _toggleButton.IsEnabledChanged -= IsEnabledHandler;

                _toggleButton.PreviewMouseUp -= this.ToggleButtonPreviewMouseUp;
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

                _toggleButton.PreviewMouseUp += this.ToggleButtonPreviewMouseUp;
            }
            ChangeVisualState(false);
        }

        private void ToggleButtonPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(this);
        }

        private void IsEnabledHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            ChangeVisualState(false);
        }

        private void CheckedHandler(object sender, RoutedEventArgs e)
        {
            var command = this.CheckedCommand;
            var commandParameter = this.CheckedCommandParameter ?? this;
            if (command != null && command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }
            
            SafeRaise.Raise(Checked, this, e);
        }

        private void UncheckedHandler(object sender, RoutedEventArgs e)
        {
            var command = this.UnCheckedCommand;
            var commandParameter = this.UnCheckedCommandParameter ?? this;
            if (command != null && command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }

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


