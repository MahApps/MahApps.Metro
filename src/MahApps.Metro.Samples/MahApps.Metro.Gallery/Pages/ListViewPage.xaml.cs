// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for ListViewPage.xaml
    /// </summary>
    public partial class ListViewPage : UserControl
    {
        public ListViewPage()
        {
            this.InitializeComponent();

            this.ColumnsExample.Watch(this.Columns,
                                      ListView.SelectionModeProperty,
                                      Control.BorderThicknessProperty,
                                      IsEnabledProperty);
            this.ColumnsExample.Watch("Layout", this.Columns, WidthProperty, HeightProperty);

            this.ReadExample.Watch(this.Reading, ListView.SelectionModeProperty);
            this.ReadExample.Watch("Layout", this.Reading, WidthProperty, HeightProperty);
        }
    }
}
