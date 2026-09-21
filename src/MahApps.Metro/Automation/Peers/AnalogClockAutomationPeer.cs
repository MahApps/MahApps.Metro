// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Windows.Automation.Peers;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Automation.Peers
{
    /// <summary>
    /// A clock face is a drawing of a time, so it is handed over as one, and what it says is the
    /// time it points at. A name of its own wins over that, for a clock that stands somewhere where
    /// the time alone does not say what it is.
    /// </summary>
    public class AnalogClockAutomationPeer : FrameworkElementAutomationPeer
    {
        public AnalogClockAutomationPeer(AnalogClock owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore()
        {
            return "AnalogClock";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Image;
        }

        protected override string GetNameCore()
        {
            var name = base.GetNameCore();
            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }

            var time = ((AnalogClock)this.Owner).Time;

            return time is null ? string.Empty : time.Value.ToString("T", CultureInfo.CurrentCulture);
        }
    }
}
