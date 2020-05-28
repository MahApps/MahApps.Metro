using MahApps.Demo.Controls;
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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ButtonExample : UserControl
    {
        public ButtonExample()
        {
            InitializeComponent();

            demoView.DemoProperties.Add(new DemoViewProperty(Button.ContentProperty, button));
            demoView.DemoProperties.Add(new DemoViewProperty(Button.HorizontalAlignmentProperty, button));
            demoView.DemoProperties.Add(new DemoViewProperty(Button.VerticalAlignmentProperty, button));
            demoView.DemoProperties.Add(new DemoViewProperty(Button.WidthProperty, button));
            demoView.DemoProperties.Add(new DemoViewProperty(Button.HeightProperty, button));

            var styleProperty = new DemoViewProperty(Button.StyleProperty, button);
            styleProperty.ItemSource = new Dictionary<string, Style>()
            {
                {"MahApps.Styles.Button", App.Current.Resources["MahApps.Styles.Button"] as Style},
                {"MahApps.Styles.Button.Circle", App.Current.Resources["MahApps.Styles.Button.Circle"] as Style}
            };
            demoView.DemoProperties.Add(styleProperty);
        }
    }
}
