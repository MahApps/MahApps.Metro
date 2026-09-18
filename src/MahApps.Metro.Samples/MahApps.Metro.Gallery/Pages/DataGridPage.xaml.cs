// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for DataGridPage.xaml
    /// </summary>
    public partial class DataGridPage : UserControl
    {
        public DataGridPage()
        {
            this.InitializeComponent();

            this.GridExample.Watch(this.Rows,
                                   DataGrid.GridLinesVisibilityProperty,
                                   DataGrid.HeadersVisibilityProperty,
                                   DataGrid.SelectionModeProperty,
                                   DataGrid.SelectionUnitProperty,
                                   DataGrid.CanUserSortColumnsProperty);
            this.GridExample.Watch("Attached",
                                   this.Rows,
                                   DataGridHelper.CellPaddingProperty,
                                   DataGridHelper.ColumnHeaderPaddingProperty,
                                   DataGridHelper.EnableCellEditAssistProperty);
            this.GridExample.Watch("Layout", this.Rows, WidthProperty, HeightProperty);

            this.AzureExample.Watch(this.Azure,
                                    DataGrid.GridLinesVisibilityProperty,
                                    DataGrid.HeadersVisibilityProperty,
                                    DataGrid.SelectionModeProperty,
                                    DataGrid.CanUserSortColumnsProperty);
        }
    }
}
