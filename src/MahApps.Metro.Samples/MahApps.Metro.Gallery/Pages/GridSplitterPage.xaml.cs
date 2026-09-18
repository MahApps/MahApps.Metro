// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for GridSplitterPage.xaml
    /// </summary>
    public partial class GridSplitterPage : UserControl
    {
        public GridSplitterPage()
        {
            this.InitializeComponent();

            this.ColumnsExample.Watch(this.Columns,
                                      GridSplitter.ShowsPreviewProperty,
                                      GridSplitter.ResizeDirectionProperty,
                                      GridSplitter.ResizeBehaviorProperty,
                                      GridSplitter.DragIncrementProperty);
            this.ColumnsExample.Watch("Layout", this.Columns, WidthProperty);

            this.RowsExample.Watch(this.Rows,
                                   GridSplitter.ShowsPreviewProperty,
                                   GridSplitter.ResizeDirectionProperty,
                                   GridSplitter.ResizeBehaviorProperty);
            this.RowsExample.Watch("Layout", this.Rows, HeightProperty);
        }
    }
}
