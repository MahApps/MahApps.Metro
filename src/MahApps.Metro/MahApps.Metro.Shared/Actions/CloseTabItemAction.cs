using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Actions
{
    public class CloseTabItemAction : TriggerAction<FrameworkElement>
    {
        private TabItem associatedTabItem;

        private TabItem AssociatedTabItem => this.associatedTabItem ?? (this.associatedTabItem = this.AssociatedObject.TryFindParent<TabItem>());

        /// <summary>
        /// Identifies the <see cref="Command" /> dependency property
        /// </summary>
        public static readonly DependencyProperty CommandProperty
            = DependencyProperty.Register(nameof(Command),
                                          typeof(ICommand),
                                          typeof(CloseTabItemAction),
                                          new PropertyMetadata(null, (s, e) => OnCommandChanged(s as CloseTabItemAction, e)));

        /// <summary>
        /// Gets or sets the command that this trigger is bound to.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CommandParameter" /> dependency property
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty
            = DependencyProperty.Register(nameof(CommandParameter),
                                          typeof(object),
                                          typeof(CloseTabItemAction),
                                          new PropertyMetadata(null,
                                                               (s, e) =>
                                                                   {
                                                                       var sender = s as CloseTabItemAction;
                                                                       if (sender?.AssociatedObject != null)
                                                                       {
                                                                           sender.EnableDisableElement();
                                                                       }
                                                                   }));

        /// <summary>
        /// Gets or sets an object that will be passed to the <see cref="Command" /> attached to this trigger.
        /// </summary>
        public object CommandParameter
        {
            get { return this.GetValue(CommandParameterProperty); }
            set { this.SetValue(CommandParameterProperty, value); }
        }

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

        protected override void OnAttached()
        {
            base.OnAttached();
            this.EnableDisableElement();
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

        private static void OnCommandChanged(CloseTabItemAction action, DependencyPropertyChangedEventArgs e)
        {
            if (action == null)
            {
                return;
            }

            if (e.OldValue != null)
            {
                ((ICommand)e.OldValue).CanExecuteChanged -= action.OnCommandCanExecuteChanged;
            }

            var command = (ICommand)e.NewValue;
            if (command != null)
            {
                command.CanExecuteChanged += action.OnCommandCanExecuteChanged;
            }

            action.EnableDisableElement();
        }

        private object GetCommandParameter()
        {
            return this.CommandParameter ?? this.AssociatedTabItem;
        }

        private void EnableDisableElement()
        {
            if (this.AssociatedObject == null)
            {
                return;
            }

            var command = this.Command;
            this.AssociatedObject.IsEnabled = command == null || command.CanExecute(this.GetCommandParameter());
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            this.EnableDisableElement();
        }
    }
}