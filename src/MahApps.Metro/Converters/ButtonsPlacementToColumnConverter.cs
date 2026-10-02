// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Converters
{
    /// <summary>
    /// Works out which of the two columns beside the text a button strip stands in, out of
    /// <see cref="TextBoxHelper.ButtonsAlignmentProperty"/> and
    /// <see cref="TextBoxHelper.ButtonsPlacementProperty"/>.
    /// </summary>
    /// <remarks>
    /// Inside means next to the text, so which column that is depends on which side the strip is on:
    /// with the buttons on the right the inner one is the left column, with the buttons on the left
    /// it is the right one. The clear button takes whichever column is left over, which is what
    /// <see cref="ClearButton"/> is for.
    /// </remarks>
    public class ButtonsPlacementToColumnConverter : IMultiValueConverter
    {
        /// <summary>The column of the buttons of <see cref="TextBoxHelper.ButtonsProperty"/>.</summary>
        public static readonly ButtonsPlacementToColumnConverter Instance = new();

        /// <summary>The column of the clear button, which is the other one.</summary>
        public static readonly ButtonsPlacementToColumnConverter ClearButton = new() { ForClearButton = true };

        /// <summary>
        /// Gets or sets whether the converter answers for the clear button instead of the buttons.
        /// </summary>
        public bool ForClearButton { get; set; }

        /// <inheritdoc />
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var onTheLeft = values.Length > 0 && values[0] is ButtonsAlignment.Left;
            var outside = values.Length > 1 && values[1] is ButtonsPlacement.Outside;

            var secondColumn = onTheLeft ^ outside;

            return secondColumn ^ this.ForClearButton ? 1 : 0;
        }

        /// <inheritdoc />
        public object?[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
