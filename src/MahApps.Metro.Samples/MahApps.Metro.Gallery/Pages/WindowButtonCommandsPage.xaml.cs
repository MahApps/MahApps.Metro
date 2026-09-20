// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Gallery.Windows;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for WindowButtonCommandsPage.xaml
    /// </summary>
    public partial class WindowButtonCommandsPage : UserControl
    {
        public WindowButtonCommandsPage()
        {
            this.InitializeComponent();
        }

        private void OnOpenWindow(object sender, RoutedEventArgs e)
        {
            var window = new TitleBarWindow { Owner = Window.GetWindow(this) };

            window.Show();
        }
    }
}
