// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Automation.Peers;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Automation.Peers
{
    /// <summary>
    /// A suggestion box is a ComboBox underneath, and a client told that would look for a list to pick
    /// from. It is a box to type in that offers help while typing.
    /// </summary>
    public class AutoSuggestBoxAutomationPeer : ComboBoxAutomationPeer
    {
        public AutoSuggestBoxAutomationPeer(AutoSuggestBox owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore()
        {
            return "AutoSuggestBox";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ComboBox;
        }
    }
}
