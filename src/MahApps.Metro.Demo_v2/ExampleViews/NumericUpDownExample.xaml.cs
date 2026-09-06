// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MahApps.Demo.Controls;
using MahApps.Metro.Controls;
using System.Windows.Controls;

namespace MahApps.Demo.ExampleViews
{
    /// <summary>
    /// Interaction logic for NumericUpDownExample.xaml
    /// </summary>
    public partial class NumericUpDownExample : UserControl
    {
        public NumericUpDownExample()
        {
            this.InitializeComponent();

            this.demoView.DemoProperties.Add(new DemoViewProperty(NumericUpDown.ValueProperty, this.numericUpDown, "NumericUpDown"));
            this.demoView.DemoProperties.Add(new DemoViewProperty(NumericUpDown.NumericInputModeProperty, this.numericUpDown));
            this.demoView.DemoProperties.Add(new DemoViewProperty(WidthProperty, this.numericUpDown));
            this.demoView.DemoProperties.Add(new DemoViewProperty(HeightProperty, this.numericUpDown));
            this.demoView.DemoProperties.Add(new DemoViewProperty(HorizontalAlignmentProperty, this.numericUpDown));
            this.demoView.DemoProperties.Add(new DemoViewProperty(VerticalAlignmentProperty, this.numericUpDown));
            this.demoView.DemoProperties.Add(new DemoViewProperty(NumericUpDown.StringFormatProperty, this.numericUpDown, "NumericUpDown"));
            this.demoView.DemoProperties.Add(new DemoViewProperty(NumericUpDown.IntervalProperty, this.numericUpDown, "NumericUpDown"));
            this.demoView.DemoProperties.Add(new DemoViewProperty(NumericUpDown.InterceptArrowKeysProperty, this.numericUpDown, "NumericUpDown"));
            this.demoView.DemoProperties.Add(new DemoViewProperty(NumericUpDown.InterceptManualEnterProperty, this.numericUpDown, "NumericUpDown"));
            this.demoView.DemoProperties.Add(new DemoViewProperty(NumericUpDown.InterceptMouseWheelProperty, this.numericUpDown, "NumericUpDown"));

            this.demoView.DemoProperties.Add(new DemoViewProperty(TextBoxHelper.ClearTextButtonProperty, this.numericUpDown, "Attached"));
        }
    }
}