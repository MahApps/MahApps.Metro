// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Automation.Peers;
using JetBrains.Annotations;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Automation.Peers
{
    public class MetroWindowAutomationPeer : FrameworkElementAutomationPeer
    {
        public MetroWindowAutomationPeer([NotNull] Window owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore()
        {
            return "MetroWindow";
        }

        protected override string GetNameCore()
        {
            string name = base.GetNameCore();

            if (name == string.Empty)
            {
                MetroWindow window = (MetroWindow)this.Owner;

                name = window.GetWindowText();
                if (name == null)
                {
                    name = string.Empty;
                }
            }

            return name;
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Window;
        }

        protected override Rect GetBoundingRectangleCore()
        {
            MetroWindow window = (MetroWindow)this.Owner;

            Rect bounds = window.GetWindowBoundingRectangle();

            return bounds;
        }
    }
}