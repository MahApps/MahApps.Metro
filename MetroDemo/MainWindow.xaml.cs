﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MahApps.Metro;
using MahApps.Metro.Controls;

namespace MetroDemo
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel(Dispatcher);
            InitializeComponent();
            var t = new DispatcherTimer(TimeSpan.FromSeconds(2), DispatcherPriority.Normal, Tick, this.Dispatcher);
        }

        void Tick(object sender, EventArgs e)
        {
            var dateTime = DateTime.Now;
            transitioning.Content = new TextBlock {Text = "DateTime " + dateTime, SnapsToDevicePixels = true};
            customTransitioning.Content = new TextBlock {Text = "Custom transistion " + dateTime.Second, SnapsToDevicePixels = true};
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var x = pivot.Items;
            pb.IsIndeterminate = !pb.IsIndeterminate;
            Flyouts[0].IsOpen = !Flyouts[0].IsOpen;
            Flyouts[1].IsOpen = !Flyouts[1].IsOpen;
        }

        private void MiLightRed(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Red"), Theme.Light);
        }

        private void MiDarkRed(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Red"), Theme.Dark);
        }

        private void MiLightGreen(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Green"), Theme.Light);
        }

        private void MiDarkGreen(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Green"), Theme.Dark);
        }

        private void MiLightBlue(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Blue"), Theme.Light);
        }

        private void MiDarkBlue(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Blue"), Theme.Dark);
        }

        private void MiLightPurple(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Purple"), Theme.Light);
        }

        private void MiDarkPurple(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Purple"), Theme.Dark);
        }

        private void BtnPanoramaClick(object sender, RoutedEventArgs e)
        {
            //new ChildWindow().ShowDialog();
            new PanoramaDemo().Show();
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //pivot.GoToItem(pi3);
            ((MainWindowViewModel) this.DataContext).SelectedIndex = 2;
        }

        private void BtnVSClick(object sender, RoutedEventArgs e)
        {
            new VSDemo().Show();
        }

        private void BtnIconsClick(object sender, RoutedEventArgs e)
        {
            new IconsWindow().Show();
        }

        private void MiDarkOrange(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Orange"), Theme.Dark);
        }

        private void MiLightOrange(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Orange"), Theme.Light);
        }

        private void IgnoreTaskbarOnMaximizedClick(object sender, RoutedEventArgs e)
        {
            this.IgnoreTaskbarOnMaximize = !this.IgnoreTaskbarOnMaximize;
        }
    }
}
