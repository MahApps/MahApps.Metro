// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace MahApps.Metro.Tests.TestHelpers
{
    /// <summary>
    /// The smallest window handle a window can host: a plain static control. It stands in for a
    /// WindowsFormsHost or a WebBrowser, which are the same thing to the window around them.
    /// </summary>
    public class TestHwndHost : HwndHost
    {
        private const int WS_CHILD = 0x40000000;
        private const int WS_VISIBLE = 0x10000000;

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr CreateWindowExW(int exStyle, string className, string windowName, int style, int x, int y, int width, int height, IntPtr parent, IntPtr menu, IntPtr instance, IntPtr param);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyWindow(IntPtr hwnd);

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            var handle = CreateWindowExW(0, "STATIC", string.Empty, WS_CHILD | WS_VISIBLE, 0, 0, 20, 20, hwndParent.Handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            return new HandleRef(this, handle);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            DestroyWindow(hwnd.Handle);
        }
    }
}
