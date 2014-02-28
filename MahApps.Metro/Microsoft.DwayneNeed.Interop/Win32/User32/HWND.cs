using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Microsoft.DwayneNeed.Win32.User32
{
    /// <summary>
    ///     A SafeHandle representing an HWND.
    /// </summary>
    /// <remarks>
    ///     HWNDs have very loose ownership semantics.  Unlike normal handles,
    ///     there is no "CloseHandle" API for HWNDs.  There are APIs like
    ///     CloseWindow or DestroyWindow, but these actually affect the window,
    ///     not just your handle to the window.  This SafeHandle type does not
    ///     actually do anything to release the handle in the finalizer, it
    ///     simply provides type safety to the PInvoke signatures.
    ///    
    ///     The StrongHWND SafeHandle will actually destroy the HWND when it
    ///     is disposed or finalized.
    ///    
    ///     Because of this loose ownership semantic, the same HWND value can
    ///     be returned from multiple APIs and can be directly compared.  Since
    ///     SafeHandles are actually reference types, we have to override all
    ///     of the comparison methods and operators.  We also support equality
    ///     between null and HWND(IntPtr.Zero).
    /// </remarks>
    public class HWND : SafeHandle
    {
        static HWND()
        {
            NULL = new HWND(IntPtr.Zero);
            BROADCAST = new HWND(new IntPtr(0xffff));
            MESSAGE = new HWND(new IntPtr(-3));
            DESKTOP = new HWND(new IntPtr(0));
            TOP = new HWND(new IntPtr(0));
            BOTTOM = new HWND(new IntPtr(1));
            TOPMOST = new HWND(new IntPtr(-1));
            NOTOPMOST = new HWND(new IntPtr(-2));
        }

        /// <summary>
        ///     Public constructor to create an empty HWND SafeHandle instance.
        /// </summary>
        /// <remarks>
        ///     This constructor is used by the marshaller.  The handle value
        ///     is then set directly.
        /// </remarks>
        public HWND()
            : base(invalidHandleValue: IntPtr.Zero, ownsHandle: false)
        {
        }

        /// <summary>
        ///     Public constructor to create an HWND SafeHandle instance for
        ///     an existing handle.
        /// </summary>
        public HWND(IntPtr hwnd) : this()
        {
            SetHandle(hwnd);
        }

        /// <summary>
        ///     Constructor for derived classes to control whether or not the
        ///     handle is owned.
        /// </summary>
        protected HWND(bool ownsHandle)
            : base(invalidHandleValue: IntPtr.Zero, ownsHandle: ownsHandle)
        {
        }

        /// <summary>
        ///     Constructor for derived classes to specify a handle and to
        ///     control whether or not the handle is owned.
        /// </summary>
        protected HWND(IntPtr hwnd, bool ownsHandle)
            : base(invalidHandleValue: IntPtr.Zero, ownsHandle: ownsHandle)
        {
            SetHandle(hwnd);
        }

        public static HWND NULL {get; private set;}
        public static HWND BROADCAST { get; private set; }
        public static HWND MESSAGE { get; private set; }
        public static HWND DESKTOP { get; private set; }
        public static HWND TOP { get; private set; }
        public static HWND BOTTOM { get; private set; }
        public static HWND TOPMOST { get; private set; }
        public static HWND NOTOPMOST { get; private set; }

        public override bool IsInvalid
        {
            get { return !IsWindow(handle); }
        }

        protected override bool ReleaseHandle()
        {
            // This should never get called, since we specify ownsHandle:false
            // when constructed.
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
            {
                return handle == IntPtr.Zero;
            }
            else
            {
                HWND other = obj as HWND;
                return other != null && Equals(other);
            }
        }

        public bool Equals(HWND other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return handle == IntPtr.Zero;
            }
            else
            {
                return other.handle == handle;
            }
        }

        public override int GetHashCode()
        {
            return handle.GetHashCode();
        }

        public static bool operator ==(HWND lvalue, HWND rvalue)
        {
            if (Object.ReferenceEquals(lvalue, null))
            {
                return Object.ReferenceEquals(rvalue, null) || rvalue.handle == IntPtr.Zero;
            }
            else if (Object.ReferenceEquals(rvalue, null))
            {
                return lvalue.handle == IntPtr.Zero;
            }
            else
            {
                return lvalue.handle == rvalue.handle;
            }
        }

        public static bool operator !=(HWND lvalue, HWND rvalue)
        {
            return !(lvalue == rvalue);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool IsWindow(IntPtr hwnd);
    }
}
