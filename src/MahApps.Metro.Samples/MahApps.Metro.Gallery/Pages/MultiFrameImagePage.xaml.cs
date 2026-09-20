// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for MultiFrameImagePage.xaml
    /// </summary>
    public partial class MultiFrameImagePage : UserControl
    {
        public MultiFrameImagePage()
        {
            this.InitializeComponent();

            this.IconExample.Watch(this.Small, MultiFrameImage.MultiFrameImageModeProperty);
            this.IconExample.Watch("Layout", this.Small, WidthProperty, HeightProperty);
        }
    }
}
