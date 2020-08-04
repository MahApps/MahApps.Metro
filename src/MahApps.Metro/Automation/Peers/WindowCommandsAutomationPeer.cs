// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Automation.Peers;
using JetBrains.Annotations;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Automation.Peers
{
    public class WindowCommandsAutomationPeer : FrameworkElementAutomationPeer
    {
        public WindowCommandsAutomationPeer([NotNull] WindowCommands owner)
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return "WindowCommands";
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ToolBar;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            string nameCore = base.GetNameCore();

            if (string.IsNullOrEmpty(nameCore))
            {
                nameCore = ((WindowCommands)this.Owner).Name;
            }

            if (string.IsNullOrEmpty(nameCore))
            {
                nameCore = this.GetClassNameCore();
            }

            return nameCore;
        }

        /// <inheritdoc />
        protected override bool IsOffscreenCore()
        {
            return !((WindowCommands)this.Owner).HasItems || base.IsOffscreenCore();
        }

        protected override Point GetClickablePointCore()
        {
            if (!((WindowCommands)this.Owner).HasItems)
            {
                return new Point(double.NaN, double.NaN);
            }

            return base.GetClickablePointCore();
        }
    }
}