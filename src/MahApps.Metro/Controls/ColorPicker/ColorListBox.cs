using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public class ColorListBox : ListBox
    {
        static ColorListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorListBox), new FrameworkPropertyMetadata(typeof(ColorListBox)));
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(ColorListBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the Header of this Control
        /// </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

    }
}
