using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(DropDownButton), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsExpandedPropertyChangedCallback)));

        private static void IsExpandedPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            DropDownButton button = (DropDownButton)dependencyObject;
            if (button.clickButton != null)
            {
                button.menu.Placement = PlacementMode.Bottom;
                button.menu.PlacementTarget = button.clickButton;
            }
        }

        public static readonly DependencyProperty ExtraTagProperty = DependencyProperty.Register("ExtraTag", typeof(Object), typeof(DropDownButton));

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(DropDownButton), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(Object), typeof(DropDownButton));
        public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(DropDownButton));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(DropDownButton));
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(DropDownButton));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(Object), typeof(DropDownButton));

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(Object), typeof(DropDownButton));

        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register("ButtonStyle", typeof(Style), typeof(DropDownButton), new FrameworkPropertyMetadata(default(Style), FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty MenuStyleProperty = DependencyProperty.Register("MenuStyle", typeof(Style), typeof(DropDownButton), new FrameworkPropertyMetadata(default(Style), FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty ArrowBrushProperty = DependencyProperty.Register("ArrowBrush", typeof(Brush), typeof(DropDownButton), new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty ArrowVisibilityProperty = DependencyProperty.Register("ArrowVisibility", typeof(Visibility), typeof(DropDownButton), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the Content of this control..
        /// </summary>
        public Object Content
        {
            get { return (Object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Reflects the parameter to pass to the CommandProperty upon execution. 
        /// </summary>
        public Object CommandParameter
        {
            get { return (Object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the target element on which to fire the command.
        /// </summary>
        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        /// <summary>
        /// Get or sets the Command property. 
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary> 
        /// Indicates whether the Menu is visible. 
        /// </summary>
        public Boolean IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// Gets or sets an extra tag.
        /// </summary>
        public Object ExtraTag
        {
            get { return GetValue(ExtraTagProperty); }
            set { SetValue(ExtraTagProperty, value); }
        }

        /// <summary>
        /// Gets or sets the dimension of children stacking.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        ///  Gets or sets the Content used to generate the icon part.
        /// </summary>
        [Bindable(true)]
        public Object Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary> 
        /// Gets or sets the ContentTemplate used to display the content of the icon part. 
        /// </summary>
        [Bindable(true)]
        public DataTemplate IconTemplate
        {
            get { return (DataTemplate)GetValue(IconTemplateProperty); }
            set { SetValue(IconTemplateProperty, value); }
        }

        /// <summary>
        /// Gets/sets the button style.
        /// </summary>
        public Style ButtonStyle
        {
            get { return (Style)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }

        /// <summary>
        /// Gets/sets the menu style.
        /// </summary>
        public Style MenuStyle
        {
            get { return (Style)GetValue(MenuStyleProperty); }
            set { SetValue(MenuStyleProperty, value); }
        }

        /// <summary>
        /// Gets/sets the brush of the button arrow icon.
        /// </summary>
        public Brush ArrowBrush
        {
            get { return (Brush)GetValue(ArrowBrushProperty); }
            set { SetValue(ArrowBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the visibility of the button arrow icon.
        /// </summary>
        public Visibility ArrowVisibility
        {
            get { return (Visibility)GetValue(ArrowVisibilityProperty); }
            set { SetValue(ArrowVisibilityProperty, value); }
        }

        private Button clickButton;
        private ContextMenu menu;

        static DropDownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(typeof(DropDownButton)));
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            IsExpanded = true;
            e.RoutedEvent = ClickEvent;
            RaiseEvent(e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            clickButton = EnforceInstance<Button>("PART_Button");
            menu = EnforceInstance<ContextMenu>("PART_Menu");
            InitializeVisualElementsContainer();
        }

        //Get element from name. If it exist then element instance return, if not, new will be created
        T EnforceInstance<T>(string partName) where T : FrameworkElement, new()
        {
            T element = GetTemplateChild(partName) as T ?? new T();
            return element;
        }

        private void InitializeVisualElementsContainer()
        {
            MouseRightButtonUp -= DropDownButton_MouseRightButtonUp;
            clickButton.Click -= ButtonClick;
            MouseRightButtonUp += DropDownButton_MouseRightButtonUp;
            clickButton.Click += ButtonClick;
        }

        void DropDownButton_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
