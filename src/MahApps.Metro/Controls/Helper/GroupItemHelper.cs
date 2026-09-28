// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// What a group of items in a list can do beyond standing there: hold its header at the top
    /// while its own items pass under it, and let that header pick the whole group at once.
    /// </summary>
    /// <remarks>
    /// Both are asked for on the list rather than on the group, since a group is built and thrown
    /// away again as the list scrolls. The two properties inherit, so the value set on the list
    /// reaches every group in it.
    /// </remarks>
    [StyleTypedProperty(Property = "GroupContainerStyle", StyleTargetType = typeof(GroupItem))]
    [StyleTypedProperty(Property = "GroupContainerStyleWithCheckBox", StyleTargetType = typeof(GroupItem))]
    public static class GroupItemHelper
    {
        private const string PART_Header = "PART_Header";
        private const string PART_CheckBox = "PART_CheckBox";

        /// <summary>Identifies the <see cref="IsHeaderStickyProperty"/> attached property.</summary>
        /// <remarks>
        /// WPF lays a group out as one <see cref="GroupItem"/> holding a header and the items of
        /// the group under it, and scrolls the two together. Holding the header in place means
        /// moving it down by as much as its group has already left the top of the view, and no
        /// further than the bottom of that group, so that the next group pushes it out of the way
        /// rather than drawing over it. The move is a render transform, so nothing is laid out
        /// again while the list scrolls.
        ///
        /// This needs the list to scroll by the pixel. A list scrolling by the row, which is what
        /// WPF does by default, never lets a group past the top edge at all: it lays the first
        /// whole row out at the top and shifts what is inside the group instead. Set
        /// VirtualizingPanel.ScrollUnit to Pixel, the way MahApps.Styles.ListView.Win10 does.
        ///
        /// The style carrying this has to name the element to hold in place PART_Header, and that
        /// element has to be free to draw over the items, which means the two sharing a cell of a
        /// grid with the header declared after them.
        /// </remarks>
        public static readonly DependencyProperty IsHeaderStickyProperty
            = DependencyProperty.RegisterAttached(
                "IsHeaderSticky",
                typeof(bool),
                typeof(GroupItemHelper),
                new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.Inherits, OnFeatureChanged));

        /// <summary>Helper for getting <see cref="IsHeaderStickyProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element">Element to read <see cref="IsHeaderStickyProperty"/> from.</param>
        /// <returns>IsHeaderSticky property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
        [AttachedPropertyBrowsableForType(typeof(GroupItem))]
        public static bool GetIsHeaderSticky(DependencyObject element)
        {
            return (bool)element.GetValue(IsHeaderStickyProperty);
        }

        /// <summary>Helper for setting <see cref="IsHeaderStickyProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element">Element to set <see cref="IsHeaderStickyProperty"/> on.</param>
        /// <param name="value">IsHeaderSticky property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
        [AttachedPropertyBrowsableForType(typeof(GroupItem))]
        public static void SetIsHeaderSticky(DependencyObject element, bool value)
        {
            element.SetValue(IsHeaderStickyProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="CanSelectAllItemsProperty"/> attached property.</summary>
        /// <remarks>
        /// Lets the box in a group header stand for every item of that group: ticking it picks
        /// them all, clearing it lets them all go, and it shows a third state for a group only
        /// some of whose items are picked. It needs a list that allows more than one item to be
        /// picked, since there is nothing for it to do otherwise.
        /// </remarks>
        public static readonly DependencyProperty CanSelectAllItemsProperty
            = DependencyProperty.RegisterAttached(
                "CanSelectAllItems",
                typeof(bool),
                typeof(GroupItemHelper),
                new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.Inherits, OnFeatureChanged));

        /// <summary>Helper for getting <see cref="CanSelectAllItemsProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element">Element to read <see cref="CanSelectAllItemsProperty"/> from.</param>
        /// <returns>CanSelectAllItems property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
        [AttachedPropertyBrowsableForType(typeof(GroupItem))]
        public static bool GetCanSelectAllItems(DependencyObject element)
        {
            return (bool)element.GetValue(CanSelectAllItemsProperty);
        }

        /// <summary>Helper for setting <see cref="CanSelectAllItemsProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element">Element to set <see cref="CanSelectAllItemsProperty"/> on.</param>
        /// <param name="value">CanSelectAllItems property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
        [AttachedPropertyBrowsableForType(typeof(GroupItem))]
        public static void SetCanSelectAllItems(DependencyObject element, bool value)
        {
            element.SetValue(CanSelectAllItemsProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="GroupContainerStyleWithCheckBoxProperty"/> attached property.</summary>
        /// <remarks>
        /// The same header with a box beside the name of the group. The helper hands this one over
        /// instead of the plain one while the list lets more than one row be picked and both
        /// <see cref="ItemHelper.IsMultiSelectCheckBoxEnabledProperty"/> and
        /// <see cref="CanSelectAllItemsProperty"/> ask for it.
        ///
        /// Two headers rather than one with a trigger in it, because a trigger naming an attached
        /// property of this library costs a thrown exception every time WPF builds the style from
        /// inside a page, where that prefix means nothing.
        /// </remarks>
        public static readonly DependencyProperty GroupContainerStyleWithCheckBoxProperty
            = DependencyProperty.RegisterAttached(
                "GroupContainerStyleWithCheckBox",
                typeof(Style),
                typeof(GroupItemHelper),
                new PropertyMetadata(null, OnGroupContainerStyleChanged));

        /// <summary>Helper for getting <see cref="GroupContainerStyleWithCheckBoxProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="ItemsControl"/> to read <see cref="GroupContainerStyleWithCheckBoxProperty"/> from.</param>
        /// <returns>The style the set of this list gives its groups while they carry a box.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
        public static Style? GetGroupContainerStyleWithCheckBox(DependencyObject element)
        {
            return (Style?)element.GetValue(GroupContainerStyleWithCheckBoxProperty);
        }

        /// <summary>Helper for setting <see cref="GroupContainerStyleWithCheckBoxProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="ItemsControl"/> to set <see cref="GroupContainerStyleWithCheckBoxProperty"/> on.</param>
        /// <param name="value">The style the set of this list gives its groups while they carry a box.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
        public static void SetGroupContainerStyleWithCheckBox(DependencyObject element, Style? value)
        {
            element.SetValue(GroupContainerStyleWithCheckBoxProperty, value);
        }

        /// <summary>Identifies the <see cref="IsGroupSelectedProperty"/> attached property.</summary>
        /// <remarks>
        /// Whether every item of the group is picked, none of them, or some. The box in the header
        /// is bound to this both ways: the list writes it whenever what is picked changes, and the
        /// box writes it when somebody ticks it, which is what then picks the group.
        /// </remarks>
        public static readonly DependencyProperty IsGroupSelectedProperty
            = DependencyProperty.RegisterAttached(
                "IsGroupSelected",
                typeof(bool?),
                typeof(GroupItemHelper),
                new FrameworkPropertyMetadata(null, OnIsGroupSelectedChanged));

        /// <summary>Helper for getting <see cref="IsGroupSelectedProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="GroupItem"/> to read <see cref="IsGroupSelectedProperty"/> from.</param>
        /// <returns>IsGroupSelected property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(GroupItem))]
        public static bool? GetIsGroupSelected(DependencyObject element)
        {
            return (bool?)element.GetValue(IsGroupSelectedProperty);
        }

        /// <summary>Helper for setting <see cref="IsGroupSelectedProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="GroupItem"/> to set <see cref="IsGroupSelectedProperty"/> on.</param>
        /// <param name="value">IsGroupSelected property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(GroupItem))]
        public static void SetIsGroupSelected(DependencyObject element, bool? value)
        {
            element.SetValue(IsGroupSelectedProperty, value);
        }

        /// <summary>Identifies the <see cref="GroupContainerStyleProperty"/> attached property.</summary>
        /// <remarks>
        /// The style a list gives the groups it shows, put there by the style of the set the list
        /// wears, since the set is the only one that knows which header belongs to it. The list is
        /// handed a <see cref="GroupStyle"/> around it, and only while it has none of its own, so
        /// an application bringing a header of its own keeps it.
        ///
        /// It is a style rather than a whole GroupStyle because GroupStyle is no dependency object.
        /// A GroupStyle kept as a resource has to be built the moment somebody asks for it, whoever
        /// that is, and the template inside it is then parsed where the prefixes of this library
        /// mean nothing.
        /// </remarks>
        public static readonly DependencyProperty GroupContainerStyleProperty
            = DependencyProperty.RegisterAttached(
                "GroupContainerStyle",
                typeof(Style),
                typeof(GroupItemHelper),
                new PropertyMetadata(null, OnGroupContainerStyleChanged));

        /// <summary>Helper for getting <see cref="GroupContainerStyleProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="ItemsControl"/> to read <see cref="GroupContainerStyleProperty"/> from.</param>
        /// <returns>The style the set of this list gives its groups.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
        public static Style? GetGroupContainerStyle(DependencyObject element)
        {
            return (Style?)element.GetValue(GroupContainerStyleProperty);
        }

        /// <summary>Helper for setting <see cref="GroupContainerStyleProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="ItemsControl"/> to set <see cref="GroupContainerStyleProperty"/> on.</param>
        /// <param name="value">The style the set of this list gives its groups.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
        public static void SetGroupContainerStyle(DependencyObject element, Style? value)
        {
            element.SetValue(GroupContainerStyleProperty, value);
        }

        /// <summary>
        /// The scroll viewer this group is being held against, remembered so that the group can let
        /// go of it again. A group is recycled while the list scrolls, so this cannot be looked up
        /// a second time when it is time to unsubscribe.
        /// </summary>
        private static readonly DependencyProperty HostProperty
            = DependencyProperty.RegisterAttached("Host", typeof(ScrollViewer), typeof(GroupItemHelper));

        /// <summary>
        /// What this group listens to the scroll viewer with. Every group needs one of its own,
        /// since it only ever moves its own header, and it has to be the same instance again to
        /// unsubscribe.
        /// </summary>
        private static readonly DependencyProperty ScrollListenerProperty
            = DependencyProperty.RegisterAttached("ScrollListener", typeof(ScrollChangedEventHandler), typeof(GroupItemHelper));

        /// <summary>The list this group belongs to, remembered for the same reason as the scroll viewer.</summary>
        private static readonly DependencyProperty OwnerProperty
            = DependencyProperty.RegisterAttached("Owner", typeof(ListBox), typeof(GroupItemHelper));

        /// <summary>What this group listens to the list with.</summary>
        private static readonly DependencyProperty SelectionListenerProperty
            = DependencyProperty.RegisterAttached("SelectionListener", typeof(SelectionChangedEventHandler), typeof(GroupItemHelper));

        /// <summary>
        /// Whether this list wants the header with the box in it. A binding keeps it current, so
        /// that turning any of the three properties behind it swaps the header over without this
        /// helper having to hold on to the list.
        /// </summary>
        private static readonly DependencyProperty WantsCheckBoxProperty
            = DependencyProperty.RegisterAttached(
                "WantsCheckBox",
                typeof(bool),
                typeof(GroupItemHelper),
                new PropertyMetadata(BooleanBoxes.FalseBox, OnWantsCheckBoxChanged));

        /// <summary>What this list was handed, so that it can be taken away again and nothing else with it.</summary>
        private static readonly DependencyProperty HandedOverProperty
            = DependencyProperty.RegisterAttached("HandedOver", typeof(GroupStyle), typeof(GroupItemHelper));

        /// <summary>
        /// Set while the group is writing its own state down after the list changed, so that the
        /// write does not read as somebody ticking the box and pick the group all over again.
        /// </summary>
        private static readonly DependencyProperty IsReadingBackProperty
            = DependencyProperty.RegisterAttached("IsReadingBack", typeof(bool), typeof(GroupItemHelper), new PropertyMetadata(BooleanBoxes.FalseBox));

        private static void OnGroupContainerStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is not ItemsControl itemsControl)
            {
                return;
            }

            itemsControl.Loaded -= OnItemsControlLoaded;
            TakeBack(itemsControl);

            if (e.NewValue is Style)
            {
                Watch(itemsControl);

                if (itemsControl.IsLoaded)
                {
                    HandOver(itemsControl);
                }
                else
                {
                    // Not before the list is up: whatever the application wrote into its own
                    // GroupStyle is only there by then, and that is what decides the question.
                    itemsControl.Loaded += OnItemsControlLoaded;
                }
            }
        }

        /// <summary>
        /// Ties the private switch to the three properties that answer whether a group header
        /// carries a box. The path of every one of them is built from the property itself, so
        /// nothing here is a name for WPF to look up by prefix.
        /// </summary>
        private static void Watch(ItemsControl itemsControl)
        {
            if (BindingOperations.GetMultiBindingExpression(itemsControl, WantsCheckBoxProperty) is not null)
            {
                return;
            }

            var binding = new MultiBinding { Converter = WantsCheckBoxConverter.Instance, Mode = BindingMode.OneWay };
            binding.Bindings.Add(new Binding { Path = new PropertyPath(ItemHelper.ShowsMultiSelectCheckBoxProperty), Source = itemsControl, Mode = BindingMode.OneWay });
            binding.Bindings.Add(new Binding { Path = new PropertyPath(CanSelectAllItemsProperty), Source = itemsControl, Mode = BindingMode.OneWay });

            BindingOperations.SetBinding(itemsControl, WantsCheckBoxProperty, binding);
        }

        private static void OnWantsCheckBoxChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ItemsControl itemsControl && itemsControl.IsLoaded)
            {
                HandOver(itemsControl);
            }
        }

        private sealed class WantsCheckBoxConverter : IMultiValueConverter
        {
            public static readonly WantsCheckBoxConverter Instance = new();

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                return BooleanBoxes.Box(values.Length == 2 && values[0] is true && values[1] is true);
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }

        private static void OnItemsControlLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is ItemsControl itemsControl)
            {
                HandOver(itemsControl);
            }
        }

        private static void HandOver(ItemsControl itemsControl)
        {
            TakeBack(itemsControl);

            var wantsCheckBox = (bool)itemsControl.GetValue(WantsCheckBoxProperty);
            var containerStyle = wantsCheckBox
                ? GetGroupContainerStyleWithCheckBox(itemsControl) ?? GetGroupContainerStyle(itemsControl)
                : GetGroupContainerStyle(itemsControl);

            if (containerStyle is null || itemsControl.GroupStyle.Count > 0)
            {
                return;
            }

            var groupStyle = new GroupStyle { ContainerStyle = containerStyle };

            itemsControl.GroupStyle.Add(groupStyle);
            itemsControl.SetValue(HandedOverProperty, groupStyle);
        }

        private static void TakeBack(ItemsControl itemsControl)
        {
            if (itemsControl.GetValue(HandedOverProperty) is GroupStyle handedOver)
            {
                itemsControl.GroupStyle.Remove(handedOver);
                itemsControl.ClearValue(HandedOverProperty);
            }
        }

        private static void OnFeatureChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            // The two properties inherit, so this is heard by everything under the list. Only a
            // group has anything to do about it.
            if (dependencyObject is not GroupItem groupItem)
            {
                return;
            }

            groupItem.Loaded -= OnGroupItemLoaded;
            groupItem.Unloaded -= OnGroupItemUnloaded;
            Detach(groupItem);

            if (GetIsHeaderSticky(groupItem) || GetCanSelectAllItems(groupItem))
            {
                groupItem.Loaded += OnGroupItemLoaded;
                groupItem.Unloaded += OnGroupItemUnloaded;

                if (groupItem.IsLoaded)
                {
                    Attach(groupItem);
                }
            }
        }

        private static void OnGroupItemLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is GroupItem groupItem)
            {
                Attach(groupItem);
            }
        }

        private static void OnGroupItemUnloaded(object sender, RoutedEventArgs e)
        {
            if (sender is GroupItem groupItem)
            {
                Detach(groupItem);
            }
        }

        private static void Attach(GroupItem groupItem)
        {
            Detach(groupItem);

            if (GetIsHeaderSticky(groupItem) && groupItem.TryFindParent<ScrollViewer>() is { } scrollViewer)
            {
                ScrollChangedEventHandler listener = (_, _) => Place(groupItem, scrollViewer);

                groupItem.SetValue(HostProperty, scrollViewer);
                groupItem.SetValue(ScrollListenerProperty, listener);
                scrollViewer.ScrollChanged += listener;
                groupItem.SizeChanged += OnGroupItemSizeChanged;

                Place(groupItem, scrollViewer);
            }

            if (GetCanSelectAllItems(groupItem) && groupItem.TryFindParent<ListBox>() is { } owner)
            {
                SelectionChangedEventHandler listener = (_, _) => ReadBack(groupItem, owner);

                groupItem.SetValue(OwnerProperty, owner);
                groupItem.SetValue(SelectionListenerProperty, listener);
                owner.SelectionChanged += listener;

                // The header of a group that carries a box has one under the name PART_Header, and
                // it is tied to the state of the group from here rather than in the template, for
                // the same reason the two headers are two styles rather than one with a trigger.
                if (GetCheckBox(groupItem) is { } checkBox)
                {
                    BindingOperations.SetBinding(checkBox,
                                                 ToggleButton.IsCheckedProperty,
                                                 new Binding
                                                 {
                                                     Path = new PropertyPath(IsGroupSelectedProperty),
                                                     Source = groupItem,
                                                     Mode = BindingMode.TwoWay
                                                 });
                }

                ReadBack(groupItem, owner);
            }
        }

        private static void Detach(GroupItem groupItem)
        {
            if (groupItem.GetValue(HostProperty) is ScrollViewer scrollViewer
                && groupItem.GetValue(ScrollListenerProperty) is ScrollChangedEventHandler scrollListener)
            {
                scrollViewer.ScrollChanged -= scrollListener;
            }

            if (groupItem.GetValue(OwnerProperty) is ListBox owner
                && groupItem.GetValue(SelectionListenerProperty) is SelectionChangedEventHandler selectionListener)
            {
                owner.SelectionChanged -= selectionListener;
            }

            if (GetCheckBox(groupItem) is { } checkBox)
            {
                BindingOperations.ClearBinding(checkBox, ToggleButton.IsCheckedProperty);
            }

            groupItem.ClearValue(HostProperty);
            groupItem.ClearValue(ScrollListenerProperty);
            groupItem.ClearValue(OwnerProperty);
            groupItem.ClearValue(SelectionListenerProperty);
            groupItem.SizeChanged -= OnGroupItemSizeChanged;

            if (GetHeader(groupItem)?.RenderTransform is TranslateTransform transform)
            {
                transform.Y = 0d;
            }
        }

        private static void OnGroupItemSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is GroupItem groupItem && groupItem.GetValue(HostProperty) is ScrollViewer scrollViewer)
            {
                Place(groupItem, scrollViewer);
            }
        }

        private static void Place(GroupItem groupItem, ScrollViewer scrollViewer)
        {
            var header = GetHeader(groupItem);
            if (header is null || !groupItem.IsVisible || !scrollViewer.IsVisible)
            {
                return;
            }

            if (header.RenderTransform is not TranslateTransform transform)
            {
                transform = new TranslateTransform();
                header.RenderTransform = transform;
            }

            var top = groupItem.TranslatePoint(new Point(0d, 0d), scrollViewer).Y;
            if (top >= 0d)
            {
                transform.Y = 0d;
                return;
            }

            // As far down as the group has gone past the top, but never past its own last item.
            var room = Math.Max(0d, groupItem.ActualHeight - header.ActualHeight);
            transform.Y = Math.Min(-top, room);
        }

        private static void OnIsGroupSelectedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is not GroupItem groupItem
                || (bool)groupItem.GetValue(IsReadingBackProperty)
                || groupItem.GetValue(OwnerProperty) is not ListBox owner)
            {
                return;
            }

            // The third state is what a group only some of whose items are picked looks like. It is
            // never something to act on, only something to show.
            if (e.NewValue is not bool wanted)
            {
                return;
            }

            var selected = owner.SelectedItems;

            foreach (var item in Items(groupItem))
            {
                if (wanted)
                {
                    if (!selected.Contains(item))
                    {
                        selected.Add(item);
                    }
                }
                else
                {
                    selected.Remove(item);
                }
            }
        }

        private static void ReadBack(GroupItem groupItem, ListBox owner)
        {
            var items = Items(groupItem).ToList();
            if (items.Count == 0)
            {
                return;
            }

            var picked = items.Count(item => owner.SelectedItems.Contains(item));
            var state = picked == 0 ? false : picked == items.Count ? true : (bool?)null;

            groupItem.SetValue(IsReadingBackProperty, BooleanBoxes.TrueBox);
            try
            {
                SetIsGroupSelected(groupItem, state);
            }
            finally
            {
                groupItem.SetValue(IsReadingBackProperty, BooleanBoxes.FalseBox);
            }
        }

        /// <summary>
        /// The items of the group, reaching through any groups inside it, since a group of groups
        /// holds those rather than items.
        /// </summary>
        private static IEnumerable<object> Items(GroupItem groupItem)
        {
            return groupItem.Content is CollectionViewGroup group ? Items(group) : Enumerable.Empty<object>();
        }

        private static IEnumerable<object> Items(CollectionViewGroup group)
        {
            foreach (var item in group.Items)
            {
                if (item is CollectionViewGroup inner)
                {
                    foreach (var deeper in Items(inner))
                    {
                        yield return deeper;
                    }
                }
                else
                {
                    yield return item;
                }
            }
        }

        private static FrameworkElement? GetHeader(GroupItem groupItem)
        {
            return groupItem.Template?.FindName(PART_Header, groupItem) as FrameworkElement;
        }

        private static ToggleButton? GetCheckBox(GroupItem groupItem)
        {
            return groupItem.Template?.FindName(PART_CheckBox, groupItem) as ToggleButton;
        }
    }
}
