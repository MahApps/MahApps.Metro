// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Automation.Peers
{
    /// <summary>
    /// A hot key box holds a key combination. The text box inside it shows one, but the box itself was
    /// not in the tree, so there was nothing for a client to ask.
    /// </summary>
    public class HotKeyBoxAutomationPeer : FrameworkElementAutomationPeer, IValueProvider
    {
        public HotKeyBoxAutomationPeer(HotKeyBox owner)
            : base(owner)
        {
        }

        private HotKeyBox Box => (HotKeyBox)this.Owner;

        public bool IsReadOnly => this.Box.IsEnabled == false;

        public string Value => this.Box.HotKey?.ToString() ?? string.Empty;

        protected override string GetClassNameCore()
        {
            return "HotKeyBox";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Edit;
        }

        public override object? GetPattern(PatternInterface patternInterface)
        {
            return patternInterface == PatternInterface.Value ? this : base.GetPattern(patternInterface);
        }

        /// <summary>
        /// A combination is pressed, not typed, so there is nothing sensible to set from here.
        /// </summary>
        public void SetValue(string value)
        {
            throw new InvalidOperationException("A hot key is set by pressing it, not by handing over text.");
        }
    }
}
