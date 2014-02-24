using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interop;
using System.Windows;
using Microsoft.DwayneNeed.Interop;
using System.Diagnostics;
using Microsoft.DwayneNeed.Win32.User32;
using Microsoft.DwayneNeed.Win32;

namespace Microsoft.DwayneNeed.Extensions
{
    public static class HwndSourceExtensions
    {
        public static Size GetClientSize(this HwndSource hwndSource)
        {
            HWND hwnd = new HWND(hwndSource.Handle);

            RECT rcClient = new RECT();
            NativeMethods.GetClientRect(hwnd, ref rcClient);

            // Client rect should always have (0,0) as the upper-left corner.
            Debug.Assert(rcClient.left == 0);
            Debug.Assert(rcClient.left == 0);

            // Convert from pixels into DIPs.
            Vector size = new Vector(rcClient.right, rcClient.bottom);
            size = hwndSource.CompositionTarget.TransformFromDevice.Transform(size);

            return new Size(size.X, size.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwndSource"></param>
        /// <returns></returns>
        public static Rect GetWindowRect(this HwndSource hwndSource)
        {
            HWND hwnd = new HWND(hwndSource.Handle);

            RECT rcWindow = new RECT();
            NativeMethods.GetWindowRect(hwnd, ref rcWindow);

            // Transform from pixels into DIPs.
            Point position = new Point(rcWindow.left, rcWindow.top);
            Vector size = new Vector(rcWindow.right - rcWindow.left, rcWindow.bottom - rcWindow.top);
            position = hwndSource.CompositionTarget.TransformFromDevice.Transform(position);
            size = hwndSource.CompositionTarget.TransformFromDevice.Transform(size);

            return new Rect(position, size);
        }

        /// <summary>
        ///     Transform a point from "screen" coordinate space into the
        ///     "client" coordinate space of the window.
        /// </summary>
        public static Point TransformScreenToClient(this HwndSource hwndSource, Point point)
        {
            HWND hwnd = new HWND(hwndSource.Handle);

            POINT pt = new POINT();
            pt.x = (int)point.X;
            pt.y = (int)point.Y;

            NativeMethods.ScreenToClient(hwnd, ref pt);

            return new Point(pt.x, pt.y);
        }

        /// <summary>
        ///     Transform a rectangle from "screen" coordinate space into the
        ///     "client" coordinate space of the window.
        /// </summary>
        public static Rect TransformScreenToClient(this HwndSource hwndSource, Rect rect)
        {
            HWND hwnd = new HWND(hwndSource.Handle);

            POINT ptUpperLeft = new POINT();
            ptUpperLeft.x = (int)rect.Left;
            ptUpperLeft.y = (int)rect.Top;

            NativeMethods.ScreenToClient(hwnd, ref ptUpperLeft);

            POINT ptLowerRight = new POINT();
            ptLowerRight.x = (int)rect.Right;
            ptLowerRight.y = (int)rect.Bottom;

            NativeMethods.ScreenToClient(hwnd, ref ptLowerRight);

            return new Rect(ptUpperLeft.x, ptUpperLeft.y, ptLowerRight.x - ptUpperLeft.x, ptLowerRight.y - ptUpperLeft.y);
        }

        /// <summary>
        ///     Transform a point from "client" coordinate space of a window
        ///     into the "screen" coordinate space.
        /// </summary>
        public static Point TransformClientToScreen(this HwndSource hwndSource, Point point)
        {
            HWND hwnd = new HWND(hwndSource.Handle);

            POINT pt = new POINT();
            pt.x = (int)point.X;
            pt.y = (int)point.Y;

            NativeMethods.ClientToScreen(hwnd, ref pt);

            return new Point(pt.x, pt.y);
        }

        /// <summary>
        ///     Transform a rectangle from the "client" coordinate space of the
        ///     window into the "screen" coordinate space.
        /// </summary>
        public static Rect TransformClientToScreen(this HwndSource hwndSource, Rect rect)
        {
            HWND hwnd = new HWND(hwndSource.Handle);

            POINT ptUpperLeft = new POINT();
            ptUpperLeft.x = (int)rect.Left;
            ptUpperLeft.y = (int)rect.Top;

            NativeMethods.ClientToScreen(hwnd, ref ptUpperLeft);

            POINT ptLowerRight = new POINT();
            ptLowerRight.x = (int)rect.Right;
            ptLowerRight.y = (int)rect.Bottom;

            NativeMethods.ClientToScreen(hwnd, ref ptLowerRight);

            return new Rect(ptUpperLeft.x, ptUpperLeft.y, ptLowerRight.x - ptUpperLeft.x, ptLowerRight.y - ptUpperLeft.y);
        }
    }
}
