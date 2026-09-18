// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for HyperlinkPage.xaml
    /// </summary>
    public partial class HyperlinkPage : UserControl
    {
        public HyperlinkPage()
        {
            this.InitializeComponent();

            // TextDecorations would belong here, since the underline is what marks the link, but a
            // decoration collection has no way back from the text it prints, so there is nothing to
            // turn: it would read as the name of its own type
            this.LinkExample.Watch(this.Link,
                                   Hyperlink.NavigateUriProperty,
                                   ContentElement.IsEnabledProperty,
                                   TextElement.FontWeightProperty,
                                   TextElement.ForegroundProperty);
        }

        /// <summary>
        /// A NavigateUri says where to, and nothing else: what a click does is up to this.
        /// </summary>
        private void OnNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });

            e.Handled = true;
        }
    }
}
