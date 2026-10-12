// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The arguments of <see cref="InfoBar.Closed"/>.
    /// </summary>
    public class InfoBarClosedEventArgs : RoutedEventArgs
    {
        public InfoBarClosedEventArgs(RoutedEvent routedEvent, InfoBarCloseReason reason)
            : base(routedEvent)
        {
            this.Reason = reason;
        }

        /// <summary>Gets what closed the bar.</summary>
        public InfoBarCloseReason Reason { get; }

        /// <inheritdoc />
        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            // a typed call, so that an exception in a handler comes out as itself rather than wrapped by DynamicInvoke
            ((EventHandler<InfoBarClosedEventArgs>)genericHandler)(genericTarget, this);
        }
    }
}
