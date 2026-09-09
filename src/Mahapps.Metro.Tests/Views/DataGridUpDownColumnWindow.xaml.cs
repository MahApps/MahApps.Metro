// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;

namespace MahApps.Metro.Tests.Views
{
    public partial class DataGridUpDownColumnWindow : TestWindow
    {
        public DataGridUpDownColumnWindow()
        {
            this.InitializeComponent();

            this.TheGrid.ItemsSource = new ObservableCollection<DataGridUpDownRow> { new DataGridUpDownRow() };
        }
    }

    public class DataGridUpDownRow
    {
        public double Measurement { get; set; } = 2.5;

        public decimal Price { get; set; } = 19.99m;

        public int Count { get; set; } = 3;

        public long Size { get; set; } = 9007199254740993;
    }
}
