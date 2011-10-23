using System;
using System.Collections;
using System.Windows;
using System.Windows.Input;

namespace MetroDemo
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel(Dispatcher);
            InitializeComponent();
        }

        private void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed)
                DragMove();
        }

        private void BtnMinClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnMaxClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }


        private void MiDarkRed(object sender, RoutedEventArgs e)
        {
            var redRd = new ResourceDictionary();
            var darkRd = new ResourceDictionary();

            redRd.Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Red.xaml");
            darkRd.Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml");


            ApplyResourceDictionary(redRd);
            ApplyResourceDictionary(darkRd);
            

        }

        private void MiLightGreen(object sender, RoutedEventArgs e)
        {
            var greenRd = new ResourceDictionary();
            var lightRd = new ResourceDictionary();

            greenRd.Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Green.xaml");
            lightRd.Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml");


            ApplyResourceDictionary(greenRd);
            ApplyResourceDictionary(lightRd);
        }

        private void ApplyResourceDictionary(ResourceDictionary rd)
        {
            foreach (DictionaryEntry r in rd)
            {
                if (Resources.Contains(r.Key))
                    Resources.Remove(r.Key);

                Resources.Add(r.Key, r.Value);
            }
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            pb.IsIndeterminate = !pb.IsIndeterminate;
        }

        private void MiLightRed(object sender, RoutedEventArgs e)
        {
            var accentRd = new ResourceDictionary();
            var themeRd = new ResourceDictionary();

            accentRd.Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Red.xaml");
            themeRd.Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml");


            ApplyResourceDictionary(accentRd);
            ApplyResourceDictionary(themeRd);
        }

        private void MiLightBlue(object sender, RoutedEventArgs e)
        {
            var accentRd = new ResourceDictionary();
            var themeRd = new ResourceDictionary();

            accentRd.Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml");
            themeRd.Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml");


            ApplyResourceDictionary(accentRd);
            ApplyResourceDictionary(themeRd);
        }

        private void MiLightPurple(object sender, RoutedEventArgs e)
        {
            var accentRd = new ResourceDictionary();
            var themeRd = new ResourceDictionary();

            accentRd.Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Purple.xaml");
            themeRd.Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml");


            ApplyResourceDictionary(accentRd);
            ApplyResourceDictionary(themeRd);
        }

        private void MiDarkBlue(object sender, RoutedEventArgs e)
        {
            var accentRd = new ResourceDictionary();
            var themeRd = new ResourceDictionary();

            accentRd.Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml");
            themeRd.Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml");


            ApplyResourceDictionary(accentRd);
            ApplyResourceDictionary(themeRd);
        }

        private void MiDarkGreen(object sender, RoutedEventArgs e)
        {
            var accentRd = new ResourceDictionary();
            var themeRd = new ResourceDictionary();

            accentRd.Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Green.xaml");
            themeRd.Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml");


            ApplyResourceDictionary(accentRd);
            ApplyResourceDictionary(themeRd);
        }

        private void MiDarkPurple(object sender, RoutedEventArgs e)
        {
            var accentRd = new ResourceDictionary();
            var themeRd = new ResourceDictionary();

            accentRd.Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Purple.xaml");
            themeRd.Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml");


            ApplyResourceDictionary(accentRd);
            ApplyResourceDictionary(themeRd);
        }

        private void BtnPanoramaClick(object sender, RoutedEventArgs e)
        {
            new PanoramaDemo().Show();
        }
    }
}
