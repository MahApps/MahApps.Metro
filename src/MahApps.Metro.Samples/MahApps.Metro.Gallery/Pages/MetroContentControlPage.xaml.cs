// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for MetroContentControlPage.xaml
    /// </summary>
    public partial class MetroContentControlPage : UserControl
    {
        private int page = 1;

        public MetroContentControlPage()
        {
            this.InitializeComponent();

            this.EntranceExample.Watch(this.Entrance,
                                       MetroContentControl.TransitionsEnabledProperty,
                                       MetroContentControl.ReverseTransitionProperty,
                                       MetroContentControl.OnlyLoadTransitionProperty,
                                       MetroContentControl.IsTransitioningProperty);

            this.ContentExample.Watch("The one above", this.Swapped, ContentControl.ContentProperty);
            this.ContentExample.Watch("The one below", this.Animated, TransitioningContentControl.TransitionProperty);
        }

        /// <summary>
        /// The entrance runs on load and on becoming visible, so playing it in between takes an ask.
        /// </summary>
        private void OnReload(object sender, RoutedEventArgs e)
        {
            this.Entrance.Reload();
        }

        /// <summary>
        /// The same new content for both, which only one of them makes anything of.
        /// </summary>
        private void OnNext(object sender, RoutedEventArgs e)
        {
            this.page = (this.page % 3) + 1;

            var text = $"Page {this.page} of 3";

            this.Swapped.SetCurrentValue(ContentProperty, text);
            this.Animated.SetCurrentValue(ContentProperty, text);
        }
    }
}
