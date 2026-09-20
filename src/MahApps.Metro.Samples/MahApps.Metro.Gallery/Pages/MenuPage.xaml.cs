// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class MenuPage : UserControl
    {
        public MenuPage()
        {
            this.InitializeComponent();

            this.BarExample.Watch(this.Bar,
                                  Menu.IsMainMenuProperty,
                                  IsEnabledProperty,
                                  FlowDirectionProperty);
            this.BarExample.Watch("Layout", this.Bar, WidthProperty);

            this.ContextExample.Watch("Layout", this.Strip, WidthProperty, HeightProperty);
        }
    }
}
