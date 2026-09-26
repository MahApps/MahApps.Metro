// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Gallery.Windows;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for MetroWindowPage.xaml
    /// </summary>
    public partial class MetroWindowPage : UserControl
    {
        public MetroWindowPage()
        {
            this.InitializeComponent();
        }

        private void OnOpenWindow(object sender, RoutedEventArgs e)
        {
            var window = new SampleWindow { Owner = Window.GetWindow(this) };

            window.Show();
        }

        private void OnOpenBackdropWindow(object sender, RoutedEventArgs e)
        {
            var window = new BackdropWindow { Owner = Window.GetWindow(this) };

            window.Show();
        }
    }
}
