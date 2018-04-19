#pragma warning disable 618
namespace MahApps.Metro.Controls
{
    using System;
    using ControlzEx.Standard;

    public static class WinApiHelper
    {
        /// <summary>
        /// Gets the relative mouse position to the given handle in client coordinates.
        /// </summary>
        /// <param name="hWnd">The handle for this method.</param>
        public static System.Windows.Point GetRelativeMousePosition(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                return new System.Windows.Point();
            }
            var point = WinApiHelper.GetPhysicalCursorPos();
            NativeMethods.ScreenToClient(hWnd, ref point);
            return new System.Windows.Point(point.X, point.Y);
        }

        /// <summary>
        /// Try to get the relative mouse position to the given handle in client coordinates.
        /// </summary>
        /// <param name="hWnd">The handle for this method.</param>
        /// <param name="point">The relative mouse position to the given handle.</param>
        public static bool TryGetRelativeMousePosition(IntPtr hWnd, out System.Windows.Point point)
        {
            POINT pt = new POINT();
            var returnValue = hWnd != IntPtr.Zero && NativeMethods.TryGetPhysicalCursorPos(out pt);
            if (returnValue)
            {
                NativeMethods.ScreenToClient(hWnd, ref pt);
                point = new System.Windows.Point(pt.X, pt.Y);
            }
            else
            {
                point = new System.Windows.Point();
            }
            return returnValue;
        }

        internal static POINT GetPhysicalCursorPos()
        {
            try
            {
                // Sometimes Win32 will fail this call, such as if you are
                // not running in the interactive desktop. For example,
                // a secure screen saver may be running.
                return NativeMethods.GetPhysicalCursorPos();
            }
            catch (Exception exception)
            {
                throw new MahAppsException("Uups, this should not happen! Sorry for this exception! Is this maybe happend on a virtual machine or via remote desktop? Please let us know, thx.", exception);
            }
        }
    }
}