// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for StatusBarPage.xaml
    /// </summary>
    public partial class StatusBarPage : UserControl
    {
        public StatusBarPage()
        {
            this.InitializeComponent();

            this.BarExample.Watch(this.Bar,
                                  BackgroundProperty,
                                  ForegroundProperty,
                                  FontSizeProperty,
                                  IsEnabledProperty);
            this.BarExample.Watch("Layout", this.Bar, WidthProperty);

            this.SeparatorExample.Watch(this.Separated, BackgroundProperty, FontSizeProperty);
            this.SeparatorExample.Watch("Layout", this.Separated, WidthProperty);

            this.JobExample.Watch(this.Job,
                                  PaddingProperty,
                                  HorizontalContentAlignmentProperty,
                                  BorderBrushProperty,
                                  BorderThicknessProperty);
            this.JobExample.Watch("Layout", this.Working, WidthProperty);

            this.CleanExample.Watch(this.Clean, BackgroundProperty, ForegroundProperty);
            this.CleanExample.Watch("Layout", this.Clean, WidthProperty);
        }
    }
}
