﻿using System;

namespace MahApps.Metro.Controls
{
    [Flags]
    public enum OverlayBehavior
    {
        /// <summary>
        /// Doesn't overlay Flyouts nor a hidden TitleBar.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Overlays opened <see cref="Flyout"/> controls.
        /// </summary>
        Flyouts = 1 << 0,

        /// <summary>
        /// Overlays a hidden TitleBar.
        /// </summary>
        HiddenTitleBar = 1 << 1,

        Always = ~(-1 << 2)
    }
}