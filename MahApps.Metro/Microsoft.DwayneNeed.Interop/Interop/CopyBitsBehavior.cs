using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Interop
{
    public enum CopyBitsBehavior
    {
        /// <summary>
        ///    Do nothing regarding the SWP_NOCOPYBITS flag.
        /// </summary>
        Default,

        /// <summary>
        /// Never copy the bits from the old location to the new location.
        /// </summary>
        /// <remarks>
        /// In our handler for WM_WINDOWPOSCHANGING, we will always set the
        /// SWP_NOCOPYBITS flag.  Windows will send a WM_PAINT message to
        /// the window to paint itself in the new location.
        /// </remarks>
        NeverCopyBits,

        /// <summary>
        /// Always copy the bits from the old location to the new location.
        /// </summary>
        /// <remarks>
        /// In our handler for WM_WINDOWPOSCHANGING, we will always clear the
        /// SWP_NOCOPYBITS flag.  Windows will copy the bits from the old
        /// location to the new location, and only send a WM_PAINT message for
        /// the area that was obscured.
        /// </remarks>
        AlwaysCopyBits,

        /// <summary>
        /// Always copy the bits from the old location to the new location,
        /// but also cause the window to repaint in the new location.
        /// </summary>
        /// <remarks>
        /// In our handler for WM_WINDOWPOSCHANGING, we will always clear the
        /// SWP_NOCOPYBITS flag.  Windows will copy the bits from the old
        /// location to the new location, and only send a WM_PAINT message for
        /// the area that was obscured.  We also manually invalidate the window
        /// in the new location so that it will repaint.
        ///
        /// Flicker is reduced because Windows will quickly copy the bits for
        /// us, and any visual artifacts from the bit copy will be repaired by
        /// repainting the window.
        /// </remarks>
        CopyBitsAndRepaint
    }
}
