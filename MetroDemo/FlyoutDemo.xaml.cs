using System.Windows;

namespace MetroDemo
{
    public partial class FlyoutDemo
    {
        public FlyoutDemo() {
            this.InitializeComponent();
        }

        private void ShowFirst(object sender, RoutedEventArgs e) {
            this.Flyouts[0].IsOpen = !this.Flyouts[0].IsOpen;
        }

        private void ShowSecond(object sender, RoutedEventArgs e) {
            this.Flyouts[1].IsOpen = !this.Flyouts[1].IsOpen;
        }

        private void ShowThird(object sender, RoutedEventArgs e) {
            this.Flyouts[2].IsOpen = !this.Flyouts[2].IsOpen;
        }

        private void ShowSettings(object sender, RoutedEventArgs e) {
            this.Flyouts[3].IsOpen = !this.Flyouts[3].IsOpen;
        }

        private void ShowLeft(object sender, RoutedEventArgs e) {
            this.Flyouts[4].IsOpen = !this.Flyouts[4].IsOpen;
        }

        private void ShowTop(object sender, RoutedEventArgs e) {
            this.Flyouts[5].IsOpen = !this.Flyouts[5].IsOpen;
        }

        private void ShowBottom(object sender, RoutedEventArgs e) {
            this.Flyouts[6].IsOpen = !this.Flyouts[6].IsOpen;
        }

        private void CloseMe(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
