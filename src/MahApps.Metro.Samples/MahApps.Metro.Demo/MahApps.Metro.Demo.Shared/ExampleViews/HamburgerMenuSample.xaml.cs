namespace MetroDemo.ExampleViews
{
    using System.Windows.Controls;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;

    public sealed partial class HamburgerMenuSample : UserControl
    {
        public HamburgerMenuSample()
        {
            this.InitializeComponent();
        }

        private void HamburgerMenu_OnItemClick(object sender, ItemClickEventArgs e)
        {
            HamburgerMenuControl.Content = e.ClickedItem;
        }

        private async void HamburgerMenu_OnOptionsItemClick(object sender, ItemClickEventArgs e)
        {
            var menuItem = e.ClickedItem as HamburgerMenuItem;
            await this.TryFindParent<MetroWindow>().ShowMessageAsync("", $"You clicked on {menuItem.Label} button");
        }
    }
}
