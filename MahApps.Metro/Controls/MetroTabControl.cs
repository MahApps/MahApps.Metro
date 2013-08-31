using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    public class MetroTabControl : BaseMetroTabControl
    {
        public MetroTabControl()
        {
            DefaultStyleKey = typeof(MetroTabControl);
        }
    }

    public abstract class BaseMetroTabControl : TabControl
    {
        public BaseMetroTabControl()
        {
            InternalCloseTabCommand = new DefaultCloseTabCommand(this);

            this.Loaded += BaseMetroTabControl_Loaded;
            this.Unloaded += BaseMetroTabControl_Unloaded;
        }

        void BaseMetroTabControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= BaseMetroTabControl_Loaded;
            this.Unloaded -= BaseMetroTabControl_Unloaded;
        }

        void BaseMetroTabControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Ensure each tabitem knows what the owning tab is.

            if (ItemsSource == null)
                foreach (TabItem item in Items)
                    if (item is MetroTabItem)
                        ((MetroTabItem)item).OwningTabControl = this;

        }

        public Thickness TabStripMargin
        {
            get { return (Thickness)GetValue(TabStripMarginProperty); }
            set { SetValue(TabStripMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabStripMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabStripMarginProperty =
            DependencyProperty.Register("TabStripMargin", typeof(Thickness), typeof(BaseMetroTabControl), new PropertyMetadata(new Thickness(0)));

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TabItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MetroTabItem { OwningTabControl = this }; //Overrides the TabControl's default behavior and returns a MetroTabItem instead of a regular one.
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element != item)
                element.SetCurrentValue(DataContextProperty, item); //dont want to set the datacontext to itself.

            base.PrepareContainerForItemOverride(element, item);
        }

        public ICommand CloseTabCommand
        {
            get { return (ICommand)GetValue(CloseTabCommandProperty); }
            set { SetValue(CloseTabCommandProperty, value); }
        }

        public static readonly DependencyProperty CloseTabCommandProperty =
            DependencyProperty.Register("CloseTabCommand", typeof(ICommand), typeof(BaseMetroTabControl), new PropertyMetadata(null));

        internal ICommand InternalCloseTabCommand
        {
            get { return (ICommand)GetValue(InternalCloseTabCommandProperty); }
            set { SetValue(InternalCloseTabCommandProperty, value); }
        }

        private static readonly DependencyProperty InternalCloseTabCommandProperty =
            DependencyProperty.Register("InternalCloseTabCommand", typeof(ICommand), typeof(BaseMetroTabControl), new PropertyMetadata(null));


        public delegate void TabItemClosingEventHandler(object sender, TabItemClosingEventArgs e);
        public event TabItemClosingEventHandler TabItemClosingEvent;

        internal bool RaiseTabItemClosingEvent(MetroTabItem closingItem)
        {
            if (TabItemClosingEvent != null)
            {
                foreach (TabItemClosingEventHandler subHandler in TabItemClosingEvent.GetInvocationList())
                {
                    var args = new TabItemClosingEventArgs(closingItem);
                    subHandler.Invoke(this, args);
                    if (args.Cancel)
                        return true;
                }
            }

            return false;
        }

        public class TabItemClosingEventArgs : CancelEventArgs
        {
            internal TabItemClosingEventArgs(MetroTabItem item)
            {
                ClosingTabItem = item;
            }

            public MetroTabItem ClosingTabItem { get; private set; }
        }

        internal class DefaultCloseTabCommand : ICommand
        {
            private readonly BaseMetroTabControl owner;
            internal DefaultCloseTabCommand(BaseMetroTabControl Owner)
            {
                owner = Owner;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event System.EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                if (parameter != null)
                {
                    var paramData = (Tuple<object, MetroTabItem>)parameter;

                    if (owner.CloseTabCommand != null) // TODO: let MetroTabControl define parameter to pass to command
                        owner.CloseTabCommand.Execute(null);
                    else
                    {
                        if (paramData.Item2 is MetroTabItem)
                        {
                            var tabItem = paramData.Item2;

                            // KIDS: don't try this at home
                            // this is not good MVVM habits and I'm only doing it
                            // because I want the demos to be absolutely bitching

                            // the control is allowed to cancel this event
                            if (owner.RaiseTabItemClosingEvent(tabItem)) return;

                            if (owner.ItemsSource == null)
                            {
                                // if the list is hard-coded (i.e. has no ItemsSource)
                                // then we remove the item from the collection
                                owner.Items.Remove(tabItem);
                            }
                            else
                            {
                                // if ItemsSource is something we cannot work with, bail out
                                var collection = owner.ItemsSource as IList;
                                if (collection == null) return;

                                // find the item and kill it (I mean, remove it)
                                foreach (var item in owner.ItemsSource.Cast<object>().Where(item => tabItem.DataContext == item))
                                {
                                    collection.Remove(item);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
