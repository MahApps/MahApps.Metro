// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for MetroThumbContentControlPage.xaml
    /// </summary>
    public partial class MetroThumbContentControlPage : UserControl
    {
        public MetroThumbContentControlPage()
        {
            this.InitializeComponent();

            this.DragExample.Watch(this.Thumb, MetroThumbContentControl.IsDraggingProperty, IsEnabledProperty);
            this.DragExample.Watch("Layout", this.Thumb, WidthProperty, HeightProperty);
        }

        /// <summary>
        /// What dragging means here: the tile follows the mouse inside its canvas, and stops at the
        /// edges rather than disappearing behind them.
        /// </summary>
        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.Thumb.Parent is not Canvas canvas)
            {
                return;
            }

            var left = Canvas.GetLeft(this.Thumb) + e.HorizontalChange;
            var top = Canvas.GetTop(this.Thumb) + e.VerticalChange;

            Canvas.SetLeft(this.Thumb, System.Math.Max(0, System.Math.Min(left, canvas.ActualWidth - this.Thumb.ActualWidth)));
            Canvas.SetTop(this.Thumb, System.Math.Max(0, System.Math.Min(top, canvas.ActualHeight - this.Thumb.ActualHeight)));
        }
    }
}
