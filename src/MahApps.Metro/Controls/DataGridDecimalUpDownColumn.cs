// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A <see cref="System.Windows.Controls.DataGrid"/> column whose cells hold a
    /// <see cref="decimal"/>, edited in a <see cref="DecimalUpDown"/>.
    /// </summary>
    /// <remarks>
    /// This is the column for money: a decimal holds 0.1 exactly, so a column of prices added up
    /// comes to the amount on the receipt.
    /// </remarks>
    public class DataGridDecimalUpDownColumn : DataGridNumericUpDownColumnBase<DecimalUpDown, decimal>
    {
        static DataGridDecimalUpDownColumn()
        {
            MinimumProperty.OverrideMetadata(typeof(DataGridDecimalUpDownColumn), new FrameworkPropertyMetadata(decimal.MinValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));
            MaximumProperty.OverrideMetadata(typeof(DataGridDecimalUpDownColumn), new FrameworkPropertyMetadata(decimal.MaxValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));
            IntervalProperty.OverrideMetadata(typeof(DataGridDecimalUpDownColumn), new FrameworkPropertyMetadata(1m, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));
        }
    }
}
