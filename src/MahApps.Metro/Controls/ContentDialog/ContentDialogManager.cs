// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Shows a <see cref="ContentDialog"/> in a <see cref="MetroWindow"/>.
    /// </summary>
    public static class ContentDialogManager
    {
        /// <summary>
        /// Shows the dialog in the dialog container of the window, over the whole window, and returns what it was closed with.
        /// </summary>
        /// <param name="window">The window to show the dialog in.</param>
        /// <param name="dialog">The dialog.</param>
        public static async Task<ContentDialogResult> ShowContentDialogAsync(this MetroWindow window, ContentDialog dialog)
        {
            return await window.ShowChildWindowAsync<ContentDialogResult>(dialog, ChildWindowManager.OverlayFillBehavior.FullWindow).ConfigureAwait(true);
        }
    }
}
