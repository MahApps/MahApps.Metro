using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using MahApps.Metro;
using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Windows.Data;

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

            CollectionViewSource.GetDefaultView(groupingComboBox.ItemsSource).GroupDescriptions.Add(new PropertyGroupDescription("Artist"));
        }

        void Tick(object sender, EventArgs e)
        {
            var dateTime = DateTime.Now;
            transitioning.Content = new TextBlock {Text = "Transitioning Content! " + dateTime, SnapsToDevicePixels = true};
            customTransitioning.Content = new TextBlock {Text = "Custom transistion! " + dateTime, SnapsToDevicePixels = true};
        }

        private void ChangeAccent(Accent accent, Theme theme)
        {
            ThemeManager.ChangeTheme(Application.Current, accent, theme);
        }

        private void ThemeLight(object sender, RoutedEventArgs e)
        {
            this.currentTheme = Theme.Light;
            this.ChangeAccent(this.currentAccent, this.currentTheme);
        }

        private void ThemeDark(object sender, RoutedEventArgs e)
        {
            this.currentTheme = Theme.Dark;
            this.ChangeAccent(this.currentAccent, this.currentTheme);
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

        private void LaunchIcons(object sender, RoutedEventArgs e)
        {
            new IconsWindow().Show();
        }

        private void LauchCleanDemo(object sender, RoutedEventArgs e)
        {
            new CleanWindowDemo().Show();
        }

        private void LaunchRibbonDemo(object sender, RoutedEventArgs e)
        {
#if NET_4_5
            //new RibbonDemo().Show();
#else
            MessageBox.Show("Ribbon is only supported on .NET 4.5 or higher.");
#endif
        }

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var flipview = ((FlipView)sender);
            switch (flipview.SelectedIndex)
            {
                case 0:
                    flipview.BannerText = "Cupcakes!";
                    break;
                case 1:
                    flipview.BannerText = "Xbox!";
                    break;
                case 2:
                    flipview.BannerText = "Chess!";
                    break;
            }
        }

        private void MetroTabControl_TabItemClosingEvent(object sender, BaseMetroTabControl.TabItemClosingEventArgs e)
        {
            if (e.ClosingTabItem.Header.ToString().StartsWith("sizes"))
                e.Cancel = true;
        }

        private void InteropDemo(object sender, RoutedEventArgs e)
        {
            new InteropDemo().Show();

        }

        private void ChangeAccent(object sender, RoutedEventArgs e)
        {
            var accentName = (string)((MenuItem)sender).Header;
            this.currentAccent = ThemeManager.DefaultAccents.First(x => x.Name == accentName);
            this.ChangeAccent(this.currentAccent, this.currentTheme);
        }
    }
}
