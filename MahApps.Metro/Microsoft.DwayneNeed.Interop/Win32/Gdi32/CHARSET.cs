using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.Gdi32
{
    public enum CHARSET : int
    {
        ANSI = 0,
        DEFAULT = 1,
        SYMBOL = 2,
        SHIFTJIS = 128,
        HANGEUL = 129,
        HANGUL = 129,
        GB2312 = 134,
        CHINESEBIG5 = 136,
        OEM = 255,
        JOHAB = 130,
        HEBREW = 177,
        ARABIC = 178,
        GREEK = 161,
        TURKISH = 162,
        VIETNAMESE = 163,
        THAI = 222,
        EASTEUROPE = 238,
        RUSSIAN = 204,
        MAC = 77,
        BALTIC = 186
    }
}
