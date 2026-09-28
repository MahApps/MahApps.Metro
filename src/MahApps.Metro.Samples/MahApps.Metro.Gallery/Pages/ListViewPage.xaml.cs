// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using MahApps.Metro.Controls;

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

            this.LongView.ItemsSource = Enumerable
                                        .Range(1, 4200)
                                        .Select(file => new KeyValuePair<string, string>($"File {file}.txt", $"{file * 42} bytes"))
                                        .ToList();

            this.ColumnsExample.Watch(this.Columns,
                                      ListView.SelectionModeProperty,
                                      ItemHelper.IsMultiSelectCheckBoxEnabledProperty,
                                      Control.BorderThicknessProperty,
                                      IsEnabledProperty);
            this.ColumnsExample.Watch("Layout", this.Columns, WidthProperty, HeightProperty);

            this.Win10ColumnsExample.Watch(this.Win10Columns,
                                          ListView.SelectionModeProperty,
                                          ItemHelper.IsMultiSelectCheckBoxEnabledProperty,
                                          IsEnabledProperty);
            this.Win10ColumnsExample.Watch("Layout", this.Win10Columns, WidthProperty, HeightProperty);

            this.WinUIColumnsExample.Watch(this.WinUIColumns,
                                          ListView.SelectionModeProperty,
                                          ItemHelper.IsMultiSelectCheckBoxEnabledProperty,
                                          IsEnabledProperty);
            this.WinUIColumnsExample.Watch("Layout", this.WinUIColumns, WidthProperty, HeightProperty);

            this.Win10Example.Watch(this.Win10,
                                    ListView.SelectionModeProperty,
                                    ItemHelper.IsMultiSelectCheckBoxEnabledProperty,
                                    IsEnabledProperty);
            this.Win10Example.Watch("Layout", this.Win10, WidthProperty, HeightProperty);

            this.WinUIExample.Watch(this.WinUI,
                                    ListView.SelectionModeProperty,
                                    ItemHelper.IsMultiSelectCheckBoxEnabledProperty,
                                    IsEnabledProperty);
            this.WinUIExample.Watch("Layout", this.WinUI, WidthProperty, HeightProperty);

            this.Win10GroupExample.Watch(this.Win10Grouped,
                                         ListView.SelectionModeProperty,
                                         ItemHelper.IsMultiSelectCheckBoxEnabledProperty,
                                         GroupItemHelper.CanSelectAllItemsProperty,
                                         GroupItemHelper.IsHeaderStickyProperty);
            this.Win10GroupExample.Watch("Layout", this.Win10Grouped, WidthProperty, HeightProperty);

            this.WinUIGroupExample.Watch(this.WinUIGrouped,
                                         ListView.SelectionModeProperty,
                                         ItemHelper.IsMultiSelectCheckBoxEnabledProperty,
                                         GroupItemHelper.CanSelectAllItemsProperty,
                                         GroupItemHelper.IsHeaderStickyProperty);
            this.WinUIGroupExample.Watch("Layout", this.WinUIGrouped, WidthProperty, HeightProperty);

            this.ReadExample.Watch(this.Reading, ListView.SelectionModeProperty);
            this.ReadExample.Watch("Layout", this.Reading, WidthProperty, HeightProperty);

            this.LongExample.Watch(this.LongView, ListView.SelectionModeProperty);
            this.LongExample.Watch("Layout", this.LongView, WidthProperty, HeightProperty);
        }
    }
}
