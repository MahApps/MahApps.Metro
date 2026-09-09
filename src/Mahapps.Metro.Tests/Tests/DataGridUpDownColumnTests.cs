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

        private FrameworkElement? CellContent(int column)
        {
            Assert.That(this.window, Is.Not.Null);

            var item = this.window.TheGrid.Items[0];
            return this.window.TheGrid.Columns[column].GetCellContent(item);
        }

        [Test]
        [Description("Each column puts the control that holds its own type into the cell.")]
        public void EveryColumnMakesTheControlOfItsType()
        {
            Assert.Multiple(() =>
                {
                    Assert.That(this.CellContent(0), Is.TypeOf<NumericUpDown>());
                    Assert.That(this.CellContent(1), Is.TypeOf<DecimalUpDown>());
                    Assert.That(this.CellContent(2), Is.TypeOf<IntegerUpDown>());
                    Assert.That(this.CellContent(3), Is.TypeOf<LongUpDown>());
                });
        }

        [Test]
        [Description("The value reaches the cell in the type the row holds, without a double in between.")]
        public void EveryColumnCarriesTheValueOfItsType()
        {
            Assert.Multiple(() =>
                {
                    Assert.That(((NumericUpDown)this.CellContent(0)!).Value, Is.EqualTo(2.5d));
                    Assert.That(((DecimalUpDown)this.CellContent(1)!).Value, Is.EqualTo(19.99m));
                    Assert.That(((IntegerUpDown)this.CellContent(2)!).Value, Is.EqualTo(3));
                    Assert.That(((LongUpDown)this.CellContent(3)!).Value, Is.EqualTo(9007199254740993L));
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

            var control = (DecimalUpDown)this.CellContent(1)!;

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
                        var content = this.CellContent(column);
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
            Assert.That(((IntegerUpDown)this.CellContent(2)!).NumericInputMode, Is.EqualTo(NumericInput.Numbers));
        }
    }
}
