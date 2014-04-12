using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace MahApps.Metro.Controls
{
    /// <summary>
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    [ContentProperty("ItemsSource")]
    [DefaultEvent("SelectionChanged"),
    TemplatePart(Name = "PART_Container", Type = typeof(Grid)),
    TemplatePart(Name = "PART_Button", Type = typeof(Button)),
    TemplatePart(Name = "PART_ButtonContent", Type = typeof(ContentControl)),
    TemplatePart(Name = "PART_Popup", Type = typeof(Popup)),
    TemplatePart(Name = "PART_Expander", Type = typeof(Button)),
    TemplatePart(Name = "PART_ListBox", Type = typeof(ListBox))]
    public class SplitButton : ItemsControl
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
            DependencyProperty.Register("Icon", typeof(Object), typeof(SplitButton));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(SplitButton));

        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(SplitButton));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(Object), typeof(SplitButton));


        public Object CommandParameter
        {
            get { return (Object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
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

        public Object Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        #endregion


        #region Variables

        private Button _clickButton;
        private Button _expander;
        private ListBox _listBox;
        private Popup _popup;


        #endregion


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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _clickButton = EnforceInstance<Button>("PART_Button");
            _expander = EnforceInstance<Button>("PART_Expander");
            _listBox = EnforceInstance<ListBox>("PART_ListBox");
            _popup = EnforceInstance<Popup>("PART_Popup");
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
            _listBox.PreviewMouseLeftButtonDown += ListBoxPreviewMouseLeftButtonDown;
            _popup.Opened += PopupOpened;
            _popup.Closed += _popup_Closed;
        }

        //Make popup close even if no selectionchanged event fired (case when user select the save item as before)
        void ListBoxPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = ContainerFromElement(_listBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {
                IsExpanded = false;
            }
        }

        void _popup_Closed(object sender, EventArgs e)
        {
            this.ReleaseMouseCapture();
        }

        void PopupOpened(object sender, EventArgs e)
        {
            Mouse.Capture(this, CaptureMode.SubTree);
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OutsideCapturedElementHandler);
        }

        private void OutsideCapturedElementHandler(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            IsExpanded = false;
            Mouse.RemovePreviewMouseDownOutsideCapturedElementHandler(this, OutsideCapturedElementHandler);
        }

    }

}
