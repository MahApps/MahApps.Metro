// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The arguments of a click on a button of a <see cref="ContentDialog"/>.
    /// </summary>
    public class ContentDialogButtonClickEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets whether the click is canceled, which keeps the dialog open.
        /// </summary>
        public bool Cancel { get; set; }
    }
}
