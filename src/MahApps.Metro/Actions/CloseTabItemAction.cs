// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Linq;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Actions
{
    public class CloseTabItemAction : CommandTriggerAction
    {
        private TabItem? associatedTabItem;

        private TabItem? AssociatedTabItem => this.associatedTabItem ??= this.AssociatedObject.TryFindParent<TabItem>();

        protected override void Invoke(object? parameter)
        {
            if (this.AssociatedObject is null || (this.AssociatedObject != null && !this.AssociatedObject.IsEnabled))
            {
                return;
            }

            var tabControl = this.AssociatedObject?.TryFindParent<TabControl>();
            var tabItem = this.AssociatedTabItem;
            if (tabControl is null || tabItem is null)
            {
                return;
            }

            var command = this.Command;
            if (command != null)
            {
                var commandParameter = this.GetCommandParameter();
                if (command.CanExecute(commandParameter))
                {
                    command.Execute(commandParameter);
                }
            }

            if (tabControl is BaseMetroTabControl metroTabControl && tabItem is MetroTabItem metroTabItem)
            {
                // run the command handler for the TabControl
                // see #555
                metroTabControl.BeginInvoke(x => x.CloseThisTabItem(metroTabItem));
            }
            else
            {
                var closeAction =
                    new Action(
                        () =>
                            {
                                // TODO Raise a closing event to cancel this action

                                if (tabControl.ItemsSource is null)
                                {
                                    // if the list is hard-coded (i.e. has no ItemsSource)
                                    // then we remove the item from the collection
                                    tabItem.ClearStyle();
                                    tabControl.Items.Remove(tabItem);
                                }
                                else
                                {
                                    // if ItemsSource is something we cannot work with, bail out
                                    var collection = tabControl.ItemsSource as IList;
                                    if (collection is null)
                                    {
                                        return;
                                    }

                                    // find the item and kill it (I mean, remove it)
                                    var item2Remove = collection.OfType<object>().FirstOrDefault(item => tabItem == item || tabItem.DataContext == item);
                                    if (item2Remove != null)
                                    {
                                        tabItem.ClearStyle();
                                        collection.Remove(item2Remove);
                                    }
                                }
                            });
                this.BeginInvoke(closeAction);
            }
        }

        protected override object? GetCommandParameter()
        {
            var parameter = this.CommandParameter;
            if (parameter is null && this.PassAssociatedObjectToCommand)
            {
                parameter = this.AssociatedTabItem;
            }

            return parameter;
        }
    }
}