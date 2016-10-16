namespace MahApps.Metro.Controls
{
    using System.Windows;
    using System.Windows.Input;

    public sealed class ItemClickEventArgs : RoutedEventArgs
    {
        public ItemClickEventArgs(object clickedObject)
        {
            this.ClickedItem = clickedObject;
        }

        public object ClickedItem { get; private set; }
    }

    public delegate void ItemClickEventHandler(object sender, ItemClickEventArgs e);

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

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            IsPaneOpen = !IsPaneOpen;
        }

        private void ButtonsListView_ItemClick(object sender, MouseButtonEventArgs e)
        {
            if (_optionsListView != null)
            {
                _optionsListView.SelectedIndex = -1;
            }

            ItemClick?.Invoke(this, new ItemClickEventArgs(_buttonsListView.SelectedItem));
        }

        private void OptionsListView_ItemClick(object sender, MouseButtonEventArgs e)
        {
            if (_buttonsListView != null)
            {
                _buttonsListView.SelectedIndex = -1;
            }

            OptionsItemClick?.Invoke(this, new ItemClickEventArgs(_optionsListView.SelectedItem));
        }
    }
}
