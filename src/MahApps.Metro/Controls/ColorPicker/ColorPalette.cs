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

        /// <summary>
        /// Gets or sets the Header of this Control
        /// </summary>
        public object Header
        {
            get => (object)this.GetValue(HeaderProperty);
            set => this.SetValue(HeaderProperty, value);
        }

        /// <summary>Identifies the <see cref="ColorNamesDictionary"/> dependency property.</summary>
        public static readonly DependencyProperty ColorNamesDictionaryProperty = DependencyProperty.Register(nameof(ColorNamesDictionary), typeof(Dictionary<Color?, string>), typeof(ColorPalette), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the HeaderTemplate of this Control
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)this.GetValue(HeaderTemplateProperty);
            set => this.SetValue(HeaderTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="HeaderTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(ColorPalette), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a custom dictionary for color to name. If null, the default dictionary will be used.
        /// </summary>
        public Dictionary<Color?, string> ColorNamesDictionary
        {
            get => (Dictionary<Color?, string>)this.GetValue(ColorNamesDictionaryProperty);
            set => this.SetValue(ColorNamesDictionaryProperty, value);
        }

        internal bool FocusSelectedItem()
        {
            ListBoxItem listBoxItem = null;
            if (this.SelectedIndex >= 0)
            {
                listBoxItem = (ListBoxItem)this.ItemContainerGenerator.ContainerFromIndex(this.SelectedIndex);
            }
            else if (this.Items.Count > 0)
            {
                listBoxItem = (ListBoxItem)this.ItemContainerGenerator.ContainerFromItem(this.Items[0]);
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