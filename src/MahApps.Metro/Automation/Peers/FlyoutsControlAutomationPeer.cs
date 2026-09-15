// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Automation.Peers
{
    /// <summary>
    /// The flyouts of a window are held in a control lying over the whole of it, so that a flyout can
    /// come in from any side. It paints nothing while every flyout is shut and a click goes straight
    /// through it, but a client asking what sits under a point goes by the box an element occupies and
    /// lands here instead of on the content underneath. This answers such a question only while there
    /// is a flyout open to answer it.
    /// </summary>
    public class FlyoutsControlAutomationPeer : FrameworkElementAutomationPeer
    {
        public FlyoutsControlAutomationPeer(FlyoutsControl owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore()
        {
            return "FlyoutsControl";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.List;
        }

        protected override AutomationPeer? GetPeerFromPointCore(Point point)
        {
            return this.AnythingOpen() ? base.GetPeerFromPointCore(point) : null;
        }

        protected override bool IsOffscreenCore()
        {
            return this.AnythingOpen() == false || base.IsOffscreenCore();
        }

        private bool AnythingOpen()
        {
            return ((FlyoutsControl)this.Owner).GetFlyouts().Any(flyout => flyout.IsOpen);
        }
    }
}
