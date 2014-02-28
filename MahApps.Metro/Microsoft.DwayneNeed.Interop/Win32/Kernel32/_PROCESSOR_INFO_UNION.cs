using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Microsoft.DwayneNeed.Win32.Kernel32
{
    [StructLayout(LayoutKind.Explicit)]
    public struct _PROCESSOR_INFO_UNION
    {
        [FieldOffset(0)]
        internal uint dwOemId;
        [FieldOffset(0)]
        internal ushort wProcessorArchitecture;
        [FieldOffset(2)]
        internal ushort wReserved;
    }
}
