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

        internal static Standard.POINT GetPhysicalCursorPos()
        {
            try
            {
                return Standard.NativeMethods.GetPhysicalCursorPos();
            }
            catch (Exception exception)
            {
                throw new MahAppsException("Uups, this should not happen! Sorry for this exception! Is this maybe happend on a virtual machine or via remote desktop? Please let us know, thx.", exception);
            }
        }
    }
}