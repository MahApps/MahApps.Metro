// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The time a handler of <see cref="BaseMetroTabControl.TabItemClosing"/> asked for. The tab
    /// control waits with the closing until every deferral it handed out is done, so a handler can
    /// await something, an answer from the user for instance, before it says whether the item may
    /// go.
    /// </summary>
    /// <remarks>
    /// A deferral that is never completed leaves the item where it is, so complete it on every way
    /// out of the handler. A <c>using</c> declaration does that.
    /// </remarks>
    public sealed class TabItemClosingDeferral : IDisposable
    {
        private Action? whenDone;

        internal TabItemClosingDeferral(Action whenDone)
        {
            this.whenDone = whenDone;
        }

        /// <summary>
        /// Tells the tab control that the handler is done. Saying it a second time does nothing.
        /// </summary>
        public void Complete()
        {
            Interlocked.Exchange(ref this.whenDone, null)?.Invoke();
        }

        /// <summary>
        /// Completes the deferral.
        /// </summary>
        public void Dispose()
        {
            this.Complete();
        }
    }
}
