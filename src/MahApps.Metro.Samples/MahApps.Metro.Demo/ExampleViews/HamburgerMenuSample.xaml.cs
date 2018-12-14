using System.Windows.Controls;

namespace MetroDemo.ExampleViews
{
    using System.Windows.Data;

    public sealed partial class HamburgerMenuSample : UserControl
    {
        public HamburgerMenuSample()
        {
            this.InitializeComponent();

            this.Loaded += HamburgerMenuSample_Loaded;
        }

        private void HamburgerMenuSample_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Loaded -= HamburgerMenuSample_Loaded;
            this.HamburgerTabControl.Items.Add(new TabItem() { Header = "Ripple Effect", Content = new HamburgerMenuRipple() { DataContext = new Binding() } });
        }
    }
}