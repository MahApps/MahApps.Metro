// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    [ContentProperty(nameof(ItemsSource))]
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ButtonContent", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_Menu", Type = typeof(ContextMenu))]
    [StyleTypedProperty(Property = nameof(ButtonStyle), StyleTargetType = typeof(Button))]
    [StyleTypedProperty(Property = nameof(MenuStyle), StyleTargetType = typeof(ContextMenu))]
    public class DropDownButton : ItemsControl, ICommandSource
    {
        public static readonly RoutedEvent ClickEvent
            = EventManager.RegisterRoutedEvent(nameof(Click),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(DropDownButton));

        public event RoutedEventHandler Click
        {
            add => this.AddHandler(ClickEvent, value);
            remove => this.RemoveHandler(ClickEvent, value);
        }

        /// <summary>Identifies the <see cref="IsExpanded"/> dependency property.</summary>
        public static readonly DependencyProperty IsExpandedProperty
            = DependencyProperty.Register(nameof(IsExpanded),
                                          typeof(bool),
                                          typeof(DropDownButton),
                                          new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsExpandedPropertyChangedCallback));

        private static void OnIsExpandedPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is DropDownButton dropDownButton)
            {
                dropDownButton.SetContextMenuPlacementTarget(dropDownButton.contextMenu);
            }
        }

        protected virtual void SetContextMenuPlacementTarget(ContextMenu contextMenu)
        {
            if (this.button != null)
            {
                contextMenu.PlacementTarget = this.button;
            }
        }

        /// <summary> 
        /// Whether or not the "popup" menu for this control is currently open
        /// </summary>
        public bool IsExpanded
        {
            get => (bool)this.GetValue(IsExpandedProperty);
            set => this.SetValue(IsExpandedProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ExtraTag"/> dependency property.</summary>
        public static readonly DependencyProperty ExtraTagProperty
            = DependencyProperty.Register(nameof(ExtraTag),
                                          typeof(object),
                                          typeof(DropDownButton));

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
                                          typeof(DropDownButton),
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
                                          typeof(DropDownButton));

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
                                          typeof(DropDownButton));

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
                                          typeof(DropDownButton),
                                          new PropertyMetadata(null, OnCommandPropertyChangedCallback));

        private static void OnCommandPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            (dependencyObject as DropDownButton)?.OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
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
                                          typeof(DropDownButton),
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
                                          typeof(DropDownButton),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the parameter to pass to the command property.
        /// </summary>
        public object CommandParameter
        {
            get => (object)this.GetValue(CommandParameterProperty);
            set => this.SetValue(CommandParameterProperty, value);
        }

        /// <summary>Identifies the <see cref="Content"/> dependency property.</summary>
        public static readonly DependencyProperty ContentProperty
            = DependencyProperty.Register(nameof(Content),
                                          typeof(object),
                                          typeof(DropDownButton));

        /// <summary>
        /// Gets or sets the content of this control.
        /// </summary>
        public object Content
        {
            get => (object)this.GetValue(ContentProperty);
            set => this.SetValue(ContentProperty, value);
        }

        /// <summary>Identifies the <see cref="ContentTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty ContentTemplateProperty
            = DependencyProperty.Register(nameof(ContentTemplate),
                                          typeof(DataTemplate),
                                          typeof(DropDownButton),
                                          new FrameworkPropertyMetadata((DataTemplate)null));

        /// <summary> 
        /// Gets or sets the data template used to display the content of the DropDownButton.
        /// </summary>
        [Bindable(true)]
        public DataTemplate ContentTemplate
        {
            get => (DataTemplate)this.GetValue(ContentTemplateProperty);
            set => this.SetValue(ContentTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="ContentTemplateSelector"/> dependency property.</summary>
        public static readonly DependencyProperty ContentTemplateSelectorProperty
            = DependencyProperty.Register(nameof(ContentTemplateSelector),
                                          typeof(DataTemplateSelector),
                                          typeof(DropDownButton),
                                          new FrameworkPropertyMetadata((DataTemplateSelector)null));

        /// <summary>
        /// Gets or sets a template selector that enables an application writer to provide custom template-selection logic.
        /// </summary>
        /// <remarks> 
        /// This property is ignored if <seealso cref="ContentTemplate"/> is set.
        /// </remarks>
        [Bindable(true)]
        public DataTemplateSelector ContentTemplateSelector
        {
            get => (DataTemplateSelector)this.GetValue(ContentTemplateSelectorProperty);
            set => this.SetValue(ContentTemplateSelectorProperty, value);
        }

        /// <summary>Identifies the <see cref="ContentStringFormat"/> dependency property.</summary>
        public static readonly DependencyProperty ContentStringFormatProperty
            = DependencyProperty.Register(nameof(ContentStringFormat),
                                          typeof(string),
                                          typeof(DropDownButton),
                                          new FrameworkPropertyMetadata((string)null));

        /// <summary>
        /// Gets or sets a composite string that specifies how to format the content property if it is displayed as a string.
        /// </summary>
        /// <remarks> 
        /// This property is ignored if <seealso cref="ContentTemplate"/> is set.
        /// </remarks>
        [Bindable(true)]
        public string ContentStringFormat
        {
            get => (string)this.GetValue(ContentStringFormatProperty);
            set => this.SetValue(ContentStringFormatProperty, value);
        }

        /// <summary>Identifies the <see cref="ButtonStyle"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonStyleProperty
            = DependencyProperty.Register(nameof(ButtonStyle),
                                          typeof(Style),
                                          typeof(DropDownButton),
                                          new FrameworkPropertyMetadata(default(Style), FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the button content style.
        /// </summary>
        public Style ButtonStyle
        {
            get => (Style)this.GetValue(ButtonStyleProperty);
            set => this.SetValue(ButtonStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="MenuStyle"/> dependency property.</summary>
        public static readonly DependencyProperty MenuStyleProperty
            = DependencyProperty.Register(nameof(MenuStyle),
                                          typeof(Style),
                                          typeof(DropDownButton),
                                          new FrameworkPropertyMetadata(default(Style), FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the "popup" menu style.
        /// </summary>
        public Style MenuStyle
        {
            get => (Style)this.GetValue(MenuStyleProperty);
            set => this.SetValue(MenuStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="ArrowBrush"/> dependency property.</summary>
        public static readonly DependencyProperty ArrowBrushProperty
            = DependencyProperty.Register(nameof(ArrowBrush),
                                          typeof(Brush),
                                          typeof(DropDownButton),
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
                                          typeof(DropDownButton),
                                          new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the foreground brush of the button arrow icon if the mouse is over the drop down button.
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
                                          typeof(DropDownButton),
                                          new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the foreground brush of the button arrow icon if the arrow button is pressed.
        /// </summary>
        public Brush ArrowPressedBrush
        {
            get => (Brush)this.GetValue(ArrowPressedBrushProperty);
            set => this.SetValue(ArrowPressedBrushProperty, value);
        }

        /// <summary>Identifies the <see cref="ArrowVisibility"/> dependency property.</summary>
        public static readonly DependencyProperty ArrowVisibilityProperty
            = DependencyProperty.Register(nameof(ArrowVisibility),
                                          typeof(Visibility),
                                          typeof(DropDownButton),
                                          new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the visibility of the button arrow icon.
        /// </summary>
        public Visibility ArrowVisibility
        {
            get => (Visibility)this.GetValue(ArrowVisibilityProperty);
            set => this.SetValue(ArrowVisibilityProperty, value);
        }

        static DropDownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(typeof(DropDownButton)));
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

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            CommandHelpers.ExecuteCommandSource(this);

            if (this.contextMenu?.HasItems == true)
            {
                this.SetCurrentValue(IsExpandedProperty, BooleanBoxes.TrueBox);
            }

            e.RoutedEvent = ClickEvent;
            this.RaiseEvent(e);
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

            this.GroupStyle.CollectionChanged -= this.OnGroupStyleCollectionChanged;

            this.contextMenu = this.GetTemplateChild("PART_Menu") as ContextMenu;
            if (this.contextMenu != null)
            {
                foreach (var groupStyle in this.GroupStyle)
                {
                    this.contextMenu.GroupStyle.Add(groupStyle);
                }

                this.GroupStyle.CollectionChanged += this.OnGroupStyleCollectionChanged;

                if (this.Items != null && this.ItemsSource == null)
                {
                    foreach (var newItem in this.Items)
                    {
                        this.TryRemoveVisualFromOldTree(newItem);
                        this.contextMenu.Items.Add(newItem);
                    }
                }
            }
        }

        private void OnGroupStyleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var groupStyle in e.OldItems.OfType<GroupStyle>())
                {
                    this.contextMenu.GroupStyle.Remove(groupStyle);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var groupStyle in e.NewItems.OfType<GroupStyle>())
                {
                    this.contextMenu.GroupStyle.Add(groupStyle);
                }
            }
        }

        /// <inheritdoc />
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            e.Handled = true;
        }

        private void TryRemoveVisualFromOldTree(object item)
        {
            if (item is Visual visual)
            {
                var parent = LogicalTreeHelper.GetParent(visual) as FrameworkElement ?? VisualTreeHelper.GetParent(visual) as FrameworkElement;
                if (Equals(this, parent))
                {
                    this.RemoveLogicalChild(visual);
                    this.RemoveVisualChild(visual);
                }
            }
        }

        /// <summary>Invoked when the <see cref="P:System.Windows.Controls.ItemsControl.Items" /> property changes.</summary>
        /// <param name="e">Information about the change.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (this.contextMenu == null || this.ItemsSource != null || this.contextMenu.ItemsSource != null)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        foreach (var newItem in e.NewItems)
                        {
                            this.TryRemoveVisualFromOldTree(newItem);
                            this.contextMenu.Items.Add(newItem);
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (var oldItem in e.OldItems)
                        {
                            this.contextMenu.Items.Remove(oldItem);
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                    {
                        foreach (var oldItem in e.OldItems)
                        {
                            this.contextMenu.Items.Remove(oldItem);
                        }
                    }

                    if (e.NewItems != null)
                    {
                        foreach (var newItem in e.NewItems)
                        {
                            this.TryRemoveVisualFromOldTree(newItem);
                            this.contextMenu.Items.Add(newItem);
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (this.Items != null)
                    {
                        this.contextMenu.Items.Clear();
                        foreach (var newItem in this.Items)
                        {
                            this.TryRemoveVisualFromOldTree(newItem);
                            this.contextMenu.Items.Add(newItem);
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Button button;
        private ContextMenu contextMenu;
    }
}