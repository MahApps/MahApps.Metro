// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The DataGrid in the Windows 10 and the WinUI look, read off the DataGrid of the Windows
    /// Community Toolkit and the TableView of microsoft-ui-xaml. The grid's own style hands its
    /// parts their looks, so every test here starts from the key of a grid style and nothing else.
    /// </summary>
    [TestFixture]
    public class DataGridSetStyleTests : WindowTestFixture<TestWindow>
    {
        // IsMouseOver reads the mouse, and a test has no mouse to put anywhere. Both of these are
        // read only to the outside and carry a key their own class keeps, which is the one handle
        // there is on the state the triggers are written against.
        private static readonly DependencyPropertyKey MouseOverKey = KeyOf(typeof(UIElement), "IsMouseOverPropertyKey");
        private static readonly DependencyPropertyKey SelectionActiveKey = KeyOf(typeof(Selector), "IsSelectionActivePropertyKey");

        private static DependencyPropertyKey KeyOf(Type owner, string field)
        {
            return (DependencyPropertyKey)owner.GetField(field, BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
        }

        public class Row
        {
            public string First { get; set; } = "one";

            public string Second { get; set; } = "two";

            public string Third { get; set; } = "three";
        }

        public class Todo
        {
            public bool Done { get; set; } = true;
        }

        private DataGrid GridFor(string styleKey, DataGridSelectionUnit selectionUnit = DataGridSelectionUnit.FullRow, IEnumerable? items = null)
        {
            var grid = new DataGrid
                       {
                           Style = (Style)this.window!.FindResource(styleKey),
                           Width = 400,
                           Height = 200,
                           AutoGenerateColumns = true,
                           SelectionUnit = selectionUnit,
                           ItemsSource = items ?? new List<Row> { new(), new() }
                       };

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

        // the brushes of a set are named after it, MahApps.Brushes.DataGrid.Win10.RowForeground for the grid MahApps.Styles.DataGrid.Win10
        private static string BrushOf(string gridKey, string name)
        {
            return $"MahApps.Brushes.DataGrid.{gridKey.Substring(gridKey.LastIndexOf('.') + 1)}.{name}";
        }

        private static DataGridRow FirstRowOf(DataGrid grid)
        {
            var row = grid.FindChildren<DataGridRow>(true).FirstOrDefault();
            Assert.That(row, Is.Not.Null, "the grid should have built a row");
            return row!;
        }

        private static DataGridCell CellOf(DataGrid grid)
        {
            var cell = FirstRowOf(grid).FindChildren<DataGridCell>(true).FirstOrDefault();
            Assert.That(cell, Is.Not.Null, "the row should have a cell");
            return cell!;
        }

        private static DataGridCell[] CellsOf(DataGrid grid)
        {
            var cells = FirstRowOf(grid).FindChildren<DataGridCell>(true).ToArray();
            Assert.That(cells, Has.Length.EqualTo(3), "the row should have three cells");
            return cells;
        }

        private static DataGridColumnHeader[] HeadersOf(DataGrid grid)
        {
            return grid.FindChildren<DataGridColumnHeader>(true).Where(h => h.Column is not null).OrderBy(h => h.Column.DisplayIndex).ToArray();
        }

        private static void PutTheMouseOn(DataGridCell cell)
        {
            cell.GetVisualAncestor<DataGridRow>()?.SetValue(MouseOverKey, true);
            cell.SetValue(MouseOverKey, true);
            ClipAssert.Pump();
        }

        [TestCase("MahApps.Styles.DataGrid.Win10", typeof(DataGridColumnHeader), "MahApps.Styles.DataGridColumnHeader.Win10")]
        [TestCase("MahApps.Styles.DataGrid.Win10", typeof(DataGridRowHeader), "MahApps.Styles.DataGridRowHeader.Win10")]
        [TestCase("MahApps.Styles.DataGrid.Win10", typeof(DataGridRow), "MahApps.Styles.DataGridRow.Win10")]
        [TestCase("MahApps.Styles.DataGrid.Win10", typeof(DataGridCell), "MahApps.Styles.DataGridCell.Win10")]
        [Description("The grid's own style hands its parts the look of its set, so a Windows 10 grid in a window of the default set still has Windows 10 rows and headers.")]
        public void TheGridHandsItsPartsTheLookOfItsSet(string gridKey, Type partType, string expectedKey)
        {
            var grid = this.GridFor(gridKey);
            grid.HeadersVisibility = DataGridHeadersVisibility.All;
            this.window!.UpdateLayout();
            ClipAssert.Pump();

            var part = grid.FindChildren<FrameworkElement>(true).FirstOrDefault(partType.IsInstanceOfType);

            Assert.That(part, Is.Not.Null, $"the grid should have a {partType.Name}");
            Assert.That(StandsOn(part!.Style, (Style)this.window.FindResource(expectedKey)), Is.True, $"the {partType.Name} should stand on {expectedKey}");
        }

        [TestCase("MahApps.Styles.DataGrid.Win10", 32d)]
        [Description("A row stands as tall as a row of the set: 32 in the toolkit DataGrid.")]
        public void TheRowsStandAsTallAsTheSetSays(string gridKey, double height)
        {
            var grid = this.GridFor(gridKey);

            Assert.That(FirstRowOf(grid).ActualHeight, Is.GreaterThanOrEqualTo(height));
        }

        [Test]
        [Description("A selected row of the Windows 10 grid is filled with the accent, as the toolkit DataGrid fills it.")]
        public void AWin10RowIsFilledWithTheAccentWhenSelected()
        {
            var grid = this.GridFor("MahApps.Styles.DataGrid.Win10");
            var row = FirstRowOf(grid);

            grid.SelectedIndex = 0;
            ClipAssert.Pump();

            var background = row.FindChild<Border>("RowBackgroundBorder");
            Assert.That(background, Is.Not.Null, "the row should paint its background on RowBackgroundBorder");
            Assert.That(background!.Background, Is.SameAs(this.window!.FindResource("MahApps.Brushes.DataGrid.Win10.RowBackgroundSelected")));
        }

        [TestCase("MahApps.Styles.DataGrid.Win10")]
        [Description("The text of a row keeps its colour when the row is selected, since the set marks a selection with the fill alone.")]
        public void TheTextKeepsItsColourWhenARowIsSelected(string gridKey)
        {
            var grid = this.GridFor(gridKey);
            var cells = CellsOf(grid);
            var expected = this.window!.FindResource(BrushOf(gridKey, "RowForeground"));

            var before = cells.Select(c => c.Foreground).ToArray();

            grid.SelectedIndex = 0;
            ClipAssert.Pump();

            Assert.Multiple(() =>
                {
                    Assert.That(before, Is.All.SameAs(expected), "before the selection");
                    Assert.That(cells.Select(c => c.Foreground), Is.All.SameAs(expected), "and after it");
                });
        }

        [TestCase("MahApps.Styles.DataGrid.Win10")]
        [Description("Where cells are what gets selected, the cell under the mouse is what answers it, in the colours of the set.")]
        public void OnlyTheCellUnderTheMouseAnswersWhereCellsAreSelected(string gridKey)
        {
            var grid = this.GridFor(gridKey, DataGridSelectionUnit.Cell);
            var cells = CellsOf(grid);

            foreach (var cell in cells)
            {
                cell.SetCurrentValue(DataGridCell.IsSelectedProperty, true);
            }

            ClipAssert.Pump();
            PutTheMouseOn(cells[1]);

            var underTheMouse = this.window!.FindResource(BrushOf(gridKey, "RowBackgroundSelectedPointerOver"));
            var selected = this.window.FindResource(BrushOf(gridKey, "RowBackgroundSelected"));

            Assert.Multiple(() =>
                {
                    Assert.That(cells[1].Background, Is.SameAs(underTheMouse), "the cell under the mouse answers it");
                    Assert.That(cells[0].Background, Is.SameAs(selected), "the one before it stays selected");
                    Assert.That(cells[2].Background, Is.SameAs(selected), "and so does the one after it");
                });
        }

        [TestCase("MahApps.Styles.DataGrid.Win10", "\uE74A")]
        [Description("The sort glyph stands in the header of the sorted column and in no other.")]
        public void TheSortGlyphShowsOnTheSortedColumnOnly(string gridKey, string glyph)
        {
            var grid = this.GridFor(gridKey);
            grid.Columns[0].SortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this.window!.UpdateLayout();
            ClipAssert.Pump();

            var headers = HeadersOf(grid);
            Assert.That(headers, Has.Length.GreaterThanOrEqualTo(2), "the grid should have a header per column");

            var sorted = headers[0].FindChild<TextBlock>("SortGlyph");
            var unsorted = headers[1].FindChild<TextBlock>("SortGlyph");

            Assert.Multiple(() =>
                {
                    Assert.That(sorted, Is.Not.Null, "the header should have a SortGlyph");
                    Assert.That(sorted!.Visibility, Is.EqualTo(Visibility.Visible), "on the sorted column");
                    Assert.That(sorted.Text, Is.EqualTo(glyph));
                    Assert.That(unsorted!.Visibility, Is.EqualTo(Visibility.Collapsed), "and not on the others");
                });
        }

        [TestCase("MahApps.Styles.DataGrid.Win10", "MahApps.Styles.TextBox.Win10")]
        [Description("A text column the grid made itself edits in the text box of the set.")]
        public void AnAutoGeneratedTextColumnEditsInTheBoxOfTheSet(string gridKey, string boxKey)
        {
            var grid = this.GridFor(gridKey);

            var column = (DataGridTextColumn)grid.Columns[0];

            Assert.That(StandsOn(column.EditingElementStyle, (Style)this.window!.FindResource(boxKey)), Is.True, $"the editing style should stand on {boxKey}");
        }

        [TestCase("MahApps.Styles.DataGrid.Win10")]
        [Description("The line under the header row runs the whole width, over the header WPF puts after the last column as well.")]
        public void TheLineUnderTheHeadersRunsTheWholeWidth(string gridKey)
        {
            var grid = this.GridFor(gridKey);

            var filler = grid.FindChildren<DataGridColumnHeader>(true).FirstOrDefault(h => h.Column is null);
            var rule = this.window!.FindResource(BrushOf(gridKey, "GridLine"));

            Assert.That(filler, Is.Not.Null, "the grid should have a header after its last column");
            Assert.That(filler!.ActualWidth, Is.GreaterThan(0), "and that header should take up the rest of the row");
            Assert.Multiple(() =>
                {
                    foreach (var header in grid.FindChildren<DataGridColumnHeader>(true))
                    {
                        var border = header.FindChild<Border>("BackgroundBorder");
                        Assert.That(border, Is.Not.Null, $"the header of {header.Column?.Header ?? "the filler"} should have a BackgroundBorder");
                        Assert.That(border!.BorderThickness.Bottom, Is.EqualTo(1), $"the header of {header.Column?.Header ?? "the filler"} draws the line");
                        Assert.That(border.BorderBrush, Is.SameAs(rule), $"the header of {header.Column?.Header ?? "the filler"} draws it in the colour of the grid lines");
                    }
                });
        }

        [TestCase("MahApps.Styles.DataGrid.Win10", 12d)]
        [Description("A check box stands where the toolkit puts it in a cell, and stays there when a click puts the cell into editing.")]
        public void ACheckBoxStaysPutWhenItsCellIsEdited(string gridKey, double offset)
        {
            var grid = this.GridFor(gridKey, items: new List<Todo> { new(), new() });
            var cell = CellOf(grid);

            var before = cell.FindChildren<CheckBox>(true).Single().TranslatePoint(default, cell).X;

            grid.CurrentCell = new DataGridCellInfo(grid.Items[0], grid.Columns[0]);
            grid.BeginEdit();
            ClipAssert.Pump();
            this.window!.UpdateLayout();

            Assert.That(cell.IsEditing, Is.True, "the cell should be in editing");
            var after = cell.FindChildren<CheckBox>(true).Single().TranslatePoint(default, cell).X;

            Assert.Multiple(() =>
                {
                    Assert.That(before, Is.EqualTo(offset).Within(0.5), "showing its value");
                    Assert.That(after, Is.EqualTo(before).Within(0.5), "and being edited");
                });
        }

        private static bool StandsOn(Style? style, Style expected)
        {
            for (var current = style; current is not null; current = current.BasedOn)
            {
                if (ReferenceEquals(current, expected))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
