using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.Gdi32
{
    public enum QUALITY : int
    {
        /// <summary>
        /// Appearance of the font does not matter.
        /// </summary>
        DEFAULT = 0,

        /// <summary>
        /// Appearance of the font is less important than when the
        /// PROOF value is used. For GDI raster fonts, scaling is enabled,
        /// which means that more font sizes are available, but the quality
        /// may be lower. Bold, italic, underline, and strikeout fonts are
        /// synthesized, if necessary.
        /// </summary>
        DRAFT = 1,

        /// <summary>
        /// Character quality of the font is more important than exact matching
        /// of the logical-font attributes. For GDI raster fonts, scaling is
        /// disabled and the font closest in size is chosen. Although the
        /// chosen font size may not be mapped exactly when PROOF is used, the
        /// quality of the font is high and there is no distortion of
        /// appearance. Bold, italic, underline, and strikeout fonts are
        /// synthesized, if necessary.
        /// </summary>
        PROOF = 2,

        /// <summary>
        /// Font is never antialiased, that is, font smoothing is not done.
        /// </summary>
        NONANTIALIASED = 3,

        /// <summary>
        /// Font is antialiased, or smoothed, if the font supports it and the
        /// size of the font is not too small or too large.
        /// </summary>
        ANTIALIASED = 4,
 
        /// <summary>
        /// If set, text is rendered (when possible) using ClearType antialiasing method. See Remarks for more information.
        /// </summary>
        CLEARTYPE = 5,

        /// <summary>
        /// No MSDN docs.
        /// </summary>
        CLEARTYPE_NATURAL = 6
    }
}
