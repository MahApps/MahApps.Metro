using System.Windows;

namespace FlyoutDemo
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void ShowFirst(object sender, RoutedEventArgs e)
        {
            Flyouts[0].IsOpen = !Flyouts[0].IsOpen;
        }

        void ShowSecond(object sender, RoutedEventArgs e)
        {
            Flyouts[1].IsOpen = !Flyouts[1].IsOpen;
        }

        void ShowThird(object sender, RoutedEventArgs e)
        {
            Flyouts[2].IsOpen = !Flyouts[2].IsOpen;
        }
    }
}
