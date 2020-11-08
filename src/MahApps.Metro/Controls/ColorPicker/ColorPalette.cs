// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
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

        /// <summary>Identifies the <see cref="Header"/> dependency property.</summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(object), typeof(ColorPalette), new PropertyMetadata(null));
        
        /// <summary>Identifies the <see cref="ColorNamesDictionary"/> dependency property.</summary>
        public static readonly DependencyProperty ColorNamesDictionaryProperty = DependencyProperty.Register(nameof(ColorNamesDictionary), typeof(Dictionary<Color?, string>), typeof(ColorPalette), new PropertyMetadata(null));
     
        /// <summary>Identifies the <see cref="HeaderTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(ColorPalette), new PropertyMetadata(null));


        /// <summary>
        /// Gets or Sets the Header of this Control
        /// </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }


        /// <summary>
        /// Gets or Sets the HeaderTemplate of this Control
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
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

            if (!(listBoxItem is null))
            {
                listBoxItem.Focus();
                return true;
            }
            return false;
        }
    }
}
