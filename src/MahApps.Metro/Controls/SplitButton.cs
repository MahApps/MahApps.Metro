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
    [TemplatePart(Name = "PART_Container", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ButtonContent", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_Expander", Type = typeof(Button))]
    public class SplitButton : ComboBox
    {
        public static readonly RoutedEvent ClickEvent
            = EventManager.RegisterRoutedEvent(nameof(Click),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(SplitButton));

        public event RoutedEventHandler Click
        {
            add { this.AddHandler(ClickEvent, value); }
            remove { this.RemoveHandler(ClickEvent, value); }
        }

        public static readonly DependencyProperty ExtraTagProperty
            = DependencyProperty.Register(
                nameof(ExtraTag),
                typeof(object),
                typeof(SplitButton));

        /// <summary>
        /// Gets or sets an extra tag.
        /// </summary>
        public object ExtraTag
        {
            get { return this.GetValue(ExtraTagProperty); }
            set { this.SetValue(ExtraTagProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty
            = DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(SplitButton),
                new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the orientation of children stacking.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)this.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(
                nameof(Icon),
                typeof(object),
                typeof(SplitButton));

        /// <summary>
        ///  Gets or sets the Content used to generate the icon part.
        /// </summary>
        [Bindable(true)]
        public object Icon
        {
            get { return this.GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconTemplateProperty
            = DependencyProperty.Register(
                nameof(IconTemplate),
                typeof(DataTemplate),
                typeof(SplitButton));

        /// <summary> 
        /// Gets or sets the ContentTemplate used to display the content of the icon part. 
        /// </summary>
        [Bindable(true)]
        public DataTemplate IconTemplate
        {
            get { return (DataTemplate)this.GetValue(IconTemplateProperty); }
            set { this.SetValue(IconTemplateProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty
            = DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(SplitButton));

        /// <summary>
        /// Get or sets the Command property. 
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandTargetProperty
            = DependencyProperty.Register(
                nameof(CommandTarget),
                typeof(IInputElement),
                typeof(SplitButton));

        /// <summary>
        /// Gets or sets the target element on which to fire the command.
        /// </summary>
        public IInputElement CommandTarget
        {
            get { return (IInputElement)this.GetValue(CommandTargetProperty); }
            set { this.SetValue(CommandTargetProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty
            = DependencyProperty.Register(
                nameof(CommandParameter),
                typeof(object),
                typeof(SplitButton));

        /// <summary>
        /// Reflects the parameter to pass to the CommandProperty upon execution. 
        /// </summary>
        public object CommandParameter
        {
            get { return this.GetValue(CommandParameterProperty); }
            set { this.SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty ButtonStyleProperty
            = DependencyProperty.Register(
                nameof(ButtonStyle),
                typeof(Style),
                typeof(SplitButton),
                new FrameworkPropertyMetadata(default(Style), FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets/sets the button style.
        /// </summary>
        public Style ButtonStyle
        {
            get { return (Style)this.GetValue(ButtonStyleProperty); }
            set { this.SetValue(ButtonStyleProperty, value); }
        }

        public static readonly DependencyProperty ButtonArrowStyleProperty
            = DependencyProperty.Register(
                nameof(ButtonArrowStyle),
                typeof(Style),
                typeof(SplitButton),
                new FrameworkPropertyMetadata(default(Style), FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets/sets the button arrow style.
        /// </summary>
        public Style ButtonArrowStyle
        {
            get { return (Style)this.GetValue(ButtonArrowStyleProperty); }
            set { this.SetValue(ButtonArrowStyleProperty, value); }
        }

        public static readonly DependencyProperty ArrowBrushProperty
            = DependencyProperty.Register(
                nameof(ArrowBrush),
                typeof(Brush),
                typeof(SplitButton),
                new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets/sets the brush of the button arrow icon.
        /// </summary>
        public Brush ArrowBrush
        {
            get { return (Brush)this.GetValue(ArrowBrushProperty); }
            set { this.SetValue(ArrowBrushProperty, value); }
        }

        public static readonly DependencyProperty ArrowMouseOverBrushProperty
            = DependencyProperty.Register(
                nameof(ArrowMouseOverBrush),
                typeof(Brush),
                typeof(SplitButton),
                new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets/sets the brush of the button arrow icon if the mouse is over the split button.
        /// </summary>
        public Brush ArrowMouseOverBrush
        {
            get { return (Brush)this.GetValue(ArrowMouseOverBrushProperty); }
            set { this.SetValue(ArrowMouseOverBrushProperty, value); }
        }

        public static readonly DependencyProperty ArrowPressedBrushProperty
            = DependencyProperty.Register(
                nameof(ArrowPressedBrush),
                typeof(Brush),
                typeof(SplitButton),
                new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets/sets the brush of the button arrow icon if the arrow button is pressed.
        /// </summary>
        public Brush ArrowPressedBrush
        {
            get { return (Brush)this.GetValue(ArrowPressedBrushProperty); }
            set { this.SetValue(ArrowPressedBrushProperty, value); }
        }

        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));

            IsEditableProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(false, null, new CoerceValueCallback(CoerceIsEnabledProperty)));
        }

        private static object CoerceIsEnabledProperty(DependencyObject dependencyObject, object value)
        {
            // For now SplitButton is not editable
            return false;
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            e.RoutedEvent = ClickEvent;
            this.RaiseEvent(e);
            this.SetCurrentValue(IsDropDownOpenProperty, false);
        }

        private void ExpanderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SetCurrentValue(IsDropDownOpenProperty, !this.IsDropDownOpen);
            e.Handled = true;
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

            if (this._expander != null)
            {
                this._expander.PreviewMouseLeftButtonDown -= this.ExpanderMouseLeftButtonDown;
            }

            this._expander = this.GetTemplateChild("PART_Expander") as Button;
            if (this._expander != null)
            {
                this._expander.PreviewMouseLeftButtonDown += this.ExpanderMouseLeftButtonDown;
            }
        }

        private void ButtonIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.SetCurrentValue(IsEnabledProperty, e.NewValue);
        }

        private Button _clickButton;
        private Button _expander;
    }
}