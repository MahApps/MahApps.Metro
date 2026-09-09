// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A <see cref="System.Windows.Controls.DataGrid"/> column whose cells hold a
    /// <see cref="double"/>, edited in a <see cref="NumericUpDown"/>.
    /// </summary>
    /// <remarks>
    /// A double is the right thing for a measurement and the wrong one for money, so a column of
    /// prices belongs in a <see cref="DataGridDecimalUpDownColumn"/>.
    /// </remarks>
    public class DataGridNumericUpDownColumn : DataGridNumericUpDownColumnBase<NumericUpDown, double>
    {
        static DataGridNumericUpDownColumn()
        {
            MinimumProperty.OverrideMetadata(typeof(DataGridNumericUpDownColumn), new FrameworkPropertyMetadata(double.MinValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));
            MaximumProperty.OverrideMetadata(typeof(DataGridNumericUpDownColumn), new FrameworkPropertyMetadata(double.MaxValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));
            IntervalProperty.OverrideMetadata(typeof(DataGridNumericUpDownColumn), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));
        }
    }
}
