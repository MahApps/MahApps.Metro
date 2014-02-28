using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.UrlMon
{
    public enum SET_FEATURE
    {
        ON_THREAD = 0x00000001,
        ON_PROCESS = 0x00000002,
        IN_REGISTRY = 0x00000004,
        ON_THREAD_LOCALMACHINE = 0x00000008,
        ON_THREAD_INTRANET = 0x00000010,
        ON_THREAD_TRUSTED = 0x00000020,
        ON_THREAD_INTERNET = 0x00000040,
        ON_THREAD_RESTRICTED = 0x00000080
    }
}
