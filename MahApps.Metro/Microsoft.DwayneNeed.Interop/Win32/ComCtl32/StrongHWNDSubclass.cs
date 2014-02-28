using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DwayneNeed.Win32.User32;
using Microsoft.DwayneNeed.Win32;

namespace Microsoft.DwayneNeed.Win32.ComCtl32
{
    internal class StrongHWNDSubclass : WindowSubclass
    {
        
        public StrongHWNDSubclass(StrongHWND strongHwnd) : base(new HWND(strongHwnd.DangerousGetHandle()))
        {
            // Note that we passed a new "weak" HWND handle to the base class.
            // This is because we don't want the StrongHWNDSubclass processing
            // a partially disposed handle in its own Dispose methods.
            _strongHwnd = strongHwnd;
        }

        protected override void Dispose(bool disposing)
        {
            // call the base class to let it disconnect the window proc.
            HWND hwnd = Hwnd;
            base.Dispose(disposing);

            NativeMethods.DestroyWindow(hwnd);
            _strongHwnd.OnHandleReleased();
        }

        private StrongHWND _strongHwnd;
    }
}
