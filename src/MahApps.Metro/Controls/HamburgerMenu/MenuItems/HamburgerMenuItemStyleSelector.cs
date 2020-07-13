// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class HamburgerMenuItemStyleSelector : StyleSelector
    {
        /// <summary>
        /// Gets or sets a value indicating whether which item container style will be used for the HamburgerMenuItem.
        /// </summary>
        public bool IsItemOptions { get; set; }

        /// <inheritdoc />
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (container != null)
            {
                var listBox = ItemsControl.ItemsControlFromItemContainer(container);
                var hamburgerMenu = listBox?.TryFindParent<HamburgerMenu>();
                if (hamburgerMenu != null)
                {
                    if (item is IHamburgerMenuHeaderItem)
                    {
                        if (hamburgerMenu.HeaderItemContainerStyle != null)
                        {
                            return hamburgerMenu.HeaderItemContainerStyle;
                        }
                    }
                    else if (item is IHamburgerMenuSeparatorItem)
                    {
                        if (hamburgerMenu.SeparatorItemContainerStyle != null)
                        {
                            return hamburgerMenu.SeparatorItemContainerStyle;
                        }
                    }
                    else
                    {
                        var itemContainerStyle = this.IsItemOptions ? hamburgerMenu.OptionsItemContainerStyle : hamburgerMenu.ItemContainerStyle;
                        if (itemContainerStyle != null)
                        {
                            return itemContainerStyle;
                        }
                    }
                }
            }

            return base.SelectStyle(item, container);
        }
    }
}