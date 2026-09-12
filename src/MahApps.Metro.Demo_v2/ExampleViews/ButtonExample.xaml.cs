// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MahApps.Demo.Controls;
using MahApps.Metro.Demo_v2;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Demo.ExampleViews
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ButtonExample : UserControl
    {
        public ButtonExample()
        {
            this.InitializeComponent();

            this.demoView.DemoProperties.Add(new DemoViewProperty(ContentProperty, this.button));
            this.demoView.DemoProperties.Add(new DemoViewProperty(HorizontalAlignmentProperty, this.button));
            this.demoView.DemoProperties.Add(new DemoViewProperty(VerticalAlignmentProperty, this.button));
            this.demoView.DemoProperties.Add(new DemoViewProperty(WidthProperty, this.button));
            this.demoView.DemoProperties.Add(new DemoViewProperty(HeightProperty, this.button));

            var styleProperty = new DemoViewProperty(StyleProperty, this.button);
            styleProperty.ItemSource = new Dictionary<string, Style>()
                                       {
                                           { "MahApps.Styles.Button", Application.Current.Resources["MahApps.Styles.Button"] as Style },
                                           { "MahApps.Styles.Button.Circle", Application.Current.Resources["MahApps.Styles.Button.Circle"] as Style }
                                       };
            this.demoView.DemoProperties.Add(styleProperty);
        }
    }
}