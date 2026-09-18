// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Documents;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for PagePage.xaml
    /// </summary>
    public partial class PagePage : UserControl
    {
        public PagePage()
        {
            this.InitializeComponent();

            this.PageExample.Watch(this.Styled,
                                   Page.BackgroundProperty,
                                   Page.ForegroundProperty);
            this.PageExample.Watch("Attached", this.Styled, TextElement.FontSizeProperty);
        }
    }
}
