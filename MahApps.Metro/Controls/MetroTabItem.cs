using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class MetroTabItem : TabItem
    {
        public Double HeaderFontSize
        {
            get { return (Double)GetValue(HeaderFontSizeProperty); }
            set { SetValue(HeaderFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderFontSizeProperty =
            DependencyProperty.Register("HeaderFontSize", typeof(Double), typeof(MetroTabItem), new PropertyMetadata(26.67));

        

        public MetroTabItem()
        {
            DefaultStyleKey = typeof(MetroTabItem);
        }
    }
}
