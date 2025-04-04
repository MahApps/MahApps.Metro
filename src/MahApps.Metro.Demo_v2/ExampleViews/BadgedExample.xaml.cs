// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MahApps.Demo.Controls;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ControlzEx;

namespace MahApps.Demo.ExampleViews
{
    /// <summary>
    /// Interaction logic for BadgedExample.xaml
    /// </summary>
    public partial class BadgedExample : UserControl
    {
        public BadgedExample()
        {
            this.InitializeComponent();

            this.demoView.DemoProperties.Add(new DemoViewProperty(BadgedEx.BadgeProperty, this.badge, "Badged"));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.badge.SetCurrentValue(ControlzEx.BadgedEx.BadgeProperty, (this.badge.Badge as int? ?? 0) + 1);
        }

        private void Button_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.badge.SetCurrentValue(ControlzEx.BadgedEx.BadgeProperty, null);
        }
    }
}