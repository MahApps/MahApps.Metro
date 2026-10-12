// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Automation.Peers;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Automation.Peers
{
    public class InfoBarAutomationPeer : FrameworkElementAutomationPeer
    {
        public InfoBarAutomationPeer(InfoBar owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore()
        {
            return "InfoBar";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.StatusBar;
        }

        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (string.IsNullOrEmpty(nameCore))
            {
                nameCore = ((InfoBar)this.Owner).Title;
            }

            return nameCore ?? string.Empty;
        }

        // a closed bar is not there for a reader, as WinUI sets it to Raw
        protected override bool IsControlElementCore()
        {
            return ((InfoBar)this.Owner).IsOpen && base.IsControlElementCore();
        }

        protected override bool IsContentElementCore()
        {
            return ((InfoBar)this.Owner).IsOpen && base.IsContentElementCore();
        }
    }
}
