using System;

namespace MahApps.Metro.Controls
{
    [Flags]
    public enum WindowCommandsBehavior
    {
        Never = 0,
        OverlayFlyout = 1 << 0,
        OverlayHiddenTitleBar = 1 << 1,
        Always = ~(-1 << 2)
    }
}
