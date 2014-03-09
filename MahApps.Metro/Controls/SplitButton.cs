using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    [ContentProperty("ItemsSource")]
    [DefaultEvent("SelectionChanged"),
    TemplatePart(Name = "PART_Container", Type = typeof(StackPanel)),
    TemplatePart(Name = "PART_Button", Type = typeof(Button)),
    TemplatePart(Name = "PART_Image", Type = typeof(Image)),
    TemplatePart(Name = "PART_ButtonContent", Type = typeof(ContentControl)),
    TemplatePart(Name = "PART_Popup", Type = typeof(Popup)),
    TemplatePart(Name = "PART_Expander", Type = typeof(Button)),
    TemplatePart(Name = "PART_ListBox", Type = typeof(ListBox))]
    public class SplitButton : Control
    {

        #region Events

        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(SplitButton));

        public static readonly RoutedEvent SelectionChangedEvent =
            EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble,
                typeof(SelectionChangedEventHandler), typeof(SplitButton));

        public event SelectionChangedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        #endregion


        #region DependencyProperties

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SplitButton));

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(SplitButton),
                new UIPropertyMetadata(null));

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(SplitButton));

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(Int32), typeof(SplitButton), new FrameworkPropertyMetadata(0));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(Object), typeof(SplitButton));

        public static readonly DependencyProperty ExtraTagProperty =
            DependencyProperty.Register("ExtraTag", typeof(Object), typeof(SplitButton));

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SplitButton),
                new FrameworkPropertyMetadata(Orientation.Horizontal));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(SplitButton));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public Int32 SelectedIndex
        {
            get { return (Int32)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public Object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public Boolean IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public Object ExtraTag
        {
            get { return GetValue(ExtraTagProperty); }
            set { SetValue(ExtraTagProperty, value); }
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        #endregion


        #region Variables

        private Button _clickButton;
        private Button _expander;
        private ListBox _listBox;
        private bool _mouseLeaved;


        #endregion


        public SplitButton()
        {
        }
        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            e.RoutedEvent = ClickEvent;
            RaiseEvent(e);
        }

        private void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.RoutedEvent = SelectionChangedEvent;
            RaiseEvent(e);

            IsExpanded = false;
        }

        private void ExpanderClick(object sender, RoutedEventArgs e)
        {
            IsExpanded = !IsExpanded;
        }

        void SplitButton_LostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_mouseLeaved)
            {
                IsExpanded = false;
                _mouseLeaved = false;
            }
        }

        private void SplitButtonSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RecalculateDimensions();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _clickButton = EnforceInstance<Button>("PART_Button");
            _expander = EnforceInstance<Button>("PART_Expander");
            _listBox = EnforceInstance<ListBox>("PART_ListBox");
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
            _expander.Click += ExpanderClick;
            _clickButton.Click += ButtonClick;
            _listBox.SelectionChanged += ListBoxSelectionChanged;
            SizeChanged += SplitButtonSizeChanged;
            LostMouseCapture += SplitButton_LostMouseCapture;
            MouseLeave += SplitButton_MouseLeave;
        }

        void SplitButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _mouseLeaved = true;
        }

        private void RecalculateDimensions()
        {
            if (Orientation == Orientation.Horizontal && !Double.IsNaN(Width))
            {
                _clickButton.Width = Math.Max(ActualWidth - _expander.ActualWidth, 0);
            }
            else if (Orientation == Orientation.Vertical && !Double.IsNaN(Height))
            {
                _clickButton.Height = Math.Max(ActualHeight - _expander.ActualHeight, 0);
            }
        }

    }

}
