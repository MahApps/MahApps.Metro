using System;

namespace MahApps.Metro.Controls
{
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
            Standard.NativeMethods.ScreenToClient(hWnd, ref point);
            return new System.Windows.Point(point.x, point.y);
        }

        /// <summary>
        /// Try to get the relative mouse position to the given handle in client coordinates.
        /// </summary>
        /// <param name="hWnd">The handle for this method.</param>
        public static bool TryGetRelativeMousePosition(IntPtr hWnd, out System.Windows.Point point)
        {
            Standard.POINT pt = new Standard.POINT();
            var returnValue = hWnd != IntPtr.Zero && Standard.NativeMethods.TryGetPhysicalCursorPos(out pt);
            if (returnValue)
            {
                Standard.NativeMethods.ScreenToClient(hWnd, ref pt);
                point = new System.Windows.Point(pt.x, pt.y);
            }
            else
            {
                point = new System.Windows.Point();
            }
            return returnValue;
        }

        internal static Standard.POINT GetPhysicalCursorPos()
        {
            try
            {
                // Sometimes Win32 will fail this call, such as if you are
                // not running in the interactive desktop. For example,
                // a secure screen saver may be running.
                return Standard.NativeMethods.GetPhysicalCursorPos();
            }
            catch (Exception exception)
            {
                throw new MahAppsException("Uups, this should not happen! Sorry for this exception! Is this maybe happend on a virtual machine or via remote desktop? Please let us know, thx.", exception);
            }
        }
    }
}