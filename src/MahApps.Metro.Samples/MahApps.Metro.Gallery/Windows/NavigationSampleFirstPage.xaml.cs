// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Windows
{
    /// <summary>
    /// Interaction logic for NavigationSampleFirstPage.xaml
    /// </summary>
    public partial class NavigationSampleFirstPage : Page
    {
        public NavigationSampleFirstPage()
        {
            this.InitializeComponent();
        }

        private void OnNext(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new NavigationSampleSecondPage());
        }

        /// <summary>
        /// What the content property of the window puts on the screen, which is not the page but a
        /// layer over every page the frame shows.
        /// </summary>
        private void OnOverlay(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is not MetroNavigationWindow window)
            {
                return;
            }

            window.OverlayContent = window.OverlayContent is null
                ? new Border
                  {
                      Background = new SolidColorBrush(Color.FromArgb(0x99, 0, 0, 0)),
                      Child = new TextBlock
                              {
                                  Text = "42 percent done",
                                  FontSize = 20,
                                  Foreground = Brushes.White,
                                  HorizontalAlignment = HorizontalAlignment.Center,
                                  VerticalAlignment = VerticalAlignment.Center
                              }
                  }
                : null;
        }
    }
}
