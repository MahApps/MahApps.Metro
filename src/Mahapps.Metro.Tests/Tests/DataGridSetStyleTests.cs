// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using MahApps.Metro.Converters;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;
using static MahApps.Metro.Tests.TestHelpers.FakeInput;
using static MahApps.Metro.Tests.TestHelpers.StyleLookups;

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

        private DataGrid GridFor(string styleKey, DataGridSelectionUnit selectionUnit = DataGridSelectionUnit.FullRow, IEnumerable? items = null, Action<DataGrid>? prepare = null)
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

            // whatever has to be there before the grid is loaded, which is when it is handed a GroupStyle
            prepare?.Invoke(grid);

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

        // the header WPF puts after the last column, the one inside the presenter of the headers
        private static DataGridColumnHeader? FillerOf(DataGrid grid)
        {
            return grid.FindChildren<DataGridColumnHeader>(true).FirstOrDefault(h => h.Column is null && h.TemplatedParent is DataGridColumnHeadersPresenter);
        }

        // the header over the vertical scroll bar, which belongs to the template of the grid rather than to the presenter
        private static DataGridColumnHeader? CornerOf(DataGrid grid)
        {
            return grid.FindChildren<DataGridColumnHeader>(true).FirstOrDefault(h => h.Column is null && h.TemplatedParent is ScrollViewer);
        }

        private static List<Row> ManyRows()
        {
            return Enumerable.Range(0, 30).Select(i => new Row { First = $"{i}" }).ToList();
        }

        [TestCase("MahApps.Styles.DataGrid.Win10", typeof(DataGridColumnHeader), "MahApps.Styles.DataGridColumnHeader.Win10")]
        [TestCase("MahApps.Styles.DataGrid.Win10", typeof(DataGridRowHeader), "MahApps.Styles.DataGridRowHeader.Win10")]
        [TestCase("MahApps.Styles.DataGrid.Win10", typeof(DataGridRow), "MahApps.Styles.DataGridRow.Win10")]
        [TestCase("MahApps.Styles.DataGrid.Win10", typeof(DataGridCell), "MahApps.Styles.DataGridCell.Win10")]
        [TestCase("MahApps.Styles.DataGrid.WinUI", typeof(DataGridColumnHeader), "MahApps.Styles.DataGridColumnHeader.WinUI")]
        [TestCase("MahApps.Styles.DataGrid.WinUI", typeof(DataGridRowHeader), "MahApps.Styles.DataGridRowHeader.WinUI")]
        [TestCase("MahApps.Styles.DataGrid.WinUI", typeof(DataGridRow), "MahApps.Styles.DataGridRow.WinUI")]
        [TestCase("MahApps.Styles.DataGrid.WinUI", typeof(DataGridCell), "MahApps.Styles.DataGridCell.WinUI")]
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
        [TestCase("MahApps.Styles.DataGrid.WinUI", 40d)]
        [Description("A row stands as tall as a row of the set: 32 in the toolkit DataGrid, 40 in TableView.")]
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

        private static Rectangle PillOf(DataGridRow row)
        {
            var pill = row.FindChild<Rectangle>("SelectionIndicator");
            Assert.That(pill, Is.Not.Null, "the row should have a SelectionIndicator");
            return pill!;
        }

        private static DataGridRow[] RowsOf(DataGrid grid)
        {
            return grid.FindChildren<DataGridRow>(true).Where(r => r.Item is Row).OrderBy(r => r.GetIndex()).ToArray();
        }

        [Test]
        [Description("A selected row of the WinUI grid shows the pill of TableView at its left, faded in, on the tint of a selected row.")]
        public void AWinUIRowShowsThePillWhenSelected()
        {
            var grid = this.GridFor("MahApps.Styles.DataGrid.WinUI");
            var row = FirstRowOf(grid);
            var pill = PillOf(row);

            var before = pill.Opacity;

            grid.SelectedIndex = 0;
            // the fade runs on the clock, so the test waits for its end rather than for a while
            ClipAssert.PumpUntil(() => pill.Opacity >= 1, 1000);

            var background = row.FindChild<Border>("RowBackgroundBorder");

            Assert.Multiple(() =>
                {
                    Assert.That(before, Is.EqualTo(0d), "before the selection");
                    Assert.That(pill.Opacity, Is.EqualTo(1d), "and after it");
                    Assert.That(background, Is.Not.Null, "the row should paint its background on RowBackgroundBorder");
                    Assert.That(background!.Background, Is.SameAs(this.window!.FindResource("MahApps.Brushes.DataGrid.WinUI.RowBackgroundSelected")));
                });

            row.IsEnabled = false;
            ClipAssert.Pump();

            Assert.That(pill.Fill, Is.SameAs(this.window!.FindResource("MahApps.Brushes.DataGrid.WinUI.SelectionIndicatorDisabled")), "a selected row that is disabled shows the pill in the disabled colour");
        }

        [Test]
        [Description("Every row of an extended selection carries a pill of its own, and a row taken out of it loses its pill.")]
        public void EveryRowOfAnExtendedSelectionCarriesItsOwnPill()
        {
            var grid = this.GridFor("MahApps.Styles.DataGrid.WinUI", items: new List<Row> { new(), new(), new() });
            var rows = RowsOf(grid);
            Assert.That(rows, Has.Length.EqualTo(3), "the grid should have built three rows");

            foreach (var item in grid.Items.OfType<Row>())
            {
                grid.SelectedItems.Add(item);
            }

            ClipAssert.PumpUntil(() => rows.All(r => PillOf(r).Opacity >= 1), 1000);
            grid.SelectedItems.Remove(grid.Items[1]);
            ClipAssert.PumpUntil(() => PillOf(rows[1]).Opacity <= 0, 1000);

            Assert.Multiple(() =>
                {
                    Assert.That(PillOf(rows[0]).Opacity, Is.EqualTo(1d), "the first row stays selected");
                    Assert.That(PillOf(rows[1]).Opacity, Is.EqualTo(0d), "the middle one was taken out");
                    Assert.That(PillOf(rows[2]).Opacity, Is.EqualTo(1d), "and the third stays selected");
                });
        }

        [Test]
        [Description("A column header answers the mouse only where a click on it sorts, as TableView has it, and shows no sort glyph while it cannot sort.")]
        public void AColumnThatCannotSortDoesNotAnswerTheMouse()
        {
            var grid = this.GridFor("MahApps.Styles.DataGrid.WinUI");
            grid.Columns[1].CanUserSort = false;
            // sorted from code, which a column that cannot sort still allows, so that a glyph would be there to show
            grid.Columns[1].SortDirection = System.ComponentModel.ListSortDirection.Descending;
            this.window!.UpdateLayout();
            ClipAssert.Pump();

            var headers = HeadersOf(grid);
            headers[0].SetValue(MouseOverKey, true);
            headers[1].SetValue(MouseOverKey, true);
            ClipAssert.Pump();

            var pointerOver = this.window.FindResource("MahApps.Brushes.DataGrid.WinUI.HeaderBackgroundPointerOver");

            Assert.Multiple(() =>
                {
                    Assert.That(headers[0].FindChildren<Border>(true).Select(b => b.Background), Has.Some.SameAs(pointerOver), "a column that sorts answers the mouse");
                    Assert.That(headers[1].FindChildren<Border>(true).Select(b => b.Background), Has.None.SameAs(pointerOver), "one that cannot sort does not");
                    Assert.That(headers[1].FindChild<Grid>("SortIndicator")!.Visibility, Is.EqualTo(Visibility.Collapsed), "and leaves out the box of the sort indicator");
                    Assert.That(headers[1].FindChild<TextBlock>("SortGlyph")!.IsVisible, Is.False, "so it shows no glyph, sorted or not");
                });
        }

        public class LongRow
        {
            public string ANameFarLongerThanItsColumn { get; set; } = "one";

            public string Second { get; set; } = "two";
        }

        [Test]
        [Description("The name of a column too narrow for it trims before the sort indicator rather than being cut off by it.")]
        public void TheHeaderNameTrimsBeforeTheSortIndicator()
        {
            var grid = this.GridFor("MahApps.Styles.DataGrid.WinUI", items: new List<LongRow> { new(), new() });
            grid.Columns[0].Width = 100;
            grid.Columns[0].SortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this.window!.UpdateLayout();
            ClipAssert.Pump();

            var header = HeadersOf(grid)[0];
            var box = header.FindChild<Grid>("SortIndicator")!;
            var text = header.FindChildren<TextBlock>(true).Single(t => t.Name != "SortGlyph");

            Assert.Multiple(() =>
                {
                    Assert.That(text.TextTrimming, Is.EqualTo(TextTrimming.CharacterEllipsis), "the name trims");
                    Assert.That(text.TranslatePoint(new Point(text.ActualWidth, 0), header).X, Is.LessThanOrEqualTo(box.TranslatePoint(default, header).X + 0.5), "and ends where the sort indicator begins");
                });
        }

        [Test]
        [Description("A header that is an element rather than a name shows that element, as it does in any other grid, since only a name gets the text that trims.")]
        public void AnElementForAHeaderStaysAnElement()
        {
            var grid = this.GridFor("MahApps.Styles.DataGrid.WinUI");
            var panel = new StackPanel { Orientation = Orientation.Horizontal, Children = { new TextBlock { Text = "42" } } };
            grid.Columns[0].Header = panel;
            this.window!.UpdateLayout();
            ClipAssert.Pump();

            Assert.That(HeadersOf(grid)[0].FindChildren<StackPanel>(true), Has.Member(panel));
        }

        [Test]
        [Description("A header set to upper case still shows its name in capitals in the text that trims.")]
        public void AnUpperCaseHeaderStaysUpperCase()
        {
            var grid = this.GridFor("MahApps.Styles.DataGrid.WinUI");
            var header = HeadersOf(grid)[0];
            header.SetValue(ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Upper);
            this.window!.UpdateLayout();
            ClipAssert.Pump();

            var text = header.FindChildren<TextBlock>(true).Single(t => t.Name != "SortGlyph");

            Assert.Multiple(() =>
                {
                    Assert.That(text.Text, Is.EqualTo("FIRST"));
                    Assert.That(text.TextTrimming, Is.EqualTo(TextTrimming.CharacterEllipsis), "and it still trims");
                });
        }

        [Test]
        [Description("Where the columns fill the grid, no line between two headers is left over against the frame of the grid, the way TableView leaves out the last one.")]
        public void TheLastHeaderDrawsNoLineAgainstTheBorder()
        {
            var grid = this.GridFor("MahApps.Styles.DataGrid.WinUI");
            grid.Columns[grid.Columns.Count - 1].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            this.window!.UpdateLayout();
            ClipAssert.Pump();

            var right = grid.ActualWidth - grid.BorderThickness.Right;
            var lines = grid.FindChildren<DataGridColumnHeader>(true)
                            .Where(h => h.ActualWidth >= 1)
                            .Select(h => h.FindChild<Rectangle>("HeaderSeparator"))
                            .Where(l => l is not null && l.IsVisible && l.ActualWidth > 0)
                            .Select(l => l!.TranslatePoint(new Point(l.ActualWidth, 0), grid).X)
                            .ToArray();

            Assert.Multiple(() =>
                {
                    Assert.That(lines, Has.Length.EqualTo(grid.Columns.Count - 1), "every header but the first draws the line before it");
                    Assert.That(lines, Has.All.LessThan(right - 0.5), "and none of them stands against the frame");
                });
        }

        // the headers that draw the line before them, by the name of their column
        private static string[] HeadersWithALeadingLine(DataGrid grid)
        {
            return HeadersOf(grid)
                   .Where(h => h.IsVisible && h.FindChild<Rectangle>("HeaderSeparator") is { IsVisible: true })
                   .Select(h => (string)h.Column.Header)
                   .ToArray();
        }

        [Test]
        [Description("A hidden first column hands the place of the first to the next header, so that one draws no line against the frame, and takes it back once it shows again.")]
        public void TheFirstVisibleHeaderDrawsNoLeadingLine()
        {
            var grid = this.GridFor("MahApps.Styles.DataGrid.WinUI");

            Assert.That(HeadersWithALeadingLine(grid), Is.EqualTo(new[] { "Second", "Third" }), "every header but the first draws the line before it");

            grid.Columns[0].Visibility = Visibility.Collapsed;
            this.window!.UpdateLayout();
            ClipAssert.Pump();

            Assert.Multiple(() =>
                {
                    Assert.That(HeadersWithALeadingLine(grid), Is.EqualTo(new[] { "Third" }), "with the first column hidden the second is first on screen and draws none");
                    Assert.That(DataGridHelper.GetIsFirstVisibleColumn(grid.Columns[1]), Is.True, "since the second column is the first one showing");
                    Assert.That(DataGridHelper.GetIsFirstVisibleColumn(grid.Columns[0]), Is.False, "and the hidden one is not");
                });

            grid.Columns[0].Visibility = Visibility.Visible;
            this.window.UpdateLayout();
            ClipAssert.Pump();

            Assert.That(HeadersWithALeadingLine(grid), Is.EqualTo(new[] { "Second", "Third" }), "showing the first column again gives the second its line back");
        }

        [Test]
        [Description("A column moved to the front takes the place of the first header with it.")]
        public void TheLeadingLineFollowsTheColumnsWhenTheyMove()
        {
            var grid = this.GridFor("MahApps.Styles.DataGrid.WinUI");

            grid.Columns[2].DisplayIndex = 0;
            this.window!.UpdateLayout();
            ClipAssert.Pump();

            Assert.Multiple(() =>
                {
                    Assert.That(HeadersOf(grid).Select(h => (string)h.Column.Header), Is.EqualTo(new[] { "Third", "First", "Second" }), "the third column should stand in front");
                    Assert.That(HeadersWithALeadingLine(grid), Is.EqualTo(new[] { "First", "Second" }), "and draw no line before it, where the others do");
                });

            grid.Columns[2].Visibility = Visibility.Collapsed;
            this.window.UpdateLayout();
            ClipAssert.Pump();

            Assert.That(HeadersWithALeadingLine(grid), Is.EqualTo(new[] { "Second" }), "hiding the column in front makes the next one first");
        }

        [Test]
        [Description("Columns generated anew for other items are looked after as well, and the ones that went away are let go.")]
        public void TheFirstVisibleColumnIsFoundAgainWhenTheColumnsAreGeneratedAnew()
        {
            var grid = this.GridFor("MahApps.Styles.DataGrid.WinUI");
            var old = grid.Columns.ToArray();

            grid.ItemsSource = new List<Todo> { new(), new() };
            this.window!.UpdateLayout();
            ClipAssert.Pump();

            Assert.Multiple(() =>
                {
                    Assert.That(grid.Columns, Has.Count.EqualTo(1), "the grid should have generated the one column of a Todo");
                    Assert.That(DataGridHelper.GetIsFirstVisibleColumn(grid.Columns[0]), Is.True, "which is the first one showing");
                    Assert.That(old.Select(DataGridHelper.GetIsFirstVisibleColumn), Has.All.False, "and the columns that went away are no first column any more");
                });

            grid.Columns.Add(new DataGridTextColumn { Header = "Late", DisplayIndex = 0 });
            this.window.UpdateLayout();
            ClipAssert.Pump();

            Assert.Multiple(() =>
                {
                    Assert.That(DataGridHelper.GetIsFirstVisibleColumn(grid.Columns[1]), Is.True, "a column added in front is first");
                    Assert.That(DataGridHelper.GetIsFirstVisibleColumn(grid.Columns[0]), Is.False, "and the one before it no more");
                });
        }

        [Test]
        [Description("Right after load the first column is marked, before any column has moved or been hidden.")]
        public void TheFirstColumnIsMarkedOnceTheGridIsLoaded()
        {
            var grid = this.GridFor("MahApps.Styles.DataGrid.WinUI");

            Assert.That(grid.Columns.Select(DataGridHelper.GetIsFirstVisibleColumn), Is.EqualTo(new[] { true, false, false }), "only the first column should be marked");
        }

        [TestCase(true)]
        [TestCase(false)]
        [Description("A first column hidden before the grid is loaded hands the place of the first to the next one from the start, whether the columns are written down or generated.")]
        public void AFirstColumnHiddenBeforeLoadIsSkippedFromTheStart(bool generated)
        {
            var grid = this.GridFor(
                "MahApps.Styles.DataGrid.WinUI",
                prepare: g =>
                    {
                        if (generated)
                        {
                            g.AutoGeneratingColumn += (_, e) => e.Column.Visibility = e.PropertyName == nameof(Row.First) ? Visibility.Collapsed : Visibility.Visible;
                            return;
                        }

                        g.AutoGenerateColumns = false;
                        foreach (var name in new[] { nameof(Row.First), nameof(Row.Second), nameof(Row.Third) })
                        {
                            g.Columns.Add(new DataGridTextColumn { Header = name, Binding = new Binding(name) });
                        }

                        g.Columns[0].Visibility = Visibility.Collapsed;
                    });

            Assert.Multiple(() =>
                {
                    Assert.That(grid.Columns.Select(DataGridHelper.GetIsFirstVisibleColumn), Is.EqualTo(new[] { false, true, false }), "the second column should be marked as the first one showing");
                    Assert.That(HeadersWithALeadingLine(grid), Is.EqualTo(new[] { "Third" }), "and its header draw no line before it");
                });
        }

        [Test]
        [Description("A grid that stops tracking leaves no column marked.")]
        public void SwitchingTheTrackingOffLeavesNoColumnMarked()
        {
            var grid = this.GridFor("MahApps.Styles.DataGrid.WinUI");

            // a hidden column let go of last is the one whose visibility falls back last
            grid.Columns[0].Visibility = Visibility.Collapsed;
            grid.Columns[2].Visibility = Visibility.Collapsed;
            ClipAssert.Pump();

            DataGridHelper.SetTracksFirstVisibleColumn(grid, false);
            ClipAssert.Pump();

            Assert.That(grid.Columns.Select(DataGridHelper.GetIsFirstVisibleColumn), Has.All.False);
        }

        [Test]
        [Description("A grid whose WinUI header is first on screen takes the handle for resizing away from its left edge as well, the way the header at DisplayIndex 0 does.")]
        public void TheFirstVisibleHeaderHasNoLeftGripper()
        {
            var grid = this.GridFor("MahApps.Styles.DataGrid.WinUI");
            grid.Columns[0].Visibility = Visibility.Collapsed;
            this.window!.UpdateLayout();
            ClipAssert.Pump();

            var second = HeadersOf(grid).Single(h => (string)h.Column.Header == "Second");

            Assert.That(second.FindChild<Thumb>("PART_LeftHeaderGripper")?.IsVisible, Is.False);
        }

        [Test]
        [Description("A grid whose style stands on the Metro one keeps Metro headers, rows and cells inside the WinUI set, since the set names the DataGrid alone.")]
        public void AGridOnTheMetroStyleKeepsTheMetroLookInsideTheSet()
        {
            var set = new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/WinUI/Controls.xaml", UriKind.Absolute) };
            this.window!.Resources.MergedDictionaries.Add(set);

            try
            {
                var grid = new DataGrid
                           {
                               Style = new Style(typeof(DataGrid), (Style)this.window.FindResource("MahApps.Styles.DataGrid")),
                               Width = 400,
                               Height = 200,
                               AutoGenerateColumns = true,
                               ItemsSource = new List<Row> { new(), new() }
                           };

                this.window.Content = grid;
                this.window.UpdateLayout();
                ClipAssert.PumpUntil(() => grid.FindChildren<DataGridRow>(true).Any());

                Assert.Multiple(() =>
                    {
                        Assert.That(StandsOn(HeadersOf(grid).First().Style, (Style)this.window.FindResource("MahApps.Styles.DataGridColumnHeader")), Is.True, "the header");
                        Assert.That(StandsOn(FirstRowOf(grid).Style, (Style)this.window.FindResource("MahApps.Styles.DataGridRow")), Is.True, "the row");
                        Assert.That(StandsOn(CellOf(grid).Style, (Style)this.window.FindResource("MahApps.Styles.DataGridCell")), Is.True, "the cell");
                    });
            }
            finally
            {
                this.window.Content = null;
                this.window.Resources.MergedDictionaries.Remove(set);
            }
        }

        [Test]
        [Description("The pill follows the theme: in a dark one it takes the lighter shade of the accent Windows gives it there.")]
        public void ThePillFollowsTheThemeInDark()
        {
            ThemeManager.Current.ChangeTheme(Application.Current, "Dark.Blue");

            try
            {
                var grid = this.GridFor("MahApps.Styles.DataGrid.WinUI");
                grid.SelectedIndex = 0;
                ClipAssert.Pump(200);

                var fill = PillOf(FirstRowOf(grid)).Fill as SolidColorBrush;

                Assert.That(fill?.Color, Is.EqualTo((Color)Application.Current.FindResource("MahApps.Colors.SystemAccentLight2")));
            }
            finally
            {
                ThemeManager.Current.ChangeTheme(Application.Current, "Light.Blue");
            }
        }

        [TestCase("MahApps.Styles.DataGrid.Win10")]
        [TestCase("MahApps.Styles.DataGrid.WinUI")]
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
        [TestCase("MahApps.Styles.DataGrid.WinUI")]
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
        [TestCase("MahApps.Styles.DataGrid.WinUI", "\uE96D")]
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
        [TestCase("MahApps.Styles.DataGrid.WinUI", "MahApps.Styles.TextBox.WinUI")]
        [Description("A text column the grid made itself edits in the text box of the set.")]
        public void AnAutoGeneratedTextColumnEditsInTheBoxOfTheSet(string gridKey, string boxKey)
        {
            var grid = this.GridFor(gridKey);

            var column = (DataGridTextColumn)grid.Columns[0];

            Assert.That(StandsOn(column.EditingElementStyle, (Style)this.window!.FindResource(boxKey)), Is.True, $"the editing style should stand on {boxKey}");
        }

        public class Order
        {
            public string Name { get; set; } = "Earl Grey";

            public DayOfWeek Day { get; set; } = DayOfWeek.Friday;

            public Uri Link { get; set; } = new("https://mahapps.com");

            public double Measurement { get; set; } = 2.5;

            public decimal Price { get; set; } = 19.99m;

            public int Count { get; set; } = 3;

            public long Size { get; set; } = 9007199254740993L;
        }

        // the numbers get the columns of this library instead of the text column WPF would make for them, the way DataGridUpDownColumnTests has them
        private static void UpDownColumnsForNumbers(object? sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var binding = new Binding(e.PropertyName);
            DataGridColumn? column = e.PropertyType == typeof(double) ? new DataGridNumericUpDownColumn { Binding = binding }
                : e.PropertyType == typeof(decimal) ? new DataGridDecimalUpDownColumn { Binding = binding }
                : e.PropertyType == typeof(int) ? new DataGridIntegerUpDownColumn { Binding = binding }
                : e.PropertyType == typeof(long) ? new DataGridLongUpDownColumn { Binding = binding }
                : null;

            if (column is not null)
            {
                column.Header = e.Column.Header;
                e.Column = column;
            }
        }

        [TestCase("MahApps.Styles.DataGrid.Win10", nameof(Order.Name), typeof(TextBox), "MahApps.Styles.TextBox.DataGrid.Editing.Win10")]
        [TestCase("MahApps.Styles.DataGrid.Win10", nameof(Order.Day), typeof(ComboBox), "MahApps.Styles.ComboBox.DataGrid.Editing.Win10")]
        [TestCase("MahApps.Styles.DataGrid.Win10", nameof(Order.Link), typeof(TextBox), "MahApps.Styles.TextBox.DataGrid.Editing.Win10")]
        [TestCase("MahApps.Styles.DataGrid.Win10", nameof(Order.Measurement), typeof(NumericUpDown), "MahApps.Styles.NumericUpDown.DataGrid.Editing.Win10")]
        [TestCase("MahApps.Styles.DataGrid.Win10", nameof(Order.Price), typeof(DecimalUpDown), "MahApps.Styles.DecimalUpDown.DataGrid.Editing.Win10")]
        [TestCase("MahApps.Styles.DataGrid.Win10", nameof(Order.Count), typeof(IntegerUpDown), "MahApps.Styles.IntegerUpDown.DataGrid.Editing.Win10")]
        [TestCase("MahApps.Styles.DataGrid.Win10", nameof(Order.Size), typeof(LongUpDown), "MahApps.Styles.LongUpDown.DataGrid.Editing.Win10")]
        [TestCase("MahApps.Styles.DataGrid.WinUI", nameof(Order.Name), typeof(TextBox), "MahApps.Styles.TextBox.DataGrid.Editing.WinUI")]
        [TestCase("MahApps.Styles.DataGrid.WinUI", nameof(Order.Day), typeof(ComboBox), "MahApps.Styles.ComboBox.DataGrid.Editing.WinUI")]
        [TestCase("MahApps.Styles.DataGrid.WinUI", nameof(Order.Link), typeof(TextBox), "MahApps.Styles.TextBox.DataGrid.Editing.WinUI")]
        [TestCase("MahApps.Styles.DataGrid.WinUI", nameof(Order.Measurement), typeof(NumericUpDown), "MahApps.Styles.NumericUpDown.DataGrid.Editing.WinUI")]
        [TestCase("MahApps.Styles.DataGrid.WinUI", nameof(Order.Price), typeof(DecimalUpDown), "MahApps.Styles.DecimalUpDown.DataGrid.Editing.WinUI")]
        [TestCase("MahApps.Styles.DataGrid.WinUI", nameof(Order.Count), typeof(IntegerUpDown), "MahApps.Styles.IntegerUpDown.DataGrid.Editing.WinUI")]
        [TestCase("MahApps.Styles.DataGrid.WinUI", nameof(Order.Size), typeof(LongUpDown), "MahApps.Styles.LongUpDown.DataGrid.Editing.WinUI")]
        [Description("A cell being edited holds the control of its column in the editing style of the set, for every kind of column the grid hands a style to.")]
        public void ACellIsEditedInTheControlOfTheSet(string gridKey, string property, Type controlType, string styleKey)
        {
            var grid = this.GridFor(gridKey, items: new List<Order> { new(), new() }, prepare: g => g.AutoGeneratingColumn += UpDownColumnsForNumbers);
            var item = grid.Items[0];
            var column = grid.Columns.Single(c => Equals(c.Header, property));

            grid.CurrentCell = new DataGridCellInfo(item, column);
            grid.BeginEdit();
            this.window!.UpdateLayout();

            try
            {
                var editor = column.GetCellContent(item);

                Assert.That(editor, Is.TypeOf(controlType), "the cell should be edited in the control of its column");
                Assert.That(StandsOn(editor!.Style, (Style)this.window.FindResource(styleKey)), Is.True, $"which should stand on {styleKey}");
            }
            finally
            {
                grid.CancelEdit(DataGridEditingUnit.Row);
            }
        }

        [TestCase("MahApps.Styles.DataGrid.Win10", nameof(Order.Name), "MahApps.Styles.TextBlock.DataGrid.Win10")]
        [TestCase("MahApps.Styles.DataGrid.Win10", nameof(Order.Measurement), "MahApps.Styles.TextBlock.NumericUpDown.DataGrid.Win10")]
        [TestCase("MahApps.Styles.DataGrid.WinUI", nameof(Order.Name), "MahApps.Styles.TextBlock.DataGrid.WinUI")]
        [TestCase("MahApps.Styles.DataGrid.WinUI", nameof(Order.Measurement), "MahApps.Styles.TextBlock.NumericUpDown.DataGrid.WinUI")]
        [Description("A cell that only shows its value writes it in the text block of its own set.")]
        public void AShowingCellWritesInTheTextBlockOfItsSet(string gridKey, string property, string styleKey)
        {
            var grid = this.GridFor(gridKey, items: new List<Order> { new(), new() }, prepare: g => g.AutoGeneratingColumn += UpDownColumnsForNumbers);
            var column = (DataGridBoundColumn)grid.Columns.Single(c => Equals(c.Header, property));

            Assert.That(column.ElementStyle, Is.SameAs(this.window!.FindResource(styleKey)));
        }

        [TestCase("MahApps.Styles.DataGrid.Win10")]
        [TestCase("MahApps.Styles.DataGrid.WinUI")]
        [Description("GH-3966 under the sets: the clear button in the box of a cell empties the box, and the object only gets the empty text when the grid commits.")]
        public void ClearingACellLeavesTheObjectAloneUntilTheGridCommits(string gridKey)
        {
            var grid = this.GridFor(gridKey, items: new System.Collections.ObjectModel.ObservableCollection<Views.DataGridClearButtonRow> { new() });
            var row = (Views.DataGridClearButtonRow)grid.Items[0];
            var column = grid.Columns[0];

            grid.CurrentCell = new DataGridCellInfo(row, column);
            grid.BeginEdit();
            this.window!.UpdateLayout();

            var editor = column.GetCellContent(row) as TextBox;
            Assert.That(editor, Is.Not.Null, "the cell should be edited in a text box");
            Assert.That(editor!.Text, Is.EqualTo("123"));
            Assert.That(editor.FindChild<Button>("PART_ClearText"), Is.Not.Null, "the box of the set should have a clear button");

            ((System.Windows.Input.RoutedCommand)MahAppsCommands.ClearControlCommand).Execute(null, editor);
            this.window.UpdateLayout();

            Assert.That(editor.Text, Is.Empty, "the box is what the button clears");
            Assert.That(row.Value, Is.EqualTo("123"), "and the object keeps what it had until the grid says so");

            grid.CommitEdit(DataGridEditingUnit.Row, true);

            Assert.That(row.Value, Is.Empty, "the commit is what hands the empty value over");
        }

        [TestCase("MahApps.Styles.DataGrid.Win10", "GridLine")]
        [TestCase("MahApps.Styles.DataGrid.WinUI", "HeaderSeparator")]
        [Description("The line under the header row is see-through, so it is drawn once: by the header WPF puts behind the others across the whole width, with no header of a column covering it or drawing it a second time.")]
        public void TheLineUnderTheHeadersIsDrawnOnceAcrossTheWholeWidth(string gridKey, string brush)
        {
            var grid = this.GridFor(gridKey);

            var filler = FillerOf(grid);
            var rule = this.window!.FindResource(BrushOf(gridKey, brush));

            Assert.That(filler, Is.Not.Null, "the grid should have a header after its last column");
            var behind = filler!.FindChild<Border>("BackgroundBorder");

            Assert.Multiple(() =>
                {
                    Assert.That(filler.ActualWidth, Is.EqualTo(grid.ActualWidth - grid.BorderThickness.Left - grid.BorderThickness.Right).Within(1), "the header behind the others spans the whole row");
                    Assert.That(behind!.BorderThickness.Bottom, Is.EqualTo(1), "and draws the line");
                    Assert.That(behind.BorderBrush, Is.SameAs(rule), $"in {brush}");

                    foreach (var header in HeadersOf(grid))
                    {
                        var border = header.FindChild<Border>("BackgroundBorder");
                        Assert.That(border!.BorderThickness.Bottom, Is.EqualTo(0), $"the header of {header.Column.Header} draws no second line");
                        Assert.That(border.TranslatePoint(new Point(0, border.ActualHeight), header).Y, Is.LessThanOrEqualTo(header.ActualHeight - 0.5), "and its fill leaves the line showing, a device pixel of it at any scale");
                    }
                });
        }

        [TestCase("MahApps.Styles.DataGrid")]
        [TestCase("MahApps.Styles.DataGrid.Azure")]
        [TestCase("MahApps.Styles.DataGrid.Win10")]
        [TestCase("MahApps.Styles.DataGrid.WinUI")]
        [Description("While the vertical scroll bar shows, the header row goes on over it to the right edge, in the look of the header after the last column.")]
        public void TheHeaderRowGoesOnOverTheVerticalScrollBar(string gridKey)
        {
            var grid = this.GridFor(gridKey, items: ManyRows());

            var bar = grid.FindChild<ScrollBar>("PART_VerticalScrollBar");
            var filler = FillerOf(grid);
            var corner = CornerOf(grid);

            Assert.That(bar, Is.Not.Null, "the grid should have a vertical scroll bar");
            Assert.That(bar!.IsVisible, Is.True, "showing for thirty rows");
            Assert.That(filler, Is.Not.Null, "the grid should have a header after its last column");
            Assert.That(corner, Is.Not.Null, "the grid should have a header over the scroll bar");

            var fillerBorder = filler!.FindChild<Border>("BackgroundBorder");
            var cornerBorder = corner!.FindChild<Border>("BackgroundBorder");

            Assert.Multiple(() =>
                {
                    Assert.That(corner.IsVisible, Is.True, "it shows while the bar does");
                    Assert.That(corner.IsHitTestVisible, Is.False, "and does not answer the mouse");
                    Assert.That(corner.Focusable, Is.False, "or take the focus");
                    Assert.That(corner.Style, Is.SameAs(grid.ColumnHeaderStyle), "it wears the header style of the grid");
                    Assert.That(corner.ActualWidth, Is.EqualTo(bar.ActualWidth).Within(0.5), "as wide as the bar");
                    Assert.That(corner.ActualHeight, Is.EqualTo(filler.ActualHeight).Within(0.5), "and as tall as the header row");
                    Assert.That(corner.TranslatePoint(default, grid).Y, Is.EqualTo(filler.TranslatePoint(default, grid).Y).Within(0.5), "standing in line with it");
                    Assert.That(corner.TranslatePoint(new Point(corner.ActualWidth, 0), grid).X, Is.EqualTo(grid.ActualWidth - grid.BorderThickness.Right).Within(0.5), "up to the right edge");
                    Assert.That(cornerBorder, Is.Not.Null, "it draws through a BackgroundBorder");
                    Assert.That(cornerBorder!.Background, Is.SameAs(fillerBorder!.Background), "filled like the header after the last column");
                    Assert.That(cornerBorder.BorderBrush, Is.SameAs(fillerBorder.BorderBrush), "with the same rule");
                    Assert.That(cornerBorder.BorderThickness, Is.EqualTo(fillerBorder.BorderThickness), "as thick");
                    Assert.That(cornerBorder.Margin, Is.EqualTo(fillerBorder.Margin), "and as far in");
                });
        }

        // the right edges of the gripper lines on show in the header row, in the coordinates of the grid
        private static double[] GripperLinesOf(DataGrid grid)
        {
            return grid.FindChildren<DataGridColumnHeader>(true)
                       .Where(h => h.IsVisible && h.ActualWidth > 0)
                       .Select(h => h.FindChild<Thumb>("PART_RightHeaderGripper"))
                       .Where(t => t is not null && t.IsVisible && t.ActualWidth > 0)
                       .Select(t => t!.TranslatePoint(new Point(t.ActualWidth, 0), grid).X)
                       .ToArray();
        }

        [TestCase("MahApps.Styles.DataGrid")]
        [TestCase("MahApps.Styles.DataGrid.Azure")]
        [TestCase("MahApps.Styles.DataGrid.Win10")]
        [Description("Over the scroll bar the header row ends the way it ends at the frame without one: no line where the bar begins, and the line of the header after the last column at the frame.")]
        public void TheHeaderRowEndsOverTheBarAsItEndsWithoutOne(string gridKey)
        {
            var without = this.GridFor(gridKey);
            var frame = without.ActualWidth - without.BorderThickness.Right;

            Assert.That(GripperLinesOf(without), Has.Some.EqualTo(frame).Within(0.5), "a grid without a bar ends its header row with a line at the frame");

            var grid = this.GridFor(gridKey, items: ManyRows());
            var bar = grid.FindChild<ScrollBar>("PART_VerticalScrollBar");
            Assert.That(bar?.IsVisible, Is.True, "thirty rows need the bar");

            var barStart = bar!.TranslatePoint(default, grid).X;
            var lines = GripperLinesOf(grid);

            Assert.Multiple(() =>
                {
                    Assert.That(lines, Has.None.EqualTo(barStart).Within(0.5), "no line where the bar begins");
                    Assert.That(lines, Has.Some.EqualTo(grid.ActualWidth - grid.BorderThickness.Right).Within(0.5), "and the line at the frame, as without a bar");
                });

            grid.ItemsSource = new List<Row> { new(), new() };
            this.window!.UpdateLayout();
            ClipAssert.Pump();

            Assert.That(GripperLinesOf(grid), Has.Some.EqualTo(grid.ActualWidth - grid.BorderThickness.Right).Within(0.5), "once the bar goes the header after the last column draws its line at the frame again");
        }

        [TestCase("MahApps.Styles.DataGrid", "MahApps.Styles.DataGridColumnHeader.Win10")]
        [TestCase("MahApps.Styles.DataGrid.Azure", "MahApps.Styles.DataGridColumnHeader.Win10")]
        [TestCase("MahApps.Styles.DataGrid.Win10", "MahApps.Styles.DataGridColumnHeader")]
        [Description("A header style with a template of its own handed over while the bar shows still leaves no line where the bar begins.")]
        public void AHeaderStyleSwappedOverTheBarLeavesNoLineWhereTheBarBegins(string gridKey, string headerKey)
        {
            var grid = this.GridFor(gridKey, items: ManyRows());

            // the row keeps its height, so nothing but the template of the headers changes
            var height = FillerOf(grid)!.ActualHeight;
            var swapped = new Style(typeof(DataGridColumnHeader), (Style)this.window!.FindResource(headerKey));
            swapped.Setters.Add(new Setter(FrameworkElement.HeightProperty, height));
            grid.ColumnHeaderStyle = swapped;
            this.window!.UpdateLayout();
            ClipAssert.Pump(100);

            var bar = grid.FindChild<ScrollBar>("PART_VerticalScrollBar");
            var barStart = bar!.TranslatePoint(default, grid).X;

            Assert.Multiple(() =>
                {
                    Assert.That(FillerOf(grid)!.Style, Is.SameAs(swapped), "the header after the last column should wear the new style");
                    Assert.That(CornerOf(grid)!.Style, Is.SameAs(swapped), "and so should the one over the bar");
                    Assert.That(CornerOf(grid)?.IsVisible, Is.True, "the header over the bar should still show");
                    Assert.That(GripperLinesOf(grid), Has.None.EqualTo(barStart).Within(0.5), "and no line stand where the bar begins");
                });
        }

        [TestCase("MahApps.Styles.DataGrid")]
        [TestCase("MahApps.Styles.DataGrid.Win10")]
        [TestCase("MahApps.Styles.DataGrid.WinUI")]
        [Description("The button that selects all stays as wide as the row headers or nothing while a grid with a frozen column scrolls sideways.")]
        public void TheSelectAllButtonKeepsAWidthWhileTheGridScrollsSideways(string gridKey)
        {
            var grid = this.GridFor(gridKey);

            // the fraction below zero only comes while the thumb is dragged, between two rounds of layout
            var button = grid.FindChildren<Button>(true).FirstOrDefault(b => b.Command == DataGrid.SelectAllCommand);
            Assert.That(button, Is.Not.Null, "the grid should have a button that selects all");

            var converter = BindingOperations.GetBinding(button!, FrameworkElement.WidthProperty)?.Converter;

            Assert.Multiple(() =>
                {
                    Assert.That(converter, Is.InstanceOf<NonNegativeDoubleConverter>(), "the width of the button should go through the converter");
                    Assert.That(converter?.Convert(-0.39665271966485705, typeof(double), null, CultureInfo.InvariantCulture), Is.EqualTo(0d), "an offset below zero should give no width");
                    Assert.That(converter?.Convert(25d, typeof(double), null, CultureInfo.InvariantCulture), Is.EqualTo(25d), "and any other offset its own");
                });
        }

        private static ResourceDictionary SetOf(string name)
        {
            return new ResourceDictionary { Source = new Uri($"pack://application:,,,/MahApps.Metro;component/Styles/{name}Controls.xaml", UriKind.Absolute) };
        }

        [TestCase("", "Win10/", 12d)]
        [TestCase("Win10/", "WinUI/", 8d)]
        [TestCase("WinUI/", "", 10d)]
        [Description("A grid moved into another set, the way the gallery does it, builds its headers anew, and the ones it drops look for no grid.")]
        public void TheHeadersChangeSetsWithoutABindingWarning(string from, string to, double padding)
        {
            var grid = new DataGrid { Width = 400, Height = 200, AutoGenerateColumns = true, ItemsSource = new List<Row> { new(), new() } };
            var host = new Border { Child = grid };
            host.Resources.MergedDictionaries.Add(SetOf(from));
            this.window!.Content = host;
            this.window.UpdateLayout();
            ClipAssert.Pump(100);

            var warnings = BindingWarnings.During(() =>
                {
                    host.Resources.MergedDictionaries.Clear();
                    host.Resources.MergedDictionaries.Add(SetOf(to));
                    this.window.UpdateLayout();
                    ClipAssert.Pump(100);
                });

            Assert.Multiple(() =>
                {
                    Assert.That(warnings, Has.None.Contains("DataGridColumnHeader"), "no header should miss the grid it takes its padding from");
                    Assert.That(HeadersOf(grid).Select(h => h.Padding.Left), Is.All.EqualTo(padding), "the headers should take the padding of the new set");
                });
        }

        [TestCase("MahApps.Styles.DataGrid")]
        [TestCase("MahApps.Styles.DataGrid.Azure")]
        [TestCase("MahApps.Styles.DataGrid.Win10")]
        [TestCase("MahApps.Styles.DataGrid.WinUI")]
        [Description("The header over the scroll bar goes away with the bar, and with the column headers.")]
        public void TheHeaderOverTheScrollBarGoesWithTheBarAndTheHeaders(string gridKey)
        {
            var few = this.GridFor(gridKey);
            var withoutBar = CornerOf(few);

            Assert.That(withoutBar, Is.Not.Null, "the grid should have a header over the scroll bar");
            Assert.That(withoutBar!.Visibility, Is.EqualTo(Visibility.Collapsed), "which is collapsed while two rows need no bar");

            var many = this.GridFor(gridKey, items: ManyRows());
            many.HeadersVisibility = DataGridHeadersVisibility.None;
            this.window!.UpdateLayout();
            ClipAssert.Pump();

            var withoutHeaders = CornerOf(many);

            Assert.Multiple(() =>
                {
                    Assert.That(many.FindChild<ScrollBar>("PART_VerticalScrollBar")?.IsVisible, Is.True, "thirty rows still need the bar");
                    Assert.That(withoutHeaders!.Visibility, Is.EqualTo(Visibility.Collapsed), "but a grid without column headers has no header over it either");
                });

            many.HeadersVisibility = DataGridHeadersVisibility.Row;
            this.window.UpdateLayout();

            Assert.That(withoutHeaders.Visibility, Is.EqualTo(Visibility.Collapsed), "nor does one with row headers only");
        }

        [Test]
        [Description("The fill of the WinUI header row is see-through as well, so the header behind the others lays it once across the whole row and the header of a column lays none.")]
        public void TheWinUIHeaderRowIsFilledOnce()
        {
            var grid = this.GridFor("MahApps.Styles.DataGrid.WinUI");

            var filler = FillerOf(grid);
            var fill = this.window!.FindResource("MahApps.Brushes.DataGrid.WinUI.HeaderBackground");

            Assert.That(filler, Is.Not.Null, "the grid should have a header after its last column");
            var behind = filler!.FindChild<Border>("BackgroundBorder");

            Assert.Multiple(() =>
                {
                    Assert.That(filler.ActualWidth, Is.EqualTo(grid.ActualWidth - grid.BorderThickness.Left - grid.BorderThickness.Right).Within(1), "the header behind the others spans the whole row");
                    Assert.That(behind!.Background, Is.SameAs(fill), "and fills it");

                    foreach (var header in HeadersOf(grid))
                    {
                        var border = header.FindChild<Border>("BackgroundBorder");
                        Assert.That(border!.Background, Is.Not.SameAs(fill), $"the header of {header.Column.Header} lays no second fill over it");
                    }
                });
        }

        [TestCase("MahApps.Styles.DataGrid.Win10", 12d)]
        [TestCase("MahApps.Styles.DataGrid.WinUI", 8d)]
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

        // two groups by First, the second one of two rows
        private static ListCollectionView Grouped()
        {
            var view = new ListCollectionView(new List<Row> { new() { First = "one" }, new() { First = "two" }, new() { First = "two" } });
            view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Row.First)));
            return view;
        }

        private static GroupItem FirstGroupOf(DataGrid grid)
        {
            // the set hands its GroupStyle over once the grid is loaded, and only then are there groups
            ClipAssert.PumpUntil(() => grid.FindChildren<GroupItem>(true).Any());

            var group = grid.FindChildren<GroupItem>(true).FirstOrDefault();
            Assert.That(group, Is.Not.Null, "the grid should have built a group");
            return group!;
        }

        [TestCase("MahApps.Styles.DataGrid", "MahApps.Styles.GroupItem.DataGrid")]
        [TestCase("MahApps.Styles.DataGrid.Win10", "MahApps.Styles.GroupItem.DataGrid.Win10")]
        [TestCase("MahApps.Styles.DataGrid.WinUI", "MahApps.Styles.GroupItem.DataGrid.WinUI")]
        [Description("A grouped grid with no GroupStyle of its own is handed the group header of its set.")]
        public void AGroupedGridTakesTheGroupHeaderOfItsSet(string gridKey, string groupKey)
        {
            var grid = this.GridFor(gridKey, items: Grouped());
            var group = FirstGroupOf(grid);
            var expected = (Style)this.window!.FindResource(groupKey);

            Assert.Multiple(() =>
                {
                    Assert.That(grid.GroupStyle, Has.Count.EqualTo(1), "the grid should have been handed one GroupStyle");
                    Assert.That(grid.GroupStyle.FirstOrDefault()?.ContainerStyle, Is.SameAs(expected), "carrying the header of the set");
                    Assert.That(group.Style, Is.SameAs(expected), "and the group should wear it");
                });
        }

        [TestCase("MahApps.Styles.DataGrid", "MahApps.Styles.GroupItem.DataGrid")]
        [TestCase("MahApps.Styles.DataGrid.Win10", "MahApps.Styles.GroupItem.DataGrid.Win10")]
        [TestCase("MahApps.Styles.DataGrid.WinUI", "MahApps.Styles.GroupItem.DataGrid.WinUI")]
        [Description("An application that defines the key of the group header anew gets its own header handed over instead.")]
        public void AGroupHeaderCanBeReplacedByItsKey(string gridKey, string groupKey)
        {
            var own = new Style(typeof(GroupItem), (Style)this.window!.FindResource(groupKey));

            var grid = this.GridFor(gridKey, items: Grouped(), prepare: g => g.Resources[groupKey] = own);
            var group = FirstGroupOf(grid);

            Assert.Multiple(() =>
                {
                    Assert.That(grid.GroupStyle.FirstOrDefault()?.ContainerStyle, Is.SameAs(own), "the grid should have been handed the header of the application");
                    Assert.That(group.Style, Is.SameAs(own), "and the group should wear it");
                });
        }

        [TestCase("MahApps.Styles.DataGrid")]
        [TestCase("MahApps.Styles.DataGrid.Win10")]
        [TestCase("MahApps.Styles.DataGrid.WinUI")]
        [Description("A grouped grid with no HeaderTemplate shows the name of each group in its header.")]
        public void AGroupHeaderShowsTheNameOfTheGroup(string gridKey)
        {
            var grid = this.GridFor(gridKey, items: Grouped());
            var group = FirstGroupOf(grid);
            var name = ((CollectionViewGroup)group.Content).Name;

            ClipAssert.PumpUntil(() => group.FindChildren<TextBlock>(true).Any(t => Equals(t.Text, name)));

            Assert.That(group.FindChildren<TextBlock>(true).Select(t => t.Text), Has.Member(name), "the header should name the group");
        }

        [TestCase("MahApps.Styles.DataGrid.Win10")]
        [TestCase("MahApps.Styles.DataGrid.WinUI")]
        [Description("A GroupStyle the application wrote stays the only one, even when it names the Metro header inside a set.")]
        public void AGroupStyleTheApplicationBroughtStays(string gridKey)
        {
            var own = new GroupStyle { ContainerStyle = (Style)this.window!.FindResource("MahApps.Styles.GroupItem.DataGrid") };

            var grid = this.GridFor(gridKey, items: Grouped(), prepare: g => g.GroupStyle.Add(own));
            ClipAssert.PumpUntil(() => grid.IsLoaded);

            Assert.Multiple(() =>
                {
                    Assert.That(grid.GroupStyle, Has.Count.EqualTo(1), "nothing should have been added");
                    Assert.That(grid.GroupStyle.FirstOrDefault(), Is.SameAs(own), "and the one there is the application's");
                });
        }

        [TestCase("MahApps.Styles.DataGrid.Win10")]
        [TestCase("MahApps.Styles.DataGrid.WinUI")]
        [Description("A grid that shows no groups is handed no GroupStyle, so it keeps the rows it built before it was loaded.")]
        public void AnUngroupedGridKeepsItsRowsAcrossLoaded(string gridKey)
        {
            var grid = new DataGrid
                       {
                           Style = (Style)this.window!.FindResource(gridKey),
                           Width = 400,
                           Height = 200,
                           AutoGenerateColumns = true,
                           ItemsSource = new List<Row> { new(), new() }
                       };

            this.window.Content = grid;
            this.window.UpdateLayout();

            var before = FirstRowOf(grid);

            ClipAssert.PumpUntil(() => grid.IsLoaded);
            this.window.UpdateLayout();

            Assert.Multiple(() =>
                {
                    Assert.That(grid.IsLoaded, Is.True, "the grid should have been loaded");
                    Assert.That(grid.GroupStyle, Has.Count.EqualTo(0), "it should have been handed no GroupStyle");
                    Assert.That(FirstRowOf(grid), Is.SameAs(before), "and should still show the row it built first");
                });
        }

        [TestCase("MahApps.Styles.DataGrid.Win10", "MahApps.Styles.GroupItem.DataGrid.Win10")]
        [TestCase("MahApps.Styles.DataGrid.WinUI", "MahApps.Styles.GroupItem.DataGrid.WinUI")]
        [Description("A grid that starts grouping after it is loaded is handed the group header of its set then.")]
        public void AGridGroupedAfterLoadTakesTheGroupHeaderOfItsSet(string gridKey, string groupKey)
        {
            var view = new ListCollectionView(new List<Row> { new() { First = "one" }, new() { First = "two" } });
            var grid = this.GridFor(gridKey, items: view);
            ClipAssert.PumpUntil(() => grid.IsLoaded);

            Assert.That(grid.GroupStyle, Has.Count.EqualTo(0), "an ungrouped grid should have no GroupStyle");

            view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Row.First)));
            var group = FirstGroupOf(grid);
            var expected = (Style)this.window!.FindResource(groupKey);

            Assert.Multiple(() =>
                {
                    Assert.That(grid.GroupStyle.FirstOrDefault()?.ContainerStyle, Is.SameAs(expected), "the grid should carry the header of the set");
                    Assert.That(group.Style, Is.SameAs(expected), "and the group should wear it");
                });
        }

        [TestCase("MahApps.Styles.DataGrid.Win10")]
        [TestCase("MahApps.Styles.DataGrid.WinUI")]
        [Description("A grouped grid loaded a second time, the way a tab switch does it, keeps the GroupStyle it was handed and the groups it built, rather than building them all anew.")]
        public void AGroupedGridKeepsItsGroupsWhenItIsLoadedAgain(string gridKey)
        {
            var grid = this.GridFor(gridKey, items: Grouped());
            var group = FirstGroupOf(grid);
            var handedOver = grid.GroupStyle.Single();

            this.window!.Content = null;
            ClipAssert.PumpUntil(() => !grid.IsLoaded);
            this.window.Content = grid;
            this.window.UpdateLayout();
            ClipAssert.PumpUntil(() => grid.IsLoaded);
            ClipAssert.Pump();

            Assert.Multiple(() =>
                {
                    Assert.That(grid.GroupStyle, Has.Count.EqualTo(1), "the grid should still carry one GroupStyle");
                    Assert.That(grid.GroupStyle.FirstOrDefault(), Is.SameAs(handedOver), "the one it was handed first");
                    Assert.That(FirstGroupOf(grid), Is.SameAs(group), "and should still show the group it built first");
                });
        }

        [TestCase("MahApps.Styles.DataGrid.Win10")]
        [TestCase("MahApps.Styles.DataGrid.WinUI")]
        [Description("A GroupStyle the application adds after the grid was handed one wins once the grid is loaded again, and the handed-over one is gone.")]
        public void AGroupStyleAddedAfterTheHandOverWinsOnTheNextLoad(string gridKey)
        {
            var grid = this.GridFor(gridKey, items: Grouped());
            FirstGroupOf(grid);
            var handedOver = grid.GroupStyle.Single();

            var own = new GroupStyle { ContainerStyle = (Style)this.window!.FindResource("MahApps.Styles.GroupItem.DataGrid") };
            grid.GroupStyle.Add(own);

            this.window.Content = null;
            ClipAssert.PumpUntil(() => !grid.IsLoaded);
            this.window.Content = grid;
            this.window.UpdateLayout();
            ClipAssert.PumpUntil(() => grid.IsLoaded);
            ClipAssert.Pump();

            Assert.Multiple(() =>
                {
                    Assert.That(grid.GroupStyle, Does.Not.Contain(handedOver), "the handed-over GroupStyle should have been taken back");
                    Assert.That(grid.GroupStyle, Has.Count.EqualTo(1), "leaving one GroupStyle");
                    Assert.That(grid.GroupStyle.FirstOrDefault(), Is.SameAs(own), "and that one the application's");
                });
        }

        [TestCase("MahApps.Styles.DataGrid.Win10")]
        [TestCase("MahApps.Styles.DataGrid.WinUI")]
        [Description("A grid that stops grouping after it is loaded gives the GroupStyle it was handed back.")]
        public void AGridUngroupedAfterLoadGivesTheGroupStyleBack(string gridKey)
        {
            var view = Grouped();
            var grid = this.GridFor(gridKey, items: view);
            FirstGroupOf(grid);

            Assert.That(grid.GroupStyle, Has.Count.EqualTo(1), "a grouped grid should have been handed a GroupStyle");

            view.GroupDescriptions.Clear();
            ClipAssert.PumpUntil(() => grid.GroupStyle.Count == 0);

            Assert.Multiple(() =>
                {
                    Assert.That(grid.GroupStyle, Has.Count.EqualTo(0), "the GroupStyle should have been taken back");
                    Assert.That(RowsOf(grid), Has.Length.EqualTo(3), "and the rows should stand without groups");
                });
        }

        [TestCase("MahApps.Styles.DataGrid.Win10")]
        [TestCase("MahApps.Styles.DataGrid.WinUI")]
        [Description("Closing a group with its header takes its rows away and leaves the header standing.")]
        public void ClosingAGroupHidesItsRows(string gridKey)
        {
            var grid = this.GridFor(gridKey, items: Grouped());
            var group = FirstGroupOf(grid);
            var header = group.FindChild<ToggleButton>("PART_Header");
            var rows = group.FindChild<ItemsPresenter>("ItemsPresenter");

            Assert.That(header, Is.Not.Null, "the group should have a header to click");
            Assert.That(rows, Is.Not.Null, "and its rows below it");
            Assert.That(rows!.Visibility, Is.EqualTo(Visibility.Visible), "an open group shows its rows");

            header!.IsChecked = false;
            ClipAssert.Pump();

            Assert.Multiple(() =>
                {
                    Assert.That(rows.Visibility, Is.EqualTo(Visibility.Collapsed), "a closed group hides them");
                    Assert.That(header.IsVisible, Is.True, "and keeps its header");
                });
        }
    }
}
