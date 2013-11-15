using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using MahApps.Metro.Controls;

namespace MetroDemo.Navigation
{
    public class NavigationDemo : MetroNavigationWindow
    {
        public NavigationDemo()
        {
            InitializeComponent();

            this.Loaded += NavigationDemo_Loaded;
            this.Unloaded += NavigationDemo_Unloaded;
        }

        void NavigationDemo_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= NavigationDemo_Loaded;
            this.Unloaded -= NavigationDemo_Unloaded;
        }

        void NavigationDemo_Loaded(object sender, RoutedEventArgs e)
        {
            this.Navigate(new HomePage());
        }
    }
}
