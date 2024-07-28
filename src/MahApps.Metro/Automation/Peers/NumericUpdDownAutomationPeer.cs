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
        public NumericUpdDownAutomationPeer([NotNull] NumericUpDown owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore()
        {
            return nameof(NumericUpDown);
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Edit;
        }
    }
}