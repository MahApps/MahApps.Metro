// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for HyperlinkButtonPage.xaml
    /// </summary>
    public partial class HyperlinkButtonPage : UserControl
    {
        public HyperlinkButtonPage()
        {
            this.InitializeComponent();

            this.TextExample.Watch(this.TextHyperlinkButton,
                                   HyperlinkButton.IsUnderlineVisibleProperty,
                                   IsEnabledProperty);
        }

        private void OnAnswerClick(object sender, RoutedEventArgs e)
        {
            this.Answer.Text = "42";
        }
    }
}
