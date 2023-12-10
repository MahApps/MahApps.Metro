// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;

namespace MahApps.Metro.Native
{
    internal static class WinApiHelper
    {
        private static SafeHandle? user32;

        /// <summary>
        /// Get caption for the given id from the user32.dll
        /// </summary>
        /// <param name="id">The id for the caption.</param>
        /// <returns>The caption from the id.</returns>
        public static unsafe string GetCaption(uint id)
        {
            if (user32 is null)
            {
                user32 = PInvoke.LoadLibrary(Path.Combine(Environment.SystemDirectory, "User32.dll"));
            }

            var chars = new char[256];

            fixed (char* pchars = chars)
            {
                //PWSTR str = new PWSTR()
                if (PInvoke.LoadString(user32, id, pchars, chars.Length) == 0)
                {
                    return string.Format("String with id '{0}' could not be found.", id);
                }
#pragma warning disable CA1307 // Specify StringComparison for clarity
                return new string(chars).Replace("&", string.Empty);
#pragma warning restore CA1307 // Specify StringComparison for clarity
            }
        }

        /// <summary>
        /// Get the working area size of the monitor from where the visual stays.
        /// </summary>
        /// <param name="visual">The visual element to get the monitor information.</param>
        /// <returns>The working area size of the monitor.</returns>
        public static unsafe Size GetMonitorWorkSize(this Visual? visual)
        {
            if (visual is null == false
                && PresentationSource.FromVisual(visual) is HwndSource source
                && source.IsDisposed == false
                && source.RootVisual is null == false
                && source.Handle != IntPtr.Zero)
            {
                // Try to get the monitor from where the owner stays and use the working area for window size properties
                var monitor = PInvoke.MonitorFromWindow(new HWND(source.Handle), MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
                if (monitor != IntPtr.Zero)
                {
                    var monitorInfo = new MONITORINFO
                                      {
                                          cbSize = (uint)Marshal.SizeOf<MONITORINFO>()
                                      };
                    PInvoke.GetMonitorInfo(monitor, &monitorInfo);
                    return new Size(monitorInfo.rcWork.right - monitorInfo.rcWork.left, monitorInfo.rcWork.bottom - monitorInfo.rcWork.top);
                }
            }

            return default;
        }

        /// <summary>
        /// Sets a window placement to a window.
        /// </summary>
        /// <param name="window">The window which should get the window placement.</param>
        /// <param name="wp">The window placement for the window.</param>
        public static unsafe void SetWindowPlacement(Window? window, WINDOWPLACEMENT? wp)
        {
            if (window is null)
            {
                return;
            }

            // Get the current DPI scale factor
            var dpiScale = VisualTreeHelper.GetDpi(window);

            var x = CalcIntValue(wp?.rcNormalPosition.left, window.Left);
            var y = CalcIntValue(wp?.rcNormalPosition.top, window.Top);
            var width = CalcIntValue(wp?.rcNormalPosition.GetWidth() * dpiScale.DpiScaleX, window.ActualWidth);
            var height = CalcIntValue(wp?.rcNormalPosition.GetHeight() * dpiScale.DpiScaleY, window.ActualHeight);

            var placement = new WINDOWPLACEMENT
                            {
                                length = (uint)Marshal.SizeOf<WINDOWPLACEMENT>(),
                                showCmd = (wp is null || wp.Value.showCmd == SHOW_WINDOW_CMD.SW_SHOWMINIMIZED ? SHOW_WINDOW_CMD.SW_SHOWNORMAL : wp.Value.showCmd),
                                rcNormalPosition = new RECT { left = x, top = y, right = x + width, bottom = y + height }
                            };

            if (wp is not null)
            {
                placement.ptMinPosition = new System.Drawing.Point(wp.Value.ptMinPosition.X, wp.Value.ptMinPosition.Y);
                placement.ptMaxPosition = new System.Drawing.Point(wp.Value.ptMaxPosition.X, wp.Value.ptMaxPosition.Y);
            }

            var hWnd = new WindowInteropHelper(window).EnsureHandle();
            if (PInvoke.SetWindowPlacement(new HWND(hWnd), &placement) == false)
            {
                Trace.TraceWarning($"{window}: The window placement {wp} could not be (SetWindowPlacement)!");
            }
        }

        private static int CalcIntValue(double? value, double fallback)
        {
            if (value is null)
            {
                return (int)fallback;
            }

            var d = (double)value;
            if (!double.IsNaN(d) && d > int.MinValue && d < int.MaxValue)
            {
                return (int)d;
            }

            return (int)fallback;
        }
    }
}