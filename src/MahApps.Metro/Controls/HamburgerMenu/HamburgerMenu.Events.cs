using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The HamburgerMenu is based on a SplitView control. By default it contains a HamburgerButton and a ListView to display menu items.
    /// </summary>
    public partial class HamburgerMenu
    {
        public static readonly RoutedEvent ItemClickEvent = EventManager.RegisterRoutedEvent("ItemClick", RoutingStrategy.Direct, typeof(ItemClickRoutedEventHandler), typeof(HamburgerMenu));

        /// <summary>
        /// Event raised when an item is clicked
        /// </summary>
        public event ItemClickRoutedEventHandler ItemClick
        {
            add { this.AddHandler(HamburgerMenu.ItemClickEvent, value); }
            remove { this.RemoveHandler(HamburgerMenu.ItemClickEvent, value); }
        }

        public static readonly RoutedEvent OptionsItemClickEvent = EventManager.RegisterRoutedEvent("OptionsItemClick", RoutingStrategy.Direct, typeof(ItemClickRoutedEventHandler), typeof(HamburgerMenu));

        /// <summary>
        /// Event raised when an options' item is clicked
        /// </summary>
        public event ItemClickRoutedEventHandler OptionsItemClick
        {
            add { this.AddHandler(HamburgerMenu.OptionsItemClickEvent, value); }
            remove { this.RemoveHandler(HamburgerMenu.OptionsItemClickEvent, value); }
        }

        public static readonly RoutedEvent ItemInvokedEvent = EventManager.RegisterRoutedEvent("ItemInvoked", RoutingStrategy.Direct, typeof(HamburgerMenuItemInvokedRoutedEventHandler), typeof(HamburgerMenu));

        /// <summary>
        /// Event raised when an item is invoked
        /// </summary>
        public event HamburgerMenuItemInvokedRoutedEventHandler ItemInvoked
        {
            add { this.AddHandler(HamburgerMenu.ItemInvokedEvent, value); }
            remove { this.RemoveHandler(HamburgerMenu.ItemInvokedEvent, value); }
        }

        public static readonly RoutedEvent HamburgerButtonClickEvent = EventManager.RegisterRoutedEvent("HamburgerButtonClick", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(HamburgerMenu));

        /// <summary>
        /// Event raised when the hamburger button is clicked
        /// </summary>
        public event RoutedEventHandler HamburgerButtonClick
        {
            add { this.AddHandler(HamburgerMenu.HamburgerButtonClickEvent, value); }
            remove { this.RemoveHandler(HamburgerMenu.HamburgerButtonClickEvent, value); }
        }

        private void OnHamburgerButtonClick(object sender, RoutedEventArgs e)
        {
            var args = new RoutedEventArgs(HamburgerMenu.HamburgerButtonClickEvent, sender);
            this.RaiseEvent(args);

            if (!args.Handled)
            {
                IsPaneOpen = !IsPaneOpen;
            }
        }

        private bool OnItemClick()
        {
            var selectedItem = _buttonsListView.SelectedItem;

            if (!CanRaiseItemEvents(selectedItem))
            {
                return false;
            }

            (selectedItem as HamburgerMenuItem)?.RaiseCommand();
            RaiseItemCommand();

            var raiseItemEvents = this.RaiseItemEvents(selectedItem);
            if (raiseItemEvents && _optionsListView != null)
            {
                _optionsListView.SelectedIndex = -1;
            }

            return raiseItemEvents;
        }

        private bool CanRaiseItemEvents(object selectedItem)
        {
            if (selectedItem is null)
            {
                return false;
            }

            if (selectedItem is IHamburgerMenuHeaderItem || selectedItem is IHamburgerMenuSeparatorItem)
            {
                if (this._buttonsListView != null)
                {
                    this._buttonsListView.SelectedIndex = -1;
                }

                return false;
            }

            return true;
        }

        private bool RaiseItemEvents(object selectedItem)
        {
            if (selectedItem is null)
            {
                return false;
            }

            var itemClickEventArgs = new ItemClickEventArgs(ItemClickEvent, this) { ClickedItem = selectedItem };
            this.RaiseEvent(itemClickEventArgs);

            var hamburgerMenuItemInvokedEventArgs = new HamburgerMenuItemInvokedEventArgs(ItemInvokedEvent, this) { InvokedItem = selectedItem, IsItemOptions = false };
            this.RaiseEvent(hamburgerMenuItemInvokedEventArgs);

            return !itemClickEventArgs.Handled && !hamburgerMenuItemInvokedEventArgs.Handled;
        }

        private bool OnOptionsItemClick()
        {
            var selectedItem = _optionsListView.SelectedItem;

            if (!CanRaiseOptionsItemEvents(selectedItem))
            {
                return false;
            }

            (selectedItem as HamburgerMenuItem)?.RaiseCommand();
            RaiseOptionsItemCommand();

            var raiseOptionsItemEvents = this.RaiseOptionsItemEvents(selectedItem);
            if (raiseOptionsItemEvents && _buttonsListView != null)
            {
                _buttonsListView.SelectedIndex = -1;
            }

            return raiseOptionsItemEvents;
        }

        private bool CanRaiseOptionsItemEvents(object selectedItem)
        {
            if (selectedItem is null)
            {
                return false;
            }

            if (selectedItem is IHamburgerMenuHeaderItem || selectedItem is IHamburgerMenuSeparatorItem)
            {
                if (this._optionsListView != null)
                {
                    this._optionsListView.SelectedIndex = -1;
                }

                return false;
            }

            return true;
        }

        private bool RaiseOptionsItemEvents(object selectedItem)
        {
            if (selectedItem is null)
            {
                return false;
            }

            if (selectedItem is IHamburgerMenuHeaderItem || selectedItem is IHamburgerMenuSeparatorItem)
            {
                return false;
            }

            var itemClickEventArgs = new ItemClickEventArgs(OptionsItemClickEvent, this) { ClickedItem = selectedItem };
            this.RaiseEvent(itemClickEventArgs);

            var hamburgerMenuItemInvokedEventArgs = new HamburgerMenuItemInvokedEventArgs(ItemInvokedEvent, this) { InvokedItem = selectedItem, IsItemOptions = true };
            this.RaiseEvent(hamburgerMenuItemInvokedEventArgs);

            return !itemClickEventArgs.Handled && !hamburgerMenuItemInvokedEventArgs.Handled;
        }

        private void ButtonsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null)
            {
                return;
            }

            listBox.SelectionChanged -= this.ButtonsListView_SelectionChanged;

            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                var canItemClick = OnItemClick();

                if (!canItemClick)
                {
                    // The following lines will fire another SelectionChanged event.
                    if (e.RemovedItems.Count > 0)
                    {
                        listBox.SelectedItem = e.RemovedItems[0];
                    }
                    else
                    {
                        listBox.SelectedIndex = -1;
                    }
                }
            }

            listBox.SelectionChanged += this.ButtonsListView_SelectionChanged;
        }

        private void OptionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null)
            {
                return;
            }

            listBox.SelectionChanged -= this.OptionsListView_SelectionChanged;

            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                var canItemClick = OnOptionsItemClick();

                if (!canItemClick)
                {
                    // The following lines will fire another SelectionChanged event.
                    if (e.RemovedItems.Count > 0)
                    {
                        listBox.SelectedItem = e.RemovedItems[0];
                    }
                    else
                    {
                        listBox.SelectedIndex = -1;
                    }
                }
            }

            listBox.SelectionChanged += this.OptionsListView_SelectionChanged;
        }
    }
}