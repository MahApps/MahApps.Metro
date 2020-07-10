// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MetroDemo.ExampleWindows
{
    /// <summary>
    /// Interaction logic for DynamicFlyout.xaml
    /// </summary>
    public partial class DynamicFlyout
    {
        public DynamicFlyout()
        {
            this.InitializeComponent();
        }

        private void CloseFlyoutClick(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
        }
    }
}