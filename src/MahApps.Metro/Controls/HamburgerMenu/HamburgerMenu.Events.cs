using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The HamburgerMenu is based on a SplitView control. By default it contains a HamburgerButton and a ListView to display menu items.
    /// </summary>
    public partial class HamburgerMenu
    {
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

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            IsPaneOpen = !IsPaneOpen;
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

            ItemClick?.Invoke(this, new ItemClickEventArgs(selectedItem));
            ItemInvoked?.Invoke(this, new HamburgerMenuItemInvokedEventArgs() { InvokedItem = selectedItem, IsItemOptions = false });
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

            OptionsItemClick?.Invoke(this, new ItemClickEventArgs(selectedItem));
            ItemInvoked?.Invoke(this, new HamburgerMenuItemInvokedEventArgs() { InvokedItem = selectedItem, IsItemOptions = true });
        }

        private ListBoxItem GetClickedListBoxItem(ItemsControl itemsControl, DependencyObject dependencyObject)
        {
            if (itemsControl == null || dependencyObject == null)
            {
                return null;
            }
            var item = ItemsControl.ContainerFromElement(itemsControl, dependencyObject) as ListBoxItem;
            return item;
        }

        private void ButtonsListView_ItemClick(object sender, MouseButtonEventArgs e)
        {
            var item = GetClickedListBoxItem(sender as ItemsControl, e.OriginalSource as DependencyObject);
            if (item != null)
            {
                // ListBox item clicked - do some cool things here
                OnItemClick();
            }
        }

        private void OptionsListView_ItemClick(object sender, MouseButtonEventArgs e)
        {
            var item = GetClickedListBoxItem(sender as ItemsControl, e.OriginalSource as DependencyObject);
            if (item != null)
            {
                // ListBox item clicked - do some cool things here
                OnOptionsItemClick();
            }
        }

        private void ButtonsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                if (Keyboard.IsKeyToggled(Key.Space) ||
                    Keyboard.IsKeyToggled(Key.Up) ||
                    Keyboard.IsKeyToggled(Key.PageUp) ||
                    Keyboard.IsKeyToggled(Key.Down) ||
                    Keyboard.IsKeyToggled(Key.PageDown))
                {
                    OnItemClick();
                }
            }
        }

        private void OptionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                if (Keyboard.IsKeyToggled(Key.Space) ||
                    Keyboard.IsKeyToggled(Key.Up) ||
                    Keyboard.IsKeyToggled(Key.PageUp) ||
                    Keyboard.IsKeyToggled(Key.Down) ||
                    Keyboard.IsKeyToggled(Key.PageDown))
                {
                    OnOptionsItemClick();
                }
            }
        }
    }
}