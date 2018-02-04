using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JetBrains.Annotations;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A standard MetroTabControl (Pivot).
    /// </summary>
    public class MetroTabControl : BaseMetroTabControl
    {
        /// <summary>
        /// Initializes a new instance of the MahApps.Metro.Controls.MetroTabControl class.
        /// </summary>
        public MetroTabControl()
        {
            DefaultStyleKey = typeof(MetroTabControl);
        }
    }

    /// <summary>
    /// A base class for every MetroTabControl (Pivot).
    /// </summary>
    public abstract class BaseMetroTabControl : TabControl
    {
        public BaseMetroTabControl()
        {
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
            return new MetroTabItem(); //Overrides the TabControl's default behavior and returns a MetroTabItem instead of a regular one.
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element != item)
            {
                element.SetValue(DataContextProperty, item); //dont want to set the datacontext to itself.
            }

            base.PrepareContainerForItemOverride(element, item);
        }

        /// <summary>
        /// Get/sets the command that executes when a MetroTabItem's close button is clicked.
        /// </summary>
        public ICommand CloseTabCommand
        {
            get { return (ICommand)GetValue(CloseTabCommandProperty); }
            set { SetValue(CloseTabCommandProperty, value); }
        }

        public static readonly DependencyProperty CloseTabCommandProperty =
            DependencyProperty.Register("CloseTabCommand", typeof(ICommand), typeof(BaseMetroTabControl), new PropertyMetadata(null));

        public delegate void TabItemClosingEventHandler(object sender, TabItemClosingEventArgs e);

        /// <summary>
        /// An event that is raised when a TabItem is closed.
        /// </summary>
        // Todo Rename this to TabItemClosing
        public event TabItemClosingEventHandler TabItemClosingEvent;

        internal bool RaiseTabItemClosingEvent(MetroTabItem closingItem)
        {
            var tabItemClosingEvent = this.TabItemClosingEvent;
            if (tabItemClosingEvent != null)
            {
                foreach (TabItemClosingEventHandler subHandler in tabItemClosingEvent.GetInvocationList().OfType<TabItemClosingEventHandler>())
                {
                    var args = new TabItemClosingEventArgs(closingItem);
                    subHandler.Invoke(this, args);
                    if (args.Cancel)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Event args that is created when a TabItem is closed.
        /// </summary>
        public class TabItemClosingEventArgs : CancelEventArgs
        {
            internal TabItemClosingEventArgs(MetroTabItem item)
            {
                ClosingTabItem = item;
            }

            /// <summary>
            /// Gets the MetroTabItem that will be closed.
            /// </summary>
            public MetroTabItem ClosingTabItem { get; private set; }
        }

        internal void CloseThisTabItem([NotNull] MetroTabItem tabItem)
        {
            if (tabItem == null)
            {
                throw new ArgumentNullException(nameof(tabItem));
            }

            if (this.CloseTabCommand != null)
            {
                var closeTabCommandParameter = tabItem.CloseTabCommandParameter ?? tabItem;
                if (this.CloseTabCommand.CanExecute(closeTabCommandParameter))
                {
                    this.CloseTabCommand.Execute(closeTabCommandParameter);
                }
            }
            else
            {
                // KIDS: don't try this at home
                // this is not good MVVM habits and I'm only doing it
                // because I want the demos to be absolutely bitching

                // the control is allowed to cancel this event
                if (this.RaiseTabItemClosingEvent(tabItem))
                {
                    return;
                }

                if (this.ItemsSource == null)
                {
                    // if the list is hard-coded (i.e. has no ItemsSource)
                    // then we remove the item from the collection
                    tabItem.ClearStyle();
                    this.Items.Remove(tabItem);
                }
                else
                {
                    // if ItemsSource is something we cannot work with, bail out
                    var collection = this.ItemsSource as IList;
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
            }
        }
    }
}
