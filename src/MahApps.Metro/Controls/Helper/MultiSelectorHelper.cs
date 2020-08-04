// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Defines a helper class for selected items binding on collections with multiselector elements
    /// </summary>
    public static class MultiSelectorHelper
    {
        public static readonly DependencyProperty SelectedItemsProperty
            = DependencyProperty.RegisterAttached(
                "SelectedItems",
                typeof(IList),
                typeof(MultiSelectorHelper),
                new FrameworkPropertyMetadata(null, OnSelectedItemsChanged));

        /// <summary>
        /// Handles disposal and creation of old and new bindings
        /// </summary>
        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ListBox || d is MultiSelector)) throw new ArgumentException("The property 'SelectedItems' may only be set on ListBox or MultiSelector elements.");

            if (e.OldValue != e.NewValue)
            {
                var oldBinding = GetSelectedItemBinding(d);
                oldBinding?.UnBind();

                var multiSelectorBinding = new MultiSelectorBinding((Selector)d, (IList)e.NewValue);
                SetSelectedItemBinding(d, multiSelectorBinding);
                multiSelectorBinding.Bind();
            }
        }

        /// <summary>
        /// Gets the selected items property binding
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelector))]
        public static IList GetSelectedItems(DependencyObject element)
        {
            return (IList)element.GetValue(SelectedItemsProperty);
        }

        /// <summary>
        /// Sets the selected items property binding
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelector))]
        public static void SetSelectedItems(DependencyObject element, IList value)
        {
            element.SetValue(SelectedItemsProperty, value);
        }

        private static readonly DependencyProperty SelectedItemBindingProperty
            = DependencyProperty.RegisterAttached(
                "SelectedItemBinding",
                typeof(MultiSelectorBinding),
                typeof(MultiSelectorHelper));

        /// <summary>
        /// Gets the <see cref="MultiSelectorBinding"/> for a binding
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(ListBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelector))]
        private static MultiSelectorBinding GetSelectedItemBinding(DependencyObject element)
        {
            return (MultiSelectorBinding)element.GetValue(SelectedItemBindingProperty);
        }

        /// <summary>
        /// Sets the <see cref="MultiSelectorBinding"/> for a bining
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(ListBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelector))]
        private static void SetSelectedItemBinding(DependencyObject element, MultiSelectorBinding value)
        {
            element.SetValue(SelectedItemBindingProperty, value);
        }

        /// <summary>
        /// Defines a binding between multi selector and property
        /// </summary>
        private class MultiSelectorBinding
        {
            private readonly Selector _selector;
            private readonly IList _collection;

            /// <summary>
            /// Creates an instance of <see cref="MultiSelectorBinding"/>
            /// </summary>
            /// <param name="selector">The selector of this binding</param>
            /// <param name="collection">The bound collection</param>
            public MultiSelectorBinding(Selector selector, IList collection)
            {
                _selector = selector;
                _collection = collection;

                if (selector is ListBox listbox)
                {
                    listbox.SelectedItems.Clear();
                    foreach (var newItem in collection)
                    {
                        listbox.SelectedItems.Add(newItem);
                    }
                }
                else if (_selector is MultiSelector multiSelector)
                {
                    multiSelector.SelectedItems.Clear();
                    foreach (var newItem in collection)
                    {
                        multiSelector.SelectedItems.Add(newItem);
                    }
                }
            }

            /// <summary>
            /// Registers the event handlers for selector and collection changes
            /// </summary>
            public void Bind()
            {
                // prevent multiple event registration
                UnBind();

                _selector.SelectionChanged += OnSelectionChanged;
                if (_collection is INotifyCollectionChanged notifyCollection)
                {
                    notifyCollection.CollectionChanged += this.OnCollectionChanged;
                }
            }

            /// <summary>
            /// Unregisters the event handlers for selector and collection changes
            /// </summary>
            public void UnBind()
            {
                _selector.SelectionChanged -= OnSelectionChanged;
                if (_collection is INotifyCollectionChanged notifyCollection)
                {
                    notifyCollection.CollectionChanged -= this.OnCollectionChanged;
                }
            }

            /// <summary>
            /// Updates the collection with changes made in the selector
            /// </summary>
            private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                var notifyCollection = _collection as INotifyCollectionChanged;
                if (notifyCollection != null)
                {
                    notifyCollection.CollectionChanged -= this.OnCollectionChanged;
                }

                try
                {
                    foreach (var oldItem in e.RemovedItems)
                    {
                        _collection.Remove(oldItem);
                    }

                    foreach (var newItem in e.AddedItems)
                    {
                        _collection.Add(newItem);
                    }
                }
                finally
                {
                    if (notifyCollection != null)
                    {
                        notifyCollection.CollectionChanged += this.OnCollectionChanged;
                    }
                }
            }

            /// <summary>
            /// Updates the selector with changes made in the collection
            /// </summary>
            private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                _selector.SelectionChanged -= OnSelectionChanged;

                try
                {
                    if (_selector is ListBox listBox)
                    {
                        switch (e.Action)
                        {
                            case NotifyCollectionChangedAction.Add:
                                foreach (var newItem in e.NewItems)
                                {
                                    listBox.SelectedItems.Add(newItem);
                                }

                                break;
                            case NotifyCollectionChangedAction.Remove:
                                foreach (var oldItem in e.OldItems)
                                {
                                    listBox.SelectedItems.Remove(oldItem);
                                }

                                break;
                            case NotifyCollectionChangedAction.Move:
                            case NotifyCollectionChangedAction.Replace:
                                RemoveItems(listBox.SelectedItems, e);
                                AddItems(listBox.SelectedItems, e);
                                break;
                            case NotifyCollectionChangedAction.Reset:
                                listBox.SelectedItems.Clear();
                                break;
                        }
                    }
                    else if (_selector is MultiSelector multiSelector)
                    {
                        switch (e.Action)
                        {
                            case NotifyCollectionChangedAction.Add:
                                foreach (var newItem in e.NewItems)
                                {
                                    multiSelector.SelectedItems.Add(newItem);
                                }

                                break;
                            case NotifyCollectionChangedAction.Remove:
                                foreach (var oldItem in e.OldItems)
                                {
                                    multiSelector.SelectedItems.Remove(oldItem);
                                }

                                break;
                            case NotifyCollectionChangedAction.Move:
                            case NotifyCollectionChangedAction.Replace:
                                RemoveItems(multiSelector.SelectedItems, e);
                                AddItems(multiSelector.SelectedItems, e);
                                break;
                            case NotifyCollectionChangedAction.Reset:
                                multiSelector.SelectedItems.Clear();
                                break;
                        }
                    }
                }
                finally
                {
                    _selector.SelectionChanged += OnSelectionChanged;
                }
            }

            private static void RemoveItems(IList list, NotifyCollectionChangedEventArgs e)
            {
                var itemCount = e.OldItems.Count;

                // For the number of items being removed, remove the item from the old starting index
                // (this will cause following items to be shifted down to fill the hole).
                for (var i = 0; i < itemCount; i++)
                {
                    list.RemoveAt(e.OldStartingIndex);
                }
            }

            private static void AddItems(IList list, NotifyCollectionChangedEventArgs e)
            {
                var itemCount = e.NewItems.Count;

                for (var i = 0; i < itemCount; i++)
                {
                    var insertionPoint = e.NewStartingIndex + i;

                    if (insertionPoint > list.Count)
                    {
                        list.Add(e.NewItems[i]);
                    }
                    else
                    {
                        list.Insert(insertionPoint, e.NewItems[i]);
                    }
                }
            }
        }
    }
}