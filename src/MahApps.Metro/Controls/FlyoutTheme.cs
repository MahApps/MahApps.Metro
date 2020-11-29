// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ControlzEx.Theming;

namespace MahApps.Metro.Controls
{
    public enum FlyoutTheme
    {
        /// <summary>
        /// Adapts the <see cref="Flyout"/> theme to the theme of the host window or application.
        /// </summary>
        Adapt,

        /// <summary>
        /// Adapts the <see cref="Flyout"/> theme to the theme of the host window or application, but inverted.
        /// </summary>
        /// <remarks>
        /// This theme can only be applied if the host window's theme abides the "Dark" and "Light" affix convention.
        /// (see <see cref="ThemeManager.GetInverseTheme"/> for more infos.
        /// </remarks>
        Inverse,

        /// <summary>
        /// Use the dark theme for the <see cref="Flyout"/>. This is the default theme.
        /// </summary>
        Dark,

        /// <summary>
        /// Use the light theme for the <see cref="Flyout"/>.
        /// </summary>
        Light,

        /// <summary>
        /// The <see cref="Flyout"/> theme will match the host window's accent color.
        /// </summary>
        Accent
    }
}