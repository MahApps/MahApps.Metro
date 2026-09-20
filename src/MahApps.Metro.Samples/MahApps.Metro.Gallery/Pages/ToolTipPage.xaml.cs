// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for ToolTipPage.xaml
    /// </summary>
    public partial class ToolTipPage : UserControl
    {
        public ToolTipPage()
        {
            this.InitializeComponent();

            this.TipExample.Watch(this.Tip,
                                  ContentControl.ContentProperty,
                                  BackgroundProperty,
                                  BorderBrushProperty,
                                  BorderThicknessProperty,
                                  ForegroundProperty,
                                  FontSizeProperty,
                                  PaddingProperty,
                                  // the plain name is the ToolTip property of this control rather
                                  // than the type, which is what a page about tooltips gets for free
                                  System.Windows.Controls.ToolTip.HasDropShadowProperty);
            this.TipExample.Watch("Attached",
                                  this.Tip,
                                  ControlsHelper.CornerRadiusProperty,
                                  ControlsHelper.ContentCharacterCasingProperty);

            this.ContentExample.Watch("The text", this.Detail, TextBlock.TextWrappingProperty);
            this.ContentExample.Watch("Layout", this.Panel, MaxWidthProperty);

            this.TimingExample.Watch("The first button",
                                     this.Ready,
                                     ToolTipService.InitialShowDelayProperty,
                                     ToolTipService.ShowDurationProperty,
                                     ToolTipService.BetweenShowDelayProperty,
                                     ToolTipService.PlacementProperty);
            this.TimingExample.Watch("The second button",
                                     this.Blocked,
                                     IsEnabledProperty,
                                     ToolTipService.ShowOnDisabledProperty);
        }
    }
}
