// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// What closed an <see cref="InfoBar"/>.
    /// </summary>
    public enum InfoBarCloseReason
    {
        /// <summary>The user clicked the close button.</summary>
        CloseButton = 0,

        /// <summary>The code set <see cref="InfoBar.IsOpen"/> to false.</summary>
        Programmatic = 1
    }
}
