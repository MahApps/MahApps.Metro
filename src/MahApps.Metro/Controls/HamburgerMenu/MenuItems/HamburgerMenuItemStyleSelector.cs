using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class HamburgerMenuItemStyleSelector : StyleSelector
    {
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
                        var itemContainerStyle = this.IsItemOptions ? hamburgerMenu.ItemContainerStyle : hamburgerMenu.OptionsItemContainerStyle;
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