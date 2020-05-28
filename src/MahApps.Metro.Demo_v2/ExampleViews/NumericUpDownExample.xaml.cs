using MahApps.Demo.Controls;
using MahApps.Metro.Controls;
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
    /// Interaction logic for NumericUpDownExample.xaml
    /// </summary>
    public partial class NumericUpDownExample : UserControl
    {
        public NumericUpDownExample()
        {
            InitializeComponent();

            demoView.DemoProperties.Add(new DemoViewProperty(NumericUpDown.ValueProperty, numericUpDown, "NumericUpDown"));
            demoView.DemoProperties.Add(new DemoViewProperty(NumericUpDown.NumericInputModeProperty, numericUpDown));
            demoView.DemoProperties.Add(new DemoViewProperty(NumericUpDown.WidthProperty, numericUpDown));
            demoView.DemoProperties.Add(new DemoViewProperty(NumericUpDown.HeightProperty, numericUpDown));
            demoView.DemoProperties.Add(new DemoViewProperty(NumericUpDown.HorizontalAlignmentProperty, numericUpDown));
            demoView.DemoProperties.Add(new DemoViewProperty(NumericUpDown.VerticalAlignmentProperty, numericUpDown));
            demoView.DemoProperties.Add(new DemoViewProperty(NumericUpDown.StringFormatProperty, numericUpDown, "NumericUpDown"));
            demoView.DemoProperties.Add(new DemoViewProperty(NumericUpDown.IntervalProperty, numericUpDown, "NumericUpDown"));
            demoView.DemoProperties.Add(new DemoViewProperty(NumericUpDown.InterceptArrowKeysProperty, numericUpDown, "NumericUpDown"));
            demoView.DemoProperties.Add(new DemoViewProperty(NumericUpDown.InterceptManualEnterProperty, numericUpDown, "NumericUpDown"));
            demoView.DemoProperties.Add(new DemoViewProperty(NumericUpDown.InterceptMouseWheelProperty, numericUpDown, "NumericUpDown"));

            demoView.DemoProperties.Add(new DemoViewProperty(TextBoxHelper.ClearTextButtonProperty, numericUpDown, "Attached"));

        }
    }
}
