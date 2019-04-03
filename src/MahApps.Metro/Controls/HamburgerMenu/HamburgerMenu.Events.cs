using System;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The HamburgerMenu is based on a SplitView control. By default it contains a HamburgerButton and a ListView to display menu items.
    /// </summary>
    public partial class HamburgerMenu
    {
        public static readonly RoutedEvent HamburgerButtonClickEvent = EventManager.RegisterRoutedEvent("HamburgerButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HamburgerMenu));

        /// <summary>
        /// Event raised when an item is clicked
        /// </summary>
        public event ItemClickEventHandler ItemClick;

        /// <summary>
        /// Event raised when an options' item is clicked
        /// </summary>
        public event ItemClickEventHandler OptionsItemClick;

        /// <summary>
        /// Event raised when an item is invoked
        /// </summary>
        public event EventHandler<HamburgerMenuItemInvokedEventArgs> ItemInvoked;

        /// <summary>
        /// Event raised when the hamburger button is clicked
        /// </summary>
        public event RoutedEventHandler HamburgerButtonClick
        {
            add
            {
                this.AddHandler(HamburgerMenu.HamburgerButtonClickEvent, value);
            }
            remove
            {
                this.RemoveHandler(HamburgerMenu.HamburgerButtonClickEvent, value);
            }
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

        private void OnItemClick()
        {
            if (_optionsListView != null)
            {
                _optionsListView.SelectedIndex = -1;
            }

            var selectedItem = _buttonsListView.SelectedItem;

            (selectedItem as HamburgerMenuItem)?.RaiseCommand();
            RaiseItemCommand();

            RaiseItemEvents(selectedItem);
        }

        private bool RaiseItemEvents(object selectedItem)
        {
            if (selectedItem is null || (ItemClick is null && ItemInvoked is null))
            {
                return false;
            }

            ItemClick?.Invoke(this, new ItemClickEventArgs(selectedItem));
            ItemInvoked?.Invoke(this, new HamburgerMenuItemInvokedEventArgs() { InvokedItem = selectedItem, IsItemOptions = false });

            return true;
        }

        private void OnOptionsItemClick()
        {
            if (_buttonsListView != null)
            {
                _buttonsListView.SelectedIndex = -1;
            }

            var selectedItem = _optionsListView.SelectedItem;

            (selectedItem as HamburgerMenuItem)?.RaiseCommand();
            RaiseOptionsItemCommand();

            RaiseOptionsItemEvents(selectedItem);
        }

        private bool RaiseOptionsItemEvents(object selectedItem)
        {
            if (selectedItem is null || (OptionsItemClick is null && ItemInvoked is null))
            {
                return false;
            }

            OptionsItemClick?.Invoke(this, new ItemClickEventArgs(selectedItem));
            ItemInvoked?.Invoke(this, new HamburgerMenuItemInvokedEventArgs() { InvokedItem = selectedItem, IsItemOptions = true });

            return true;
        }

        private void ButtonsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                OnItemClick();
            }
        }

        private void OptionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                OnOptionsItemClick();
            }
        }
    }
}