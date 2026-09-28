// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;

namespace MahApps.Metro.Controls
{
    public partial class HamburgerMenu
    {
        /// <summary>
        /// The items this menu has taken into its logical tree, in the order they were taken.
        /// </summary>
        /// <remarks>
        /// An item only hears about an exchanged resource dictionary while it sits in the tree, and it
        /// only sits there while the menu both takes it as a logical child and names it among its
        /// logical children. One of the two alone is not enough: without the first the item has no
        /// parent to look up from, without the second the walk that invalidates the tree never reaches
        /// it.
        /// </remarks>
        private readonly List<HamburgerMenuItemBase> adoptedItems = new();

        private static void OnItemsSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var menu = (HamburgerMenu)dependencyObject;

            if (e.OldValue is INotifyCollectionChanged oldSource)
            {
                oldSource.CollectionChanged -= menu.OnItemsSourceCollectionChanged;
            }

            if (e.NewValue is INotifyCollectionChanged newSource)
            {
                newSource.CollectionChanged += menu.OnItemsSourceCollectionChanged;
            }

            menu.AdoptTheItems();
        }

        private void OnItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            this.AdoptTheItems();
        }

        /// <summary>
        /// Brings the logical children in line with what the two sources hold.
        /// </summary>
        private void AdoptTheItems()
        {
            var wanted = new List<HamburgerMenuItemBase>();
            Gather(this.ItemsSource, wanted);
            Gather(this.OptionsItemsSource, wanted);

            for (var i = this.adoptedItems.Count - 1; i >= 0; i--)
            {
                var item = this.adoptedItems[i];
                if (wanted.Contains(item))
                {
                    continue;
                }

                this.adoptedItems.RemoveAt(i);
                this.RemoveLogicalChild(item);
            }

            foreach (var item in wanted)
            {
                if (this.adoptedItems.Contains(item))
                {
                    continue;
                }

                // An item that already belongs to somebody else stays where it is. Taking it would
                // throw, and a menu is not worth an exception over a item somebody else owns.
                if (item.Parent is not null && !ReferenceEquals(item.Parent, this))
                {
                    continue;
                }

                this.adoptedItems.Add(item);
                this.AddLogicalChild(item);
            }
        }

        private static void Gather(object? source, ICollection<HamburgerMenuItemBase> into)
        {
            if (source is not IEnumerable items)
            {
                return;
            }

            foreach (var item in items)
            {
                if (item is HamburgerMenuItemBase menuItem)
                {
                    into.Add(menuItem);
                }
            }
        }

        /// <inheritdoc />
        protected override IEnumerator LogicalChildren
        {
            get
            {
                var all = new List<object>();

                var inherited = base.LogicalChildren;
                while (inherited is not null && inherited.MoveNext())
                {
                    if (inherited.Current is not null)
                    {
                        all.Add(inherited.Current);
                    }
                }

                all.AddRange(this.adoptedItems);

                return all.GetEnumerator();
            }
        }
    }
}
