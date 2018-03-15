using System;

namespace MahApps.Metro.Controls
{
    [Flags]
    public enum WindowCommandsOverlayBehavior
    {
        /// <summary>
        /// Doesn't overlay flyouts nor a hidden TitleBar.
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
