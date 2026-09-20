// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Gallery.Windows;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for MetroNavigationWindowPage.xaml
    /// </summary>
    public partial class MetroNavigationWindowPage : UserControl
    {
        public MetroNavigationWindowPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The window is put together here rather than in a file of its own, because
        /// MetroNavigationWindow is written in XAML itself and a type that is cannot be the root
        /// of another XAML file. The pages it shows are ordinary markup.
        /// </summary>
        private void OnOpenWindow(object sender, RoutedEventArgs e)
        {
            var window = new MetroNavigationWindow
                         {
                             Title = "A window that keeps a journal",
                             Width = 640,
                             Height = 420,
                             ShowHomeButton = true,
                             WindowStartupLocation = WindowStartupLocation.CenterOwner,
                             Owner = Window.GetWindow(this)
                         };

            window.Show();
            window.Navigate(new NavigationSampleFirstPage());
        }
    }
}
