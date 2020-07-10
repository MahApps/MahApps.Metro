// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace MahApps.Metro.Behaviors
{
    /// <summary>
    /// <para>
    ///     Sets the first TabItem with Visibility="<see cref="Visibility.Visible"/>" as
    ///     the SelectedItem of the TabControl.
    /// </para>
    /// <para>
    ///     If there is no visible TabItem, null is set as the SelectedItem
    /// </para>
    /// </summary>
    public class TabControlSelectFirstVisibleTabBehavior : Behavior<TabControl>
    {
        protected override void OnAttached()
        {
            AssociatedObject.SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            List<TabItem> tabItems = AssociatedObject.Items.Cast<TabItem>().ToList();
            TabItem selectedItem = AssociatedObject.SelectedItem as TabItem;

            //if the selected item is visible already, return
            if (selectedItem != null && selectedItem.Visibility == Visibility.Visible)
            {
                return;
            }

            //get first visible item
            TabItem firstVisible = tabItems.FirstOrDefault(t => t.Visibility == Visibility.Visible);
            if (firstVisible != null)
            {
                AssociatedObject.SelectedIndex = tabItems.IndexOf(firstVisible);
            }
            else
            {
                //there is no visible item
                //Raises SelectionChanged again one time (second time, oldValue == newValue)
                AssociatedObject.SelectedItem = null;
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= OnSelectionChanged;
        }
    }
}