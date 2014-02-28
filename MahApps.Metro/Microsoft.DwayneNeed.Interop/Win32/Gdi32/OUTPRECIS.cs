using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.Gdi32
{
    public enum OUTPRECIS : int
    {
        /// <summary>
        /// The default font mapper behavior.
        /// </summary>
        DEFAULT  = 0,

        /// <summary>
        /// This value is not used by the font mapper, but it is returned when raster fonts are enumerated.
        /// </summary>
        STRING = 1,

        /// <summary>
        /// Not used.
        /// </summary>
        CHARACTER  = 2,
 
        /// <summary>
        /// This value is not used by the font mapper, but it is returned when TrueType, other outline-based fonts, and vector fonts are enumerated.
        /// </summary>
        STROKE = 3,

        /// <summary>
        /// Instructs the font mapper to choose a TrueType font when the system contains multiple fonts with the same name.
        /// </summary>
        TT = 4,

        /// <summary>
        /// Instructs the font mapper to choose a Device font when the system contains multiple fonts with the same name.
        /// </summary>
        DEVICE = 5,
 
        /// <summary>
        /// Instructs the font mapper to choose a raster font when the system contains multiple fonts with the same name.
        /// </summary>
        RASTER = 6,

        /// <summary>
        /// Instructs the font mapper to choose from only TrueType fonts. If there are no TrueType fonts installed in the system, the font mapper returns to default behavior.
        /// </summary>
        TT_ONLY = 7,

        /// <summary>
        /// This value instructs the font mapper to choose from TrueType and other outline-based fonts.
        /// </summary>
        OUTLINE = 8,

        /// <summary>
        /// No MSDN docs.
        /// </summary>
        SCREEN_OUTLINE = 9,

        /// <summary>
        /// Instructs the font mapper to choose from only PostScript fonts. If there are no PostScript fonts installed in the system, the font mapper returns to default behavior.
        /// </summary>
        PS_ONLY = 10
    }
}
