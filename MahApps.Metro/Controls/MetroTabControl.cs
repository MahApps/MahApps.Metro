using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    public class MetroTabControl : BaseMetroTabControl
    {
        public MetroTabControl():base()
        {
            DefaultStyleKey = typeof(MetroTabControl);         
        }
    }

    public abstract class BaseMetroTabControl: TabControl
    {
        public BaseMetroTabControl()
        {
            InternalCloseTabCommand = new DefaultCloseTabCommand(this);
        }

        public Thickness TabStripMargin
        {
            get { return (Thickness)GetValue(TabStripMarginProperty); }
            set { SetValue(TabStripMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabStripMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabStripMarginProperty =
            DependencyProperty.Register("TabStripMargin", typeof(Thickness), typeof(BaseMetroTabControl), new PropertyMetadata(new Thickness(0)));

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MetroTabItem() { OwningTabControl = this }; //Overrides the TabControl's default behavior and returns a MetroTabItem instead of a regular one.
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            element.SetCurrentValue(MetroTabItem.DataContextProperty, item);
            base.PrepareContainerForItemOverride(element, item);
        }

        public ICommand CloseTabCommand
        {
            get { return (ICommand)GetValue(CloseTabCommandProperty); }
            set { SetValue(CloseTabCommandProperty, value); }
        }

        public static readonly DependencyProperty CloseTabCommandProperty =
            DependencyProperty.Register("CloseTabCommand", typeof(ICommand), typeof(BaseMetroTabControl), new PropertyMetadata(null));

        private ICommand InternalCloseTabCommand
        {
            get { return (ICommand)GetValue(InternalCloseTabCommandProperty); }
            set { SetValue(InternalCloseTabCommandProperty, value); }
        }
        private static readonly DependencyProperty InternalCloseTabCommandProperty =
            DependencyProperty.Register("InternalCloseTabCommand", typeof(ICommand), typeof(BaseMetroTabControl), new PropertyMetadata(null));


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
                    if (owner.CloseTabCommand != null)
                        owner.CloseTabCommand.Execute(parameter);
                    else
                    {
                        if (parameter is MetroTabItem)
                        {
                            var tabItem = (MetroTabItem)parameter;
                            owner.Items.Remove(tabItem);
                        }
                    }
                }
            }
        }
    }
}
