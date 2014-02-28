using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.Gdi32
{
    public enum FF : int
    {
        /// <summary>
        /// Use default font.
        /// </summary>
        DONTCARE = (0<<4),

        /// <summary>
        /// Fonts with variable stroke width and with serifs. MS Serif is an
        /// example.
        /// </summary>
        ROMAN = (1<<4),

        /// <summary>
        /// Fonts with variable stroke width and without serifs. MS Sans Serif
        /// is an example.
        /// </summary>
        SWISS = (2<<4),

        /// <summary>
        /// Fonts with constant stroke width, with or without serifs. Pica,
        /// Elite, and Courier New are examples.
        /// </summary>
        MODERN = (3<<4),

        /// <summary>
        /// Fonts designed to look like handwriting. Script and Cursive are
        /// examples.
        /// </summary>
        SCRIPT = (4<<4),

        /// <summary>
        /// Novelty fonts. Old English is an example.
        /// </summary>
        DECORATIVE = (5<<4)
    }
}
