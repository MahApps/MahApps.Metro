// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    [ContentProperty(nameof(ItemsSource))]
    [TemplatePart(Name = "PART_Container", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ButtonContent", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_Expander", Type = typeof(Button))]
    [StyleTypedProperty(Property = nameof(ButtonStyle), StyleTargetType = typeof(Button))]
    [StyleTypedProperty(Property = nameof(ButtonArrowStyle), StyleTargetType = typeof(Button))]
    public class SplitButton : ComboBox, ICommandSource
    {
        /// <summary>Identifies the <see cref="Click"/> routed event.</summary>
        public static readonly RoutedEvent ClickEvent
            = EventManager.RegisterRoutedEvent(nameof(Click),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(SplitButton));

        public event RoutedEventHandler Click
        {
            add => this.AddHandler(ClickEvent, value);
            remove => this.RemoveHandler(ClickEvent, value);
        }

        /// <summary>Identifies the <see cref="ExtraTag"/> dependency property.</summary>
        public static readonly DependencyProperty ExtraTagProperty
            = DependencyProperty.Register(nameof(ExtraTag),
                                          typeof(object),
                                          typeof(SplitButton));

        /// <summary>
        /// Gets or sets an extra tag.
        /// </summary>
        public object ExtraTag
        {
            get => this.GetValue(ExtraTagProperty);
            set => this.SetValue(ExtraTagProperty, value);
        }

        /// <summary>Identifies the <see cref="Orientation"/> dependency property.</summary>
        public static readonly DependencyProperty OrientationProperty
            = DependencyProperty.Register(nameof(Orientation),
                                          typeof(Orientation),
                                          typeof(SplitButton),
                                          new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the orientation of children stacking.
        /// </summary>
        public Orientation Orientation
        {
            get => (Orientation)this.GetValue(OrientationProperty);
            set => this.SetValue(OrientationProperty, value);
        }

        /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(nameof(Icon),
                                          typeof(object),
                                          typeof(SplitButton));

        /// <summary>
        /// Gets or sets the content for the icon part.
        /// </summary>
        [Bindable(true)]
        public object Icon
        {
            get => this.GetValue(IconProperty);
            set => this.SetValue(IconProperty, value);
        }

        /// <summary>Identifies the <see cref="IconTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty IconTemplateProperty
            = DependencyProperty.Register(nameof(IconTemplate),
                                          typeof(DataTemplate),
                                          typeof(SplitButton));

        /// <summary> 
        /// Gets or sets the DataTemplate for the icon part.
        /// </summary>
        [Bindable(true)]
        public DataTemplate IconTemplate
        {
            get => (DataTemplate)this.GetValue(IconTemplateProperty);
            set => this.SetValue(IconTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="Command"/> dependency property.</summary>
        public static readonly DependencyProperty CommandProperty
            = DependencyProperty.Register(nameof(Command),
                                          typeof(ICommand),
                                          typeof(SplitButton),
                                          new PropertyMetadata(null, OnCommandPropertyChangedCallback));

        private static void OnCommandPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            (dependencyObject as SplitButton)?.OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        /// <summary>
        /// Gets or sets the command to invoke when the content button is pressed.
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)this.GetValue(CommandProperty);
            set => this.SetValue(CommandProperty, value);
        }

        /// <summary>Identifies the <see cref="CommandTarget"/> dependency property.</summary>
        public static readonly DependencyProperty CommandTargetProperty
            = DependencyProperty.Register(nameof(CommandTarget),
                                          typeof(IInputElement),
                                          typeof(SplitButton),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the element on which to raise the specified command.
        /// </summary>
        public IInputElement CommandTarget
        {
            get => (IInputElement)this.GetValue(CommandTargetProperty);
            set => this.SetValue(CommandTargetProperty, value);
        }

        /// <summary>Identifies the <see cref="CommandParameter"/> dependency property.</summary>
        public static readonly DependencyProperty CommandParameterProperty
            = DependencyProperty.Register(nameof(CommandParameter),
                                          typeof(object),
                                          typeof(SplitButton),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the parameter to pass to the command property.
        /// </summary>
        public object CommandParameter
        {
            get => this.GetValue(CommandParameterProperty);
            set => this.SetValue(CommandParameterProperty, value);
        }

        /// <summary>Identifies the <see cref="ButtonStyle"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonStyleProperty
            = DependencyProperty.Register(nameof(ButtonStyle),
                                          typeof(Style),
                                          typeof(SplitButton),
                                          new FrameworkPropertyMetadata(default(Style), FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the button content style.
        /// </summary>
        public Style ButtonStyle
        {
            get => (Style)this.GetValue(ButtonStyleProperty);
            set => this.SetValue(ButtonStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="ButtonArrowStyle"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonArrowStyleProperty
            = DependencyProperty.Register(nameof(ButtonArrowStyle),
                                          typeof(Style),
                                          typeof(SplitButton),
                                          new FrameworkPropertyMetadata(default(Style), FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the button arrow style.
        /// </summary>
        public Style ButtonArrowStyle
        {
            get => (Style)this.GetValue(ButtonArrowStyleProperty);
            set => this.SetValue(ButtonArrowStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="ArrowBrush"/> dependency property.</summary>
        public static readonly DependencyProperty ArrowBrushProperty
            = DependencyProperty.Register(nameof(ArrowBrush),
                                          typeof(Brush),
                                          typeof(SplitButton),
                                          new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the foreground brush for the button arrow icon.
        /// </summary>
        public Brush ArrowBrush
        {
            get => (Brush)this.GetValue(ArrowBrushProperty);
            set => this.SetValue(ArrowBrushProperty, value);
        }

        /// <summary>Identifies the <see cref="ArrowMouseOverBrush"/> dependency property.</summary>
        public static readonly DependencyProperty ArrowMouseOverBrushProperty
            = DependencyProperty.Register(nameof(ArrowMouseOverBrush),
                                          typeof(Brush),
                                          typeof(SplitButton),
                                          new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the foreground brush of the button arrow icon if the mouse is over the split button.
        /// </summary>
        public Brush ArrowMouseOverBrush
        {
            get => (Brush)this.GetValue(ArrowMouseOverBrushProperty);
            set => this.SetValue(ArrowMouseOverBrushProperty, value);
        }

        /// <summary>Identifies the <see cref="ArrowPressedBrush"/> dependency property.</summary>
        public static readonly DependencyProperty ArrowPressedBrushProperty
            = DependencyProperty.Register(nameof(ArrowPressedBrush),
                                          typeof(Brush),
                                          typeof(SplitButton),
                                          new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the foreground brush of the button arrow icon if the arrow button is pressed.
        /// </summary>
        public Brush ArrowPressedBrush
        {
            get => (Brush)this.GetValue(ArrowPressedBrushProperty);
            set => this.SetValue(ArrowPressedBrushProperty, value);
        }

        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));

            IsEditableProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(false, null, CoerceIsEditableProperty));
        }

        private void OnCommandChanged(ICommand oldCommand, ICommand newCommand)
        {
            if (oldCommand != null)
            {
                this.UnhookCommand(oldCommand);
            }

            if (newCommand != null)
            {
                this.HookCommand(newCommand);
            }
        }

        private void UnhookCommand(ICommand command)
        {
            CanExecuteChangedEventManager.RemoveHandler(command, this.OnCanExecuteChanged);
            this.UpdateCanExecute();
        }

        private void HookCommand(ICommand command)
        {
            CanExecuteChangedEventManager.AddHandler(command, this.OnCanExecuteChanged);
            this.UpdateCanExecute();
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            this.UpdateCanExecute();
        }

        private void UpdateCanExecute()
        {
            this.CanExecute = this.Command == null || CommandHelpers.CanExecuteCommandSource(this);
        }

        /// <inheritdoc />
        protected override bool IsEnabledCore => base.IsEnabledCore && this.CanExecute;

        private bool canExecute = true;

        private bool CanExecute
        {
            get => this.canExecute;
            set
            {
                if (value == this.canExecute)
                {
                    return;
                }

                this.canExecute = value;
                this.CoerceValue(IsEnabledProperty);
            }
        }

        private static object CoerceIsEditableProperty(DependencyObject dependencyObject, object value)
        {
            // For now SplitButton is not editable
            return false;
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            CommandHelpers.ExecuteCommandSource(this);

            e.RoutedEvent = ClickEvent;
            this.RaiseEvent(e);

            this.SetCurrentValue(IsDropDownOpenProperty, BooleanBoxes.FalseBox);
        }

        private void ExpanderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SetCurrentValue(IsDropDownOpenProperty, BooleanBoxes.Box(!this.IsDropDownOpen));
            e.Handled = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.button != null)
            {
                this.button.Click -= this.ButtonClick;
            }

            this.button = this.GetTemplateChild("PART_Button") as Button;
            if (this.button != null)
            {
                this.button.Click += this.ButtonClick;
            }

            if (this.expanderButton != null)
            {
                this.expanderButton.PreviewMouseLeftButtonDown -= this.ExpanderMouseLeftButtonDown;
            }

            this.expanderButton = this.GetTemplateChild("PART_Expander") as Button;
            if (this.expanderButton != null)
            {
                this.expanderButton.PreviewMouseLeftButtonDown += this.ExpanderMouseLeftButtonDown;
            }
        }

        private Button button;
        private Button expanderButton;
    }
}