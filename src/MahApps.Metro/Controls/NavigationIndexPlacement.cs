// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// For specifying where the navigation index is placed relative to the <see cref="FlipViewItem"/>.
    /// </summary>
    public enum NavigationIndexPlacement
    {
        /// <summary>
        /// Index on left side
        /// </summary>
        Left,
        /// <summary>
        /// Index on right side
        /// </summary>
        Right,
        /// <summary>
        /// Index on top side
        /// </summary>
        Top,
        /// <summary>
        /// Index on bottom side
        /// </summary>
        Bottom,

        /// <summary>
        /// Index on left side over the item
        /// </summary>
        LeftOverItem,
        /// <summary>
        /// Index on right side over the item
        /// </summary>
        RightOverItem,
        /// <summary>
        /// Index on top side over the item
        /// </summary>
        TopOverItem,
        /// <summary>
        /// Index on bottom side over the item
        /// </summary>
        BottomOverItem
    }
}