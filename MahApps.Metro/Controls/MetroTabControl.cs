using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    public class MetroTabControl : BaseMetroTabControl
    {
        public MetroTabControl()
            : base()
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
            this.Loaded += BaseMetroTabControl_Loaded;

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
            return new MetroTabItem() { OwningTabControl = this }; //Overrides the TabControl's default behavior and returns a MetroTabItem instead of a regular one.
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element != item)
                element.SetCurrentValue(MetroTabItem.DataContextProperty, item); //dont want to set the datacontext to itself.

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
            foreach (TabItemClosingEventHandler subHandler in TabItemClosingEvent.GetInvocationList())
            {
                TabItemClosingEventArgs args = new TabItemClosingEventArgs(closingItem);
                subHandler.Invoke(this, args);
                if (args.Cancel)
                    return true;
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
            private BaseMetroTabControl owner = null;
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
                    Tuple<object, MetroTabItem> paramData = (Tuple<object, MetroTabItem>)parameter;

                    if (owner.CloseTabCommand != null && !(paramData.Item1 is TextBlock)) //best way I could tell if the tabitem is from databinding or not.
                        owner.CloseTabCommand.Execute(paramData.Item1);
                    else
                    {
                        if (paramData.Item2 is MetroTabItem)
                        {
                            if (!owner.RaiseTabItemClosingEvent((MetroTabItem)paramData.Item2)) //Allows the user to cancel closing a tab.
                            {
                                var tabItem = (MetroTabItem)paramData.Item2;
                                owner.Items.Remove(tabItem);
                            }
                        }
                    }
                }
            }
        }
    }
}
