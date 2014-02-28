using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.User32
{
    // It would be great to use the HWND type for hwnd, but this is not
    // possible because you will get a MarshalDirectiveException complaining
    // that the unmanaged code cannot pass in a SafeHandle.  Instead, most
    // classes that use a WNDPROC will expose its own virtual that creates
    // new HWND instances for the incomming handles.
    public delegate IntPtr WNDPROC(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
}
