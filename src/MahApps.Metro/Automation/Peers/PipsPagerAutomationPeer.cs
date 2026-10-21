// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// Ported from PipsPagerAutomationPeer.cpp of microsoft-ui-xaml, Copyright (c) Microsoft Corporation, MIT License.

using System;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using MahApps.Metro.Controls;
using AutomationStrings = MahApps.Metro.Lang.Automation;

namespace MahApps.Metro.Automation.Peers
{
    /// <summary>
    /// Exposes the <see cref="PipsPager"/> to UI Automation as a menu that offers the pip of the selected page as its selection.
    /// </summary>
    public class PipsPagerAutomationPeer : FrameworkElementAutomationPeer, ISelectionProvider
    {
        public PipsPagerAutomationPeer(PipsPager owner)
            : base(owner)
        {
        }

        private PipsPager PipsPager => (PipsPager)this.Owner;

        protected override string GetClassNameCore()
        {
            return nameof(PipsPager);
        }

        protected override string GetNameCore()
        {
            var name = base.GetNameCore();
            return string.IsNullOrEmpty(name) ? AutomationStrings.PipsPagerNameText : name;
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Menu;
        }

        public override object? GetPattern(PatternInterface patternInterface)
        {
            return patternInterface == PatternInterface.Selection ? this : base.GetPattern(patternInterface);
        }

        internal void RaiseSelectionChanged()
        {
            if (ListenerExists(AutomationEvents.SelectionPatternOnInvalidated))
            {
                this.RaiseAutomationEvent(AutomationEvents.SelectionPatternOnInvalidated);
            }
        }

        // ISelectionProvider

        bool ISelectionProvider.CanSelectMultiple => false;

        bool ISelectionProvider.IsSelectionRequired => true;

        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            if (this.PipsPager.GetSelectedPip() is { } pip && CreatePeerForElement(pip) is { } peer)
            {
                return new[] { this.ProviderFromPeer(peer) };
            }

            return Array.Empty<IRawElementProviderSimple>();
        }
    }
}
