// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A <see cref="System.Windows.Controls.DataGrid"/> column whose cells hold a
    /// <see cref="int"/>, edited in a <see cref="IntegerUpDown"/>.
    /// </summary>
    /// <remarks>
    /// For something counted rather than measured. Nothing behind a decimal separator can be typed
    /// into a cell of this column.
    /// </remarks>
    public class DataGridIntegerUpDownColumn : DataGridNumericUpDownColumnBase<IntegerUpDown, int>
    {
        static DataGridIntegerUpDownColumn()
        {
            MinimumProperty.OverrideMetadata(typeof(DataGridIntegerUpDownColumn), new FrameworkPropertyMetadata(int.MinValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));
            MaximumProperty.OverrideMetadata(typeof(DataGridIntegerUpDownColumn), new FrameworkPropertyMetadata(int.MaxValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));
            IntervalProperty.OverrideMetadata(typeof(DataGridIntegerUpDownColumn), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));
            NumericInputModeProperty.OverrideMetadata(typeof(DataGridIntegerUpDownColumn), new FrameworkPropertyMetadata(NumericInput.Numbers, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));
        }
    }
}
