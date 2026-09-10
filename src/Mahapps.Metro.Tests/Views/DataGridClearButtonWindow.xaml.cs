// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;

namespace MahApps.Metro.Tests.Views
{
    public partial class DataGridClearButtonWindow : TestWindow
    {
        public DataGridClearButtonWindow()
        {
            this.InitializeComponent();

            this.TheGrid.ItemsSource = new ObservableCollection<DataGridClearButtonRow> { new DataGridClearButtonRow() };
        }
    }

    public class DataGridClearButtonRow
    {
        public string Value { get; set; } = "123";
    }
}
