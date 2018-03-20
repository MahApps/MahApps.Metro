using System.Windows.Controls;

namespace MetroDemo.ExampleViews
{
    using MahApps.Metro.Controls;

    public sealed partial class HamburgerMenuCreatorsUpdate : UserControl
    {
        public HamburgerMenuCreatorsUpdate()
        {
            this.InitializeComponent();
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            HamburgerMenuControl.Content = e.InvokedItem;
        }
    }
}