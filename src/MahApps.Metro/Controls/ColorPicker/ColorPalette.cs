using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public class ColorPalette : ListBox
    {
        static ColorPalette()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPalette), new FrameworkPropertyMetadata(typeof(ColorPalette)));
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(object), typeof(ColorPalette), new PropertyMetadata(null));
        public static readonly DependencyProperty ColorNamesDictionaryProperty = DependencyProperty.Register(nameof(ColorNamesDictionary), typeof(Dictionary<Color?, string>), typeof(ColorPalette), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the Header of this Control
        /// </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }


        /// <summary>
        /// Gets or sets a custom dictionary for color to name. If null, the degault dictionary will be used.
        /// </summary>
        public Dictionary<Color?, string> ColorNamesDictionary
        {
            get { return (Dictionary<Color?, string>)GetValue(ColorNamesDictionaryProperty); }
            set { SetValue(ColorNamesDictionaryProperty, value); }
        }


        internal bool FocusSelectedItem()
        {
            ListBoxItem listBoxItem = null;
            if (SelectedIndex >= 0)
            {
                listBoxItem = (ListBoxItem)ItemContainerGenerator.ContainerFromIndex(SelectedIndex);
            }
            else if (Items.Count > 0)
            {
                listBoxItem = (ListBoxItem)ItemContainerGenerator.ContainerFromItem(Items[0]);
            }

            if (listBoxItem != null)
            {
                listBoxItem?.Focus();
                return true;
            }
            return false;
        }
    }
}
