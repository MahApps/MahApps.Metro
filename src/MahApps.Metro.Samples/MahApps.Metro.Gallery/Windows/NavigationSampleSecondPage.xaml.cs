// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Gallery.Windows
{
    /// <summary>
    /// Interaction logic for NavigationSampleSecondPage.xaml
    /// </summary>
    public partial class NavigationSampleSecondPage : Page
    {
        public NavigationSampleSecondPage()
        {
            this.InitializeComponent();
        }

        private void OnBack(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService?.CanGoBack == true)
            {
                this.NavigationService.GoBack();
            }
        }
    }
}
