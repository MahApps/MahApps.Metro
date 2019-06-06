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
                    if (item is HamburgerMenuSeparatorItem)
                    {
                        if (hamburgerMenu.SeparatorItemContainerStyle != null)
                        {
                            return hamburgerMenu.SeparatorItemContainerStyle;
                        }
                    }
                    else if (item is HamburgerMenuItem)
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