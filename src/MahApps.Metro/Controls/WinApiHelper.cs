// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
            // Try to get the monitor from where the owner stays and use the working area for window size properties
            if (visual != null && PresentationSource.FromVisual(visual) is HwndSource source)
            {
                var monitor = NativeMethods.MonitorFromWindow(source.Handle, MonitorOptions.MONITOR_DEFAULTTONEAREST);
                if (monitor != IntPtr.Zero)
                {
                    MONITORINFO monitorInfo = NativeMethods.GetMonitorInfoW(monitor);
                    var rcWorkArea = monitorInfo.rcWork;

                    return new Size(rcWorkArea.Width, rcWorkArea.Height);
                }
            }

            return default;
        }
#pragma warning restore 618
    }
}