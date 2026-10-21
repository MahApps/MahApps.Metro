// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// Ported from PipsPager.idl of microsoft-ui-xaml, Copyright (c) Microsoft Corporation, MIT License.

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// When the previous and the next button of a <see cref="PipsPager"/> are shown.
    /// </summary>
    public enum PipsPagerButtonVisibility
    {
        /// <summary>The button is always shown, apart from the edge it can't go beyond.</summary>
        Visible,

        /// <summary>The button is shown while the pointer is over the pager or the keyboard focus is in it.</summary>
        VisibleOnPointerOver,

        /// <summary>The button is not shown and takes no room.</summary>
        Collapsed
    }
}
