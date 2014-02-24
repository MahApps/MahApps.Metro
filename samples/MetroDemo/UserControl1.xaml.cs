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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : MetroUserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        private void ShowRightFlyout_Click(object sender, RoutedEventArgs e)
        {
            var flyout = this.Flyouts.Items[0] as UserControlFlyout;
            if (flyout == null)
            {
                return;
            }

            flyout.IsOpen = !flyout.IsOpen;
        }

        private void ShowLeftFlyout_Click(object sender, RoutedEventArgs e)
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
