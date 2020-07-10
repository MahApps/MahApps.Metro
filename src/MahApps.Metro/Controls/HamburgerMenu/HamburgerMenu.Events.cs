// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public partial class HamburgerMenu
    {
        /// <summary>Identifies the <see cref="ItemClick"/> routed event.</summary>
        public static readonly RoutedEvent ItemClickEvent
            = EventManager.RegisterRoutedEvent(nameof(ItemClick),
                                               RoutingStrategy.Direct,
                                               typeof(ItemClickRoutedEventHandler),
                                               typeof(HamburgerMenu));

        /// <summary>
        /// Event raised when an item is clicked
        /// </summary>
        public event ItemClickRoutedEventHandler ItemClick
        {
            add => this.AddHandler(ItemClickEvent, value);
            remove => this.RemoveHandler(ItemClickEvent, value);
        }

        /// <summary>Identifies the <see cref="OptionsItemClick"/> routed event.</summary>
        public static readonly RoutedEvent OptionsItemClickEvent
            = EventManager.RegisterRoutedEvent(nameof(OptionsItemClick),
                                               RoutingStrategy.Direct,
                                               typeof(ItemClickRoutedEventHandler),
                                               typeof(HamburgerMenu));

        /// <summary>
        /// Event raised when an options' item is clicked
        /// </summary>
        public event ItemClickRoutedEventHandler OptionsItemClick
        {
            add => this.AddHandler(OptionsItemClickEvent, value);
            remove => this.RemoveHandler(OptionsItemClickEvent, value);
        }

        /// <summary>Identifies the <see cref="ItemInvoked"/> routed event.</summary>
        public static readonly RoutedEvent ItemInvokedEvent
            = EventManager.RegisterRoutedEvent(nameof(ItemInvoked),
                                               RoutingStrategy.Direct,
                                               typeof(HamburgerMenuItemInvokedRoutedEventHandler),
                                               typeof(HamburgerMenu));

        /// <summary>
        /// Event raised when an item is invoked
        /// </summary>
        public event HamburgerMenuItemInvokedRoutedEventHandler ItemInvoked
        {
            add => this.AddHandler(ItemInvokedEvent, value);
            remove => this.RemoveHandler(ItemInvokedEvent, value);
        }

        /// <summary>Identifies the <see cref="HamburgerButtonClick"/> routed event.</summary>
        public static readonly RoutedEvent HamburgerButtonClickEvent
            = EventManager.RegisterRoutedEvent(nameof(HamburgerButtonClick),
                                               RoutingStrategy.Direct,
                                               typeof(RoutedEventHandler),
                                               typeof(HamburgerMenu));

        /// <summary>
        /// Event raised when the hamburger button is clicked
        /// </summary>
        public event RoutedEventHandler HamburgerButtonClick
        {
            add => this.AddHandler(HamburgerButtonClickEvent, value);
            remove => this.RemoveHandler(HamburgerButtonClickEvent, value);
        }

        private void OnHamburgerButtonClick(object sender, RoutedEventArgs e)
        {
            var args = new RoutedEventArgs(HamburgerButtonClickEvent, sender);
            this.RaiseEvent(args);

            if (!args.Handled)
            {
                this.IsPaneOpen = !this.IsPaneOpen;
            }
        }

        private bool OnItemClick()
        {
            var selectedItem = this.buttonsListView.SelectedItem;

            if (!this.CanRaiseItemEvents(selectedItem))
            {
                return false;
            }

            (selectedItem as HamburgerMenuItem)?.RaiseCommand();
            this.RaiseItemCommand();

            var raiseItemEvents = this.RaiseItemEvents(selectedItem);
            if (raiseItemEvents && this.optionsListView != null)
            {
                this.optionsListView.SelectedIndex = -1;
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
                if (this.buttonsListView != null)
                {
                    this.buttonsListView.SelectedIndex = -1;
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
            var selectedItem = this.optionsListView.SelectedItem;

            if (!this.CanRaiseOptionsItemEvents(selectedItem))
            {
                return false;
            }

            (selectedItem as HamburgerMenuItem)?.RaiseCommand();
            this.RaiseOptionsItemCommand();

            var raiseOptionsItemEvents = this.RaiseOptionsItemEvents(selectedItem);
            if (raiseOptionsItemEvents && this.buttonsListView != null)
            {
                this.buttonsListView.SelectedIndex = -1;
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
                if (this.optionsListView != null)
                {
                    this.optionsListView.SelectedIndex = -1;
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
            if (!(sender is ListBox listBox))
            {
                return;
            }

            listBox.SelectionChanged -= this.ButtonsListView_SelectionChanged;

            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                var canItemClick = this.OnItemClick();

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
            if (!(sender is ListBox listBox))
            {
                return;
            }

            listBox.SelectionChanged -= this.OptionsListView_SelectionChanged;

            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                var canItemClick = this.OnOptionsItemClick();

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