using System.Windows;
using MahApps.Metro.Controls;

namespace MetroDemo.ExampleWindows
{
    public partial class FlyoutDemo
    {
        public FlyoutDemo()
        {
            this.DataContext = new MainWindowViewModel();
            this.InitializeComponent();
        }

        private void ShowFirst(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(0);
        }

        private void ShowSecond(object sender, RoutedEventArgs e) {
            this.ToggleFlyout(1);
        }

        private void ShowThird(object sender, RoutedEventArgs e) {
            this.ToggleFlyout(2);
        }

        private void ShowAccent(object sender, RoutedEventArgs e) {
            this.ToggleFlyout(3);
        }

        private void ShowInverse(object sender, RoutedEventArgs e) {
            this.ToggleFlyout(4);
        }

        private void ShowAdapt(object sender, RoutedEventArgs e)  {
            this.ToggleFlyout(5);
        }

        private void ShowSettings(object sender, RoutedEventArgs e) {
            this.ToggleFlyout(6);
        }

        private void ShowLeft(object sender, RoutedEventArgs e) {
            this.ToggleFlyout(7);
        }

        private void ShowTop(object sender, RoutedEventArgs e) {
            this.ToggleFlyout(8);
        }

        private void ShowBottom(object sender, RoutedEventArgs e) {
            this.ToggleFlyout(9);
        }

        private void ShowModal(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(10);
        }

        private void CloseMe(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ToggleFlyout(int index)
        {
            var flyout = this.Flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }

            flyout.IsOpen = !flyout.IsOpen;
        }

        private void ShowSettingsLeft(object sender, RoutedEventArgs e)
        {
            var flyout = (Flyout)this.Flyouts.Items[6];
            flyout.Position = Position.Left;
        }

        private void ShowSettingsRight(object sender, RoutedEventArgs e)
        {
            var flyout = (Flyout)this.Flyouts.Items[6];
            flyout.Position = Position.Right;
        }
    }
}
