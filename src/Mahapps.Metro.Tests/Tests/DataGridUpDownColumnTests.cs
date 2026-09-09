// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class DataGridUpDownColumnTests
    {
        private DataGridUpDownColumnWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<DataGridUpDownColumnWindow>().ConfigureAwait(false);
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
            // a test that edited a cell leaves the control in it, and the next one would find that.
            // The row has to be let go of as well, or the grid stays in a transaction that refuses
            // to have the items refreshed.
            this.window?.TheGrid.CancelEdit(DataGridEditingUnit.Cell);
            this.window?.TheGrid.CancelEdit(DataGridEditingUnit.Row);
            this.window?.TheGrid.UpdateLayout();
        }

        private FrameworkElement? CellContent(int column)
        {
            Assert.That(this.window, Is.Not.Null);

            var item = this.window.TheGrid.Items[0];
            return this.window.TheGrid.Columns[column].GetCellContent(item);
        }

        /// <summary>Puts the first row of a column into edit mode and hands back what is in the cell.</summary>
        private FrameworkElement? EditingContent(int column)
        {
            Assert.That(this.window, Is.Not.Null);

            var grid = this.window.TheGrid;
            var item = grid.Items[0];

            grid.CancelEdit();
            grid.CurrentCell = new DataGridCellInfo(item, grid.Columns[column]);
            grid.BeginEdit();
            grid.UpdateLayout();

            return grid.Columns[column].GetCellContent(item);
        }

        [Test]
        [Description("A cell that is only showing its value holds text, not a control.")]
        public void AShowingCellHoldsText()
        {
            Assert.Multiple(() =>
                {
                    for (var column = 0; column < 4; column++)
                    {
                        Assert.That(this.CellContent(column), Is.TypeOf<TextBlock>(), "column " + column);
                    }
                });
        }

        [Test]
        [Description("Each column puts the control that holds its own type into a cell being edited.")]
        public void EveryColumnMakesTheControlOfItsTypeToEditIn()
        {
            Assert.Multiple(() =>
                {
                    Assert.That(this.EditingContent(0), Is.TypeOf<NumericUpDown>());
                    Assert.That(this.EditingContent(1), Is.TypeOf<DecimalUpDown>());
                    Assert.That(this.EditingContent(2), Is.TypeOf<IntegerUpDown>());
                    Assert.That(this.EditingContent(3), Is.TypeOf<LongUpDown>());
                });
        }

        [Test]
        [Description("The value shows as the control would have written it, the long one digit for digit.")]
        public void AShowingCellReadsLikeTheControlWould()
        {
            Assert.Multiple(() =>
                {
                    Assert.That(((TextBlock)this.CellContent(0)!).Text, Is.EqualTo("2.5"));
                    Assert.That(((TextBlock)this.CellContent(1)!).Text, Is.EqualTo("19.99"));
                    Assert.That(((TextBlock)this.CellContent(2)!).Text, Is.EqualTo("3"));
                    Assert.That(((TextBlock)this.CellContent(3)!).Text, Is.EqualTo("9007199254740993"));
                });
        }

        [Test]
        [Description("A number reads right aligned in a showing cell, the way the control writes it.")]
        public void AShowingCellReadsRightAligned()
        {
            Assert.Multiple(() =>
                {
                    for (var column = 0; column < 4; column++)
                    {
                        var block = (TextBlock)this.CellContent(column)!;
                        Assert.That(block.TextAlignment, Is.EqualTo(TextAlignment.Right), "column " + column);
                        Assert.That(block.VerticalAlignment, Is.EqualTo(VerticalAlignment.Center), "column " + column);
                    }
                });
        }

        [Test]
        [Description("A TextAlignment set on the column wins over what the style asks for.")]
        public void WhatTheColumnAsksForWinsOverTheStyle()
        {
            Assert.That(this.window, Is.Not.Null);

            var column = (DataGridDecimalUpDownColumn)this.window.TheGrid.Columns[1];

            try
            {
                column.TextAlignment = TextAlignment.Left;
                this.window.TheGrid.Items.Refresh();
                this.window.TheGrid.UpdateLayout();

                Assert.That(((TextBlock)this.CellContent(1)!).TextAlignment, Is.EqualTo(TextAlignment.Left));
            }
            finally
            {
                column.ClearValue(DataGridDecimalUpDownColumn.TextAlignmentProperty);
                this.window.TheGrid.Items.Refresh();
                this.window.TheGrid.UpdateLayout();
            }
        }

        [Test]
        [Description("A StringFormat on the column decides the text of a cell that is only showing.")]
        public void AStringFormatDecidesWhatAShowingCellReads()
        {
            Assert.That(this.window, Is.Not.Null);

            var column = (DataGridDecimalUpDownColumn)this.window.TheGrid.Columns[1];
            column.StringFormat = "{}{0:N2} EUR";
            this.window.TheGrid.UpdateLayout();

            Assert.That(((TextBlock)this.CellContent(1)!).Text, Is.EqualTo("19.99 EUR"));

            column.ClearValue(DataGridDecimalUpDownColumn.StringFormatProperty);
        }

        [Test]
        [Description("The value reaches the cell being edited in the type the row holds, without a double in between.")]
        public void EveryColumnCarriesTheValueOfItsType()
        {
            Assert.Multiple(() =>
                {
                    Assert.That(((NumericUpDown)this.EditingContent(0)!).Value, Is.EqualTo(2.5d));
                    Assert.That(((DecimalUpDown)this.EditingContent(1)!).Value, Is.EqualTo(19.99m));
                    Assert.That(((IntegerUpDown)this.EditingContent(2)!).Value, Is.EqualTo(3));
                    Assert.That(((LongUpDown)this.EditingContent(3)!).Value, Is.EqualTo(9007199254740993L));
                });
        }

        [Test]
        [Description("A column brings the bounds and the step of the type it holds.")]
        public void EveryColumnBringsTheBoundsOfItsType()
        {
            Assert.That(this.window, Is.Not.Null);

            var doubles = (DataGridNumericUpDownColumn)this.window.TheGrid.Columns[0];
            var decimals = (DataGridDecimalUpDownColumn)this.window.TheGrid.Columns[1];
            var ints = (DataGridIntegerUpDownColumn)this.window.TheGrid.Columns[2];
            var longs = (DataGridLongUpDownColumn)this.window.TheGrid.Columns[3];

            Assert.Multiple(() =>
                {
                    Assert.That(doubles.Minimum, Is.EqualTo(double.MinValue));
                    Assert.That(doubles.Maximum, Is.EqualTo(double.MaxValue));
                    Assert.That(doubles.Interval, Is.EqualTo(1d));

                    Assert.That(decimals.Minimum, Is.EqualTo(decimal.MinValue));
                    Assert.That(decimals.Maximum, Is.EqualTo(decimal.MaxValue));
                    Assert.That(decimals.Interval, Is.EqualTo(1m));

                    Assert.That(ints.Minimum, Is.EqualTo(int.MinValue));
                    Assert.That(ints.Maximum, Is.EqualTo(int.MaxValue));
                    Assert.That(ints.Interval, Is.EqualTo(1));

                    Assert.That(longs.Minimum, Is.EqualTo(long.MinValue));
                    Assert.That(longs.Maximum, Is.EqualTo(long.MaxValue));
                    Assert.That(longs.Interval, Is.EqualTo(1L));
                });
        }

        [Test]
        [Description("What is set on the column reaches the control in the cell, typed or not.")]
        public void WhatIsSetOnTheColumnReachesTheControl()
        {
            Assert.That(this.window, Is.Not.Null);

            var column = (DataGridDecimalUpDownColumn)this.window.TheGrid.Columns[1];

            column.Interval = 0.05m;
            column.Maximum = 500m;
            column.StringFormat = "C2";
            column.HideUpDownButtons = true;

            var control = (DecimalUpDown)this.EditingContent(1)!;

            Assert.Multiple(() =>
                {
                    Assert.That(control.Interval, Is.EqualTo(0.05m));
                    Assert.That(control.Maximum, Is.EqualTo(500m));
                    Assert.That(control.StringFormat, Is.EqualTo("C2"));
                    Assert.That(control.HideUpDownButtons, Is.True);
                });
        }

        [Test]
        [Description("Every column takes the style the DataGrid holds for its own kind, for the cell and for editing.")]
        public void EveryColumnTakesTheStyleTheGridHoldsForIt()
        {
            Assert.That(this.window, Is.Not.Null);

            var grid = this.window.TheGrid;

            (DataGridColumn Column, DependencyProperty Cell, DependencyProperty Editing)[] wanted =
                {
                    (grid.Columns[0], DataGridHelper.AutoGeneratedNumericUpDownColumnStyleProperty, DataGridHelper.AutoGeneratedNumericUpDownColumnEditingStyleProperty),
                    (grid.Columns[1], DataGridHelper.AutoGeneratedDecimalUpDownColumnStyleProperty, DataGridHelper.AutoGeneratedDecimalUpDownColumnEditingStyleProperty),
                    (grid.Columns[2], DataGridHelper.AutoGeneratedIntegerUpDownColumnStyleProperty, DataGridHelper.AutoGeneratedIntegerUpDownColumnEditingStyleProperty),
                    (grid.Columns[3], DataGridHelper.AutoGeneratedLongUpDownColumnStyleProperty, DataGridHelper.AutoGeneratedLongUpDownColumnEditingStyleProperty)
                };

            Assert.Multiple(() =>
                {
                    foreach (var (column, cell, editing) in wanted)
                    {
                        var bound = (DataGridBoundColumn)column;
                        var name = column.GetType().Name;

                        Assert.That(bound.ElementStyle, Is.Not.Null, name);
                        Assert.That(bound.ElementStyle, Is.SameAs(grid.GetValue(cell)), name);

                        Assert.That(bound.EditingElementStyle, Is.Not.Null, name + " editing");
                        Assert.That(bound.EditingElementStyle, Is.SameAs(grid.GetValue(editing)), name + " editing");
                    }
                });
        }

        [Test]
        [Description("The style a column hands over fits the control in the cell, so WPF takes it.")]
        public void TheStyleFitsTheControlInTheCell()
        {
            Assert.Multiple(() =>
                {
                    for (var column = 0; column < 4; column++)
                    {
                        var content = this.EditingContent(column);
                        Assert.That(content, Is.Not.Null, "column " + column);
                        Assert.That(content!.Style, Is.Not.Null, content.GetType().Name);
                        Assert.That(content.Style.TargetType.IsInstanceOfType(content), Is.True, content.GetType().Name);
                    }
                });
        }

        [Test]
        [Description("An integer column takes no decimals, the way its control does.")]
        public void AnIntegerColumnTakesNoDecimals()
        {
            Assert.That(this.window, Is.Not.Null);

            var column = (DataGridIntegerUpDownColumn)this.window.TheGrid.Columns[2];

            Assert.That(column.NumericInputMode, Is.EqualTo(NumericInput.Numbers));
            Assert.That(((IntegerUpDown)this.EditingContent(2)!).NumericInputMode, Is.EqualTo(NumericInput.Numbers));
        }
    }
}
