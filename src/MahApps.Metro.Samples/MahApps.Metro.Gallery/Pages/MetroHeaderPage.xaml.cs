// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for MetroHeaderPage.xaml
    /// </summary>
    public partial class MetroHeaderPage : UserControl
    {
        public MetroHeaderPage()
        {
            this.InitializeComponent();

            this.AboveExample.Watch(this.Above, MetroHeader.HeaderProperty, MetroHeader.OrientationProperty);
            this.AboveExample.Watch("Attached",
                                    this.Above,
                                    HeaderedControlHelper.HeaderFontSizeProperty,
                                    HeaderedControlHelper.HeaderMarginProperty);

            this.BesideExample.Watch(this.Beside, MetroHeader.HeaderProperty, MetroHeader.OrientationProperty);
            this.BesideExample.Watch("Attached",
                                     this.Beside,
                                     HeaderedControlHelper.HeaderMarginProperty,
                                     HeaderedControlHelper.HeaderVerticalContentAlignmentProperty);
        }
    }
}
