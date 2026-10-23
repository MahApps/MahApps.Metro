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
using System.Windows.Media;
using System.Windows.Shapes;
using ControlzEx.Theming;
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

        [TestCase("MahApps.Styles.DataGrid.Win10", "GridLine")]
        [TestCase("MahApps.Styles.DataGrid.WinUI", "HeaderSeparator")]
        [Description("The line under the header row is see-through, so it is drawn once: by the header WPF puts behind the others across the whole width, with no header of a column covering it or drawing it a second time.")]
        public void TheLineUnderTheHeadersIsDrawnOnceAcrossTheWholeWidth(string gridKey, string brush)
        {
            var grid = this.GridFor(gridKey);

            var filler = grid.FindChildren<DataGridColumnHeader>(true).FirstOrDefault(h => h.Column is null);
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

        [Test]
        [Description("The fill of the WinUI header row is see-through as well, so the header behind the others lays it once across the whole row and the header of a column lays none.")]
        public void TheWinUIHeaderRowIsFilledOnce()
        {
            var grid = this.GridFor("MahApps.Styles.DataGrid.WinUI");

            var filler = grid.FindChildren<DataGridColumnHeader>(true).FirstOrDefault(h => h.Column is null);
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
