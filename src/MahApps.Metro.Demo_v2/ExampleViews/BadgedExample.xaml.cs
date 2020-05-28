using MahApps.Demo.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Demo_v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MahApps.Demo.ExampleViews
{
    /// <summary>
    /// Interaction logic for BadgedExample.xaml
    /// </summary>
    public partial class BadgedExample : UserControl
    {
        public BadgedExample()
        {
            InitializeComponent();

            demoView.DemoProperties.Add(new DemoViewProperty(Badged.BadgeProperty, badge, "Badged"));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            badge.SetCurrentValue(ControlzEx.BadgedEx.BadgeProperty, (badge.Badge as int? ?? 0) + 1);
        }

        private void Button_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            badge.SetCurrentValue(ControlzEx.BadgedEx.BadgeProperty, null);
        }
    }
}
