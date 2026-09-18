// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for TransitioningContentControlPage.xaml
    /// </summary>
    public partial class TransitioningContentControlPage : UserControl
    {
        private int page = 1;

        public TransitioningContentControlPage()
        {
            this.InitializeComponent();

            this.TransitionExample.Watch(this.Transitioning,
                                         TransitioningContentControl.TransitionProperty,
                                         TransitioningContentControl.RestartTransitionOnContentChangeProperty,
                                         TransitioningContentControl.IsTransitioningProperty);
            this.TransitionExample.Watch("Layout", this.Transitioning, WidthProperty, HeightProperty);
        }

        /// <summary>
        /// The transition plays on a content change, so the button hands it a new one.
        /// </summary>
        private void OnNext(object sender, RoutedEventArgs e)
        {
            this.page = this.page % 3 + 1;

            this.Transitioning.SetCurrentValue(ContentProperty, $"Page {this.page} of 3");
        }
    }
}
