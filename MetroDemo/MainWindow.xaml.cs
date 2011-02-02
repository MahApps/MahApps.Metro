using System;
using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace MetroDemo
{
    public partial class MainWindow
    {
        public MainWindow()
        {
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
    }
}
