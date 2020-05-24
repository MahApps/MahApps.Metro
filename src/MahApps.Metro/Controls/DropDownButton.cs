using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    [ContentProperty("ItemsSource")]
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ButtonContent", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_Menu", Type = typeof(ContextMenu))]
    public class DropDownButton : ItemsControl
    {
        public static readonly RoutedEvent ClickEvent
            = EventManager.RegisterRoutedEvent(nameof(Click),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(DropDownButton));

        public event RoutedEventHandler Click
        {
            add { this.AddHandler(ClickEvent, value); }
            remove { this.RemoveHandler(ClickEvent, value); }
        }

        /// <summary>Identifies the <see cref="IsExpanded"/> dependency property.</summary>
        public static readonly DependencyProperty IsExpandedProperty
            = DependencyProperty.Register(
                nameof(IsExpanded),
                typeof(bool),
                typeof(DropDownButton),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsExpandedPropertyChangedCallback));

        private static void IsExpandedPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            DropDownButton dropDownButton = (DropDownButton)dependencyObject;
            dropDownButton.SetContextMenuPlacementTarget(dropDownButton._contextMenu);
        }

        protected virtual void SetContextMenuPlacementTarget(ContextMenu contextMenu)
        {
            if (this._clickButton != null)
            {
                contextMenu.PlacementTarget = this._clickButton;
            }
        }

        /// <summary> 
        /// Whether or not the "popup" menu for this control is currently open
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)this.GetValue(IsExpandedProperty); }
            set { this.SetValue(IsExpandedProperty, value); }
        }

        /// <summary>Identifies the <see cref="ExtraTag"/> dependency property.</summary>
        public static readonly DependencyProperty ExtraTagProperty
            = DependencyProperty.Register(
                nameof(ExtraTag),
                typeof(object),
                typeof(DropDownButton));

        /// <summary>
        /// Gets or sets an extra tag.
        /// </summary>
        public object ExtraTag
        {
            get { return this.GetValue(ExtraTagProperty); }
            set { this.SetValue(ExtraTagProperty, value); }
        }

        /// <summary>Identifies the <see cref="Orientation"/> dependency property.</summary>
        public static readonly DependencyProperty OrientationProperty
            = DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(DropDownButton),
                new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the orientation of children stacking.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)this.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }

        /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(
                nameof(Icon),
                typeof(object),
                typeof(DropDownButton));

        /// <summary>
        /// Gets or sets the content for the icon part.
        /// </summary>
        [Bindable(true)]
        public object Icon
        {
            get { return this.GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        /// <summary>Identifies the <see cref="IconTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty IconTemplateProperty
            = DependencyProperty.Register(
                nameof(IconTemplate),
                typeof(DataTemplate),
                typeof(DropDownButton));

        /// <summary> 
        /// Gets or sets the DataTemplate for the icon part.
        /// </summary>
        [Bindable(true)]
        public DataTemplate IconTemplate
        {
            get { return (DataTemplate)this.GetValue(IconTemplateProperty); }
            set { this.SetValue(IconTemplateProperty, value); }
        }

        /// <summary>Identifies the <see cref="Command"/> dependency property.</summary>
        public static readonly DependencyProperty CommandProperty
            = DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(DropDownButton));

        /// <summary>
        /// Gets or sets the command to invoke when the content button is pressed.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
        }

        /// <summary>Identifies the <see cref="CommandTarget"/> dependency property.</summary>
        public static readonly DependencyProperty CommandTargetProperty
            = DependencyProperty.Register(
                nameof(CommandTarget),
                typeof(IInputElement),
                typeof(DropDownButton));

        /// <summary>
        /// Gets or sets the element on which to raise the specified command.
        /// </summary>
        public IInputElement CommandTarget
        {
            get { return (IInputElement)this.GetValue(CommandTargetProperty); }
            set { this.SetValue(CommandTargetProperty, value); }
        }

        /// <summary>Identifies the <see cref="CommandParameter"/> dependency property.</summary>
        public static readonly DependencyProperty CommandParameterProperty
            = DependencyProperty.Register(
                nameof(CommandParameter),
                typeof(object),
                typeof(DropDownButton));

        /// <summary>
        /// Gets or sets the parameter to pass to the command property.
        /// </summary>
        public object CommandParameter
        {
            get { return (object)this.GetValue(CommandParameterProperty); }
            set { this.SetValue(CommandParameterProperty, value); }
        }

        /// <summary>Identifies the <see cref="Content"/> dependency property.</summary>
        public static readonly DependencyProperty ContentProperty
            = DependencyProperty.Register(
                nameof(Content),
                typeof(object),
                typeof(DropDownButton));

        /// <summary>
        /// Gets or sets the content of this control.
        /// </summary>
        public object Content
        {
            get { return (object)this.GetValue(ContentProperty); }
            set { this.SetValue(ContentProperty, value); }
        }

        /// <summary>Identifies the <see cref="ContentTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty ContentTemplateProperty
            = DependencyProperty.Register(
                nameof(ContentTemplate),
                typeof(DataTemplate),
                typeof(DropDownButton),
                new FrameworkPropertyMetadata((DataTemplate)null));

        /// <summary> 
        /// Gets or sets the data template used to display the content of the DropDownButton.
        /// </summary>
        [Bindable(true)]
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)this.GetValue(ContentTemplateProperty); }
            set { this.SetValue(ContentTemplateProperty, value); }
        }

        /// <summary>Identifies the <see cref="ContentTemplateSelector"/> dependency property.</summary>
        public static readonly DependencyProperty ContentTemplateSelectorProperty
            = DependencyProperty.Register(
                nameof(ContentTemplateSelector),
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
            get { return (DataTemplateSelector)this.GetValue(ContentTemplateSelectorProperty); }
            set { this.SetValue(ContentTemplateSelectorProperty, value); }
        }

        /// <summary>Identifies the <see cref="ContentStringFormat"/> dependency property.</summary>
        public static readonly DependencyProperty ContentStringFormatProperty
            = DependencyProperty.Register(
                nameof(ContentStringFormat),
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
            get { return (string)this.GetValue(ContentStringFormatProperty); }
            set { this.SetValue(ContentStringFormatProperty, value); }
        }

        /// <summary>Identifies the <see cref="ButtonStyle"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonStyleProperty
            = DependencyProperty.Register(
                nameof(ButtonStyle),
                typeof(Style),
                typeof(DropDownButton),
                new FrameworkPropertyMetadata(default(Style), FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the button content style.
        /// </summary>
        public Style ButtonStyle
        {
            get { return (Style)this.GetValue(ButtonStyleProperty); }
            set { this.SetValue(ButtonStyleProperty, value); }
        }

        /// <summary>Identifies the <see cref="MenuStyle"/> dependency property.</summary>
        public static readonly DependencyProperty MenuStyleProperty
            = DependencyProperty.Register(
                nameof(MenuStyle),
                typeof(Style),
                typeof(DropDownButton),
                new FrameworkPropertyMetadata(default(Style), FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the "popup" menu style.
        /// </summary>
        public Style MenuStyle
        {
            get { return (Style)this.GetValue(MenuStyleProperty); }
            set { this.SetValue(MenuStyleProperty, value); }
        }

        /// <summary>Identifies the <see cref="ArrowBrush"/> dependency property.</summary>
        public static readonly DependencyProperty ArrowBrushProperty
            = DependencyProperty.Register(
                nameof(ArrowBrush),
                typeof(Brush),
                typeof(DropDownButton),
                new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the foreground brush for the button arrow icon.
        /// </summary>
        public Brush ArrowBrush
        {
            get { return (Brush)this.GetValue(ArrowBrushProperty); }
            set { this.SetValue(ArrowBrushProperty, value); }
        }

        /// <summary>Identifies the <see cref="ArrowMouseOverBrush"/> dependency property.</summary>
        public static readonly DependencyProperty ArrowMouseOverBrushProperty
            = DependencyProperty.Register(
                nameof(ArrowMouseOverBrush),
                typeof(Brush),
                typeof(DropDownButton),
                new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the foreground brush of the button arrow icon if the mouse is over the drop down button.
        /// </summary>
        public Brush ArrowMouseOverBrush
        {
            get { return (Brush)this.GetValue(ArrowMouseOverBrushProperty); }
            set { this.SetValue(ArrowMouseOverBrushProperty, value); }
        }

        /// <summary>Identifies the <see cref="ArrowPressedBrush"/> dependency property.</summary>
        public static readonly DependencyProperty ArrowPressedBrushProperty
            = DependencyProperty.Register(
                nameof(ArrowPressedBrush),
                typeof(Brush),
                typeof(DropDownButton),
                new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the foreground brush of the button arrow icon if the arrow button is pressed.
        /// </summary>
        public Brush ArrowPressedBrush
        {
            get { return (Brush)this.GetValue(ArrowPressedBrushProperty); }
            set { this.SetValue(ArrowPressedBrushProperty, value); }
        }

        /// <summary>Identifies the <see cref="ArrowVisibility"/> dependency property.</summary>
        public static readonly DependencyProperty ArrowVisibilityProperty
            = DependencyProperty.Register(
                nameof(ArrowVisibility),
                typeof(Visibility),
                typeof(DropDownButton),
                new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the visibility of the button arrow icon.
        /// </summary>
        public Visibility ArrowVisibility
        {
            get { return (Visibility)this.GetValue(ArrowVisibilityProperty); }
            set { this.SetValue(ArrowVisibilityProperty, value); }
        }

        static DropDownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(typeof(DropDownButton)));
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (this._contextMenu?.HasItems == true)
            {
                this.SetCurrentValue(IsExpandedProperty, true);
            }

            e.RoutedEvent = ClickEvent;
            this.RaiseEvent(e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this._clickButton != null)
            {
                this._clickButton.Click -= this.ButtonClick;
                this._clickButton.IsEnabledChanged -= this.ButtonIsEnabledChanged;
            }

            this._clickButton = this.GetTemplateChild("PART_Button") as Button;
            if (this._clickButton != null)
            {
                this._clickButton.Click += this.ButtonClick;
                this._clickButton.IsEnabledChanged += this.ButtonIsEnabledChanged;
            }

            this._contextMenu = this.GetTemplateChild("PART_Menu") as ContextMenu;

            if (this._contextMenu != null && this.Items != null && this.ItemsSource == null)
            {
                foreach (var newItem in this.Items)
                {
                    this.TryRemoveVisualFromOldTree(newItem);
                    this._contextMenu.Items.Add(newItem);
                }
            }
        }

        /// <inheritdoc />
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            e.Handled = true;
        }

        private void ButtonIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.SetCurrentValue(IsEnabledProperty, e.NewValue);
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
            if (this._contextMenu == null || this.ItemsSource != null || this._contextMenu.ItemsSource != null)
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
                            this._contextMenu.Items.Add(newItem);
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (var oldItem in e.OldItems)
                        {
                            this._contextMenu.Items.Remove(oldItem);
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                    {
                        foreach (var oldItem in e.OldItems)
                        {
                            this._contextMenu.Items.Remove(oldItem);
                        }
                    }

                    if (e.NewItems != null)
                    {
                        foreach (var newItem in e.NewItems)
                        {
                            this.TryRemoveVisualFromOldTree(newItem);
                            this._contextMenu.Items.Add(newItem);
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (this.Items != null)
                    {
                        this._contextMenu.Items.Clear();
                        foreach (var newItem in this.Items)
                        {
                            this.TryRemoveVisualFromOldTree(newItem);
                            this._contextMenu.Items.Add(newItem);
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Button _clickButton;
        private ContextMenu _contextMenu;
    }
}