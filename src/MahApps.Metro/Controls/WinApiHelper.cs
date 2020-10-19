// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using ControlzEx.Standard;

namespace MahApps.Metro.Controls
{
    public static class WinApiHelper
    {
        /// <summary>
        /// Get the working area size of the monitor from where the visual stays.
        /// </summary>
        /// <param name="visual">The visual element to get the monitor information.</param>
        /// <returns>The working area size of the monitor.</returns>
#pragma warning disable 618
        public static Size GetMonitorWorkSize(this Visual visual)
        {
            if (visual is null == false
                && PresentationSource.FromVisual(visual) is HwndSource source
                && source.IsDisposed == false
                && source.RootVisual is null == false
                && source.Handle != IntPtr.Zero)
            {
                // Try to get the monitor from where the owner stays and use the working area for window size properties
                var monitor = NativeMethods.MonitorFromWindow(source.Handle, MonitorOptions.MONITOR_DEFAULTTONEAREST);
                if (monitor != IntPtr.Zero)
                {
                    var monitorInfo = NativeMethods.GetMonitorInfoW(monitor);
                    return new Size(monitorInfo.rcWork.Width, monitorInfo.rcWork.Height);
                }
            }

            return default;
        }
#pragma warning restore 618

        /// <summary> Gets the text associated with the given window handle. </summary>
        /// <param name="window"> The window to act on. </param>
        /// <returns> The window text. </returns>
#pragma warning disable 618
        internal static string GetWindowText(this Window window)
        {
            if (window is null == false
                && PresentationSource.FromVisual(window) is HwndSource source
                && source.IsDisposed == false
                && source.RootVisual is null == false
                && source.Handle != IntPtr.Zero)
            {
                var builder = new StringBuilder(512);
                NativeMethods.GetWindowText(source.Handle, builder, builder.Capacity);
                return builder.ToString();
            }

            return default;
        }

        /// <summary> Gets the text associated with the given window handle. </summary>
        /// <param name="window"> The window to act on. </param>
        /// <returns> The window text. </returns>
        internal static Rect GetWindowBoundingRectangle(this Window window)
        {
            Rect bounds = new Rect(0, 0, 0, 0);

            if (window is null == false
                && PresentationSource.FromVisual(window) is HwndSource source
                && source.IsDisposed == false
                && source.RootVisual is null == false
                && source.Handle != IntPtr.Zero)
            {
                RECT rc = new RECT(0, 0, 0, 0);

                try
                {
                    rc = NativeMethods.GetWindowRect(source.Handle);
                }
                // Allow empty catch statements.
#pragma warning disable 56502
                catch (Win32Exception)
                {
                }
                // Disallow empty catch statements.
#pragma warning restore 56502

                bounds = new Rect(rc.Left, rc.Top, rc.Width, rc.Height);
            }

            return bounds;
        }
#pragma warning restore 618
    }
}