// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// What a <see cref="ContentDialog"/> was closed with.
    /// </summary>
    public enum ContentDialogResult
    {
        /// <summary>No button was clicked, or the close button was.</summary>
        None,

        /// <summary>The primary button was clicked.</summary>
        Primary,

        /// <summary>The secondary button was clicked.</summary>
        Secondary
    }
}
