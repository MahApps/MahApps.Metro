using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The HamburgerMenu is based on a SplitView control. By default it contains a HamburgerButton and a ListView to display menu items.
    /// </summary>
    public partial class HamburgerMenu
    {
        private ControlTemplate _defaultItemFocusVisualTemplate = null;

        /// <summary>
        /// Identifies the <see cref="OpenPaneLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OpenPaneLengthProperty = DependencyProperty.Register(nameof(OpenPaneLength), typeof(double), typeof(HamburgerMenu), new PropertyMetadata(240.0, OpenPaneLengthPropertyChangedCallback));

        private static void OpenPaneLengthPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != args.OldValue)
            {
                (dependencyObject as HamburgerMenu)?.ChangeItemFocusVisualStyle();
            }
        }

        /// <summary>
        /// Identifies the <see cref="PanePlacement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanePlacementProperty = DependencyProperty.Register(nameof(PanePlacement), typeof(SplitViewPanePlacement), typeof(HamburgerMenu), new PropertyMetadata(SplitViewPanePlacement.Left));

        /// <summary>
        /// Identifies the <see cref="DisplayMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register(nameof(DisplayMode), typeof(SplitViewDisplayMode), typeof(HamburgerMenu), new PropertyMetadata(SplitViewDisplayMode.CompactInline));

        /// <summary>
        /// Identifies the <see cref="CompactPaneLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CompactPaneLengthProperty = DependencyProperty.Register(nameof(CompactPaneLength), typeof(double), typeof(HamburgerMenu), new PropertyMetadata(48.0, CompactPaneLengthPropertyChangedCallback));

        private static void CompactPaneLengthPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != args.OldValue)
            {
                (dependencyObject as HamburgerMenu)?.ChangeItemFocusVisualStyle();
            }
        }

        /// <summary>
        /// Identifies the <see cref="PaneMargin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaneMarginProperty = DependencyProperty.Register(nameof(PaneMargin), typeof(Thickness), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="PaneHeaderMargin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaneHeaderMarginProperty = DependencyProperty.Register(nameof(PaneHeaderMargin), typeof(Thickness), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="PaneBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaneBackgroundProperty = DependencyProperty.Register(nameof(PaneBackground), typeof(Brush), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="PaneForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaneForegroundProperty = DependencyProperty.Register(nameof(PaneForeground), typeof(Brush), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IsPaneOpen"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPaneOpenProperty = DependencyProperty.Register(nameof(IsPaneOpen), typeof(bool), typeof(HamburgerMenu), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsPaneOpenPropertyChangedCallback));

        private static void IsPaneOpenPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != args.OldValue)
            {
                (dependencyObject as HamburgerMenu)?.ChangeItemFocusVisualStyle();
            }
        }

        /// <summary>
        /// Identifies the <see cref="ItemsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemContainerStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register(nameof(ItemContainerStyle), typeof(Style), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.Register(nameof(ItemTemplateSelector), typeof(DataTemplateSelector), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SelectedItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(HamburgerMenu), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Identifies the <see cref="SelectedIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(HamburgerMenu), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

        /// <summary>
        /// Identifies the <see cref="ContentTransition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTransitionProperty = DependencyProperty.Register(nameof(ContentTransition), typeof(TransitionType), typeof(HamburgerMenu), new FrameworkPropertyMetadata(TransitionType.Normal));

        /// <summary>
        /// Identifies the <see cref="ItemCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemCommandProperty = DependencyProperty.Register(nameof(ItemCommand), typeof(ICommand), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemCommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemCommandParameterProperty = DependencyProperty.Register(nameof(ItemCommandParameter), typeof(object), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="VerticalScrollBarOnLeftSide"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarOnLeftSideProperty = DependencyProperty.Register(nameof(VerticalScrollBarOnLeftSide), typeof(bool), typeof(HamburgerMenu), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="ShowSelectionIndicator"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowSelectionIndicatorProperty = DependencyProperty.Register(nameof(ShowSelectionIndicator), typeof(bool), typeof(HamburgerMenu), new PropertyMetadata(false));

        public static readonly DependencyPropertyKey ItemFocusVisualStylePropertyKey = DependencyProperty.RegisterReadOnly(nameof(ItemFocusVisualStyle), typeof(Style), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemFocusVisualStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemFocusVisualStyleProperty = ItemFocusVisualStylePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets the width of the pane when it's fully expanded.
        /// </summary>
        public double OpenPaneLength
        {
            get { return (double)GetValue(OpenPaneLengthProperty); }
            set { SetValue(OpenPaneLengthProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that specifies whether the pane is shown on the right or left side of the control.
        /// </summary>
        public SplitViewPanePlacement PanePlacement
        {
            get { return (SplitViewPanePlacement)GetValue(PanePlacementProperty); }
            set { SetValue(PanePlacementProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that specifies how the pane and content areas are shown.
        /// </summary>
        public SplitViewDisplayMode DisplayMode
        {
            get { return (SplitViewDisplayMode)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the pane in its compact display mode.
        /// </summary>
        public double CompactPaneLength
        {
            get { return (double)GetValue(CompactPaneLengthProperty); }
            set { SetValue(CompactPaneLengthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the margin for the pane.
        /// </summary>
        public Thickness PaneMargin
        {
            get { return (Thickness)GetValue(PaneMarginProperty); }
            set { SetValue(PaneMarginProperty, value); }
        }

        /// <summary>
        /// Gets or sets the margin for the pane header.
        /// </summary>
        public Thickness PaneHeaderMargin
        {
            get { return (Thickness)GetValue(PaneHeaderMarginProperty); }
            set { SetValue(PaneHeaderMarginProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Brush to apply to the background of the Pane area of the control.
        /// </summary>
        public Brush PaneBackground
        {
            get { return (Brush)GetValue(PaneBackgroundProperty); }
            set { SetValue(PaneBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Brush to apply to the foreground of the Pane area of the control.
        /// </summary>
        public Brush PaneForeground
        {
            get { return (Brush)GetValue(PaneForegroundProperty); }
            set { SetValue(PaneForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pane is expanded to its full width.
        /// </summary>
        public bool IsPaneOpen
        {
            get { return (bool)GetValue(IsPaneOpenProperty); }
            set { SetValue(IsPaneOpenProperty, value); }
        }

        /// <summary>
        /// Gets or sets an object source used to generate the content of the menu.
        /// </summary>
        public object ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Style used for each item.
        /// </summary>
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the DataTemplate used to display each item.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the DataTemplateSelector used to display each item.
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Gets the collection used to generate the content of the items list.
        /// </summary>
        /// <exception cref="Exception">
        /// Exception thrown if ButtonsListView is not yet defined.
        /// </exception>
        public ItemCollection Items
        {
            get
            {
                if (_buttonsListView == null)
                {
                    throw new Exception("ButtonsListView is not defined yet. Please use ItemsSource instead.");
                }

                return _buttonsListView.Items;
            }
        }

        /// <summary>
        /// Gets or sets the selected menu item.
        /// </summary>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selected menu index.
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public TransitionType ContentTransition
        {
            get { return (TransitionType)GetValue(ContentTransitionProperty); }
            set { SetValue(ContentTransitionProperty, value); }
        }

        /// <summary>
        /// Gets or sets a command which will be executed if an item is clicked by the user.
        /// </summary>
        public ICommand ItemCommand
        {
            get { return (ICommand)GetValue(ItemCommandProperty); }
            set { SetValue(ItemCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command parameter which will be passed by the ItemCommand.
        /// </summary>
        public object ItemCommandParameter
        {
            get { return (object)GetValue(ItemCommandParameterProperty); }
            set { SetValue(ItemCommandParameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets wheather the ScrollBar of the HamburgerMenu is on the left side or on the right side.
        /// </summary>
        public bool VerticalScrollBarOnLeftSide
        {
            get { return (bool)GetValue(VerticalScrollBarOnLeftSideProperty); }
            set { SetValue(VerticalScrollBarOnLeftSideProperty, value); }
        }

        /// <summary>
        /// Gets or sets wheather a selection indicator will be shown on the HamburgerMenuItem.
        /// </summary>
        public bool ShowSelectionIndicator
        {
            get { return (bool)GetValue(ShowSelectionIndicatorProperty); }
            set { SetValue(ShowSelectionIndicatorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the default FocusVisualStyle for a HamburgerMenuItem.
        /// This style can be override at the HamburgerMenuItem style by setting the FocusVisualStyle property.
        /// </summary>
        public Style ItemFocusVisualStyle
        {
            get { return (Style)GetValue(ItemFocusVisualStyleProperty); }
            private set { SetValue(ItemFocusVisualStylePropertyKey, value); }
        }

        /// <summary>
        /// Executes the item command which can be set by the user.
        /// </summary>
        public void RaiseItemCommand()
        {
            var command = ItemCommand;
            var commandParameter = ItemCommandParameter ?? this;
            if (command != null && command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }
        }

        private void ChangeItemFocusVisualStyle()
        {
            _defaultItemFocusVisualTemplate = _defaultItemFocusVisualTemplate ?? TryFindResource("HamburgerMenuItemFocusVisualTemplate") as ControlTemplate;
            if (_defaultItemFocusVisualTemplate != null)
            {
                var focusVisualStyle = new Style(typeof(Control));
                focusVisualStyle.Setters.Add(new Setter(Control.TemplateProperty, _defaultItemFocusVisualTemplate));
                focusVisualStyle.Setters.Add(new Setter(Control.WidthProperty, IsPaneOpen ? OpenPaneLength : CompactPaneLength));
                focusVisualStyle.Setters.Add(new Setter(Control.HorizontalAlignmentProperty, HorizontalAlignment.Left));
                focusVisualStyle.Seal();

                SetValue(ItemFocusVisualStylePropertyKey, focusVisualStyle);
            }
        }
    }
}
