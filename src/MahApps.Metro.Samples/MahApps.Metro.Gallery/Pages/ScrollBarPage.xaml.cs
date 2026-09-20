// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for ScrollBarPage.xaml
    /// </summary>
    public partial class ScrollBarPage : UserControl
    {
        public ScrollBarPage()
        {
            this.InitializeComponent();

            this.ViewerExample.Watch(this.Viewer,
                                     ScrollViewer.HorizontalScrollBarVisibilityProperty,
                                     ScrollViewer.VerticalScrollBarVisibilityProperty,
                                     Control.PaddingProperty);
            this.ViewerExample.Watch("Attached",
                                     this.Viewer,
                                     ScrollViewerHelper.VerticalScrollBarOnLeftSideProperty,
                                     ScrollViewerHelper.IsHorizontalScrollWheelEnabledProperty,
                                     ScrollViewerHelper.BubbleUpScrollEventToParentScrollviewerProperty);
            this.ViewerExample.Watch("Layout", this.Viewer, WidthProperty, HeightProperty);

            this.BarExample.Watch(this.Bar,
                                  ScrollBar.OrientationProperty,
                                  RangeBase.ValueProperty,
                                  RangeBase.MinimumProperty,
                                  RangeBase.MaximumProperty,
                                  ScrollBar.ViewportSizeProperty,
                                  IsEnabledProperty);
            this.BarExample.Watch("Layout", this.Bar, WidthProperty);

            this.SizeExample.Watch(this.Thick,
                                   ScrollViewer.HorizontalScrollBarVisibilityProperty,
                                   ScrollViewer.VerticalScrollBarVisibilityProperty);
            this.SizeExample.Watch("Layout", this.Thick, WidthProperty, HeightProperty);

            this.StudioExample.Watch(this.Studio,
                                     ScrollBar.OrientationProperty,
                                     RangeBase.ValueProperty,
                                     ScrollBar.ViewportSizeProperty,
                                     IsEnabledProperty);
        }
    }
}
