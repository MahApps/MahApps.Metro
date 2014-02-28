using Microsoft.DwayneNeed.Win32.Common;
using Microsoft.DwayneNeed.Win32.Kernel32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.DwayneNeed.Win32.Gdi32
{
    /// <summary>
    /// A handle to a GDI object.
    /// </summary>
    public abstract class HGDIOBJ : ThreadAffinitizedHandle
    {
        protected HGDIOBJ() : base(true)
        {
        }

        protected HGDIOBJ(IntPtr hObject) : base(true)
        {
            SetHandle(hObject);
        }

        protected override bool ReleaseHandleSameThread()
        {
            if (DangerousOwnsHandle)
            {
                DeleteObject(this.handle);
            }

            return true;
        }

        public bool DangerousOwnsHandle;

        /// <summary>
        /// Retrieves the GDI object type.
        /// </summary>
        public OBJ ObjectType
        {
            get
            {
                return GetObjectType(this);
            }
        }

        #region PInvoke
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern OBJ GetObjectType(HGDIOBJ hObject);

        // The handle type is IntPtr because this function is called during
        // handle cleanup, and the SafeHandle itself cannot be marshalled.
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject(IntPtr hObject);

        #endregion

    }
}
