// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// ChildWindow close reasons.
    /// </summary>
    public enum CloseReason
    {
        /// <summary>
        /// The dialog is not closed, or a close was canceled.
        /// </summary>
        None,
        /// <summary>
        /// The dialog was closed automatically.
        /// </summary>
        AutoClose,
        /// <summary>
        /// The Dialog was closed by hitting the overlay.
        /// </summary>
        Overlay,
        /// <summary>
        /// The Dialog was closed by e.g. an Ok button (optional, can be used when it's necessary).
        /// </summary>
        Ok,
        /// <summary>
        /// The Dialog was closed by e.g. an Apply button (optional, can be used when it's necessary).
        /// </summary>
        Apply,
        /// <summary>
        /// The Dialog was closed by e.g. a Cancel button (optional, can be used when it's necessary).
        /// </summary>
        Cancel,
        /// <summary>
        /// The Dialog was closed by the Close method or the Close button on the title bar.
        /// </summary>
        Close,
        /// <summary>
        /// The Dialog was closed by the Escape key.
        /// </summary>
        Escape
    }
}