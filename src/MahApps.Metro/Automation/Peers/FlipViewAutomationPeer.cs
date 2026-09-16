// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Windows.Automation.Peers;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Automation.Peers
{
    /// <summary>
    /// A flip view shows one page at a time and builds nothing for the others, so there is no list of
    /// pages for a client to walk. What it can say is which page is showing and how many there are,
    /// and the page itself is in the tree the way any content is.
    /// </summary>
    public class FlipViewAutomationPeer : FrameworkElementAutomationPeer
    {
        public FlipViewAutomationPeer(FlipView owner)
            : base(owner)
        {
        }

        private FlipView View => (FlipView)this.Owner;

        protected override string GetClassNameCore()
        {
            return "FlipView";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        /// <summary>
        /// Where in the run of pages this one is, counted the way somebody reading it would.
        /// </summary>
        protected override string GetItemStatusCore()
        {
            var view = this.View;

            if (view.Items.Count == 0)
            {
                return string.Empty;
            }

            return string.Format(CultureInfo.CurrentCulture, "{0} / {1}", view.SelectedIndex + 1, view.Items.Count);
        }
    }
}
