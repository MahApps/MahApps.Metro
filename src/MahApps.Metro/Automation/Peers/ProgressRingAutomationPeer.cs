// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Automation.Peers;
using JetBrains.Annotations;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Automation.Peers
{
    public class ProgressRingAutomationPeer : FrameworkElementAutomationPeer
    {
        public ProgressRingAutomationPeer([NotNull] ProgressRing owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore()
        {
            return nameof(ProgressRing);
        }

        protected override string GetNameCore()
        {
            string? nameCore = base.GetNameCore();

            if (this.Owner is ProgressRing { IsActive: true })
            {
                return nameof(ProgressRing.IsActive) + nameCore;
            }

            return nameCore!;
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ProgressBar;
        }
    }
}