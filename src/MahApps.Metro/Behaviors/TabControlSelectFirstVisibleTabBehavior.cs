// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            base.OnAttached();

            this.AssociatedObject.SelectionChanged += this.OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            // We don't need select the TabItem if the selected item is already visible
            if (this.AssociatedObject.SelectedItem is TabItem selectedItem && selectedItem.Visibility == Visibility.Visible)
            {
                return;
            }

            // Get the first visible item
            var tabItems = this.AssociatedObject.Items.OfType<TabItem>().ToList();
            var firstVisible = tabItems.FirstOrDefault(t => t.Visibility == Visibility.Visible);

            if (firstVisible != null)
            {
                this.AssociatedObject.SetCurrentValue(Selector.SelectedIndexProperty, tabItems.IndexOf(firstVisible));
            }
            else
            {
                // There is no visible item
                // Raises SelectionChanged again one time (second time, oldValue == newValue)
                this.AssociatedObject.SetCurrentValue(Selector.SelectedItemProperty, null);
            }
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.SelectionChanged -= this.OnSelectionChanged;

            base.OnDetaching();
        }
    }
}