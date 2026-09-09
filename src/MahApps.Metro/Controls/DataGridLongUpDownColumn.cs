// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A <see cref="System.Windows.Controls.DataGrid"/> column whose cells hold a
    /// <see cref="long"/>, edited in a <see cref="LongUpDown"/>.
    /// </summary>
    /// <remarks>
    /// The same as a <see cref="DataGridIntegerUpDownColumn"/>, for the counts that run past two
    /// billion: a file size in bytes, an identifier out of a database.
    /// </remarks>
    public class DataGridLongUpDownColumn : DataGridNumericUpDownColumnBase<LongUpDown, long>
    {
        static DataGridLongUpDownColumn()
        {
            MinimumProperty.OverrideMetadata(typeof(DataGridLongUpDownColumn), new FrameworkPropertyMetadata(long.MinValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));
            MaximumProperty.OverrideMetadata(typeof(DataGridLongUpDownColumn), new FrameworkPropertyMetadata(long.MaxValue, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));
            IntervalProperty.OverrideMetadata(typeof(DataGridLongUpDownColumn), new FrameworkPropertyMetadata(1L, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));
            NumericInputModeProperty.OverrideMetadata(typeof(DataGridLongUpDownColumn), new FrameworkPropertyMetadata(NumericInput.Numbers, FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));
        }
    }
}
