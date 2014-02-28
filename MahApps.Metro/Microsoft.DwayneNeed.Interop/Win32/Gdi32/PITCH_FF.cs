using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.Gdi32
{
    /// <summary>
    /// A union of a PITCH and a FF value.
    /// </summary>
    public struct PITCH_FF
    {
        public PITCH_FF(PITCH pitch = PITCH.DEFAULT, FF family = FF.DONTCARE)
        {
            _value = (int)pitch | (int)family;
        }

        public static implicit operator PITCH_FF(PITCH pitch)
        {
            return new PITCH_FF(pitch);
        }

        public static implicit operator PITCH_FF(FF family)
        {
            return new PITCH_FF(PITCH.DEFAULT, family);
        }

        public int Value
        {
            get
            {
                return _value;
            }
        }

        private int _value;
    }
}
