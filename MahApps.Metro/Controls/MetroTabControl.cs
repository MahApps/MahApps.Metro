using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    public class MetroTabControl : TabControl
    {
        public MetroTabControl()
        {
            DefaultStyleKey = typeof(MetroTabControl);
        }

        public Thickness TabStripMargin
        {
            get { return (Thickness)GetValue(TabStripMarginProperty); }
            set { SetValue(TabStripMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabStripMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabStripMarginProperty =
            DependencyProperty.Register("TabStripMargin", typeof(Thickness), typeof(MetroTabControl), new PropertyMetadata(new Thickness(0)));

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MetroTabItem() { OwningTabControl = this }; //Overrides the TabControl's default behavior and returns a MetroTabItem instead of a regular one.
        }

        public ICommand CloseTabCommand
        {
            get { return (ICommand)GetValue(CloseTabCommandProperty); }
            set { SetValue(CloseTabCommandProperty, value); }
        }

        public static readonly DependencyProperty CloseTabCommandProperty =
            DependencyProperty.Register("CloseTabCommand", typeof(ICommand), typeof(MetroTabControl), new PropertyMetadata(new DefaultCloseTabCommand()));

        internal class DefaultCloseTabCommand : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event System.EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                if (parameter != null && parameter is MetroTabItem)
                {
                    var tabItem = (MetroTabItem)parameter;

                    ((TabControl)tabItem.OwningTabControl).Items.Remove(tabItem);
                }
            }
        }
    }
}
