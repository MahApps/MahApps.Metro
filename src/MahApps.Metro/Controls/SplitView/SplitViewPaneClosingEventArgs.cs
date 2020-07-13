// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MahApps.Metro.Controls
{
    /// <summary>
    ///     Provides event data for the <see cref="SplitView.PaneClosing" /> event.
    /// </summary>
    public sealed class SplitViewPaneClosingEventArgs
    {
        /// <summary>
        ///     Gets or sets a value that indicates whether the pane closing action should be canceled.
        /// </summary>
        /// <returns>true to cancel the pane closing action; otherwise, false.</returns>
        public bool Cancel { get; set; }
    }
}