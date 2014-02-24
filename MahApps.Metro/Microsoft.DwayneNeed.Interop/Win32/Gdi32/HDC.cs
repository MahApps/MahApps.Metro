using Microsoft.DwayneNeed.Win32.Common;
using Microsoft.DwayneNeed.Win32.Kernel32;
using Microsoft.DwayneNeed.Win32.User32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.Gdi32
{
    /// <summary>
    /// There are two kinds of ownership relationships to DCs: "owned" and
    /// "borrowed".  An "owned" DC is the typical GDI pattern, where you
    /// create a DC and then you must destroy it.  A "borrowed" DC is the
    /// typical User32 pattern, where you get a DC for a window and release
    /// it when you are done with it.
    /// 
    /// These ownership models are mutually exclusive, so we create a base
    /// HDC type, and derive the OwnedHDC and BorrowedHDC classes from it.
    /// 
    /// Functions that take an HDC parameter should accept the base HDC type.
    /// </summary>
    public class HDC : HGDIOBJ
    {
        /// <summary>
        /// Private constructor to create an HDC SafeHandle instance.
        /// </summary>
        /// <remarks>
        /// This constructor is used by the marshaller.  The handle value is
        /// then set directly.
        /// </remarks>
        private HDC()
        {
        }

        /// <summary>
        /// Public constructor to create an HDC SafeHandle instance that wraps
        /// an existing handle.
        /// </summary>
        /// <remarks>
        /// To cause the handle to be released when this object is disposed or
        /// finalized, you must set DangerousOwnsHandle to true.
        /// </remarks>
        public HDC(IntPtr hdc)
            : base(hdc)
        {
        }
        
        public static HDC CreateCompatibleDC(HDC hdc)
        {
            HDC value = null;

            if (hdc == null)
            {
                value = _CreateCompatibleDC(IntPtr.Zero);
            }
            else
            {
                value = _CreateCompatibleDC(hdc);
            }

            if (value != null && value.DangerousGetHandle() != IntPtr.Zero)
            {
                value.DangerousOwnsHandle = true;
            }

            return value;
        }

        public HGDIOBJ SelectObject(HGDIOBJ obj)
        {
            HGDIOBJ oldObject = SelectObject(this, obj);
            switch (oldObject.ObjectType)
            {
                case OBJ.BRUSH:
                    return new HBRUSH(oldObject.DangerousGetHandle());

                default:
                    throw new NotImplementedException();
            }
        }

        public HBRUSH SelectBrush(HBRUSH brush)
        {
            return (HBRUSH)SelectObject(brush);
        }

        public bool FillRect(ref RECT rect, HBRUSH brush)
        {
            return FillRect(this, ref rect, brush);
        }

        public bool FillRect(ref RECT rect, COLOR color)
        {
            return FillRect(this, ref rect, (int)color + 1);
        }

        #region PInvoke
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteDC(HDC hdc);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        private static extern HDC _CreateCompatibleDC(HDC hdc);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        private static extern HDC _CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        private static extern HGDIOBJ SelectObject(HDC hdc, HGDIOBJ hgdiobj);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FillRect(HDC hDC, ref RECT lprc, HBRUSH hbr);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FillRect(HDC hDC, ref RECT lprc, int color);
        #endregion
    }
}
