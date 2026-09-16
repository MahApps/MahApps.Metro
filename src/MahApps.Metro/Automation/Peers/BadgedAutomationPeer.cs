// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Automation.Peers;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Automation.Peers
{
    /// <summary>
    /// A badge is a mark on whatever it wraps, the count of unread messages over an inbox button and
    /// the like. Drawn on its own it reaches a client as a stray piece of text beside the thing it
    /// belongs to, so it is handed over as the status of that thing instead.
    /// </summary>
    public class BadgedAutomationPeer : FrameworkElementAutomationPeer
    {
        public BadgedAutomationPeer(Badged owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore()
        {
            return "Badged";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Group;
        }

        protected override string GetItemStatusCore()
        {
            var badge = ((Badged)this.Owner).Badge;

            return badge?.ToString() ?? string.Empty;
        }
    }
}
