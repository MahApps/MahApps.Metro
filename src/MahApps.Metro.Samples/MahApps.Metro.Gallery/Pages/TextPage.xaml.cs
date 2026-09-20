// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Documents;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for TextPage.xaml
    /// </summary>
    public partial class TextPage : UserControl
    {
        public TextPage()
        {
            this.InitializeComponent();

            this.CaptionExample.Watch(this.Caption, ContentControl.ContentProperty, PaddingProperty, IsEnabledProperty);
            this.CaptionExample.Watch("Attached",
                                      this.Caption,
                                      ControlsHelper.RecognizesAccessKeyProperty,
                                      ControlsHelper.ContentCharacterCasingProperty);

            this.ChipExample.Watch(this.Chip,
                                   ContentControl.ContentProperty,
                                   BackgroundProperty,
                                   ForegroundProperty,
                                   PaddingProperty,
                                   IsEnabledProperty);
            this.ChipExample.Watch("Attached", this.Chip, ControlsHelper.CornerRadiusProperty);

            this.BlockExample.Watch(this.Block,
                                    TextBlock.TextProperty,
                                    TextBlock.TextWrappingProperty,
                                    TextBlock.TextTrimmingProperty,
                                    TextBlock.TextAlignmentProperty,
                                    TextElement.FontSizeProperty,
                                    TextElement.ForegroundProperty);
            this.BlockExample.Watch("Layout", this.Block, WidthProperty);

            this.WatermarkExample.Watch(this.Watermark, TextBlock.TextProperty, OpacityProperty);
            this.WatermarkExample.Watch("The collapsing one", this.Collapsing, TextBlock.TextProperty, OpacityProperty);
        }
    }
}
