using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Actions
{
    public class CloseTabItemAction : CommandTriggerAction
    {
        private TabItem associatedTabItem;

        private TabItem AssociatedTabItem => this.associatedTabItem ?? (this.associatedTabItem = this.AssociatedObject.TryFindParent<TabItem>());

        [Obsolete("This property will be deleted in the next release.")]
        public static readonly DependencyProperty TabControlProperty =
            DependencyProperty.Register(nameof(TabControl),
                                        typeof(TabControl),
                                        typeof(CloseTabItemAction),
                                        new PropertyMetadata(default(TabControl)));

        [Obsolete("This property will be deleted in the next release.")]
        public TabControl TabControl
        {
            get { return (TabControl)this.GetValue(TabControlProperty); }
            set { this.SetValue(TabControlProperty, value); }
        }

        [Obsolete("This property will be deleted in the next release.")]
        public static readonly DependencyProperty TabItemProperty =
            DependencyProperty.Register(nameof(TabItem),
                                        typeof(TabItem),
                                        typeof(CloseTabItemAction),
                                        new PropertyMetadata(default(TabItem)));

        [Obsolete("This property will be deleted in the next release.")]
        public TabItem TabItem
        {
            get { return (TabItem)this.GetValue(TabItemProperty); }
            set { this.SetValue(TabItemProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            if (this.AssociatedObject == null || (this.AssociatedObject != null && !this.AssociatedObject.IsEnabled))
            {
                return;
            }

            var tabControl = this.AssociatedObject.TryFindParent<TabControl>();
            var tabItem = this.AssociatedTabItem;
            if (tabControl == null || tabItem == null)
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

            if (tabControl is MetroTabControl && tabItem is MetroTabItem)
            {
                // run the command handler for the TabControl
                // see #555
                tabControl.BeginInvoke(() => ((MetroTabControl)tabControl).CloseThisTabItem((MetroTabItem)tabItem));
            }
            else
            {
                var closeAction =
                    new Action(
                        () =>
                            {
                                // TODO Raise a closing event to cancel this action

                                if (tabControl.ItemsSource == null)
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
                                    if (collection == null)
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

        protected override object GetCommandParameter()
        {
            return this.CommandParameter ?? this.AssociatedTabItem;
        }
    }
}