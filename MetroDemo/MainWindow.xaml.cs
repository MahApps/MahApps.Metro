using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using MahApps.Metro;
using MahApps.Metro.Controls;

namespace MetroDemo
{
    public partial class MainWindow
    {
        private Theme currentTheme = Theme.Light;
        private Accent currentAccent = ThemeManager.DefaultAccents.First(x => x.Name == "Blue");

        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
            InitializeComponent();
            var t = new DispatcherTimer(TimeSpan.FromSeconds(2), DispatcherPriority.Normal, Tick, this.Dispatcher);
        }

        void Tick(object sender, EventArgs e)
        {
            var dateTime = DateTime.Now;
            transitioning.Content = new TextBlock {Text = "Transitioning Content! " + dateTime, SnapsToDevicePixels = true};
            customTransitioning.Content = new TextBlock {Text = "Custom transistion! " + dateTime, SnapsToDevicePixels = true};
        }

        private void ChangeAccent(string accentName)
        {
            this.currentAccent = ThemeManager.DefaultAccents.First(x => x.Name == accentName);

            ThemeManager.ChangeTheme(this, this.currentAccent, this.currentTheme);
        }

        private void AccentRed(object sender, RoutedEventArgs e)
        {
            this.ChangeAccent("Red");
        }

        private void AccentGreen(object sender, RoutedEventArgs e)
        {
            this.ChangeAccent("Green");
        }

        private void AccentBlue(object sender, RoutedEventArgs e)
        {
            this.ChangeAccent("Blue");
        }

        private void AccentPurple(object sender, RoutedEventArgs e)
        {
            this.ChangeAccent("Purple");
        }

        private void AccentOrange(object sender, RoutedEventArgs e)
        {
            this.ChangeAccent("Orange");
        }

        private void ThemeLight(object sender, RoutedEventArgs e)
        {
            this.currentTheme = Theme.Light;
            ThemeManager.ChangeTheme(this, this.currentAccent, Theme.Light);
        }

        private void ThemeDark(object sender, RoutedEventArgs e)
        {
            this.currentTheme = Theme.Dark;
            ThemeManager.ChangeTheme(this, this.currentAccent, Theme.Dark);
        }

        private void LaunchVisualStudioDemo(object sender, RoutedEventArgs e)
        {
            new VSDemo().Show();
        }

        private void LaunchFlyoutDemo(object sender, RoutedEventArgs e)
        {
            new FlyoutDemo().Show();
        }

        private void LaunchPanoramaDemo(object sender, RoutedEventArgs e)
        {
            new PanoramaDemo().Show();
        }
    }
}
