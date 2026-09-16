// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4294: what colour the text of a data grid row has while the mouse is on it. The row and the
    /// cells both carry a style with something to say about that, so the answer only holds together
    /// while the two agree on who says it for which selection unit.
    /// </summary>
    /// <remarks>
    /// The theme paints selected text and text under the mouse in the same colour, so a grid left as
    /// it comes shows nothing either way. These tests hand the grid two colours it can be told apart by,
    /// which is what the report did as well.
    /// </remarks>
    [TestFixture]
    public class DataGridSelectionForegroundTests
    {
        private static readonly Color Selected = Colors.Lime;
        private static readonly Color UnderTheMouse = Colors.Red;

        // IsMouseOver reads the mouse, and a test has no mouse to put anywhere. Both of these are
        // read only to the outside and carry a key their own class keeps, which is the one handle
        // there is on the state the triggers are written against.
        private static readonly DependencyPropertyKey MouseOverKey = KeyOf(typeof(UIElement), "IsMouseOverPropertyKey");
        private static readonly DependencyPropertyKey SelectionActiveKey = KeyOf(typeof(Selector), "IsSelectionActivePropertyKey");

        private static DependencyPropertyKey KeyOf(System.Type owner, string field)
        {
            return (DependencyPropertyKey)owner.GetField(field, BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
        }

        private TestWindow window = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window.Close();
        }

        public class Row
        {
            public string First { get; set; } = "one";

            public string Second { get; set; } = "two";

            public string Third { get; set; } = "three";
        }

        private DataGrid GridWith(DataGridSelectionUnit selectionUnit)
        {
            var grid = new DataGrid
                       {
                           Width = 400,
                           Height = 200,
                           AutoGenerateColumns = true,
                           SelectionUnit = selectionUnit,
                           ItemsSource = new List<Row> { new(), new() }
                       };

            grid.Resources.Add("MahApps.Brushes.DataGrid.Selection.Text", new SolidColorBrush(Selected));
            grid.Resources.Add("MahApps.Brushes.DataGrid.Selection.Text.MouseOver", new SolidColorBrush(UnderTheMouse));

            this.window.Content = grid;
            this.window.UpdateLayout();

            // the rows are built off the dispatcher, so they are not there the moment the grid is
            for (var turn = 0; turn < 20 && grid.FindChildren<DataGridRow>(true).Any() == false; turn++)
            {
                ClipAssert.Pump(50);
            }

            // and a selection only looks selected while the window is the one being looked at
            grid.SetValue(SelectionActiveKey, true);

            return grid;
        }

        private static DataGridCell[] CellsOf(DataGrid grid)
        {
            var row = grid.FindChildren<DataGridRow>(true).FirstOrDefault();
            Assert.That(row, Is.Not.Null, "the grid should have built a row");

            var cells = row!.FindChildren<DataGridCell>(true).ToArray();
            Assert.That(cells, Has.Length.EqualTo(3), "and three cells in it");

            return cells;
        }

        private static void PutTheMouseOn(DataGridCell cell)
        {
            cell.GetVisualAncestor<DataGridRow>()?.SetValue(MouseOverKey, true);
            cell.SetValue(MouseOverKey, true);
            ClipAssert.Pump();
        }

        private static Color ColourOf(DataGridCell cell)
        {
            Assert.That(cell.Foreground, Is.InstanceOf<SolidColorBrush>(), "a cell should be painted in a plain colour");
            return ((SolidColorBrush)cell.Foreground).Color;
        }

        [Test]
        [Description("A row selected as a whole keeps one colour, the mouse resting on one of its cells or not.")]
        public void TheWholeSelectedRowKeepsOneColourUnderTheMouse()
        {
            var grid = this.GridWith(DataGridSelectionUnit.FullRow);
            var cells = CellsOf(grid);

            grid.SelectedIndex = 0;
            ClipAssert.Pump();

            Assert.That(cells.Select(ColourOf), Is.All.EqualTo(Selected), "the selected row should be in the selection colour");

            PutTheMouseOn(cells[1]);

            Assert.That(cells.Select(ColourOf), Is.All.EqualTo(Selected), "and it should still be, all of it, with the mouse on the middle cell");
        }

        [Test]
        [Description("A row nobody selected still answers the mouse, and answers with all of itself.")]
        public void AnUnselectedRowAnswersTheMouseAsOneRow()
        {
            var grid = this.GridWith(DataGridSelectionUnit.FullRow);
            var cells = CellsOf(grid);

            PutTheMouseOn(cells[1]);

            Assert.That(cells.Select(ColourOf), Is.All.EqualTo(UnderTheMouse), "the whole row should take the colour for the mouse");
        }

        [Test]
        [Description("Where cells are what gets selected, the cell under the mouse is what answers it.")]
        public void OnlyTheCellUnderTheMouseAnswersWhereCellsAreSelected()
        {
            var grid = this.GridWith(DataGridSelectionUnit.Cell);
            var cells = CellsOf(grid);

            foreach (var cell in cells)
            {
                cell.SetCurrentValue(DataGridCell.IsSelectedProperty, true);
            }

            ClipAssert.Pump();
            PutTheMouseOn(cells[1]);

            Assert.That(ColourOf(cells[1]), Is.EqualTo(UnderTheMouse), "the cell under the mouse answers it");
            Assert.That(ColourOf(cells[0]), Is.EqualTo(Selected), "the one before it stays selected");
            Assert.That(ColourOf(cells[2]), Is.EqualTo(Selected), "and so does the one after it");
        }
    }
}
