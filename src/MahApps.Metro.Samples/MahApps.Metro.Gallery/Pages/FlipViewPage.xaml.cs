// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for FlipViewPage.xaml
    /// </summary>
    public partial class FlipViewPage : UserControl
    {
        public FlipViewPage()
        {
            this.InitializeComponent();

            this.FlipExample.Watch(this.Flip,
                                   FlipView.BannerTextProperty,
                                   FlipView.IsBannerEnabledProperty,
                                   FlipView.CircularNavigationProperty,
                                   FlipView.OrientationProperty,
                                   FlipView.MouseHoverBorderEnabledProperty,
                                   FlipView.SelectedIndexProperty);
            this.FlipExample.Watch("Layout", this.Flip, WidthProperty, HeightProperty);
        }
    }
}
