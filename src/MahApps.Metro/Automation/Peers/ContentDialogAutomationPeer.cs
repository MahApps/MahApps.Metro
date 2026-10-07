// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Automation.Peers;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Automation.Peers
{
    public class ContentDialogAutomationPeer : FrameworkElementAutomationPeer
    {
        public ContentDialogAutomationPeer(ContentDialog owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore()
        {
            return "ContentDialog";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Window;
        }

        protected override string GetNameCore()
        {
            string? nameCore = base.GetNameCore();
            if (string.IsNullOrEmpty(nameCore))
            {
                nameCore = ((ContentDialog)this.Owner).Title;
            }

            if (string.IsNullOrEmpty(nameCore))
            {
                nameCore = ((ContentDialog)this.Owner).Name;
            }

            if (string.IsNullOrEmpty(nameCore))
            {
                nameCore = this.GetClassNameCore();
            }

            return nameCore!;
        }
    }
}
