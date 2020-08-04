// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MahApps.Metro.Controls
{
    public interface IHamburgerMenuItemBase
    {
        /// <summary>
        /// Gets or sets the value indicating whether this element is visible in the user interface (UI). This is a dependency property.
        /// </summary>
        /// <returns>
        /// true if the item is visible, otherwise false. The default value is true.
        /// </returns>
        bool IsVisible { get; set; }
    }
}