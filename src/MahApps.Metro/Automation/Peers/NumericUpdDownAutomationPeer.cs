// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Automation.Peers;
using JetBrains.Annotations;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Automation.Peers
{
    public class NumericUpdDownAutomationPeer : FrameworkElementAutomationPeer
    {
        public NumericUpdDownAutomationPeer([NotNull] NumericUpDownBase owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore()
        {
            // Every control built on the same base reports itself, so a DecimalUpDown does not
            // announce that it is a NumericUpDown.
            return this.Owner.GetType().Name;
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Edit;
        }
    }
}