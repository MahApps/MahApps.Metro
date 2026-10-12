// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The arguments of <see cref="InfoBar.Closing"/>. Setting <see cref="Cancel"/> keeps the bar open.
    /// </summary>
    public class InfoBarClosingEventArgs : RoutedEventArgs
    {
        public InfoBarClosingEventArgs(RoutedEvent routedEvent, InfoBarCloseReason reason)
            : base(routedEvent)
        {
            this.Reason = reason;
        }

        /// <summary>Gets what is closing the bar.</summary>
        public InfoBarCloseReason Reason { get; }

        /// <summary>Gets or sets whether the bar stays open.</summary>
        public bool Cancel { get; set; }

        /// <inheritdoc />
        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            // a typed call, so that an exception in a handler comes out as itself rather than wrapped by DynamicInvoke
            ((EventHandler<InfoBarClosingEventArgs>)genericHandler)(genericTarget, this);
        }
    }
}
