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

namespace MetroDemo
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class UserControl2 : MetroUserControl
    {
        public UserControl2()
        {
            InitializeComponent();
        }

        private void ShowTop_Click(object sender, RoutedEventArgs e)
        {
            var flyout = this.Flyouts.Items[0] as UserControlFlyout;
            if (flyout == null)
            {
                return;
            }

            flyout.IsOpen = !flyout.IsOpen;
        }

        private void ShowBottom_Click(object sender, RoutedEventArgs e)
        {
            var flyout = this.Flyouts.Items[1] as UserControlFlyout;
            if (flyout == null)
            {
                return;
            }

            flyout.IsOpen = !flyout.IsOpen;
        }
    }
}
