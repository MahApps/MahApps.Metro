// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Enum NumericInput which indicates what input is allowed for NumericUpdDown.
    /// </summary>
    [Flags]
    public enum NumericInput
    {
        /// <summary>
        /// Only numbers are allowed
        /// </summary>
        Numbers = 1 << 1, // Only Numbers
        /// <summary>
        /// Numbers with decimal point and allowed scientific input
        /// </summary>
        Decimal = 2 << 1,
        /// <summary>
        /// All is allowed
        /// </summary>
        All = Numbers | Decimal
    }
}