// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
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

            this.LongView.ItemsSource = Enumerable
                                        .Range(1, 4200)
                                        .Select(file => new KeyValuePair<string, string>($"File {file}.txt", $"{file * 42} bytes"))
                                        .ToList();

            this.ColumnsExample.Watch(this.Columns,
                                      ListView.SelectionModeProperty,
                                      Control.BorderThicknessProperty,
                                      IsEnabledProperty);
            this.ColumnsExample.Watch("Layout", this.Columns, WidthProperty, HeightProperty);

            this.ReadExample.Watch(this.Reading, ListView.SelectionModeProperty);
            this.ReadExample.Watch("Layout", this.Reading, WidthProperty, HeightProperty);

            this.LongExample.Watch(this.LongView, ListView.SelectionModeProperty);
            this.LongExample.Watch("Layout", this.LongView, WidthProperty, HeightProperty);
        }
    }
}
