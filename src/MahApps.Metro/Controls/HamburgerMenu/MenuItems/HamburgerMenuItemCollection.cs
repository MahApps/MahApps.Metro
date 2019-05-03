using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The HamburgerMenuItemCollection provides typed collection of HamburgerMenuItemBase.
    /// </summary>
    public class HamburgerMenuItemCollection : FreezableCollection<HamburgerMenuItemBase>
    {
        protected override Freezable CreateInstanceCore()
        {
            return new HamburgerMenuItemCollection();
        }
    }
}
