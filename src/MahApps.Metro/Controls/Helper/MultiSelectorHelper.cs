// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Defines a helper class for selected items binding on <see cref="ListBox"/> or <see cref="MultiSelector"/> controls.
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
            if (!(d is ListBox || d is MultiSelector))
            {
                throw new ArgumentException("The property 'SelectedItems' may only be set on ListBox, MultiSelector or MultiSelectionComboBox elements.");
            }

            if (e.OldValue != e.NewValue)
            {
                var oldBinding = GetSelectedItemBinding(d);
                oldBinding?.UnBind();

                if (e.NewValue is IList newList)
                {
                    var multiSelectorBinding = new MultiSelectorBinding((Selector)d, newList);
                    SetSelectedItemBinding(d, multiSelectorBinding);
                    multiSelectorBinding.Bind();
                }
                else
                {
                    SetSelectedItemBinding((Selector)d, null);
                }
            }
        }

        /// <summary>Helper for getting <see cref="SelectedItemsProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="SelectedItemsProperty"/> from.</param>
        /// <returns>SelectedItems property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelector))]
        public static IList? GetSelectedItems(DependencyObject element)
        {
            return (IList?)element.GetValue(SelectedItemsProperty);
        }

        /// <summary>Helper for setting <see cref="SelectedItemsProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="SelectedItemsProperty"/> on.</param>
        /// <param name="value">SelectedItems property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ListBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelector))]
        public static void SetSelectedItems(DependencyObject element, IList? value)
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
        private static MultiSelectorBinding? GetSelectedItemBinding(DependencyObject element)
        {
            return (MultiSelectorBinding?)element.GetValue(SelectedItemBindingProperty);
        }

        /// <summary>
        /// Sets the <see cref="MultiSelectorBinding"/> for a binding
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(ListBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelector))]
        private static void SetSelectedItemBinding(DependencyObject element, MultiSelectorBinding? value)
        {
            element.SetValue(SelectedItemBindingProperty, value);
        }

        /// <summary>
        /// Defines a binding between multi selector and property
        /// </summary>
        private class MultiSelectorBinding
        {
            private readonly ObservableCollection<object>? _selectedItems;
            private readonly IList _collection;

            /// <summary>
            /// Creates an instance of <see cref="MultiSelectorBinding"/>
            /// </summary>
            /// <param name="selector">The selector of this binding</param>
            /// <param name="collection">The bound collection</param>
            public MultiSelectorBinding(Selector selector, IList collection)
            {
                this._collection = collection;

                if (selector is ListBox listbox)
                {
                    _selectedItems = listbox.SelectedItems as ObservableCollection<object>;
                }
                else if (selector is MultiSelector multiSelector)
                {
                    _selectedItems = multiSelector.SelectedItems as ObservableCollection<object>;
                }
                if (_selectedItems is null)
                {
                    throw new NullReferenceException("SelectedItems-Collection was null.");
                }

                _selectedItems?.Clear();
                foreach (var newItem in collection)
                {
                    if (newItem is not null)
                    {
                        _selectedItems?.Add(newItem);
                    }
                }
            }

            /// <summary>
            /// Registers the event handlers for selector and collection changes
            /// </summary>
            public void Bind()
            {
                // prevent multiple event registration
                this.UnBind();

                if (this._selectedItems is not null)
                {
                    this._selectedItems.CollectionChanged += OnSelectedItemsCollectionChanged;
                }

                if (this._collection is INotifyCollectionChanged notifyCollection)
                {
                    notifyCollection.CollectionChanged += this.OnCollectionChanged;
                }
            }

            /// <summary>
            /// Clear the event handlers for selector and collection changes
            /// </summary>
            public void UnBind()
            {
                if (this._selectedItems is not null)
                {
                    this._selectedItems.CollectionChanged -= OnSelectedItemsCollectionChanged;
                }

                if (this._collection is INotifyCollectionChanged notifyCollection)
                {
                    notifyCollection.CollectionChanged -= this.OnCollectionChanged;
                }
            }


            /// <summary>
            /// Updates the selector with changes made in the collection
            /// </summary>
            private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
            {
                if (this._selectedItems is null)
                {
                    return;
                }

                this._selectedItems.CollectionChanged -= OnSelectedItemsCollectionChanged;

                try
                {
                    SyncCollections(_collection, _selectedItems, e);
                }
                finally
                {
                    this._selectedItems.CollectionChanged += OnSelectedItemsCollectionChanged;
                }
            }

            /// <summary>
            /// Updates the source collection with changes made in the selector
            /// </summary>
            private void OnSelectedItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
            {
                if (_collection is INotifyCollectionChanged notifyCollection)
                {
                    notifyCollection.CollectionChanged -= OnCollectionChanged;
                }

                try
                {
                    SyncCollections(_selectedItems, _collection, e);
                }
                finally
                {
                    if (_collection is INotifyCollectionChanged notifyCollection1)
                    {
                        notifyCollection1.CollectionChanged += OnCollectionChanged;
                    }
                }
            }

            private static void SyncCollections(IList? sourceCollection, IList? targetCollection, NotifyCollectionChangedEventArgs e)
            {
                if (sourceCollection is null || targetCollection is null)
                {
                    return;
                }


                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewItems is null)
                        {
                            break;
                        }
                        foreach (var item in e.NewItems)
                        {
                            targetCollection.Add(item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        if (e.OldItems is null)
                        {
                            break;
                        }
                        foreach (var item in e.OldItems)
                        {
                            targetCollection.Remove(item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        if (e.NewItems is null || e.OldItems is null)
                        {
                            break;
                        }
                        foreach (var item in e.NewItems)
                        {
                            targetCollection.Add(item);
                        }
                        foreach (var item in e.OldItems)
                        {
                            targetCollection.Remove(item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        targetCollection.Clear();

                        for (int i = 0; i < sourceCollection.Count; i++)
                        {
                            targetCollection.Add(sourceCollection[i]);
                        }
                        break;
                }
            }
        }
    }
}