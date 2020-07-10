// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls.Primitives;

namespace MahApps.Metro.Controls
{
    public enum MouseWheelChange
    {
        /// <summary>
        /// Change the value of the slider if the user rotates the mouse wheel by the value defined for <see cref="RangeBase.SmallChange"/>
        /// </summary>
        SmallChange,
        /// <summary>
        /// Change the value of the slider if the user rotates the mouse wheel by the value defined for <see cref="RangeBase.LargeChange"/>
        /// </summary>
        LargeChange
    }
}