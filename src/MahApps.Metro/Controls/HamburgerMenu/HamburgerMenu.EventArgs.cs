using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// EventArgs used for the <see cref="HamburgerMenu"/> ItemClick and OptionsItemClick event.
    /// </summary>
    public class ItemClickEventArgs : RoutedEventArgs
    {
        /// <inheritdoc />
        public ItemClickEventArgs()
        {
        }

        /// <inheritdoc />
        public ItemClickEventArgs(RoutedEvent routedEvent)
            : base(routedEvent)
        {
        }

        /// <inheritdoc />
        public ItemClickEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
        }

        /// <summary>
        /// Gets the clicked item
        /// </summary>
        public object ClickedItem { get; internal set; }
    }

    /// <summary>
    /// RoutedEventHandler used for the <see cref="HamburgerMenu"/> ItemClick and OptionsItemClick event.
    /// </summary>
    public delegate void ItemClickRoutedEventHandler(object sender, ItemClickEventArgs args);

    /// <summary>
    /// EventArgs used for the <see cref="HamburgerMenu"/> ItemInvoked event.
    /// </summary>
    public class HamburgerMenuItemInvokedEventArgs : RoutedEventArgs
    {
        /// <inheritdoc />
        public HamburgerMenuItemInvokedEventArgs()
        {
        }

        /// <inheritdoc />
        public HamburgerMenuItemInvokedEventArgs(RoutedEvent routedEvent)
            : base(routedEvent)
        {
        }

        /// <inheritdoc />
        public HamburgerMenuItemInvokedEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
        }

        /// <summary>
        /// Gets the invoked item
        /// </summary>
        public object InvokedItem { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the invoked item is an options item
        /// </summary>
        public bool IsItemOptions { get; internal set; }
    }

    /// <summary>
    /// RoutedEventHandler used for the <see cref="HamburgerMenu"/> ItemInvoked event.
    /// </summary>
    public delegate void HamburgerMenuItemInvokedRoutedEventHandler(object sender, HamburgerMenuItemInvokedEventArgs args);
}