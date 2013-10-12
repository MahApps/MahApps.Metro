using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace MetroDemo.Navigation
{
    /// <summary>
    /// Interaction logic for NavigationWindowDemo.xaml
    /// </summary>
    public partial class NavigationWindowDemo : MetroNavigationWindow
    {
        public NavigationWindowDemo()
        {
            InitializeComponent();
        }

        private void MetroNavigationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //very very very bad way to replicate 'await Task.Delay(2000)'
            System.Threading.Tasks.Task.Factory.StartNew(() => System.Threading.Thread.Sleep(2000)).ContinueWith(x =>
                Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(() =>
            Navigate(new Uri("/Navigation/NavigationDemoPage1.xaml", UriKind.Relative)))));
        }
    }
}
