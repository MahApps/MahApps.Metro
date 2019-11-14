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
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached(
           "SelectedItems",
           typeof(IList),
           typeof(MultiSelectorHelper),
           new PropertyMetadata(null, OnSelectedItemsChanged));

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

        private static readonly DependencyProperty SelectedItemBindingProperty = DependencyProperty.RegisterAttached(
           "SelectedItemBinding",
           typeof(MultiSelectorBinding),
           typeof(MultiSelectorHelper));

        /// <summary>
        /// Gets the <see cref="MultiSelectorBinding"/> for a binding
        /// </summary>
        private static MultiSelectorBinding GetSelectedItemBinding(DependencyObject element)
        {
            return (MultiSelectorBinding)element.GetValue(SelectedItemBindingProperty);
        }

        /// <summary>
        /// Sets the <see cref="MultiSelectorBinding"/> for a bining
        /// </summary>
        private static void SetSelectedItemBinding(DependencyObject element, MultiSelectorBinding value)
        {
            element.SetValue(SelectedItemBindingProperty, value);
        }

        /// <summary>
        /// Handles disposal and creation of old and new bindings
        /// </summary>
        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ListBox || d is MultiSelector)) throw new ArgumentException("The property 'SelectedItems' may only be set on ListBox or MultiSelector elements.");

            var oldBinding = GetSelectedItemBinding(d);
            if (oldBinding != null) oldBinding.UnBind();

            SetSelectedItemBinding(d, new MultiSelectorBinding(d, (IList)e.NewValue));
            GetSelectedItemBinding(d).Bind();
        }

        /// <summary>
        /// Defines a binding between multi selector and property
        /// </summary>
        private class MultiSelectorBinding
        {
            private readonly object _selector;

            private readonly IList _collection;

            /// <summary>
            /// Creates an instance of <see cref="MultiSelectorBinding"/>
            /// </summary>
            /// <param name="selector">The selector of this binding</param>
            /// <param name="collection">The bound collection</param>
            public MultiSelectorBinding(object selector, IList collection)
            {
                _selector = selector;
                _collection = collection;

                if (selector is ListBox listbox)
                {
                    listbox.SelectedItems.Clear();
                    foreach (var newItem in collection) listbox.SelectedItems.Add(newItem);
                }
                if (_selector is MultiSelector multiSelector)
                {
                    multiSelector.SelectedItems.Clear();
                    foreach (var newItem in collection) multiSelector.SelectedItems.Add(newItem);
                }
            }

            /// <summary>
            /// Registers the event handlers for selector and collection changes
            /// </summary>
            public void Bind()
            {
                (_selector as Selector).SelectionChanged += OnSelectionChanged;
                (_collection as INotifyCollectionChanged).CollectionChanged += OnCollectionChanged;
            }

            /// <summary>
            /// Unregisters the event handlers for selector and collection changes
            /// </summary>
            public void UnBind()
            {
                (_selector as Selector).SelectionChanged -= OnSelectionChanged;
                (_collection as INotifyCollectionChanged).CollectionChanged -= OnCollectionChanged;
            }

            /// <summary>
            /// Updates the collection with changes made in the selector
            /// </summary>
            private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                (_collection as INotifyCollectionChanged).CollectionChanged -= OnCollectionChanged;

                foreach (var oldItem in e.RemovedItems) _collection.Remove(oldItem);
                foreach (var newItem in e.AddedItems) _collection.Add(newItem);

                (_collection as INotifyCollectionChanged).CollectionChanged += OnCollectionChanged;
            }

            /// <summary>
            /// Updates the selector with changes made in the collection
            /// </summary>
            private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                (_selector as Selector).SelectionChanged -= OnSelectionChanged;

                if (_selector is ListBox listBox)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach (var newItem in e.NewItems) listBox.SelectedItems.Add(newItem);
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            foreach (var oldItem in e.OldItems) listBox.SelectedItems.Remove(oldItem);
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            foreach (var newItem in e.NewItems) listBox.SelectedItems.Add(newItem);
                            foreach (var oldItem in e.OldItems) listBox.SelectedItems.Remove(oldItem);
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            listBox.SelectedItems.Clear();
                            break;
                    }
                }
                if (_selector is MultiSelector multiSelector)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach (var newItem in e.NewItems) multiSelector.SelectedItems.Add(newItem);
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            foreach (var oldItem in e.OldItems) multiSelector.SelectedItems.Remove(oldItem);
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            foreach (var newItem in e.NewItems) multiSelector.SelectedItems.Add(newItem);
                            foreach (var oldItem in e.OldItems) multiSelector.SelectedItems.Remove(oldItem);
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            multiSelector.SelectedItems.Clear();
                            break;
                    }
                }

                (_selector as Selector).SelectionChanged += OnSelectionChanged;
            }
        }
    }
}
