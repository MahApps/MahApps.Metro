// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Threading;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Automation.Peers
{
    public class HyperlinkButtonAutomationPeer : ButtonBaseAutomationPeer, IInvokeProvider
    {
        public HyperlinkButtonAutomationPeer(HyperlinkButton owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore()
        {
            return "HyperlinkButton";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Hyperlink;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            return patternInterface == PatternInterface.Invoke ? this : base.GetPattern(patternInterface);
        }

        void IInvokeProvider.Invoke()
        {
            if (!this.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            // like the peer of the WPF button: the click runs after the call returns, so a client is never held up by it
            this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback(_ =>
                {
                    ((HyperlinkButton)this.Owner).AutomationClick();
                    return null;
                }), null);
        }
    }
}
