// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using JetBrains.Annotations;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Automation.Peers
{
    public class ProgressRingAutomationPeer : RangeBaseAutomationPeer, IRangeValueProvider
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

        // a ring says how far along something is. Nobody sets that from the outside, which is the
        // one thing RangeBase would otherwise let through.
        bool IRangeValueProvider.IsReadOnly => true;

        double IRangeValueProvider.SmallChange => double.NaN;

        double IRangeValueProvider.LargeChange => double.NaN;

        void IRangeValueProvider.SetValue(double val)
        {
            throw new InvalidOperationException($"A {nameof(ProgressRing)} says how far along something is and cannot be set to {val} from the outside.");
        }
    }
}