using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class MetroTabControl : TabControl
    {
        public Thickness TabStripMargin
        {
            get { return (Thickness)GetValue(TabStripMarginProperty); }
            set { SetValue(TabStripMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabStripMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabStripMarginProperty =
            DependencyProperty.Register("TabStripMargin", typeof(Thickness), typeof(MetroTabControl), new PropertyMetadata(new Thickness(0)));
        

        public MetroTabControl()
        {
            DefaultStyleKey = typeof(MetroTabControl);
        }
    }
}
