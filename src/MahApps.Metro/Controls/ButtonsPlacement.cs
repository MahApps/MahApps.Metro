// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Says where the buttons of <see cref="TextBoxHelper.ButtonsProperty"/> stand in relation to
    /// the clear button, which is the one a text control brings by itself.
    /// </summary>
    public enum ButtonsPlacement
    {
        /// <summary>
        /// Between the text and the clear button.
        /// </summary>
        Inside,

        /// <summary>
        /// On the far side of the clear button, so that the clear button is the one next to the text.
        /// </summary>
        Outside
    }
}
