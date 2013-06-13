using System.Windows;

namespace MetroDemo
{
    using MahApps.Metro.Controls;

    public partial class FlyoutDemo
    {
        public FlyoutDemo() {
            this.InitializeComponent();
        }

        private void ShowFirst(object sender, RoutedEventArgs e)
        {
            ToggleFlyout(0);
        }

        private void ShowSecond(object sender, RoutedEventArgs e) {
            ToggleFlyout(1);
        }

        private void ShowThird(object sender, RoutedEventArgs e) {
            ToggleFlyout(2);
        }

        private void ShowSettings(object sender, RoutedEventArgs e) {
            ToggleFlyout(3);
        }

        private void ShowLeft(object sender, RoutedEventArgs e) {
            ToggleFlyout(4);
        }

        private void ShowTop(object sender, RoutedEventArgs e) {
            ToggleFlyout(5);
        }

        private void ShowBottom(object sender, RoutedEventArgs e) {
            ToggleFlyout(6);
        }

        private void CloseMe(object sender, RoutedEventArgs e) {
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
    }
}
