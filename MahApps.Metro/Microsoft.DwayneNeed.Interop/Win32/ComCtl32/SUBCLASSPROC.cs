using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DwayneNeed.Win32.User32;

namespace Microsoft.DwayneNeed.Win32.ComCtl32
{
    // It would be great to use the HWND type for hwnd, but this is not
    // possible because you will get a MarshalDirectiveException complaining
    // that the unmanaged code cannot pass in a SafeHandle.  This is because
    // native code is calling this callback, and native code doesn't know how
    // to create a managed SafeHandle for the native handle.  I was a little
    // surprised that the marshaller can't do this automatically, but
    // apparently it can't.
    //
    // Instead, most classes that use a SUBCLASSPROC will expose their own
    // virtual that creates new HWND instances for the incomming handles.
    public delegate IntPtr SUBCLASSPROC(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, IntPtr id, IntPtr data);
}
