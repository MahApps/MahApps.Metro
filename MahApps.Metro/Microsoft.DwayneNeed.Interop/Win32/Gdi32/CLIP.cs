using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.Gdi32
{
    public enum CLIP : int
    {
        /// <summary>
        /// Specifies default clipping behavior.
        /// </summary>
        DEFAULT_PRECIS = 0,

        /// <summary>
        /// Not used.
        /// </summary>
        CHARACTER_PRECIS = 1,

        /// <summary>
        /// Not used by the font mapper, but is returned when raster, vector,
        /// or TrueType fonts are enumerated. For compatibility, this value is
        /// always returned when enumerating fonts.
        /// </summary>
        STROKE_PRECIS = 2,

        /// <summary>
        /// When this value is used, the rotation for all fonts depends on
        /// whether the orientation of the coordinate system is left-handed or
        /// right-handed. If not used, device fonts always rotate
        /// counterclockwise, but the rotation of other fonts is dependent on
        /// the orientation of the coordinate system.
        /// </summary>
        LH_ANGLES = (1 << 4),

        /// <summary>
        /// Not used.
        /// </summary>
        TT_ALWAYS = (2 << 4),

        /// <summary>
        /// Windows XP SP1: Turns off font association for the font. Note that
        /// this flag is not guaranteed to have any effect on any platform
        /// after Windows Server 2003.
        /// </summary>
        DFA_DISABLE = (4 << 4),
        
        /// <summary>
        /// You must specify this flag to use an embedded read-only font.
        /// </summary>
        EMBEDDED = (8 << 4)
    }
}
