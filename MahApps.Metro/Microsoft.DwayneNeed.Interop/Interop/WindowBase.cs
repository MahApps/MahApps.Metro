using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows;
using Microsoft.DwayneNeed.Win32.User32;
using Microsoft.DwayneNeed.Win32.ComCtl32;
using Microsoft.DwayneNeed.Win32;

namespace Microsoft.DwayneNeed.Interop
{
    public class WindowBase : IDisposable
    {
        internal WindowBase()
        {
            _wndProc = new WNDPROC(WndProc);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                StrongHWND strongHwnd = _hwnd as StrongHWND;

                if (strongHwnd != null)
                {
                    // Replace the StrongHWND reference with a regular "weak"
                    // HWND reference so that messages processed during
                    // disposing do not have to deal with a partially disposed
                    // SafeHandle.
                    _hwnd = new HWND(strongHwnd.DangerousGetHandle());

                    strongHwnd.Dispose();

                    // All done, replace the "weak" HWND reference with null.
                    _hwnd = null;
                }
            }
        }

        public HWND Handle
        {
            get
            {
                return _hwnd;
            }
        }

        protected virtual void Initialize()
        {
        }

        protected virtual IntPtr OnMessage(WM message, IntPtr wParam, IntPtr lParam)
        {
            return NativeMethods.DefWindowProc(_hwnd, message, wParam, lParam);
        }

        public void SetLayeredWindowAttributes(byte? alpha, Color? colorKey = null)
        {
            LWA flags = 0;
            byte bAlpha = 0;
            uint crKey = 0;

            if (alpha != null)
            {
                bAlpha = alpha.Value;
                flags |= LWA.ALPHA;
            }

            if (colorKey != null)
            {
                uint r = (uint) colorKey.Value.R;
                uint g = (uint) colorKey.Value.G;
                uint b = (uint) colorKey.Value.B;

                crKey = r + (g << 8) + (b << 16);
                flags |= LWA.COLORKEY;
            }

            NativeMethods.SetLayeredWindowAttributes(_hwnd, crKey, bAlpha, flags);
        }

        public bool Hide()
        {
            return NativeMethods.ShowWindow(_hwnd, SW.HIDE);
        }

        public bool Restore()
        {
            return NativeMethods.ShowWindow(_hwnd, SW.RESTORE);
        }

        public bool Minimize(bool force)
        {
            return NativeMethods.ShowWindow(_hwnd, force ? SW.FORCEMINIMIZE : SW.MINIMIZE);
        }

        public bool Maximize()
        {
            return NativeMethods.ShowWindow(_hwnd, SW.MAXIMIZE);
        }

        public bool Show(WindowShowState showState = WindowShowState.Current, bool activate = true)
        {
            switch (showState)
            {
                case WindowShowState.Default:
                    return NativeMethods.ShowWindow(_hwnd, SW.SHOWDEFAULT);
                
                case WindowShowState.Current:
                    return NativeMethods.ShowWindow(_hwnd, activate ? SW.SHOW : SW.SHOWNA);

                case WindowShowState.Normal:
                    return NativeMethods.ShowWindow(_hwnd, activate ? SW.SHOWNORMAL : SW.SHOWNOACTIVATE);

                case WindowShowState.Minimized:
                    return NativeMethods.ShowWindow(_hwnd, activate ? SW.SHOWMINIMIZED : SW.SHOWMINNOACTIVE);

                case WindowShowState.Maximized:
                    return NativeMethods.ShowWindow(_hwnd, SW.SHOWMAXIMIZED);

                default:
                    return false;
            }
        }

        /// <summary>
        ///     Called from WindowClass.CreateWindow to intialize this instance
        ///     when the HWND has been created.
        /// </summary>
        /// <param name="hwnd">
        ///     The HWND that was created.
        /// </param>
        /// <param name="param">
        ///     The creation parameter that was passsed to
        ///     WindowClass.CreateWindow.
        /// </param>
        internal IntPtr InitializeFromFirstMessage(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam)
        {
            _hwnd = new HWND(hwnd);

            // Replace the window proceedure for this window instance.
            IntPtr wndProc = Marshal.GetFunctionPointerForDelegate(_wndProc);
            NativeMethods.SetWindowLongPtr(_hwnd, GWL.WNDPROC, wndProc);

            // Give the window a chance to initialize.
            Initialize();

            // Manually invoke the window proceedure for this message.
            return OnMessage((WM) message, wParam, lParam);
        }

        internal void TransferHandleOwnership(StrongHWND hwnd)
        {
            Debug.Assert(hwnd == _hwnd); // equivalency, not reference equals
            _hwnd = hwnd;
        }

        internal IntPtr WndProc(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam)
        {
            Debug.Assert(hwnd == _hwnd.DangerousGetHandle());

            return OnMessage((WM)message, wParam, lParam);
        }

        private HWND _hwnd; // Will be a StrongHWND after TransferHandleOwnership
        private WNDPROC _wndProc;
    }
}
