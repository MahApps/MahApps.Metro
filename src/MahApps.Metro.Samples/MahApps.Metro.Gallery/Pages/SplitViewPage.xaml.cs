// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for SplitViewPage.xaml
    /// </summary>
    public partial class SplitViewPage : UserControl
    {
        public SplitViewPage()
        {
            this.InitializeComponent();

            this.SplitExample.Watch(this.Split,
                                    SplitView.DisplayModeProperty,
                                    SplitView.IsPaneOpenProperty,
                                    SplitView.PanePlacementProperty,
                                    SplitView.OpenPaneLengthProperty,
                                    SplitView.CompactPaneLengthProperty,
                                    SplitView.CanResizeOpenPaneProperty);
            this.SplitExample.Watch("Layout", this.Split, WidthProperty, HeightProperty);
        }
    }
}
