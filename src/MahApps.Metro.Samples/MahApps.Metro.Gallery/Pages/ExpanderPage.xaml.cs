// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for ExpanderPage.xaml
    /// </summary>
    public partial class ExpanderPage : UserControl
    {
        public ExpanderPage()
        {
            this.InitializeComponent();

            this.PlainExample.Watch(this.Plain,
                                    Expander.IsExpandedProperty,
                                    Expander.ExpandDirectionProperty,
                                    HeaderedContentControl.HeaderProperty);
            this.PlainExample.Watch("Attached",
                                    this.Plain,
                                    ExpanderHelper.ShowToggleButtonProperty,
                                    HeaderedControlHelper.HeaderFontSizeProperty,
                                    HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty,
                                    ControlsHelper.CornerRadiusProperty);
            this.PlainExample.Watch("Layout", this.Plain, WidthProperty);

            this.Win10Example.Watch(this.Win10,
                                    Expander.IsExpandedProperty,
                                    Expander.ExpandDirectionProperty,
                                    HeaderedContentControl.HeaderProperty);
            this.Win10Example.Watch("Attached", this.Win10, ExpanderHelper.ShowToggleButtonProperty);

            this.WinUIExample.Watch(this.WinUI,
                                    Expander.IsExpandedProperty,
                                    Expander.ExpandDirectionProperty,
                                    HeaderedContentControl.HeaderProperty);
            this.WinUIExample.Watch("Attached", this.WinUI, ExpanderHelper.ShowToggleButtonProperty);

            this.AlignmentExample.Watch(this.Alignment,
                                        Expander.IsExpandedProperty,
                                        Control.HorizontalContentAlignmentProperty);
            this.AlignmentExample.Watch("Attached", this.Alignment, HeaderedControlHelper.HeaderHorizontalContentAlignmentProperty);

            this.StudioExample.Watch(this.Studio,
                                     Expander.IsExpandedProperty,
                                     Expander.ExpandDirectionProperty,
                                     HeaderedContentControl.HeaderProperty);
        }
    }
}
