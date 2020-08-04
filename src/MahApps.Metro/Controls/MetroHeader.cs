// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using MahApps.Metro.Automation.Peers;

namespace MahApps.Metro.Controls
{
    public class MetroHeader : GroupBox
    {
        static MetroHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroHeader), new FrameworkPropertyMetadata(typeof(MetroHeader)));
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="M:System.Windows.UIElement.OnCreateAutomationPeer" />)
        /// </summary>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new MetroHeaderAutomationPeer(this);
        }
    }
}