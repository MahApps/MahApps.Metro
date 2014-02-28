using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.Gdi32
{
    public struct COLORREF
    {
        public COLORREF(uint cr)
        {
            _value = cr;
        }

        public COLORREF(byte red, byte green, byte blue)
        {
            _value = (uint)(red | ((short)green) << 8 | ((int)blue) << 16);
        }

        public uint Value
        {
            get
            {
                return _value;
            }
        }

        private uint _value;
    }
}
