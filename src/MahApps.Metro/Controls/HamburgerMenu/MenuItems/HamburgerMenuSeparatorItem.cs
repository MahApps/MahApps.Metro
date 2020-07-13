// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The HamburgerMenuSeparatorItem provides an separator based implementation for HamburgerMenu entries.
    /// </summary>
    public class HamburgerMenuSeparatorItem : HamburgerMenuItemBase, IHamburgerMenuSeparatorItem
    {
        protected override Freezable CreateInstanceCore()
        {
            return new HamburgerMenuSeparatorItem();
        }
    }
}