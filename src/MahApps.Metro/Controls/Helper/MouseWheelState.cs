// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MahApps.Metro.Controls
{
    public enum MouseWheelState
    {
        /// <summary>
        /// Do not change the value of the slider if the user rotates the mouse wheel.
        /// </summary>
        None,
        /// <summary>
        /// Change the value of the slider only if the control is focused.
        /// </summary>
        ControlFocused,
        /// <summary>
        /// Changes the value of the slider if the mouse pointer is over this element.
        /// </summary>
        MouseHover
    }
}