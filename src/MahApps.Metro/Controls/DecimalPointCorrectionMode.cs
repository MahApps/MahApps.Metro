// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MahApps.Metro.Controls
{
    public enum DecimalPointCorrectionMode
    {
        /// <summary>
        /// (Default) No correction is applied, and any style
        /// inherited setting may influence the correction behavior.
        /// </summary>
        Inherits,

        /// <summary>
        /// Enable the decimal-point correction for generic numbers.
        /// </summary>
        Number,

        /// <summary>
        /// Enable the decimal-point correction for currency numbers.
        /// </summary>
        Currency,

        /// <summary>
        /// Enable the decimal-point correction for percent-numbers.
        /// </summary>
        Percent,
    }
}