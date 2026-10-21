// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// Ported from PipsPager.idl of microsoft-ui-xaml, Copyright (c) Microsoft Corporation, MIT License.

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Whether the buttons of a <see cref="PipsPager"/> go round from the last page to the first and back.
    /// </summary>
    public enum PipsPagerWrapMode
    {
        /// <summary>The buttons stop at the first and at the last page.</summary>
        None,

        /// <summary>Next on the last page goes to the first one, previous on the first page to the last one.</summary>
        Wrap
    }
}
