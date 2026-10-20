// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// Ported from PipsPagerSelectedIndexChangedEventArgs.h of microsoft-ui-xaml, Copyright (c) Microsoft Corporation, MIT License.

using System;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The arguments of <see cref="PipsPager.SelectedIndexChanged"/>. Like in WinUI they carry nothing,
    /// the new page is in <see cref="PipsPager.SelectedPageIndex"/>.
    /// </summary>
    public sealed class PipsPagerSelectedIndexChangedEventArgs : EventArgs
    {
    }
}
