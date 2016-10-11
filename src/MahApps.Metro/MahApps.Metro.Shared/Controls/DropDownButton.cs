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
    [TemplatePart(Name = "PART_Button", Type = typeof(Button)),
     TemplatePart(Name = "PART_Image", Type = typeof(Image)),
     TemplatePart(Name = "PART_ButtonContent", Type = typeof(ContentControl)),
     TemplatePart(Name = "PART_Menu", Type = typeof(ContextMenu))]
    public class DropDownButton : ItemsControl
    {
        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble,
                                             typeof(RoutedEventHandler), typeof(DropDownButton));

        public event RoutedEventHandler Click
        {
            add { this.AddHandler(ClickEvent, value); }
            remove { this.RemoveHandler(ClickEvent, value); }
        }

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(DropDownButton), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsExpandedPropertyChangedCallback)));

        private static void IsExpandedPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            DropDownButton dropDownButton = (DropDownButton)dependencyObject;
            dropDownButton.SetContextMenuPlacementTarget(dropDownButton.menu);
        }

        protected virtual void SetContextMenuPlacementTarget(ContextMenu contextMenu)
        {
            if (this.clickButton != null)
            {
                contextMenu.PlacementTarget = this.clickButton;
            }
        }

        public static readonly DependencyProperty ExtraTagProperty = DependencyProperty.Register("ExtraTag", typeof(object), typeof(DropDownButton));

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(DropDownButton), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(DropDownButton));
        public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(DropDownButton));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(DropDownButton));
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(DropDownButton));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(DropDownButton));

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(DropDownButton));
        /// <summary>
        /// The DependencyProperty for the ContentTemplate property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(DropDownButton), new FrameworkPropertyMetadata((DataTemplate)null));
        /// <summary>
        /// The DependencyProperty for the ContentTemplateSelector property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(DropDownButton), new FrameworkPropertyMetadata((DataTemplateSelector)null));
        /// <summary>
        /// The DependencyProperty for the ContentStringFormat property. 
        /// </summary> 
        public static readonly DependencyProperty ContentStringFormatProperty = DependencyProperty.Register("ContentStringFormat", typeof(string), typeof(DropDownButton), new FrameworkPropertyMetadata((string)null));

        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register("ButtonStyle", typeof(Style), typeof(DropDownButton), new FrameworkPropertyMetadata(default(Style), FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty MenuStyleProperty = DependencyProperty.Register("MenuStyle", typeof(Style), typeof(DropDownButton), new FrameworkPropertyMetadata(default(Style), FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty ArrowBrushProperty = DependencyProperty.Register("ArrowBrush", typeof(Brush), typeof(DropDownButton), new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty ArrowMouseOverBrushProperty = DependencyProperty.Register("ArrowMouseOverBrush", typeof(Brush), typeof(DropDownButton), new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty ArrowPressedBrushProperty = DependencyProperty.Register("ArrowPressedBrush", typeof(Brush), typeof(DropDownButton), new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty ArrowVisibilityProperty = DependencyProperty.Register("ArrowVisibility", typeof(Visibility), typeof(DropDownButton), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the Content of this control..
        /// </summary>
        public object Content
        {
            get { return (object)this.GetValue(ContentProperty); }
            set { this.SetValue(ContentProperty, value); }
        }

        /// <summary> 
        /// ContentTemplate is the template used to display the content of the control. 
        /// </summary>
        [Bindable(true)]
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)this.GetValue(ContentTemplateProperty); }
            set { this.SetValue(ContentTemplateProperty, value); }
        }

        /// <summary>
        /// ContentTemplateSelector allows to provide custom logic for choosing the template used to display the content of the control.
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

        /// <summary>
        /// ContentStringFormat is the format used to display the content of the control as a string
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

        /// <summary>
        /// Reflects the parameter to pass to the CommandProperty upon execution. 
        /// </summary>
        public object CommandParameter
        {
            get { return (object)this.GetValue(CommandParameterProperty); }
            set { this.SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the target element on which to fire the command.
        /// </summary>
        public IInputElement CommandTarget
        {
            get { return (IInputElement)this.GetValue(CommandTargetProperty); }
            set { this.SetValue(CommandTargetProperty, value); }
        }

        /// <summary>
        /// Get or sets the Command property. 
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
        }

        /// <summary> 
        /// Indicates whether the Menu is visible. 
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)this.GetValue(IsExpandedProperty); }
            set { this.SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// Gets or sets an extra tag.
        /// </summary>
        public object ExtraTag
        {
            get { return this.GetValue(ExtraTagProperty); }
            set { this.SetValue(ExtraTagProperty, value); }
        }

        /// <summary>
        /// Gets or sets the dimension of children stacking.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)this.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }

        /// <summary>
        ///  Gets or sets the Content used to generate the icon part.
        /// </summary>
        [Bindable(true)]
        public object Icon
        {
            get { return this.GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        /// <summary> 
        /// Gets or sets the ContentTemplate used to display the content of the icon part. 
        /// </summary>
        [Bindable(true)]
        public DataTemplate IconTemplate
        {
            get { return (DataTemplate)this.GetValue(IconTemplateProperty); }
            set { this.SetValue(IconTemplateProperty, value); }
        }

        /// <summary>
        /// Gets/sets the button style.
        /// </summary>
        public Style ButtonStyle
        {
            get { return (Style)this.GetValue(ButtonStyleProperty); }
            set { this.SetValue(ButtonStyleProperty, value); }
        }

        /// <summary>
        /// Gets/sets the menu style.
        /// </summary>
        public Style MenuStyle
        {
            get { return (Style)this.GetValue(MenuStyleProperty); }
            set { this.SetValue(MenuStyleProperty, value); }
        }

        /// <summary>
        /// Gets/sets the brush of the button arrow icon.
        /// </summary>
        public Brush ArrowBrush
        {
            get { return (Brush)this.GetValue(ArrowBrushProperty); }
            set { this.SetValue(ArrowBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the brush of the button arrow icon if the mouse is over the drop down button.
        /// </summary>
        public Brush ArrowMouseOverBrush
        {
            get { return (Brush)this.GetValue(ArrowMouseOverBrushProperty); }
            set { this.SetValue(ArrowMouseOverBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the brush of the button arrow icon if the arrow button is pressed.
        /// </summary>
        public Brush ArrowPressedBrush
        {
            get { return (Brush)this.GetValue(ArrowPressedBrushProperty); }
            set { this.SetValue(ArrowPressedBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the visibility of the button arrow icon.
        /// </summary>
        public Visibility ArrowVisibility
        {
            get { return (Visibility)this.GetValue(ArrowVisibilityProperty); }
            set { this.SetValue(ArrowVisibilityProperty, value); }
        }

        private Button clickButton;
        private ContextMenu menu;

        static DropDownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(typeof(DropDownButton)));
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            this.IsExpanded = true;
            e.RoutedEvent = ClickEvent;
            this.RaiseEvent(e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.clickButton = this.EnforceInstance<Button>("PART_Button");
            this.menu = this.EnforceInstance<ContextMenu>("PART_Menu");
            this.InitializeVisualElementsContainer();
            if (this.menu != null && this.Items != null && this.ItemsSource == null)
            {
                foreach (var newItem in this.Items)
                {
                    this.TryRemoveVisualFromOldTree(newItem);
                    this.menu.Items.Add(newItem);
                }
            }
        }

        private void TryRemoveVisualFromOldTree(object newItem)
        {
            var visual = newItem as Visual;
            if (visual != null)
            {
                var fe = LogicalTreeHelper.GetParent(visual) as FrameworkElement ?? VisualTreeHelper.GetParent(visual) as FrameworkElement;
                if (Equals(this, fe))
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
            if (this.menu == null || this.ItemsSource != null || this.menu.ItemsSource != null)
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
                            this.menu.Items.Add(newItem);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (var oldItem in e.OldItems)
                        {
                            this.menu.Items.Remove(oldItem);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                    {
                        foreach (var oldItem in e.OldItems)
                        {
                            this.menu.Items.Remove(oldItem);
                        }
                    }
                    if (e.NewItems != null)
                    {
                        foreach (var newItem in e.NewItems)
                        {
                            this.TryRemoveVisualFromOldTree(newItem);
                            this.menu.Items.Add(newItem);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (this.Items != null)
                    {
                        this.menu.Items.Clear();
                        foreach (var newItem in this.Items)
                        {
                            this.TryRemoveVisualFromOldTree(newItem);
                            this.menu.Items.Add(newItem);
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //Get element from name. If it exist then element instance return, if not, new will be created
        private T EnforceInstance<T>(string partName) where T : FrameworkElement, new()
        {
            T element = this.GetTemplateChild(partName) as T ?? new T();
            return element;
        }

        private void InitializeVisualElementsContainer()
        {
            this.MouseRightButtonUp -= this.DropDownButtonMouseRightButtonUp;
            this.clickButton.Click -= this.ButtonClick;
            this.MouseRightButtonUp += this.DropDownButtonMouseRightButtonUp;
            this.clickButton.Click += this.ButtonClick;
        }

        private void DropDownButtonMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}