// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Automation.Peers;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Automation.Peers
{
    /// <summary>
    /// A tile carries its caption in a title of its own rather than in its content, so a tile with
    /// nothing but a title reaches a client as a button with no caption at all.
    /// </summary>
    public class TileAutomationPeer : ButtonAutomationPeer
    {
        public TileAutomationPeer(Tile owner)
            : base(owner)
        {
        }

        protected override string GetClassNameCore()
        {
            return "Tile";
        }

        protected override string GetNameCore()
        {
            var name = base.GetNameCore();

            if (string.IsNullOrEmpty(name))
            {
                name = ((Tile)this.Owner).Title;
            }

            return name ?? string.Empty;
        }
    }
}
