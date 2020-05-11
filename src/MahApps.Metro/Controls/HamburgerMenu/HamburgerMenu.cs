using System.Windows;
using System.Windows.Controls;
using ControlzEx;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The HamburgerMenu is based on a SplitView control. By default it contains a HamburgerButton and a ListView to display menu items.
    /// </summary>
    [TemplatePart(Name = "HamburgerButton", Type = typeof(Button))]
    [TemplatePart(Name = "ButtonsListView", Type = typeof(ListBox))]
    [TemplatePart(Name = "OptionsListView", Type = typeof(ListBox))]
    public partial class HamburgerMenu : ContentControl
    {
        private Button _hamburgerButton;
        private ListBox _buttonsListView;
        private ListBox _optionsListView;
        private readonly PropertyChangeNotifier actualWidthPropertyChangeNotifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="HamburgerMenu"/> class.
        /// </summary>
        public HamburgerMenu()
        {
            DefaultStyleKey = typeof(HamburgerMenu);

            actualWidthPropertyChangeNotifier = new PropertyChangeNotifier(this, ActualWidthProperty);
            actualWidthPropertyChangeNotifier.ValueChanged += (s, e) => CoerceValue(OpenPaneLengthProperty);
        }

        /// <summary>
        /// Override default OnApplyTemplate to capture children controls
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (_hamburgerButton != null)
            {
                _hamburgerButton.Click -= OnHamburgerButtonClick;
            }

            if (_buttonsListView != null)
            {
                _buttonsListView.SelectionChanged -= ButtonsListView_SelectionChanged;
            }

            if (_optionsListView != null)
            {
                _optionsListView.SelectionChanged -= OptionsListView_SelectionChanged;
            }

            _hamburgerButton = (Button)GetTemplateChild("HamburgerButton");
            _buttonsListView = (ListBox)GetTemplateChild("ButtonsListView");
            _optionsListView = (ListBox)GetTemplateChild("OptionsListView");

            if (_hamburgerButton != null)
            {
                _hamburgerButton.Click += OnHamburgerButtonClick;
            }

            if (_buttonsListView != null)
            {
                _buttonsListView.SelectionChanged += ButtonsListView_SelectionChanged;
            }

            if (_optionsListView != null)
            {
                _optionsListView.SelectionChanged += OptionsListView_SelectionChanged;
            }

            ChangeItemFocusVisualStyle();

            Loaded -= HamburgerMenu_Loaded;
            Loaded += HamburgerMenu_Loaded;

            base.OnApplyTemplate();
        }

        private void HamburgerMenu_Loaded(object sender, RoutedEventArgs e)
        {
            if (GetValue(ContentProperty) != null)
            {
                return;
            }

            var item = _buttonsListView?.SelectedItem;
            var canRaiseItemEvents = CanRaiseItemEvents(item);
            if (canRaiseItemEvents && RaiseItemEvents(item))
            {
                return;
            }

            var optionItem = _optionsListView?.SelectedItem;
            var canRaiseOptionsItemEvents = CanRaiseOptionsItemEvents(optionItem);
            if (canRaiseOptionsItemEvents && RaiseOptionsItemEvents(optionItem))
            {
                return;
            }

            if (canRaiseItemEvents || canRaiseOptionsItemEvents)
            {
                var selectedItem = item ?? optionItem;
                if (selectedItem != null)
                {
                    SetCurrentValue(ContentProperty, selectedItem);
                }
            }
        }
    }
}