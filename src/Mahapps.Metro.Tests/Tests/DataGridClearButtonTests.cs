// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class DataGridClearButtonTests
    {
        private DataGridClearButtonWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<DataGridClearButtonWindow>().ConfigureAwait(false);
            this.window.Invoke(() => this.window.TheGrid.UpdateLayout());
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [SetUp]
        public void SetUp()
        {
            Assert.That(this.window, Is.Not.Null);

            this.window.TheGrid.CancelEdit(DataGridEditingUnit.Cell);
            this.window.TheGrid.CancelEdit(DataGridEditingUnit.Row);
            this.window.TheGrid.ItemsSource = new System.Collections.ObjectModel.ObservableCollection<DataGridClearButtonRow> { new DataGridClearButtonRow() };
            this.window.TheGrid.UpdateLayout();
        }

        /// <summary>
        /// GH-3966: the clear button used to hand the empty text to the bound object the moment it was
        /// pressed. A DataGrid gives its cell over at the commit, after CellEditEnding, and a handler that
        /// wants to refuse the change has nothing left to refuse if the object already took it.
        /// </summary>
        [Test]
        public void ClearingACellLeavesTheObjectAloneUntilTheGridCommits()
        {
            Assert.That(this.window, Is.Not.Null);

            var grid = this.window.TheGrid;
            var row = (DataGridClearButtonRow)grid.Items[0];
            var column = grid.Columns[0];

            grid.CurrentCell = new DataGridCellInfo(row, column);
            grid.BeginEdit();
            grid.UpdateLayout();

            var editor = column.GetCellContent(row) as TextBox;
            Assert.That(editor, Is.Not.Null, "the cell should be edited in a text box");
            Assert.That(editor!.Text, Is.EqualTo("123"));

            var clearButton = editor.FindChild<Button>("PART_ClearText");
            Assert.That(clearButton, Is.Not.Null, "the MahApps style should have put a clear button in the box");

            // what the button does when it is pressed: the command, aimed at the box it belongs to
            ((RoutedCommand)MahAppsCommands.ClearControlCommand).Execute(null, editor);
            grid.UpdateLayout();

            Assert.That(editor.Text, Is.Empty, "the box is what the button clears");
            Assert.That(row.Value, Is.EqualTo("123"), "and the object keeps what it had until the grid says so");

            grid.CommitEdit(DataGridEditingUnit.Row, true);

            Assert.That(row.Value, Is.Empty, "the commit is what hands the empty value over");
        }
    }
}
