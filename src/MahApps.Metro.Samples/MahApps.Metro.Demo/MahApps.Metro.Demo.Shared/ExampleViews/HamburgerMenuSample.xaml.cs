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
            // instead using binding Content="{Binding RelativeSource={RelativeSource Self}, Mode=OneWay, Path=SelectedItem}"
            // we can do this
            HamburgerMenuControl.Content = e.ClickedItem;

            // close the menu if a item was selected
            if (this.HamburgerMenuControl.IsPaneOpen)
            {
                this.HamburgerMenuControl.IsPaneOpen = false;
            }
        }

        private async void HamburgerMenu_OnOptionsItemClick(object sender, ItemClickEventArgs e)
        {
            var menuItem = e.ClickedItem as HamburgerMenuItem;
            await this.TryFindParent<MetroWindow>().ShowMessageAsync("", $"You clicked on {menuItem.Label} button");
        }
    }
}
