// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Automation.Peers
{
    /// <summary>
    /// A drop down button holds the entries of its menu, which made it a list to a client, with the
    /// button itself and the caption on it nowhere to be seen. It is a button that opens something.
    /// </summary>
    public class DropDownButtonAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider
    {
        public DropDownButtonAutomationPeer(DropDownButton owner)
            : base(owner)
        {
        }

        private DropDownButton Button => (DropDownButton)this.Owner;

        public ExpandCollapseState ExpandCollapseState => this.Button.IsExpanded ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;

        protected override string GetClassNameCore()
        {
            return "DropDownButton";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Button;
        }

        protected override string GetNameCore()
        {
            var name = base.GetNameCore();

            // a plain framework element peer looks no further than what was set by hand
            if (string.IsNullOrEmpty(name))
            {
                name = this.Button.Content as string ?? this.Button.Content?.ToString();
            }

            return name ?? string.Empty;
        }

        public override object? GetPattern(PatternInterface patternInterface)
        {
            return patternInterface == PatternInterface.ExpandCollapse ? this : base.GetPattern(patternInterface);
        }

        public void Collapse()
        {
            this.Button.SetCurrentValue(DropDownButton.IsExpandedProperty, false);
        }

        public void Expand()
        {
            this.Button.SetCurrentValue(DropDownButton.IsExpandedProperty, true);
        }
    }
}
