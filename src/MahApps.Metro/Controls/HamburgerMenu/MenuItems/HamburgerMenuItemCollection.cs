// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The HamburgerMenuItemCollection provides typed collection of HamburgerMenuItemBase.
    /// </summary>
    /// <remarks>
    /// A plain observable collection rather than a freezable one. The items carry their own place in
    /// the logical tree of the menu, and a freezable collection would hand them a second home they
    /// cannot have.
    /// </remarks>
    public class HamburgerMenuItemCollection : ObservableCollection<HamburgerMenuItemBase>
    {
    }
}
