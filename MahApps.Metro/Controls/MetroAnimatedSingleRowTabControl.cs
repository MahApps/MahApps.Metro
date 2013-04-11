using System.Windows;
using System.Windows.Controls;

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
    }
}
