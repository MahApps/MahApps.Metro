using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    public class MetroAnimatedSingleRowTabControl : TabControl
    {
        public MetroAnimatedSingleRowTabControl()
        {
            DefaultStyleKey = typeof(MetroAnimatedSingleRowTabControl);
        }
        
        public Thickness TabStripMargin
        {
            get { return (Thickness)GetValue(TabStripMarginProperty); }
            set { SetValue(TabStripMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabStripMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabStripMarginProperty =
            DependencyProperty.Register("TabStripMargin", typeof(Thickness), typeof(MetroAnimatedSingleRowTabControl), new PropertyMetadata(new Thickness(0)));

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MetroTabItem(); //Overrides the TabControl's default behavior and returns a MetroTabItem instead of a regular one.
        }

        public ICommand CloseTabCommand
        {
            get { return (ICommand)GetValue(CloseTabCommandProperty); }
            set { SetValue(CloseTabCommandProperty, value); }
        }

        public static readonly DependencyProperty CloseTabCommandProperty =
            DependencyProperty.Register("CloseTabCommand", typeof(ICommand), typeof(MetroAnimatedSingleRowTabControl), new PropertyMetadata(new MahApps.Metro.Controls.MetroTabControl.DefaultCloseTabCommand()));
    }
}
