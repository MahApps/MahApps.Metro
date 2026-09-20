// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for GroupBoxPage.xaml
    /// </summary>
    public partial class GroupBoxPage : UserControl
    {
        public GroupBoxPage()
        {
            this.InitializeComponent();

            this.PlainExample.Watch(this.Plain, HeaderedContentControl.HeaderProperty);
            this.PlainExample.Watch("Attached",
                                    this.Plain,
                                    HeaderedControlHelper.HeaderFontSizeProperty,
                                    HeaderedControlHelper.HeaderMarginProperty,
                                    HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty,
                                    ControlsHelper.CornerRadiusProperty);
            this.PlainExample.Watch("Layout", this.Plain, WidthProperty);

            this.CleanExample.Watch(this.Clean, HeaderedContentControl.HeaderProperty);
            this.CleanExample.Watch("Layout", this.Clean, WidthProperty);

            this.StudioExample.Watch(this.Studio, HeaderedContentControl.HeaderProperty);
        }
    }
}
